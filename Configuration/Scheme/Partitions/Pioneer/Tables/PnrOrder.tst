<?xml version="1.0" encoding="utf-8"?>
<SchemeTable Partition="54ed79f5-11c0-4f74-b8b0-793d596699d6" ID="1f8166a9-9621-49ab-ad5f-0f0cfeb1bc66" Name="PnrOrder" Group="Pnr" InstanceType="Cards" ContentType="Entries">
	<Description>Приказы</Description>
	<SchemeComplexColumn IsSystem="true" IsPermanent="true" IsSealed="true" ID="1f8166a9-9621-00ab-2000-0f0cfeb1bc66" Name="ID" Type="Reference(Typified) Not Null" ReferencedTable="1074eadd-21d7-4925-98c8-40d1e5f0ca0e">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="1f8166a9-9621-01ab-4000-0f0cfeb1bc66" Name="ID" Type="Guid Not Null" ReferencedColumn="9a58123b-b2e9-4137-9c6c-5dab0ec02747" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn ID="edb59fec-dd9a-4cf7-8f57-62cffe892950" Name="RegistrationDate" Type="Date Null">
		<Description>Дата проекта</Description>
	</SchemePhysicalColumn>
	<SchemeComplexColumn ID="1469687c-199e-4d4c-8e65-1c82adb76866" Name="Project" Type="Reference(Typified) Null" ReferencedTable="44de4db0-e013-441c-8441-4cb3ef09a649">
		<Description>Проект</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="1469687c-199e-004c-4000-0c82adb76866" Name="ProjectID" Type="Guid Null" ReferencedColumn="44de4db0-e013-011c-4000-0cb3ef09a649" />
		<SchemeReferencingColumn ID="c3e91fd0-cd4d-441c-85d1-99f338efb8b0" Name="ProjectName" Type="String(Max) Null" ReferencedColumn="6c2fd8b3-e69b-4696-8a8a-2ab2d593af07" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn ID="1a050d72-dd42-4a71-bd8b-fd1f86435f2c" Name="Comments" Type="String(Max) Null">
		<Description>Комментарии к документу</Description>
	</SchemePhysicalColumn>
	<SchemeComplexColumn ID="221db5c2-6efd-4a55-929f-4bddb70b3ca1" Name="DocumentKind" Type="Reference(Typified) Null" ReferencedTable="a769bbbe-73f6-42eb-aa6b-b0ff72e07845">
		<Description>Вид документа</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="221db5c2-6efd-0055-4000-0bddb70b3ca1" Name="DocumentKindID" Type="Guid Null" ReferencedColumn="a769bbbe-73f6-01eb-4000-00ff72e07845" />
		<SchemeReferencingColumn ID="e0607709-a240-4a60-b9de-d60f0b206a25" Name="DocumentKindName" Type="String(Max) Null" ReferencedColumn="dfe455ca-273a-47cc-a9c1-cc620f0136ac" />
	</SchemeComplexColumn>
	<SchemeComplexColumn ID="37ec18c5-926d-43d3-8006-4f35f9d5597e" Name="Organization" Type="Reference(Typified) Null" ReferencedTable="303dd35d-b998-49b3-a10b-ed30d31bd36a">
		<Description>Организация ГК Пионер (из блоков)</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="37ec18c5-926d-00d3-4000-0f35f9d5597e" Name="OrganizationID" Type="Guid Null" ReferencedColumn="303dd35d-b998-01b3-4000-0d30d31bd36a" />
		<SchemeReferencingColumn ID="f6363375-2c02-4e33-854a-ece1c77e7073" Name="OrganizationName" Type="String(Max) Null" ReferencedColumn="d8ecabaa-f192-4546-9af6-1f9712daabdb" />
		<SchemeReferencingColumn ID="906de17a-2c63-4b6d-a90d-78184795a91d" Name="OrganizationLegalEntityIndex" Type="String(64) Null" ReferencedColumn="d16a0f17-c4b6-405b-af03-e20ba1aa16a2" />
	</SchemeComplexColumn>
	<SchemeComplexColumn ID="cb32e433-3ac9-4c30-a9a8-48ff413f27f1" Name="WriteOff" Type="Reference(Typified) Null" ReferencedTable="81f6010b-9641-4aa5-8897-b8e8603fbf4b">
		<Description>Списать в дело</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="cb32e433-3ac9-0030-4000-08ff413f27f1" Name="WriteOffID" Type="Guid Null" ReferencedColumn="81f6010b-9641-01a5-4000-08e8603fbf4b" />
		<SchemeReferencingColumn ID="429f5852-5a58-417d-a425-760371e0e5b0" Name="WriteOffName" Type="String(128) Null" ReferencedColumn="616d6b2e-35d5-424d-846b-618eb25962d0" />
		<SchemeReferencingColumn ID="af0613a4-b588-4166-aff2-51556bfcff83" Name="WriteOffIdx" Type="String(Max) Null" ReferencedColumn="1f7ca0d4-1b64-43f8-b973-d14791af51a0" />
	</SchemeComplexColumn>
	<SchemeComplexColumn ID="81596976-5524-4790-94e5-b351f94a1a4d" Name="DepartmentManager" Type="Reference(Typified) Null" ReferencedTable="6c977939-bbfc-456f-a133-f1c2244e3cc3">
		<Description>Руководитель подразделения</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="81596976-5524-0090-4000-0351f94a1a4d" Name="DepartmentManagerID" Type="Guid Null" ReferencedColumn="6c977939-bbfc-016f-4000-01c2244e3cc3" />
		<SchemeReferencingColumn ID="326184ec-e5d5-407a-9952-628b6b8a11ce" Name="DepartmentManagerName" Type="String(128) Null" ReferencedColumn="1782f76a-4743-4aa4-920c-7edaee860964" />
	</SchemeComplexColumn>
	<SchemeComplexColumn ID="d4e97a9c-27ea-4682-8ac7-f7ddd0fce1c7" Name="HeadDirectorate" Type="Reference(Typified) Null" ReferencedTable="6c977939-bbfc-456f-a133-f1c2244e3cc3">
		<Description>Руководитель дирекции</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="d4e97a9c-27ea-0082-4000-07ddd0fce1c7" Name="HeadDirectorateID" Type="Guid Null" ReferencedColumn="6c977939-bbfc-016f-4000-01c2244e3cc3" />
		<SchemeReferencingColumn ID="227d9a83-89a8-45c4-a20d-013855078a29" Name="HeadDirectorateName" Type="String(128) Null" ReferencedColumn="1782f76a-4743-4aa4-920c-7edaee860964" />
	</SchemeComplexColumn>
	<SchemeComplexColumn ID="a2ca04b1-a2f5-4d97-9db4-9019c6d703f0" Name="LegalEntityIndex" Type="Reference(Typified) Null" ReferencedTable="00e3d618-aa1e-4bac-9297-3010b710425c">
		<Description>Индекс ЮЛ</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="a2ca04b1-a2f5-0097-4000-0019c6d703f0" Name="LegalEntityIndexID" Type="Guid Null" ReferencedColumn="00e3d618-aa1e-01ac-4000-0010b710425c" />
		<SchemeReferencingColumn ID="737654e7-bd71-4815-9107-dce486492ff0" Name="LegalEntityIndexIdx" Type="String(Max) Null" ReferencedColumn="14c84e81-9e95-4cfa-98cf-62c786ba4bfe" />
	</SchemeComplexColumn>
	<SchemeComplexColumn ID="e422aa95-55b1-4050-850c-1d8b2e0ef9bb" Name="ProjectBlock" Type="Reference(Typified) Null" ReferencedTable="44de4db0-e013-441c-8441-4cb3ef09a649">
		<Description>Проект (поле для блоков)</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="e422aa95-55b1-0050-4000-0d8b2e0ef9bb" Name="ProjectBlockID" Type="Guid Null" ReferencedColumn="44de4db0-e013-011c-4000-0cb3ef09a649" />
		<SchemeReferencingColumn ID="db25cba4-0c6b-4240-b3ff-d0a3d5b99973" Name="ProjectBlockName" Type="String(Max) Null" ReferencedColumn="6c2fd8b3-e69b-4696-8a8a-2ab2d593af07" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn ID="7a4e42b3-a6aa-47b9-b12d-b59d8a86ddae" Name="ExtID" Type="Guid Null">
		<Description>GUID карточки из системы заказчика</Description>
	</SchemePhysicalColumn>
	<SchemeComplexColumn ID="a47c1852-92fa-4338-8885-ab465bad5c46" Name="Department" Type="Reference(Typified) Null" ReferencedTable="81f6010b-9641-4aa5-8897-b8e8603fbf4b">
		<Description>Подразделение</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="a47c1852-92fa-0038-4000-0b465bad5c46" Name="DepartmentID" Type="Guid Null" ReferencedColumn="81f6010b-9641-01a5-4000-08e8603fbf4b" />
		<SchemeReferencingColumn ID="21dea5b4-3ff6-481a-8947-53bdfbe9d46a" Name="DepartmentName" Type="String(128) Null" ReferencedColumn="616d6b2e-35d5-424d-846b-618eb25962d0" />
		<SchemeReferencingColumn ID="45b5f74c-7d98-4cb7-932e-681280f81b7c" Name="DepartmentIdx" Type="String(Max) Null" ReferencedColumn="1f7ca0d4-1b64-43f8-b973-d14791af51a0" />
	</SchemeComplexColumn>
	<SchemePrimaryKey IsSystem="true" IsPermanent="true" IsSealed="true" ID="1f8166a9-9621-00ab-5000-0f0cfeb1bc66" Name="pk_PnrOrder" IsClustered="true">
		<SchemeIndexedColumn Column="1f8166a9-9621-01ab-4000-0f0cfeb1bc66" />
	</SchemePrimaryKey>
</SchemeTable>