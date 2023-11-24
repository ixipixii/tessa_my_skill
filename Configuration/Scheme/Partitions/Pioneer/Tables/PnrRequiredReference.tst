<?xml version="1.0" encoding="utf-8"?>
<SchemeTable Partition="54ed79f5-11c0-4f74-b8b0-793d596699d6" ID="5252bdd2-2ad8-4525-aa48-b956e522ae9d" Name="PnrRequiredReference" Group="Pnr">
	<Description>Справочник Требуется/Не требуется</Description>
	<SchemePhysicalColumn ID="7b0bf1da-7eb9-4f66-8d62-4962e8dc4888" Name="ID" Type="Int16 Not Null" />
	<SchemePhysicalColumn ID="bdf2b76d-7422-4258-b29a-462e67376af0" Name="Name" Type="String(Max) Null" />
	<SchemePrimaryKey ID="c6c5a081-5071-437d-be5d-11e4f5fe3219" Name="pk_PnrRequiredReference" IsClustered="true">
		<SchemeIndexedColumn Column="7b0bf1da-7eb9-4f66-8d62-4962e8dc4888" />
	</SchemePrimaryKey>
	<SchemeRecord>
		<ID ID="7b0bf1da-7eb9-4f66-8d62-4962e8dc4888">0</ID>
		<Name ID="bdf2b76d-7422-4258-b29a-462e67376af0">Не требуется</Name>
	</SchemeRecord>
	<SchemeRecord>
		<ID ID="7b0bf1da-7eb9-4f66-8d62-4962e8dc4888">1</ID>
		<Name ID="bdf2b76d-7422-4258-b29a-462e67376af0">Требуется</Name>
	</SchemeRecord>
</SchemeTable>