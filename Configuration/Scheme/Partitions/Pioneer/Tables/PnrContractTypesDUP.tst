<?xml version="1.0" encoding="utf-8"?>
<SchemeTable Partition="54ed79f5-11c0-4f74-b8b0-793d596699d6" ID="fcf98667-97d7-4863-bd5e-4096fb3e7340" Name="PnrContractTypesDUP" Group="Pnr" InstanceType="Cards" ContentType="Entries">
	<Description>Справочник Типы договора ДУП</Description>
	<SchemeComplexColumn IsSystem="true" IsPermanent="true" IsSealed="true" ID="fcf98667-97d7-0063-2000-0096fb3e7340" Name="ID" Type="Reference(Typified) Not Null" ReferencedTable="1074eadd-21d7-4925-98c8-40d1e5f0ca0e">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="fcf98667-97d7-0163-4000-0096fb3e7340" Name="ID" Type="Guid Not Null" ReferencedColumn="9a58123b-b2e9-4137-9c6c-5dab0ec02747" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn ID="870c1bc8-5fcd-4b05-83a9-239febcde3c3" Name="Name" Type="String(Max) Null">
		<Description>Наименование</Description>
	</SchemePhysicalColumn>
	<SchemePrimaryKey IsSystem="true" IsPermanent="true" IsSealed="true" ID="fcf98667-97d7-0063-5000-0096fb3e7340" Name="pk_PnrContractTypesDUP" IsClustered="true">
		<SchemeIndexedColumn Column="fcf98667-97d7-0163-4000-0096fb3e7340" />
	</SchemePrimaryKey>
</SchemeTable>