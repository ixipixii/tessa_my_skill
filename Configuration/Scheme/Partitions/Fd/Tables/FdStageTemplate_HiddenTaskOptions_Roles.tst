<?xml version="1.0" encoding="utf-8"?>
<SchemeTable Partition="c3b14969-a553-4005-9a07-e9705c15b1e1" ID="48621767-72dc-4fb5-bce2-a3d9744f22c6" Name="FdStageTemplate_HiddenTaskOptions_Roles" Group="Fd" InstanceType="Cards" ContentType="Collections">
	<SchemeComplexColumn IsSystem="true" IsPermanent="true" IsSealed="true" ID="48621767-72dc-00b5-2000-03d9744f22c6" Name="ID" Type="Reference(Typified) Not Null" ReferencedTable="1074eadd-21d7-4925-98c8-40d1e5f0ca0e">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="48621767-72dc-01b5-4000-03d9744f22c6" Name="ID" Type="Guid Not Null" ReferencedColumn="9a58123b-b2e9-4137-9c6c-5dab0ec02747" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn IsSystem="true" IsPermanent="true" IsSealed="true" ID="48621767-72dc-00b5-3100-03d9744f22c6" Name="RowID" Type="Guid Not Null" />
	<SchemeComplexColumn ID="b857aa5b-cfa5-46c9-b120-f0e8955ec610" Name="Role" Type="Reference(Typified) Not Null" ReferencedTable="81f6010b-9641-4aa5-8897-b8e8603fbf4b" WithForeignKey="false">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="b857aa5b-cfa5-00c9-4000-00e8955ec610" Name="RoleID" Type="Guid Not Null" ReferencedColumn="81f6010b-9641-01a5-4000-08e8603fbf4b" />
		<SchemeReferencingColumn ID="4c1fb9e7-c676-44d2-b19a-52ba188854fa" Name="RoleName" Type="String(128) Not Null" ReferencedColumn="616d6b2e-35d5-424d-846b-618eb25962d0" />
	</SchemeComplexColumn>
	<SchemeComplexColumn ID="4d1ee5ee-5586-47f0-836d-f5d794222f60" Name="Parent" Type="Reference(Typified) Not Null" ReferencedTable="65bb59e8-3f1a-4a4a-8a2c-f6c924546bbc" IsReferenceToOwner="true">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="4d1ee5ee-5586-00f0-4000-05d794222f60" Name="ParentRowID" Type="Guid Not Null" ReferencedColumn="65bb59e8-3f1a-004a-3100-06c924546bbc" />
	</SchemeComplexColumn>
	<SchemePrimaryKey IsSystem="true" IsPermanent="true" IsSealed="true" ID="48621767-72dc-00b5-5000-03d9744f22c6" Name="pk_FdStageTemplate_HiddenTaskOptions_Roles">
		<SchemeIndexedColumn Column="48621767-72dc-00b5-3100-03d9744f22c6" />
	</SchemePrimaryKey>
	<SchemeIndex IsSystem="true" IsPermanent="true" IsSealed="true" ID="48621767-72dc-00b5-7000-03d9744f22c6" Name="idx_FdStageTemplate_HiddenTaskOptions_Roles_ID" IsClustered="true">
		<SchemeIndexedColumn Column="48621767-72dc-01b5-4000-03d9744f22c6" />
	</SchemeIndex>
</SchemeTable>