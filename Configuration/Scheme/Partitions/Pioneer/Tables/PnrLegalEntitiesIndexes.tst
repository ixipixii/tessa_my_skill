<?xml version="1.0" encoding="utf-8"?>
<SchemeTable Partition="54ed79f5-11c0-4f74-b8b0-793d596699d6" ID="00e3d618-aa1e-4bac-9297-3010b710425c" Name="PnrLegalEntitiesIndexes" Group="Pnr" InstanceType="Cards" ContentType="Entries">
	<Description>Справочник Индексы юридических лиц</Description>
	<SchemeComplexColumn IsSystem="true" IsPermanent="true" IsSealed="true" ID="00e3d618-aa1e-00ac-2000-0010b710425c" Name="ID" Type="Reference(Typified) Not Null" ReferencedTable="1074eadd-21d7-4925-98c8-40d1e5f0ca0e">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="00e3d618-aa1e-01ac-4000-0010b710425c" Name="ID" Type="Guid Not Null" ReferencedColumn="9a58123b-b2e9-4137-9c6c-5dab0ec02747" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn ID="e2679580-3697-4965-b035-bb512133ac50" Name="Name" Type="String(Max) Null">
		<Description>Название</Description>
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="14c84e81-9e95-4cfa-98cf-62c786ba4bfe" Name="Idx" Type="String(Max) Null">
		<Description>Индекс</Description>
	</SchemePhysicalColumn>
	<SchemePrimaryKey IsSystem="true" IsPermanent="true" IsSealed="true" ID="00e3d618-aa1e-00ac-5000-0010b710425c" Name="pk_PnrLegalEntitiesIndexes" IsClustered="true">
		<SchemeIndexedColumn Column="00e3d618-aa1e-01ac-4000-0010b710425c" />
	</SchemePrimaryKey>
</SchemeTable>