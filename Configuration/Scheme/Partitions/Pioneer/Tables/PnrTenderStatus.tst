<?xml version="1.0" encoding="utf-8"?>
<SchemeTable Partition="54ed79f5-11c0-4f74-b8b0-793d596699d6" ID="60efb9ae-b38b-4568-b44d-dca30e74dc17" Name="PnrTenderStatus" Group="Pnr">
	<Description>Справочник Статус тендера</Description>
	<SchemePhysicalColumn ID="88bfaf6d-36e4-492c-9595-ffa4e2119ae9" Name="ID" Type="Int16 Not Null" />
	<SchemePhysicalColumn ID="bd173e71-ebda-4a27-be54-0171037c05b7" Name="Name" Type="String(Max) Null">
		<Description>Статус</Description>
	</SchemePhysicalColumn>
	<SchemePrimaryKey ID="71e21020-219c-4527-846f-dc5fa0a36acd" Name="pk_PnrTenderStatus" IsClustered="true">
		<SchemeIndexedColumn Column="88bfaf6d-36e4-492c-9595-ffa4e2119ae9" />
	</SchemePrimaryKey>
	<SchemeRecord>
		<ID ID="88bfaf6d-36e4-492c-9595-ffa4e2119ae9">0</ID>
		<Name ID="bd173e71-ebda-4a27-be54-0171037c05b7">Не пройден</Name>
	</SchemeRecord>
	<SchemeRecord>
		<ID ID="88bfaf6d-36e4-492c-9595-ffa4e2119ae9">1</ID>
		<Name ID="bd173e71-ebda-4a27-be54-0171037c05b7">Пройден</Name>
	</SchemeRecord>
</SchemeTable>