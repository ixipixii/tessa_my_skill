<?xml version="1.0" encoding="utf-8"?>
<SchemeTable Partition="54ed79f5-11c0-4f74-b8b0-793d596699d6" ID="55ecf577-db07-4c32-af7b-0acb48a2b769" Name="PnrPowerAttorneyTypes" Group="Pnr" InstanceType="Cards" ContentType="Entries">
	<Description>Справочник Типы доверенностей</Description>
	<SchemeComplexColumn IsSystem="true" IsPermanent="true" IsSealed="true" ID="55ecf577-db07-0032-2000-0acb48a2b769" Name="ID" Type="Reference(Typified) Not Null" ReferencedTable="1074eadd-21d7-4925-98c8-40d1e5f0ca0e">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="55ecf577-db07-0132-4000-0acb48a2b769" Name="ID" Type="Guid Not Null" ReferencedColumn="9a58123b-b2e9-4137-9c6c-5dab0ec02747" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn ID="21aa8f6f-14bc-4567-992d-d90974e559c2" Name="Name" Type="String(Max) Null">
		<Description>Название</Description>
	</SchemePhysicalColumn>
	<SchemePrimaryKey IsSystem="true" IsPermanent="true" IsSealed="true" ID="55ecf577-db07-0032-5000-0acb48a2b769" Name="pk_PnrPowerAttorneyTypes" IsClustered="true">
		<SchemeIndexedColumn Column="55ecf577-db07-0132-4000-0acb48a2b769" />
	</SchemePrimaryKey>
</SchemeTable>