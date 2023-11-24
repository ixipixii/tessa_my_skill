<?xml version="1.0" encoding="utf-8"?>
<SchemeTable Partition="54ed79f5-11c0-4f74-b8b0-793d596699d6" ID="a769bbbe-73f6-42eb-aa6b-b0ff72e07845" Name="PnrDocumentKinds" Group="Pnr" InstanceType="Cards" ContentType="Entries">
	<Description>Справочник Виды документа</Description>
	<SchemeComplexColumn IsSystem="true" IsPermanent="true" IsSealed="true" ID="a769bbbe-73f6-00eb-2000-00ff72e07845" Name="ID" Type="Reference(Typified) Not Null" ReferencedTable="1074eadd-21d7-4925-98c8-40d1e5f0ca0e">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="a769bbbe-73f6-01eb-4000-00ff72e07845" Name="ID" Type="Guid Not Null" ReferencedColumn="9a58123b-b2e9-4137-9c6c-5dab0ec02747" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn ID="dfe455ca-273a-47cc-a9c1-cc620f0136ac" Name="Name" Type="String(Max) Null">
		<Description>Название</Description>
	</SchemePhysicalColumn>
	<SchemePrimaryKey IsSystem="true" IsPermanent="true" IsSealed="true" ID="a769bbbe-73f6-00eb-5000-00ff72e07845" Name="pk_PnrDocumentKinds" IsClustered="true">
		<SchemeIndexedColumn Column="a769bbbe-73f6-01eb-4000-00ff72e07845" />
	</SchemePrimaryKey>
</SchemeTable>