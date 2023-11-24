<?xml version="1.0" encoding="utf-8"?>
<SchemeTable Partition="c3b14969-a553-4005-9a07-e9705c15b1e1" ID="8fca79d1-6d3f-4c78-ac7c-837be5e62fe5" Name="FdProcessTemplate_CardStates" Group="Fd" InstanceType="Cards" ContentType="Collections">
	<Description>Состояния карточки, в которых применим процесс</Description>
	<SchemeComplexColumn IsSystem="true" IsPermanent="true" IsSealed="true" ID="8fca79d1-6d3f-0078-2000-037be5e62fe5" Name="ID" Type="Reference(Typified) Not Null" ReferencedTable="1074eadd-21d7-4925-98c8-40d1e5f0ca0e">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="8fca79d1-6d3f-0178-4000-037be5e62fe5" Name="ID" Type="Guid Not Null" ReferencedColumn="9a58123b-b2e9-4137-9c6c-5dab0ec02747" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn IsSystem="true" IsPermanent="true" IsSealed="true" ID="8fca79d1-6d3f-0078-3100-037be5e62fe5" Name="RowID" Type="Guid Not Null" />
	<SchemeComplexColumn ID="af5a0dde-187a-4cb6-8a94-a6388f8f58ca" Name="State" Type="Reference(Typified) Not Null" ReferencedTable="c5e015fe-1b55-4a18-8787-074b5d2ec80c">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="af5a0dde-187a-00b6-4000-06388f8f58ca" Name="StateID" Type="Guid Not Null" ReferencedColumn="c5e015fe-1b55-0118-4000-074b5d2ec80c" />
		<SchemeReferencingColumn ID="4e532bb3-78a5-4c47-951c-5dc729051ac1" Name="StateName" Type="String(128) Not Null" ReferencedColumn="09d1dfdb-262f-4b6e-b14d-64e5f51b7fa7" />
	</SchemeComplexColumn>
	<SchemePrimaryKey IsSystem="true" IsPermanent="true" IsSealed="true" ID="8fca79d1-6d3f-0078-5000-037be5e62fe5" Name="pk_FdProcessTemplate_CardStates">
		<SchemeIndexedColumn Column="8fca79d1-6d3f-0078-3100-037be5e62fe5" />
	</SchemePrimaryKey>
	<SchemeIndex IsSystem="true" IsPermanent="true" IsSealed="true" ID="8fca79d1-6d3f-0078-7000-037be5e62fe5" Name="idx_FdProcessTemplate_CardStates_ID" IsClustered="true">
		<SchemeIndexedColumn Column="8fca79d1-6d3f-0178-4000-037be5e62fe5" />
	</SchemeIndex>
</SchemeTable>