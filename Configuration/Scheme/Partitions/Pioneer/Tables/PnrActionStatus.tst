<?xml version="1.0" encoding="utf-8"?>
<SchemeTable Partition="54ed79f5-11c0-4f74-b8b0-793d596699d6" ID="00262199-e52d-4275-b7af-3925446e3fdd" Name="PnrActionStatus" Group="Pnr">
	<Description>Статус действия</Description>
	<SchemePhysicalColumn ID="3e012001-e372-46b0-9029-8d3301313d67" Name="ID" Type="Int16 Not Null" />
	<SchemePhysicalColumn ID="421cb3f3-5523-47fd-8704-5d758c385efb" Name="Name" Type="String(Max) Not Null" />
	<SchemePrimaryKey ID="1ca98c6b-4438-4ff1-b686-59feaee4f208" Name="pk_PnrActionStatus" IsClustered="true">
		<SchemeIndexedColumn Column="3e012001-e372-46b0-9029-8d3301313d67" />
	</SchemePrimaryKey>
	<SchemeRecord>
		<ID ID="3e012001-e372-46b0-9029-8d3301313d67">1</ID>
		<Name ID="421cb3f3-5523-47fd-8704-5d758c385efb">Активна</Name>
	</SchemeRecord>
	<SchemeRecord>
		<ID ID="3e012001-e372-46b0-9029-8d3301313d67">2</ID>
		<Name ID="421cb3f3-5523-47fd-8704-5d758c385efb">Удалена</Name>
	</SchemeRecord>
	<SchemeRecord>
		<ID ID="3e012001-e372-46b0-9029-8d3301313d67">3</ID>
		<Name ID="421cb3f3-5523-47fd-8704-5d758c385efb">Дубликат</Name>
	</SchemeRecord>
	<SchemeRecord>
		<ID ID="3e012001-e372-46b0-9029-8d3301313d67">4</ID>
		<Name ID="421cb3f3-5523-47fd-8704-5d758c385efb">Скрыта</Name>
	</SchemeRecord>
</SchemeTable>