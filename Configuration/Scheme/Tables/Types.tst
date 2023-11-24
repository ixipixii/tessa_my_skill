<?xml version="1.0" encoding="utf-8"?>
<SchemeTable IsSystem="true" IsPermanent="true" ID="b0538ece-8468-4d0b-8b4e-5a1d43e024db" Name="Types" Group="System">
	<Description>Contains metadata that describes types which used by Tessa.</Description>
	<SchemePhysicalColumn IsPermanent="true" IsSealed="true" ID="a628a864-c858-4200-a6b7-da78c8e6e1f4" Name="ID" Type="Guid Not Null" IsRowGuidColumn="true">
		<Description>ID of a type.</Description>
	</SchemePhysicalColumn>
	<SchemePhysicalColumn IsPermanent="true" IsSealed="true" ID="71181642-0d62-45f9-8ad8-ccec4bd4ce22" Name="Name" Type="String(128) Not Null">
		<Description>Name of a type.</Description>
	</SchemePhysicalColumn>
	<SchemePhysicalColumn IsPermanent="true" IsSealed="true" ID="39d7d0ad-a8ac-4509-ac9e-3ebcfeeea569" Name="Definition" Type="Xml Not Null">
		<Description>Definition of a type.</Description>
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="0a02451e-2e06-4001-9138-b4805e641afa" Name="Caption" Type="String(128) Not Null">
		<Description>Caption of a type.</Description>
	</SchemePhysicalColumn>
	<SchemePrimaryKey IsPermanent="true" IsSealed="true" ID="c2789dd3-b51f-4adb-a1c2-8635695e9bed" Name="pk_Types" IsClustered="true">
		<SchemeIndexedColumn Column="a628a864-c858-4200-a6b7-da78c8e6e1f4" />
	</SchemePrimaryKey>
	<SchemeUniqueKey IsPermanent="true" IsSealed="true" ID="cb1555cd-79c0-49d8-ac5f-1ac0df4d045d" Name="ndx_Types_Name">
		<SchemeIndexedColumn Column="71181642-0d62-45f9-8ad8-ccec4bd4ce22" />
	</SchemeUniqueKey>
</SchemeTable>