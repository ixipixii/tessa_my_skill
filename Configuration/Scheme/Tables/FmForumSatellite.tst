<?xml version="1.0" encoding="utf-8"?>
<SchemeTable ID="82f8e183-6132-421d-a5e7-656c791b749f" Name="FmForumSatellite" Group="Fm" InstanceType="Cards" ContentType="Entries">
	<Description>Карточка сателлит, в которой хранятся все данные для топиков</Description>
	<SchemeComplexColumn IsSystem="true" IsPermanent="true" IsSealed="true" ID="82f8e183-6132-001d-2000-056c791b749f" Name="ID" Type="Reference(Typified) Not Null" ReferencedTable="1074eadd-21d7-4925-98c8-40d1e5f0ca0e">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="82f8e183-6132-011d-4000-056c791b749f" Name="ID" Type="Guid Not Null" ReferencedColumn="9a58123b-b2e9-4137-9c6c-5dab0ec02747" />
	</SchemeComplexColumn>
	<SchemeComplexColumn ID="27370312-87c1-49fb-a9a5-d8b2f735d37e" Name="MainCard" Type="Reference(Typified) Not Null" ReferencedTable="1074eadd-21d7-4925-98c8-40d1e5f0ca0e">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="27370312-87c1-00fb-4000-08b2f735d37e" Name="MainCardID" Type="Guid Not Null" ReferencedColumn="9a58123b-b2e9-4137-9c6c-5dab0ec02747" />
	</SchemeComplexColumn>
	<SchemePrimaryKey IsSystem="true" IsPermanent="true" IsSealed="true" ID="82f8e183-6132-001d-5000-056c791b749f" Name="pk_FmForumSatellite" IsClustered="true">
		<SchemeIndexedColumn Column="82f8e183-6132-011d-4000-056c791b749f" />
	</SchemePrimaryKey>
</SchemeTable>