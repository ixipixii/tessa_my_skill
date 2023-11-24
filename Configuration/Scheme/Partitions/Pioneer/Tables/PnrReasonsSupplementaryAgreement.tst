<?xml version="1.0" encoding="utf-8"?>
<SchemeTable Partition="54ed79f5-11c0-4f74-b8b0-793d596699d6" ID="20cc279f-9ad6-4d8d-b1d3-9cb4150d1222" Name="PnrReasonsSupplementaryAgreement" Group="Pnr" InstanceType="Cards" ContentType="Entries">
	<Description>Справочник Причина заключения Дополнительного Соглашения</Description>
	<SchemeComplexColumn IsSystem="true" IsPermanent="true" IsSealed="true" ID="20cc279f-9ad6-008d-2000-0cb4150d1222" Name="ID" Type="Reference(Typified) Not Null" ReferencedTable="1074eadd-21d7-4925-98c8-40d1e5f0ca0e">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="20cc279f-9ad6-018d-4000-0cb4150d1222" Name="ID" Type="Guid Not Null" ReferencedColumn="9a58123b-b2e9-4137-9c6c-5dab0ec02747" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn ID="03ac4acf-77bd-4b69-8c86-a31e8e476af4" Name="Name" Type="String(Max) Null">
		<Description>Наименование</Description>
	</SchemePhysicalColumn>
	<SchemePrimaryKey IsSystem="true" IsPermanent="true" IsSealed="true" ID="20cc279f-9ad6-008d-5000-0cb4150d1222" Name="pk_PnrReasonsSupplementaryAgreement" IsClustered="true">
		<SchemeIndexedColumn Column="20cc279f-9ad6-018d-4000-0cb4150d1222" />
	</SchemePrimaryKey>
</SchemeTable>