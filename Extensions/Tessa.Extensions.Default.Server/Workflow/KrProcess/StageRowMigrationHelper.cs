using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tessa.Cards;
using Tessa.Extensions.Default.Server.Workflow.KrProcess.Serialization;
using Tessa.Extensions.Default.Shared.Workflow.KrProcess;
using Tessa.Platform;
using Tessa.Platform.Storage;

namespace Tessa.Extensions.Default.Server.Workflow.KrProcess
{
    public static class StageRowMigrationHelper
    {

        public static async Task MigrateAsync(
            Card source,
            Card target,
            KrProcessSerializerHiddenStageMode hiddenStageMode,
            IKrStageSerializer serializer,
            ISignatureProvider signatureProvider = null,
            CancellationToken cancellationToken = default)
        {
            var sourceRows = source.GetStagesSection().Rows;
            var rowsMapping = new Dictionary<Guid, Guid>(2 * sourceRows.Count);
            var settingsMapping = new Dictionary<Guid, IDictionary<string, object>>(2 * sourceRows.Count);
            var signatures = new Dictionary<string, object>(2 * sourceRows.Count);
            var orders = new Dictionary<string, object>(2 * sourceRows.Count);
            bool? containsPrevRowID = null;

            foreach (var sourceRow in sourceRows)
            {
                var newRowID = Guid.NewGuid();
                var oldRowID = GetRowID(sourceRow);
                rowsMapping.Add(oldRowID, newRowID);
                sourceRow.RowID = newRowID;

                var settings = sourceRow.TryGet<string>(KrConstants.KrStages.Settings);
                var settingsStorage = serializer.Deserialize<Dictionary<string, object>>(settings);
                settingsMapping.Add(newRowID, settingsStorage);

                // Восстанавливаем идентификаторы дочерних строк, т.к. при сохранении и проверке подписи будут уже новые.
                foreach (var refToStage in serializer.ReferencesToStages)
                {
                    if (settingsStorage.TryGetValue(refToStage.SectionName, out var obj)
                        && obj is List<object> list)
                    {
                        foreach (var rowObj in list)
                        {
                            if (rowObj is IDictionary<string, object> row
                                && row.TryGetValue(refToStage.RowIDFieldName, out var oldChildRowIDObj)
                                && oldChildRowIDObj is Guid oldChildRowID
                                && rowsMapping.TryGetValue(oldChildRowID, out var newChildRowID))
                            {
                                row[refToStage.RowIDFieldName] = newChildRowID;
                            }
                        }
                    }
                }

                if (signatureProvider != null)
                {
                    // Подписываем необходимую информацию об этапе.
                    var bytes = ConvertKrStageRowToBytes(sourceRow, settingsStorage);
                    var signature = signatureProvider.Sign(bytes);
                    signatures[newRowID.ToString()] = signature;
                    orders[newRowID.ToString()] = sourceRow.TryGet<int>(KrConstants.Order);
                }
            }

            await serializer.DeserializeSectionsAsync(
                source,
                target,
                settingsMapping,
                hiddenStageMode: hiddenStageMode,
                cancellationToken: cancellationToken);

            SetStateInserted(target, serializer);

            SetAllStagesInactive(target);

            if (signatures.Count != 0)
            {
                target.Info[KrConstants.Keys.KrStageRowsSignatures] = signatures;
                target.Info[KrConstants.Keys.KrStageRowsOrders] = orders;
            }

            Guid GetRowID(
                CardRow row)
            {
                if (containsPrevRowID != false
                    && row.Fields.TryGetValue(CardHelper.UserKeyPrefix + KrConstants.RowID, out var oldRowIDObj)
                    && oldRowIDObj is Guid oldRowID)
                {
                    containsPrevRowID = true;
                    return oldRowID;
                }

                containsPrevRowID = false;
                return row.RowID;
            }
        }

        public static IDictionary<string, object> GetSignatures(
            IDictionary<string, object> signatureStorage)
        {
            return signatureStorage.TryGet<Dictionary<string, object>>(KrConstants.Keys.KrStageRowsSignatures);
        }

        public static IDictionary<string, object> GetOrders(
            IDictionary<string, object> signatureStorage)
        {
            return signatureStorage.TryGet<Dictionary<string, object>>(KrConstants.Keys.KrStageRowsOrders);
        }

        public static void VerifyRow(
            CardRow row,
            IDictionary<string, object> signatures,
            IDictionary<string, object> orders,
            out bool rowChanged,
            out bool orderChanged,
            IKrStageSerializer serializer,
            ISignatureProvider signatureProvider)
        {
            rowChanged = false;
            orderChanged = false;
            if (signatures is null
                || (signatures.TryGetValue(row.RowID.ToString(), out var signatureObj)
                && !CheckRowSignature(row, signatureObj as byte[], serializer, signatureProvider)))
            {
                rowChanged = true;
            }

            if (orders is null
                || (orders.TryGetValue(row.RowID.ToString(), out var currentOrderObj)
                && !row.TryGet<int?>(KrConstants.Order).Equals(currentOrderObj)))
            {
                orderChanged = true;
            }
        }

        private static void SetStateInserted(
            Card card,
            IKrStageSerializer serializer)
        {
            foreach (var stageName in serializer.SettingsSectionNames)
            {
                if (card.Sections.TryGetValue(stageName, out var section))
                {
                    foreach (var row in section.Rows)
                    {
                        row.State = CardRowState.Inserted;
                    }
                }
            }
        }

        private static void SetAllStagesInactive(
            Card card)
        {
            if (!card.TryGetStagesSection(out var stagesSec))
            {
                return;
            }

            foreach (var row in stagesSec.Rows)
            {
                row[KrConstants.KrStages.StageStateID] = KrStageState.Inactive.ID;
                row[KrConstants.KrStages.StageStateName] = KrStageState.Inactive.TryGetDefaultName();
            }

            if (card.TryGetStagePositions(out var stagePositions))
            {
                foreach (var sp in stagePositions)
                {
                    var row = sp.CardRow;
                    if (row != null)
                    {
                        row[KrConstants.KrStages.StageStateID] = KrStageState.Inactive.ID;
                        row[KrConstants.KrStages.StageStateName] = KrStageState.Inactive.TryGetDefaultName();
                    }
                }
            }
        }

        private static byte[] ConvertKrStageRowToBytes(
            CardRow row,
            Dictionary<string, object> settingsStorage)
        {
            var pairs = settingsStorage.OrderBy(p => p.Key).ToList();
            var storage = new List<object> { row[KrConstants.Name], row[KrConstants.KrStages.TimeLimit] };
            storage.AddRange(pairs.Select(x => x.Key));
            storage.AddRange(pairs.Select(x => x.Value));
            var bytes = new Dictionary<string, object> { { "_", storage } }.ToSerializable().Serialize();
            return bytes;
        }

        private static bool CheckRowSignature(
            CardRow row,
            byte[] signature,
            IKrStageSerializer serializer,
            ISignatureProvider signatureProvider)
        {
            if (signature is null)
            {
                return true;
            }

            // Проверяем подписанную при создании строку, чтобы удостоверится, что пользователь не менял ее
            // до первого сохранения
            var settings = row.TryGet<string>(KrConstants.KrStages.Settings);
            var settingsStorage = serializer.Deserialize<Dictionary<string, object>>(settings);
            var bytes = ConvertKrStageRowToBytes(row, settingsStorage);
            return signatureProvider.Verify(bytes, signature);
        }
    }
}