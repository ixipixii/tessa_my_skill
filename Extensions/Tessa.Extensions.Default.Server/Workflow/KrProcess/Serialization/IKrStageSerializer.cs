using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Tessa.Cards;
using Tessa.Cards.Extensions;
using Tessa.Extensions.Default.Server.Workflow.KrCompilers;
using Tessa.Platform.Storage;

namespace Tessa.Extensions.Default.Server.Workflow.KrProcess.Serialization
{
    public interface IKrStageSerializer
    {
        /// <summary>
        /// Список секций-настроек.
        /// </summary>
        IReadOnlyList<string> SettingsSectionNames { get; }

        /// <summary>
        /// Список полей-настроек.
        /// </summary>
        IReadOnlyList<string> SettingsFieldNames { get; }

        /// <summary>
        /// Список прямых детей секции этапа.
        /// </summary>
        IReadOnlyList<ReferenceToStage> ReferencesToStages { get; }

        /// <summary>
        /// Список секция и столбцов, по которым в табличных контролах проводится сортировка
        /// </summary>
        IReadOnlyList<OrderColumn> OrderColumns { get; }

        /// <summary>
        /// Установить информацию по сериализуемым секциям и полям.
        /// </summary>
        /// <param name="data"></param>
        void SetData(
            KrStageSerializerData data);

        /// <summary>
        /// Сериализовать объект в JSON.
        /// </summary>
        /// <param name="value">Сериализуемый объект.</param>
        /// <returns>Сериализованный объект</returns>
        string Serialize(
            object value);

        /// <summary>
        /// Десериализовать объект из JSON/
        /// </summary>
        /// <typeparam name="T">Тип десериализуемого объекта</typeparam>
        /// <param name="json">Сериализованный объект</param>
        /// <returns>Десериализованный объект</returns>
        T Deserialize<T>(
            string json);

        /// <summary>
        /// Создание настроек этапов по полной структуре виртуальных секций без использования __ParentRowID
        /// </summary>
        /// <param name="allSections"></param>
        /// <returns></returns>
        IDictionary<Guid, IDictionary<string, object>> CreateStageSettings(
            StringDictionaryStorage<CardSection> allSections);

        /// <summary>
        /// Влить изменения в настройках.
        /// </summary>
        /// <param name="krStagesSection"></param>
        /// <param name="updatedSections"></param>
        /// <param name="cancellationToken">Объект, посредством которого можно отменить асинхронную задачу.</param>
        /// <returns>Асинхронная задача.</returns>
        ValueTask<IDictionary<Guid, IDictionary<string, object>>> MergeStageSettingsAsync(
            CardSection krStagesSection,
            StringDictionaryStorage<CardSection> updatedSections,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Сериализовать настройки этапа
        /// </summary>
        /// <param name="stageRow"></param>
        /// <param name="settingsStorage"></param>
        void SerializeSettingsStorage(
            CardRow stageRow,
            IDictionary<string, object> settingsStorage);

        /// <summary>
        /// Десериализовать настройки из этапа.
        /// </summary>
        /// <param name="settings">Настройки этапа</param>
        /// <param name="rowID">Идентификатор строки этапа</param>
        /// <param name="repairStorage">Следует ли восстановить структуру в соотвествии с текущей метаинформацией</param>
        /// <param name="cancellationToken">Объект, посредством которого можно отменить асинхронную задачу.</param>
        /// <returns></returns>
        ValueTask<IDictionary<string, object>> DeserializeSettingsStorageAsync(
            string settings,
            Guid rowID,
            bool repairStorage = true,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Десериализовать настройки из этапа
        /// </summary>
        /// <param name="stageRow">Строка этапа</param>
        /// <param name="repairStorage">Следует ли восстановить структуру в соотвествии с текущей метаинформацией</param>
        /// <param name="cancellationToken">Объект, посредством которого можно отменить асинхронную задачу.</param>
        /// <returns></returns>
        ValueTask<IDictionary<string, object>> DeserializeSettingsStorageAsync(
            CardRow stageRow,
            bool repairStorage = true,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Десериализовать настройки в виртуальные секции.
        /// </summary>
        /// <param name="sourceCard">
        /// Исходная карточка.
        /// </param>
        /// <param name="destCard">
        /// Карточка назначения
        /// </param>
        /// <param name="predeserializedSettings">
        /// Подмножество десериализованных настроек, которое можно использовать вместо явной десериализации из секции
        /// </param>
        /// <param name="hiddenStageMode">
        /// Режим работы со скрытыми этапами
        /// </param>
        /// <param name="cardContext">
        /// Внешний контекст расширения на карточку.
        /// </param>
        /// <param name="cancellationToken">Объект, посредством которого можно отменить асинхронную задачу.</param>
        /// <returns>Асинхронная задача.</returns>
        Task DeserializeSectionsAsync(
            Card sourceCard,
            Card destCard,
            IDictionary<Guid, IDictionary<string, object>> predeserializedSettings = null,
            KrProcessSerializerHiddenStageMode hiddenStageMode = KrProcessSerializerHiddenStageMode.Ignore,
            ICardExtensionContext cardContext = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Обновление сериализуемых настроек в секции KrStages
        /// </summary>
        /// <param name="innerCard"></param>
        /// <param name="outerCard"></param>
        /// <param name="stageStorages"></param>
        /// <param name="krProcessCache"></param>
        /// <param name="cardContext">
        /// Внешний контекст расширения на карточку.
        /// </param>
        /// <param name="cancellationToken">Объект, посредством которого можно отменить асинхронную задачу.</param>
        /// <returns>Асинхронная задача.</returns>
        Task UpdateStageSettingsAsync(
            Card innerCard,
            Card outerCard,
            IDictionary<Guid, IDictionary<string, object>> stageStorages,
            IKrProcessCache krProcessCache,
            ICardExtensionContext cardContext = null,
            CancellationToken cancellationToken = default);
    }
}