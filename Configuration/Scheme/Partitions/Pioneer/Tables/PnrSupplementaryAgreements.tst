<?xml version="1.0" encoding="utf-8"?>
<SchemeTable Partition="54ed79f5-11c0-4f74-b8b0-793d596699d6" ID="3bcd9132-1236-4348-b89e-0b0de8d5b0e4" Name="PnrSupplementaryAgreements" Group="Pnr" InstanceType="Cards" ContentType="Entries">
	<Description>Дополнительные соглашения</Description>
	<SchemeComplexColumn IsSystem="true" IsPermanent="true" IsSealed="true" ID="3bcd9132-1236-0048-2000-0b0de8d5b0e4" Name="ID" Type="Reference(Typified) Not Null" ReferencedTable="1074eadd-21d7-4925-98c8-40d1e5f0ca0e">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="3bcd9132-1236-0148-4000-0b0de8d5b0e4" Name="ID" Type="Guid Not Null" ReferencedColumn="9a58123b-b2e9-4137-9c6c-5dab0ec02747" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn ID="9f5cf438-0b0c-444f-8257-9fcf587bc21a" Name="ExternalNumber" Type="String(Max) Null">
		<Description>Внешний номер</Description>
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="f479ae7a-4852-452e-8e02-1298b7c37064" Name="ProjectDate" Type="Date Null">
		<Description>Дата заключения</Description>
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="14edfd42-a590-481f-8bce-5033a226da9a" Name="Subject" Type="String(Max) Null">
		<Description>Предмет договора</Description>
	</SchemePhysicalColumn>
	<SchemeComplexColumn ID="713aef59-ee2a-4d1f-a4da-4de18c7165f0" Name="Partner" Type="Reference(Typified) Null" ReferencedTable="5d47ef13-b6f4-47ef-9815-3b3d0e6d475a">
		<Description>Контрагент</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="713aef59-ee2a-001f-4000-0de18c7165f0" Name="PartnerID" Type="Guid Null" ReferencedColumn="5d47ef13-b6f4-01ef-4000-0b3d0e6d475a" />
		<SchemeReferencingColumn ID="08cfe531-d567-422d-b97b-c4306fcdd342" Name="PartnerName" Type="String(255) Null" ReferencedColumn="f1c960e0-951e-4837-8474-bb61d98f40f0" />
	</SchemeComplexColumn>
	<SchemeComplexColumn ID="199419ae-f10a-4b5e-88cb-d600b224dc6e" Name="Reason" Type="Reference(Typified) Null" ReferencedTable="20cc279f-9ad6-4d8d-b1d3-9cb4150d1222">
		<Description>Причина заключения ДС</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="199419ae-f10a-005e-4000-0600b224dc6e" Name="ReasonID" Type="Guid Null" ReferencedColumn="20cc279f-9ad6-018d-4000-0cb4150d1222" />
		<SchemeReferencingColumn ID="a899089c-243d-47b2-84b2-6ba60e8130bf" Name="ReasonName" Type="String(Max) Null" ReferencedColumn="03ac4acf-77bd-4b69-8c86-a31e8e476af4" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn ID="b34bbab0-bab1-4adf-96d5-12b253f87387" Name="IsAmountChanged" Type="Boolean Null">
		<Description>Изменение суммы договора</Description>
		<SchemeDefaultConstraint IsPermanent="true" ID="dc88de87-73b6-49fa-9748-43e8093be4b8" Name="df_PnrSupplementaryAgreements_IsAmountChanged" Value="false" />
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="1a71f6e1-a9a4-4a5e-9780-41138291fc69" Name="Amount" Type="Decimal(20, 2) Null">
		<Description>Сумма договора с учетом ДС</Description>
		<SchemeDefaultConstraint IsPermanent="true" ID="8c1f443b-a2e6-4001-84c9-35d47aaf3000" Name="df_PnrSupplementaryAgreements_Amount" Value="0" />
	</SchemePhysicalColumn>
	<SchemeComplexColumn ID="ca7ad1de-532f-4d08-97ac-88ccdf9b6ff5" Name="Organization" Type="Reference(Typified) Null" ReferencedTable="303dd35d-b998-49b3-a10b-ed30d31bd36a">
		<Description>Организация ГК Пионер</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="ca7ad1de-532f-0008-4000-08ccdf9b6ff5" Name="OrganizationID" Type="Guid Null" ReferencedColumn="303dd35d-b998-01b3-4000-0d30d31bd36a" />
		<SchemeReferencingColumn ID="9f789580-8036-4197-8fbb-d42cc8c432cb" Name="OrganizationName" Type="String(Max) Null" ReferencedColumn="d8ecabaa-f192-4546-9af6-1f9712daabdb" />
	</SchemeComplexColumn>
	<SchemeComplexColumn ID="0d7d9870-bf7b-4d7f-a290-e341767a1af7" Name="Project" Type="Reference(Typified) Null" ReferencedTable="44de4db0-e013-441c-8441-4cb3ef09a649">
		<Description>Проект</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="0d7d9870-bf7b-007f-4000-0341767a1af7" Name="ProjectID" Type="Guid Null" ReferencedColumn="44de4db0-e013-011c-4000-0cb3ef09a649" />
		<SchemeReferencingColumn ID="5fd94f50-5f3e-4239-b294-f95c6c186992" Name="ProjectName" Type="String(Max) Null" ReferencedColumn="6c2fd8b3-e69b-4696-8a8a-2ab2d593af07" />
		<SchemeReferencingColumn ID="6ce02fe0-a8b4-4167-ae92-9aad86f54b77" Name="ProjectInArchive" Type="Boolean Null" ReferencedColumn="aa8c2f04-090c-4dc0-83ee-782e3f47f3fa" />
	</SchemeComplexColumn>
	<SchemeComplexColumn ID="ea491033-f19c-465f-95ba-f5fe23c01ca0" Name="CostItem" Type="Reference(Typified) Null" ReferencedTable="b2c21f32-7f00-4d58-9a2d-f45cbb5b2308">
		<Description>Статья затрат</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="ea491033-f19c-005f-4000-05fe23c01ca0" Name="CostItemID" Type="Guid Null" ReferencedColumn="b2c21f32-7f00-0158-4000-045cbb5b2308" />
		<SchemeReferencingColumn ID="f8d4e668-0613-4ef7-aa67-69741f2662a2" Name="CostItemName" Type="String(Max) Null" ReferencedColumn="03d21b73-553a-4e8d-947c-c1dbf2ace92c" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn ID="bc339ab8-dcaa-424d-929b-995fe3228360" Name="AmountSA" Type="Decimal(20, 2) Null">
		<Description>Сумма ДС (руб.)</Description>
		<SchemeDefaultConstraint IsPermanent="true" ID="52c51a90-5622-4a69-8159-cbca87b84748" Name="df_PnrSupplementaryAgreements_AmountSA" Value="0" />
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="f0dc3956-d841-462d-923f-226b2ae75227" Name="PrepaidExpenseAmount" Type="Decimal(20, 2) Null">
		<Description>Сумма аванса (руб.)</Description>
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="5c9038bb-c502-4ce0-b551-a4cf7ddc3db1" Name="IsInBudget" Type="Boolean Null">
		<Description>В бюджете</Description>
		<SchemeDefaultConstraint IsPermanent="true" ID="14bcf3ce-6bb6-4b6b-a792-23a7ac465ebf" Name="df_PnrSupplementaryAgreements_IsInBudget" Value="false" />
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="7cce0783-f609-4a27-9935-12ac41a8272d" Name="IsTenderHeld" Type="Boolean Null">
		<Description>Проведен тендер</Description>
		<SchemeDefaultConstraint IsPermanent="true" ID="b5ed4edb-36da-47dc-9e86-ddbf66c8602f" Name="df_PnrSupplementaryAgreements_IsTenderHeld" Value="false" />
	</SchemePhysicalColumn>
	<SchemeComplexColumn ID="19f45564-4c06-49a6-8930-a297160f0e7b" Name="VATRate" Type="Reference(Typified) Null" ReferencedTable="016fa9e6-6a00-4568-a35c-751b2910363f">
		<Description>Ставка НДС</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="19f45564-4c06-00a6-4000-0297160f0e7b" Name="VATRateID" Type="Int16 Null" ReferencedColumn="6929a82d-3f57-4145-a59c-fe6d2499891c" />
		<SchemeReferencingColumn ID="a46f80a2-a43c-47bc-8a0d-7641e6a23ef1" Name="VATRateValue" Type="String(Max) Null" ReferencedColumn="832900f8-35cc-4a23-bf8d-fdd084624a48" />
	</SchemeComplexColumn>
	<SchemeComplexColumn ID="a5e99f0d-d20b-456f-b3af-4d82b2c6dc7d" Name="Form" Type="Reference(Typified) Null" ReferencedTable="9c841787-0ad6-4239-a80d-37b931ad8b76">
		<Description>Форма договора</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="a5e99f0d-d20b-006f-4000-0d82b2c6dc7d" Name="FormID" Type="Guid Null" ReferencedColumn="9c841787-0ad6-0139-4000-07b931ad8b76" />
		<SchemeReferencingColumn ID="67f57f99-296e-4fc0-8820-7daf9fe7e758" Name="FormName" Type="String(Max) Null" ReferencedColumn="4546eb18-4631-4482-b663-74ad25c48ab8" />
	</SchemeComplexColumn>
	<SchemeComplexColumn ID="01c3478b-e80d-4398-b3eb-b77f35174e2f" Name="Type" Type="Reference(Typified) Null" ReferencedTable="e41cd076-fa8f-4ee2-857c-2f1ecf257eb7">
		<Description>Тип договора</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="01c3478b-e80d-0098-4000-077f35174e2f" Name="TypeID" Type="Guid Null" ReferencedColumn="e41cd076-fa8f-01e2-4000-0f1ecf257eb7" />
		<SchemeReferencingColumn ID="d7a3420e-1ce6-40a3-b75e-814c3710c650" Name="TypeName" Type="String(Max) Null" ReferencedColumn="6fb304a3-3a0c-4184-a9e1-09342acdee0f" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn ID="da66b51f-8d06-4f42-9da1-85d3b5b6a450" Name="StartDate" Type="Date Null">
		<Description>Дата начала</Description>
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="5bf98971-5dba-4567-a2c6-33e15204f7bd" Name="EndDate" Type="Date Null">
		<Description>Дата окончания</Description>
	</SchemePhysicalColumn>
	<SchemeComplexColumn ID="5e978ed4-f511-4499-be09-e3cd83fb04bb" Name="SettlementCurrency" Type="Reference(Typified) Null" ReferencedTable="3612e150-032f-4a68-bf8e-8e094e5a3a73">
		<Description>Валюта расчета</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="5e978ed4-f511-0099-4000-03cd83fb04bb" Name="SettlementCurrencyID" Type="Guid Null" ReferencedColumn="3612e150-032f-0168-4000-0e094e5a3a73" />
		<SchemeReferencingColumn ID="499e1fa1-90c3-417a-8ef8-85709c766083" Name="SettlementCurrencyName" Type="String(128) Null" ReferencedColumn="60b11ca9-a5b7-48f7-a5c6-6233d166b19a" />
		<SchemeReferencingColumn ID="9d218edb-6d1a-41f7-91e9-25c5ca0830f5" Name="SettlementCurrencyCode" Type="String(64) Null" ReferencedColumn="d307679e-6f6b-4429-83ba-16d0d8b8ecc2" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn ID="71c138ed-1ea0-406f-bab2-8917fdd027f1" Name="IsUntil2019" Type="Boolean Null">
		<Description>Основной договор заключен до 2019</Description>
	</SchemePhysicalColumn>
	<SchemeComplexColumn ID="a7b3c43c-ddce-491c-86d6-d888c6f531a8" Name="MainContract" Type="Reference(Typified) Null" ReferencedTable="018d6a5b-9d0e-4a97-a762-86b91f82fbfb">
		<Description>Основной договор</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="a7b3c43c-ddce-001c-4000-0888c6f531a8" Name="MainContractID" Type="Guid Null" ReferencedColumn="018d6a5b-9d0e-0197-4000-06b91f82fbfb" />
		<SchemeReferencingColumn ID="9e8ecb98-ab88-41c6-9d82-f1262709907a" Name="MainContractSubject" Type="String(Max) Null" ReferencedColumn="9185f5fa-2a0f-49e5-b37f-929e533e31e2" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn ID="222428d1-5870-40a6-9376-eb2fee68b817" Name="Comment" Type="String(Max) Null">
		<Description>Комментарий</Description>
	</SchemePhysicalColumn>
	<SchemeComplexColumn ID="8539510f-2b07-4f38-811b-ee6853bd9d8f" Name="WriteOff" Type="Reference(Typified) Null" ReferencedTable="81f6010b-9641-4aa5-8897-b8e8603fbf4b">
		<Description>Списать в дело</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="8539510f-2b07-0038-4000-0e6853bd9d8f" Name="WriteOffID" Type="Guid Null" ReferencedColumn="81f6010b-9641-01a5-4000-08e8603fbf4b" />
		<SchemeReferencingColumn ID="fcb92677-ff83-4422-9c38-13815c399bae" Name="WriteOffName" Type="String(128) Null" ReferencedColumn="616d6b2e-35d5-424d-846b-618eb25962d0" />
		<SchemeReferencingColumn ID="b5c5288f-4eaa-4223-a5d1-32c7f35d2828" Name="WriteOffIdx" Type="String(Max) Null" ReferencedColumn="1f7ca0d4-1b64-43f8-b973-d14791af51a0" />
	</SchemeComplexColumn>
	<SchemeComplexColumn ID="3029227a-d11f-4e48-a29d-40ecf2ba808a" Name="Kind" Type="Reference(Typified) Null" ReferencedTable="4e9d936c-89c0-470d-be69-b4ce3e0aa294">
		<Description>Вид договора</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="3029227a-d11f-0048-4000-00ecf2ba808a" Name="KindID" Type="Guid Null" ReferencedColumn="4e9d936c-89c0-010d-4000-04ce3e0aa294" />
		<SchemeReferencingColumn ID="4f85d854-6ceb-4db2-9cc1-668686f1b195" Name="KindName" Type="String(128) Null" ReferencedColumn="86c831ea-880e-4fef-ab09-59bc9c36c392" />
	</SchemeComplexColumn>
	<SchemeComplexColumn ID="d4da628b-2f62-4903-a19a-fc823800359e" Name="DownPayment" Type="Reference(Typified) Null" ReferencedTable="9f7e3f9d-38f3-41e1-ba99-668d2aef445a">
		<Description>% авансового платежа</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="d4da628b-2f62-0003-4000-0c823800359e" Name="DownPaymentID" Type="Int16 Null" ReferencedColumn="c474984c-4376-4025-9260-59625b58c702" />
		<SchemeReferencingColumn ID="10b7e177-4722-4d39-ba5d-64900c0eded1" Name="DownPaymentValue" Type="String(Max) Null" ReferencedColumn="1ca24188-9964-4896-bde8-c1d014b470ff" />
	</SchemeComplexColumn>
	<SchemeComplexColumn ID="c0363cbb-5633-4e95-b832-f96f995960a3" Name="DefermentPayment" Type="Reference(Typified) Null" ReferencedTable="b75460ff-5276-4f6e-a04e-e52c02bcb6e0">
		<Description>Отсрочка платежа</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="c0363cbb-5633-0095-4000-096f995960a3" Name="DefermentPaymentID" Type="Int16 Null" ReferencedColumn="2c48c263-dfe2-464c-8249-8acf89730d90" />
		<SchemeReferencingColumn ID="db4d0c83-8ca2-4ef3-a1f0-e9f3fbfea530" Name="DefermentPaymentValue" Type="String(Max) Null" ReferencedColumn="dee708ee-1c93-4c36-b02e-3c9fa6da2b26" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn ID="f16e2cad-8fd0-40d7-9445-a7c51e01e3ab" Name="GroundConcluding" Type="String(Max) Null">
		<Description>Основание для заключения договора</Description>
	</SchemePhysicalColumn>
	<SchemeComplexColumn ID="0676cebe-2fa3-4543-8db9-5fdfbc27350d" Name="IsNoIncreaseAmount" Type="Reference(Typified) Null" ReferencedTable="c77707ce-b8de-4d47-8d55-08c315491728">
		<Description>ДС без повышения стоимости</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="0676cebe-2fa3-0043-4000-0fdfbc27350d" Name="IsNoIncreaseAmountID" Type="Int16 Null" ReferencedColumn="2cd1796f-0b93-4e5b-b0e4-b0bc91d20370" />
		<SchemeReferencingColumn ID="f91d28ac-e8b7-43fa-99c2-ed650d436d0e" Name="IsNoIncreaseAmountName" Type="String(Max) Null" ReferencedColumn="d3a53179-d767-4989-a760-2e085122b5a7" />
	</SchemeComplexColumn>
	<SchemeComplexColumn ID="928b0c00-4ae5-4c4b-abe2-47f27c91176a" Name="IsWarranty" Type="Reference(Typified) Null" ReferencedTable="c77707ce-b8de-4d47-8d55-08c315491728">
		<Description>Гарантийные обязательства</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="928b0c00-4ae5-004b-4000-07f27c91176a" Name="IsWarrantyID" Type="Int16 Null" ReferencedColumn="2cd1796f-0b93-4e5b-b0e4-b0bc91d20370" />
		<SchemeReferencingColumn ID="2c9bae98-85d0-4c23-b7c1-a6083acf739a" Name="IsWarrantyName" Type="String(Max) Null" ReferencedColumn="d3a53179-d767-4989-a760-2e085122b5a7" />
	</SchemeComplexColumn>
	<SchemeComplexColumn ID="d006f3ee-55ca-4cfe-96de-0381d1797cfb" Name="KindDUP" Type="Reference(Typified) Null" ReferencedTable="3537eeca-1c13-4cc8-ad6d-872b6f1b52ca">
		<Description>Вид договора (ДУП)</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="d006f3ee-55ca-00fe-4000-0381d1797cfb" Name="KindDUPID" Type="Guid Null" ReferencedColumn="3537eeca-1c13-01c8-4000-072b6f1b52ca" />
		<SchemeReferencingColumn ID="50578281-fcf3-4723-9c26-d939c5575830" Name="KindDUPName" Type="String(Max) Null" ReferencedColumn="c81e197b-ef24-4565-9597-516257c7eb0a" />
	</SchemeComplexColumn>
	<SchemeComplexColumn ID="41890984-d1dd-45e4-a9db-62d8ae7adbd8" Name="GuaranteePeriod" Type="Reference(Typified) Null" ReferencedTable="2df4371b-30ff-43b8-9049-5895ef3fb1bf">
		<Description>Гарантийный срок на работы (год)</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="41890984-d1dd-00e4-4000-02d8ae7adbd8" Name="GuaranteePeriodID" Type="Int16 Null" ReferencedColumn="5978f4a9-8621-47a1-9988-ac3256271b01" />
		<SchemeReferencingColumn ID="e63c9818-9db2-432e-80f6-82cd338c7070" Name="GuaranteePeriodValue" Type="String(Max) Null" ReferencedColumn="f82e3c75-2582-45ce-9e6a-eec978ebbf58" />
	</SchemeComplexColumn>
	<SchemeComplexColumn ID="061bd4ab-737b-4f99-b8a0-5eb98a281a4a" Name="GuaranteeDeductions" Type="Reference(Typified) Null" ReferencedTable="a4816fd0-bf53-4a61-b56c-afc7563b0f9a">
		<Description>% гарантийных удержаний</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="061bd4ab-737b-0099-4000-0eb98a281a4a" Name="GuaranteeDeductionsID" Type="Int16 Null" ReferencedColumn="df2b660f-dbaa-49e8-8525-09f71f01b287" />
		<SchemeReferencingColumn ID="44e7bbf5-da12-4781-8b25-0fcbf1c713c9" Name="GuaranteeDeductionsValue" Type="String(Max) Null" ReferencedColumn="4768db7c-d6e0-4612-8079-6bb93dd78563" />
	</SchemeComplexColumn>
	<SchemeComplexColumn ID="a2cbe5db-e83e-4415-a5ed-91bd371f51db" Name="PhasedImplementation" Type="Reference(Typified) Null" ReferencedTable="c77707ce-b8de-4d47-8d55-08c315491728">
		<Description>Предусмотрено поэтапное выполнение</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="a2cbe5db-e83e-0015-4000-01bd371f51db" Name="PhasedImplementationID" Type="Int16 Null" ReferencedColumn="2cd1796f-0b93-4e5b-b0e4-b0bc91d20370" />
		<SchemeReferencingColumn ID="c60b58c3-28c6-4715-8fa8-3fbae9bbde60" Name="PhasedImplementationName" Type="String(Max) Null" ReferencedColumn="d3a53179-d767-4989-a760-2e085122b5a7" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn ID="ac9d7862-c6bf-4e24-a0d1-d89bda53eaef" Name="PlannedActDate" Type="Date Null">
		<Description>Планируемая дата актирования</Description>
	</SchemePhysicalColumn>
	<SchemeComplexColumn ID="f618d0d8-9ea4-485a-a2d6-5ce39abdb064" Name="Development" Type="Reference(Typified) Null" ReferencedTable="5252bdd2-2ad8-4525-aa48-b956e522ae9d">
		<Description>Разработка договора</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="f618d0d8-9ea4-005a-4000-0ce39abdb064" Name="DevelopmentID" Type="Int16 Null" ReferencedColumn="7b0bf1da-7eb9-4f66-8d62-4962e8dc4888" />
		<SchemeReferencingColumn ID="7a763947-c3c9-4cd8-bb51-3c09f3595211" Name="DevelopmentName" Type="String(Max) Null" ReferencedColumn="bdf2b76d-7422-4258-b29a-462e67376af0" />
	</SchemeComplexColumn>
	<SchemeComplexColumn ID="f355ae74-c0c6-4573-84d5-3e108549e44f" Name="TypeDUP" Type="Reference(Typified) Null" ReferencedTable="fcf98667-97d7-4863-bd5e-4096fb3e7340">
		<Description>Тип договора ДУП</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="f355ae74-c0c6-0073-4000-0e108549e44f" Name="TypeDUPID" Type="Guid Null" ReferencedColumn="fcf98667-97d7-0163-4000-0096fb3e7340" />
		<SchemeReferencingColumn ID="bbbf92ab-a2cb-4f2b-94a0-912f712168ff" Name="TypeDUPName" Type="String(Max) Null" ReferencedColumn="870c1bc8-5fcd-4b05-83a9-239febcde3c3" />
	</SchemeComplexColumn>
	<SchemeComplexColumn ID="08ceef57-3012-4453-96d6-12f957c4333a" Name="ConditionStartingWork" Type="Reference(Typified) Null" ReferencedTable="e50c0238-8162-4db0-bf46-b54ef3f36bc7">
		<Description>Условие начала выполнения работ</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="08ceef57-3012-0053-4000-02f957c4333a" Name="ConditionStartingWorkID" Type="Guid Null" ReferencedColumn="e50c0238-8162-01b0-4000-054ef3f36bc7" />
		<SchemeReferencingColumn ID="294dffac-e44d-4fec-8689-1615886c19c1" Name="ConditionStartingWorkName" Type="String(Max) Null" ReferencedColumn="a995dc5f-63af-4320-9a09-f52960df09af" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn ID="38e4350c-b550-4114-87a6-f0577a84c503" Name="LinkCardCRM" Type="String(Max) Null">
		<Description>Ссылка на карточку договора в CRM</Description>
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="f8a3a524-07f2-42df-bd39-704e7d35fcb0" Name="MDMKey" Type="String(Max) Null">
		<Description>MDM-Key</Description>
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="cc93f81c-3d67-4cc1-9c12-5ed5a0ea2ace" Name="HyperlinkCard" Type="String(Max) Null">
		<Description>Гиперссылка на карточку</Description>
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="4da8c6f5-8817-4390-922c-79aae66aa22c" Name="ParentMDM" Type="String(Max) Null">
		<Description>MDM родителя</Description>
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="30134b46-62ea-46f6-8bcd-6519b7374e7e" Name="ExtID" Type="Guid Null">
		<Description>Уникальный ID ДС из системы заказчика</Description>
	</SchemePhysicalColumn>
	<SchemeComplexColumn ID="7a8db724-bf51-4dbe-b3b0-e0275784fc6d" Name="CFO" Type="Reference(Typified) Null" ReferencedTable="b5e873a7-4f25-4731-b7bf-93586f07b53a">
		<Description>ЦФО</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="7a8db724-bf51-00be-4000-00275784fc6d" Name="CFOID" Type="Guid Null" ReferencedColumn="b5e873a7-4f25-0131-4000-03586f07b53a" />
		<SchemeReferencingColumn ID="3af03c56-cf31-433a-964b-16368a42b88f" Name="CFOName" Type="String(Max) Null" ReferencedColumn="20d4f2eb-ce34-4c44-87b8-8b386c283930" />
	</SchemeComplexColumn>
	<SchemeComplexColumn ID="932e9c57-edd2-4fd6-8293-4a74555aad72" Name="Signatory" Type="Reference(Typified) Null" ReferencedTable="6c977939-bbfc-456f-a133-f1c2244e3cc3">
		<Description>Подписант</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="932e9c57-edd2-00d6-4000-0a74555aad72" Name="SignatoryID" Type="Guid Null" ReferencedColumn="6c977939-bbfc-016f-4000-01c2244e3cc3" />
		<SchemeReferencingColumn ID="28525840-6b4f-4f50-8bed-04163dcde174" Name="SignatoryName" Type="String(128) Null" ReferencedColumn="1782f76a-4743-4aa4-920c-7edaee860964" />
	</SchemeComplexColumn>
	<SchemeComplexColumn ID="dc75650f-9b8d-48ea-91da-61900db6d37f" Name="ImplementationStage" Type="Reference(Typified) Null" ReferencedTable="acb2c154-f00a-4dbd-adb0-8b77ca861ae2">
		<Description>Стадия реализации</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="dc75650f-9b8d-00ea-4000-01900db6d37f" Name="ImplementationStageID" Type="Guid Null" ReferencedColumn="acb2c154-f00a-01bd-4000-0b77ca861ae2" />
		<SchemeReferencingColumn ID="b625c9b2-22ad-4e50-b334-1ce7f866437b" Name="ImplementationStageName" Type="String(Max) Null" ReferencedColumn="32b4bb15-8adb-4851-b9c6-d990cb2e3396" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn ID="46ca4a23-46a5-487e-9ad9-2f4b447bd642" Name="MDMSentDate" Type="DateTime Null">
		<Description>Дата последней отправки в MDM (НСИ), если пусто, значит отправки не было.</Description>
	</SchemePhysicalColumn>
	<SchemeComplexColumn ID="96038baa-247c-4ec3-abfa-45b33ead69f4" Name="Department" Type="Reference(Typified) Null" ReferencedTable="81f6010b-9641-4aa5-8897-b8e8603fbf4b">
		<Description>Подразделение</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="96038baa-247c-00c3-4000-05b33ead69f4" Name="DepartmentID" Type="Guid Null" ReferencedColumn="81f6010b-9641-01a5-4000-08e8603fbf4b" />
		<SchemeReferencingColumn ID="3f712750-8698-4c07-82cb-1ccbf5ebc8c7" Name="DepartmentName" Type="String(128) Null" ReferencedColumn="616d6b2e-35d5-424d-846b-618eb25962d0" />
		<SchemeReferencingColumn ID="9e21c71e-9a56-4e79-8232-be06291df64e" Name="DepartmentIdx" Type="String(Max) Null" ReferencedColumn="1f7ca0d4-1b64-43f8-b973-d14791af51a0" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn ID="d198814a-8d12-48ec-abf0-7d1dae511617" Name="GuaranteePeriodMonth" Type="Int32 Null">
		<Description>Гарантийный срок на работы (мес)</Description>
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="8c1e258c-6e50-4384-9bee-b9b12927c176" Name="IsProjectInArchive" Type="Boolean Null">
		<Description>Проект карточки 'Завершенный'</Description>
		<SchemeDefaultConstraint IsPermanent="true" ID="53525099-3f0c-4438-b081-661adba1ac8d" Name="df_PnrSupplementaryAgreements_IsProjectInArchive" Value="false" />
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="8de1308f-2c2b-4fde-813c-19098880d82a" Name="IsHeadDepartmentMTOInStage" Type="Boolean Null">
		<Description>Рук-ль отдела МТО на стадии Внутреннее согласование договора ДУП</Description>
		<SchemeDefaultConstraint IsPermanent="true" ID="57bc33c5-b168-4cfa-a4a6-99483343ea86" Name="df_PnrSupplementaryAgreements_IsHeadDepartmentMTOInStage" Value="false" />
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="e39a740f-da58-477e-a267-e53b4c62c6c9" Name="IsHeadDepartmentSVISInStage" Type="Boolean Null">
		<Description>Рук-ль СВИС на стадии Внутреннее согласование договора ДУП</Description>
		<SchemeDefaultConstraint IsPermanent="true" ID="a277628a-ad58-4930-9c9b-f27b439b5457" Name="df_PnrSupplementaryAgreements_IsHeadDepartmentSVISInStage" Value="false" />
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="4f545f33-4319-4e2b-abae-7aeb9d20b8ec" Name="IsHeadDepartmentSPIAInStage" Type="Boolean Null">
		<Description>Рук-ль СПиА на стадии Внутреннее согласование договора ДУП</Description>
		<SchemeDefaultConstraint IsPermanent="true" ID="26e4f141-f8ae-4c1f-b69a-ba7b8e6d0cf6" Name="df_PnrSupplementaryAgreements_IsHeadDepartmentSPIAInStage" Value="false" />
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="4c64c32e-bfda-45c0-afd9-bbb11380e877" Name="IsProjectGIPInStage" Type="Boolean Null">
		<Description>ГИП проекта на стадии Внутреннее согласование договора ДУП</Description>
		<SchemeDefaultConstraint IsPermanent="true" ID="6128ea47-2bfa-472e-a21e-e6552bd76580" Name="df_PnrSupplementaryAgreements_IsProjectGIPInStage" Value="false" />
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="7a001dd8-699a-4f61-85d1-5ed8c4d34a95" Name="IsConstructionManagerInStage" Type="Boolean Null">
		<Description>Рук-ль строительства на стадии Внутреннее согласование договора ДУП</Description>
		<SchemeDefaultConstraint IsPermanent="true" ID="bb5652fb-87e0-49b0-884b-badc242cbc3f" Name="df_PnrSupplementaryAgreements_IsConstructionManagerInStage" Value="false" />
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="093047ad-40b8-41da-97a8-f32c4874ab42" Name="IsInternalApprovalStage" Type="Boolean Null">
		<Description>Внутреннее согласование договора ДУП в маршруте</Description>
		<SchemeDefaultConstraint IsPermanent="true" ID="03a54640-3027-4338-bc7d-f7c1e5d28d00" Name="df_PnrSupplementaryAgreements_IsInternalApprovalStage" Value="false" />
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="26266fa0-3d8a-4f1e-8f5f-cb95af53c37d" Name="ApartmentNumber" Type="String(Max) Null" />
	<SchemePhysicalColumn ID="4471103b-8113-4c53-bf28-f45ae3d95a92" Name="Flat" Type="String(Max) Null" />
	<SchemePhysicalColumn ID="06e1c682-bbe3-4207-8164-d26e86ac8f2b" Name="ActionStatus" Type="String(Max) Null" />
	<SchemePhysicalColumn ID="e6cfa839-f2f9-4236-8ebe-6528fc9b8d35" Name="Urgency" Type="String(Max) Null" />
	<SchemePhysicalColumn ID="24273db7-02d4-4543-b5dd-c0e7888ccef2" Name="MDMContractNumber" Type="String(Max) Null" />
	<SchemeComplexColumn ID="5b25ca86-df10-44bc-b5b1-e75c1ea54d2d" Name="CRMApprove" Type="Reference(Typified) Null" ReferencedTable="0df3fb02-10e0-4049-8f88-ed070acaa11e">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="5b25ca86-df10-00bc-4000-075c1ea54d2d" Name="CRMApproveID" Type="Int16 Null" ReferencedColumn="a74a1d03-37a9-4ca0-be09-6db0463cbbb9" />
		<SchemeReferencingColumn ID="fe75d8f5-815d-4640-9fe5-5cf5aea6a014" Name="CRMApproveName" Type="String(Max) Null" ReferencedColumn="39381382-1bd5-499a-b858-47de893eeae0" />
	</SchemeComplexColumn>
	<SchemePrimaryKey IsSystem="true" IsPermanent="true" IsSealed="true" ID="3bcd9132-1236-0048-5000-0b0de8d5b0e4" Name="pk_PnrSupplementaryAgreements" IsClustered="true">
		<SchemeIndexedColumn Column="3bcd9132-1236-0148-4000-0b0de8d5b0e4" />
	</SchemePrimaryKey>
	<SchemeIndex ID="ac001a89-01c1-4398-89f0-91275562ba35" Name="ndx_PnrSupplementaryAgreements_IDPartnerName">
		<SchemeIndexedColumn Column="3bcd9132-1236-0148-4000-0b0de8d5b0e4" />
		<SchemeIndexedColumn Column="08cfe531-d567-422d-b97b-c4306fcdd342" />
	</SchemeIndex>
</SchemeTable>