using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Tessa.Cards;
using Tessa.Platform.Storage;

namespace Tessa.Extensions.Default.Shared.Workflow.KrProcess
{
    [Serializable]
    public sealed class KrStagePositionInfo: StorageObject
    {
        [NonSerialized]
        private CardRow cardRow;

        private KrStagePositionInfo(
            CardRow stageRow,
            int absoluteOrder,
            int? shiftedOrder,
            bool saveRow,
            bool hidden,
            CardRow belowStage,
            CardRow aboveStage)
            : base(new Dictionary<string, object>())
        {
            this.Set(nameof(this.RowID), stageRow.RowID);
            this.Set(nameof(this.GroupOrder), stageRow[KrConstants.StageGroupOrder]);
            this.Set(nameof(this.AbsoluteOrder), absoluteOrder);
            this.Set(nameof(this.ShiftedOrder), shiftedOrder);
            this.Set(nameof(this.Hidden), hidden);
            this.Set(nameof(this.Name), stageRow[KrConstants.Name]);
            this.Set(nameof(this.StageTypeID), stageRow[KrConstants.KrStages.StageTypeID]);
            this.Set(nameof(this.StageGroupID), stageRow[KrConstants.StageGroupID]);
            this.Set(nameof(this.GroupPosition), stageRow[KrConstants.KrStages.BasedOnStageTemplateGroupPositionID]);
            this.Set(nameof(this.CardRow), saveRow ? stageRow.GetStorage() : null);
            this.Set(nameof(this.Skip), stageRow.Get<bool>(KrConstants.KrStages.Skip));

            this.Set(nameof(this.BelowVisibleStageRowID), belowStage?.RowID);
            this.Set(nameof(this.AboveVisibleStageRowID), aboveStage?.RowID);
        }

        /// <inheritdoc />
        public KrStagePositionInfo(
            Dictionary<string, object> storage)
            : base(storage)
        {
        }

        /// <inheritdoc />
        public KrStagePositionInfo(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context)
        {
        }

        /// <summary>
        /// Идентификатор скрытого этапа (оригинала из шаблона).
        /// </summary>
        public Guid RowID => this.Get<Guid>(nameof(this.RowID));

        /// <summary>
        /// Порядок сортировки группы этапов.
        /// </summary>
        public int GroupOrder => this.Get<int>(nameof(this.GroupOrder));

        /// <summary>
        /// Абсолютный порядок этапа в маршруте.
        /// </summary>
        public int AbsoluteOrder => this.Get<int>(nameof(this.AbsoluteOrder));

        /// <summary>
        /// Сдвинутый порядок с учетом скрытых этапов.
        /// </summary>
        public int? ShiftedOrder => this.Get<int?>(nameof(this.ShiftedOrder));

        /// <summary>
        /// Признак того, что этап является скрытым.
        /// </summary>
        public bool Hidden => this.Get<bool>(nameof(this.Hidden));

        /// <summary>
        /// Название этапа.
        /// </summary>
        public string Name => this.Get<string>(nameof(this.Name));

        /// <summary>
        /// Групповая позиция этапа в рамках одной группы. <see cref="GroupPosition"/>
        /// </summary>
        public int? GroupPosition => this.Get<int?>(nameof(this.GroupPosition));

        /// <summary>
        /// Идентификатор типа этапа.
        /// </summary>
        public Guid StageTypeID => this.Get<Guid>(nameof(this.StageTypeID));

        /// <summary>
        /// Идентификатор группы этапа.
        /// </summary>
        public Guid StageGroupID => this.Get<Guid>(nameof(this.StageGroupID));

        /// <summary>
        /// Идентификатор ближайщего видимого этапа под текущим.
        /// </summary>
        public Guid? AboveVisibleStageRowID => this.Get<Guid?>(nameof(this.AboveVisibleStageRowID));

        /// <summary>
        /// Идентификатор ближайщего видимого этапа над текущим.
        /// </summary>
        public Guid? BelowVisibleStageRowID => this.Get<Guid?>(nameof(this.BelowVisibleStageRowID));

        /// <summary>
        /// Строка этапа.
        /// </summary>
        public CardRow CardRow => 
            this.cardRow ??= this.CreateCardRow();

        /// <summary>
        /// Признак пропуска этапа.
        /// </summary>
        public bool Skip => this.Get<bool>(nameof(this.Skip));

        private CardRow CreateCardRow()
        {
            var storage = this.Get<Dictionary<string, object>>(nameof(this.CardRow));
            return storage != null
                ? new CardRow(storage)
                : null;
        }

        public static KrStagePositionInfo CreateVisible(
            CardRow stageRow,
            int absoluteOrder,
            int? shiftedOrder)
        {
            return new KrStagePositionInfo(
                stageRow,
                absoluteOrder,
                shiftedOrder,
                false,
                false, 
                null,
                null);
        }

        public static KrStagePositionInfo CreateHidden(
            CardRow stageRow,
            int absoluteOrder,
            bool saveRow,
            CardRow belowStage,
            CardRow aboveStage)
        {
            return new KrStagePositionInfo(
                stageRow,
                absoluteOrder,
                null,
                saveRow,
                true, 
                belowStage, 
                aboveStage);
        }
    }
}