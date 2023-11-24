<?xml version="1.0" encoding="utf-8"?>
<SchemeTable Partition="54ed79f5-11c0-4f74-b8b0-793d596699d6" ID="dadfadd9-d28c-454e-aebb-d900fda5a6eb" Name="PnrPartnerRequests" Group="Pnr" InstanceType="Cards" ContentType="Entries">
	<Description>Заявка на контрагента</Description>
	<SchemeComplexColumn IsSystem="true" IsPermanent="true" IsSealed="true" ID="dadfadd9-d28c-004e-2000-0900fda5a6eb" Name="ID" Type="Reference(Typified) Not Null" ReferencedTable="1074eadd-21d7-4925-98c8-40d1e5f0ca0e">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="dadfadd9-d28c-014e-4000-0900fda5a6eb" Name="ID" Type="Guid Not Null" ReferencedColumn="9a58123b-b2e9-4137-9c6c-5dab0ec02747" />
	</SchemeComplexColumn>
	<SchemeComplexColumn ID="3228c4b9-303a-4ee3-8ec2-58b628010602" Name="RequestType" Type="Reference(Typified) Null" ReferencedTable="ded5bb90-77ba-40fb-9bf2-9bf740d13cdd">
		<Description>Тип заявки</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="3228c4b9-303a-00e3-4000-08b628010602" Name="RequestTypeID" Type="Int16 Null" ReferencedColumn="2f368bfb-94ed-4196-bfa2-725904d66e17" />
		<SchemeReferencingColumn ID="f7eacda5-a0f6-4401-82ac-0749c490a426" Name="RequestTypeName" Type="String(Max) Null" ReferencedColumn="e618de38-4d8d-4dbb-9b95-bf73e65bf80f" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn ID="bd487677-54a9-40f6-a3f3-600e6cc688f8" Name="RegistrationNo" Type="String(Max) Null">
		<Description>Рег. №</Description>
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="ec87f923-9553-4c1c-8fd1-d173a0baea91" Name="RegistrationDate" Type="Date Null">
		<Description>Дата регистрации</Description>
	</SchemePhysicalColumn>
	<SchemeComplexColumn ID="9f3dd989-04f8-463f-9c7a-3ffc64c8de92" Name="Partner" Type="Reference(Typified) Null" ReferencedTable="5d47ef13-b6f4-47ef-9815-3b3d0e6d475a">
		<Description>Контрагент</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="9f3dd989-04f8-003f-4000-0ffc64c8de92" Name="PartnerID" Type="Guid Null" ReferencedColumn="5d47ef13-b6f4-01ef-4000-0b3d0e6d475a" />
		<SchemeReferencingColumn ID="b62498e1-a976-422b-9375-6f858fc7ab1b" Name="PartnerName" Type="String(255) Null" ReferencedColumn="f1c960e0-951e-4837-8474-bb61d98f40f0" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn ID="16438fc3-c5a9-4064-90b0-597d2afe0065" Name="Name" Type="String(Max) Null">
		<Description>Наименование</Description>
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="9ae94b2f-f7f9-4ac0-86d0-086166615ad0" Name="FullName" Type="String(Max) Null">
		<Description>Полное наименование контрагента</Description>
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="067e9e76-be83-4bba-a448-d46c46d49458" Name="ShortName" Type="String(Max) Null">
		<Description>Краткое наименование контагента</Description>
	</SchemePhysicalColumn>
	<SchemeComplexColumn ID="af585588-d825-4940-aaa4-29e6c36f6738" Name="Type" Type="Reference(Typified) Null" ReferencedTable="354e4f5a-e50c-4a11-84d0-6e0a98a81ca5">
		<Description>Тип контрагента</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="af585588-d825-0040-4000-09e6c36f6738" Name="TypeID" Type="Int32 Null" ReferencedColumn="876c8cd8-505f-40f4-ba4a-65ae78b22945" />
		<SchemeReferencingColumn ID="8e80dda8-be29-4fcf-b1d4-2c5feb6fd569" Name="TypeName" Type="String(256) Null" ReferencedColumn="695e6069-4bde-406a-b880-a0a27c87117e" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn ID="95954600-c5db-4e82-96b6-748385a54d5d" Name="INN" Type="String(Max) Null">
		<Description>ИНН</Description>
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="64d77c0a-df94-4435-8f26-abbc9edd6b13" Name="KPP" Type="String(Max) Null">
		<Description>КПП</Description>
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="19b4705e-40b9-46a4-b5ad-0cf64c96ad62" Name="OGRN" Type="String(Max) Null">
		<Description>ОГРН</Description>
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="07e07dce-12dc-4a2d-bb09-e151f4da57bc" Name="Passport" Type="String(Max) Null">
		<Description>Паспортные данные</Description>
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="ce3c1a76-b0de-4905-81c6-a1b95f3d4668" Name="Birthday" Type="Date Null">
		<Description>День рождения</Description>
	</SchemePhysicalColumn>
	<SchemeComplexColumn ID="5376755a-690e-488e-8ca0-12821bc6f917" Name="CountryRegistration" Type="Reference(Typified) Null" ReferencedTable="ba073bea-da30-46b2-badd-98ebb9e3c1ac">
		<Description>Страна регистрации</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="5376755a-690e-008e-4000-02821bc6f917" Name="CountryRegistrationID" Type="Guid Null" ReferencedColumn="ba073bea-da30-01b2-4000-08ebb9e3c1ac" />
		<SchemeReferencingColumn ID="77fe77bc-f80c-47ae-bd97-976c8d62918c" Name="CountryRegistrationName" Type="String(Max) Null" ReferencedColumn="fc3e5211-75f1-4b9a-8585-980fbb64df82" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn ID="58890cb8-0ed2-4912-a339-8d37d61cc0ff" Name="Comment" Type="String(Max) Null">
		<Description>Комментарии</Description>
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="64a216d2-2d15-4dd9-a3ef-181b532f359c" Name="RequiresApprovalCA" Type="Boolean Null">
		<Description>Требует согласование КА</Description>
	</SchemePhysicalColumn>
	<SchemeComplexColumn ID="a8a148c5-66a1-4f74-a7d2-a6d7a84712b7" Name="SpecialSign" Type="Reference(Typified) Null" ReferencedTable="f3fe8747-5d52-4f09-b34a-2cc0309565a6">
		<Description>Особый признак контрагента</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="a8a148c5-66a1-0074-4000-06d7a84712b7" Name="SpecialSignID" Type="Int16 Null" ReferencedColumn="05cf3057-a85c-461c-91db-985517c720ac" />
		<SchemeReferencingColumn ID="0bac8315-969b-4c2e-ab3f-1a66e0920db1" Name="SpecialSignName" Type="String(Max) Null" ReferencedColumn="d4304713-0b40-4aa0-8a00-46a059bc395c" />
	</SchemeComplexColumn>
	<SchemeComplexColumn ID="3cdeac59-26b1-4024-8330-0877951e8adc" Name="NonResident" Type="Reference(Typified) Null" ReferencedTable="c77707ce-b8de-4d47-8d55-08c315491728">
		<Description>Нерезидент</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="3cdeac59-26b1-0024-4000-0877951e8adc" Name="NonResidentID" Type="Int16 Null" ReferencedColumn="2cd1796f-0b93-4e5b-b0e4-b0bc91d20370" />
		<SchemeReferencingColumn ID="bdb38bb6-ec74-4fbe-989e-21792642b47e" Name="NonResidentName" Type="String(Max) Null" ReferencedColumn="d3a53179-d767-4989-a760-2e085122b5a7" />
	</SchemeComplexColumn>
	<SchemeComplexColumn ID="33a33ba4-24be-43ec-bb83-4d4bd6b6dcdd" Name="Direction" Type="Reference(Typified) Null" ReferencedTable="41d0d309-7f0b-4f14-a0b4-0f55c50fed7b">
		<Description>Направление</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="33a33ba4-24be-00ec-4000-0d4bd6b6dcdd" Name="DirectionID" Type="Int16 Null" ReferencedColumn="33080600-7fdf-4c78-a8a4-d7dcf5e5d5cc" />
		<SchemeReferencingColumn ID="0f84b9d7-b41c-4a18-aaca-4e72ec49997a" Name="DirectionName" Type="String(Max) Null" ReferencedColumn="c8bed3cc-fc2a-4232-b43a-8559fd5fa4d3" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn ID="5c9ffcc3-84a6-43eb-9173-41d497177b3f" Name="IdentityDocument" Type="String(256) Null">
		<Description>Документ, удостоверяющий личность</Description>
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="ef8286e4-e2e4-4bd8-8637-aa31132f4bd3" Name="IdentityDocumentKind" Type="String(128) Null">
		<Description>Документ. Вид документа.</Description>
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="78167913-126f-48bf-848d-4cfb8fc7914d" Name="IdentityDocumentSeries" Type="String(32) Null">
		<Description>Документ. Серия.</Description>
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="76928df2-2b6a-4f0b-a535-c8a70ec60d84" Name="IdentityDocumentNumber" Type="String(32) Null">
		<Description>Документ. Номер.</Description>
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="3ae10722-922b-4004-9bc4-4fc4a733d2cf" Name="IdentityDocumentIssuedBy" Type="String(256) Null">
		<Description>Документ. Кем выдан.</Description>
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="9bc99888-967a-48b9-90be-444c18429a25" Name="IdentityDocumentIssueDate" Type="Date Null">
		<Description>Документ. Когда выдан.</Description>
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="b55dc889-0258-42d2-bcee-9293d19902b4" Name="MDMSentDate" Type="DateTime Null">
		<Description>Дата последней отправки в MDM (НСИ), если пусто, значит отправки не было.</Description>
	</SchemePhysicalColumn>
	<SchemePrimaryKey IsSystem="true" IsPermanent="true" IsSealed="true" ID="dadfadd9-d28c-004e-5000-0900fda5a6eb" Name="pk_PnrPartnerRequests" IsClustered="true">
		<SchemeIndexedColumn Column="dadfadd9-d28c-014e-4000-0900fda5a6eb" />
	</SchemePrimaryKey>
</SchemeTable>