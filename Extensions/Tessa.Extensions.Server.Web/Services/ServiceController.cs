using System;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Tessa.Cards;
using Tessa.Extensions.Shared.Services;
using Tessa.Platform;
using Tessa.Platform.Data;
using Tessa.Platform.Runtime;
using Tessa.Web;
using Tessa.Web.Services;
using Unity;

namespace Tessa.Extensions.Server.Web.Services
{
    /// <summary>
    /// Контроллер, для которого задан базовый путь "service". Является примером реализации сервисов в рамках приложения Tessa.
    /// </summary>
    [Route("service"), AllowAnonymous]
    public sealed class ServiceController :
        TessaControllerBase
    {
        #region Constructors

        /// <summary>
        /// Конструктор запрашивает зависимости от контейнера ASP.NET Core, у которого они отличаются от зависимостей в UnityContainer.
        /// </summary>
        /// <param name="scope">Объект, посредством которого можно получить доступ к API Tessa, в т.ч. к UnityContainer.</param>
        /// <param name="hostEnvironment">Информация по среде выполнения ASP.NET Core. Запрошена для примера, обычно не требуется.</param>
        /// <param name="options">Настройки веб-сервиса.</param>
        public ServiceController(
            ITessaWebScope scope,
            IWebHostEnvironment hostEnvironment,
            IOptions<WebOptions> options)
        {
            this.scope = scope;
            this.hostEnvironment = hostEnvironment;
            this.options = options;
        }

        #endregion

        #region Fields

        private readonly ITessaWebScope scope;

        private readonly IWebHostEnvironment hostEnvironment;

        private readonly IOptions<WebOptions> options;

        #endregion

        #region Private Methods

        /// <summary>
        /// Запрашивает сервисы из контейнера Unity. Такие запросы нельзя выполнять в конструкторе.
        /// </summary>
        /// <typeparam name="T">Тип запрашиваемого сервиса.</typeparam>
        /// <returns>Запрошенный сервис.</returns>
        private T Resolve<T>(string name = null) => this.scope.UnityContainer.Resolve<T>(name);


        private string GetLoadedExtensions()
        {
            IUnityContainer container = this.scope.UnityContainer;
            IExtensionAssemblyInfo assemblyInfo = container.ResolveAssemblyInfo();

            return string.Join(Environment.NewLine, assemblyInfo.ServerExtensions);
        }

        #endregion

        #region Controller Methods

        /*
         * Метод доступен по базовому адресу контроллера, не требует авторизации и не обращается к сессии.
         * Для проверки функционирования сервиса перейдите по URL вида: https://localhost/tessa/web/service
         */
        /// <summary>
        /// Возвращает текстовое описание для конфигурации веб-сервиса, если в конфигурации
        /// установлена настройка <c>HealthCheckIsEnabled</c> равной <c>true</c>.
        /// </summary>
        /// <returns>Текстовое описание для конфигурации веб-сервиса.</returns>
        // GET service
        [HttpGet, Produces(MediaTypeNames.Text.Plain)]
        public string Get() =>
            this.options.Value.HealthCheckIsEnabled
                ? $"Syntellect Tessa, build {BuildInfo.Version} of {FormattingHelper.FormatDate(BuildInfo.Date, convertToLocal: false)}{Environment.NewLine}" +
                $"Instance: \"{this.scope.InstanceName}\". Environment: \"{this.hostEnvironment.EnvironmentName}\".{Environment.NewLine}{Environment.NewLine}" +
                $"Running on {EnvironmentHelper.OSQualifiedFriendlyName}{Environment.NewLine}" +
                $"{EnvironmentHelper.NetRuntimeFriendlyName}{Environment.NewLine}{Environment.NewLine}" +
                $"Server extensions:{Environment.NewLine}{this.GetLoadedExtensions()}"
                : "Health check is disabled in configuration";


        /*
         * Метод для входа по паре логин/пароль. Здесь используется клиентская информация по умолчанию и идентификатор неизвестного приложения;
         * обычно вместо этого задают конкретное приложение и передают параметры.
         *
         * Метод возвращает строку с токеном, которую надо передавать в следующие методы
         * или же использоваться API Tessa для проброса токена в HTTP-заголовок "Tessa-Session".
         */
        /// <summary>
        /// Открывает сессию для входа пользователя по паре логин/пароль. Возвращает строку, содержащую токен сессии,
        /// который должен передаваться во все другие запросы к веб-сервисам.
        /// </summary>
        /// <param name="parameters">Параметры входа.</param>
        /// <param name="cancellationToken">Объект, посредством которого можно отменить асинхронную задачу.</param>
        /// <returns>Строка, содержащая токен сессии.</returns>
        // POST service/Login
        [HttpPost("Login"), Consumes(MediaTypes.JsonName, TessaBson), Produces(MediaTypeNames.Text.Plain)]
        public async Task<string> PostLogin(
            [FromBody] IntegrationLoginParameters parameters,
            CancellationToken cancellationToken = default)
        {
            ISessionToken token = await this.Resolve<ISessionServer>()
                .OpenSessionAsync(
                    parameters.Login,
                    parameters.Password,
                    ApplicationIdentifiers.Other,
                    ApplicationLicenseType.Web,
                    SessionServiceType.WebClient,
                    SessionClientParameters.CreateCurrent(),
                    cancellationToken: cancellationToken);

            return token.SerializeToXml(new SessionSerializationOptions { Mode = SessionSerializationMode.Auth });
        }


        /*
         * Метод для закрытия сессии. Здесь используется клиентская информация по умолчанию и идентификатор неизвестного приложения;
         * обычно вместо этого задают конкретное приложение и передают параметры.
         */
        /// <summary>
        /// Закрывает сессию с указанием строки с токеном сессии. Токен возвращается методом открытия сессии <see cref="PostLogin"/>.
        /// Методу не требуется наличие информации по сессии в HTTP-заголовке.
        /// </summary>
        /// <param name="token">Токен закрываемой сессии.</param>
        /// <param name="cancellationToken">Объект, посредством которого можно отменить асинхронную задачу.</param>
        /// <returns>Асинхронная задача.</returns>
        // POST service/Logout
        [HttpPost("Logout"), Consumes(MediaTypeNames.Text.Plain, TessaBson)]
        [SessionMethod]
        public Task PostLogout(
            [FromBody, SessionToken] string token,
            CancellationToken cancellationToken = default) =>
            this.Resolve<ISessionService>().CloseSessionWithTokenAsync(token, cancellationToken);


        /*
         * Атрибут SessionMethod нужен для того, чтобы выполнять действия в пределах сессии.
         * Если в атрибуте указать UserAccessLevel.Administrator, то метод сможет вызвать только администратор Tessa.
         *
         * Если убрать атрибут, то любая внешняя система может вызвать метод от любого пользователя,
         * а также не будет доступна информация по текущей сессии. См. метод GetDataWithoutCheckingToken.
         */
        /// <summary>
        /// Выполняет некоторый запрос для заданного параметра и возвращает результат.
        /// Требует наличия токена сессии в HTTP-заголовке <c>Tessa-Session</c>.
        /// Это метод для тестирования возможностей REST веб-сервиса. Метод требует наличия сессии.
        /// </summary>
        /// <param name="parameter">Параметр запроса.</param>
        /// <param name="cancellationToken">Объект, посредством которого можно отменить асинхронную задачу.</param>
        /// <returns>Результат запроса.</returns>
        /// <remarks>
        /// Информация по HTTP-заголовкам, используемым платформой, доступна в методе <see cref="SessionHttpRequestHeader"/>.
        /// </remarks>
        // POST service/GetData
        [HttpPost("GetData"), Consumes(MediaTypeNames.Text.Plain, TessaBson), Produces(MediaTypeNames.Text.Plain)]
        [SessionMethod]
        public async Task<string> PostGetData(
            [FromBody] string parameter,
            CancellationToken cancellationToken = default)
        {
            // максимум первые десять символов
            string data = string.IsNullOrEmpty(parameter)
                ? parameter
                : parameter.Substring(0, Math.Min(10, parameter.Length));

            IDbScope dbScope = this.Resolve<IDbScope>();
            await using (dbScope.Create())
            {
                DbManager db = dbScope.Db;

                return await db
                    .SetCommand(
                        // запрос "SELECT @Data"
                        dbScope.BuilderFactory
                            .Select().P("Data")
                            .Build(),
                        db.Parameter("Data", data))
                    .LogCommand()
                    .ExecuteAsync<string>(cancellationToken);
            }
        }


        /*
         * В первом параметре метода содержится строка с сериализованным токеном сессии.
         * Это задаётся атрибутом [SessionToken].
         */
        /// <summary>
        /// Выполняет некоторый запрос для заданного параметра и возвращает результат.
        /// Токен сессии передаётся в параметре <paramref name="token"/>. Не требует наличия токена сессии в HTTP-заголовке.
        /// Это метод для тестирования возможностей REST веб-сервиса. Метод требует наличия сессии.
        /// </summary>
        /// <param name="token">Токен текущей сессии.</param>
        /// <param name="parameter">Параметр запроса.</param>
        /// <param name="cancellationToken">Объект, посредством которого можно отменить асинхронную задачу.</param>
        /// <returns>Результат запроса.</returns>
        // POST service/GetDataWhenTokenInFirstParameter
        [HttpPost("GetDataWhenTokenInFirstParameter"), Consumes(TessaBson), Produces(MediaTypeNames.Text.Plain)]
        [SessionMethod(UserAccessLevel.Administrator)]
        public Task<string> PostGetDataWhenTokenInFirstParameter(
            [FromBody, SessionToken] string token,
            [FromBody] string parameter,
            CancellationToken cancellationToken = default) =>
            this.PostGetData(parameter, cancellationToken);


        /*
         * Метод не использует сессию через атрибут [SessionMethod]
         * и выполняет любые действия от пользователя "System"
         */
        // POST service/GetDataWithoutCheckingToken
        /// <summary>
        /// Выполняет некоторый запрос для заданного параметра и возвращает результат.
        /// Это метод для тестирования возможностей REST веб-сервиса. Метод не требует наличия сессии.
        /// </summary>
        /// <param name="parameter">Параметр запроса.</param>
        /// <param name="cancellationToken">Объект, посредством которого можно отменить асинхронную задачу.</param>
        /// <returns>Результат запроса.</returns>
        [HttpPost("GetDataWithoutCheckingToken"), Consumes(MediaTypeNames.Text.Plain, TessaBson), Produces(MediaTypeNames.Text.Plain)]
        public async Task<string> PostGetDataWithoutCheckingToken(
            [FromBody] string parameter,
            CancellationToken cancellationToken = default)
        {
            var serverSettings = this.Resolve<ITessaServerSettings>();
            await using (SessionContext.Create(Session.CreateSystemToken(SessionType.Server, serverSettings)))
            {
                // код внутри будет выполняться от имени пользователя System
                return await this.PostGetData(parameter, cancellationToken);
            }
        }


        /*
         * Метод для загрузки карточки, используя сериализацию BSON.
         * И сериализация, и передача сессии обычно выполняются средствами API.
         */
        /// <summary>
        /// Загружает карточку по заданному запросу для desktop-клиента.
        /// Метод идентичен типовому методу загрузки карточки в контроллере <c>CardsController</c>.
        /// Это метод для тестирования возможностей REST веб-сервиса. Метод требует наличия сессии.
        /// </summary>
        /// <param name="request">Запрос на загрузку карточки.</param>
        /// <param name="cancellationToken">Объект, посредством которого можно отменить асинхронную задачу.</param>
        /// <returns>Ответ на запрос на загрузку карточки.</returns>
        // POST service/GetCard
        [HttpPost("GetCard"), Consumes(TessaBson), Produces(TessaBson)]
        [SessionMethod]
        public Task<CardGetResponse> PostGetCard(
            [FromBody] CardGetRequest request,
            CancellationToken cancellationToken = default) =>
            // ICardService проставляет ServiceType и вызывает ICardRepository
            this.Resolve<ICardService>().GetAsync(request, cancellationToken);

        /// <summary>
        /// Открывает карточку и возвращает JSON с типизированной структурой объекта <see cref="CardGetResponse"/>.
        /// Токен сессии передаётся в HTTP-заголовке "Tessa-Session".
        /// </summary>
        /// <param name="id">Идентификатор карточки <see cref="Guid"/>. Передаётся в адресной строке.</param>
        /// <param name="type">Алиас типа карточки. Передаётся как параметр в адресной строке. Необязательный параметр.</param>
        /// <param name="cancellationToken">Объект, посредством которого можно отменить асинхронную задачу.</param>
        /// <returns>Ответ на запрос на загрузку карточки, сериализованный как типизированный JSON.</returns>
        [HttpPost("card/{id}"), Produces(MediaTypeNames.Text.Plain)]
        [SessionMethod]
        public async Task<string> PostGetCardByID(
            Guid id,
            string type = null,
            CancellationToken cancellationToken = default)
        {
            // если не установить ServiceType, то запрос выполнится с пропуском ряда проверок на валидность запроса и прав пользователя
            var request = new CardGetRequest { CardID = id, CardTypeName = type, ServiceType = CardServiceType.WebClient };
            var response = await this.Resolve<ICardRepository>().GetAsync(request, cancellationToken);

            return response.ToTypedJson();
        }

        #endregion
    }
}