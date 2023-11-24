using System;
using System.Linq;
using System.Collections.Generic;
using Tessa.Cards;
using Tessa.Cards.Caching;
using Tessa.Platform.Data;
using System.Threading;
using System.Threading.Tasks;
using Tessa.Platform;
using Tessa.Files;
using Tessa.Platform.Storage;
using Tessa.Platform.Conditions;
using Unity;

namespace Tessa.Extensions.Default.Server.Files.VirtualFiles
{
    public sealed class KrVirtualFileCache :
        IKrVirtualFileCache,
        IDisposable
    {
        #region Nested Types

        private class Cache
        {
            private readonly Dictionary<Guid, IKrVirtualFile> itemsByKey;

            public Cache(IKrVirtualFile[] items)
            {
                this.Items = items;
                this.itemsByKey = items.ToDictionary(x => x.ID, x => x);
            }

            public IKrVirtualFile[] Items { get; }

            public IKrVirtualFile Get(Guid id) => this.itemsByKey.TryGetValue(id, out IKrVirtualFile result) ? result : null;
        }

        #endregion

        #region Fields

        private readonly ICardCache cache;
        private readonly IDbScope dbScope;
        private readonly AsyncLock asyncLock = new AsyncLock();

        private const string CacheKey = CardHelper.SystemKeyPrefix + nameof(KrVirtualFileCache);
        private const string TypesCackeKey = CacheKey + "Types";

        #endregion

        #region Constructors

        public KrVirtualFileCache(
            ICardCache cache,
            IDbScope dbScope,
            [OptionalDependency] IUnityDisposableContainer container = null)
        {
            this.cache = cache ?? throw new ArgumentNullException(nameof(cache));
            this.dbScope = dbScope ?? throw new ArgumentNullException(nameof(dbScope));

            container?.Register(this);
        }

        #endregion

        #region IKrVirtualFileCache Implementation

        public async ValueTask<IKrVirtualFile> GetAsync(Guid virtualFileID, CancellationToken cancellationToken = default) =>
            (await cache.Settings.GetAsync(CacheKey, InitCacheAsync, cancellationToken))
            .Get(virtualFileID);

        public async ValueTask<IKrVirtualFile[]> GetAllAsync(CancellationToken cancellationToken = default) =>
            (await cache.Settings.GetAsync(CacheKey, InitCacheAsync, cancellationToken))
            .Items;

        public ValueTask<Guid[]> GetAllowedTypesAsync(CancellationToken cancellationToken = default) =>
            cache.Settings.GetAsync(TypesCackeKey, InitTypesAsync, cancellationToken);

        public async Task InvalidateAsync(CancellationToken cancellationToken = default)
        {
            using (await this.asyncLock.EnterAsync(cancellationToken))
            {
                await cache.Settings.InvalidateAsync(CacheKey, cancellationToken);
                await cache.Settings.InvalidateAsync(TypesCackeKey, cancellationToken);
            }
        }

        #endregion

        #region Private Methods

        private async Task<Guid[]> InitTypesAsync(string arg, CancellationToken cancellationToken = default)
        {
            using (await this.asyncLock.EnterAsync(cancellationToken))
            await using (dbScope.CreateNew())
            {
                var db = dbScope.Db;
                var builder = dbScope.BuilderFactory;

                return (await db.SetCommand(
                        builder
                            .SelectDistinct()
                            .C("vft", "TypeID") // 0
                            .From("KrVirtualFileCardTypes", "vft").NoLock()
                            .Build())
                    .LogCommand()
                    .ExecuteListAsync<Guid>(cancellationToken)).ToArray();
            }
        }

        private async Task<Cache> InitCacheAsync(string arg, CancellationToken cancellationToken = default)
        {
            using (await this.asyncLock.EnterAsync(cancellationToken))
            await using (dbScope.CreateNew())
            {
                var db = dbScope.Db;
                var builder = dbScope.BuilderFactory;

                db.SetCommand(
                        builder
                            .Select()
                            .C("vf", "ID", "FileVersionID", "FileTemplateID", "FileName", "FileCategoryID", "FileCategoryName", "Conditions", "InitializationScenario") // 0 - 7
                            .C("f", "Name") // 8
                            .C("vfv", "RowID", "FileTemplateID", "FileName") // 9 - 11
                            .C("ff", "Name") // 12
                            .From("KrVirtualFiles", "vf").NoLock()
                            .LeftJoin("KrVirtualFileVersions", "vfv").NoLock()
                            .On().C("vf", "ID").Equals().C("vfv", "ID")
                            .InnerJoin("Files", "f").NoLock()
                            .On().C("f", "ID").Equals().C("vf", "FileTemplateID")
                            .LeftJoin("Files", "ff").NoLock()
                            .On().C("ff", "ID").Equals().C("vfv", "FileTemplateID")
                            .OrderBy("vf", "ID").By("vfv", "Order", SortOrder.Ascending)
                            .Build())
                    .LogCommand();

                var items = new List<IKrVirtualFile>();
                await using (var reader = await db.ExecuteReaderAsync(cancellationToken))
                {
                    var prevFileID = Guid.Empty;
                    IKrVirtualFile prevFile = null;
                    IKrVirtualFileVersion mainVersion = null;
                    while (await reader.ReadAsync(cancellationToken))
                    {
                        var currentFileID = reader.GetValue<Guid>(0);

                        if (currentFileID != prevFileID)
                        {
                            prevFile?.Versions.Add(mainVersion);

                            prevFileID = currentFileID;
                            var templateName = reader.GetValue<string>(8);
                            var fileName = reader.GetValue<string>(3);

                            if (string.IsNullOrEmpty(fileName))
                            {
                                fileName = templateName;
                            }
                            else
                            {
                                fileName += Tessa.Platform.IO.FileHelper.GetExtension(templateName);
                            }

                            var fileCategoryID = reader.GetValue<Guid?>(4);
                            var conditions = reader.GetValue<string>(6);

                            prevFile =
                                new KrVirtualFile
                                {
                                    ID = currentFileID,
                                    Name = fileName,
                                    FileCategory = fileCategoryID.HasValue ? new FileCategory(fileCategoryID, reader.GetValue<string>(5)) : null,
                                    Conditions = string.IsNullOrEmpty(conditions)
                                        ? null
                                        : ConditionSettings.GetFromList(StorageHelper.DeserializeListFromTypedJson(conditions)),
                                    InitializationScenario = reader.GetValue<string>(7),
                                };

                            mainVersion =
                                new KrVirtualFileVersion
                                {
                                    ID = reader.GetValue<Guid>(1),
                                    FileTemplateID = reader.GetValue<Guid>(2),
                                    Name = fileName,
                                };

                            items.Add(prevFile);
                        }

                        var additionalVersionID = reader.GetValue<Guid?>(9);
                        if (additionalVersionID.HasValue)
                        {
                            var templateName = reader.GetValue<string>(12);
                            var fileName = reader.GetValue<string>(11);

                            if (string.IsNullOrEmpty(fileName))
                            {
                                fileName = templateName;
                            }
                            else
                            {
                                fileName += Tessa.Platform.IO.FileHelper.GetExtension(templateName);
                            }

                            if (prevFile != null)
                            {
                                prevFile.Versions.Add(
                                    new KrVirtualFileVersion
                                    {
                                        ID = additionalVersionID.Value,
                                        FileTemplateID = reader.GetValue<Guid>(10),
                                        Name = fileName,
                                    });
                            }
                        }
                    }

                    prevFile?.Versions.Add(mainVersion);
                }

                var result = new Cache(items.ToArray());

                return result;
            }
        }

        #endregion

        #region IDisposable Members

        public void Dispose() => this.asyncLock.Dispose();

        #endregion
    }
}