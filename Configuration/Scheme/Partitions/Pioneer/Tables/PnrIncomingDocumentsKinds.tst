<?xml version="1.0" encoding="utf-8"?>
<SchemeTable Partition="54ed79f5-11c0-4f74-b8b0-793d596699d6" ID="d115b79e-50eb-442e-a659-9bcc6ddc8c80" Name="PnrIncomingDocumentsKinds" Group="Pnr" InstanceType="Cards" ContentType="Entries">
	<Description>Справочник Виды входящих документов</Description>
	<SchemeComplexColumn IsSystem="true" IsPermanent="true" IsSealed="true" ID="d115b79e-50eb-002e-2000-0bcc6ddc8c80" Name="ID" Type="Reference(Typified) Not Null" ReferencedTable="1074eadd-21d7-4925-98c8-40d1e5f0ca0e">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="d115b79e-50eb-012e-4000-0bcc6ddc8c80" Name="ID" Type="Guid Not Null" ReferencedColumn="9a58123b-b2e9-4137-9c6c-5dab0ec02747" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn ID="1988a27c-81d8-4d96-812b-dda2afbb36ed" Name="Name" Type="String(Max) Null">
		<Description>Название</Description>
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="cc2d39e5-6ea9-4dbf-a7ef-b4070e2f1fdc" Name="Idx" Type="String(Max) Null">
		<Description>Индекс</Description>
	</SchemePhysicalColumn>
	<SchemePrimaryKey IsSystem="true" IsPermanent="true" IsSealed="true" ID="d115b79e-50eb-002e-5000-0bcc6ddc8c80" Name="pk_PnrIncomingDocumentsKinds" IsClustered="true">
		<SchemeIndexedColumn Column="d115b79e-50eb-012e-4000-0bcc6ddc8c80" />
	</SchemePrimaryKey>
</SchemeTable>