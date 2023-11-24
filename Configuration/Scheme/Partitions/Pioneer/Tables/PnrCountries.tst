<?xml version="1.0" encoding="utf-8"?>
<SchemeTable Partition="54ed79f5-11c0-4f74-b8b0-793d596699d6" ID="ba073bea-da30-46b2-badd-98ebb9e3c1ac" Name="PnrCountries" Group="Pnr" InstanceType="Cards" ContentType="Entries">
	<Description>Справочник стран</Description>
	<SchemeComplexColumn IsSystem="true" IsPermanent="true" IsSealed="true" ID="ba073bea-da30-00b2-2000-08ebb9e3c1ac" Name="ID" Type="Reference(Typified) Not Null" ReferencedTable="1074eadd-21d7-4925-98c8-40d1e5f0ca0e">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="ba073bea-da30-01b2-4000-08ebb9e3c1ac" Name="ID" Type="Guid Not Null" ReferencedColumn="9a58123b-b2e9-4137-9c6c-5dab0ec02747" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn ID="fc3e5211-75f1-4b9a-8585-980fbb64df82" Name="Name" Type="String(Max) Null">
		<Description>Название</Description>
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="6a0be8e0-6e60-43fa-a304-415a975c092b" Name="Identifier" Type="String(Max) Null">
		<Description>Идентификатор</Description>
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="aae8ec09-899c-4497-af6b-2728d68b776a" Name="MDMKey" Type="String(36) Null">
		<Description>MDM-Key</Description>
	</SchemePhysicalColumn>
	<SchemePrimaryKey IsSystem="true" IsPermanent="true" IsSealed="true" ID="ba073bea-da30-00b2-5000-08ebb9e3c1ac" Name="pk_PnrCountries" IsClustered="true">
		<SchemeIndexedColumn Column="ba073bea-da30-01b2-4000-08ebb9e3c1ac" />
	</SchemePrimaryKey>
</SchemeTable>