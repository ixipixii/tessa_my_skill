<?xml version="1.0" encoding="utf-8"?>
<SchemeTable Partition="54ed79f5-11c0-4f74-b8b0-793d596699d6" ID="5b315e88-7b15-4f6e-8007-70ff02f02a72" Name="PnrIncomingUK" Group="Pnr" InstanceType="Cards" ContentType="Entries">
	<Description>Входящие УК</Description>
	<SchemeComplexColumn IsSystem="true" IsPermanent="true" IsSealed="true" ID="5b315e88-7b15-006e-2000-00ff02f02a72" Name="ID" Type="Reference(Typified) Not Null" ReferencedTable="1074eadd-21d7-4925-98c8-40d1e5f0ca0e">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="5b315e88-7b15-016e-4000-00ff02f02a72" Name="ID" Type="Guid Not Null" ReferencedColumn="9a58123b-b2e9-4137-9c6c-5dab0ec02747" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn ID="ff648f8a-8f5e-43ec-8946-b425c0c96db2" Name="RegistrationDate" Type="Date Null">
		<Description>Дата регистрации</Description>
	</SchemePhysicalColumn>
	<SchemeComplexColumn ID="1b9b89c3-3b6e-4a4b-9619-dcb038d09889" Name="LegalEntityIndex" Type="Reference(Typified) Null" ReferencedTable="00e3d618-aa1e-4bac-9297-3010b710425c">
		<Description>Индекс ЮЛ УК ПС</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="1b9b89c3-3b6e-004b-4000-0cb038d09889" Name="LegalEntityIndexID" Type="Guid Null" ReferencedColumn="00e3d618-aa1e-01ac-4000-0010b710425c" />
		<SchemeReferencingColumn ID="4cb54546-51b0-4da8-8d6b-72af5e7412a1" Name="LegalEntityIndexIdx" Type="String(Max) Null" ReferencedColumn="14c84e81-9e95-4cfa-98cf-62c786ba4bfe" />
	</SchemeComplexColumn>
	<SchemeComplexColumn ID="ad91839a-27b0-4c16-9303-a4a3bace7e51" Name="Organization" Type="Reference(Typified) Null" ReferencedTable="303dd35d-b998-49b3-a10b-ed30d31bd36a">
		<Description>Организация ГК Пионер</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="ad91839a-27b0-0016-4000-04a3bace7e51" Name="OrganizationID" Type="Guid Null" ReferencedColumn="303dd35d-b998-01b3-4000-0d30d31bd36a" />
		<SchemeReferencingColumn ID="1df1c36e-77f2-4b6c-9357-826623c98294" Name="OrganizationName" Type="String(Max) Null" ReferencedColumn="d8ecabaa-f192-4546-9af6-1f9712daabdb" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn ID="0a82e2fb-a731-4f9b-9541-93898b9feb7e" Name="FullName" Type="String(Max) Null">
		<Description>Ф.И.О.</Description>
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="b7604ffe-87eb-4d2e-a762-83b4279a99dd" Name="Comments" Type="String(Max) Null">
		<Description>Комментарий</Description>
	</SchemePhysicalColumn>
	<SchemeComplexColumn ID="db573a36-c832-4383-9253-28ad56c91249" Name="City" Type="Reference(Typified) Null" ReferencedTable="75aa0d06-9bff-4e01-96b6-2ca62af269ae">
		<Description>Город</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="db573a36-c832-0083-4000-08ad56c91249" Name="CityID" Type="Guid Null" ReferencedColumn="75aa0d06-9bff-0101-4000-0ca62af269ae" />
		<SchemeReferencingColumn ID="b31613f5-6cd3-47dc-b5ff-755439fc6c16" Name="CityName" Type="String(Max) Null" ReferencedColumn="e70abb74-73d9-4232-ac52-a8ff12c357f5" />
	</SchemeComplexColumn>
	<SchemeComplexColumn ID="aa180301-d15d-4751-892f-136d3475195e" Name="DocumentKind" Type="Reference(Typified) Null" ReferencedTable="49fe89ac-890e-461e-bb09-29c6cbfb5774">
		<Description>Вид входящего документа УК ПС</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="aa180301-d15d-0051-4000-036d3475195e" Name="DocumentKindID" Type="Guid Null" ReferencedColumn="49fe89ac-890e-011e-4000-09c6cbfb5774" />
		<SchemeReferencingColumn ID="5e291ac1-7c79-41a0-b056-8ec8f57e449a" Name="DocumentKindName" Type="String(Max) Null" ReferencedColumn="6e5369cd-220b-4b88-a0a3-61e50b984612" />
		<SchemeReferencingColumn ID="1dc6add4-7ecc-40fe-b1a9-62af41faf2f6" Name="DocumentKindIdx" Type="String(Max) Null" ReferencedColumn="a03a7f49-19bb-4801-a86d-f23b844734bd" />
	</SchemeComplexColumn>
	<SchemeComplexColumn ID="fa4df883-9171-4930-b2b2-f19ab6ecf706" Name="Original" Type="Reference(Typified) Null" ReferencedTable="df7dfdfc-512a-463e-85e6-5ffdda9170e6">
		<Description>Оригинал</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="fa4df883-9171-0030-4000-019ab6ecf706" Name="OriginalID" Type="Guid Null" ReferencedColumn="df7dfdfc-512a-013e-4000-0ffdda9170e6" />
		<SchemeReferencingColumn ID="2932ce72-ced4-4259-a0c9-e5c23b41d998" Name="OriginalName" Type="String(Max) Null" ReferencedColumn="dde49959-2cf8-48cc-a237-ddc570e99536" />
	</SchemeComplexColumn>
	<SchemeComplexColumn ID="a20315dc-c54c-42f5-9261-3d67dccb7d90" Name="Correspondent" Type="Reference(Typified) Null" ReferencedTable="5d47ef13-b6f4-47ef-9815-3b3d0e6d475a">
		<Description>Корреспондент</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="a20315dc-c54c-00f5-4000-0d67dccb7d90" Name="CorrespondentID" Type="Guid Null" ReferencedColumn="5d47ef13-b6f4-01ef-4000-0b3d0e6d475a" />
		<SchemeReferencingColumn ID="ff9238a6-f7ac-4a80-9c57-36a29c6fdc88" Name="CorrespondentName" Type="String(255) Null" ReferencedColumn="f1c960e0-951e-4837-8474-bb61d98f40f0" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn ID="453cf6cc-38dd-48b9-a53d-9edb0987f4e1" Name="ExternalNumber" Type="String(Max) Null">
		<Description>№ внешний</Description>
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="869e8f52-ef2e-44d9-b262-dd9a976aeb0b" Name="ExternalDate" Type="Date Null">
		<Description>Дата внешняя</Description>
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="e1bab36c-71f3-4c1c-aa19-e4d20f43e620" Name="Summary" Type="String(Max) Null">
		<Description>Краткое содержание</Description>
	</SchemePhysicalColumn>
	<SchemeComplexColumn ID="de53ea02-56a0-484b-bafd-8ef91385b387" Name="DeliveryType" Type="Reference(Typified) Null" ReferencedTable="e84635a0-4790-4b3b-b7d4-7bd4472cef35">
		<Description>Тип доставки</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="de53ea02-56a0-004b-4000-0ef91385b387" Name="DeliveryTypeID" Type="Guid Null" ReferencedColumn="e84635a0-4790-013b-4000-0bd4472cef35" />
		<SchemeReferencingColumn ID="77597113-7ff9-42dc-9c34-63f875c4f326" Name="DeliveryTypeName" Type="String(Max) Null" ReferencedColumn="4345bc79-699c-4a2a-8c8f-e5ff9165203e" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn ID="4dc0df8a-0744-4714-93e2-57d75687ece8" Name="MailID" Type="String(Max) Null">
		<Description>Почтовый идентификатор</Description>
	</SchemePhysicalColumn>
	<SchemeComplexColumn ID="2d2c892f-17e7-4056-9ca8-bdd2a8b814e4" Name="Department" Type="Reference(Typified) Null" ReferencedTable="81f6010b-9641-4aa5-8897-b8e8603fbf4b">
		<Description>Подразделение</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="2d2c892f-17e7-0056-4000-0dd2a8b814e4" Name="DepartmentID" Type="Guid Null" ReferencedColumn="81f6010b-9641-01a5-4000-08e8603fbf4b" />
		<SchemeReferencingColumn ID="12e6c0b6-c8a3-494e-8899-fa29d9ef3cca" Name="DepartmentIdx" Type="String(Max) Null" ReferencedColumn="1f7ca0d4-1b64-43f8-b973-d14791af51a0" />
		<SchemeReferencingColumn ID="b0bd69b7-5d60-41e8-b6d7-7deadc4ef033" Name="DepartmentName" Type="String(128) Null" ReferencedColumn="616d6b2e-35d5-424d-846b-618eb25962d0" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn ID="3f4839b7-943c-41f1-b3a6-1244a212b60e" Name="Contacts" Type="String(Max) Null">
		<Description>Контакты</Description>
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="efe2b7e6-bf76-447c-b662-94055c5d9966" Name="Housing" Type="String(Max) Null">
		<Description>Корпус</Description>
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="91daf2bd-7e07-45fb-bc8b-010235745380" Name="ApartmentNumber" Type="String(Max) Null">
		<Description>Номер квартиры</Description>
	</SchemePhysicalColumn>
	<SchemeComplexColumn ID="9b71ae2a-c171-436a-8777-96579b666778" Name="ComplaintKind" Type="Reference(Typified) Null" ReferencedTable="ea8131ae-ac8c-4f9c-814a-1698efd714ee">
		<Description>Вид рекламации</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="9b71ae2a-c171-006a-4000-06579b666778" Name="ComplaintKindID" Type="Guid Null" ReferencedColumn="ea8131ae-ac8c-019c-4000-0698efd714ee" />
		<SchemeReferencingColumn ID="b4fdc437-d14c-4782-83ae-81d7d1b00a44" Name="ComplaintKindName" Type="String(Max) Null" ReferencedColumn="f5e4f06c-41ba-4654-8ead-f142dc6bda82" />
	</SchemeComplexColumn>
	<SchemeComplexColumn ID="938a96e4-9182-49c3-b42d-e89a7b99e2f2" Name="ComplaintFormat" Type="Reference(Typified) Null" ReferencedTable="36de7b98-236e-4cf4-b77d-e16efa400939">
		<Description>Формат рекламации</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="938a96e4-9182-00c3-4000-089a7b99e2f2" Name="ComplaintFormatID" Type="Guid Null" ReferencedColumn="36de7b98-236e-01f4-4000-016efa400939" />
		<SchemeReferencingColumn ID="827bc985-aaab-43fe-9e3e-fcbb92dc66cf" Name="ComplaintFormatName" Type="String(Max) Null" ReferencedColumn="076e7d9e-f973-4b13-afaa-ed6165032c77" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn ID="1f361f53-d8e9-4bdf-93ce-731bdc0facfb" Name="ExtID" Type="Guid Null">
		<Description>GUID карточки из системы заказчика</Description>
	</SchemePhysicalColumn>
	<SchemeComplexColumn ID="00431764-3bdf-4f24-b744-314790680b88" Name="Approver" Type="Reference(Typified) Null" ReferencedTable="6c977939-bbfc-456f-a133-f1c2244e3cc3">
		<Description>Утверждающий</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="00431764-3bdf-0024-4000-014790680b88" Name="ApproverID" Type="Guid Null" ReferencedColumn="6c977939-bbfc-016f-4000-01c2244e3cc3" />
		<SchemeReferencingColumn ID="54b02063-59a5-4a7f-b777-856f0ed45b3a" Name="ApproverName" Type="String(128) Null" ReferencedColumn="1782f76a-4743-4aa4-920c-7edaee860964" />
	</SchemeComplexColumn>
	<SchemePrimaryKey IsSystem="true" IsPermanent="true" IsSealed="true" ID="5b315e88-7b15-006e-5000-00ff02f02a72" Name="pk_PnrIncomingUK" IsClustered="true">
		<SchemeIndexedColumn Column="5b315e88-7b15-016e-4000-00ff02f02a72" />
	</SchemePrimaryKey>
</SchemeTable>