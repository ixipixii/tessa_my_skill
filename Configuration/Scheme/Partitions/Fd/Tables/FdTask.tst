<?xml version="1.0" encoding="utf-8"?>
<SchemeTable Partition="c3b14969-a553-4005-9a07-e9705c15b1e1" ID="41932ab3-2575-42ad-ad4e-1dbd29162372" Name="FdTask" Group="Fd" InstanceType="Tasks" ContentType="Entries">
	<SchemeComplexColumn IsSystem="true" IsPermanent="true" IsSealed="true" ID="41932ab3-2575-00ad-2000-0dbd29162372" Name="ID" Type="Reference(Typified) Not Null" ReferencedTable="5bfa9936-bb5a-4e8f-89a9-180bfd8f75f8">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="41932ab3-2575-01ad-4000-0dbd29162372" Name="ID" Type="Guid Not Null" ReferencedColumn="5bfa9936-bb5a-008f-3100-080bfd8f75f8" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn ID="acc4d435-1e78-4a79-8b07-0c74dac672e6" Name="Comment" Type="String(Max) Null" />
	<SchemeComplexColumn ID="f9ce1cc0-7c38-4d9f-a1f9-0007048b31d4" Name="Delegate" Type="Reference(Typified) Null" ReferencedTable="81f6010b-9641-4aa5-8897-b8e8603fbf4b" WithForeignKey="false">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="f9ce1cc0-7c38-009f-4000-0007048b31d4" Name="DelegateID" Type="Guid Null" ReferencedColumn="81f6010b-9641-01a5-4000-08e8603fbf4b" />
		<SchemeReferencingColumn ID="c3b05848-8b4a-493f-b737-9d1e2b442533" Name="DelegateName" Type="String(128) Null" ReferencedColumn="616d6b2e-35d5-424d-846b-618eb25962d0" />
	</SchemeComplexColumn>
	<SchemePrimaryKey IsSystem="true" IsPermanent="true" IsSealed="true" ID="41932ab3-2575-00ad-5000-0dbd29162372" Name="pk_FdTask" IsClustered="true">
		<SchemeIndexedColumn Column="41932ab3-2575-01ad-4000-0dbd29162372" />
	</SchemePrimaryKey>
</SchemeTable>