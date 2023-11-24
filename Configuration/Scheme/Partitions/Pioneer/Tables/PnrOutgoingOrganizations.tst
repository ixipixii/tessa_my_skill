<?xml version="1.0" encoding="utf-8"?>
<SchemeTable Partition="54ed79f5-11c0-4f74-b8b0-793d596699d6" ID="e976277b-4eb9-41e7-8f8c-0b3cb27dd7e6" Name="PnrOutgoingOrganizations" Group="Pnr" InstanceType="Cards" ContentType="Collections">
	<Description>Исходящий: Организация ГК Пионер</Description>
	<SchemeComplexColumn IsSystem="true" IsPermanent="true" IsSealed="true" ID="e976277b-4eb9-00e7-2000-0b3cb27dd7e6" Name="ID" Type="Reference(Typified) Not Null" ReferencedTable="1074eadd-21d7-4925-98c8-40d1e5f0ca0e">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="e976277b-4eb9-01e7-4000-0b3cb27dd7e6" Name="ID" Type="Guid Not Null" ReferencedColumn="9a58123b-b2e9-4137-9c6c-5dab0ec02747" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn IsSystem="true" IsPermanent="true" IsSealed="true" ID="e976277b-4eb9-00e7-3100-0b3cb27dd7e6" Name="RowID" Type="Guid Not Null" />
	<SchemeComplexColumn ID="8e5fab3a-c412-4f76-8b1f-fdd1d97e848d" Name="Organization" Type="Reference(Typified) Null" ReferencedTable="303dd35d-b998-49b3-a10b-ed30d31bd36a">
		<Description>Организация ГК Пионер</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="8e5fab3a-c412-0076-4000-0dd1d97e848d" Name="OrganizationID" Type="Guid Null" ReferencedColumn="303dd35d-b998-01b3-4000-0d30d31bd36a" />
		<SchemeReferencingColumn ID="294b6535-4b9f-4207-8de1-279237da79b8" Name="OrganizationName" Type="String(Max) Null" ReferencedColumn="d8ecabaa-f192-4546-9af6-1f9712daabdb" />
		<SchemeReferencingColumn ID="02af2b40-857c-4b9e-85d3-b81629e2bed8" Name="OrganizationLegalEntityIndex" Type="String(64) Null" ReferencedColumn="d16a0f17-c4b6-405b-af03-e20ba1aa16a2" />
	</SchemeComplexColumn>
	<SchemePrimaryKey IsSystem="true" IsPermanent="true" IsSealed="true" ID="e976277b-4eb9-00e7-5000-0b3cb27dd7e6" Name="pk_PnrOutgoingOrganizations">
		<SchemeIndexedColumn Column="e976277b-4eb9-00e7-3100-0b3cb27dd7e6" />
	</SchemePrimaryKey>
	<SchemeIndex IsSystem="true" IsPermanent="true" IsSealed="true" ID="e976277b-4eb9-00e7-7000-0b3cb27dd7e6" Name="idx_PnrOutgoingOrganizations_ID" IsClustered="true">
		<SchemeIndexedColumn Column="e976277b-4eb9-01e7-4000-0b3cb27dd7e6" />
	</SchemeIndex>
</SchemeTable>