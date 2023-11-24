<?xml version="1.0" encoding="utf-8"?>
<SchemeTable Partition="54ed79f5-11c0-4f74-b8b0-793d596699d6" ID="303dd35d-b998-49b3-a10b-ed30d31bd36a" Name="PnrOrganizations" Group="Pnr" InstanceType="Cards" ContentType="Entries">
	<Description>Справочник организаций</Description>
	<SchemeComplexColumn IsSystem="true" IsPermanent="true" IsSealed="true" ID="303dd35d-b998-00b3-2000-0d30d31bd36a" Name="ID" Type="Reference(Typified) Not Null" ReferencedTable="1074eadd-21d7-4925-98c8-40d1e5f0ca0e">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="303dd35d-b998-01b3-4000-0d30d31bd36a" Name="ID" Type="Guid Not Null" ReferencedColumn="9a58123b-b2e9-4137-9c6c-5dab0ec02747" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn ID="d8ecabaa-f192-4546-9af6-1f9712daabdb" Name="Name" Type="String(Max) Null">
		<Description>Наименование</Description>
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="2ea114e1-9f71-43bd-873c-719f8c237244" Name="FullName" Type="String(Max) Null">
		<Description>Полное наименование</Description>
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="acc98b26-c920-44dd-bf67-609f52181c71" Name="ShortName" Type="String(Max) Null">
		<Description>Сокращенное наименование</Description>
	</SchemePhysicalColumn>
	<SchemeComplexColumn ID="23848b3c-8dbc-4ad2-bb0d-6ca40c91f754" Name="GeneralDirector" Type="Reference(Typified) Null" ReferencedTable="6c977939-bbfc-456f-a133-f1c2244e3cc3">
		<Description>Генеральный директор</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="23848b3c-8dbc-00d2-4000-0ca40c91f754" Name="GeneralDirectorID" Type="Guid Null" ReferencedColumn="6c977939-bbfc-016f-4000-01c2244e3cc3" />
		<SchemeReferencingColumn ID="d1cff8d0-6d3f-4b62-8aaa-efa5b9226335" Name="GeneralDirectorName" Type="String(128) Null" ReferencedColumn="1782f76a-4743-4aa4-920c-7edaee860964" />
	</SchemeComplexColumn>
	<SchemeComplexColumn ID="9168ed46-4a16-4f75-b5f8-e8a31d710458" Name="ChiefAccountant" Type="Reference(Typified) Null" ReferencedTable="6c977939-bbfc-456f-a133-f1c2244e3cc3">
		<Description>Главный бухгалтер</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="9168ed46-4a16-0075-4000-08a31d710458" Name="ChiefAccountantID" Type="Guid Null" ReferencedColumn="6c977939-bbfc-016f-4000-01c2244e3cc3" />
		<SchemeReferencingColumn ID="df5642b8-f65f-4c00-9699-3e5fb2f80775" Name="ChiefAccountantName" Type="String(128) Null" ReferencedColumn="1782f76a-4743-4aa4-920c-7edaee860964" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn ID="5d4f389e-749f-4a1b-8db1-71562e568a70" Name="Prefix" Type="String(Max) Null">
		<Description>Префикс</Description>
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="2ef9c641-e677-4970-9a26-238ec8095fa9" Name="PartnerType" Type="String(Max) Null">
		<Description>Тип контрагента</Description>
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="1fa7ca1c-120c-45fd-9fe0-7ed93865af1b" Name="INN" Type="String(Max) Null">
		<Description>ИНН</Description>
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="9698eaea-033c-40e7-9a2d-0553a079cd81" Name="KPP" Type="String(Max) Null">
		<Description>КПП</Description>
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="24d213ae-da88-422b-97ed-b631fd71e063" Name="OKPO" Type="String(Max) Null">
		<Description>ОКПО</Description>
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="00e8cc14-b921-4746-a87d-9b15adac9209" Name="OGRN" Type="String(Max) Null">
		<Description>ОГРН</Description>
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="5db44c98-dc86-4f40-ab17-770aa8519372" Name="IssueDate" Type="Date Null">
		<Description>Дата выдачи</Description>
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="acb22e78-5fc7-44db-a882-aa618c1e097b" Name="RegistrationDate" Type="Date Null">
		<Description>Дата регистрации</Description>
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="fddcc1bb-3555-4aaa-a72a-e3affcbee251" Name="ForeignOrganization" Type="String(Max) Null">
		<Description>Иностранная организация</Description>
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="65214bda-52e9-4513-8348-50f0a62fc146" Name="ForeignOrganizationName" Type="String(Max) Null">
		<Description>Наименование иностранной организации</Description>
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="69cc3ee3-5b44-44f6-8b60-557c759b6ff5" Name="TaxAuthorityCode" Type="String(Max) Null">
		<Description>Код налогового органа</Description>
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="ab9859a0-9fab-4427-a506-65c2250dfbec" Name="TaxAuthorityName" Type="String(Max) Null">
		<Description>Наименование налогового органа</Description>
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="057a740e-7f87-472c-9681-5813e6180745" Name="OKVEDCode" Type="String(Max) Null">
		<Description>Код ОКВЭД</Description>
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="f6f45362-605a-43ae-9a2e-3265cf59eecd" Name="OKVEDName" Type="String(Max) Null">
		<Description>Наименование ОКВЭД</Description>
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="0f274097-d5ba-4b58-8589-7eb03b7fd88c" Name="OKVEDCode2" Type="String(Max) Null">
		<Description>Код ОКВЭД 2</Description>
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="7f0c0e51-61ad-4039-a9a6-318f7186e89b" Name="OKVEDName2" Type="String(Max) Null">
		<Description>Наименование ОКВЭД 2</Description>
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="389b0ee2-e454-4bb5-b2d9-c31251b2cf4a" Name="OKOPFCode" Type="String(Max) Null">
		<Description>Код ОКОПФ</Description>
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="90ec6bca-2e2b-448f-99c2-7932caa2eeb8" Name="OKOPFName" Type="String(Max) Null">
		<Description>Наименование ОКОПФ</Description>
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="5ae0dd06-bcb7-4879-9593-f09fd483607a" Name="OKFSCode" Type="String(Max) Null">
		<Description>Код ОКФС</Description>
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="91942f91-2ee2-4a59-9df4-364e162c5791" Name="OKFSName" Type="String(Max) Null">
		<Description>Наименование ОКФС</Description>
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="0313a136-2d0b-44fc-81b8-9aefa0d48c7a" Name="AuthorityPFRCode" Type="String(Max) Null">
		<Description>Код органа ПФР</Description>
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="0e97bfd7-43a6-44aa-b75e-d703a4f115fe" Name="TerritorialAuthorityPFRName" Type="String(Max) Null">
		<Description>Наименование территориального органа ПФР</Description>
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="6c05e821-fa79-4d97-b30a-fa875e00cef4" Name="RegistrationNumberPFR" Type="String(Max) Null">
		<Description>Регистрационный номер ПФР</Description>
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="d6d682de-17c4-4483-acdd-85a7f83d5b4a" Name="AuthorityFSGSCode" Type="String(Max) Null">
		<Description>Код органа ФСГС</Description>
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="23684d8c-368f-4351-a737-821a4a9f2117" Name="TerritorialAuthorityFSSName" Type="String(Max) Null">
		<Description>Наименование территориального органа ФCC</Description>
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="bbcef54e-a238-4a3c-a348-5e7d70fcc980" Name="CertificateAuthorityCode" Type="String(Max) Null">
		<Description>Свидетельство код органа</Description>
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="02cd624c-f2f7-422b-80f5-6b6fb07a7920" Name="CertificateAuthorityName" Type="String(Max) Null">
		<Description>Свидетельство наименование органа</Description>
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="791d4426-32f9-4029-8403-6e2273ce3ad1" Name="CertificateSeriesNumber" Type="String(Max) Null">
		<Description>Серия номер свидетельства</Description>
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="6a1406ce-3435-4bc5-9561-c37b262a3f1a" Name="RegistrationNumberFSS" Type="String(Max) Null">
		<Description>Регистрационный номер ФСС</Description>
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="8a9b7148-f248-42f3-976b-676f486b4e62" Name="SubordinationCodeFSS" Type="String(Max) Null">
		<Description>Код подчиненности ФСС</Description>
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="98c8ffc8-85b0-4bae-bd8f-e9c1ade9fd5f" Name="Removed" Type="String(Max) Null">
		<Description>Удалено</Description>
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="1d3336cd-2bdc-422a-ae22-04debd9cb312" Name="ReferencePosition" Type="String(Max) Null">
		<Description>Эталонная позиция</Description>
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="981e1764-4513-4f64-aaee-5d8ffb25d6f4" Name="ContactInformation" Type="String(Max) Null">
		<Description>Контактная информация</Description>
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="f03b6b3d-8ea9-4c77-9fb1-092f89ed8255" Name="MDMKey" Type="String(Max) Null">
		<Description>MDM_key</Description>
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="51197f31-5d8e-4e7c-a2c2-2190a56da87a" Name="Abbreviation" Type="String(Max) Null">
		<Description>Аббревиатура</Description>
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="26a9308e-a4cf-435c-be6a-68d3c49cfc8c" Name="PositionHeadLegalEntity" Type="String(Max) Null">
		<Description>Должность руководителя ЮЛ</Description>
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="b298d65c-7eb6-42ac-84a2-90e9e5f2c57d" Name="PositionHeadLegalEntityDative" Type="String(Max) Null">
		<Description>Должность руководителя ЮЛ  в дательном падеже</Description>
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="ecf2a2ab-ab54-4172-9dc1-c53c6c6128e2" Name="PositionHeadLegalEntityGenitive" Type="String(Max) Null">
		<Description>Должность руководителя ЮЛ  в родительном падеже</Description>
	</SchemePhysicalColumn>
	<SchemeComplexColumn ID="3ef60d1e-155b-48cf-b618-31b257fab9c4" Name="HeadLegalEntity" Type="Reference(Typified) Null" ReferencedTable="6c977939-bbfc-456f-a133-f1c2244e3cc3">
		<Description>Руководитель ЮЛ </Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="3ef60d1e-155b-00cf-4000-01b257fab9c4" Name="HeadLegalEntityID" Type="Guid Null" ReferencedColumn="6c977939-bbfc-016f-4000-01c2244e3cc3" />
		<SchemeReferencingColumn ID="32fb5c55-45ae-45f0-8a7f-5a992c3173e0" Name="HeadLegalEntityName" Type="String(128) Null" ReferencedColumn="1782f76a-4743-4aa4-920c-7edaee860964" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn ID="373ce295-184d-47fd-aaa2-f7d888f0b8f3" Name="HeadLegalEntityDative" Type="String(Max) Null">
		<Description>Руководитель ЮЛ (ФИО в дат. падеже)</Description>
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="69660686-a619-456e-a2c9-6fb7988e7015" Name="HeadLegalEntityGenitive" Type="String(Max) Null">
		<Description>Руководитель ЮЛ (ФИО в род. падеже)</Description>
	</SchemePhysicalColumn>
	<SchemeComplexColumn ID="0921024e-d892-4c16-b039-3b8a37cf1024" Name="ChiefAccountantProcess" Type="Reference(Typified) Null" ReferencedTable="6c977939-bbfc-456f-a133-f1c2244e3cc3">
		<Description>Главный бухгалтер для процессов</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="0921024e-d892-0016-4000-0b8a37cf1024" Name="ChiefAccountantProcessID" Type="Guid Null" ReferencedColumn="6c977939-bbfc-016f-4000-01c2244e3cc3" />
		<SchemeReferencingColumn ID="e6822a6d-ae73-4fac-aa7e-38afc8a8f9e0" Name="ChiefAccountantProcessName" Type="String(128) Null" ReferencedColumn="1782f76a-4743-4aa4-920c-7edaee860964" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn ID="ec9eab45-ce95-4e03-8b26-c1df01152899" Name="Idx" Type="String(Max) Null">
		<Description>Индекс</Description>
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="e89159c9-431d-43d8-883d-17ea07053a4e" Name="Address" Type="String(Max) Null">
		<Description>Адрес</Description>
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="0032da35-0b42-48ab-95f4-fbb4bac82533" Name="Bank" Type="String(Max) Null">
		<Description>Банк</Description>
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="90e9bfa8-f770-4850-bbb6-d9318ef6ec5d" Name="INNBank" Type="String(Max) Null">
		<Description>ИНН</Description>
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="e2a39ea3-62e2-40f4-860d-f080b9e98d60" Name="BIK" Type="String(Max) Null">
		<Description>БИК</Description>
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="9f4ec5ad-d072-4875-836a-18e4a4f129dd" Name="SettlementAccount" Type="String(Max) Null">
		<Description>Расчетный счет</Description>
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="e596b730-0448-42aa-b555-057e9dd65041" Name="CorrespondentAccount" Type="String(Max) Null">
		<Description>Корр. счет №</Description>
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="968b936b-4502-43fb-8389-74a19a841a73" Name="WorkPhone" Type="String(Max) Null">
		<Description>Рабочий телефон</Description>
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="dddb5a79-1fa1-43c7-ac15-c809c83f479d" Name="FaxNumber" Type="String(Max) Null">
		<Description>Номер факса</Description>
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="859407a2-4e3c-427f-a432-a19c138eab26" Name="Website" Type="String(Max) Null">
		<Description>Сайт</Description>
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="4351867a-083d-4593-96eb-9840d91b3804" Name="OKPOCode" Type="String(Max) Null">
		<Description>Код по ОКПО</Description>
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="d8532e99-f60b-4bbb-84a3-354864ad7d19" Name="OKATO" Type="String(Max) Null">
		<Description>ОКАТО</Description>
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="c5b6e213-0de3-48c4-b105-fe28b4a6407e" Name="OKOGU" Type="String(Max) Null">
		<Description>ОКОГУ</Description>
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="aaf35fe5-ccf6-47c7-915a-60ecfa354904" Name="OKTMO" Type="String(Max) Null">
		<Description>ОКТМО</Description>
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="c35dfe04-01bd-4c07-87ce-ed0fd2fa00d9" Name="GroupDocuments" Type="String(Max) Null">
		<Description>Группа документов</Description>
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="b1872260-eb83-40d7-b63d-abae538a0138" Name="RegistryCard" Type="String(Max) Null">
		<Description>Учетная карточка</Description>
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="c3c4145b-5bab-4e22-8a03-141a25ef4a85" Name="ActionStatus" Type="String(Max) Null">
		<Description>Статус действия</Description>
	</SchemePhysicalColumn>
	<SchemeComplexColumn ID="9456d488-04c0-4652-9973-1015696666aa" Name="Partner" Type="Reference(Typified) Null" ReferencedTable="5d47ef13-b6f4-47ef-9815-3b3d0e6d475a">
		<Description>КА</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="9456d488-04c0-0052-4000-0015696666aa" Name="PartnerID" Type="Guid Null" ReferencedColumn="5d47ef13-b6f4-01ef-4000-0b3d0e6d475a" />
		<SchemeReferencingColumn ID="d8c047a6-ea8e-4abd-af1f-1207c8ff9374" Name="PartnerName" Type="String(255) Null" ReferencedColumn="f1c960e0-951e-4837-8474-bb61d98f40f0" />
	</SchemeComplexColumn>
	<SchemeComplexColumn ID="26196ed1-faa0-432c-99ee-7ca1d4cdc8c0" Name="CountryPermanentResidence" Type="Reference(Typified) Null" ReferencedTable="ba073bea-da30-46b2-badd-98ebb9e3c1ac">
		<Description>Страна постоянного местонахождения</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="26196ed1-faa0-002c-4000-0ca1d4cdc8c0" Name="CountryPermanentResidenceID" Type="Guid Null" ReferencedColumn="ba073bea-da30-01b2-4000-08ebb9e3c1ac" />
		<SchemeReferencingColumn ID="8050d7a7-50ac-4819-aa20-3fa6e501cc1c" Name="CountryPermanentResidenceName" Type="String(Max) Null" ReferencedColumn="fc3e5211-75f1-4b9a-8585-980fbb64df82" />
	</SchemeComplexColumn>
	<SchemeComplexColumn ID="849d63c5-2cfb-4364-a10b-ae38724b1b7a" Name="CountryRegistration" Type="Reference(Typified) Null" ReferencedTable="ba073bea-da30-46b2-badd-98ebb9e3c1ac">
		<Description>Страна регистрации</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="849d63c5-2cfb-0064-4000-0e38724b1b7a" Name="CountryRegistrationID" Type="Guid Null" ReferencedColumn="ba073bea-da30-01b2-4000-08ebb9e3c1ac" />
		<SchemeReferencingColumn ID="44d98bc1-6e8e-4e3f-a90f-51a480b23111" Name="CountryRegistrationName" Type="String(Max) Null" ReferencedColumn="fc3e5211-75f1-4b9a-8585-980fbb64df82" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn ID="d16a0f17-c4b6-405b-af03-e20ba1aa16a2" Name="LegalEntityIndex" Type="String(64) Null">
		<Description>Индекс юридических лиц</Description>
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="7657f787-3874-4884-92bf-24fa49c0fa63" Name="IsHidden" Type="Boolean Null" />
	<SchemePrimaryKey IsSystem="true" IsPermanent="true" IsSealed="true" ID="303dd35d-b998-00b3-5000-0d30d31bd36a" Name="pk_PnrOrganizations" IsClustered="true">
		<SchemeIndexedColumn Column="303dd35d-b998-01b3-4000-0d30d31bd36a" />
	</SchemePrimaryKey>
</SchemeTable>