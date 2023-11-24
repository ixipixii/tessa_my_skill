using Tessa.Platform;

namespace Tessa.Extensions.Default.Server.Workflow.KrCompilers
{
    public interface IKrProcessButton : IKrSecondaryProcess, IVisibilitySources
    {
        /// <summary>
        /// Отображаемое название кнопки.
        /// </summary>
        string Caption { get; }

        /// <summary>
        /// Значок.
        /// </summary>
        string Icon { get; }

        /// <summary>
        /// Размер кнопки.
        /// </summary>
        TileSize TileSize { get; }

        /// <summary>
        /// Подсказка.
        /// </summary>
        string Tooltip { get; }

        /// <summary>
        /// Группа кнопки.
        /// </summary>
        string TileGroup { get; }

        /// <summary>
        /// Сообщение, которое нужно показать после нажатия.
        /// </summary>
        string Message { get; }

        /// <summary>
        /// Обновить список заданий после нажатия: флажок, если стоит, 
        /// то после выполнения нужно вызвать метод обновления списка заданий и показа уведомления.
        /// </summary>
        bool RefreshAndNotify { get; }

        /// <summary>
        /// Спрашивать подтверждения перед нажатием.
        /// </summary>
        bool AskConfirmation { get; }

        /// <summary>
        /// Текст подтверждения при нажатии.
        /// </summary>
        string ConfirmationMessage { get; }

        /// <summary>
        /// Выполнить группировку действий.
        /// </summary>
        bool ActionGrouping { get; }
        
        /// <summary>
        /// Сочетание клавиш
        /// </summary>
        string ButtonHotkey { get; }
    }
}