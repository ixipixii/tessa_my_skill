<?xml version="1.0" encoding="utf-8"?>
<SchemeTable Partition="54ed79f5-11c0-4f74-b8b0-793d596699d6" ID="c77707ce-b8de-4d47-8d55-08c315491728" Name="PnrYesNoReference" Group="Pnr">
	<Description>Справочник Да/Нет</Description>
	<SchemePhysicalColumn ID="2cd1796f-0b93-4e5b-b0e4-b0bc91d20370" Name="ID" Type="Int16 Not Null" />
	<SchemePhysicalColumn ID="d3a53179-d767-4989-a760-2e085122b5a7" Name="Name" Type="String(Max) Null" />
	<SchemePrimaryKey ID="1f9ecd49-0e29-4fe2-aba7-1e21cf9932ea" Name="pk_PnrYesNoReference" IsClustered="true">
		<SchemeIndexedColumn Column="2cd1796f-0b93-4e5b-b0e4-b0bc91d20370" />
	</SchemePrimaryKey>
	<SchemeRecord>
		<ID ID="2cd1796f-0b93-4e5b-b0e4-b0bc91d20370">0</ID>
		<Name ID="d3a53179-d767-4989-a760-2e085122b5a7">Нет</Name>
	</SchemeRecord>
	<SchemeRecord>
		<ID ID="2cd1796f-0b93-4e5b-b0e4-b0bc91d20370">1</ID>
		<Name ID="d3a53179-d767-4989-a760-2e085122b5a7">Да</Name>
	</SchemeRecord>
</SchemeTable>