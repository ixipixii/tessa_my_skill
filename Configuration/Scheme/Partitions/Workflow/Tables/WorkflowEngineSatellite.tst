<?xml version="1.0" encoding="utf-8"?>
<SchemeTable Partition="dd8eeaba-9042-4fb5-9e8e-f7544463464f" ID="a0b9fc52-af7b-4ad4-9432-6425a6833ce7" Name="WorkflowEngineSatellite" Group="WorkflowEngine" InstanceType="Cards" ContentType="Entries">
	<Description>Основная секция для карточки-сателлита WorkflowEngine</Description>
	<SchemeComplexColumn IsSystem="true" IsPermanent="true" IsSealed="true" ID="a0b9fc52-af7b-00d4-2000-0425a6833ce7" Name="ID" Type="Reference(Typified) Not Null" ReferencedTable="1074eadd-21d7-4925-98c8-40d1e5f0ca0e">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="a0b9fc52-af7b-01d4-4000-0425a6833ce7" Name="ID" Type="Guid Not Null" ReferencedColumn="9a58123b-b2e9-4137-9c6c-5dab0ec02747" />
	</SchemeComplexColumn>
	<SchemeComplexColumn ID="cc56fc78-1d82-4b5f-8032-ea88872f7ca5" Name="MainCard" Type="Reference(Abstract) Not Null">
		<Description>Основная карточ</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="cc56fc78-1d82-005f-4000-0a88872f7ca5" Name="MainCardID" Type="Guid Not Null" ReferencedColumn="9a58123b-b2e9-4137-9c6c-5dab0ec02747" />
	</SchemeComplexColumn>
	<SchemePrimaryKey IsSystem="true" IsPermanent="true" IsSealed="true" ID="a0b9fc52-af7b-00d4-5000-0425a6833ce7" Name="pk_WorkflowEngineSatellite" IsClustered="true">
		<SchemeIndexedColumn Column="a0b9fc52-af7b-01d4-4000-0425a6833ce7" />
	</SchemePrimaryKey>
</SchemeTable>