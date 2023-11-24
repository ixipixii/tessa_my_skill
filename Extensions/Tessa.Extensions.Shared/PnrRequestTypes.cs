using System;

namespace Tessa.Extensions.Shared
{
    public static class PnrRequestTypes
    {
        /// <summary>
		/// Получить данные Юр лица по ID Организации ГК Пионер
		/// </summary>
		public static readonly Guid GetIndexLegalEntityRequestTypeID = new Guid("1C14A441-B5AB-494A-A9BE-BE01DC9472C2");

		/// <summary>
		/// Получить данные подразделения пользователя
		/// </summary>
		public static readonly Guid GetUserDepartmentInfoRequestTypeID = new Guid("0FD22222-31BF-435F-A0F5-01F8CBC99FE0");

		public static readonly Guid GetPdfFromDocxTypeID = new Guid("5F646016-961A-4790-BF4F-B0C26490E722");

		public static readonly Guid PartnerInfo = new Guid("788206C1-AC9A-48D8-8E2B-0A249850669F");

		/// <summary>
		/// Получить Руководитель ЮЛ по ID Организация ГК
		/// </summary>
		public static readonly Guid GetOrganizationHead = new Guid("C97A1449-34C6-4F3F-87E5-7C1706C8C70D");

		/// <summary>
		/// Получить входит ли пользователь в роль
		/// </summary>
		public static readonly Guid GetIsUserInRoleExtension = new Guid("D8D5496E-B3F7-4709-BCA2-3EBAA6EA059F");

		/// <summary>
		/// Установить статус Fd для карточки
		/// </summary>
		public static readonly Guid SetFdStateCard = new Guid("96F565BB-B3B1-4DA6-981C-BD5449BA9E9B");
	}
}
