<?xml version="1.0" encoding="utf-8"?>
<SchemeTable Partition="54ed79f5-11c0-4f74-b8b0-793d596699d6" ID="5cc50e05-889f-4688-931f-b6443de62b28" Name="PnrOutgoing" Group="Pnr" InstanceType="Cards" ContentType="Entries">
	<Description>Исходящий документ</Description>
	<SchemeComplexColumn IsSystem="true" IsPermanent="true" IsSealed="true" ID="5cc50e05-889f-0088-2000-06443de62b28" Name="ID" Type="Reference(Typified) Not Null" ReferencedTable="1074eadd-21d7-4925-98c8-40d1e5f0ca0e">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="5cc50e05-889f-0188-4000-06443de62b28" Name="ID" Type="Guid Not Null" ReferencedColumn="9a58123b-b2e9-4137-9c6c-5dab0ec02747" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn ID="22a8f867-ff06-4834-adb8-7b5e02139016" Name="RegistrationDate" Type="Date Null">
		<Description>Дата регистрации</Description>
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="88aca2cc-a773-4d64-8803-3d94dbeb31ec" Name="Summary" Type="String(Max) Null">
		<Description>Краткое содержание</Description>
	</SchemePhysicalColumn>
	<SchemeComplexColumn ID="105bc029-6777-41db-b47d-f32e6b626578" Name="Destination" Type="Reference(Typified) Null" ReferencedTable="5d47ef13-b6f4-47ef-9815-3b3d0e6d475a">
		<Description>Адресат (контрагент)</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="105bc029-6777-00db-4000-032e6b626578" Name="DestinationID" Type="Guid Null" ReferencedColumn="5d47ef13-b6f4-01ef-4000-0b3d0e6d475a" />
		<SchemeReferencingColumn ID="f5963de3-fde7-4182-a92f-e99a184790dd" Name="DestinationName" Type="String(255) Null" ReferencedColumn="f1c960e0-951e-4837-8474-bb61d98f40f0" />
	</SchemeComplexColumn>
	<SchemeComplexColumn ID="509defb8-e2da-4f74-a1df-285f69c54b14" Name="LegalEntityIndex" Type="Reference(Typified) Null" ReferencedTable="00e3d618-aa1e-4bac-9297-3010b710425c">
		<Description>Индекс юридического лица</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="509defb8-e2da-0074-4000-085f69c54b14" Name="LegalEntityIndexID" Type="Guid Null" ReferencedColumn="00e3d618-aa1e-01ac-4000-0010b710425c" />
		<SchemeReferencingColumn ID="05193214-e3e6-4dac-8d1d-15f6073fded4" Name="LegalEntityIndexIdx" Type="String(Max) Null" ReferencedColumn="14c84e81-9e95-4cfa-98cf-62c786ba4bfe" />
		<SchemeReferencingColumn ID="ec87e18e-aa09-49ae-affd-b9638e0d5409" Name="LegalEntityIndexName" Type="String(Max) Null" ReferencedColumn="e2679580-3697-4965-b035-bb512133ac50" />
	</SchemeComplexColumn>
	<SchemeComplexColumn ID="74f98fe2-83d7-40db-8c5f-b76e6a2f2fdd" Name="Project" Type="Reference(Typified) Null" ReferencedTable="44de4db0-e013-441c-8441-4cb3ef09a649">
		<Description>Проект</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="74f98fe2-83d7-00db-4000-076e6a2f2fdd" Name="ProjectID" Type="Guid Null" ReferencedColumn="44de4db0-e013-011c-4000-0cb3ef09a649" />
		<SchemeReferencingColumn ID="57b3bd01-de50-4945-9a71-7cafa2f17383" Name="ProjectName" Type="String(Max) Null" ReferencedColumn="6c2fd8b3-e69b-4696-8a8a-2ab2d593af07" />
	</SchemeComplexColumn>
	<SchemeComplexColumn ID="f04b605f-65aa-496c-af2a-7afc7556a9d6" Name="Organization" Type="Reference(Typified) Null" ReferencedTable="303dd35d-b998-49b3-a10b-ed30d31bd36a">
		<Description>Организация ГК Пионер</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="f04b605f-65aa-006c-4000-0afc7556a9d6" Name="OrganizationID" Type="Guid Null" ReferencedColumn="303dd35d-b998-01b3-4000-0d30d31bd36a" />
		<SchemeReferencingColumn ID="8986e537-48d6-40c1-90a9-7127d91ed1c8" Name="OrganizationName" Type="String(Max) Null" ReferencedColumn="d8ecabaa-f192-4546-9af6-1f9712daabdb" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn ID="a31f576e-c2b6-49fe-ab44-52cfcfd72964" Name="Comments" Type="String(Max) Null">
		<Description>Комментарии к документу</Description>
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="e146d022-a349-4497-8ef2-0398f7e39f5c" Name="Contacts" Type="String(Max) Null">
		<Description>Контакты</Description>
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="a3cbf64c-e5f8-4e43-816e-08b5b43dd00e" Name="ApartmentNumber" Type="String(Max) Null">
		<Description>Номер квартиры</Description>
	</SchemePhysicalColumn>
	<SchemeComplexColumn ID="cde8153f-d899-4ee4-96c9-f837733f865b" Name="ComplaintKind" Type="Reference(Typified) Null" ReferencedTable="ea8131ae-ac8c-4f9c-814a-1698efd714ee">
		<Description>Вид рекламации</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="cde8153f-d899-00e4-4000-0837733f865b" Name="ComplaintKindID" Type="Guid Null" ReferencedColumn="ea8131ae-ac8c-019c-4000-0698efd714ee" />
		<SchemeReferencingColumn ID="6bf75a5a-ed37-445b-869a-6b87c758ec1f" Name="ComplaintKindName" Type="String(Max) Null" ReferencedColumn="f5e4f06c-41ba-4654-8ead-f142dc6bda82" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn ID="a01205b2-60dd-4a36-a915-2844b4c3245e" Name="FullName" Type="String(Max) Null">
		<Description>ФИО</Description>
	</SchemePhysicalColumn>
	<SchemeComplexColumn ID="6ef264a0-75bd-4598-a435-762b1427a941" Name="Department" Type="Reference(Typified) Null" ReferencedTable="81f6010b-9641-4aa5-8897-b8e8603fbf4b">
		<Description>Подразделение</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="6ef264a0-75bd-0098-4000-062b1427a941" Name="DepartmentID" Type="Guid Null" ReferencedColumn="81f6010b-9641-01a5-4000-08e8603fbf4b" />
		<SchemeReferencingColumn ID="d54ffb76-4e95-4b27-b180-47cb104d8d1a" Name="DepartmentName" Type="String(128) Null" ReferencedColumn="616d6b2e-35d5-424d-846b-618eb25962d0" />
		<SchemeReferencingColumn ID="3d723875-05ba-4d81-aa33-29dca6d39450" Name="DepartmentIdx" Type="String(Max) Null" ReferencedColumn="1f7ca0d4-1b64-43f8-b973-d14791af51a0" />
	</SchemeComplexColumn>
	<SchemeComplexColumn ID="fdc0afcc-e023-4dc3-b7af-01fa9a13eaa2" Name="Signatory" Type="Reference(Typified) Null" ReferencedTable="6c977939-bbfc-456f-a133-f1c2244e3cc3">
		<Description>Подписант</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="fdc0afcc-e023-00c3-4000-01fa9a13eaa2" Name="SignatoryID" Type="Guid Null" ReferencedColumn="6c977939-bbfc-016f-4000-01c2244e3cc3" />
		<SchemeReferencingColumn ID="99fa61ae-5f2f-45bb-b886-b2a62cf1d0b7" Name="SignatoryName" Type="String(128) Null" ReferencedColumn="1782f76a-4743-4aa4-920c-7edaee860964" />
	</SchemeComplexColumn>
	<SchemeComplexColumn ID="bea035f1-e7a5-4051-ac91-eaf2c58eaa79" Name="DocumentKind" Type="Reference(Typified) Null" ReferencedTable="60cd9278-5793-42b4-b159-c185da3788f1">
		<Description>Вид исходящего документа</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="bea035f1-e7a5-0051-4000-0af2c58eaa79" Name="DocumentKindID" Type="Guid Null" ReferencedColumn="60cd9278-5793-01b4-4000-0185da3788f1" />
		<SchemeReferencingColumn ID="4fadd497-f213-470b-a513-31c8cc51e0f2" Name="DocumentKindIdx" Type="String(Max) Null" ReferencedColumn="22c5829b-e76e-4969-ba94-9463d6a6e5b8" />
		<SchemeReferencingColumn ID="e8c2dfef-47f4-4404-88d7-89a8c673de1f" Name="DocumentKindName" Type="String(Max) Null" ReferencedColumn="d85714ae-c4d9-47ac-8d05-d64d4c85634f" />
	</SchemeComplexColumn>
	<SchemeComplexColumn ID="d1223967-282b-4849-bee7-834f8caa17a0" Name="WriteOff" Type="Reference(Typified) Null" ReferencedTable="81f6010b-9641-4aa5-8897-b8e8603fbf4b">
		<Description>Списать в дело</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="d1223967-282b-0049-4000-034f8caa17a0" Name="WriteOffID" Type="Guid Null" ReferencedColumn="81f6010b-9641-01a5-4000-08e8603fbf4b" />
		<SchemeReferencingColumn ID="4bf0799e-a8fd-46be-84a1-bb61685ca215" Name="WriteOffName" Type="String(128) Null" ReferencedColumn="616d6b2e-35d5-424d-846b-618eb25962d0" />
		<SchemeReferencingColumn ID="a5b1e162-fbae-44fb-ac38-c5d425cdf0a0" Name="WriteOffIdx" Type="String(Max) Null" ReferencedColumn="1f7ca0d4-1b64-43f8-b973-d14791af51a0" />
	</SchemeComplexColumn>
	<SchemeComplexColumn ID="38e16213-63e5-4740-943e-b4401554133d" Name="ComplaintFormat" Type="Reference(Typified) Null" ReferencedTable="36de7b98-236e-4cf4-b77d-e16efa400939">
		<Description>Формат рекламации</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="38e16213-63e5-0040-4000-04401554133d" Name="ComplaintFormatID" Type="Guid Null" ReferencedColumn="36de7b98-236e-01f4-4000-016efa400939" />
		<SchemeReferencingColumn ID="02dd4150-988f-4b33-ba78-bfe93899b6e1" Name="ComplaintFormatName" Type="String(Max) Null" ReferencedColumn="076e7d9e-f973-4b13-afaa-ed6165032c77" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn ID="239a7d4a-fc3b-4816-9594-a9c859b1d122" Name="ExtID" Type="Guid Null">
		<Description>GUID карточки из системы заказчика</Description>
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="107d2d80-5598-4394-8537-57a1ddaad100" Name="DestinationFIO" Type="String(Max) Null">
		<Description>ФИО контрагента</Description>
	</SchemePhysicalColumn>
	<SchemeComplexColumn ID="8511efa2-5051-46ef-ba6e-c91a2c342042" Name="FullNameRef" Type="Reference(Typified) Null" ReferencedTable="5d47ef13-b6f4-47ef-9815-3b3d0e6d475a">
		<Description>ФИО</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="8511efa2-5051-00ef-4000-091a2c342042" Name="FullNameRefID" Type="Guid Null" ReferencedColumn="5d47ef13-b6f4-01ef-4000-0b3d0e6d475a" />
		<SchemeReferencingColumn ID="c1503296-7950-4b3a-9730-8094e15ddcbb" Name="FullNameRefName" Type="String(255) Null" ReferencedColumn="f1c960e0-951e-4837-8474-bb61d98f40f0" />
	</SchemeComplexColumn>
	<SchemePrimaryKey IsSystem="true" IsPermanent="true" IsSealed="true" ID="5cc50e05-889f-0088-5000-06443de62b28" Name="pk_PnrOutgoing" IsClustered="true">
		<SchemeIndexedColumn Column="5cc50e05-889f-0188-4000-06443de62b28" />
	</SchemePrimaryKey>
	<SchemeIndex ID="78dfbf59-5294-40dc-8cad-e71628a9fe4f" Name="ndx_PnrOutgoing_IDDestinationName">
		<SchemeIndexedColumn Column="5cc50e05-889f-0188-4000-06443de62b28" />
		<SchemeIndexedColumn Column="f5963de3-fde7-4182-a92f-e99a184790dd" />
	</SchemeIndex>
</SchemeTable>