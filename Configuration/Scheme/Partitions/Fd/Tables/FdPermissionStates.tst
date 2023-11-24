<?xml version="1.0" encoding="utf-8"?>
<SchemeTable Partition="c3b14969-a553-4005-9a07-e9705c15b1e1" ID="e5b8bb42-ac63-4f58-b0d0-8a55d84b7b6f" Name="FdPermissionStates" Group="Fd Permissions" InstanceType="Cards" ContentType="Collections">
	<Description>Коллекция состояний, расширяющая типовые права доступа</Description>
	<SchemeComplexColumn IsSystem="true" IsPermanent="true" IsSealed="true" ID="e5b8bb42-ac63-0058-2000-0a55d84b7b6f" Name="ID" Type="Reference(Typified) Not Null" ReferencedTable="1074eadd-21d7-4925-98c8-40d1e5f0ca0e">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="e5b8bb42-ac63-0158-4000-0a55d84b7b6f" Name="ID" Type="Guid Not Null" ReferencedColumn="9a58123b-b2e9-4137-9c6c-5dab0ec02747" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn IsSystem="true" IsPermanent="true" IsSealed="true" ID="e5b8bb42-ac63-0058-3100-0a55d84b7b6f" Name="RowID" Type="Guid Not Null" />
	<SchemeComplexColumn ID="df3245c9-f3b5-449c-8c7b-8617cc935d22" Name="State" Type="Reference(Typified) Not Null" ReferencedTable="c5e015fe-1b55-4a18-8787-074b5d2ec80c">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="df3245c9-f3b5-009c-4000-0617cc935d22" Name="StateID" Type="Guid Not Null" ReferencedColumn="c5e015fe-1b55-0118-4000-074b5d2ec80c" />
		<SchemeReferencingColumn ID="ae69497d-b290-4b8c-b415-086379828961" Name="StateName" Type="String(128) Not Null" ReferencedColumn="09d1dfdb-262f-4b6e-b14d-64e5f51b7fa7" />
		<SchemePhysicalColumn ID="fc972df6-7faf-442e-b39d-627490942182" Name="StateCardTypeCaption" Type="String(128) Not Null" />
	</SchemeComplexColumn>
	<SchemePrimaryKey IsSystem="true" IsPermanent="true" IsSealed="true" ID="e5b8bb42-ac63-0058-5000-0a55d84b7b6f" Name="pk_FdPermissionStates">
		<SchemeIndexedColumn Column="e5b8bb42-ac63-0058-3100-0a55d84b7b6f" />
	</SchemePrimaryKey>
	<SchemeIndex IsSystem="true" IsPermanent="true" IsSealed="true" ID="e5b8bb42-ac63-0058-7000-0a55d84b7b6f" Name="idx_FdPermissionStates_ID" IsClustered="true">
		<SchemeIndexedColumn Column="e5b8bb42-ac63-0158-4000-0a55d84b7b6f" />
	</SchemeIndex>
</SchemeTable>