<?xml version="1.0" encoding="utf-8"?>
<SchemeTable Partition="c3b14969-a553-4005-9a07-e9705c15b1e1" ID="9a51283d-02ff-4a86-9d88-144addad3805" Name="FdCardTypeFieldVirtual" Group="Fd Criteria" IsVirtual="true" InstanceType="Cards" ContentType="Entries">
	<SchemeComplexColumn IsSystem="true" IsPermanent="true" IsSealed="true" ID="9a51283d-02ff-0086-2000-044addad3805" Name="ID" Type="Reference(Typified) Not Null" ReferencedTable="1074eadd-21d7-4925-98c8-40d1e5f0ca0e">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="9a51283d-02ff-0186-4000-044addad3805" Name="ID" Type="Guid Not Null" ReferencedColumn="9a58123b-b2e9-4137-9c6c-5dab0ec02747" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn ID="09b3454b-d9d3-4c6b-9ebf-12cfc1d911d5" Name="Caption" Type="String(128) Not Null" />
	<SchemePhysicalColumn ID="5268e016-ff47-4a2b-ae09-631d68ae6455" Name="SectionTypeName" Type="String(128) Not Null" />
	<SchemePhysicalColumn ID="cb758997-e875-489a-9854-89398204eb1e" Name="DataTypeID" Type="Guid Not Null" />
	<SchemePhysicalColumn ID="87f02e9b-5552-433b-8862-35dcd83de844" Name="DataTypeCaption" Type="String(128) Not Null" />
	<SchemePhysicalColumn ID="c1764def-bad2-4a9d-81b1-384c56013aa1" Name="SectionID" Type="Guid Not Null" />
	<SchemePhysicalColumn ID="65d67953-e0ef-4c96-9a70-cbfc42518985" Name="SectionName" Type="String(128) Not Null" />
	<SchemePhysicalColumn ID="d245f7a9-a177-4622-a71e-f703bcb8aaae" Name="ComplexColumnID" Type="Guid Null" />
	<SchemePhysicalColumn ID="01ceb5cc-dcba-4bdd-a748-d46092bcb0e4" Name="PhysicalColumnID" Type="Guid Null" />
	<SchemePhysicalColumn ID="16570f16-6fb3-4b9f-926d-2645823096fe" Name="PhysicalColumnName" Type="String(128) Null" />
	<SchemePhysicalColumn ID="30af168c-855d-467c-b867-4f324c68c760" Name="ComplexColumnName" Type="String(128) Null" />
	<SchemePhysicalColumn ID="494ec73f-bef9-45eb-bef3-77fa2a0a8e9b" Name="ComplexColumnReferencedTableID" Type="Guid Null" />
	<SchemePhysicalColumn ID="711f51a2-4d38-41bd-b7b1-a0dd5be05505" Name="ComplexColumnReferencedTableName" Type="String(128) Null" />
	<SchemePrimaryKey IsSystem="true" IsPermanent="true" IsSealed="true" ID="9a51283d-02ff-0086-5000-044addad3805" Name="pk_FdCardTypeFieldVirtual" IsClustered="true">
		<SchemeIndexedColumn Column="9a51283d-02ff-0186-4000-044addad3805" />
	</SchemePrimaryKey>
</SchemeTable>