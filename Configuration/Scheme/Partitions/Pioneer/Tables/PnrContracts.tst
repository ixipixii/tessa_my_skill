<?xml version="1.0" encoding="utf-8"?>
<SchemeTable Partition="54ed79f5-11c0-4f74-b8b0-793d596699d6" ID="018d6a5b-9d0e-4a97-a762-86b91f82fbfb" Name="PnrContracts" Group="Pnr" InstanceType="Cards" ContentType="Entries">
	<Description>Договора</Description>
	<SchemeComplexColumn IsSystem="true" IsPermanent="true" IsSealed="true" ID="018d6a5b-9d0e-0097-2000-06b91f82fbfb" Name="ID" Type="Reference(Typified) Not Null" ReferencedTable="1074eadd-21d7-4925-98c8-40d1e5f0ca0e">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="018d6a5b-9d0e-0197-4000-06b91f82fbfb" Name="ID" Type="Guid Not Null" ReferencedColumn="9a58123b-b2e9-4137-9c6c-5dab0ec02747" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn ID="b3309225-22bb-4187-ade8-f3a92802ad58" Name="ExternalNumber" Type="String(Max) Null">
		<Description>Внешний номер</Description>
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="2735845f-a025-4442-b4da-0c0e844200f8" Name="ProjectDate" Type="Date Null">
		<Description>Дата заключения</Description>
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="9185f5fa-2a0f-49e5-b37f-929e533e31e2" Name="Subject" Type="String(Max) Null">
		<Description>Предмет договора</Description>
	</SchemePhysicalColumn>
	<SchemeComplexColumn ID="5e3ca099-f7ee-4870-85cd-5de0080b8f01" Name="Partner" Type="Reference(Typified) Null" ReferencedTable="5d47ef13-b6f4-47ef-9815-3b3d0e6d475a">
		<Description>Контрагент</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="5e3ca099-f7ee-0070-4000-0de0080b8f01" Name="PartnerID" Type="Guid Null" ReferencedColumn="5d47ef13-b6f4-01ef-4000-0b3d0e6d475a" />
		<SchemeReferencingColumn ID="86c38621-7325-423f-a974-79d6ef17b320" Name="PartnerName" Type="String(255) Null" ReferencedColumn="f1c960e0-951e-4837-8474-bb61d98f40f0" />
	</SchemeComplexColumn>
	<SchemeComplexColumn ID="b8bb0377-646f-4d90-a8fa-548f32ffe5b2" Name="Organization" Type="Reference(Typified) Null" ReferencedTable="303dd35d-b998-49b3-a10b-ed30d31bd36a">
		<Description>Организация ГК Пионер</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="b8bb0377-646f-0090-4000-048f32ffe5b2" Name="OrganizationID" Type="Guid Null" ReferencedColumn="303dd35d-b998-01b3-4000-0d30d31bd36a" />
		<SchemeReferencingColumn ID="7c03bf6f-f45b-4aa0-a3ff-afb734081e14" Name="OrganizationName" Type="String(Max) Null" ReferencedColumn="d8ecabaa-f192-4546-9af6-1f9712daabdb" />
	</SchemeComplexColumn>
	<SchemeComplexColumn ID="50483eaa-b340-4ef7-a838-d5fa3b7e2f82" Name="Project" Type="Reference(Typified) Null" ReferencedTable="44de4db0-e013-441c-8441-4cb3ef09a649">
		<Description>Проект</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="50483eaa-b340-00f7-4000-05fa3b7e2f82" Name="ProjectID" Type="Guid Null" ReferencedColumn="44de4db0-e013-011c-4000-0cb3ef09a649" />
		<SchemeReferencingColumn ID="af6c3658-7289-464e-9c17-37897aa46b9b" Name="ProjectName" Type="String(Max) Null" ReferencedColumn="6c2fd8b3-e69b-4696-8a8a-2ab2d593af07" />
		<SchemeReferencingColumn ID="95640736-ab64-4578-9dff-2e0034bc0ce6" Name="ProjectInArchive" Type="Boolean Null" ReferencedColumn="aa8c2f04-090c-4dc0-83ee-782e3f47f3fa" />
	</SchemeComplexColumn>
	<SchemeComplexColumn ID="6f82f0c3-f609-4a8b-86d8-f53b70dbc4ef" Name="CostItem" Type="Reference(Typified) Null" ReferencedTable="b2c21f32-7f00-4d58-9a2d-f45cbb5b2308">
		<Description>Статья затрат</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="6f82f0c3-f609-008b-4000-053b70dbc4ef" Name="CostItemID" Type="Guid Null" ReferencedColumn="b2c21f32-7f00-0158-4000-045cbb5b2308" />
		<SchemeReferencingColumn ID="73226395-2ce1-42ff-bdd5-658af89bbac8" Name="CostItemName" Type="String(Max) Null" ReferencedColumn="03d21b73-553a-4e8d-947c-c1dbf2ace92c" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn ID="d81f14ce-56a8-4601-84c6-fd85ab75351a" Name="Amount" Type="Decimal(20, 2) Null">
		<Description>Сумма договора</Description>
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="26596e19-8e78-44d2-af50-268f9640057f" Name="PrepaidExpenseAmount" Type="Decimal(20, 2) Null">
		<Description>Сумма аванса</Description>
	</SchemePhysicalColumn>
	<SchemeComplexColumn ID="b995b510-6946-4d0e-885a-9f8c62cc32cd" Name="VATRate" Type="Reference(Typified) Null" ReferencedTable="016fa9e6-6a00-4568-a35c-751b2910363f">
		<Description>Ставка НДС</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="b995b510-6946-000e-4000-0f8c62cc32cd" Name="VATRateID" Type="Int16 Null" ReferencedColumn="6929a82d-3f57-4145-a59c-fe6d2499891c" />
		<SchemeReferencingColumn ID="feb364dc-1c65-46b5-8231-a1fcebf7eeae" Name="VATRateValue" Type="String(Max) Null" ReferencedColumn="832900f8-35cc-4a23-bf8d-fdd084624a48" />
	</SchemeComplexColumn>
	<SchemeComplexColumn ID="59a06749-cd76-48f1-8308-67acb07e05f5" Name="Form" Type="Reference(Typified) Null" ReferencedTable="9c841787-0ad6-4239-a80d-37b931ad8b76">
		<Description>Форма договора</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="59a06749-cd76-00f1-4000-07acb07e05f5" Name="FormID" Type="Guid Null" ReferencedColumn="9c841787-0ad6-0139-4000-07b931ad8b76" />
		<SchemeReferencingColumn ID="a17b18e7-e201-4064-a976-b6d0a002338d" Name="FormName" Type="String(Max) Null" ReferencedColumn="4546eb18-4631-4482-b663-74ad25c48ab8" />
	</SchemeComplexColumn>
	<SchemeComplexColumn ID="6e6f689d-cf6e-4627-90d4-4b0ab7fbd7b8" Name="Type" Type="Reference(Typified) Null" ReferencedTable="e41cd076-fa8f-4ee2-857c-2f1ecf257eb7">
		<Description>Тип договора</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="6e6f689d-cf6e-0027-4000-0b0ab7fbd7b8" Name="TypeID" Type="Guid Null" ReferencedColumn="e41cd076-fa8f-01e2-4000-0f1ecf257eb7" />
		<SchemeReferencingColumn ID="878c4152-0940-4af8-bbc5-17087145c619" Name="TypeName" Type="String(Max) Null" ReferencedColumn="6fb304a3-3a0c-4184-a9e1-09342acdee0f" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn ID="a9ea9109-d355-416b-9f7a-74cb06787ca1" Name="StartDate" Type="Date Null">
		<Description>Дата начала</Description>
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="4848c136-a166-4941-a5fa-d55e61db9aa5" Name="EndDate" Type="Date Null">
		<Description>Дата окончания</Description>
	</SchemePhysicalColumn>
	<SchemeComplexColumn ID="fc5897d4-ca6a-4c90-9b15-95b33f6fce39" Name="SettlementCurrency" Type="Reference(Typified) Null" ReferencedTable="3612e150-032f-4a68-bf8e-8e094e5a3a73">
		<Description>Валюта расчета</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="fc5897d4-ca6a-0090-4000-05b33f6fce39" Name="SettlementCurrencyID" Type="Guid Null" ReferencedColumn="3612e150-032f-0168-4000-0e094e5a3a73" />
		<SchemeReferencingColumn ID="5d90623b-d2fb-4f84-9a99-19e2972b3c43" Name="SettlementCurrencyName" Type="String(128) Null" ReferencedColumn="60b11ca9-a5b7-48f7-a5c6-6233d166b19a" />
		<SchemeReferencingColumn ID="15cdf7fb-3ce9-45ea-8a28-94de738e2ab4" Name="SettlementCurrencyCode" Type="String(64) Null" ReferencedColumn="d307679e-6f6b-4429-83ba-16d0d8b8ecc2" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn ID="fd504df1-ca58-40ea-83eb-c996c0d2f6fa" Name="Comment" Type="String(Max) Null">
		<Description>Комментарии</Description>
	</SchemePhysicalColumn>
	<SchemeComplexColumn ID="35a6faf0-4130-443f-981b-a163a5e333cc" Name="DownPayment" Type="Reference(Typified) Null" ReferencedTable="9f7e3f9d-38f3-41e1-ba99-668d2aef445a">
		<Description>% авансового платежа</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="35a6faf0-4130-003f-4000-0163a5e333cc" Name="DownPaymentID" Type="Int16 Null" ReferencedColumn="c474984c-4376-4025-9260-59625b58c702" />
		<SchemeReferencingColumn ID="731e529e-b1ed-4fd9-95aa-7a30e3ec7681" Name="DownPaymentValue" Type="String(Max) Null" ReferencedColumn="1ca24188-9964-4896-bde8-c1d014b470ff" />
	</SchemeComplexColumn>
	<SchemeComplexColumn ID="612ab7d7-eb81-493f-b409-e9984e91b692" Name="DefermentPayment" Type="Reference(Typified) Null" ReferencedTable="b75460ff-5276-4f6e-a04e-e52c02bcb6e0">
		<Description>Отсрочка платежа</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="612ab7d7-eb81-003f-4000-09984e91b692" Name="DefermentPaymentID" Type="Int16 Null" ReferencedColumn="2c48c263-dfe2-464c-8249-8acf89730d90" />
		<SchemeReferencingColumn ID="89444296-b11c-4ff0-89a0-40224b82a7fd" Name="DefermentPaymentValue" Type="String(Max) Null" ReferencedColumn="dee708ee-1c93-4c36-b02e-3c9fa6da2b26" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn ID="30b99df7-3dd9-4cfb-8966-166e78f9042d" Name="GroundConcluding" Type="String(Max) Null">
		<Description>Основание для заключения договора и причины выбора КА</Description>
	</SchemePhysicalColumn>
	<SchemeComplexColumn ID="fd4d4ab9-d0ee-4006-9c38-84165386fa07" Name="IsWarranty" Type="Reference(Typified) Null" ReferencedTable="c77707ce-b8de-4d47-8d55-08c315491728">
		<Description>Гарантийные обязательства</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="fd4d4ab9-d0ee-0006-4000-04165386fa07" Name="IsWarrantyID" Type="Int16 Null" ReferencedColumn="2cd1796f-0b93-4e5b-b0e4-b0bc91d20370" />
		<SchemeReferencingColumn ID="dec9fce2-338f-4762-aff2-8e51d2c7e32c" Name="IsWarrantyName" Type="String(Max) Null" ReferencedColumn="d3a53179-d767-4989-a760-2e085122b5a7" />
	</SchemeComplexColumn>
	<SchemeComplexColumn ID="92368cee-67f5-4b08-888c-d3612157573e" Name="KindDUP" Type="Reference(Typified) Null" ReferencedTable="3537eeca-1c13-4cc8-ad6d-872b6f1b52ca">
		<Description>Вид договора (ДУП)</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="92368cee-67f5-0008-4000-03612157573e" Name="KindDUPID" Type="Guid Null" ReferencedColumn="3537eeca-1c13-01c8-4000-072b6f1b52ca" />
		<SchemeReferencingColumn ID="9d7b986a-4644-4880-bc2f-50a93b79ef3f" Name="KindDUPName" Type="String(Max) Null" ReferencedColumn="c81e197b-ef24-4565-9597-516257c7eb0a" />
	</SchemeComplexColumn>
	<SchemeComplexColumn ID="36d2ac1a-41f0-49f5-977e-2d44f580e6e2" Name="GuaranteePeriod" Type="Reference(Typified) Null" ReferencedTable="2df4371b-30ff-43b8-9049-5895ef3fb1bf">
		<Description>Гарантийный срок на работы (год)</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="36d2ac1a-41f0-00f5-4000-0d44f580e6e2" Name="GuaranteePeriodID" Type="Int16 Null" ReferencedColumn="5978f4a9-8621-47a1-9988-ac3256271b01" />
		<SchemeReferencingColumn ID="797c24d2-bccf-4d88-9d6c-1eed9fb91567" Name="GuaranteePeriodValue" Type="String(Max) Null" ReferencedColumn="f82e3c75-2582-45ce-9e6a-eec978ebbf58" />
	</SchemeComplexColumn>
	<SchemeComplexColumn ID="e85684b3-05cf-4e67-971c-588ff09d79c8" Name="GuaranteeDeductions" Type="Reference(Typified) Null" ReferencedTable="a4816fd0-bf53-4a61-b56c-afc7563b0f9a">
		<Description>% гарантийных удержаний</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="e85684b3-05cf-0067-4000-088ff09d79c8" Name="GuaranteeDeductionsID" Type="Int16 Null" ReferencedColumn="df2b660f-dbaa-49e8-8525-09f71f01b287" />
		<SchemeReferencingColumn ID="06871fd7-a932-4076-bb1b-db26f04c578c" Name="GuaranteeDeductionsValue" Type="String(Max) Null" ReferencedColumn="4768db7c-d6e0-4612-8079-6bb93dd78563" />
	</SchemeComplexColumn>
	<SchemeComplexColumn ID="2cf69187-fa02-487c-acab-8afc368642ed" Name="PhasedImplementation" Type="Reference(Typified) Null" ReferencedTable="c77707ce-b8de-4d47-8d55-08c315491728">
		<Description>Предусмотрено поэтапное выполнение</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="2cf69187-fa02-007c-4000-0afc368642ed" Name="PhasedImplementationID" Type="Int16 Null" ReferencedColumn="2cd1796f-0b93-4e5b-b0e4-b0bc91d20370" />
		<SchemeReferencingColumn ID="b7c3b0eb-6339-4a17-892c-0205a61db616" Name="PhasedImplementationName" Type="String(Max) Null" ReferencedColumn="d3a53179-d767-4989-a760-2e085122b5a7" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn ID="c5673bfe-0cb4-4d90-88b2-30ca58298953" Name="PlannedActDate" Type="Date Null">
		<Description>Планируемая дата актирования</Description>
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="4f1c7a5d-b94f-44f4-bc9e-19b3e70eb0bd" Name="LinkCardCRM" Type="String(Max) Null">
		<Description>Ссылка на карточку договора в CRM</Description>
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="d771c947-5bf6-4a8e-a7e4-7c5e169b6189" Name="MDMKey" Type="String(Max) Null">
		<Description>MDM-Key</Description>
	</SchemePhysicalColumn>
	<SchemeComplexColumn ID="3a2220b8-48bb-4508-b29b-54ac310b159d" Name="CFO" Type="Reference(Typified) Null" ReferencedTable="b5e873a7-4f25-4731-b7bf-93586f07b53a">
		<Description>ЦФО</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="3a2220b8-48bb-0008-4000-04ac310b159d" Name="CFOID" Type="Guid Null" ReferencedColumn="b5e873a7-4f25-0131-4000-03586f07b53a" />
		<SchemeReferencingColumn ID="2d18bfa2-1b53-4e13-8e86-2ea7a32eaccc" Name="CFOName" Type="String(Max) Null" ReferencedColumn="20d4f2eb-ce34-4c44-87b8-8b386c283930" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn ID="1c5132df-118a-4ec9-b00c-707941165269" Name="IsInBudget" Type="Boolean Null">
		<Description>В бюджете</Description>
		<SchemeDefaultConstraint IsPermanent="true" ID="b6061e49-3648-45e4-b1d0-28cf108a1d2d" Name="df_PnrContracts_IsInBudget" Value="false" />
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="61ff41bf-724f-4ae3-9785-2df3b863b5b4" Name="IsTenderHeld" Type="Boolean Null">
		<Description>Проведен тендер</Description>
	</SchemePhysicalColumn>
	<SchemeComplexColumn ID="1db25ac0-9123-4aca-9129-073a93fca5d8" Name="Kind" Type="Reference(Typified) Null" ReferencedTable="4e9d936c-89c0-470d-be69-b4ce3e0aa294">
		<Description>Вид договора</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="1db25ac0-9123-00ca-4000-073a93fca5d8" Name="KindID" Type="Guid Null" ReferencedColumn="4e9d936c-89c0-010d-4000-04ce3e0aa294" />
		<SchemeReferencingColumn ID="9dc2179b-ff09-4d0b-ad74-418a6ff8bb81" Name="KindName" Type="String(128) Null" ReferencedColumn="86c831ea-880e-4fef-ab09-59bc9c36c392" />
	</SchemeComplexColumn>
	<SchemeComplexColumn ID="d3f706d6-6f3c-4aa9-b556-771667311d6f" Name="Kind1C" Type="Reference(Typified) Null" ReferencedTable="c341867a-e7c4-4176-9306-3a0abbb56e3c">
		<Description>Вид договора (1С)</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="d3f706d6-6f3c-00a9-4000-071667311d6f" Name="Kind1CID" Type="Guid Null" ReferencedColumn="c341867a-e7c4-0176-4000-0a0abbb56e3c" />
		<SchemeReferencingColumn ID="137e5fb4-427f-4f33-93aa-97fe318370f8" Name="Kind1CName" Type="String(Max) Null" ReferencedColumn="b9fedf5a-b596-4aa4-b188-19f7c545a982" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn ID="c8e66fc0-15af-403e-8b41-17d660a3b5f2" Name="DefermentPaymentDUP" Type="String(Max) Null">
		<Description>Отсрочка платежа (Договоры ДУП)</Description>
	</SchemePhysicalColumn>
	<SchemeComplexColumn ID="14a502da-530c-4670-986a-1c04756e23f1" Name="Development" Type="Reference(Typified) Null" ReferencedTable="5252bdd2-2ad8-4525-aa48-b956e522ae9d">
		<Description>Разработка договора</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="14a502da-530c-0070-4000-0c04756e23f1" Name="DevelopmentID" Type="Int16 Null" ReferencedColumn="7b0bf1da-7eb9-4f66-8d62-4962e8dc4888" />
		<SchemeReferencingColumn ID="4e32d9ec-4b67-4e0c-91c8-34cb2d0e8a68" Name="DevelopmentName" Type="String(Max) Null" ReferencedColumn="bdf2b76d-7422-4258-b29a-462e67376af0" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn ID="44dd3425-9151-4218-8e75-e33243ff5935" Name="HyperlinkCard" Type="String(Max) Null">
		<Description>Гиперссылка на карточку</Description>
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="e4e84d0d-2aa9-43ce-8eca-5327bafc8e5e" Name="ParentMDM" Type="String(Max) Null">
		<Description>MDM родителя</Description>
	</SchemePhysicalColumn>
	<SchemeComplexColumn ID="2921ce5b-06a5-4eb2-9dd8-f18a388a1854" Name="ConditionStartingWork" Type="Reference(Typified) Null" ReferencedTable="e50c0238-8162-4db0-bf46-b54ef3f36bc7">
		<Description>Условие начала выполнения работ</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="2921ce5b-06a5-00b2-4000-018a388a1854" Name="ConditionStartingWorkID" Type="Guid Null" ReferencedColumn="e50c0238-8162-01b0-4000-054ef3f36bc7" />
		<SchemeReferencingColumn ID="230d8c9f-7d1f-47b5-bb3d-da1877d2638b" Name="ConditionStartingWorkName" Type="String(Max) Null" ReferencedColumn="a995dc5f-63af-4320-9a09-f52960df09af" />
	</SchemeComplexColumn>
	<SchemeComplexColumn ID="02df57b1-a29e-4cd2-8677-2155f863da67" Name="LegalEntityIndex" Type="Reference(Typified) Null" ReferencedTable="00e3d618-aa1e-4bac-9297-3010b710425c">
		<Description>Индекс ЮЛ</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="02df57b1-a29e-00d2-4000-0155f863da67" Name="LegalEntityIndexID" Type="Guid Null" ReferencedColumn="00e3d618-aa1e-01ac-4000-0010b710425c" />
		<SchemeReferencingColumn ID="87d4a605-3689-47ea-90cb-9f4d8cd29aa8" Name="LegalEntityIndexIdx" Type="String(Max) Null" ReferencedColumn="14c84e81-9e95-4cfa-98cf-62c786ba4bfe" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn ID="4aea26cb-1b4f-4453-a4a1-b039bf602857" Name="ExtID" Type="Guid Null">
		<Description>Уникальный ID договора из системы заказчика</Description>
	</SchemePhysicalColumn>
	<SchemeComplexColumn ID="ef517cb7-758d-4143-b917-3359a57f8a82" Name="Signatory" Type="Reference(Typified) Null" ReferencedTable="6c977939-bbfc-456f-a133-f1c2244e3cc3">
		<Description>Подписант</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="ef517cb7-758d-0043-4000-0359a57f8a82" Name="SignatoryID" Type="Guid Null" ReferencedColumn="6c977939-bbfc-016f-4000-01c2244e3cc3" />
		<SchemeReferencingColumn ID="e4b1b6e6-8557-4d4e-95f4-7bf08a6fc6a5" Name="SignatoryName" Type="String(128) Null" ReferencedColumn="1782f76a-4743-4aa4-920c-7edaee860964" />
	</SchemeComplexColumn>
	<SchemeComplexColumn ID="0dfe9d30-888f-48ae-854d-241b58a775c9" Name="ImplementationStage" Type="Reference(Typified) Null" ReferencedTable="acb2c154-f00a-4dbd-adb0-8b77ca861ae2">
		<Description>Стадия реализации</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="0dfe9d30-888f-00ae-4000-041b58a775c9" Name="ImplementationStageID" Type="Guid Null" ReferencedColumn="acb2c154-f00a-01bd-4000-0b77ca861ae2" />
		<SchemeReferencingColumn ID="a8d549aa-0a75-415f-bdc2-72f18ecc72b6" Name="ImplementationStageName" Type="String(Max) Null" ReferencedColumn="32b4bb15-8adb-4851-b9c6-d990cb2e3396" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn ID="83ef285e-3790-4acf-b2cd-9c8df512c0e6" Name="MDMSentDate" Type="DateTime Null">
		<Description>Дата последней отправки в MDM (НСИ), если пусто, значит отправки не было.</Description>
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="942c04e7-0d9e-4d05-9cd8-3da5d1894b3c" Name="CRMContractStatus" Type="String(Max) Null">
		<Description>Статус договора в CRM</Description>
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="4f9ab5af-512e-4b6a-b95e-180cacac734f" Name="IsRequiresApproval" Type="Boolean Null">
		<Description>Требует согласования</Description>
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="87f1e083-167e-4a39-861f-208450acbca1" Name="ApartmentNumber" Type="String(Max) Null">
		<Description>Номер квартиры</Description>
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="056aecf4-3d5d-4303-86e3-eda0758e3b6f" Name="MDMContractNumber" Type="String(Max) Null">
		<Description>Номер договора для МДМ</Description>
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="66d5a936-4555-4036-b488-b00583f202d6" Name="ActionStatus" Type="String(Max) Null">
		<Description>Статус действия</Description>
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="3c0fd072-c7e6-488e-ae5a-6a094abbf303" Name="Urgency" Type="String(Max) Null">
		<Description>Urgency</Description>
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="ad24e1ae-36bc-40c7-b00a-a8eff8c0f415" Name="Flat" Type="String(100) Null">
		<Description>Код объекта недвижимости</Description>
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="3f4521fc-e55a-4973-ad81-4b7c5f311e56" Name="CRMContractApprove" Type="Boolean Null">
		<Description>Согласование бухгалтерией для интеграции с CRM</Description>
		<SchemeDefaultConstraint IsPermanent="true" ID="76c56c4f-ae66-45db-ac78-78e7c3bee2e1" Name="df_PnrContracts_CRMContractApprove" Value="false" />
	</SchemePhysicalColumn>
	<SchemeComplexColumn ID="b6c6a60d-bf9f-4d3c-a54d-c51fa116235e" Name="Department" Type="Reference(Typified) Null" ReferencedTable="81f6010b-9641-4aa5-8897-b8e8603fbf4b">
		<Description>Подразделение</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="b6c6a60d-bf9f-003c-4000-051fa116235e" Name="DepartmentID" Type="Guid Null" ReferencedColumn="81f6010b-9641-01a5-4000-08e8603fbf4b" />
		<SchemeReferencingColumn ID="4c03dff0-a712-4777-af9a-d574adae741d" Name="DepartmentIdx" Type="String(Max) Null" ReferencedColumn="1f7ca0d4-1b64-43f8-b973-d14791af51a0" />
		<SchemeReferencingColumn ID="68127557-f0ad-49bf-9311-21fe45097e8d" Name="DepartmentName" Type="String(128) Null" ReferencedColumn="616d6b2e-35d5-424d-846b-618eb25962d0" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn ID="fdb8d1a5-eda4-4bf2-b4d9-5462598aed72" Name="GuaranteePeriodMonth" Type="Int32 Null">
		<Description>Гарантийный срок на работы (мес)</Description>
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="9459d089-e875-4438-aac9-d0ed4ebb811b" Name="IsProjectInArchive" Type="Boolean Null">
		<Description>Проект карточки 'Завершенный'</Description>
		<SchemeDefaultConstraint IsPermanent="true" ID="177ecd4b-8ddb-416d-80b4-893bc4d84018" Name="df_PnrContracts_IsProjectInArchive" Value="false" />
	</SchemePhysicalColumn>
	<SchemeComplexColumn ID="12f451e4-b2fb-4f78-9e9a-868c22e98d10" Name="CRMApprove" Type="Reference(Typified) Null" ReferencedTable="0df3fb02-10e0-4049-8f88-ed070acaa11e">
		<Description>Согласование бухгалтерией для интеграции с CRM</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="12f451e4-b2fb-0078-4000-068c22e98d10" Name="CRMApproveID" Type="Int16 Null" ReferencedColumn="a74a1d03-37a9-4ca0-be09-6db0463cbbb9" />
		<SchemeReferencingColumn ID="5f89ca09-ab6f-4098-b34c-11d395bb284c" Name="CRMApproveName" Type="String(Max) Null" ReferencedColumn="39381382-1bd5-499a-b858-47de893eeae0" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn ID="e30911c7-84af-4503-a24f-21f87fb6bc85" Name="IsHeadDepartmentMTOInStage" Type="Boolean Null">
		<Description>Рук-ль отдела МТО на стадии Внутреннее согласование договора ДУП</Description>
		<SchemeDefaultConstraint IsPermanent="true" ID="412b16d8-22bf-4ee5-ac9d-b9560c8d0355" Name="df_PnrContracts_IsHeadDepartmentMTOInStage" Value="false" />
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="02a07b81-68aa-4316-8063-7714dd20f79b" Name="IsHeadDepartmentSVISInStage" Type="Boolean Null">
		<Description>Рук-ль СВИС на стадии Внутреннее согласование договора ДУП</Description>
		<SchemeDefaultConstraint IsPermanent="true" ID="68ab52f5-e6a1-4b16-8ac6-434227fcbf82" Name="df_PnrContracts_IsHeadDepartmentSVISInStage" Value="false" />
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="5ea444da-d14b-4d27-bb3d-a28e558b53db" Name="IsHeadDepartmentSPIAInStage" Type="Boolean Null">
		<Description>Рук-ль СПиА на стадии Внутреннее согласование договора ДУП</Description>
		<SchemeDefaultConstraint IsPermanent="true" ID="836fad10-8640-4587-9bcc-7fbc26868e0c" Name="df_PnrContracts_IsHeadDepartmentSPIAInStage" Value="false" />
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="5781eed2-b191-40f6-b7d6-fa920655e9af" Name="IsProjectGIPInStage" Type="Boolean Null">
		<Description>ГИП проекта на стадии Внутреннее согласование договора ДУП</Description>
		<SchemeDefaultConstraint IsPermanent="true" ID="75db9a49-84ab-4446-b69e-27f495b5e7b5" Name="df_PnrContracts_IsProjectGIPInStage" Value="false" />
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="8f48619a-4404-4ad8-a029-53dbd976dd29" Name="IsConstructionManagerInStage" Type="Boolean Null">
		<Description>Рук-ль строительства на стадии Внутреннее согласование договора ДУП</Description>
		<SchemeDefaultConstraint IsPermanent="true" ID="84c95b80-ddcf-4621-a6c1-a71dfaed7774" Name="df_PnrContracts_IsConstructionManagerInStage" Value="false" />
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="9f182c87-3eb9-4527-8098-6da40615b4ac" Name="IsInternalApprovalStage" Type="Boolean Null">
		<Description>Внутреннее согласование договора ДУП в маршруте</Description>
		<SchemeDefaultConstraint IsPermanent="true" ID="8491cd58-1d26-41a9-8920-0cd7414357f5" Name="df_PnrContracts_IsInternalApprovalStage" Value="false" />
	</SchemePhysicalColumn>
	<SchemePrimaryKey IsSystem="true" IsPermanent="true" IsSealed="true" ID="018d6a5b-9d0e-0097-5000-06b91f82fbfb" Name="pk_PnrContracts" IsClustered="true">
		<SchemeIndexedColumn Column="018d6a5b-9d0e-0197-4000-06b91f82fbfb" />
	</SchemePrimaryKey>
	<SchemeIndex ID="d3ae07d5-a535-4850-b2de-340cd765a76a" Name="ndx_PnrContracts_IDPartnerName">
		<SchemeIndexedColumn Column="018d6a5b-9d0e-0197-4000-06b91f82fbfb" />
		<SchemeIndexedColumn Column="86c38621-7325-423f-a974-79d6ef17b320" />
	</SchemeIndex>
</SchemeTable>