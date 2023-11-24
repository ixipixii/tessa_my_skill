<?xml version="1.0" encoding="utf-8"?>
<SchemeTable Partition="c3b14969-a553-4005-9a07-e9705c15b1e1" ID="d105dac5-a5cd-48a1-bc18-c04a6a214d8b" Name="FdTask_DelegateRoles" Group="Fd" InstanceType="Tasks" ContentType="Collections">
	<Description>Список сотрудников для делегирования задания</Description>
	<SchemeComplexColumn IsSystem="true" IsPermanent="true" IsSealed="true" ID="d105dac5-a5cd-00a1-2000-004a6a214d8b" Name="ID" Type="Reference(Typified) Not Null" ReferencedTable="5bfa9936-bb5a-4e8f-89a9-180bfd8f75f8">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="d105dac5-a5cd-01a1-4000-004a6a214d8b" Name="ID" Type="Guid Not Null" ReferencedColumn="5bfa9936-bb5a-008f-3100-080bfd8f75f8" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn IsSystem="true" IsPermanent="true" IsSealed="true" ID="d105dac5-a5cd-00a1-3100-004a6a214d8b" Name="RowID" Type="Guid Not Null" />
	<SchemeComplexColumn ID="2ac9a6f7-da42-497e-a46b-02dd3150cb42" Name="Delegate" Type="Reference(Typified) Not Null" ReferencedTable="81f6010b-9641-4aa5-8897-b8e8603fbf4b" WithForeignKey="false">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="2ac9a6f7-da42-007e-4000-02dd3150cb42" Name="DelegateID" Type="Guid Not Null" ReferencedColumn="81f6010b-9641-01a5-4000-08e8603fbf4b" />
		<SchemeReferencingColumn ID="f9eea527-0b5b-4e6a-bfed-3ad2041fba3e" Name="DelegateName" Type="String(128) Not Null" ReferencedColumn="616d6b2e-35d5-424d-846b-618eb25962d0" />
	</SchemeComplexColumn>
	<SchemePrimaryKey IsSystem="true" IsPermanent="true" IsSealed="true" ID="d105dac5-a5cd-00a1-5000-004a6a214d8b" Name="pk_FdTask_DelegateRoles">
		<SchemeIndexedColumn Column="d105dac5-a5cd-00a1-3100-004a6a214d8b" />
	</SchemePrimaryKey>
	<SchemeIndex IsSystem="true" IsPermanent="true" IsSealed="true" ID="d105dac5-a5cd-00a1-7000-004a6a214d8b" Name="idx_FdTask_DelegateRoles_ID" IsClustered="true">
		<SchemeIndexedColumn Column="d105dac5-a5cd-01a1-4000-004a6a214d8b" />
	</SchemeIndex>
</SchemeTable>