<?xml version="1.0" encoding="utf-8"?>
<SchemeTable Partition="54ed79f5-11c0-4f74-b8b0-793d596699d6" ID="dc90eaa3-a5f0-414b-85a9-6e445b4467f4" Name="PnrIncomingOrganizations" Group="Pnr" InstanceType="Cards" ContentType="Collections">
	<Description>Входящий: Организация ГК Пионер</Description>
	<SchemeComplexColumn IsSystem="true" IsPermanent="true" IsSealed="true" ID="dc90eaa3-a5f0-004b-2000-0e445b4467f4" Name="ID" Type="Reference(Typified) Not Null" ReferencedTable="1074eadd-21d7-4925-98c8-40d1e5f0ca0e">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="dc90eaa3-a5f0-014b-4000-0e445b4467f4" Name="ID" Type="Guid Not Null" ReferencedColumn="9a58123b-b2e9-4137-9c6c-5dab0ec02747" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn IsSystem="true" IsPermanent="true" IsSealed="true" ID="dc90eaa3-a5f0-004b-3100-0e445b4467f4" Name="RowID" Type="Guid Not Null" />
	<SchemeComplexColumn ID="12fd48fe-6bc7-4b10-a1e7-c83cef10bb6e" Name="Organization" Type="Reference(Typified) Null" ReferencedTable="303dd35d-b998-49b3-a10b-ed30d31bd36a">
		<Description>Организация ГК Пионер</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="12fd48fe-6bc7-0010-4000-083cef10bb6e" Name="OrganizationID" Type="Guid Null" ReferencedColumn="303dd35d-b998-01b3-4000-0d30d31bd36a" />
		<SchemeReferencingColumn ID="f955d181-9e68-4852-8580-e2d4eec3d551" Name="OrganizationName" Type="String(Max) Null" ReferencedColumn="d8ecabaa-f192-4546-9af6-1f9712daabdb" />
		<SchemeReferencingColumn ID="14b6e6e9-57ed-4c2d-ab9b-1b11220fa733" Name="OrganizationLegalEntityIndex" Type="String(64) Null" ReferencedColumn="d16a0f17-c4b6-405b-af03-e20ba1aa16a2" />
	</SchemeComplexColumn>
	<SchemePrimaryKey IsSystem="true" IsPermanent="true" IsSealed="true" ID="dc90eaa3-a5f0-004b-5000-0e445b4467f4" Name="pk_PnrIncomingOrganizations">
		<SchemeIndexedColumn Column="dc90eaa3-a5f0-004b-3100-0e445b4467f4" />
	</SchemePrimaryKey>
	<SchemeIndex IsSystem="true" IsPermanent="true" IsSealed="true" ID="dc90eaa3-a5f0-004b-7000-0e445b4467f4" Name="idx_PnrIncomingOrganizations_ID" IsClustered="true">
		<SchemeIndexedColumn Column="dc90eaa3-a5f0-014b-4000-0e445b4467f4" />
	</SchemeIndex>
</SchemeTable>