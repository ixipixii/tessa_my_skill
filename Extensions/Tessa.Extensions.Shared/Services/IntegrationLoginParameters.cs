namespace Tessa.Extensions.Shared.Services
{
    /// <summary>
    /// Параметры входа в учётную запись для интеграционных сервисов.
    /// </summary>
    public class IntegrationLoginParameters
    {
        /// <summary>
        /// Логин к учётной записи.
        /// </summary>
        public string Login { get; set; }

        /// <summary>
        /// Пароль к учётной записи.
        /// </summary>
        public string Password { get; set; }
    }
}