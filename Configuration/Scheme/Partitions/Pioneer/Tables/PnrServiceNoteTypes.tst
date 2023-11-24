<?xml version="1.0" encoding="utf-8"?>
<SchemeTable Partition="54ed79f5-11c0-4f74-b8b0-793d596699d6" ID="9f2d4747-edad-421c-a624-3ea3f0720b0f" Name="PnrServiceNoteTypes" Group="Pnr" InstanceType="Cards" ContentType="Entries">
	<Description>Типы служебных записок</Description>
	<SchemeComplexColumn IsSystem="true" IsPermanent="true" IsSealed="true" ID="9f2d4747-edad-001c-2000-0ea3f0720b0f" Name="ID" Type="Reference(Typified) Not Null" ReferencedTable="1074eadd-21d7-4925-98c8-40d1e5f0ca0e">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="9f2d4747-edad-011c-4000-0ea3f0720b0f" Name="ID" Type="Guid Not Null" ReferencedColumn="9a58123b-b2e9-4137-9c6c-5dab0ec02747" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn ID="5a48e37a-d79b-4cfb-93cb-af1b2fc0a809" Name="Name" Type="String(Max) Null">
		<Description>Название</Description>
	</SchemePhysicalColumn>
	<SchemePrimaryKey IsSystem="true" IsPermanent="true" IsSealed="true" ID="9f2d4747-edad-001c-5000-0ea3f0720b0f" Name="pk_PnrServiceNoteTypes" IsClustered="true">
		<SchemeIndexedColumn Column="9f2d4747-edad-011c-4000-0ea3f0720b0f" />
	</SchemePrimaryKey>
</SchemeTable>