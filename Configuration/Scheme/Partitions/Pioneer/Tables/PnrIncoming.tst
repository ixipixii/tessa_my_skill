<?xml version="1.0" encoding="utf-8"?>
<SchemeTable Partition="54ed79f5-11c0-4f74-b8b0-793d596699d6" ID="4008401e-f670-4ce9-8f08-5ca4fca330de" Name="PnrIncoming" Group="Pnr" InstanceType="Cards" ContentType="Entries">
	<Description>Входящий документ</Description>
	<SchemeComplexColumn IsSystem="true" IsPermanent="true" IsSealed="true" ID="4008401e-f670-00e9-2000-0ca4fca330de" Name="ID" Type="Reference(Typified) Not Null" ReferencedTable="1074eadd-21d7-4925-98c8-40d1e5f0ca0e">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="4008401e-f670-01e9-4000-0ca4fca330de" Name="ID" Type="Guid Not Null" ReferencedColumn="9a58123b-b2e9-4137-9c6c-5dab0ec02747" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn ID="b2889ca4-46ae-4ae2-99c6-992d9ef2461a" Name="RegistrationDate" Type="Date Null">
		<Description>Дата регистрации</Description>
	</SchemePhysicalColumn>
	<SchemeComplexColumn ID="91ec7dd7-03a4-4ab1-8cda-6b0bbaa03b13" Name="Correspondent" Type="Reference(Typified) Null" ReferencedTable="5d47ef13-b6f4-47ef-9815-3b3d0e6d475a">
		<Description>Корреспондент (организация)</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="91ec7dd7-03a4-00b1-4000-0b0bbaa03b13" Name="CorrespondentID" Type="Guid Null" ReferencedColumn="5d47ef13-b6f4-01ef-4000-0b3d0e6d475a" />
		<SchemeReferencingColumn ID="be457b91-b56b-4163-b745-32a5965bc173" Name="CorrespondentName" Type="String(255) Null" ReferencedColumn="f1c960e0-951e-4837-8474-bb61d98f40f0" />
	</SchemeComplexColumn>
	<SchemeComplexColumn ID="0ca2ba44-6768-400e-873c-5167600450fb" Name="Original" Type="Reference(Typified) Null" ReferencedTable="df7dfdfc-512a-463e-85e6-5ffdda9170e6">
		<Description>Оригинал</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="0ca2ba44-6768-000e-4000-0167600450fb" Name="OriginalID" Type="Guid Null" ReferencedColumn="df7dfdfc-512a-013e-4000-0ffdda9170e6" />
		<SchemeReferencingColumn ID="df416b7e-5bdd-4ab9-9836-6408634ebcbb" Name="OriginalName" Type="String(Max) Null" ReferencedColumn="dde49959-2cf8-48cc-a237-ddc570e99536" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn ID="0a042e27-40fd-4b72-87fc-48830434f9d7" Name="FullName" Type="String(Max) Null">
		<Description>ФИО</Description>
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="0be03d78-ac92-48e1-b4ce-05b19356f4d0" Name="ExternalDate" Type="DateTime Null">
		<Description>Дата внешняя</Description>
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="6db6f0bc-4b91-44ae-b10f-e70af7cde944" Name="ExternalNumber" Type="String(Max) Null">
		<Description>№ внешний</Description>
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="5389cd5a-989b-4ca6-946b-0869daea97fb" Name="Summary" Type="String(Max) Null">
		<Description>Краткое содержание</Description>
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="ecfa96e2-1dcc-43a9-b9bc-e0ea9f14db55" Name="Comments" Type="String(Max) Null">
		<Description>Комментарии к документу</Description>
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="1d78a5bf-b53a-48bd-9934-4cf66ae97eaa" Name="MailID" Type="String(Max) Null">
		<Description>Почтовый идентификатор</Description>
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="9aeb9f68-c15e-4ab7-a701-3d12fbba6dcf" Name="Contacts" Type="String(Max) Null">
		<Description>Контакты</Description>
	</SchemePhysicalColumn>
	<SchemeComplexColumn ID="d94015fc-e09a-40a4-b279-ca6cf4dfb683" Name="Project" Type="Reference(Typified) Null" ReferencedTable="44de4db0-e013-441c-8441-4cb3ef09a649">
		<Description>Проект</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="d94015fc-e09a-00a4-4000-0a6cf4dfb683" Name="ProjectID" Type="Guid Null" ReferencedColumn="44de4db0-e013-011c-4000-0cb3ef09a649" />
		<SchemeReferencingColumn ID="c5f5b4b4-2172-458b-90d4-975d9c8f8442" Name="ProjectName" Type="String(Max) Null" ReferencedColumn="6c2fd8b3-e69b-4696-8a8a-2ab2d593af07" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn ID="d513f92d-b578-4991-9d4c-014701876551" Name="ApartmentNumber" Type="String(Max) Null">
		<Description>Номер квартиры</Description>
	</SchemePhysicalColumn>
	<SchemeComplexColumn ID="4692424a-8fea-47bb-93d7-1b35715d7e30" Name="ComplaintKind" Type="Reference(Typified) Null" ReferencedTable="ea8131ae-ac8c-4f9c-814a-1698efd714ee">
		<Description>Вид рекламации</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="4692424a-8fea-00bb-4000-0b35715d7e30" Name="ComplaintKindID" Type="Guid Null" ReferencedColumn="ea8131ae-ac8c-019c-4000-0698efd714ee" />
		<SchemeReferencingColumn ID="c16edbfb-b560-4cef-86d1-7cfdfdd7efc8" Name="ComplaintKindName" Type="String(Max) Null" ReferencedColumn="f5e4f06c-41ba-4654-8ead-f142dc6bda82" />
	</SchemeComplexColumn>
	<SchemeComplexColumn ID="8ca1e06b-6369-46f4-aa33-991bec561c9d" Name="DeliveryType" Type="Reference(Typified) Null" ReferencedTable="e84635a0-4790-4b3b-b7d4-7bd4472cef35">
		<Description>Тип доставки</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="8ca1e06b-6369-00f4-4000-091bec561c9d" Name="DeliveryTypeID" Type="Guid Null" ReferencedColumn="e84635a0-4790-013b-4000-0bd4472cef35" />
		<SchemeReferencingColumn ID="338b5b0d-9dea-4e92-97d3-ac954e7bfaa4" Name="DeliveryTypeName" Type="String(Max) Null" ReferencedColumn="4345bc79-699c-4a2a-8c8f-e5ff9165203e" />
	</SchemeComplexColumn>
	<SchemeComplexColumn ID="93cad4b5-c2fb-4102-a07c-90761d265cf8" Name="Department" Type="Reference(Typified) Null" ReferencedTable="81f6010b-9641-4aa5-8897-b8e8603fbf4b">
		<Description>Подразделение</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="93cad4b5-c2fb-0002-4000-00761d265cf8" Name="DepartmentID" Type="Guid Null" ReferencedColumn="81f6010b-9641-01a5-4000-08e8603fbf4b" />
		<SchemeReferencingColumn ID="2bf8b620-1397-4693-bc55-79fb48ad4b12" Name="DepartmentName" Type="String(128) Null" ReferencedColumn="616d6b2e-35d5-424d-846b-618eb25962d0" />
		<SchemeReferencingColumn ID="852bee8e-65eb-4792-af53-6478cc2b4d0e" Name="DepartmentIdx" Type="String(Max) Null" ReferencedColumn="1f7ca0d4-1b64-43f8-b973-d14791af51a0" />
	</SchemeComplexColumn>
	<SchemeComplexColumn ID="3d21a762-0790-4318-91bb-04b7ad8bea36" Name="LegalEntityIndex" Type="Reference(Typified) Null" ReferencedTable="00e3d618-aa1e-4bac-9297-3010b710425c">
		<Description>Индекс ЮЛ</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="3d21a762-0790-0018-4000-04b7ad8bea36" Name="LegalEntityIndexID" Type="Guid Null" ReferencedColumn="00e3d618-aa1e-01ac-4000-0010b710425c" />
		<SchemeReferencingColumn ID="00025f69-58a3-4632-8f42-48cc2262d796" Name="LegalEntityIndexIdx" Type="String(Max) Null" ReferencedColumn="14c84e81-9e95-4cfa-98cf-62c786ba4bfe" />
	</SchemeComplexColumn>
	<SchemeComplexColumn ID="026b77b8-bca7-45bd-99e1-0970cf7878a0" Name="DocumentKind" Type="Reference(Typified) Null" ReferencedTable="d115b79e-50eb-442e-a659-9bcc6ddc8c80">
		<Description>Вид входящего документа</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="026b77b8-bca7-00bd-4000-0970cf7878a0" Name="DocumentKindID" Type="Guid Null" ReferencedColumn="d115b79e-50eb-012e-4000-0bcc6ddc8c80" />
		<SchemeReferencingColumn ID="57135c3c-66ae-4e6c-aa2e-333d37003f96" Name="DocumentKindIdx" Type="String(Max) Null" ReferencedColumn="cc2d39e5-6ea9-4dbf-a7ef-b4070e2f1fdc" />
		<SchemeReferencingColumn ID="3a0b8a66-a21b-4724-9a87-c6d238bdfdef" Name="DocumentKindName" Type="String(Max) Null" ReferencedColumn="1988a27c-81d8-4d96-812b-dda2afbb36ed" />
	</SchemeComplexColumn>
	<SchemeComplexColumn ID="f9973d7b-3b83-432e-9f23-2df20031e71e" Name="WriteOff" Type="Reference(Typified) Null" ReferencedTable="81f6010b-9641-4aa5-8897-b8e8603fbf4b">
		<Description>Списать в дело</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="f9973d7b-3b83-002e-4000-0df20031e71e" Name="WriteOffID" Type="Guid Null" ReferencedColumn="81f6010b-9641-01a5-4000-08e8603fbf4b" />
		<SchemeReferencingColumn ID="d75b8bcd-16ff-4b0c-85fa-436ffeb6938b" Name="WriteOffName" Type="String(128) Null" ReferencedColumn="616d6b2e-35d5-424d-846b-618eb25962d0" />
		<SchemeReferencingColumn ID="1c1c2b8c-ec6e-41f5-b0c5-edae0d962e6c" Name="WriteOffIdx" Type="String(Max) Null" ReferencedColumn="1f7ca0d4-1b64-43f8-b973-d14791af51a0" />
	</SchemeComplexColumn>
	<SchemeComplexColumn ID="3d0beb3c-8210-48e3-810f-97bc7d04adb1" Name="ComplaintFormat" Type="Reference(Typified) Null" ReferencedTable="36de7b98-236e-4cf4-b77d-e16efa400939">
		<Description>Формат рекламации</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="3d0beb3c-8210-00e3-4000-07bc7d04adb1" Name="ComplaintFormatID" Type="Guid Null" ReferencedColumn="36de7b98-236e-01f4-4000-016efa400939" />
		<SchemeReferencingColumn ID="fa417352-fbcf-40a7-98a5-7449e0774e8f" Name="ComplaintFormatName" Type="String(Max) Null" ReferencedColumn="076e7d9e-f973-4b13-afaa-ed6165032c77" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn ID="8c72aea0-6fed-4f66-9c09-2002d6ea1f6e" Name="ExtID" Type="Guid Null">
		<Description>GUID карточки из системы заказчика</Description>
	</SchemePhysicalColumn>
	<SchemeComplexColumn ID="3aa0fcac-9ff2-424f-8616-b9ec09c6d07e" Name="FullNameRef" Type="Reference(Typified) Null" ReferencedTable="5d47ef13-b6f4-47ef-9815-3b3d0e6d475a">
		<Description>ФИО</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="3aa0fcac-9ff2-004f-4000-09ec09c6d07e" Name="FullNameRefID" Type="Guid Null" ReferencedColumn="5d47ef13-b6f4-01ef-4000-0b3d0e6d475a" />
		<SchemeReferencingColumn ID="ae96e722-1b0b-4595-8c79-41c62d2496e4" Name="FullNameRefName" Type="String(255) Null" ReferencedColumn="f1c960e0-951e-4837-8474-bb61d98f40f0" />
	</SchemeComplexColumn>
	<SchemePrimaryKey IsSystem="true" IsPermanent="true" IsSealed="true" ID="4008401e-f670-00e9-5000-0ca4fca330de" Name="pk_PnrIncoming" IsClustered="true">
		<SchemeIndexedColumn Column="4008401e-f670-01e9-4000-0ca4fca330de" />
	</SchemePrimaryKey>
	<SchemeIndex ID="009ccf6e-322a-4d18-a1f3-91e5805673e3" Name="ndx_PnrIncoming_IDCorrespondentName">
		<SchemeIndexedColumn Column="4008401e-f670-01e9-4000-0ca4fca330de" />
		<SchemeIndexedColumn Column="be457b91-b56b-4163-b745-32a5965bc173" />
	</SchemeIndex>
</SchemeTable>