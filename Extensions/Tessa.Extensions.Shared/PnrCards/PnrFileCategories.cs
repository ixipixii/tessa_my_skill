using System;

namespace Tessa.Extensions.Shared.PnrCards
{
    public static class PnrFileCategories
    {
        /// <summary>
        /// Категория файлов "Скан-копия"
        /// </summary>
        public static readonly Guid PnrScanCopyID = new Guid("cd8abd0c-9914-4472-a71c-7463eeeea9f1");
        public static readonly string PnrScanCopyName = "Скан-копия";

        /// <summary>
        /// Категория файлов "Проект доверенности"
        /// </summary>
        public static readonly Guid PnrDraftPowerOfAttorneyID = new Guid("7a268847-ea83-47ef-924f-44784369c5b6");
        public static readonly string PnrDraftPowerOfAttorneyName = "Проект доверенности";

        /// <summary>
        /// Категория файлов "Проект служебной записки"
        /// </summary>
        public static readonly Guid PnrProjectServiceNoteID = new Guid("336bb08f-6e44-4db9-bda4-d9eec90741a0");
        public static readonly string PnrProjectServiceNoteName = "Проект служебной записки";

        /// <summary>
        /// Категория файлов "Бухгалтерский баланс ф.1, ф.2 за последний отчетный год"
        /// </summary>
        public static readonly Guid PnrBalanceSheetID = new Guid("26e2b3fa-75ca-4e7f-8bf2-471a37f18afb");
        public static readonly string PnrBalanceSheetName = "Бухгалтерский баланс ф.1, ф.2 за последний отчетный год";

        /// <summary>
        /// Категория файлов "Выписка ЕГРЮЛ"
        /// </summary>
        public static readonly Guid PnrExtractUnifiedStateRegisterLegalEntitiesID = new Guid("38c92449-b662-4d3e-8cec-ede12708deee");
        public static readonly string PnrExtractUnifiedStateRegisterLegalEntitiesName = "Выписка ЕГРЮЛ";

        /// <summary>
        /// Категория файлов "Договор аренды по фактическому адресу (аренда офиса + свидетельство о собственности помещения)"
        /// </summary>
        public static readonly Guid PnrLeaseContractID = new Guid("5c27056c-ecfe-4d07-a7a5-7e31330dccb1");
        public static readonly string PnrLeaseContractName = "Договор аренды по фактическому адресу (аренда офиса + свидетельство о собственности помещения)";

        /// <summary>
        /// Категория файлов "Копия паспорта генерального директора (1 и 2 страницы)"
        /// </summary>
        public static readonly Guid PnrCopyCEOPassportID = new Guid("6af5ff26-ba59-481b-8ebe-72679184948f");
        public static readonly string PnrCopyCEOPassportName = "Копия паспорта генерального директора (1 и 2 страницы)";

        /// <summary>
        /// Категория файлов "Паспорт (1 страница и страница с регистрацией)"
        /// </summary>
        public static readonly Guid PnrPassportID = new Guid("fa55183f-7085-4f0a-895b-a948d88e3785");
        public static readonly string PnrPassportName = "Паспорт (1 страница и страница с регистрацией)";

        /// <summary>
        /// Категория файлов "Правоустанавливающие документы на фактический адрес (свидетельство о собственности, договор аренды (субаренды) с актом приема-передачи помещений и свидетельством о собственности основного бенефициара)"
        /// </summary>
        public static readonly Guid PnrDocumentsTitleActualAddressID = new Guid("9aaf86cf-0ad1-4993-abc4-32fe23629bed");
        public static readonly string PnrDocumentsTitleActualAddressName = "Правоустанавливающие документы на фактический адрес (свидетельство о собственности, договор аренды (субаренды) с актом приема-передачи помещений и свидетельством о собственности основного бенефициара)";

        /// <summary>
        /// Категория файлов "Приказ о назначении генерального директора"
        /// </summary>
        public static readonly Guid PnrOrderAppointmentGeneralDirectorID = new Guid("c6096f5a-e276-471b-9a05-aea8d57a28f5");
        public static readonly string PnrOrderAppointmentGeneralDirectorName = "Приказ о назначении генерального директора";

        /// <summary>
        /// Категория файлов "Прочие документы"
        /// </summary>
        public static readonly Guid PnrOtherDocumentsID = new Guid("6512cd32-366b-42c1-adfc-5606b3e482ea");
        public static readonly string PnrOtherDocumentsName = "Прочие документы";

        /// <summary>
        /// Категория файлов "Решение о создании юридического лица"
        /// </summary>
        public static readonly Guid PnrDecisionCreateLegalEntityID = new Guid("ff36b3bc-1a06-465d-9d95-d056ce91f176");
        public static readonly string PnrDecisionCreateLegalEntityName = "Решение о создании юридического лица";

        /// <summary>
        /// Категория файлов "РСВ1 за последнюю отчетную дату"
        /// </summary>
        public static readonly Guid PnrRSV1LastReportingDateID = new Guid("e4809b65-20be-4b2b-9c32-b3a0b4c9e6b3");
        public static readonly string PnrRSV1LastReportingDateName = "РСВ1 за последнюю отчетную дату";

        /// <summary>
        /// Категория файлов "Свидетельство ИНН"
        /// </summary>
        public static readonly Guid PnrTINCertificateID = new Guid("348a34ed-e528-40d0-b610-0ebd2880e70e");
        public static readonly string PnrTINCertificateName = "Свидетельство ИНН";

        /// <summary>
        /// Категория файлов "Свидетельство ИНН физ.лица"
        /// </summary>
        public static readonly Guid PnrTINCertificateIndividualID = new Guid("74705b0d-bbd9-45ef-b561-3f8f90d0454e");
        public static readonly string PnrTINCertificateIndividualName = "Свидетельство ИНН физ.лица";

        /// <summary>
        /// Категория файлов "Свидетельство о регистрации физического лица в качестве индивидуального предпринимателя"
        /// </summary>
        public static readonly Guid PnrCertificateRegistrationIndividualID = new Guid("ed31fcae-5a0a-4f34-ac02-9151bb17dbb7");
        public static readonly string PnrCertificateRegistrationIndividualName = "Свидетельство о регистрации физического лица в качестве индивидуального предпринимателя";

        /// <summary>
        /// Категория файлов "Справка из территориальной (межрайонной) ИФНС об отсутствии задолженности по налогам и сборам"
        /// </summary>
        public static readonly Guid PnrCertificateTerritorialInspectorateID = new Guid("a34bf635-7f8f-4c97-aa0f-b187b9fa5008");
        public static readonly string PnrCertificateTerritorialInspectorateName = "Справка из территориальной (межрайонной) ИФНС об отсутствии задолженности по налогам и сборам";

        /// <summary>
        /// Категория файлов "Справка ФНС об отсутствии задолженности по налогам и сборам"
        /// </summary>
        public static readonly Guid PnrCertificateFederalTaxServiceID = new Guid("d00bdb2b-fa76-4a25-b89d-151490713203");
        public static readonly string PnrCertificateFederalTaxServiceName = "Справка ФНС об отсутствии задолженности по налогам и сборам";

        /// <summary>
        /// Категория файлов "СРО, лицензии (при наличии)"
        /// </summary>
        public static readonly Guid PnrSROLicensesID = new Guid("2775698b-08e4-4e66-9948-ba744bc9fc54");
        public static readonly string PnrSROLicensesName = "СРО, лицензии (при наличии)";

        /// <summary>
        /// Категория файлов "Учетная карточка Контрагента"
        /// </summary>
        public static readonly Guid PnrAccountCardPartnerID = new Guid("d5826cc8-ed83-4dc9-a43a-99e727661a93");
        public static readonly string PnrAccountCardPartnerName = "Учетная карточка Контрагента";

        /// <summary>
        /// Категория файлов "Учетная карточка предприятия с контактными телефонами (номера телефонов не мобильные)"
        /// </summary>
        public static readonly Guid PnrAccountCardCompanyID = new Guid("90886bb2-66d7-48ab-869e-7eac61fbc34a");
        public static readonly string PnrAccountCardCompanyName = "Учетная карточка предприятия с контактными телефонами (номера телефонов не мобильные)";

        /// <summary>
        /// Категория файлов "Файл приказа"
        /// </summary>
        public static readonly Guid PnrOrderID = new Guid("778ca9cf-b94b-43b4-b517-c34370e429fa");
        public static readonly string PnrOrderName = "Файл приказа";
    }
}
