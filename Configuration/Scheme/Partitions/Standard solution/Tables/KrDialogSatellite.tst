<?xml version="1.0" encoding="utf-8"?>
<SchemeTable Partition="d1b372f3-7565-4309-9037-5e5a0969d94e" ID="32561dd5-a010-4c57-8b3d-9c3e621034f4" Name="KrDialogSatellite" Group="Kr" InstanceType="Cards" ContentType="Entries">
	<SchemeComplexColumn IsSystem="true" IsPermanent="true" IsSealed="true" ID="32561dd5-a010-0057-2000-0c3e621034f4" Name="ID" Type="Reference(Typified) Not Null" ReferencedTable="1074eadd-21d7-4925-98c8-40d1e5f0ca0e">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="32561dd5-a010-0157-4000-0c3e621034f4" Name="ID" Type="Guid Not Null" ReferencedColumn="9a58123b-b2e9-4137-9c6c-5dab0ec02747" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn ID="fa36cab2-ca56-45c0-885c-7edaba282276" Name="MainCardID" Type="Guid Not Null" />
	<SchemePhysicalColumn ID="8137daf7-d529-4047-a8da-0fa4cc5c659f" Name="TypeID" Type="Guid Not Null" />
	<SchemePrimaryKey IsSystem="true" IsPermanent="true" IsSealed="true" ID="32561dd5-a010-0057-5000-0c3e621034f4" Name="pk_KrDialogSatellite" IsClustered="true">
		<SchemeIndexedColumn Column="32561dd5-a010-0157-4000-0c3e621034f4" />
	</SchemePrimaryKey>
	<SchemeIndex ID="4568eb44-f43c-4ccb-98a8-ef7916fe1f2c" Name="ndx_KrDialogSatellite_MainCardID">
		<SchemeIndexedColumn Column="fa36cab2-ca56-45c0-885c-7edaba282276" />
	</SchemeIndex>
</SchemeTable>