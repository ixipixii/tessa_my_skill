<?xml version="1.0" encoding="utf-8"?>
<SchemeTable Partition="c3b14969-a553-4005-9a07-e9705c15b1e1" ID="22949891-5a1b-4660-90ff-3be20299851b" Name="FdAdditionalApprovalUsers" Group="Fd" InstanceType="Tasks" ContentType="Collections">
	<SchemeComplexColumn IsSystem="true" IsPermanent="true" IsSealed="true" ID="22949891-5a1b-0060-2000-0be20299851b" Name="ID" Type="Reference(Typified) Not Null" ReferencedTable="5bfa9936-bb5a-4e8f-89a9-180bfd8f75f8">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="22949891-5a1b-0160-4000-0be20299851b" Name="ID" Type="Guid Not Null" ReferencedColumn="5bfa9936-bb5a-008f-3100-080bfd8f75f8" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn IsSystem="true" IsPermanent="true" IsSealed="true" ID="22949891-5a1b-0060-3100-0be20299851b" Name="RowID" Type="Guid Not Null" />
	<SchemeComplexColumn ID="c02c2e7a-8504-43c9-8f83-b5ef0a0ac626" Name="Role" Type="Reference(Typified) Null" ReferencedTable="81f6010b-9641-4aa5-8897-b8e8603fbf4b" WithForeignKey="false">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="c02c2e7a-8504-00c9-4000-05ef0a0ac626" Name="RoleID" Type="Guid Null" ReferencedColumn="81f6010b-9641-01a5-4000-08e8603fbf4b" />
		<SchemeReferencingColumn ID="78a5bce1-18f9-444e-b617-c18ecae34ab4" Name="RoleName" Type="String(128) Null" ReferencedColumn="616d6b2e-35d5-424d-846b-618eb25962d0" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn ID="dd6bb0f3-2262-4bc9-9039-561e9eb60dc5" Name="Order" Type="Int32 Not Null">
		<SchemeDefaultConstraint IsPermanent="true" ID="24d65ddc-65ad-4458-bcc6-d8c825f0c371" Name="df_FdAdditionalApprovalUsers_Order" Value="0" />
	</SchemePhysicalColumn>
	<SchemePrimaryKey IsSystem="true" IsPermanent="true" IsSealed="true" ID="22949891-5a1b-0060-5000-0be20299851b" Name="pk_FdAdditionalApprovalUsers">
		<SchemeIndexedColumn Column="22949891-5a1b-0060-3100-0be20299851b" />
	</SchemePrimaryKey>
	<SchemeIndex IsSystem="true" IsPermanent="true" IsSealed="true" ID="22949891-5a1b-0060-7000-0be20299851b" Name="idx_FdAdditionalApprovalUsers_ID" IsClustered="true">
		<SchemeIndexedColumn Column="22949891-5a1b-0160-4000-0be20299851b" />
	</SchemeIndex>
</SchemeTable>