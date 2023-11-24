<?xml version="1.0" encoding="utf-8"?>
<SchemeTable Partition="54ed79f5-11c0-4f74-b8b0-793d596699d6" ID="99a8f2ec-3a0f-4d02-84ed-25e04396fc8b" Name="PnrOutgoingUK" Group="Pnr" InstanceType="Cards" ContentType="Entries">
	<Description>Исходящие УК</Description>
	<SchemeComplexColumn IsSystem="true" IsPermanent="true" IsSealed="true" ID="99a8f2ec-3a0f-0002-2000-05e04396fc8b" Name="ID" Type="Reference(Typified) Not Null" ReferencedTable="1074eadd-21d7-4925-98c8-40d1e5f0ca0e">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="99a8f2ec-3a0f-0102-4000-05e04396fc8b" Name="ID" Type="Guid Not Null" ReferencedColumn="9a58123b-b2e9-4137-9c6c-5dab0ec02747" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn ID="4a8e6ae3-af80-44de-b7ac-681cc54b1329" Name="RegistrationDate" Type="Date Null">
		<Description>Дата регистрации</Description>
	</SchemePhysicalColumn>
	<SchemeComplexColumn ID="92fb87a5-2ba8-4cff-b052-ba30bb7d8e2f" Name="LegalEntityIndex" Type="Reference(Typified) Null" ReferencedTable="00e3d618-aa1e-4bac-9297-3010b710425c">
		<Description>Индекс юридического лица</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="92fb87a5-2ba8-00ff-4000-0a30bb7d8e2f" Name="LegalEntityIndexID" Type="Guid Null" ReferencedColumn="00e3d618-aa1e-01ac-4000-0010b710425c" />
		<SchemeReferencingColumn ID="29cfb604-5749-46c2-ba1b-5fc47f621bef" Name="LegalEntityIndexName" Type="String(Max) Null" ReferencedColumn="e2679580-3697-4965-b035-bb512133ac50" />
		<SchemeReferencingColumn ID="994a1eac-7da7-4dd9-96bf-3b665c10c623" Name="LegalEntityIndexIdx" Type="String(Max) Null" ReferencedColumn="14c84e81-9e95-4cfa-98cf-62c786ba4bfe" />
	</SchemeComplexColumn>
	<SchemeComplexColumn ID="baabb346-9646-476c-a1f0-d1e74aaff2f8" Name="Organization" Type="Reference(Typified) Null" ReferencedTable="303dd35d-b998-49b3-a10b-ed30d31bd36a">
		<Description>Организация ГК Пионер</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="baabb346-9646-006c-4000-01e74aaff2f8" Name="OrganizationID" Type="Guid Null" ReferencedColumn="303dd35d-b998-01b3-4000-0d30d31bd36a" />
		<SchemeReferencingColumn ID="b0547326-0bcf-4816-806f-f366e57aebe9" Name="OrganizationName" Type="String(Max) Null" ReferencedColumn="d8ecabaa-f192-4546-9af6-1f9712daabdb" />
	</SchemeComplexColumn>
	<SchemeComplexColumn ID="96c348b6-c010-40d3-ad2b-9280eea46e52" Name="Destination" Type="Reference(Typified) Null" ReferencedTable="5d47ef13-b6f4-47ef-9815-3b3d0e6d475a">
		<Description>Адресат</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="96c348b6-c010-00d3-4000-0280eea46e52" Name="DestinationID" Type="Guid Null" ReferencedColumn="5d47ef13-b6f4-01ef-4000-0b3d0e6d475a" />
		<SchemeReferencingColumn ID="51bd9a25-f9b6-4d0a-b540-9811aa395969" Name="DestinationName" Type="String(255) Null" ReferencedColumn="f1c960e0-951e-4837-8474-bb61d98f40f0" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn ID="302db0f3-d2f0-474f-9547-b48334002d45" Name="FullName" Type="String(Max) Null">
		<Description>ФИО</Description>
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="737f24f8-a7a7-4ddf-b5e6-a66923e05bce" Name="Comments" Type="String(Max) Null">
		<Description>Комментарии</Description>
	</SchemePhysicalColumn>
	<SchemeComplexColumn ID="4acef659-88bd-4ae7-a31f-32f4a53e2bbd" Name="City" Type="Reference(Typified) Null" ReferencedTable="75aa0d06-9bff-4e01-96b6-2ca62af269ae">
		<Description>Город</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="4acef659-88bd-00e7-4000-02f4a53e2bbd" Name="CityID" Type="Guid Null" ReferencedColumn="75aa0d06-9bff-0101-4000-0ca62af269ae" />
		<SchemeReferencingColumn ID="673d0435-f455-42c1-b71b-b908f93dd7c3" Name="CityName" Type="String(Max) Null" ReferencedColumn="e70abb74-73d9-4232-ac52-a8ff12c357f5" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn ID="f3bd3a3e-6d30-4607-b15f-31d4bf6d5162" Name="Summary" Type="String(Max) Null">
		<Description>Краткое содержание</Description>
	</SchemePhysicalColumn>
	<SchemeComplexColumn ID="699e4dac-9cd9-45b0-8ed8-95da722d8dcd" Name="Department" Type="Reference(Typified) Null" ReferencedTable="81f6010b-9641-4aa5-8897-b8e8603fbf4b">
		<Description>Подразделение</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="699e4dac-9cd9-00b0-4000-05da722d8dcd" Name="DepartmentID" Type="Guid Null" ReferencedColumn="81f6010b-9641-01a5-4000-08e8603fbf4b" />
		<SchemeReferencingColumn ID="aa2e38a6-3a77-44bd-ba3e-19b24187c510" Name="DepartmentName" Type="String(128) Null" ReferencedColumn="616d6b2e-35d5-424d-846b-618eb25962d0" />
		<SchemeReferencingColumn ID="70f0d7a8-8c41-4e29-9688-72a38221e8b0" Name="DepartmentIdx" Type="String(Max) Null" ReferencedColumn="1f7ca0d4-1b64-43f8-b973-d14791af51a0" />
	</SchemeComplexColumn>
	<SchemeComplexColumn ID="cc9007fc-e1c9-4474-a365-279430e25440" Name="Signatory" Type="Reference(Typified) Null" ReferencedTable="6c977939-bbfc-456f-a133-f1c2244e3cc3">
		<Description>Подписант</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="cc9007fc-e1c9-0074-4000-079430e25440" Name="SignatoryID" Type="Guid Null" ReferencedColumn="6c977939-bbfc-016f-4000-01c2244e3cc3" />
		<SchemeReferencingColumn ID="3c357885-4248-4901-a7e8-5e3084b36881" Name="SignatoryName" Type="String(128) Null" ReferencedColumn="1782f76a-4743-4aa4-920c-7edaee860964" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn ID="10d80c68-adbe-4ad9-a19e-4a5f8799dbbd" Name="ExtID" Type="Guid Null">
		<Description>GUID карточки из системы заказчика</Description>
	</SchemePhysicalColumn>
	<SchemeComplexColumn ID="fde41547-c47a-49fc-9b38-830cbbf23b50" Name="DocumentKind" Type="Reference(Typified) Null" ReferencedTable="451af913-aadf-456d-9fb1-a1b2f9b58197">
		<Description>Вид исходящего документа</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="fde41547-c47a-00fc-4000-030cbbf23b50" Name="DocumentKindID" Type="Guid Null" ReferencedColumn="451af913-aadf-016d-4000-01b2f9b58197" />
		<SchemeReferencingColumn ID="354a829b-4097-46f3-a1b3-26827b48c698" Name="DocumentKindIdx" Type="String(Max) Null" ReferencedColumn="816df957-c7b2-4863-80e7-d3f472b58446" />
		<SchemeReferencingColumn ID="54c23a36-5306-40c4-a58e-f6ea1f007c7b" Name="DocumentKindName" Type="String(Max) Null" ReferencedColumn="8edb9891-4aeb-40e6-bf41-e00ee087f07f" />
	</SchemeComplexColumn>
	<SchemeComplexColumn ID="8055ff2a-3bf6-4187-b59f-95fc6ff1b116" Name="Original" Type="Reference(Typified) Null" ReferencedTable="df7dfdfc-512a-463e-85e6-5ffdda9170e6">
		<Description>Оригинал</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="8055ff2a-3bf6-0087-4000-05fc6ff1b116" Name="OriginalID" Type="Guid Null" ReferencedColumn="df7dfdfc-512a-013e-4000-0ffdda9170e6" />
		<SchemeReferencingColumn ID="89d62ef2-0282-49e7-9ab4-5daeca13d742" Name="OriginalName" Type="String(Max) Null" ReferencedColumn="dde49959-2cf8-48cc-a237-ddc570e99536" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn ID="89c2118f-f24d-4d2e-a1fa-18b870a93e6a" Name="ExternalNumber" Type="String(Max) Null">
		<Description>№ внешний</Description>
	</SchemePhysicalColumn>
	<SchemeComplexColumn ID="06ec5c18-2fef-4959-8854-458632be43ea" Name="DeliveryType" Type="Reference(Typified) Null" ReferencedTable="e84635a0-4790-4b3b-b7d4-7bd4472cef35">
		<Description>Тип доставки</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="06ec5c18-2fef-0059-4000-058632be43ea" Name="DeliveryTypeID" Type="Guid Null" ReferencedColumn="e84635a0-4790-013b-4000-0bd4472cef35" />
		<SchemeReferencingColumn ID="e8950ac7-305a-41c7-a1ba-0f314390dfe7" Name="DeliveryTypeName" Type="String(Max) Null" ReferencedColumn="4345bc79-699c-4a2a-8c8f-e5ff9165203e" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn ID="6d45e062-73fe-4081-b9de-ddfd362ef371" Name="Contacts" Type="String(Max) Null">
		<Description>Контакты</Description>
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="70f9aa8c-2d60-46d8-b6b9-c55e4785e505" Name="ApartmentNumber" Type="String(Max) Null">
		<Description>Номер квартиры</Description>
	</SchemePhysicalColumn>
	<SchemeComplexColumn ID="e5361c31-d7ec-4fab-a548-a520030c9b67" Name="ComplaintKind" Type="Reference(Typified) Null" ReferencedTable="ea8131ae-ac8c-4f9c-814a-1698efd714ee">
		<Description>Вид рекламации</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="e5361c31-d7ec-00ab-4000-0520030c9b67" Name="ComplaintKindID" Type="Guid Null" ReferencedColumn="ea8131ae-ac8c-019c-4000-0698efd714ee" />
		<SchemeReferencingColumn ID="d07694bd-12a6-4249-a65f-33573434683f" Name="ComplaintKindName" Type="String(Max) Null" ReferencedColumn="f5e4f06c-41ba-4654-8ead-f142dc6bda82" />
	</SchemeComplexColumn>
	<SchemeComplexColumn ID="a1af0263-e3d9-4198-98a5-157000dd8baa" Name="ComplaintFormat" Type="Reference(Typified) Null" ReferencedTable="36de7b98-236e-4cf4-b77d-e16efa400939">
		<Description>Формат рекламации</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="a1af0263-e3d9-0098-4000-057000dd8baa" Name="ComplaintFormatID" Type="Guid Null" ReferencedColumn="36de7b98-236e-01f4-4000-016efa400939" />
		<SchemeReferencingColumn ID="7a4c76b9-c97c-482d-918e-d1d14cc41bf8" Name="ComplaintFormatName" Type="String(Max) Null" ReferencedColumn="076e7d9e-f973-4b13-afaa-ed6165032c77" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn ID="9be97a6f-a452-4bf4-81ae-a79d8fbea376" Name="Housing" Type="String(Max) Null">
		<Description>Корпус</Description>
	</SchemePhysicalColumn>
	<SchemeComplexColumn ID="f2710dbb-3108-47b7-97ac-87b8e5ca20b5" Name="Approver" Type="Reference(Typified) Null" ReferencedTable="6c977939-bbfc-456f-a133-f1c2244e3cc3">
		<Description>Утверждающий</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="f2710dbb-3108-00b7-4000-07b8e5ca20b5" Name="ApproverID" Type="Guid Null" ReferencedColumn="6c977939-bbfc-016f-4000-01c2244e3cc3" />
		<SchemeReferencingColumn ID="b74096cf-6975-4452-8e48-bbc95224de8b" Name="ApproverName" Type="String(128) Null" ReferencedColumn="1782f76a-4743-4aa4-920c-7edaee860964" />
	</SchemeComplexColumn>
	<SchemePrimaryKey IsSystem="true" IsPermanent="true" IsSealed="true" ID="99a8f2ec-3a0f-0002-5000-05e04396fc8b" Name="pk_PnrOutgoingUK" IsClustered="true">
		<SchemeIndexedColumn Column="99a8f2ec-3a0f-0102-4000-05e04396fc8b" />
	</SchemePrimaryKey>
</SchemeTable>