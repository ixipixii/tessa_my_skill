namespace Tessa.Extensions.Default.Client.Views
{
    using System;
    using System.Threading;

    using Tessa.UI.Views;

    /// <summary>
    /// Расширение позволяющее имитировать задержку получения данных представлением.
    /// </summary>
    public sealed class GetDataWithDelayExtension : IWorkplaceViewComponentExtension
    {
        #region Public Methods and Operators

        /// <summary>
        /// Вызывается при клонировании модели <paramref name="source"/> в контексте <paramref name="context"/>
        /// </summary>
        /// <param name="source">
        /// Исходная модель
        /// </param>
        /// <param name="cloned">
        /// Клориованная модель
        /// </param>
        /// <param name="context">
        /// Контекст клонирования
        /// </param>
        public void Clone(IWorkplaceViewComponent source, IWorkplaceViewComponent cloned, ICloneableContext context)
        {
        }

        /// <summary>
        /// Вызывается после создания модели <paramref name="model"/> 
        /// </summary>
        /// <param name="model">
        /// Инициализируемая модель
        /// </param>
        public void Initialize(IWorkplaceViewComponent model)
        {
            var wrappedGetData = model.GetData;
            model.GetData = (component, request) =>
                {
                    Thread.Sleep(TimeSpan.FromSeconds(3));
                    return wrappedGetData(component, request);
                };
        }

        /// <summary>
        /// Вызывается после создания модели <paramref name="model"/> перед отображени в UI
        /// </summary>
        /// <param name="model">
        /// Модель
        /// </param>
        public void Initialized(IWorkplaceViewComponent model)
        {
        }

        #endregion
    }
}