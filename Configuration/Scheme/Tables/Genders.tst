<?xml version="1.0" encoding="utf-8"?>
<SchemeTable ID="71032cb4-2c7b-43d3-9330-96e9aee23bdb" Name="Genders" Group="Roles">
	<Description>Биологический пол.</Description>
	<SchemePhysicalColumn ID="30da2be3-3839-47ab-bb6c-5bdb816bf597" Name="ID" Type="Int16 Not Null">
		<Description>Идентификатор биологического пола.</Description>
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="8379b3e2-12af-4585-a234-80c5eef33847" Name="Name" Type="String(128) Not Null">
		<Description>Название биологического пола.</Description>
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="e6f5fdbd-8622-42bd-ab1a-14ef6ce77f37" Name="Caption" Type="String(128) Not Null">
		<Description>Отображаемое пользователю название биологического пола.</Description>
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="25d65fdd-778b-4092-b72b-9de8e7819108" Name="Abbreviation" Type="String(128) Not Null">
		<Description>Сокращённое отображаемое пользователю название биологического пола.</Description>
	</SchemePhysicalColumn>
	<SchemePrimaryKey ID="64a5c1d6-7a3c-445e-a0ff-dec366d042fa" Name="pk_Genders" IsClustered="true">
		<SchemeIndexedColumn Column="30da2be3-3839-47ab-bb6c-5bdb816bf597" />
	</SchemePrimaryKey>
	<SchemeRecord>
		<ID ID="30da2be3-3839-47ab-bb6c-5bdb816bf597">1</ID>
		<Name ID="8379b3e2-12af-4585-a234-80c5eef33847">Male</Name>
		<Caption ID="e6f5fdbd-8622-42bd-ab1a-14ef6ce77f37">$Enum_Genders_Caption_Male</Caption>
		<Abbreviation ID="25d65fdd-778b-4092-b72b-9de8e7819108">$Enum_Genders_Abbreviation_Male</Abbreviation>
	</SchemeRecord>
	<SchemeRecord>
		<ID ID="30da2be3-3839-47ab-bb6c-5bdb816bf597">2</ID>
		<Name ID="8379b3e2-12af-4585-a234-80c5eef33847">Female</Name>
		<Caption ID="e6f5fdbd-8622-42bd-ab1a-14ef6ce77f37">$Enum_Genders_Caption_Female</Caption>
		<Abbreviation ID="25d65fdd-778b-4092-b72b-9de8e7819108">$Enum_Genders_Abbreviation_Female</Abbreviation>
	</SchemeRecord>
</SchemeTable>