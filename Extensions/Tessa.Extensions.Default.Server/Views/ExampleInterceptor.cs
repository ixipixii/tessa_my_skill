// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExampleInterceptor.cs" company="Syntellect">
//   Tessa Project
// </copyright>
// <summary>
//   Пример реализации перехватчика представлений
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Tessa.Extensions.Default.Server.Views
{
    #region

    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using Tessa.Views;

    #endregion

    /// <summary>
    ///     Пример реализации перехватчика представлений
    /// </summary>
    public sealed class ExampleInterceptor : IViewInterceptor
    {
        #region Constants

        /// <summary>
        ///     Псевдоним перехватываемого представления
        /// </summary>
        private const string InterceptedViewAlias = "InterceptedViewAlias";

        /// <summary>
        ///     Псевдоним вызываемого представления
        /// </summary>
        private const string OtherViewAlias = "OtherViewAlias";

        #endregion

        #region Fields

        /// <summary>
        ///     Сервис представлений
        /// </summary>
        private readonly Func<IViewService> viewService;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ExampleInterceptor"/> class.
        ///     Инициализирует новый экземпляр класса <see cref="T:System.Object"/>.
        /// </summary>
        /// <param name="viewService">
        /// Сервис представлений
        /// </param>
        public ExampleInterceptor(Func<IViewService> viewService)
        {
            this.viewService = viewService;
            this.InterceptedViews = new[] { InterceptedViewAlias };
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets список обрабатываемых представлений
        /// </summary>
        public string[] InterceptedViews { get; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Осуществляет выполнение запроса на получение данных
        /// </summary>
        /// <param name="request">
        /// Запрос
        /// </param>
        /// <param name="cancellationToken">Объект, посредством которого можно отменить асинхронную задачу.</param>
        /// <returns>
        /// Результат обработки
        /// </returns>
        public async Task<ITessaViewResult> GetDataAsync(ITessaViewRequest request, CancellationToken cancellationToken = default)
        {
            if (request.View?.Alias != InterceptedViewAlias)
            {
                throw new InvalidOperationException("Unknown view");
            }

            ITessaView view = await this.viewService().GetByNameAsync(OtherViewAlias, cancellationToken);
            if (view is null)
            {
                throw new InvalidOperationException("Unknown view");
            }

            return await view.GetDataAsync(request, cancellationToken);

        }

        /// <summary>
        /// Вызывает инициализацию перехватчика передавая в него
        ///     список перехватываемых представлений <paramref name="overlayViews"/>
        /// </summary>
        /// <param name="overlayViews">
        /// Список перехватываемых представлений
        /// </param>
        public void InitOverlay(IDictionary<string, ITessaView> overlayViews)
        {
        }

        #endregion
    }
}