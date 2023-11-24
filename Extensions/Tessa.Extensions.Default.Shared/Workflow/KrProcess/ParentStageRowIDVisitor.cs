using System;
using System.Collections.Generic;
using Tessa.Cards;

namespace Tessa.Extensions.Default.Shared.Workflow.KrProcess
{
    public class ParentStageRowIDVisitor : DescendantSectionsVisitor
    {
        /// <inheritdoc />
        public ParentStageRowIDVisitor(
            ICardMetadata cardMetadata)
            : base(cardMetadata)
        {
        }

        /// <inheritdoc />
        protected override void VisitTopLevelSection(
            CardRow row,
            CardSection section,
            IDictionary<Guid, Guid> stageMapping)
        {
            row[KrConstants.Keys.ParentStageRowID] = row.RowID;
        }

        /// <inheritdoc />
        protected override void VisitNestedSection(
            CardRow row,
            CardSection section,
            Guid parentRowID,
            Guid topLevelRowID,
            IDictionary<Guid, Guid> stageMapping)
        {
            row[KrConstants.Keys.ParentStageRowID] = topLevelRowID;
        }
    }
}