<?xml version="1.0" encoding="utf-8"?>
<SchemeTable Partition="54ed79f5-11c0-4f74-b8b0-793d596699d6" ID="1c027022-e5e3-4a6c-a5a8-f625445e1d86" Name="PnrOrdersTypes" Group="Pnr" InstanceType="Cards" ContentType="Entries">
	<Description>Справочник Виды приказов</Description>
	<SchemeComplexColumn IsSystem="true" IsPermanent="true" IsSealed="true" ID="1c027022-e5e3-006c-2000-0625445e1d86" Name="ID" Type="Reference(Typified) Not Null" ReferencedTable="1074eadd-21d7-4925-98c8-40d1e5f0ca0e">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="1c027022-e5e3-016c-4000-0625445e1d86" Name="ID" Type="Guid Not Null" ReferencedColumn="9a58123b-b2e9-4137-9c6c-5dab0ec02747" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn ID="9f394ab0-7600-4476-aeba-c06cf1c250be" Name="Name" Type="String(Max) Null">
		<Description>Название</Description>
	</SchemePhysicalColumn>
	<SchemePrimaryKey IsSystem="true" IsPermanent="true" IsSealed="true" ID="1c027022-e5e3-006c-5000-0625445e1d86" Name="pk_PnrOrdersTypes" IsClustered="true">
		<SchemeIndexedColumn Column="1c027022-e5e3-016c-4000-0625445e1d86" />
	</SchemePrimaryKey>
</SchemeTable>