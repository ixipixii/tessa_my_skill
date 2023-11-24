<?xml version="1.0" encoding="utf-8"?>
<SchemeTable ID="5d47ef13-b6f4-47ef-9815-3b3d0e6d475a" Partition="d1b372f3-7565-4309-9037-5e5a0969d94e">
	<SchemePhysicalColumn Partition="54ed79f5-11c0-4f74-b8b0-793d596699d6" ID="5834c93b-0662-4522-8d86-10b632b8b5db" Name="DateCreation" Type="Date Null">
		<Description>Дата создания</Description>
	</SchemePhysicalColumn>
	<SchemePhysicalColumn Partition="54ed79f5-11c0-4f74-b8b0-793d596699d6" ID="4ab34ead-af3f-4036-8b26-cff56064a24f" Name="Validity" Type="Date Null">
		<Description>Срок действия</Description>
	</SchemePhysicalColumn>
	<SchemePhysicalColumn Partition="54ed79f5-11c0-4f74-b8b0-793d596699d6" ID="9fbae09b-c39a-41aa-bf43-62fd9671428e" Name="DateApproval" Type="Date Null">
		<Description>Дата одобрения СБ</Description>
	</SchemePhysicalColumn>
	<SchemePhysicalColumn Partition="54ed79f5-11c0-4f74-b8b0-793d596699d6" ID="a1c0a79a-c1a3-4487-a86a-35e5332a4d12" Name="CreationRequests" Type="String(Max) Null">
		<Description>Заявки на создание</Description>
	</SchemePhysicalColumn>
	<SchemePhysicalColumn Partition="54ed79f5-11c0-4f74-b8b0-793d596699d6" ID="8d50272e-fc71-4e7b-8567-92dbe2058567" Name="CoordinationRequests" Type="String(Max) Null">
		<Description>Заявки на согласование</Description>
	</SchemePhysicalColumn>
	<SchemePhysicalColumn Partition="54ed79f5-11c0-4f74-b8b0-793d596699d6" ID="efc37b7a-97a7-4dcf-a529-f3a513a94df8" Name="MDMKey" Type="String(Max) Null">
		<Description>MDM-key</Description>
	</SchemePhysicalColumn>
	<SchemeComplexColumn Partition="54ed79f5-11c0-4f74-b8b0-793d596699d6" ID="b4471626-8efb-48c3-abff-785f42ef2b87" Name="Direction" Type="Reference(Typified) Null" ReferencedTable="14cf5907-5876-4b3e-8470-41fd6c9abbcf">
		<Description>Направление</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="b4471626-8efb-00c3-4000-085f42ef2b87" Name="DirectionID" Type="Guid Null" ReferencedColumn="14cf5907-5876-013e-4000-01fd6c9abbcf" />
		<SchemeReferencingColumn ID="bc7c3d30-1911-41e9-a0a8-a99b7992a01f" Name="DirectionName" Type="String(Max) Null" ReferencedColumn="dce07234-3902-4b2d-a5ea-ef4c0ae3fea2" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn Partition="54ed79f5-11c0-4f74-b8b0-793d596699d6" ID="152c2ac5-fab9-4fd7-bd27-22008bf03874" Name="NonResident" Type="Boolean Not Null">
		<Description>Нерезидент</Description>
		<SchemeDefaultConstraint IsPermanent="true" ID="99f0a2ca-80a2-4958-87ef-1c19d35f3ab3" Name="df_Partners_NonResident" Value="false" />
	</SchemePhysicalColumn>
	<SchemePhysicalColumn Partition="54ed79f5-11c0-4f74-b8b0-793d596699d6" ID="17a12aaf-1fd4-4266-8f65-48ac382d71cf" Name="IdentityDocument" Type="String(256) Null">
		<Description>Документ, удостоверяющий личность</Description>
	</SchemePhysicalColumn>
	<SchemePhysicalColumn Partition="54ed79f5-11c0-4f74-b8b0-793d596699d6" ID="5cbd954a-d9b6-4194-9722-b034666d6955" Name="IdentityDocumentKind" Type="String(128) Null">
		<Description>Документ. Вид документа.</Description>
	</SchemePhysicalColumn>
	<SchemePhysicalColumn Partition="54ed79f5-11c0-4f74-b8b0-793d596699d6" ID="4bd077ee-becc-432f-a7dc-80f859ace832" Name="IdentityDocumentSeries" Type="String(32) Null">
		<Description>Документ. Серия.</Description>
	</SchemePhysicalColumn>
	<SchemePhysicalColumn Partition="54ed79f5-11c0-4f74-b8b0-793d596699d6" ID="b66c1c17-0ec1-4832-83fb-6fcc3978bdc8" Name="IdentityDocumentNumber" Type="String(32) Null">
		<Description>Документ. Номер.</Description>
	</SchemePhysicalColumn>
	<SchemePhysicalColumn Partition="54ed79f5-11c0-4f74-b8b0-793d596699d6" ID="ce6f91ed-a7ed-47ec-a32c-05ec4f54db5a" Name="IdentityDocumentIssuedBy" Type="String(256) Null">
		<Description>Документ. Кем выдан.</Description>
	</SchemePhysicalColumn>
	<SchemePhysicalColumn Partition="54ed79f5-11c0-4f74-b8b0-793d596699d6" ID="1ee3ba91-2c89-4337-a33f-0a5d5ac7c1dd" Name="IdentityDocumentIssueDate" Type="Date Null">
		<Description>Документ. Когда выдан.</Description>
	</SchemePhysicalColumn>
	<SchemeComplexColumn Partition="54ed79f5-11c0-4f74-b8b0-793d596699d6" ID="f62a3c97-aee2-403b-9a85-75d60189ece6" Name="CountryRegistration" Type="Reference(Typified) Null" ReferencedTable="ba073bea-da30-46b2-badd-98ebb9e3c1ac">
		<Description>Страна регистрации</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="f62a3c97-aee2-003b-4000-05d60189ece6" Name="CountryRegistrationID" Type="Guid Null" ReferencedColumn="ba073bea-da30-01b2-4000-08ebb9e3c1ac" />
		<SchemeReferencingColumn ID="c75bab74-9c58-4c02-b126-c5e72dc0c4d8" Name="CountryRegistrationName" Type="String(Max) Null" ReferencedColumn="fc3e5211-75f1-4b9a-8585-980fbb64df82" />
	</SchemeComplexColumn>
	<SchemeComplexColumn Partition="54ed79f5-11c0-4f74-b8b0-793d596699d6" ID="4d1768bb-44c2-426c-9809-2af571da2c00" Name="Status" Type="Reference(Typified) Null" ReferencedTable="65cd42e4-d0f7-4bab-a256-45cfec7b286a">
		<Description>Статус</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="4d1768bb-44c2-006c-4000-0af571da2c00" Name="StatusID" Type="Int16 Null" ReferencedColumn="29458ef8-7c58-4f9f-8f3b-a2098f54fd13" />
		<SchemeReferencingColumn ID="9f63c79d-8d77-4091-a0f2-ae01bc2d93af" Name="StatusName" Type="String(100) Null" ReferencedColumn="fb59bbdc-5ec5-4fbf-ab31-19756af8b406" />
	</SchemeComplexColumn>
	<SchemeComplexColumn Partition="54ed79f5-11c0-4f74-b8b0-793d596699d6" ID="8eb5ad1f-4917-4dc9-9237-ca97df036784" Name="SpecialSign" Type="Reference(Typified) Null" ReferencedTable="f3fe8747-5d52-4f09-b34a-2cc0309565a6">
		<Description>Особый признак контрагента</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="8eb5ad1f-4917-00c9-4000-0a97df036784" Name="SpecialSignID" Type="Int16 Null" ReferencedColumn="05cf3057-a85c-461c-91db-985517c720ac" />
		<SchemeReferencingColumn ID="32a2a234-c386-40e2-9fa5-ab7e75e9d405" Name="SpecialSignName" Type="String(Max) Null" ReferencedColumn="d4304713-0b40-4aa0-8a00-46a059bc395c" />
	</SchemeComplexColumn>
</SchemeTable>