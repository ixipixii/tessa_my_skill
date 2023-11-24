<?xml version="1.0" encoding="utf-8"?>
<SchemeTable Partition="54ed79f5-11c0-4f74-b8b0-793d596699d6" ID="c660e403-c8b3-4600-a112-9aac137aeeb9" Name="PnrSupplementaryAgreementsUK" Group="Pnr" InstanceType="Cards" ContentType="Entries">
	<Description>Дополнительные соглашения УК</Description>
	<SchemeComplexColumn IsSystem="true" IsPermanent="true" IsSealed="true" ID="c660e403-c8b3-0000-2000-0aac137aeeb9" Name="ID" Type="Reference(Typified) Not Null" ReferencedTable="1074eadd-21d7-4925-98c8-40d1e5f0ca0e">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="c660e403-c8b3-0100-4000-0aac137aeeb9" Name="ID" Type="Guid Not Null" ReferencedColumn="9a58123b-b2e9-4137-9c6c-5dab0ec02747" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn ID="31c20bf0-a77b-45a2-8f7a-1ddcc46bef02" Name="ExternalNumber" Type="String(Max) Null">
		<Description>Внешний номер</Description>
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="10253d26-79ba-4492-a28a-2da790d47906" Name="RegistrationDate" Type="Date Null">
		<Description>Дата регистрации</Description>
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="706ad11d-624a-44e3-9f6b-78c9a7bccf8b" Name="Subject" Type="String(Max) Null">
		<Description>Предмет договора</Description>
	</SchemePhysicalColumn>
	<SchemeComplexColumn ID="7b2680ba-f93e-45da-bd6e-58c72ceed390" Name="Organization" Type="Reference(Typified) Null" ReferencedTable="303dd35d-b998-49b3-a10b-ed30d31bd36a">
		<Description>Организация ГК Пионер</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="7b2680ba-f93e-00da-4000-08c72ceed390" Name="OrganizationID" Type="Guid Null" ReferencedColumn="303dd35d-b998-01b3-4000-0d30d31bd36a" />
		<SchemeReferencingColumn ID="9306d69c-37cc-4e47-b7b9-fd85d639cd88" Name="OrganizationName" Type="String(Max) Null" ReferencedColumn="d8ecabaa-f192-4546-9af6-1f9712daabdb" />
	</SchemeComplexColumn>
	<SchemeComplexColumn ID="b3311371-b890-48b8-bad6-4b94743b56fa" Name="Partner" Type="Reference(Typified) Null" ReferencedTable="5d47ef13-b6f4-47ef-9815-3b3d0e6d475a">
		<Description>Контрагент</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="b3311371-b890-00b8-4000-0b94743b56fa" Name="PartnerID" Type="Guid Null" ReferencedColumn="5d47ef13-b6f4-01ef-4000-0b3d0e6d475a" />
		<SchemeReferencingColumn ID="ac6695dc-56a7-406c-bb68-ed85adef6f4a" Name="PartnerName" Type="String(255) Null" ReferencedColumn="f1c960e0-951e-4837-8474-bb61d98f40f0" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn ID="9e93d345-9153-421d-bef8-8f57ad623fbd" Name="StartDate" Type="Date Null">
		<Description>Дата начала</Description>
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="bfdbb9fa-1f66-4cbb-b8dd-dd9eb2c0a146" Name="EndDate" Type="Date Null">
		<Description>Дата окончания</Description>
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="234e35b2-b503-48a0-b1f0-735ae4da3de3" Name="Amount" Type="Decimal(20, 2) Null">
		<Description>Сумма (руб.)</Description>
	</SchemePhysicalColumn>
	<SchemeComplexColumn ID="c3191be8-876b-42d6-9151-a4f752b5be81" Name="SettlementCurrency" Type="Reference(Typified) Null" ReferencedTable="3612e150-032f-4a68-bf8e-8e094e5a3a73">
		<Description>Валюта расчетов</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="c3191be8-876b-00d6-4000-04f752b5be81" Name="SettlementCurrencyID" Type="Guid Null" ReferencedColumn="3612e150-032f-0168-4000-0e094e5a3a73" />
		<SchemeReferencingColumn ID="d48d7ae1-3821-4662-84e0-c41b0ff422b9" Name="SettlementCurrencyName" Type="String(128) Null" ReferencedColumn="60b11ca9-a5b7-48f7-a5c6-6233d166b19a" />
		<SchemeReferencingColumn ID="83671cd7-42f6-4b95-bb64-2ac0feee862a" Name="SettlementCurrencyCode" Type="String(64) Null" ReferencedColumn="d307679e-6f6b-4429-83ba-16d0d8b8ecc2" />
	</SchemeComplexColumn>
	<SchemeComplexColumn ID="31c11cb6-873b-4bcc-a1d5-c3be3fd9b648" Name="VATRate" Type="Reference(Typified) Null" ReferencedTable="016fa9e6-6a00-4568-a35c-751b2910363f">
		<Description>Ставка НДС</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="31c11cb6-873b-00cc-4000-03be3fd9b648" Name="VATRateID" Type="Int16 Null" ReferencedColumn="6929a82d-3f57-4145-a59c-fe6d2499891c" />
		<SchemeReferencingColumn ID="9c541798-9c53-4499-b445-2689fd9f03ac" Name="VATRateValue" Type="String(Max) Null" ReferencedColumn="832900f8-35cc-4a23-bf8d-fdd084624a48" />
	</SchemeComplexColumn>
	<SchemeComplexColumn ID="28e54f98-bc04-42d0-a251-f8d5c90a8b13" Name="Responsible" Type="Reference(Typified) Null" ReferencedTable="6c977939-bbfc-456f-a133-f1c2244e3cc3">
		<Description>Ответственный</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="28e54f98-bc04-00d0-4000-08d5c90a8b13" Name="ResponsibleID" Type="Guid Null" ReferencedColumn="6c977939-bbfc-016f-4000-01c2244e3cc3" />
		<SchemeReferencingColumn ID="ba26d4e2-c768-439d-b0e3-38eb68756d63" Name="ResponsibleName" Type="String(128) Null" ReferencedColumn="1782f76a-4743-4aa4-920c-7edaee860964" />
	</SchemeComplexColumn>
	<SchemeComplexColumn ID="a1bb305d-9a46-47e3-9fb0-5400d41ac583" Name="City" Type="Reference(Typified) Null" ReferencedTable="75aa0d06-9bff-4e01-96b6-2ca62af269ae">
		<Description>Город</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="a1bb305d-9a46-00e3-4000-0400d41ac583" Name="CityID" Type="Guid Null" ReferencedColumn="75aa0d06-9bff-0101-4000-0ca62af269ae" />
		<SchemeReferencingColumn ID="9a6bb154-fe97-4f86-8dfa-8ae18c64fff3" Name="CityName" Type="String(Max) Null" ReferencedColumn="e70abb74-73d9-4232-ac52-a8ff12c357f5" />
	</SchemeComplexColumn>
	<SchemeComplexColumn ID="5d6c78d9-81da-4122-83b3-2fad885379f3" Name="CFO" Type="Reference(Typified) Null" ReferencedTable="b5e873a7-4f25-4731-b7bf-93586f07b53a">
		<Description>ЦФО</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="5d6c78d9-81da-0022-4000-0fad885379f3" Name="CFOID" Type="Guid Null" ReferencedColumn="b5e873a7-4f25-0131-4000-03586f07b53a" />
		<SchemeReferencingColumn ID="a6e76067-3c80-4116-9f39-278a2f6d6139" Name="CFOName" Type="String(Max) Null" ReferencedColumn="20d4f2eb-ce34-4c44-87b8-8b386c283930" />
	</SchemeComplexColumn>
	<SchemeComplexColumn ID="1ad8d4ff-eef1-45e6-96f6-373edd732ebf" Name="Form" Type="Reference(Typified) Null" ReferencedTable="9c841787-0ad6-4239-a80d-37b931ad8b76">
		<Description>Форма договора</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="1ad8d4ff-eef1-00e6-4000-073edd732ebf" Name="FormID" Type="Guid Null" ReferencedColumn="9c841787-0ad6-0139-4000-07b931ad8b76" />
		<SchemeReferencingColumn ID="8c1b112d-69d9-4aa2-adb7-212d6b288928" Name="FormName" Type="String(Max) Null" ReferencedColumn="4546eb18-4631-4482-b663-74ad25c48ab8" />
	</SchemeComplexColumn>
	<SchemeComplexColumn ID="c4b6d95f-5aeb-4a39-8aa2-c340f42bcb61" Name="Development" Type="Reference(Typified) Null" ReferencedTable="5252bdd2-2ad8-4525-aa48-b956e522ae9d">
		<Description>Разработка договора</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="c4b6d95f-5aeb-0039-4000-0340f42bcb61" Name="DevelopmentID" Type="Int16 Null" ReferencedColumn="7b0bf1da-7eb9-4f66-8d62-4962e8dc4888" />
		<SchemeReferencingColumn ID="815b171b-beeb-49d4-8b49-09a9a7425299" Name="DevelopmentName" Type="String(Max) Null" ReferencedColumn="bdf2b76d-7422-4258-b29a-462e67376af0" />
	</SchemeComplexColumn>
	<SchemeComplexColumn ID="b445bf17-2809-4546-b4e5-fd85ed2c5449" Name="Type" Type="Reference(Typified) Null" ReferencedTable="e41cd076-fa8f-4ee2-857c-2f1ecf257eb7">
		<Description>Тип договора (УК ПС)</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="b445bf17-2809-0046-4000-0d85ed2c5449" Name="TypeID" Type="Guid Null" ReferencedColumn="e41cd076-fa8f-01e2-4000-0f1ecf257eb7" />
		<SchemeReferencingColumn ID="244ca761-2c20-459b-9e38-6ba4eaa95a6f" Name="TypeName" Type="String(Max) Null" ReferencedColumn="6fb304a3-3a0c-4184-a9e1-09342acdee0f" />
	</SchemeComplexColumn>
	<SchemeComplexColumn ID="05447afa-a641-4f5d-9d70-9475b11be4e2" Name="Kind1C" Type="Reference(Typified) Null" ReferencedTable="c341867a-e7c4-4176-9306-3a0abbb56e3c">
		<Description>Вид договора (1С)</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="05447afa-a641-005d-4000-0475b11be4e2" Name="Kind1CID" Type="Guid Null" ReferencedColumn="c341867a-e7c4-0176-4000-0a0abbb56e3c" />
		<SchemeReferencingColumn ID="19180ce0-6343-411e-a189-5680b5944576" Name="Kind1CName" Type="String(Max) Null" ReferencedColumn="b9fedf5a-b596-4aa4-b188-19f7c545a982" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn ID="e8f4718e-11c5-47dd-80c5-e8092868262d" Name="PlannedActDate" Type="Date Null">
		<Description>Планируемая дата актирования</Description>
	</SchemePhysicalColumn>
	<SchemeComplexColumn ID="f258fec9-8ef8-4e65-8e75-54bb73ae2a12" Name="Reason" Type="Reference(Typified) Null" ReferencedTable="20cc279f-9ad6-4d8d-b1d3-9cb4150d1222">
		<Description>Причина заключения ДС</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="f258fec9-8ef8-0065-4000-04bb73ae2a12" Name="ReasonID" Type="Guid Null" ReferencedColumn="20cc279f-9ad6-018d-4000-0cb4150d1222" />
		<SchemeReferencingColumn ID="cfeeb3e5-4f97-4a8f-9a6b-761b86bd471d" Name="ReasonName" Type="String(Max) Null" ReferencedColumn="03ac4acf-77bd-4b69-8c86-a31e8e476af4" />
	</SchemeComplexColumn>
	<SchemePrimaryKey IsSystem="true" IsPermanent="true" IsSealed="true" ID="c660e403-c8b3-0000-5000-0aac137aeeb9" Name="pk_PnrSupplementaryAgreementsUK" IsClustered="true">
		<SchemeIndexedColumn Column="c660e403-c8b3-0100-4000-0aac137aeeb9" />
	</SchemePrimaryKey>
</SchemeTable>