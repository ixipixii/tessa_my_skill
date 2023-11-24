<?xml version="1.0" encoding="utf-8"?>
<SchemeTable Partition="54ed79f5-11c0-4f74-b8b0-793d596699d6" ID="451af913-aadf-456d-9fb1-a1b2f9b58197" Name="PnrOutgoingDocumentsKindsUK" Group="Pnr" InstanceType="Cards" ContentType="Entries">
	<SchemeComplexColumn IsSystem="true" IsPermanent="true" IsSealed="true" ID="451af913-aadf-006d-2000-01b2f9b58197" Name="ID" Type="Reference(Typified) Not Null" ReferencedTable="1074eadd-21d7-4925-98c8-40d1e5f0ca0e">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="451af913-aadf-016d-4000-01b2f9b58197" Name="ID" Type="Guid Not Null" ReferencedColumn="9a58123b-b2e9-4137-9c6c-5dab0ec02747" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn ID="8edb9891-4aeb-40e6-bf41-e00ee087f07f" Name="Name" Type="String(Max) Null">
		<Description>Название</Description>
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="816df957-c7b2-4863-80e7-d3f472b58446" Name="Idx" Type="String(Max) Null">
		<Description>Индекс</Description>
	</SchemePhysicalColumn>
	<SchemePrimaryKey IsSystem="true" IsPermanent="true" IsSealed="true" ID="451af913-aadf-006d-5000-01b2f9b58197" Name="pk_PnrOutgoingDocumentsKindsUK" IsClustered="true">
		<SchemeIndexedColumn Column="451af913-aadf-016d-4000-01b2f9b58197" />
	</SchemePrimaryKey>
</SchemeTable>