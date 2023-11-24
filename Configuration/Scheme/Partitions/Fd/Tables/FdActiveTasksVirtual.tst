<?xml version="1.0" encoding="utf-8"?>
<SchemeTable Partition="c3b14969-a553-4005-9a07-e9705c15b1e1" ID="00343461-1765-436c-a452-0c599e8298a1" Name="FdActiveTasksVirtual" Group="Fd" IsVirtual="true" InstanceType="Cards" ContentType="Collections">
	<SchemeComplexColumn IsSystem="true" IsPermanent="true" IsSealed="true" ID="00343461-1765-006c-2000-0c599e8298a1" Name="ID" Type="Reference(Typified) Not Null" ReferencedTable="1074eadd-21d7-4925-98c8-40d1e5f0ca0e">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="00343461-1765-016c-4000-0c599e8298a1" Name="ID" Type="Guid Not Null" ReferencedColumn="9a58123b-b2e9-4137-9c6c-5dab0ec02747" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn IsSystem="true" IsPermanent="true" IsSealed="true" ID="00343461-1765-006c-3100-0c599e8298a1" Name="RowID" Type="Guid Not Null" />
	<SchemePhysicalColumn ID="a6bfc4f5-cb33-4f79-89f8-735c16dc3ed0" Name="TaskID" Type="Guid Not Null">
		<Description>Ссылка на активное задание</Description>
	</SchemePhysicalColumn>
	<SchemeComplexColumn ID="8e8f3dca-0048-4f86-8d0d-0798df6ac9a3" Name="Process" Type="Reference(Typified) Not Null" ReferencedTable="ce0e5c9d-a8d2-498a-b8a0-d9bed35d5735">
		<Description>Ссылка на экземпляр процесса</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="8e8f3dca-0048-0086-4000-0798df6ac9a3" Name="ProcessRowID" Type="Guid Not Null" ReferencedColumn="ce0e5c9d-a8d2-008a-3100-09bed35d5735" />
	</SchemeComplexColumn>
	<SchemeComplexColumn ID="d0ce232f-12bb-43f5-b78b-eb623361f531" Name="Stage" Type="Reference(Typified) Not Null" ReferencedTable="ba857d60-e905-4e9f-bb44-e2d21dd25027">
		<Description>Ссылка на экземпляр этапа</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="d0ce232f-12bb-00f5-4000-0b623361f531" Name="StageRowID" Type="Guid Not Null" ReferencedColumn="ba857d60-e905-009f-3100-02d21dd25027" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn ID="4cbb0524-204c-413a-9055-400c3a403c97" Name="ParticipantID" Type="Guid Not Null" />
	<SchemePrimaryKey IsSystem="true" IsPermanent="true" IsSealed="true" ID="00343461-1765-006c-5000-0c599e8298a1" Name="pk_FdActiveTasksVirtual">
		<SchemeIndexedColumn Column="00343461-1765-006c-3100-0c599e8298a1" />
	</SchemePrimaryKey>
	<SchemeIndex IsSystem="true" IsPermanent="true" IsSealed="true" ID="00343461-1765-006c-7000-0c599e8298a1" Name="idx_FdActiveTasksVirtual_ID" IsClustered="true">
		<SchemeIndexedColumn Column="00343461-1765-016c-4000-0c599e8298a1" />
	</SchemeIndex>
</SchemeTable>