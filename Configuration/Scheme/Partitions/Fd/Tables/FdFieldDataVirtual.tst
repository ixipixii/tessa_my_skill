<?xml version="1.0" encoding="utf-8"?>
<SchemeTable Partition="c3b14969-a553-4005-9a07-e9705c15b1e1" ID="d30606d0-3bdd-4c18-b6ee-5d8d19cfb333" Name="FdFieldDataVirtual" Group="Fd Fields" IsVirtual="true" InstanceType="Cards" ContentType="Entries">
	<SchemeComplexColumn IsSystem="true" IsPermanent="true" IsSealed="true" ID="d30606d0-3bdd-0018-2000-0d8d19cfb333" Name="ID" Type="Reference(Typified) Not Null" ReferencedTable="1074eadd-21d7-4925-98c8-40d1e5f0ca0e">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="d30606d0-3bdd-0118-4000-0d8d19cfb333" Name="ID" Type="Guid Not Null" ReferencedColumn="9a58123b-b2e9-4137-9c6c-5dab0ec02747" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn ID="d21b0bc8-ff67-48ee-8d18-cff15549a309" Name="Name" Type="String(Max) Not Null" />
	<SchemePhysicalColumn ID="70ca101a-e2e3-4ca6-bf91-d8df5cd78ecd" Name="ControlTypeID" Type="Guid Null" />
	<SchemePhysicalColumn ID="2e0e0121-ab28-4b59-bb45-4974a54ab3e0" Name="ControlTypeName" Type="String(Max) Null" />
	<SchemePhysicalColumn ID="3063c6fd-b546-4368-86f1-6d78c7238201" Name="SectionID" Type="Guid Null" />
	<SchemePhysicalColumn ID="fb17f159-853e-461a-849d-47dd60090e8a" Name="SectionName" Type="String(Max) Null" />
	<SchemePhysicalColumn ID="577c0a40-af09-40f6-b262-dc64ee10649b" Name="PhysicalColumnID" Type="Guid Null" />
	<SchemePhysicalColumn ID="f65e0789-478e-42d3-bb01-b51ebffabb37" Name="ComplexColumnID" Type="Guid Null" />
	<SchemePhysicalColumn ID="5dbe1a72-b58a-41b5-9d89-6755023f4938" Name="ColumnName" Type="String(Max) Null" />
	<SchemePhysicalColumn ID="4ce6bdbf-395e-418f-a59f-97a3c9daace0" Name="ReferencedTableID" Type="Guid Null" />
	<SchemePhysicalColumn ID="333d1847-c2da-4fd1-97e0-c4f6426be594" Name="ReferencedTableName" Type="String(Max) Null" />
	<SchemePhysicalColumn ID="fc5e528e-67d6-4b8c-80f9-70c08bcbdddd" Name="SpecialMode" Type="Int32 Null" />
	<SchemePrimaryKey IsSystem="true" IsPermanent="true" IsSealed="true" ID="d30606d0-3bdd-0018-5000-0d8d19cfb333" Name="pk_FdFieldDataVirtual" IsClustered="true">
		<SchemeIndexedColumn Column="d30606d0-3bdd-0118-4000-0d8d19cfb333" />
	</SchemePrimaryKey>
</SchemeTable>