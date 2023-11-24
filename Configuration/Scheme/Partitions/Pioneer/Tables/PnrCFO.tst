<?xml version="1.0" encoding="utf-8"?>
<SchemeTable Partition="54ed79f5-11c0-4f74-b8b0-793d596699d6" ID="b5e873a7-4f25-4731-b7bf-93586f07b53a" Name="PnrCFO" Group="Pnr" InstanceType="Cards" ContentType="Entries">
	<Description>ЦФО</Description>
	<SchemeComplexColumn IsSystem="true" IsPermanent="true" IsSealed="true" ID="b5e873a7-4f25-0031-2000-03586f07b53a" Name="ID" Type="Reference(Typified) Not Null" ReferencedTable="1074eadd-21d7-4925-98c8-40d1e5f0ca0e">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="b5e873a7-4f25-0131-4000-03586f07b53a" Name="ID" Type="Guid Not Null" ReferencedColumn="9a58123b-b2e9-4137-9c6c-5dab0ec02747" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn ID="4681e48c-d5ae-4491-a3c0-1d06adbed879" Name="Code" Type="String(Max) Null">
		<Description>Код</Description>
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="20d4f2eb-ce34-4c44-87b8-8b386c283930" Name="Name" Type="String(Max) Null">
		<Description>Наименование</Description>
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="14a8ac6c-208e-417d-b246-d2b6d11bc575" Name="Type" Type="String(Max) Null">
		<Description>Вид ЦФО</Description>
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="0c243757-60ec-4a86-81b7-258d8c52110e" Name="Description" Type="String(Max) Null">
		<Description>Описание</Description>
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="8c85268d-aa83-41d2-ab9a-d5e6591db75d" Name="Used" Type="String(Max) Null">
		<Description>Используется</Description>
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="adefd874-7667-4b51-90e5-d4fae38127fe" Name="MDMKey" Type="String(Max) Null">
		<Description>MDM-Key</Description>
	</SchemePhysicalColumn>
	<SchemeComplexColumn ID="83497c6d-f066-462b-9015-2070c7078bfa" Name="ParentCFO" Type="Reference(Typified) Null" ReferencedTable="b5e873a7-4f25-4731-b7bf-93586f07b53a">
		<Description>Родительский ЦФО</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="83497c6d-f066-002b-4000-0070c7078bfa" Name="ParentCFOID" Type="Guid Null" ReferencedColumn="b5e873a7-4f25-0131-4000-03586f07b53a" />
		<SchemeReferencingColumn ID="38360063-3289-4914-83a7-9a6b5c419663" Name="ParentCFOName" Type="String(Max) Null" ReferencedColumn="20d4f2eb-ce34-4c44-87b8-8b386c283930" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn ID="64a595d0-7ff7-443f-98ac-85fe99340510" Name="IsHidden" Type="Boolean Null" />
	<SchemePrimaryKey IsSystem="true" IsPermanent="true" IsSealed="true" ID="b5e873a7-4f25-0031-5000-03586f07b53a" Name="pk_PnrCFO" IsClustered="true">
		<SchemeIndexedColumn Column="b5e873a7-4f25-0131-4000-03586f07b53a" />
	</SchemePrimaryKey>
</SchemeTable>