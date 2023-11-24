<?xml version="1.0" encoding="utf-8"?>
<SchemeTable Partition="c3b14969-a553-4005-9a07-e9705c15b1e1" ID="3c578ad8-2876-4e69-9d91-2d67f5d35e0c" Name="FdSatelliteCommonInfo" Group="Fd" InstanceType="Cards" ContentType="Entries">
	<Description>Сателлит с процессами по карточке. (по сути аналог KrApprovalCommonInfo)</Description>
	<SchemeComplexColumn IsSystem="true" IsPermanent="true" IsSealed="true" ID="3c578ad8-2876-0069-2000-0d67f5d35e0c" Name="ID" Type="Reference(Typified) Not Null" ReferencedTable="1074eadd-21d7-4925-98c8-40d1e5f0ca0e">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="3c578ad8-2876-0169-4000-0d67f5d35e0c" Name="ID" Type="Guid Not Null" ReferencedColumn="9a58123b-b2e9-4137-9c6c-5dab0ec02747" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn ID="6182bc21-5c6f-4542-b0ca-ba16d236e689" Name="MainCardId" Type="Guid Null" />
	<SchemeComplexColumn ID="e6846070-29b8-4669-9e76-91a8924f927f" Name="State" Type="Reference(Typified) Null" ReferencedTable="c5e015fe-1b55-4a18-8787-074b5d2ec80c">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="e6846070-29b8-0069-4000-01a8924f927f" Name="StateID" Type="Guid Null" ReferencedColumn="c5e015fe-1b55-0118-4000-074b5d2ec80c" />
		<SchemeReferencingColumn ID="4ee55509-5c14-432d-a746-94ce0728bee0" Name="StateName" Type="String(128) Null" ReferencedColumn="09d1dfdb-262f-4b6e-b14d-64e5f51b7fa7" />
	</SchemeComplexColumn>
	<SchemePrimaryKey IsSystem="true" IsPermanent="true" IsSealed="true" ID="3c578ad8-2876-0069-5000-0d67f5d35e0c" Name="pk_FdSatelliteCommonInfo" IsClustered="true">
		<SchemeIndexedColumn Column="3c578ad8-2876-0169-4000-0d67f5d35e0c" />
	</SchemePrimaryKey>
	<SchemeIndex ID="6a96ef1a-46bf-49b5-b2b3-ac6819a1e609" Name="ndx_FdSatelliteCommonInfo_MainCardId">
		<SchemeIndexedColumn Column="6182bc21-5c6f-4542-b0ca-ba16d236e689" />
	</SchemeIndex>
</SchemeTable>