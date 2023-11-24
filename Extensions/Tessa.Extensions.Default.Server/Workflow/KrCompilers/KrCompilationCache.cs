using System;
using System.Threading;
using Tessa.Cards.Caching;
using Tessa.Platform;
using Tessa.Platform.Collections;

namespace Tessa.Extensions.Default.Server.Workflow.KrCompilers
{
    public sealed class KrCompilationCache : IKrCompilationCache
    {
        #region constants

        private const string CacheKey = "KrCompilationCache";

        #endregion

        #region fields

        private readonly ICardCache cardCache;
        private readonly IKrProcessCache processCache;
        private readonly IKrCompiler krCompiler;
        private readonly IKrCompilationResultStorage сompilationResultStorage;


        private readonly ReaderWriterLockSlim cacheLock =
            new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);

        #endregion

        #region constructor

        public KrCompilationCache(
            ICardCache cardCache,
            IKrProcessCache processCache,
            IKrCompiler krCompiler,
            IKrCompilationResultStorage сompilationResultStorage)
        {
            this.cardCache = cardCache;
            this.processCache = processCache;
            this.krCompiler = krCompiler;
            this.сompilationResultStorage = сompilationResultStorage;
        }

        #endregion

        #region private

        private static bool TessaVersionIsEqual(
            IKrCompilationResult res) =>
            res.Result.BuildDate == BuildInfo.Date && res.Result.BuildVersion == BuildInfo.Version;

        private IKrCompilationResult Compile(string key)
        {
            using (new WriterLockSlimWrapper(this.cacheLock))
            {
                var res = this.cardCache.Settings.TryGetAsync<IKrCompilationResult>(CacheKey).GetAwaiter().GetResult(); // TODO async
                if (res != null)
                {
                    return res;
                }

                res = this.сompilationResultStorage.GetCompilationResult(Guid.Empty);
                if (res != null)
                {
                    if (TessaVersionIsEqual(res))
                    {
                        return res;
                    }
                    this.сompilationResultStorage.DeleteCompilationResult(Guid.Empty);
                }

                var krCompileContext = new KrCompilationContext();
                krCompileContext.Stages.AddRange(this.processCache.GetAllRuntimeStages().Values);
                krCompileContext.StageTemplates.AddRange(this.processCache.GetAllStageTemplates().Values);
                krCompileContext.CommonMethods.AddRange(this.processCache.GetAllCommonMethods());
                krCompileContext.StageGroups.AddRange(this.processCache.GetAllStageGroups().Values);
                krCompileContext.SecondaryProcesses.AddRange(this.processCache.GetAllPureProcesses().Values);
                krCompileContext.SecondaryProcesses.AddRange(this.processCache.GetAllButtons().Values);
                krCompileContext.SecondaryProcesses.AddRange(this.processCache.GetAllActions().Values);

                var result = this.krCompiler.Compile(krCompileContext);
                this.сompilationResultStorage.Upsert(Guid.Empty, result, true);
                return result;
            }
        }

        #endregion

        #region implementation

        /// <summary>
        /// Выполняет сборку, если в кэше нет сборки или сборка invalidate
        /// Если в кэше есть актуальная сборка, возвращается null
        /// </summary>
        /// <returns>Результат сборки или null</returns>
        public IKrCompilationResult Build()
        {
            if(this.cardCache.Settings.ContainsAsync(CacheKey).GetAwaiter().GetResult()) // TODO async
            {
                return null;
            }
            return this.cardCache.Settings.GetAsync(CacheKey, this.Compile).GetAwaiter().GetResult(); // TODO async
        }

        /// <summary>
        /// Явно сбрасывается кэш сборки и выполняется пересборка
        /// Кэш KrStageTenolateCache не сбрасывается
        /// </summary>
        /// <returns>Результат компиляции</returns>
        public IKrCompilationResult Rebuild()
        {
            this.Invalidate();
            return this.cardCache.Settings.GetAsync(CacheKey, this.Compile).GetAwaiter().GetResult(); // TODO async
        }

        /// <summary>
        /// Получение значения из кэша
        /// </summary>
        /// <returns>Результат сборки</returns>
        public IKrCompilationResult Get()
        {
            return this.cardCache.Settings.GetAsync(CacheKey, this.Compile).GetAwaiter().GetResult(); // TODO async
        }

        /// <summary>
        /// Сброс значения кэша
        /// </summary>
        public void Invalidate()
        {
            using (new ReaderLockSlimWrapper(this.cacheLock))
            {
                this.сompilationResultStorage.DeleteCompilationResult(Guid.Empty);
                this.cardCache.Settings.InvalidateAsync(CacheKey).GetAwaiter().GetResult(); // TODO async
            }
        }

        #endregion
    }
}
