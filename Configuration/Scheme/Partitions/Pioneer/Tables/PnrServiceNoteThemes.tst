<?xml version="1.0" encoding="utf-8"?>
<SchemeTable Partition="54ed79f5-11c0-4f74-b8b0-793d596699d6" ID="fd957afa-ded7-48d5-bb33-9a7db3960247" Name="PnrServiceNoteThemes" Group="Pnr" InstanceType="Cards" ContentType="Entries">
	<Description>Справочник Тематики служебных записок</Description>
	<SchemeComplexColumn IsSystem="true" IsPermanent="true" IsSealed="true" ID="fd957afa-ded7-00d5-2000-0a7db3960247" Name="ID" Type="Reference(Typified) Not Null" ReferencedTable="1074eadd-21d7-4925-98c8-40d1e5f0ca0e">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="fd957afa-ded7-01d5-4000-0a7db3960247" Name="ID" Type="Guid Not Null" ReferencedColumn="9a58123b-b2e9-4137-9c6c-5dab0ec02747" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn ID="8c13a21c-7143-4fa5-9c2e-3f5e3e5d0710" Name="Name" Type="String(Max) Null">
		<Description>Название</Description>
	</SchemePhysicalColumn>
	<SchemeComplexColumn ID="8e63169f-37c5-4a16-a966-bb03d6157fa4" Name="Type" Type="Reference(Typified) Null" ReferencedTable="9f2d4747-edad-421c-a624-3ea3f0720b0f">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="8e63169f-37c5-0016-4000-0b03d6157fa4" Name="TypeID" Type="Guid Null" ReferencedColumn="9f2d4747-edad-011c-4000-0ea3f0720b0f" />
		<SchemeReferencingColumn ID="58de2bde-d5f9-48f0-832a-9249d94593cc" Name="TypeName" Type="String(Max) Null" ReferencedColumn="5a48e37a-d79b-4cfb-93cb-af1b2fc0a809" />
	</SchemeComplexColumn>
	<SchemePrimaryKey IsSystem="true" IsPermanent="true" IsSealed="true" ID="fd957afa-ded7-00d5-5000-0a7db3960247" Name="pk_PnrServiceNoteThemes" IsClustered="true">
		<SchemeIndexedColumn Column="fd957afa-ded7-01d5-4000-0a7db3960247" />
	</SchemePrimaryKey>
</SchemeTable>