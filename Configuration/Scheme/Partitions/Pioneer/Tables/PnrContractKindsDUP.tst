<?xml version="1.0" encoding="utf-8"?>
<SchemeTable Partition="54ed79f5-11c0-4f74-b8b0-793d596699d6" ID="3537eeca-1c13-4cc8-ad6d-872b6f1b52ca" Name="PnrContractKindsDUP" Group="Pnr" InstanceType="Cards" ContentType="Entries">
	<Description>Справочник Виды договора (ДУП)</Description>
	<SchemeComplexColumn IsSystem="true" IsPermanent="true" IsSealed="true" ID="3537eeca-1c13-00c8-2000-072b6f1b52ca" Name="ID" Type="Reference(Typified) Not Null" ReferencedTable="1074eadd-21d7-4925-98c8-40d1e5f0ca0e">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="3537eeca-1c13-01c8-4000-072b6f1b52ca" Name="ID" Type="Guid Not Null" ReferencedColumn="9a58123b-b2e9-4137-9c6c-5dab0ec02747" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn ID="c81e197b-ef24-4565-9597-516257c7eb0a" Name="Name" Type="String(Max) Null">
		<Description>Название</Description>
	</SchemePhysicalColumn>
	<SchemePrimaryKey IsSystem="true" IsPermanent="true" IsSealed="true" ID="3537eeca-1c13-00c8-5000-072b6f1b52ca" Name="pk_PnrContractKindsDUP" IsClustered="true">
		<SchemeIndexedColumn Column="3537eeca-1c13-01c8-4000-072b6f1b52ca" />
	</SchemePrimaryKey>
</SchemeTable>