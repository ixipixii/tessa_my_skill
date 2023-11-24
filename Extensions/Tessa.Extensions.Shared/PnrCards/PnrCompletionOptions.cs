using System;

namespace Tessa.Extensions.Shared.PnrCards
{
    public static class PnrCompletionOptions
    {
        /// <summary>
		/// Вариант завершения "Зарегистрировать"
		/// </summary>
		public static readonly Guid PnrRegistration = new Guid("2fc72593-a98c-458f-a3d9-fedf49edb8af");

        /// <summary>
        /// Вариант завершения "Запуск процесса"
        /// </summary>
        public static readonly Guid FdStartProcess = new Guid("24139f8f-27d4-4c48-9cc2-3c746d7687ce");

        /// <summary>
        /// Вариант завершения "В черный список"
        /// </summary>
        public static readonly Guid PnrBlackList = new Guid("96bdde20-9e50-4e3a-a66f-748d0e1f68ec");

        /// <summary>
        /// Вариант завершения "Согласован"
        /// </summary>
        public static readonly Guid PnrAgreed = new Guid("efb52208-e923-430f-9066-a2949cf5a5ba");

        /// <summary>
        /// Вариант завершения "На исполнение"
        /// </summary>
        public static readonly Guid PnrToExecution = new Guid("74153acf-b003-4890-afb4-c4fceb79a32d");

        /// <summary>
        /// Вариант завершения "Исполнено"
        /// </summary>
        public static readonly Guid FdExecuted = new Guid("98cd6684-07d3-44c3-b825-3dbe7d28acad");

        /// <summary>
        /// Вариант завершения "Проверено"
        /// </summary>
        public static readonly Guid PnrCheck = new Guid("635b1d24-3f4e-4c89-a57b-bec01c8164e8");

        /// <summary>
        /// Вариант завершения "Отправить"
        /// </summary>
        public static readonly Guid SendToPerformer = new Guid("f4ebe563-14f6-4b20-a61f-0bac4c11c8ac");

        /// <summary>
        /// Вариант завершения "Подписать"
        /// </summary>
        public static readonly Guid PnrSignDocument = new Guid("33b26500-8158-4873-b729-7f43ee7789d8");

        /// <summary>
        /// При утверждении документа на этапе оформления
        /// </summary>
        public static readonly Guid PnrFormalization = new Guid("77abbb07-282a-465e-bc4b-9295f0c3ecaa");

            

    }
}
