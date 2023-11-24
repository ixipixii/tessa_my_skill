<?xml version="1.0" encoding="utf-8"?>
<SchemeTable Partition="54ed79f5-11c0-4f74-b8b0-793d596699d6" ID="e3eebe1e-1e3a-414b-8eaa-0cb017b61074" Name="PnrActs" Group="Pnr" InstanceType="Cards" ContentType="Entries">
	<Description>Акты</Description>
	<SchemeComplexColumn IsSystem="true" IsPermanent="true" IsSealed="true" ID="e3eebe1e-1e3a-004b-2000-0cb017b61074" Name="ID" Type="Reference(Typified) Not Null" ReferencedTable="1074eadd-21d7-4925-98c8-40d1e5f0ca0e">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="e3eebe1e-1e3a-014b-4000-0cb017b61074" Name="ID" Type="Guid Not Null" ReferencedColumn="9a58123b-b2e9-4137-9c6c-5dab0ec02747" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn ID="543896a2-f7ba-413f-bef0-a20dad2f124a" Name="ProjectDate" Type="Date Null">
		<Description>Дата проекта</Description>
	</SchemePhysicalColumn>
	<SchemeComplexColumn ID="d96d0c94-2176-44c3-a857-80dc1b375ed2" Name="Organization" Type="Reference(Typified) Null" ReferencedTable="303dd35d-b998-49b3-a10b-ed30d31bd36a">
		<Description>Организация ГК Пионер</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="d96d0c94-2176-00c3-4000-00dc1b375ed2" Name="OrganizationID" Type="Guid Null" ReferencedColumn="303dd35d-b998-01b3-4000-0d30d31bd36a" />
		<SchemeReferencingColumn ID="97c23c5c-e65a-424e-9ebf-f69506dcabe6" Name="OrganizationName" Type="String(Max) Null" ReferencedColumn="d8ecabaa-f192-4546-9af6-1f9712daabdb" />
	</SchemeComplexColumn>
	<SchemeComplexColumn ID="814cd9d8-102a-437c-b944-c42567ac399f" Name="Project" Type="Reference(Typified) Null" ReferencedTable="44de4db0-e013-441c-8441-4cb3ef09a649">
		<Description>Проект</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="814cd9d8-102a-007c-4000-042567ac399f" Name="ProjectID" Type="Guid Null" ReferencedColumn="44de4db0-e013-011c-4000-0cb3ef09a649" />
		<SchemeReferencingColumn ID="79675b91-7cbf-4336-91c0-c5cdc7ec6aa5" Name="ProjectName" Type="String(Max) Null" ReferencedColumn="6c2fd8b3-e69b-4696-8a8a-2ab2d593af07" />
	</SchemeComplexColumn>
	<SchemeComplexColumn ID="a603f279-9dbe-4c3c-9d79-181093c5633a" Name="Type" Type="Reference(Typified) Null" ReferencedTable="8c5887a3-8ecc-42da-b5c1-c696bc056541">
		<Description>Тип акта</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="a603f279-9dbe-003c-4000-081093c5633a" Name="TypeID" Type="Guid Null" ReferencedColumn="8c5887a3-8ecc-01da-4000-0696bc056541" />
		<SchemeReferencingColumn ID="97421dc5-4286-4b98-b326-92852c2d9ae1" Name="TypeName" Type="String(Max) Null" ReferencedColumn="5c36b332-5b54-4d96-8435-a36ea16aa25b" />
	</SchemeComplexColumn>
	<SchemeComplexColumn ID="2bd788a2-ef43-4940-9392-a197a9779907" Name="ImplementationStage" Type="Reference(Typified) Null" ReferencedTable="acb2c154-f00a-4dbd-adb0-8b77ca861ae2">
		<Description>Стадия реализации</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="2bd788a2-ef43-0040-4000-0197a9779907" Name="ImplementationStageID" Type="Guid Null" ReferencedColumn="acb2c154-f00a-01bd-4000-0b77ca861ae2" />
		<SchemeReferencingColumn ID="0de4d16d-5e99-43c2-b682-e4ddd77de184" Name="ImplementationStageName" Type="String(Max) Null" ReferencedColumn="32b4bb15-8adb-4851-b9c6-d990cb2e3396" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn ID="5af3e2df-c2d9-40b5-a874-749200262c75" Name="CMP_Project" Type="String(Max) Null">
		<Description>СМР или Проект</Description>
	</SchemePhysicalColumn>
	<SchemeComplexColumn ID="65dbfa05-bce5-41ff-b5b3-ca2068453685" Name="Destination" Type="Reference(Typified) Null" ReferencedTable="5d47ef13-b6f4-47ef-9815-3b3d0e6d475a">
		<Description>Контрагент</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="65dbfa05-bce5-00ff-4000-0a2068453685" Name="DestinationID" Type="Guid Null" ReferencedColumn="5d47ef13-b6f4-01ef-4000-0b3d0e6d475a" />
		<SchemeReferencingColumn ID="f93fc11b-652f-4d38-8b96-a6948cd7b3f1" Name="DestinationName" Type="String(255) Null" ReferencedColumn="f1c960e0-951e-4837-8474-bb61d98f40f0" />
	</SchemeComplexColumn>
	<SchemePrimaryKey IsSystem="true" IsPermanent="true" IsSealed="true" ID="e3eebe1e-1e3a-004b-5000-0cb017b61074" Name="pk_PnrActs" IsClustered="true">
		<SchemeIndexedColumn Column="e3eebe1e-1e3a-014b-4000-0cb017b61074" />
	</SchemePrimaryKey>
</SchemeTable>