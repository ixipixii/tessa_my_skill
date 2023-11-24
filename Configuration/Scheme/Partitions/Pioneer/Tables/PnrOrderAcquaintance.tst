<?xml version="1.0" encoding="utf-8"?>
<SchemeTable Partition="54ed79f5-11c0-4f74-b8b0-793d596699d6" ID="8386595c-03a0-4734-83f8-f32329c68b19" Name="PnrOrderAcquaintance" Group="Pnr" InstanceType="Cards" ContentType="Collections">
	<Description>Список на ознакомление в Приказах</Description>
	<SchemeComplexColumn IsSystem="true" IsPermanent="true" IsSealed="true" ID="8386595c-03a0-0034-2000-032329c68b19" Name="ID" Type="Reference(Typified) Not Null" ReferencedTable="1074eadd-21d7-4925-98c8-40d1e5f0ca0e">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="8386595c-03a0-0134-4000-032329c68b19" Name="ID" Type="Guid Not Null" ReferencedColumn="9a58123b-b2e9-4137-9c6c-5dab0ec02747" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn IsSystem="true" IsPermanent="true" IsSealed="true" ID="8386595c-03a0-0034-3100-032329c68b19" Name="RowID" Type="Guid Not Null" />
	<SchemeComplexColumn ID="15b5f6a0-63a0-4442-8e03-2f84dfa485b3" Name="Role" Type="Reference(Typified) Null" ReferencedTable="81f6010b-9641-4aa5-8897-b8e8603fbf4b">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="15b5f6a0-63a0-0042-4000-0f84dfa485b3" Name="RoleID" Type="Guid Null" ReferencedColumn="81f6010b-9641-01a5-4000-08e8603fbf4b" />
		<SchemeReferencingColumn ID="6282a342-002e-4fa8-9513-e2fffb4e7ee2" Name="RoleName" Type="String(128) Null" ReferencedColumn="616d6b2e-35d5-424d-846b-618eb25962d0" />
	</SchemeComplexColumn>
	<SchemePrimaryKey IsSystem="true" IsPermanent="true" IsSealed="true" ID="8386595c-03a0-0034-5000-032329c68b19" Name="pk_PnrOrderAcquaintance">
		<SchemeIndexedColumn Column="8386595c-03a0-0034-3100-032329c68b19" />
	</SchemePrimaryKey>
	<SchemeIndex IsSystem="true" IsPermanent="true" IsSealed="true" ID="8386595c-03a0-0034-7000-032329c68b19" Name="idx_PnrOrderAcquaintance_ID" IsClustered="true">
		<SchemeIndexedColumn Column="8386595c-03a0-0134-4000-032329c68b19" />
	</SchemeIndex>
</SchemeTable>