<?xml version="1.0" encoding="utf-8"?>
<SchemeTable Partition="54ed79f5-11c0-4f74-b8b0-793d596699d6" ID="acb2c154-f00a-4dbd-adb0-8b77ca861ae2" Name="PnrImplementationStages" Group="Pnr" InstanceType="Cards" ContentType="Entries">
	<Description>Стадии реализации</Description>
	<SchemeComplexColumn IsSystem="true" IsPermanent="true" IsSealed="true" ID="acb2c154-f00a-00bd-2000-0b77ca861ae2" Name="ID" Type="Reference(Typified) Not Null" ReferencedTable="1074eadd-21d7-4925-98c8-40d1e5f0ca0e">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="acb2c154-f00a-01bd-4000-0b77ca861ae2" Name="ID" Type="Guid Not Null" ReferencedColumn="9a58123b-b2e9-4137-9c6c-5dab0ec02747" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn ID="32b4bb15-8adb-4851-b9c6-d990cb2e3396" Name="Name" Type="String(Max) Null">
		<Description>Наименование</Description>
	</SchemePhysicalColumn>
	<SchemeComplexColumn ID="4ed627a0-2af9-4816-aa1e-f4017f8ff027" Name="ParentStage" Type="Reference(Typified) Null" ReferencedTable="acb2c154-f00a-4dbd-adb0-8b77ca861ae2">
		<Description>Родительская стадия</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="4ed627a0-2af9-0016-4000-04017f8ff027" Name="ParentStageID" Type="Guid Null" ReferencedColumn="acb2c154-f00a-01bd-4000-0b77ca861ae2" />
		<SchemeReferencingColumn ID="de8225ce-d259-498d-9568-9a714493ab56" Name="ParentStageName" Type="String(Max) Null" ReferencedColumn="32b4bb15-8adb-4851-b9c6-d990cb2e3396" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn ID="711d08ff-544b-42a6-a733-570d83ccb7a6" Name="IsExcludeFromSelection" Type="Boolean Null">
		<Description>Исключить из выбора</Description>
	</SchemePhysicalColumn>
	<SchemePrimaryKey IsSystem="true" IsPermanent="true" IsSealed="true" ID="acb2c154-f00a-00bd-5000-0b77ca861ae2" Name="pk_PnrImplementationStages" IsClustered="true">
		<SchemeIndexedColumn Column="acb2c154-f00a-01bd-4000-0b77ca861ae2" />
	</SchemePrimaryKey>
</SchemeTable>