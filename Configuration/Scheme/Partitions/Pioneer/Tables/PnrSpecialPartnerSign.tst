<?xml version="1.0" encoding="utf-8"?>
<SchemeTable Partition="54ed79f5-11c0-4f74-b8b0-793d596699d6" ID="f3fe8747-5d52-4f09-b34a-2cc0309565a6" Name="PnrSpecialPartnerSign" Group="Pnr">
	<Description>Особый статус контрагента</Description>
	<SchemePhysicalColumn ID="05cf3057-a85c-461c-91db-985517c720ac" Name="ID" Type="Int16 Not Null" />
	<SchemePhysicalColumn ID="d4304713-0b40-4aa0-8a00-46a059bc395c" Name="Name" Type="String(Max) Null" />
	<SchemePrimaryKey ID="66c39cef-965f-49b3-a3f9-46b29ace1eb4" Name="pk_PnrSpecialPartnerSign" IsClustered="true">
		<SchemeIndexedColumn Column="05cf3057-a85c-461c-91db-985517c720ac" />
	</SchemePrimaryKey>
	<SchemeRecord>
		<ID ID="05cf3057-a85c-461c-91db-985517c720ac">0</ID>
		<Name ID="d4304713-0b40-4aa0-8a00-46a059bc395c">Нет</Name>
	</SchemeRecord>
	<SchemeRecord>
		<ID ID="05cf3057-a85c-461c-91db-985517c720ac">1</ID>
		<Name ID="d4304713-0b40-4aa0-8a00-46a059bc395c">Монополист</Name>
	</SchemeRecord>
	<SchemeRecord>
		<ID ID="05cf3057-a85c-461c-91db-985517c720ac">2</ID>
		<Name ID="d4304713-0b40-4aa0-8a00-46a059bc395c">Гос. органы</Name>
	</SchemeRecord>
</SchemeTable>