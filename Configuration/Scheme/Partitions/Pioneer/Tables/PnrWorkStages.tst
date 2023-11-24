<?xml version="1.0" encoding="utf-8"?>
<SchemeTable Partition="54ed79f5-11c0-4f74-b8b0-793d596699d6" ID="716bab18-c31a-4be6-a47b-ff085c0ad0c3" Name="PnrWorkStages" Group="Pnr" InstanceType="Cards" ContentType="Entries">
	<Description>Справочник Стадии работ</Description>
	<SchemeComplexColumn IsSystem="true" IsPermanent="true" IsSealed="true" ID="716bab18-c31a-00e6-2000-0f085c0ad0c3" Name="ID" Type="Reference(Typified) Not Null" ReferencedTable="1074eadd-21d7-4925-98c8-40d1e5f0ca0e">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="716bab18-c31a-01e6-4000-0f085c0ad0c3" Name="ID" Type="Guid Not Null" ReferencedColumn="9a58123b-b2e9-4137-9c6c-5dab0ec02747" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn ID="2ab85ac3-764e-4689-8b67-9b79db35c395" Name="Name" Type="String(Max) Null">
		<Description>Название</Description>
	</SchemePhysicalColumn>
	<SchemePrimaryKey IsSystem="true" IsPermanent="true" IsSealed="true" ID="716bab18-c31a-00e6-5000-0f085c0ad0c3" Name="pk_PnrWorkStages" IsClustered="true">
		<SchemeIndexedColumn Column="716bab18-c31a-01e6-4000-0f085c0ad0c3" />
	</SchemePrimaryKey>
</SchemeTable>