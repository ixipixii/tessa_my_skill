<?xml version="1.0" encoding="utf-8"?>
<SchemeTable Partition="54ed79f5-11c0-4f74-b8b0-793d596699d6" ID="75aa0d06-9bff-4e01-96b6-2ca62af269ae" Name="PnrCities" Group="Pnr" InstanceType="Cards" ContentType="Entries">
	<Description>Справочник Города</Description>
	<SchemeComplexColumn IsSystem="true" IsPermanent="true" IsSealed="true" ID="75aa0d06-9bff-0001-2000-0ca62af269ae" Name="ID" Type="Reference(Typified) Not Null" ReferencedTable="1074eadd-21d7-4925-98c8-40d1e5f0ca0e">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="75aa0d06-9bff-0101-4000-0ca62af269ae" Name="ID" Type="Guid Not Null" ReferencedColumn="9a58123b-b2e9-4137-9c6c-5dab0ec02747" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn ID="e70abb74-73d9-4232-ac52-a8ff12c357f5" Name="Name" Type="String(Max) Null">
		<Description>Название</Description>
	</SchemePhysicalColumn>
	<SchemePrimaryKey IsSystem="true" IsPermanent="true" IsSealed="true" ID="75aa0d06-9bff-0001-5000-0ca62af269ae" Name="pk_PnrCities" IsClustered="true">
		<SchemeIndexedColumn Column="75aa0d06-9bff-0101-4000-0ca62af269ae" />
	</SchemePrimaryKey>
</SchemeTable>