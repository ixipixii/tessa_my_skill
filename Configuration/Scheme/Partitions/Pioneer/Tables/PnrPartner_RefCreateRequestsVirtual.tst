<?xml version="1.0" encoding="utf-8"?>
<SchemeTable Partition="54ed79f5-11c0-4f74-b8b0-793d596699d6" ID="d9b3f400-d306-4d12-ba18-0a592fb47289" Name="PnrPartner_RefCreateRequestsVirtual" Group="Pnr" IsVirtual="true" InstanceType="Cards" ContentType="Collections">
	<SchemeComplexColumn IsSystem="true" IsPermanent="true" IsSealed="true" ID="d9b3f400-d306-0012-2000-0a592fb47289" Name="ID" Type="Reference(Typified) Not Null" ReferencedTable="1074eadd-21d7-4925-98c8-40d1e5f0ca0e">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="d9b3f400-d306-0112-4000-0a592fb47289" Name="ID" Type="Guid Not Null" ReferencedColumn="9a58123b-b2e9-4137-9c6c-5dab0ec02747" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn IsSystem="true" IsPermanent="true" IsSealed="true" ID="d9b3f400-d306-0012-3100-0a592fb47289" Name="RowID" Type="Guid Not Null" />
	<SchemeComplexColumn ID="92d17487-a71e-43c2-8746-d2a58867240c" Name="Doc" Type="Reference(Typified) Not Null" ReferencedTable="a161e289-2f99-4699-9e95-6e3336be8527" WithForeignKey="false">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="92d17487-a71e-00c2-4000-02a58867240c" Name="DocID" Type="Guid Not Null" ReferencedColumn="a161e289-2f99-0199-4000-0e3336be8527" />
		<SchemeReferencingColumn ID="8f754339-6a16-4b0c-8a8d-2d6f41c0527c" Name="DocFullNumber" Type="String(200) Null" ReferencedColumn="eeb023b5-f473-4a8b-bef2-88e07b0d0688" />
	</SchemeComplexColumn>
	<SchemePrimaryKey IsSystem="true" IsPermanent="true" IsSealed="true" ID="d9b3f400-d306-0012-5000-0a592fb47289" Name="pk_PnrPartner_RefCreateRequestsVirtual">
		<SchemeIndexedColumn Column="d9b3f400-d306-0012-3100-0a592fb47289" />
	</SchemePrimaryKey>
	<SchemeIndex IsSystem="true" IsPermanent="true" IsSealed="true" ID="d9b3f400-d306-0012-7000-0a592fb47289" Name="idx_PnrPartner_RefCreateRequestsVirtual_ID" IsClustered="true">
		<SchemeIndexedColumn Column="d9b3f400-d306-0112-4000-0a592fb47289" />
	</SchemeIndex>
</SchemeTable>