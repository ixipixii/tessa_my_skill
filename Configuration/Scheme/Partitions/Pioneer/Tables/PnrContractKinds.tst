<?xml version="1.0" encoding="utf-8"?>
<SchemeTable Partition="54ed79f5-11c0-4f74-b8b0-793d596699d6" ID="4e9d936c-89c0-470d-be69-b4ce3e0aa294" Name="PnrContractKinds" Group="Pnr" InstanceType="Cards" ContentType="Entries">
	<Description>Справочник Виды договора</Description>
	<SchemeComplexColumn IsSystem="true" IsPermanent="true" IsSealed="true" ID="4e9d936c-89c0-000d-2000-04ce3e0aa294" Name="ID" Type="Reference(Typified) Not Null" ReferencedTable="1074eadd-21d7-4925-98c8-40d1e5f0ca0e">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="4e9d936c-89c0-010d-4000-04ce3e0aa294" Name="ID" Type="Guid Not Null" ReferencedColumn="9a58123b-b2e9-4137-9c6c-5dab0ec02747" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn ID="86c831ea-880e-4fef-ab09-59bc9c36c392" Name="Name" Type="String(128) Not Null">
		<Description>Название</Description>
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="241f704e-8590-4861-a3a3-e19095a824b1" Name="MDMName" Type="String(128) Null">
		<Description>Название во внешней системе для интеграции</Description>
	</SchemePhysicalColumn>
	<SchemePrimaryKey IsSystem="true" IsPermanent="true" IsSealed="true" ID="4e9d936c-89c0-000d-5000-04ce3e0aa294" Name="pk_PnrContractKinds" IsClustered="true">
		<SchemeIndexedColumn Column="4e9d936c-89c0-010d-4000-04ce3e0aa294" />
	</SchemePrimaryKey>
</SchemeTable>