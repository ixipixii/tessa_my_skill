<?xml version="1.0" encoding="utf-8"?>
<SchemeTable Partition="54ed79f5-11c0-4f74-b8b0-793d596699d6" ID="df7dfdfc-512a-463e-85e6-5ffdda9170e6" Name="PnrOriginalsTypes" Group="Pnr" InstanceType="Cards" ContentType="Entries">
	<Description>Справочник Виды оригиналов</Description>
	<SchemeComplexColumn IsSystem="true" IsPermanent="true" IsSealed="true" ID="df7dfdfc-512a-003e-2000-0ffdda9170e6" Name="ID" Type="Reference(Typified) Not Null" ReferencedTable="1074eadd-21d7-4925-98c8-40d1e5f0ca0e">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="df7dfdfc-512a-013e-4000-0ffdda9170e6" Name="ID" Type="Guid Not Null" ReferencedColumn="9a58123b-b2e9-4137-9c6c-5dab0ec02747" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn ID="dde49959-2cf8-48cc-a237-ddc570e99536" Name="Name" Type="String(Max) Null">
		<Description>Название</Description>
	</SchemePhysicalColumn>
	<SchemePrimaryKey IsSystem="true" IsPermanent="true" IsSealed="true" ID="df7dfdfc-512a-003e-5000-0ffdda9170e6" Name="pk_PnrOriginalsTypes" IsClustered="true">
		<SchemeIndexedColumn Column="df7dfdfc-512a-013e-4000-0ffdda9170e6" />
	</SchemePrimaryKey>
</SchemeTable>