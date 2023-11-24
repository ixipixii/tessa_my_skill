using System;
using System.Collections.Generic;
using Tessa.Cards;
using Tessa.Extensions.Default.Server.Workflow.KrObjectModel;
using Tessa.Extensions.Default.Shared.Workflow.KrProcess;
using Tessa.Platform.Collections;
using Tessa.Platform.Scopes;
using Tessa.Platform.Storage;
using Tessa.Platform.Validation;

namespace Tessa.Extensions.Default.Server.Workflow.KrProcess.Scope
{
    /// <summary>
    /// Контекст Kr расширений на сохранение
    /// </summary>
    public sealed class KrScopeContext : IDisposable
    {
        #region constructor

        private KrScopeContext()
        {
        }

        #endregion

        #region public

        public IValidationResultBuilder ValidationResult { get; } = new ValidationResultBuilder();

        public HashSet<Guid, ProcessHolder> ProcessHolders { get; } =
            new HashSet<Guid, ProcessHolder>(p => p.ProcessHolderID);

        public HashSet<Guid, Card> MainKrSatellites { get; } =
            new HashSet<Guid, Card>(c => c.GetApprovalInfoSection().Fields.TryGet<Guid>(KrConstants.KrProcessCommonInfo.MainCardID));

        public Dictionary<Guid, Card> SecondaryKrSatellites { get; } = new Dictionary<Guid, Card>();

        public Dictionary<Guid, Card> Cards { get; } = new Dictionary<Guid, Card>();
        
        public Dictionary<Guid, ICardFileContainer> CardFileContainers { get; } = new Dictionary<Guid, ICardFileContainer>();

        public HashSet<Guid> CardsWithTaskHistory { get; } = new HashSet<Guid>();

        public HashSet<Guid> ForceIncrementCardVersion { get; } = new HashSet<Guid>();

        public Dictionary<string, object> Info { get; } = new Dictionary<string,object>();

        public bool IsDisposed { get; private set; } = false;

        #endregion

        #region internal

        internal HashSet<Guid> Locks { get; } = new HashSet<Guid>();

        internal Dictionary<Guid, Guid> LockKeys { get; } = new Dictionary<Guid, Guid>();

        internal Stack<KrScopeLevel> LevelStack { get; } = new Stack<KrScopeLevel>();

        #endregion

        #region static

        /// <summary>
        /// Текущий контекст <see cref="KrScopeContext"/>.
        /// </summary>
        public static KrScopeContext Current => InheritableRetainingScope<KrScopeContext>.Value;

        /// <summary>
        /// Признак того, что текущий код выполняется внутри операции с контекстом <see cref="KrScopeContext"/>,
        /// а свойство <see cref="Current"/> ссылается на действительный контекст.
        /// </summary>
        /// <remarks>
        /// Если текущее свойство возвращает <c>false</c>, то свойство <see cref="Current"/>
        /// возвращает ссылку на пустой контекст.
        /// </remarks>
        public static bool HasCurrent => Current != null;

        #endregion

        #region Lifecycle

        public static IInheritableScopeInstance<KrScopeContext> Create() =>
            InheritableRetainingScope<KrScopeContext>.Create(() => new KrScopeContext());

        public void Dispose()
        {
            this.IsDisposed = true;
        }

        #endregion
    }
}