<?xml version="1.0" encoding="utf-8"?>
<SchemeTable Partition="c3b14969-a553-4005-9a07-e9705c15b1e1" ID="6817bc39-0f53-4628-96ce-0b769572a42d" Name="FdStageTemplate_ErrorsVirtual" Group="FdDiagnostic" IsVirtual="true" InstanceType="Cards" ContentType="Collections">
	<SchemeComplexColumn IsSystem="true" IsPermanent="true" IsSealed="true" ID="6817bc39-0f53-0028-2000-0b769572a42d" Name="ID" Type="Reference(Typified) Not Null" ReferencedTable="1074eadd-21d7-4925-98c8-40d1e5f0ca0e">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="6817bc39-0f53-0128-4000-0b769572a42d" Name="ID" Type="Guid Not Null" ReferencedColumn="9a58123b-b2e9-4137-9c6c-5dab0ec02747" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn IsSystem="true" IsPermanent="true" IsSealed="true" ID="6817bc39-0f53-0028-3100-0b769572a42d" Name="RowID" Type="Guid Not Null" />
	<SchemePhysicalColumn ID="8dafe54a-e8d7-436d-b4a6-1135816c2769" Name="FieldName" Type="String(256) Null" />
	<SchemePhysicalColumn ID="0951e5c3-2a7b-4d21-8983-9e600e69afef" Name="Description" Type="String(Max) Not Null" />
	<SchemePhysicalColumn ID="75c7aaf8-981f-4f83-89bf-dc85616cab76" Name="FieldValue" Type="String(Max) Null" />
	<SchemePhysicalColumn ID="855e4c05-b2ef-4cad-8dd6-04cf8466324e" Name="GroupName" Type="String(256) Null" />
	<SchemePrimaryKey IsSystem="true" IsPermanent="true" IsSealed="true" ID="6817bc39-0f53-0028-5000-0b769572a42d" Name="pk_FdStageTemplate_ErrorsVirtual">
		<SchemeIndexedColumn Column="6817bc39-0f53-0028-3100-0b769572a42d" />
	</SchemePrimaryKey>
	<SchemeIndex IsSystem="true" IsPermanent="true" IsSealed="true" ID="6817bc39-0f53-0028-7000-0b769572a42d" Name="idx_FdStageTemplate_ErrorsVirtual_ID" IsClustered="true">
		<SchemeIndexedColumn Column="6817bc39-0f53-0128-4000-0b769572a42d" />
	</SchemeIndex>
</SchemeTable>