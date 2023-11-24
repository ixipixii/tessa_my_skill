<?xml version="1.0" encoding="utf-8"?>
<SchemeTable Partition="54ed79f5-11c0-4f74-b8b0-793d596699d6" ID="c341867a-e7c4-4176-9306-3a0abbb56e3c" Name="PnrContractKinds1C" Group="Pnr" InstanceType="Cards" ContentType="Entries">
	<Description>Справочник Виды договора 1С</Description>
	<SchemeComplexColumn IsSystem="true" IsPermanent="true" IsSealed="true" ID="c341867a-e7c4-0076-2000-0a0abbb56e3c" Name="ID" Type="Reference(Typified) Not Null" ReferencedTable="1074eadd-21d7-4925-98c8-40d1e5f0ca0e">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="c341867a-e7c4-0176-4000-0a0abbb56e3c" Name="ID" Type="Guid Not Null" ReferencedColumn="9a58123b-b2e9-4137-9c6c-5dab0ec02747" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn ID="b9fedf5a-b596-4aa4-b188-19f7c545a982" Name="Name" Type="String(Max) Null">
		<Description>Название</Description>
	</SchemePhysicalColumn>
	<SchemePrimaryKey IsSystem="true" IsPermanent="true" IsSealed="true" ID="c341867a-e7c4-0076-5000-0a0abbb56e3c" Name="pk_PnrContractKinds1C" IsClustered="true">
		<SchemeIndexedColumn Column="c341867a-e7c4-0176-4000-0a0abbb56e3c" />
	</SchemePrimaryKey>
</SchemeTable>