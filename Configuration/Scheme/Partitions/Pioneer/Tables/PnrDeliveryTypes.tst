<?xml version="1.0" encoding="utf-8"?>
<SchemeTable Partition="54ed79f5-11c0-4f74-b8b0-793d596699d6" ID="e84635a0-4790-4b3b-b7d4-7bd4472cef35" Name="PnrDeliveryTypes" Group="Pnr" InstanceType="Cards" ContentType="Entries">
	<Description>Справочник Типы доставки</Description>
	<SchemeComplexColumn IsSystem="true" IsPermanent="true" IsSealed="true" ID="e84635a0-4790-003b-2000-0bd4472cef35" Name="ID" Type="Reference(Typified) Not Null" ReferencedTable="1074eadd-21d7-4925-98c8-40d1e5f0ca0e">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="e84635a0-4790-013b-4000-0bd4472cef35" Name="ID" Type="Guid Not Null" ReferencedColumn="9a58123b-b2e9-4137-9c6c-5dab0ec02747" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn ID="4345bc79-699c-4a2a-8c8f-e5ff9165203e" Name="Name" Type="String(Max) Null">
		<Description>Название</Description>
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="c413a143-d091-469c-a1b0-cfeb3b54f6fb" Name="Description" Type="String(Max) Null">
		<Description>Описание</Description>
	</SchemePhysicalColumn>
	<SchemePrimaryKey IsSystem="true" IsPermanent="true" IsSealed="true" ID="e84635a0-4790-003b-5000-0bd4472cef35" Name="pk_PnrDeliveryTypes" IsClustered="true">
		<SchemeIndexedColumn Column="e84635a0-4790-013b-4000-0bd4472cef35" />
	</SchemePrimaryKey>
</SchemeTable>