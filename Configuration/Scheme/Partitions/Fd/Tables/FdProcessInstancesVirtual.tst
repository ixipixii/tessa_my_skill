<?xml version="1.0" encoding="utf-8"?>
<SchemeTable Partition="c3b14969-a553-4005-9a07-e9705c15b1e1" ID="ce0e5c9d-a8d2-498a-b8a0-d9bed35d5735" Name="FdProcessInstancesVirtual" Group="Fd" IsVirtual="true" InstanceType="Cards" ContentType="Collections">
	<SchemeComplexColumn IsSystem="true" IsPermanent="true" IsSealed="true" ID="ce0e5c9d-a8d2-008a-2000-09bed35d5735" Name="ID" Type="Reference(Typified) Not Null" ReferencedTable="1074eadd-21d7-4925-98c8-40d1e5f0ca0e">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="ce0e5c9d-a8d2-018a-4000-09bed35d5735" Name="ID" Type="Guid Not Null" ReferencedColumn="9a58123b-b2e9-4137-9c6c-5dab0ec02747" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn IsSystem="true" IsPermanent="true" IsSealed="true" ID="ce0e5c9d-a8d2-008a-3100-09bed35d5735" Name="RowID" Type="Guid Not Null" />
	<SchemeComplexColumn ID="5b5168de-9acd-4b4f-9881-dd78d5497454" Name="BasedOnProcessTemplate" Type="Reference(Typified) Not Null" ReferencedTable="7d88534f-8bce-4084-a150-64e9594d77c8">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="5b5168de-9acd-004f-4000-0d78d5497454" Name="BasedOnProcessTemplateID" Type="Guid Not Null" ReferencedColumn="7d88534f-8bce-0184-4000-04e9594d77c8" />
		<SchemeReferencingColumn ID="7fca78f7-2793-4c6b-bf5a-39bc2273851a" Name="BasedOnProcessTemplateName" Type="String(255) Not Null" ReferencedColumn="40395010-1055-40c2-bc1e-a095c16b3517" />
	</SchemeComplexColumn>
	<SchemeComplexColumn ID="0ac92393-c1a6-43ca-9e19-09559dbb18b3" Name="Author" Type="Reference(Typified) Null" ReferencedTable="6c977939-bbfc-456f-a133-f1c2244e3cc3">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="0ac92393-c1a6-00ca-4000-09559dbb18b3" Name="AuthorID" Type="Guid Null" ReferencedColumn="6c977939-bbfc-016f-4000-01c2244e3cc3" />
		<SchemeReferencingColumn ID="c295c8a3-98eb-472d-9ade-d084af132033" Name="AuthorName" Type="String(128) Null" ReferencedColumn="1782f76a-4743-4aa4-920c-7edaee860964" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn ID="b95880f6-48e2-4048-a4de-c229e8fa973b" Name="Cycle" Type="Int16 Not Null">
		<SchemeDefaultConstraint IsPermanent="true" ID="a5ad5ea9-1c45-44b7-bd0d-df5901c51b02" Name="df_FdProcessInstancesVirtual_Cycle" Value="0" />
	</SchemePhysicalColumn>
	<SchemeComplexColumn ID="3a2edbab-8641-45d2-8a74-932681a9950a" Name="State" Type="Reference(Typified) Not Null" ReferencedTable="44c3a9b3-d245-445c-a1fe-2a21ce021e1e">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="3a2edbab-8641-00d2-4000-032681a9950a" Name="StateID" Type="Int16 Not Null" ReferencedColumn="3ba53faf-6661-4352-8c57-35e0b6c0dc3b" />
		<SchemeReferencingColumn ID="42268940-8f77-4b61-bdbf-fee2fb002495" Name="StateName" Type="String(128) Not Null" ReferencedColumn="13bffd8e-b32f-4243-b8f9-ec07da0ef5c1" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn ID="fef1890e-a05f-4191-b66e-a5754ad52ec4" Name="Order" Type="Int32 Not Null">
		<SchemeDefaultConstraint IsPermanent="true" ID="3eddb660-39ec-4f99-83c2-59e8aa527d1e" Name="df_FdProcessInstancesVirtual_Order" Value="0" />
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="25a03f66-d8c2-4487-9c78-c6cdac64db76" Name="Name" Type="String(255) Not Null" />
	<SchemePhysicalColumn ID="a126e1a2-1bd0-4af4-b915-e562f37178e8" Name="StartDate" Type="DateTime Not Null" />
	<SchemePhysicalColumn ID="4c72acb7-c703-47b6-8fe8-5c7fb0712f85" Name="NeedRebuild" Type="Boolean Not Null">
		<SchemeDefaultConstraint IsPermanent="true" ID="5bf34d5f-bc48-47e4-9760-168b038a919a" Name="df_FdProcessInstancesVirtual_NeedRebuild" Value="false" />
	</SchemePhysicalColumn>
	<SchemeComplexColumn ID="26003461-8946-40f4-82a7-87b4de592ddd" Name="ParentProcessInstance" Type="Reference(Typified) Null" ReferencedTable="2114510a-e165-4491-afcc-1756400e30a0" WithForeignKey="false">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="26003461-8946-00f4-4000-07b4de592ddd" Name="ParentProcessInstanceRowID" Type="Guid Null" ReferencedColumn="2114510a-e165-0091-3100-0756400e30a0" />
		<SchemeReferencingColumn ID="d63f6563-6be6-457d-b6fe-9885d12522ed" Name="ParentProcessInstanceName" Type="String(255) Null" ReferencedColumn="1279d223-16d3-45ae-95ed-be8da5b802d9" />
	</SchemeComplexColumn>
	<SchemePrimaryKey IsSystem="true" IsPermanent="true" IsSealed="true" ID="ce0e5c9d-a8d2-008a-5000-09bed35d5735" Name="pk_FdProcessInstancesVirtual">
		<SchemeIndexedColumn Column="ce0e5c9d-a8d2-008a-3100-09bed35d5735" />
	</SchemePrimaryKey>
	<SchemeIndex IsSystem="true" IsPermanent="true" IsSealed="true" ID="ce0e5c9d-a8d2-008a-7000-09bed35d5735" Name="idx_FdProcessInstancesVirtual_ID" IsClustered="true">
		<SchemeIndexedColumn Column="ce0e5c9d-a8d2-018a-4000-09bed35d5735" />
	</SchemeIndex>
</SchemeTable>