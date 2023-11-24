<?xml version="1.0" encoding="utf-8"?>
<SchemeTable Partition="c3b14969-a553-4005-9a07-e9705c15b1e1" ID="53ad6a32-f205-4c68-8df4-bfbba18a1cea" Name="FdSatelliteCommonInfoVirtual" Group="Fd" IsVirtual="true" InstanceType="Cards" ContentType="Entries">
	<SchemeComplexColumn IsSystem="true" IsPermanent="true" IsSealed="true" ID="53ad6a32-f205-0068-2000-0fbba18a1cea" Name="ID" Type="Reference(Typified) Not Null" ReferencedTable="1074eadd-21d7-4925-98c8-40d1e5f0ca0e">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="53ad6a32-f205-0168-4000-0fbba18a1cea" Name="ID" Type="Guid Not Null" ReferencedColumn="9a58123b-b2e9-4137-9c6c-5dab0ec02747" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn ID="53829095-6d91-428c-958c-3444a40a2311" Name="MainCardId" Type="Guid Null" />
	<SchemeComplexColumn ID="53cd2159-752f-41b9-8ada-cecdc8d136fe" Name="State" Type="Reference(Typified) Null" ReferencedTable="c5e015fe-1b55-4a18-8787-074b5d2ec80c">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="53cd2159-752f-00b9-4000-0ecdc8d136fe" Name="StateID" Type="Guid Null" ReferencedColumn="c5e015fe-1b55-0118-4000-074b5d2ec80c" />
		<SchemeReferencingColumn ID="54558f3f-53b6-4645-9fdb-6aacaf7f3193" Name="StateName" Type="String(128) Null" ReferencedColumn="09d1dfdb-262f-4b6e-b14d-64e5f51b7fa7" />
	</SchemeComplexColumn>
	<SchemePrimaryKey IsSystem="true" IsPermanent="true" IsSealed="true" ID="53ad6a32-f205-0068-5000-0fbba18a1cea" Name="pk_FdSatelliteCommonInfoVirtual" IsClustered="true">
		<SchemeIndexedColumn Column="53ad6a32-f205-0168-4000-0fbba18a1cea" />
	</SchemePrimaryKey>
</SchemeTable>