<?xml version="1.0" encoding="utf-8"?>
<SchemeTable Partition="54ed79f5-11c0-4f74-b8b0-793d596699d6" ID="9c841787-0ad6-4239-a80d-37b931ad8b76" Name="PnrContractForms" Group="Pnr" InstanceType="Cards" ContentType="Entries">
	<Description>Справочник Формы договора</Description>
	<SchemeComplexColumn IsSystem="true" IsPermanent="true" IsSealed="true" ID="9c841787-0ad6-0039-2000-07b931ad8b76" Name="ID" Type="Reference(Typified) Not Null" ReferencedTable="1074eadd-21d7-4925-98c8-40d1e5f0ca0e">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="9c841787-0ad6-0139-4000-07b931ad8b76" Name="ID" Type="Guid Not Null" ReferencedColumn="9a58123b-b2e9-4137-9c6c-5dab0ec02747" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn ID="4546eb18-4631-4482-b663-74ad25c48ab8" Name="Name" Type="String(Max) Null">
		<Description>Название</Description>
	</SchemePhysicalColumn>
	<SchemePrimaryKey IsSystem="true" IsPermanent="true" IsSealed="true" ID="9c841787-0ad6-0039-5000-07b931ad8b76" Name="pk_PnrContractForms" IsClustered="true">
		<SchemeIndexedColumn Column="9c841787-0ad6-0139-4000-07b931ad8b76" />
	</SchemePrimaryKey>
</SchemeTable>