<?xml version="1.0" encoding="utf-8"?>
<SchemeTable Partition="d1b372f3-7565-4309-9037-5e5a0969d94e" ID="509d961f-00cf-4403-a78f-6736841de448" Name="TEST_CarMainInfo" Group="Test" InstanceType="Cards" ContentType="Entries">
	<SchemeComplexColumn IsSystem="true" IsPermanent="true" IsSealed="true" ID="509d961f-00cf-0003-2000-0736841de448" Name="ID" Type="Reference(Typified) Not Null" ReferencedTable="1074eadd-21d7-4925-98c8-40d1e5f0ca0e">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="509d961f-00cf-0103-4000-0736841de448" Name="ID" Type="Guid Not Null" ReferencedColumn="9a58123b-b2e9-4137-9c6c-5dab0ec02747" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn ID="be4bab2d-a613-4c64-95f7-ac6dcf12ecfc" Name="Name" Type="String(50) Not Null" />
	<SchemePhysicalColumn ID="ef4db447-b0b5-4474-a6b7-6c5c75465355" Name="MaxSpeed" Type="Int32 Not Null">
		<Description>Км/ч</Description>
		<SchemeDefaultConstraint IsPermanent="true" ID="a4762bee-51db-4402-9749-86d34fa6ec97" Name="df_TEST_CarMainInfo_MaxSpeed" Value="60" />
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="ef54ad22-5cfa-4b4d-a810-aad2dbe4df3b" Name="Running" Type="Double Null">
		<SchemeDefaultConstraint IsPermanent="true" ID="4b8d0453-f6e4-4787-9bb4-6571a7dc3289" Name="df_TEST_CarMainInfo_Running" Value="0" />
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="1c46bc70-369a-42fb-9dd3-854dc3d14efc" Name="Cost" Type="Currency Not Null">
		<SchemeDefaultConstraint IsPermanent="true" ID="59c1c582-bbbb-4663-966e-ac17cf660e7a" Name="df_TEST_CarMainInfo_Cost" Value="0" />
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="bfff044e-ea59-4f77-9c62-d9f26aba549b" Name="ReleaseDate" Type="Date Null" />
	<SchemeComplexColumn ID="b87bdcf1-0f1b-4267-aa16-efdef4570892" Name="Driver" Type="Reference(Typified) Null" ReferencedTable="6c977939-bbfc-456f-a133-f1c2244e3cc3">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="b87bdcf1-0f1b-0067-4000-0fdef4570892" Name="DriverID" Type="Guid Null" ReferencedColumn="6c977939-bbfc-016f-4000-01c2244e3cc3" />
		<SchemeReferencingColumn ID="78bc1bd7-5b5c-47f6-969b-60467cec599c" Name="DriverName" Type="String(128) Null" ReferencedColumn="1782f76a-4743-4aa4-920c-7edaee860964">
			<Description>Отображаемое имя пользователя.</Description>
		</SchemeReferencingColumn>
	</SchemeComplexColumn>
	<SchemePrimaryKey IsSystem="true" IsPermanent="true" IsSealed="true" ID="509d961f-00cf-0003-5000-0736841de448" Name="pk_TEST_CarMainInfo" IsClustered="true">
		<SchemeIndexedColumn Column="509d961f-00cf-0103-4000-0736841de448" />
	</SchemePrimaryKey>
</SchemeTable>