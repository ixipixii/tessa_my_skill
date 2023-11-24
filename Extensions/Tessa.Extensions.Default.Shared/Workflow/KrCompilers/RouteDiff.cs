using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Tessa.Platform.Storage;

namespace Tessa.Extensions.Default.Shared.Workflow.KrCompilers
{
    /// <summary>
    /// Одно различие между маршрутом согласования до расчета и после.
    /// </summary>
    [Serializable]
    public sealed class RouteDiff : StorageObject
    {
        private RouteDiff(
            RouteDiffAction action,
            Guid elementID,
            string actualName,
            string oldName,
            bool hasPlainChanges,
            bool hidden)
            : this(new Dictionary<string, object>())
        {
            this.Set(nameof(this.Action), action);
            this.Set(nameof(this.RowID), elementID);
            this.Set(nameof(this.ActualName), actualName);
            this.Set(nameof(this.OldName), oldName);
            this.Set(nameof(this.HasPlainChanges), hasPlainChanges);
            this.Set(nameof(this.HiddenStage), hidden);
        }

        public RouteDiff(Dictionary<string, object> storage)
            : base(storage)
        {
        }

        private RouteDiff(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        /// <summary>
        /// Добавление/изменение/удаление.
        /// </summary>
        public RouteDiffAction Action => this.Get<RouteDiffAction>(nameof(this.Action));

        /// <summary>
        /// RowID БД.
        /// </summary>
        public Guid RowID => this.Get<Guid>(nameof(this.RowID));

        /// <summary>
        /// Текущее имя или null, если элемент удален.
        /// </summary>
        public string ActualName => this.Get<string>(nameof(this.ActualName));

        /// <summary>
        /// Предыдущее имя. OldName == ActualName, если имя не поменялось. null, если элемент добавляется.
        /// </summary>
        public string OldName => this.Get<string>(nameof(this.OldName));

        /// <summary>
        /// Есть ли изменения полей.
        /// </summary>
        public bool HasPlainChanges => this.Get<bool>(nameof(this.HasPlainChanges));

        /// <summary>
        /// Этап, в котором произошли изменения, является скрытым
        /// </summary>
        public bool HiddenStage => this.Get<bool>(nameof(this.HiddenStage));

        #region factory methods
        
        /// <summary>
        /// Новый этап.
        /// </summary>
        /// <param name="id">RowID в БД нового этапа</param>
        /// <param name="newName">Название нового этапа.</param>
        /// <param name="hiddenStage">Этап является скрытым</param>
        /// <returns></returns>
        public static RouteDiff NewStage(Guid id, string newName, bool hiddenStage)
        {
            return new RouteDiff(RouteDiffAction.Insert, id, newName, null, true, hiddenStage);
        }

        /// <summary>
        /// Этап изменен.
        /// </summary>
        /// <param name="id">RowID в БД измененного этапа</param>
        /// <param name="actualName">Актуальное название этапа.</param>
        /// <param name="oldName">Предыдущее название этапа или null, если название не поменялось.</param>
        /// <param name="hasPlainChanges">Были ли изменения полей.</param>
        /// <param name="hiddenStage">Этап является скрытым</param>
        /// <returns></returns>
        public static RouteDiff ModifyStage(
            Guid id, 
            string actualName,
            bool hiddenStage,
            string oldName = null, 
            bool hasPlainChanges = true)
        {
            return new RouteDiff(RouteDiffAction.Modify, id, actualName, oldName ?? actualName, hasPlainChanges, hiddenStage);
        }

        /// <summary>
        /// Этап удален.
        /// </summary>
        /// <param name="id">RowID удаленного этапа</param>
        /// <param name="name">Название удаленного этапа.</param>
        /// <param name="hiddenStage">Этап является скрытым</param>
        /// <returns></returns>
        public static RouteDiff DeleteStage(Guid id, string name, bool hiddenStage)
        {
            return new RouteDiff(RouteDiffAction.Delete, id, null, name, false, hiddenStage);
        }

        #endregion
    }
}