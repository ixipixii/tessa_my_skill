<?xml version="1.0" encoding="utf-8"?>
<SchemeTable Partition="54ed79f5-11c0-4f74-b8b0-793d596699d6" ID="0507b1f0-c020-44ac-867e-bde14fdbb2a9" Name="PnrCurrencies" Group="Pnr" InstanceType="Cards" ContentType="Entries">
	<Description>Справочник Валюты</Description>
	<SchemeComplexColumn IsSystem="true" IsPermanent="true" IsSealed="true" ID="0507b1f0-c020-00ac-2000-0de14fdbb2a9" Name="ID" Type="Reference(Typified) Not Null" ReferencedTable="1074eadd-21d7-4925-98c8-40d1e5f0ca0e">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="0507b1f0-c020-01ac-4000-0de14fdbb2a9" Name="ID" Type="Guid Not Null" ReferencedColumn="9a58123b-b2e9-4137-9c6c-5dab0ec02747" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn ID="0c8b239d-fbbb-4f07-ab97-a452ea5c4156" Name="SymbolCode" Type="String(Max) Null">
		<Description>Код символьный</Description>
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="bc90a400-e01f-4099-bd8c-5f576ad4329f" Name="Name" Type="String(Max) Null">
		<Description>Наименование валюты</Description>
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="1974a231-5e01-4d92-8004-1c5b1a2645de" Name="NumberCode" Type="String(Max) Null">
		<Description>Код числовой</Description>
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="bffbb46f-4a6a-45c9-84cc-e5652fdcd716" Name="HideOnSelection" Type="Boolean Null">
		<Description>Скрывать при выборе</Description>
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="c4504689-e2ec-4d23-b082-07db2f7f4682" Name="MDMKey" Type="String(Max) Null">
		<Description>MDM-Key</Description>
	</SchemePhysicalColumn>
	<SchemePrimaryKey IsSystem="true" IsPermanent="true" IsSealed="true" ID="0507b1f0-c020-00ac-5000-0de14fdbb2a9" Name="pk_PnrCurrencies" IsClustered="true">
		<SchemeIndexedColumn Column="0507b1f0-c020-01ac-4000-0de14fdbb2a9" />
	</SchemePrimaryKey>
</SchemeTable>