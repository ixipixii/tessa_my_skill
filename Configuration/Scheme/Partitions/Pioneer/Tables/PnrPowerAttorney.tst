<?xml version="1.0" encoding="utf-8"?>
<SchemeTable Partition="54ed79f5-11c0-4f74-b8b0-793d596699d6" ID="67643700-2f43-4703-a5cd-1d81ae4f207c" Name="PnrPowerAttorney" Group="Pnr" InstanceType="Cards" ContentType="Entries">
	<Description>Доверенности</Description>
	<SchemeComplexColumn IsSystem="true" IsPermanent="true" IsSealed="true" ID="67643700-2f43-0003-2000-0d81ae4f207c" Name="ID" Type="Reference(Typified) Not Null" ReferencedTable="1074eadd-21d7-4925-98c8-40d1e5f0ca0e">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="67643700-2f43-0103-4000-0d81ae4f207c" Name="ID" Type="Guid Not Null" ReferencedColumn="9a58123b-b2e9-4137-9c6c-5dab0ec02747" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn ID="59fb61a9-2927-4a2a-8f36-72040558642f" Name="ProjectDate" Type="Date Null">
		<Description>Дата проекта</Description>
	</SchemePhysicalColumn>
	<SchemeComplexColumn ID="c9aedac1-dd55-41fd-b2cf-b28e2edfcc42" Name="Organization" Type="Reference(Typified) Null" ReferencedTable="303dd35d-b998-49b3-a10b-ed30d31bd36a">
		<Description>Организация ГК Пионер</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="c9aedac1-dd55-00fd-4000-028e2edfcc42" Name="OrganizationID" Type="Guid Null" ReferencedColumn="303dd35d-b998-01b3-4000-0d30d31bd36a" />
		<SchemeReferencingColumn ID="01594498-a650-4f54-bff4-92ce292c2a59" Name="OrganizationName" Type="String(Max) Null" ReferencedColumn="d8ecabaa-f192-4546-9af6-1f9712daabdb" />
	</SchemeComplexColumn>
	<SchemeComplexColumn ID="b535332e-d8ab-46ee-99f3-a9ab9710ee65" Name="Confidant" Type="Reference(Typified) Null" ReferencedTable="6c977939-bbfc-456f-a133-f1c2244e3cc3">
		<Description>Доверенное лицо</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="b535332e-d8ab-00ee-4000-09ab9710ee65" Name="ConfidantID" Type="Guid Null" ReferencedColumn="6c977939-bbfc-016f-4000-01c2244e3cc3" />
		<SchemeReferencingColumn ID="9ddb747b-2720-4642-bd2f-eaaf3e5bb35f" Name="ConfidantName" Type="String(128) Null" ReferencedColumn="1782f76a-4743-4aa4-920c-7edaee860964" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn ID="97909eae-1142-4ddf-8630-e16425d1ae44" Name="Credentials" Type="String(Max) Null">
		<Description>Полномочия по доверенности</Description>
	</SchemePhysicalColumn>
	<SchemeComplexColumn ID="b92320b7-5b19-44c7-aa57-d30e6a2be457" Name="Destination" Type="Reference(Typified) Null" ReferencedTable="5d47ef13-b6f4-47ef-9815-3b3d0e6d475a">
		<Description>Адресат (организация)</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="b92320b7-5b19-00c7-4000-030e6a2be457" Name="DestinationID" Type="Guid Null" ReferencedColumn="5d47ef13-b6f4-01ef-4000-0b3d0e6d475a" />
		<SchemeReferencingColumn ID="c3856a95-c5b4-4c42-a919-f3972c3c00f3" Name="DestinationName" Type="String(255) Null" ReferencedColumn="f1c960e0-951e-4837-8474-bb61d98f40f0" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn ID="ffd5f06f-ceb7-4c13-a955-4577b0863791" Name="StartDate" Type="Date Null">
		<Description>Дата начала</Description>
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="d685197d-87ce-43bb-b17c-88a175456435" Name="EndDate" Type="Date Null">
		<Description>Дата окончания</Description>
	</SchemePhysicalColumn>
	<SchemeComplexColumn ID="86bee320-6383-45f9-80a6-1069e4ef8753" Name="Employee" Type="Reference(Typified) Null" ReferencedTable="c77707ce-b8de-4d47-8d55-08c315491728">
		<Description>Сотрудник ГК Пионер</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="86bee320-6383-00f9-4000-0069e4ef8753" Name="EmployeeID" Type="Int16 Null" ReferencedColumn="2cd1796f-0b93-4e5b-b0e4-b0bc91d20370" />
		<SchemeReferencingColumn ID="c1102115-34a1-4bd7-b809-ee033034d167" Name="EmployeeName" Type="String(Max) Null" ReferencedColumn="d3a53179-d767-4989-a760-2e085122b5a7" />
	</SchemeComplexColumn>
	<SchemeComplexColumn ID="c8a985f7-f049-408f-a5c0-cf0236323bdf" Name="Type" Type="Reference(Typified) Null" ReferencedTable="55ecf577-db07-4c32-af7b-0acb48a2b769">
		<Description>Тип доверенности</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="c8a985f7-f049-008f-4000-0f0236323bdf" Name="TypeID" Type="Guid Null" ReferencedColumn="55ecf577-db07-0132-4000-0acb48a2b769" />
		<SchemeReferencingColumn ID="d3701095-9289-4a3f-b82e-ff5e6334d4dc" Name="TypeName" Type="String(Max) Null" ReferencedColumn="21aa8f6f-14bc-4567-992d-d90974e559c2" />
	</SchemeComplexColumn>
	<SchemeComplexColumn ID="742de78c-7f6f-4de2-8d92-9ec2e7abb3d5" Name="WriteOff" Type="Reference(Typified) Null" ReferencedTable="81f6010b-9641-4aa5-8897-b8e8603fbf4b">
		<Description>Списать в дело</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="742de78c-7f6f-00e2-4000-0ec2e7abb3d5" Name="WriteOffID" Type="Guid Null" ReferencedColumn="81f6010b-9641-01a5-4000-08e8603fbf4b" />
		<SchemeReferencingColumn ID="2ccc41e1-6d22-41cf-990d-d3de2e828b31" Name="WriteOffName" Type="String(128) Null" ReferencedColumn="616d6b2e-35d5-424d-846b-618eb25962d0" />
		<SchemeReferencingColumn ID="dea56ec8-59e7-4c4f-a6dc-d99584c7a7da" Name="WriteOffIdx" Type="String(Max) Null" ReferencedColumn="1f7ca0d4-1b64-43f8-b973-d14791af51a0" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn ID="7166b19a-4f38-452f-bde6-d527655713ca" Name="ConfidantNotEmployee" Type="String(Max) Null">
		<Description>Доверенное лицо не сотрудник компании</Description>
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="c2bb8620-f565-45f3-9b63-abe482d2a7d4" Name="ExtID" Type="Guid Null">
		<Description>GUID карточки из системы заказчика</Description>
	</SchemePhysicalColumn>
	<SchemePrimaryKey IsSystem="true" IsPermanent="true" IsSealed="true" ID="67643700-2f43-0003-5000-0d81ae4f207c" Name="pk_PnrPowerAttorney" IsClustered="true">
		<SchemeIndexedColumn Column="67643700-2f43-0103-4000-0d81ae4f207c" />
	</SchemePrimaryKey>
</SchemeTable>