using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Tessa.Cards;
using Tessa.Extensions.Shared.Services;
using Tessa.Platform.Runtime;

namespace Tessa.Extensions.Client.Services
{
    /// <summary>
    /// Прокси-класс для обращения к методам контроллера Tessa.Extensions.Server.Web/Controllers/ServiceController.
    /// Все методы в нём асинхронные, но аналогичны методам сервиса <see cref="IService"/>.
    /// </summary>
    public sealed class ServiceWebProxy :
        WebProxy
    {
        #region Constructors

        /*
         * Базовому конструктору передаётся путь к контроллеру, задаваемый в атрибуте [Route("...")] на классе контроллера.
         */
        /// <doc path='info[@type="class" and @item=".ctor"]'/>
        public ServiceWebProxy()
            : base("service")
        {
        }

        #endregion

        #region Methods

        /*
         * Флаг RequestFlags.IgnoreSession запрещает передавать токен сессии в HTTP-заголовке "Tessa-Session".
         * Это актуально в методах логина, при передаче токена в параметре или при вызове методов, которым не требуется логин.
         */
        public Task<string> LoginAsync(IntegrationLoginParameters parameters, CancellationToken cancellationToken = default) =>
            this.SendAsync<string>(HttpMethod.Post, "Login", RequestFlags.IgnoreSession, cancellationToken,
                request => request.Content = TessaHttpContent.FromJsonObject(parameters));

        public Task LogoutAsync(string token, CancellationToken cancellationToken = default) =>
            this.PostWithFlagsAndCancellationAsync<Void>("Logout", RequestFlags.IgnoreSession, cancellationToken, token);

        public Task<string> GetDataAsync(string parameter, CancellationToken cancellationToken = default) =>
            this.PostWithCancellationAsync<string>("GetData", cancellationToken, parameter);

        public Task<string> GetDataWhenTokenInFirstParameterAsync(string token, string parameter, CancellationToken cancellationToken = default) =>
            this.PostWithFlagsAndCancellationAsync<string>("GetDataWhenTokenInFirstParameter", RequestFlags.IgnoreSession, cancellationToken, token, parameter);

        public Task<string> GetDataWithoutCheckingTokenAsync(string parameter, CancellationToken cancellationToken = default) =>
            this.PostWithFlagsAndCancellationAsync<string>("GetDataWithoutCheckingToken", RequestFlags.IgnoreSession, cancellationToken, parameter);

        /*
         * Для передачи потоковых данных в метод используйте единственный параметр типа Stream.
         * Для получения потоковых данных от сервера возвращайте значение Task<Stream>.
         */
        public Task<CardGetResponse> GetCardAsync(CardGetRequest request, CancellationToken cancellationToken = default) =>
            this.PostWithCancellationAsync<CardGetResponse>("GetCard", cancellationToken, request);

        /*
         * Пример передачи параметров через адресную строку.
         */
        public Task<string> GetCardByIDAsync(Guid cardID, string cardTypeName = null, CancellationToken cancellationToken = default) =>
            this.PostWithCancellationAsync<string>(
                $"card/{cardID}" + (string.IsNullOrEmpty(cardTypeName) ? null : $"?type={HttpUtility.UrlEncode(cardTypeName)}"),
                cancellationToken);

        #endregion
    }
}