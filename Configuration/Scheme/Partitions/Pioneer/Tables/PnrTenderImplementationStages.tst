<?xml version="1.0" encoding="utf-8"?>
<SchemeTable Partition="54ed79f5-11c0-4f74-b8b0-793d596699d6" ID="3422c63f-696a-46f0-8438-1f34f3e1fa1a" Name="PnrTenderImplementationStages" Group="Pnr">
	<Description>Справочник Стадии реализации тендера</Description>
	<SchemePhysicalColumn ID="c1c386ce-465b-462b-a164-3aaba50cff58" Name="ID" Type="Int16 Not Null" />
	<SchemePhysicalColumn ID="7e4a1e62-3868-4f34-8a70-319febb91c3e" Name="Name" Type="String(Max) Null">
		<Description>Стадия</Description>
	</SchemePhysicalColumn>
	<SchemePrimaryKey ID="6c0fd381-6992-44ae-9698-7fca101c785a" Name="pk_PnrTenderImplementationStages" IsClustered="true">
		<SchemeIndexedColumn Column="c1c386ce-465b-462b-a164-3aaba50cff58" />
	</SchemePrimaryKey>
	<SchemeRecord>
		<ID ID="c1c386ce-465b-462b-a164-3aaba50cff58">0</ID>
		<Name ID="7e4a1e62-3868-4f34-8a70-319febb91c3e">Разработка</Name>
	</SchemeRecord>
</SchemeTable>