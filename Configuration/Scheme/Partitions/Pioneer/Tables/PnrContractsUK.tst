<?xml version="1.0" encoding="utf-8"?>
<SchemeTable Partition="54ed79f5-11c0-4f74-b8b0-793d596699d6" ID="f061690d-d75c-4806-8097-9dc308ed6984" Name="PnrContractsUK" Group="Pnr" InstanceType="Cards" ContentType="Entries">
	<Description>Договор УК</Description>
	<SchemeComplexColumn IsSystem="true" IsPermanent="true" IsSealed="true" ID="f061690d-d75c-0006-2000-0dc308ed6984" Name="ID" Type="Reference(Typified) Not Null" ReferencedTable="1074eadd-21d7-4925-98c8-40d1e5f0ca0e">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="f061690d-d75c-0106-4000-0dc308ed6984" Name="ID" Type="Guid Not Null" ReferencedColumn="9a58123b-b2e9-4137-9c6c-5dab0ec02747" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn ID="b9e0ef87-dcb3-4211-a1f4-d4c31c3e15d0" Name="ExternalNumber" Type="String(Max) Null">
		<Description>Внешний номер</Description>
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="70ecfb99-0751-4efb-a485-5c55c5437254" Name="RegistrationDate" Type="Date Null">
		<Description>Дата регистрации</Description>
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="567fcfb7-285b-4468-8423-d339ad9e4c4d" Name="Subject" Type="String(Max) Null">
		<Description>Предмет договора</Description>
	</SchemePhysicalColumn>
	<SchemeComplexColumn ID="fea73160-7ed6-4095-a99d-2d2e0991af89" Name="Organization" Type="Reference(Typified) Null" ReferencedTable="303dd35d-b998-49b3-a10b-ed30d31bd36a">
		<Description>Организация ГК Пионер</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="fea73160-7ed6-0095-4000-0d2e0991af89" Name="OrganizationID" Type="Guid Null" ReferencedColumn="303dd35d-b998-01b3-4000-0d30d31bd36a" />
		<SchemeReferencingColumn ID="8fdbac6f-1aee-426d-a0c1-7cce25b9512d" Name="OrganizationName" Type="String(Max) Null" ReferencedColumn="d8ecabaa-f192-4546-9af6-1f9712daabdb" />
	</SchemeComplexColumn>
	<SchemeComplexColumn ID="193e196e-83e0-412b-90f6-e51faf98ec07" Name="Partner" Type="Reference(Typified) Null" ReferencedTable="5d47ef13-b6f4-47ef-9815-3b3d0e6d475a">
		<Description>Контрагент</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="193e196e-83e0-002b-4000-051faf98ec07" Name="PartnerID" Type="Guid Null" ReferencedColumn="5d47ef13-b6f4-01ef-4000-0b3d0e6d475a" />
		<SchemeReferencingColumn ID="e0933bc0-2919-486c-9d84-12e3fd84fa73" Name="PartnerName" Type="String(255) Null" ReferencedColumn="f1c960e0-951e-4837-8474-bb61d98f40f0" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn ID="efe87eee-1789-426d-ac3f-462bcd693bb6" Name="StartDate" Type="Date Null">
		<Description>Дата начала</Description>
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="f77a52b6-c735-424e-bdaa-b5a733d03c79" Name="EndDate" Type="Date Null">
		<Description>Дата окончания</Description>
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="8b8fbb12-81f2-4d53-a79b-3f735cf3c79c" Name="Amount" Type="Decimal(20, 2) Null">
		<Description>Сумма (руб.)</Description>
	</SchemePhysicalColumn>
	<SchemeComplexColumn ID="a9f55b88-2716-4e97-81fe-854ba00b05ac" Name="SettlementCurrency" Type="Reference(Typified) Null" ReferencedTable="3612e150-032f-4a68-bf8e-8e094e5a3a73">
		<Description>Валюта расчета</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="a9f55b88-2716-0097-4000-054ba00b05ac" Name="SettlementCurrencyID" Type="Guid Null" ReferencedColumn="3612e150-032f-0168-4000-0e094e5a3a73" />
		<SchemeReferencingColumn ID="cae499d0-26a4-4d48-a108-62ab096faa19" Name="SettlementCurrencyName" Type="String(128) Null" ReferencedColumn="60b11ca9-a5b7-48f7-a5c6-6233d166b19a" />
		<SchemeReferencingColumn ID="bd0cf78b-89fe-4973-bbdc-7c43c44c7864" Name="SettlementCurrencyCode" Type="String(64) Null" ReferencedColumn="d307679e-6f6b-4429-83ba-16d0d8b8ecc2" />
	</SchemeComplexColumn>
	<SchemeComplexColumn ID="0e762510-ec22-44ff-935a-02c1cd96d8c7" Name="VATRate" Type="Reference(Typified) Null" ReferencedTable="016fa9e6-6a00-4568-a35c-751b2910363f">
		<Description>Ставка НДС</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="0e762510-ec22-00ff-4000-02c1cd96d8c7" Name="VATRateID" Type="Int16 Null" ReferencedColumn="6929a82d-3f57-4145-a59c-fe6d2499891c" />
		<SchemeReferencingColumn ID="f6bdb916-0b3d-44c4-8698-645d9c9cf904" Name="VATRateValue" Type="String(Max) Null" ReferencedColumn="832900f8-35cc-4a23-bf8d-fdd084624a48" />
	</SchemeComplexColumn>
	<SchemeComplexColumn ID="d5a5b718-d2e7-4c00-8ff9-d8b114128101" Name="Responsible" Type="Reference(Typified) Null" ReferencedTable="6c977939-bbfc-456f-a133-f1c2244e3cc3">
		<Description>Ответственный</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="d5a5b718-d2e7-0000-4000-08b114128101" Name="ResponsibleID" Type="Guid Null" ReferencedColumn="6c977939-bbfc-016f-4000-01c2244e3cc3" />
		<SchemeReferencingColumn ID="3f49d379-70b8-4da2-9856-52f55c01cf41" Name="ResponsibleName" Type="String(128) Null" ReferencedColumn="1782f76a-4743-4aa4-920c-7edaee860964" />
	</SchemeComplexColumn>
	<SchemeComplexColumn ID="7f811df3-f37d-45a8-9aeb-86103992f137" Name="City" Type="Reference(Typified) Null" ReferencedTable="75aa0d06-9bff-4e01-96b6-2ca62af269ae">
		<Description>Город</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="7f811df3-f37d-00a8-4000-06103992f137" Name="CityID" Type="Guid Null" ReferencedColumn="75aa0d06-9bff-0101-4000-0ca62af269ae" />
		<SchemeReferencingColumn ID="bb47b4fc-b1ae-4e8d-be45-3a03c850080b" Name="CityName" Type="String(Max) Null" ReferencedColumn="e70abb74-73d9-4232-ac52-a8ff12c357f5" />
	</SchemeComplexColumn>
	<SchemeComplexColumn ID="554023d0-0b7d-4691-bcea-31b514a21bdc" Name="CFO" Type="Reference(Typified) Null" ReferencedTable="b5e873a7-4f25-4731-b7bf-93586f07b53a">
		<Description>ЦФО</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="554023d0-0b7d-0091-4000-01b514a21bdc" Name="CFOID" Type="Guid Null" ReferencedColumn="b5e873a7-4f25-0131-4000-03586f07b53a" />
		<SchemeReferencingColumn ID="71f34285-7dd5-4d02-b417-9558c664f71f" Name="CFOName" Type="String(Max) Null" ReferencedColumn="20d4f2eb-ce34-4c44-87b8-8b386c283930" />
	</SchemeComplexColumn>
	<SchemeComplexColumn ID="77c8c097-2770-447c-ba03-a43034797dac" Name="Form" Type="Reference(Typified) Null" ReferencedTable="9c841787-0ad6-4239-a80d-37b931ad8b76">
		<Description>Форма договора</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="77c8c097-2770-007c-4000-043034797dac" Name="FormID" Type="Guid Null" ReferencedColumn="9c841787-0ad6-0139-4000-07b931ad8b76" />
		<SchemeReferencingColumn ID="2c5b348a-d1c3-4903-9b44-758135a39022" Name="FormName" Type="String(Max) Null" ReferencedColumn="4546eb18-4631-4482-b663-74ad25c48ab8" />
	</SchemeComplexColumn>
	<SchemeComplexColumn ID="f9ffe99e-70cf-4956-a07b-981e526124ba" Name="Development" Type="Reference(Typified) Null" ReferencedTable="5252bdd2-2ad8-4525-aa48-b956e522ae9d">
		<Description>Разработка договора</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="f9ffe99e-70cf-0056-4000-081e526124ba" Name="DevelopmentID" Type="Int16 Null" ReferencedColumn="7b0bf1da-7eb9-4f66-8d62-4962e8dc4888" />
		<SchemeReferencingColumn ID="f724a334-ae25-487b-a8ca-8b3b07fbf1a5" Name="DevelopmentName" Type="String(Max) Null" ReferencedColumn="bdf2b76d-7422-4258-b29a-462e67376af0" />
	</SchemeComplexColumn>
	<SchemeComplexColumn ID="57308b35-cf8c-4720-9b8c-bee7fdc60c13" Name="Type" Type="Reference(Typified) Null" ReferencedTable="e41cd076-fa8f-4ee2-857c-2f1ecf257eb7">
		<Description>Тип договора (УК ПС)</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="57308b35-cf8c-0020-4000-0ee7fdc60c13" Name="TypeID" Type="Guid Null" ReferencedColumn="e41cd076-fa8f-01e2-4000-0f1ecf257eb7" />
		<SchemeReferencingColumn ID="7119209a-f42d-42fc-9255-785a9874e298" Name="TypeName" Type="String(Max) Null" ReferencedColumn="6fb304a3-3a0c-4184-a9e1-09342acdee0f" />
	</SchemeComplexColumn>
	<SchemeComplexColumn ID="12befd18-316d-4061-bcab-fcd636650d3b" Name="Kind1C" Type="Reference(Typified) Null" ReferencedTable="c341867a-e7c4-4176-9306-3a0abbb56e3c">
		<Description>Вид договора (1С)</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="12befd18-316d-0061-4000-0cd636650d3b" Name="Kind1CID" Type="Guid Null" ReferencedColumn="c341867a-e7c4-0176-4000-0a0abbb56e3c" />
		<SchemeReferencingColumn ID="1b97ecc6-f464-409f-af51-bcc7f6e7cb6b" Name="Kind1CName" Type="String(Max) Null" ReferencedColumn="b9fedf5a-b596-4aa4-b188-19f7c545a982" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn ID="6486934c-7b96-4803-a5af-3d87501f344d" Name="PlannedActDate" Type="Date Null">
		<Description>Планируемая дата актирования</Description>
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="36b16261-4d58-47c4-8f57-0bd24aceb783" Name="ExtID" Type="Guid Null">
		<Description>Уникальный ID договора из системы заказчика</Description>
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="6b0ceb62-5871-496a-add9-5d612c2da6ae" Name="MDMKey" Type="String(Max) Null">
		<Description>MDM-Key</Description>
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="77300d9c-2908-44c1-80da-954e1fafce3b" Name="ParentMDM" Type="String(Max) Null">
		<Description>MDM родителя</Description>
	</SchemePhysicalColumn>
	<SchemePrimaryKey IsSystem="true" IsPermanent="true" IsSealed="true" ID="f061690d-d75c-0006-5000-0dc308ed6984" Name="pk_PnrContractsUK" IsClustered="true">
		<SchemeIndexedColumn Column="f061690d-d75c-0106-4000-0dc308ed6984" />
	</SchemePrimaryKey>
</SchemeTable>