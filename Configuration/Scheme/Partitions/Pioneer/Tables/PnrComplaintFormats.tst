<?xml version="1.0" encoding="utf-8"?>
<SchemeTable Partition="54ed79f5-11c0-4f74-b8b0-793d596699d6" ID="36de7b98-236e-4cf4-b77d-e16efa400939" Name="PnrComplaintFormats" Group="Pnr" InstanceType="Cards" ContentType="Entries">
	<Description>Справочник Форматы рекламации</Description>
	<SchemeComplexColumn IsSystem="true" IsPermanent="true" IsSealed="true" ID="36de7b98-236e-00f4-2000-016efa400939" Name="ID" Type="Reference(Typified) Not Null" ReferencedTable="1074eadd-21d7-4925-98c8-40d1e5f0ca0e">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="36de7b98-236e-01f4-4000-016efa400939" Name="ID" Type="Guid Not Null" ReferencedColumn="9a58123b-b2e9-4137-9c6c-5dab0ec02747" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn ID="076e7d9e-f973-4b13-afaa-ed6165032c77" Name="Name" Type="String(Max) Null">
		<Description>Название</Description>
	</SchemePhysicalColumn>
	<SchemePrimaryKey IsSystem="true" IsPermanent="true" IsSealed="true" ID="36de7b98-236e-00f4-5000-016efa400939" Name="pk_PnrComplaintFormats" IsClustered="true">
		<SchemeIndexedColumn Column="36de7b98-236e-01f4-4000-016efa400939" />
	</SchemePrimaryKey>
</SchemeTable>