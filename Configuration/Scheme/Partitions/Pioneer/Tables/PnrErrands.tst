<?xml version="1.0" encoding="utf-8"?>
<SchemeTable Partition="54ed79f5-11c0-4f74-b8b0-793d596699d6" ID="bafcf7b4-92ca-4aff-8fe4-3d56af236a08" Name="PnrErrands" Group="Pnr" InstanceType="Cards" ContentType="Entries">
	<Description>Поручения</Description>
	<SchemeComplexColumn IsSystem="true" IsPermanent="true" IsSealed="true" ID="bafcf7b4-92ca-00ff-2000-0d56af236a08" Name="ID" Type="Reference(Typified) Not Null" ReferencedTable="1074eadd-21d7-4925-98c8-40d1e5f0ca0e">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="bafcf7b4-92ca-01ff-4000-0d56af236a08" Name="ID" Type="Guid Not Null" ReferencedColumn="9a58123b-b2e9-4137-9c6c-5dab0ec02747" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn ID="6417c88d-f96a-43a0-a826-b2709dd5fe3a" Name="PeriodExecution" Type="Date Null">
		<Description>Срок исполнения</Description>
	</SchemePhysicalColumn>
	<SchemeComplexColumn ID="f100b7a5-cec4-4631-962b-6238fd6ed81e" Name="Project" Type="Reference(Typified) Null" ReferencedTable="44de4db0-e013-441c-8441-4cb3ef09a649">
		<Description>Проект</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="f100b7a5-cec4-0031-4000-0238fd6ed81e" Name="ProjectID" Type="Guid Null" ReferencedColumn="44de4db0-e013-011c-4000-0cb3ef09a649" />
		<SchemeReferencingColumn ID="c6441f29-9e9f-42b9-bd4e-57501d444f3f" Name="ProjectName" Type="String(Max) Null" ReferencedColumn="6c2fd8b3-e69b-4696-8a8a-2ab2d593af07" />
	</SchemeComplexColumn>
	<SchemeComplexColumn ID="07e845da-b964-42d1-9d95-edcea1febe53" Name="Partner" Type="Reference(Typified) Null" ReferencedTable="5d47ef13-b6f4-47ef-9815-3b3d0e6d475a">
		<Description>Контрагент</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="07e845da-b964-00d1-4000-0dcea1febe53" Name="PartnerID" Type="Guid Null" ReferencedColumn="5d47ef13-b6f4-01ef-4000-0b3d0e6d475a" />
		<SchemeReferencingColumn ID="c0ee22fc-dc0b-41b2-a04b-78e6f15ca92c" Name="PartnerName" Type="String(255) Null" ReferencedColumn="f1c960e0-951e-4837-8474-bb61d98f40f0" />
	</SchemeComplexColumn>
	<SchemeComplexColumn ID="edb9cd65-aaf4-4604-aff2-82cf88bd1993" Name="Organization" Type="Reference(Typified) Null" ReferencedTable="303dd35d-b998-49b3-a10b-ed30d31bd36a">
		<Description>Организация ГК Пионер</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="edb9cd65-aaf4-0004-4000-02cf88bd1993" Name="OrganizationID" Type="Guid Null" ReferencedColumn="303dd35d-b998-01b3-4000-0d30d31bd36a" />
		<SchemeReferencingColumn ID="25a38cdb-c6e7-4df5-ad56-bebb79878002" Name="OrganizationName" Type="String(Max) Null" ReferencedColumn="d8ecabaa-f192-4546-9af6-1f9712daabdb" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn ID="f50b7a15-f19d-433a-affa-7aaa0d53f5c1" Name="Comment" Type="String(Max) Null">
		<Description>Комментарий</Description>
	</SchemePhysicalColumn>
	<SchemePrimaryKey IsSystem="true" IsPermanent="true" IsSealed="true" ID="bafcf7b4-92ca-00ff-5000-0d56af236a08" Name="pk_PnrErrands" IsClustered="true">
		<SchemeIndexedColumn Column="bafcf7b4-92ca-01ff-4000-0d56af236a08" />
	</SchemePrimaryKey>
</SchemeTable>