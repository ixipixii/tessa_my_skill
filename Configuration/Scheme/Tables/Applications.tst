﻿<?xml version="1.0" encoding="utf-8"?>
<SchemeTable ID="6134967a-914b-45eb-99bd-a0ebefdca9f4" Name="Applications" Group="System" InstanceType="Cards" ContentType="Entries">
	<Description>Приложения</Description>
	<SchemeComplexColumn IsSystem="true" IsPermanent="true" IsSealed="true" ID="6134967a-914b-00eb-2000-00ebefdca9f4" Name="ID" Type="Reference(Typified) Not Null" ReferencedTable="1074eadd-21d7-4925-98c8-40d1e5f0ca0e">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="6134967a-914b-01eb-4000-00ebefdca9f4" Name="ID" Type="Guid Not Null" ReferencedColumn="9a58123b-b2e9-4137-9c6c-5dab0ec02747" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn ID="16b68c29-642f-46c8-afbb-1fb18fc430fd" Name="Name" Type="String(256) Not Null" />
	<SchemePhysicalColumn ID="fe275c08-2947-4616-8a96-f6657248e05d" Name="Alias" Type="String(128) Not Null" />
	<SchemePhysicalColumn ID="62698ca1-c1bb-417f-9e27-337cc10c5dda" Name="ExecutableFileName" Type="String(256) Not Null" />
	<SchemePhysicalColumn ID="06002c89-9d80-4e09-a3e1-ff66c7117c65" Name="AppVersion" Type="String(128) Not Null" />
	<SchemePhysicalColumn ID="cb6d5b57-eda3-4751-9bcf-ff918e43bde1" Name="ExtensionVersion" Type="String(128) Null" />
	<SchemePhysicalColumn ID="6f9320cc-86a9-447e-a4c9-f3a980729f63" Name="PlatformVersion" Type="String(128) Null" />
	<SchemePhysicalColumn ID="f54ffb15-23ba-4e56-b0e3-8268956426d8" Name="ForAdmin" Type="Boolean Not Null" />
	<SchemePhysicalColumn ID="1a5c3e51-0448-425a-8ba9-9b9235eea30d" Name="Icon" Type="Binary(Max) Null" />
	<SchemePhysicalColumn ID="cc89174b-ff1c-43f1-9430-2e24df143172" Name="GroupName" Type="String(256) Null" />
	<SchemePrimaryKey IsSystem="true" IsPermanent="true" IsSealed="true" ID="6134967a-914b-00eb-5000-00ebefdca9f4" Name="pk_Applications" IsClustered="true">
		<SchemeIndexedColumn Column="6134967a-914b-01eb-4000-00ebefdca9f4" />
	</SchemePrimaryKey>
	<SchemeIndex ID="31b20a2f-6b19-4d95-83f6-7e18b64175f8" Name="ndx_Applications_Name">
		<SchemeIndexedColumn Column="16b68c29-642f-46c8-afbb-1fb18fc430fd">
			<Expression Dbms="PostgreSql">lower("Name")</Expression>
		</SchemeIndexedColumn>
	</SchemeIndex>
	<SchemeIndex ID="c5238f52-0757-4799-954b-216e89fd21de" Name="ndx_Applications_Alias" IsUnique="true">
		<SchemeIndexedColumn Column="fe275c08-2947-4616-8a96-f6657248e05d">
			<Expression Dbms="PostgreSql">lower("Alias")</Expression>
		</SchemeIndexedColumn>
	</SchemeIndex>
</SchemeTable>