<?xml version="1.0" encoding="utf-8"?>
<SchemeTable Partition="54ed79f5-11c0-4f74-b8b0-793d596699d6" ID="14cf5907-5876-4b3e-8470-41fd6c9abbcf" Name="PnrDirections" Group="Pnr" InstanceType="Cards" ContentType="Entries">
	<Description>Справочник Направления</Description>
	<SchemeComplexColumn IsSystem="true" IsPermanent="true" IsSealed="true" ID="14cf5907-5876-003e-2000-01fd6c9abbcf" Name="ID" Type="Reference(Typified) Not Null" ReferencedTable="1074eadd-21d7-4925-98c8-40d1e5f0ca0e">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="14cf5907-5876-013e-4000-01fd6c9abbcf" Name="ID" Type="Guid Not Null" ReferencedColumn="9a58123b-b2e9-4137-9c6c-5dab0ec02747" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn ID="dce07234-3902-4b2d-a5ea-ef4c0ae3fea2" Name="Name" Type="String(Max) Null">
		<Description>Название</Description>
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="3c5edadd-7506-4bba-a725-919cba57cb70" Name="Description" Type="String(Max) Null">
		<Description>Описание</Description>
	</SchemePhysicalColumn>
	<SchemePrimaryKey IsSystem="true" IsPermanent="true" IsSealed="true" ID="14cf5907-5876-003e-5000-01fd6c9abbcf" Name="pk_PnrDirections" IsClustered="true">
		<SchemeIndexedColumn Column="14cf5907-5876-013e-4000-01fd6c9abbcf" />
	</SchemePrimaryKey>
</SchemeTable>