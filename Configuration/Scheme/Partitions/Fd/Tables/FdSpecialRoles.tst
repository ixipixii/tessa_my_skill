<?xml version="1.0" encoding="utf-8"?>
<SchemeTable Partition="c3b14969-a553-4005-9a07-e9705c15b1e1" ID="6e99ace1-78b1-4b05-8e71-f86598de608b" Name="FdSpecialRoles" Group="Fd Dictionary" InstanceType="Cards" ContentType="Entries">
	<Description>Справочник особых ролей</Description>
	<SchemeComplexColumn IsSystem="true" IsPermanent="true" IsSealed="true" ID="6e99ace1-78b1-0005-2000-086598de608b" Name="ID" Type="Reference(Typified) Not Null" ReferencedTable="1074eadd-21d7-4925-98c8-40d1e5f0ca0e">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="6e99ace1-78b1-0105-4000-086598de608b" Name="ID" Type="Guid Not Null" ReferencedColumn="9a58123b-b2e9-4137-9c6c-5dab0ec02747" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn ID="bf1d03e3-5964-4648-95ca-9ab82b541191" Name="Name" Type="String(255) Not Null" />
	<SchemePrimaryKey IsSystem="true" IsPermanent="true" IsSealed="true" ID="6e99ace1-78b1-0005-5000-086598de608b" Name="pk_FdSpecialRoles" IsClustered="true">
		<SchemeIndexedColumn Column="6e99ace1-78b1-0105-4000-086598de608b" />
	</SchemePrimaryKey>
</SchemeTable>