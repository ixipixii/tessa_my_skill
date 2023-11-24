<?xml version="1.0" encoding="utf-8"?>
<SchemeTable Partition="54ed79f5-11c0-4f74-b8b0-793d596699d6" ID="e50c0238-8162-4db0-bf46-b54ef3f36bc7" Name="PnrConditionsStartingWork" Group="Pnr" InstanceType="Cards" ContentType="Entries">
	<Description>Справочник Условие начала выполнения работ</Description>
	<SchemeComplexColumn IsSystem="true" IsPermanent="true" IsSealed="true" ID="e50c0238-8162-00b0-2000-054ef3f36bc7" Name="ID" Type="Reference(Typified) Not Null" ReferencedTable="1074eadd-21d7-4925-98c8-40d1e5f0ca0e">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="e50c0238-8162-01b0-4000-054ef3f36bc7" Name="ID" Type="Guid Not Null" ReferencedColumn="9a58123b-b2e9-4137-9c6c-5dab0ec02747" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn ID="a995dc5f-63af-4320-9a09-f52960df09af" Name="Name" Type="String(Max) Null">
		<Description>Название</Description>
	</SchemePhysicalColumn>
	<SchemePrimaryKey IsSystem="true" IsPermanent="true" IsSealed="true" ID="e50c0238-8162-00b0-5000-054ef3f36bc7" Name="pk_PnrConditionsStartingWork" IsClustered="true">
		<SchemeIndexedColumn Column="e50c0238-8162-01b0-4000-054ef3f36bc7" />
	</SchemePrimaryKey>
</SchemeTable>