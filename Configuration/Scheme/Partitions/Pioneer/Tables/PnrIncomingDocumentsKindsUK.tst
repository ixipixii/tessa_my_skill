<?xml version="1.0" encoding="utf-8"?>
<SchemeTable Partition="54ed79f5-11c0-4f74-b8b0-793d596699d6" ID="49fe89ac-890e-461e-bb09-29c6cbfb5774" Name="PnrIncomingDocumentsKindsUK" Group="Pnr" InstanceType="Cards" ContentType="Entries">
	<Description>Справочник Виды входящих документов УК ПС</Description>
	<SchemeComplexColumn IsSystem="true" IsPermanent="true" IsSealed="true" ID="49fe89ac-890e-001e-2000-09c6cbfb5774" Name="ID" Type="Reference(Typified) Not Null" ReferencedTable="1074eadd-21d7-4925-98c8-40d1e5f0ca0e">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="49fe89ac-890e-011e-4000-09c6cbfb5774" Name="ID" Type="Guid Not Null" ReferencedColumn="9a58123b-b2e9-4137-9c6c-5dab0ec02747" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn ID="6e5369cd-220b-4b88-a0a3-61e50b984612" Name="Name" Type="String(Max) Null">
		<Description>Название</Description>
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="a03a7f49-19bb-4801-a86d-f23b844734bd" Name="Idx" Type="String(Max) Null">
		<Description>Индекс</Description>
	</SchemePhysicalColumn>
	<SchemePrimaryKey IsSystem="true" IsPermanent="true" IsSealed="true" ID="49fe89ac-890e-001e-5000-09c6cbfb5774" Name="pk_PnrIncomingDocumentsKindsUK" IsClustered="true">
		<SchemeIndexedColumn Column="49fe89ac-890e-011e-4000-09c6cbfb5774" />
	</SchemePrimaryKey>
</SchemeTable>