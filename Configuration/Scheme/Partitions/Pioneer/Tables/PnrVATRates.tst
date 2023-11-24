<?xml version="1.0" encoding="utf-8"?>
<SchemeTable Partition="54ed79f5-11c0-4f74-b8b0-793d596699d6" ID="016fa9e6-6a00-4568-a35c-751b2910363f" Name="PnrVATRates" Group="Pnr">
	<Description>Справочник Ставки НДС</Description>
	<SchemePhysicalColumn ID="6929a82d-3f57-4145-a59c-fe6d2499891c" Name="ID" Type="Int16 Not Null" />
	<SchemePhysicalColumn ID="832900f8-35cc-4a23-bf8d-fdd084624a48" Name="Value" Type="String(Max) Null">
		<Description>значение НДС в %</Description>
	</SchemePhysicalColumn>
	<SchemePrimaryKey ID="c2a1d32b-0c2d-432f-bd29-1dad91c8dd50" Name="pk_PnrVATRates" IsClustered="true">
		<SchemeIndexedColumn Column="6929a82d-3f57-4145-a59c-fe6d2499891c" />
	</SchemePrimaryKey>
	<SchemeRecord>
		<ID ID="6929a82d-3f57-4145-a59c-fe6d2499891c">1</ID>
		<Value ID="832900f8-35cc-4a23-bf8d-fdd084624a48">0%</Value>
	</SchemeRecord>
	<SchemeRecord>
		<ID ID="6929a82d-3f57-4145-a59c-fe6d2499891c">2</ID>
		<Value ID="832900f8-35cc-4a23-bf8d-fdd084624a48">10%</Value>
	</SchemeRecord>
	<SchemeRecord>
		<ID ID="6929a82d-3f57-4145-a59c-fe6d2499891c">3</ID>
		<Value ID="832900f8-35cc-4a23-bf8d-fdd084624a48">18%</Value>
	</SchemeRecord>
	<SchemeRecord>
		<ID ID="6929a82d-3f57-4145-a59c-fe6d2499891c">4</ID>
		<Value ID="832900f8-35cc-4a23-bf8d-fdd084624a48">20%</Value>
	</SchemeRecord>
	<SchemeRecord>
		<ID ID="6929a82d-3f57-4145-a59c-fe6d2499891c">0</ID>
		<Value ID="832900f8-35cc-4a23-bf8d-fdd084624a48">Без НДС</Value>
	</SchemeRecord>
</SchemeTable>