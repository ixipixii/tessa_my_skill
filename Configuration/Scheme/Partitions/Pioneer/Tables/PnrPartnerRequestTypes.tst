<?xml version="1.0" encoding="utf-8"?>
<SchemeTable Partition="54ed79f5-11c0-4f74-b8b0-793d596699d6" ID="ded5bb90-77ba-40fb-9bf2-9bf740d13cdd" Name="PnrPartnerRequestTypes" Group="Pnr">
	<Description>Тип заявки на контрагента</Description>
	<SchemePhysicalColumn ID="2f368bfb-94ed-4196-bfa2-725904d66e17" Name="ID" Type="Int16 Not Null" />
	<SchemePhysicalColumn ID="e618de38-4d8d-4dbb-9b95-bf73e65bf80f" Name="Name" Type="String(Max) Null" />
	<SchemePrimaryKey ID="2f414864-cc06-4e71-a345-be2f744aa23e" Name="pk_PnrPartnerRequestTypes" IsClustered="true">
		<SchemeIndexedColumn Column="2f368bfb-94ed-4196-bfa2-725904d66e17" />
	</SchemePrimaryKey>
	<SchemeRecord>
		<ID ID="2f368bfb-94ed-4196-bfa2-725904d66e17">0</ID>
		<Name ID="e618de38-4d8d-4dbb-9b95-bf73e65bf80f">Создание нового контрагента</Name>
	</SchemeRecord>
	<SchemeRecord>
		<ID ID="2f368bfb-94ed-4196-bfa2-725904d66e17">1</ID>
		<Name ID="e618de38-4d8d-4dbb-9b95-bf73e65bf80f">Согласование контрагента</Name>
	</SchemeRecord>
</SchemeTable>