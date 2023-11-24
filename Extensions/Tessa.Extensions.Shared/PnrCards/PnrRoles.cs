using System;

namespace Tessa.Extensions.Shared.PnrCards
{
    public static class PnrRoles
    {
        // Динамические роли:

        public static readonly Guid EmployeeGkID = new Guid("c365e148-71f9-4731-8025-3aeb5225af9f");
        public static readonly string EmployeeGkName = "Сотрудник ГК";

        public static readonly Guid EmployeeUkID = new Guid("b620333e-0fcb-4b69-9576-02208bc8d0d4");
        public static readonly string EmployeeUkName = "Сотрудник УК";

        // Статические роли:

        /// <summary>
        /// ID карточки роли Делопроизводитель
        /// </summary>
        public static readonly Guid Clerk = new Guid("aace84c9-ddb0-40f5-9457-db0e1aec0f77");

        /// <summary>
        /// ID карточки роли Офис-менеджер
        /// </summary>
        public static readonly Guid OfficeManager = new Guid("100b850e-87df-417e-adb2-7e0c95a6f05e");

    }
}
