using System;
using System.Collections.Generic;
using Tessa.Platform;

namespace Tessa.Extensions.Default.Server.Workflow.KrCompilers
{
    public sealed class KrProcessButton : KrSecondaryProcess, IKrProcessButton
    {
        public KrProcessButton(
            Guid id,
            string name,
            bool isGlobal,
            bool async,
            string executionAccessDeniedMessage,
            bool runOnce,
            IEnumerable<Guid> contextRolesIDs,
            string executionSqlCondition,
            string executionSourceCondition,
            string caption,
            string icon,
            TileSize tileSize,
            string tooltip,
            string tileGroup,
            string message,
            bool refreshAndNotify,
            bool askConfirmation,
            string confirmationMessage,
            bool actionGrouping,
            string buttonHotkey,
            string visibilitySqlCondition,
            string visibilitySourceCondition) 
        : base(id, name, isGlobal, async,
            executionAccessDeniedMessage, runOnce, contextRolesIDs, 
            executionSqlCondition, executionSourceCondition)
        {
            this.Caption = caption;
            this.Icon = icon;
            this.Tooltip = tooltip;
            this.TileSize = tileSize;
            this.TileGroup = tileGroup;
            this.Message = message;
            this.RefreshAndNotify = refreshAndNotify;
            this.AskConfirmation = askConfirmation;
            this.ConfirmationMessage = confirmationMessage;
            this.ActionGrouping = actionGrouping;
            this.ButtonHotkey = buttonHotkey;
            this.VisibilitySqlCondition = visibilitySqlCondition;
            this.VisibilitySourceCondition = visibilitySourceCondition;
        }

        /// <inheritdoc />
        public string Caption { get; }

        /// <inheritdoc />
        public string Icon { get; }

        /// <inheritdoc />
        public TileSize TileSize { get; }

        /// <inheritdoc />
        public string Tooltip { get; }

        /// <inheritdoc />
        public string TileGroup { get; }

        /// <inheritdoc />
        public string Message { get; }

        /// <inheritdoc />
        public bool RefreshAndNotify { get; }

        /// <inheritdoc />
        public bool AskConfirmation { get; }

        /// <inheritdoc />
        public string ConfirmationMessage { get; }

        /// <inheritdoc />
        public bool ActionGrouping { get; }
        
        /// <inheritdoc />
        public string ButtonHotkey { get; }

        /// <inheritdoc />
        public string VisibilitySqlCondition { get; }

        /// <inheritdoc />
        public string VisibilitySourceCondition { get; }
    }
}