<?xml version="1.0" encoding="utf-8"?>
<SchemeTable Partition="54ed79f5-11c0-4f74-b8b0-793d596699d6" ID="2e599272-1226-411f-a5b8-115fd8f880b3" Name="PnrServiceNote" Group="Pnr" InstanceType="Cards" ContentType="Entries">
	<Description>Служебные записки</Description>
	<SchemeComplexColumn IsSystem="true" IsPermanent="true" IsSealed="true" ID="2e599272-1226-001f-2000-015fd8f880b3" Name="ID" Type="Reference(Typified) Not Null" ReferencedTable="1074eadd-21d7-4925-98c8-40d1e5f0ca0e">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="2e599272-1226-011f-4000-015fd8f880b3" Name="ID" Type="Guid Not Null" ReferencedColumn="9a58123b-b2e9-4137-9c6c-5dab0ec02747" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn ID="750843fa-50c9-4b20-9428-18191fa51a30" Name="Summary" Type="String(Max) Null">
		<Description>Краткое содержание</Description>
	</SchemePhysicalColumn>
	<SchemeComplexColumn ID="4f7b3b88-c74b-451a-b438-bb6285103bbc" Name="Organization" Type="Reference(Typified) Null" ReferencedTable="303dd35d-b998-49b3-a10b-ed30d31bd36a">
		<Description>Организация ГК Пионер</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="4f7b3b88-c74b-001a-4000-0b6285103bbc" Name="OrganizationID" Type="Guid Null" ReferencedColumn="303dd35d-b998-01b3-4000-0d30d31bd36a" />
		<SchemeReferencingColumn ID="d0c63823-0d0b-4430-8bda-62ef554fc96e" Name="OrganizationName" Type="String(Max) Null" ReferencedColumn="d8ecabaa-f192-4546-9af6-1f9712daabdb" />
	</SchemeComplexColumn>
	<SchemeComplexColumn ID="480661d8-7b94-4486-9c75-0e484c834e03" Name="Project" Type="Reference(Typified) Null" ReferencedTable="44de4db0-e013-441c-8441-4cb3ef09a649">
		<Description>Проект</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="480661d8-7b94-0086-4000-0e484c834e03" Name="ProjectID" Type="Guid Null" ReferencedColumn="44de4db0-e013-011c-4000-0cb3ef09a649" />
		<SchemeReferencingColumn ID="79c09d6c-5d85-483b-b6ff-69565e39258e" Name="ProjectName" Type="String(Max) Null" ReferencedColumn="6c2fd8b3-e69b-4696-8a8a-2ab2d593af07" />
	</SchemeComplexColumn>
	<SchemeComplexColumn ID="3f4b9456-825c-465c-aef7-ef4f3d9ae953" Name="Department" Type="Reference(Typified) Null" ReferencedTable="81f6010b-9641-4aa5-8897-b8e8603fbf4b">
		<Description>Подразделение</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="3f4b9456-825c-005c-4000-0f4f3d9ae953" Name="DepartmentID" Type="Guid Null" ReferencedColumn="81f6010b-9641-01a5-4000-08e8603fbf4b" />
		<SchemeReferencingColumn ID="8d895652-1148-4381-ac08-3ff17ff5d944" Name="DepartmentName" Type="String(128) Null" ReferencedColumn="616d6b2e-35d5-424d-846b-618eb25962d0" />
		<SchemeReferencingColumn ID="5d8d8119-3004-4d8f-8926-37d1ffb9a430" Name="DepartmentIdx" Type="String(Max) Null" ReferencedColumn="1f7ca0d4-1b64-43f8-b973-d14791af51a0" />
	</SchemeComplexColumn>
	<SchemeComplexColumn ID="a8218fa4-bcd2-438b-b8d6-0fa5a4572bfa" Name="DestinationDepartment" Type="Reference(Typified) Null" ReferencedTable="81f6010b-9641-4aa5-8897-b8e8603fbf4b">
		<Description>Подразделение адресата</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="a8218fa4-bcd2-008b-4000-0fa5a4572bfa" Name="DestinationDepartmentID" Type="Guid Null" ReferencedColumn="81f6010b-9641-01a5-4000-08e8603fbf4b" />
		<SchemeReferencingColumn ID="d44e003e-3cf9-40de-846f-906a763d9863" Name="DestinationDepartmentName" Type="String(128) Null" ReferencedColumn="616d6b2e-35d5-424d-846b-618eb25962d0" />
		<SchemeReferencingColumn ID="46fe2600-6671-4d5c-bca9-5ad696d716d6" Name="DestinationDepartmentIdx" Type="String(Max) Null" ReferencedColumn="1f7ca0d4-1b64-43f8-b973-d14791af51a0" />
	</SchemeComplexColumn>
	<SchemeComplexColumn ID="885fc929-eb86-470f-ba24-e79d9693fd70" Name="Destination" Type="Reference(Typified) Null" ReferencedTable="6c977939-bbfc-456f-a133-f1c2244e3cc3">
		<Description>Адресат</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="885fc929-eb86-000f-4000-079d9693fd70" Name="DestinationID" Type="Guid Null" ReferencedColumn="6c977939-bbfc-016f-4000-01c2244e3cc3" />
		<SchemeReferencingColumn ID="77bbc66f-3d28-4f72-8619-801993a676b2" Name="DestinationName" Type="String(128) Null" ReferencedColumn="1782f76a-4743-4aa4-920c-7edaee860964" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn ID="4a608ca8-8333-4f43-ac79-36cb3626bc8f" Name="Comments" Type="String(Max) Null">
		<Description>Комментарий к документу</Description>
	</SchemePhysicalColumn>
	<SchemeComplexColumn ID="d840d977-dd05-4858-b995-1550a5bd3726" Name="GroupDocuments" Type="Reference(Typified) Null" ReferencedTable="509214b4-3b67-4597-b5ec-d09ad9273a85">
		<Description>Группа документов</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="d840d977-dd05-0058-4000-0550a5bd3726" Name="GroupDocumentsID" Type="Int16 Null" ReferencedColumn="2a8ee9d0-9ebc-4be0-9ff7-b58c66626682" />
		<SchemeReferencingColumn ID="eec004ed-6901-4705-83bc-e38373d6791a" Name="GroupDocumentsName" Type="String(64) Null" ReferencedColumn="d7c89108-4613-4915-ab8d-4f092abf370f" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn ID="9d61c650-cd6c-4aab-a45a-e3d824a87178" Name="ProjectDate" Type="Date Null">
		<Description>Дата проекта</Description>
	</SchemePhysicalColumn>
	<SchemeComplexColumn ID="3444f2a0-0371-47be-acd7-c1e42dcbfe3a" Name="LegalEntityIndex" Type="Reference(Typified) Null" ReferencedTable="00e3d618-aa1e-4bac-9297-3010b710425c">
		<Description>Индекс ЮЛ</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="3444f2a0-0371-00be-4000-01e42dcbfe3a" Name="LegalEntityIndexID" Type="Guid Null" ReferencedColumn="00e3d618-aa1e-01ac-4000-0010b710425c" />
		<SchemeReferencingColumn ID="9359fc91-43a6-487d-8bf4-13462bb3b617" Name="LegalEntityIndexIdx" Type="String(Max) Null" ReferencedColumn="14c84e81-9e95-4cfa-98cf-62c786ba4bfe" />
	</SchemeComplexColumn>
	<SchemeComplexColumn ID="7f14aeaf-ca03-4132-9a99-103c50c37ea7" Name="ServiceNoteType" Type="Reference(Typified) Null" ReferencedTable="9f2d4747-edad-421c-a624-3ea3f0720b0f">
		<Description>Тип служебной записки</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="7f14aeaf-ca03-0032-4000-003c50c37ea7" Name="ServiceNoteTypeID" Type="Guid Null" ReferencedColumn="9f2d4747-edad-011c-4000-0ea3f0720b0f" />
		<SchemeReferencingColumn ID="dd2f1bc1-d5de-4bc4-a2ef-f0a5af215403" Name="ServiceNoteTypeName" Type="String(Max) Null" ReferencedColumn="5a48e37a-d79b-4cfb-93cb-af1b2fc0a809" />
	</SchemeComplexColumn>
	<SchemeComplexColumn ID="c61194a2-8e27-4e08-a3d0-ea8cb141302e" Name="WriteOff" Type="Reference(Typified) Null" ReferencedTable="81f6010b-9641-4aa5-8897-b8e8603fbf4b">
		<Description>Списать в дело</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="c61194a2-8e27-0008-4000-0a8cb141302e" Name="WriteOffID" Type="Guid Null" ReferencedColumn="81f6010b-9641-01a5-4000-08e8603fbf4b" />
		<SchemeReferencingColumn ID="1aeb28f0-d501-4a9c-9b5c-98fac612e928" Name="WriteOffName" Type="String(128) Null" ReferencedColumn="616d6b2e-35d5-424d-846b-618eb25962d0" />
		<SchemeReferencingColumn ID="6e75b480-706e-4397-a439-26ee2d74762a" Name="WriteOffIdx" Type="String(Max) Null" ReferencedColumn="1f7ca0d4-1b64-43f8-b973-d14791af51a0" />
	</SchemeComplexColumn>
	<SchemeComplexColumn ID="e098515a-c3da-4c7a-808a-0da3b615270c" Name="ServiceNoteTheme" Type="Reference(Typified) Null" ReferencedTable="fd957afa-ded7-48d5-bb33-9a7db3960247">
		<Description>Тематика служебной записки</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="e098515a-c3da-007a-4000-0da3b615270c" Name="ServiceNoteThemeID" Type="Guid Null" ReferencedColumn="fd957afa-ded7-01d5-4000-0a7db3960247" />
		<SchemeReferencingColumn ID="462980cc-ae1d-401d-8fa9-203711c70f67" Name="ServiceNoteThemeName" Type="String(Max) Null" ReferencedColumn="8c13a21c-7143-4fa5-9c2e-3f5e3e5d0710" />
	</SchemeComplexColumn>
	<SchemeComplexColumn ID="2a503e57-af26-41d5-9385-19700b686436" Name="CFO" Type="Reference(Typified) Null" ReferencedTable="b5e873a7-4f25-4731-b7bf-93586f07b53a">
		<Description>ЦФО</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="2a503e57-af26-00d5-4000-09700b686436" Name="CFOID" Type="Guid Null" ReferencedColumn="b5e873a7-4f25-0131-4000-03586f07b53a" />
		<SchemeReferencingColumn ID="58ad3c52-3a5b-48b2-a534-445910d65d68" Name="CFOName" Type="String(Max) Null" ReferencedColumn="20d4f2eb-ce34-4c44-87b8-8b386c283930" />
	</SchemeComplexColumn>
	<SchemeComplexColumn ID="605f51c6-71af-42a4-93de-cc4557dc40c5" Name="OrganizationBlock" Type="Reference(Typified) Null" ReferencedTable="303dd35d-b998-49b3-a10b-ed30d31bd36a">
		<Description>Организация в блоке "По вопросам финансовой деятельности и обучения"</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="605f51c6-71af-00a4-4000-0c4557dc40c5" Name="OrganizationBlockID" Type="Guid Null" ReferencedColumn="303dd35d-b998-01b3-4000-0d30d31bd36a" />
		<SchemeReferencingColumn ID="98aa82ab-bdae-4717-a684-140e5cbf7c51" Name="OrganizationBlockName" Type="String(Max) Null" ReferencedColumn="d8ecabaa-f192-4546-9af6-1f9712daabdb" />
	</SchemeComplexColumn>
	<SchemePrimaryKey IsSystem="true" IsPermanent="true" IsSealed="true" ID="2e599272-1226-001f-5000-015fd8f880b3" Name="pk_PnrServiceNote" IsClustered="true">
		<SchemeIndexedColumn Column="2e599272-1226-011f-4000-015fd8f880b3" />
	</SchemePrimaryKey>
</SchemeTable>