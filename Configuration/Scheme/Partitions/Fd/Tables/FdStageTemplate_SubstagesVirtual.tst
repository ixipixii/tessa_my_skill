<?xml version="1.0" encoding="utf-8"?>
<SchemeTable Partition="c3b14969-a553-4005-9a07-e9705c15b1e1" ID="cfaf0103-604a-4df8-b236-fa2e0d2d81f2" Name="FdStageTemplate_SubstagesVirtual" Group="Fd" IsVirtual="true" InstanceType="Cards" ContentType="Collections">
	<Description>Подэтапы этапа</Description>
	<SchemeComplexColumn IsSystem="true" IsPermanent="true" IsSealed="true" ID="cfaf0103-604a-00f8-2000-0a2e0d2d81f2" Name="ID" Type="Reference(Typified) Not Null" ReferencedTable="1074eadd-21d7-4925-98c8-40d1e5f0ca0e">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="cfaf0103-604a-01f8-4000-0a2e0d2d81f2" Name="ID" Type="Guid Not Null" ReferencedColumn="9a58123b-b2e9-4137-9c6c-5dab0ec02747" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn IsSystem="true" IsPermanent="true" IsSealed="true" ID="cfaf0103-604a-00f8-3100-0a2e0d2d81f2" Name="RowID" Type="Guid Not Null" />
	<SchemeComplexColumn ID="37cbe8bb-71a3-4896-8fa9-dffd7a2abaa4" Name="Stage" Type="Reference(Typified) Not Null" ReferencedTable="323be76d-516a-4ee9-b6b0-e76391d70426" WithForeignKey="false">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="37cbe8bb-71a3-0096-4000-0ffd7a2abaa4" Name="StageID" Type="Guid Not Null" ReferencedColumn="323be76d-516a-01e9-4000-076391d70426" />
		<SchemeReferencingColumn ID="8c97b1a7-1b9c-4b1e-a368-53f55b6ec265" Name="StageName" Type="String(255) Not Null" ReferencedColumn="7bc776b2-ac1d-4c81-b4b3-c52ecf5b1937" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn ID="17fbdccc-943c-49be-a5cb-86302f70b8c1" Name="Order" Type="Int32 Null" />
	<SchemePrimaryKey IsSystem="true" IsPermanent="true" IsSealed="true" ID="cfaf0103-604a-00f8-5000-0a2e0d2d81f2" Name="pk_FdStageTemplate_SubstagesVirtual">
		<SchemeIndexedColumn Column="cfaf0103-604a-00f8-3100-0a2e0d2d81f2" />
	</SchemePrimaryKey>
	<SchemeIndex IsSystem="true" IsPermanent="true" IsSealed="true" ID="cfaf0103-604a-00f8-7000-0a2e0d2d81f2" Name="idx_FdStageTemplate_SubstagesVirtual_ID" IsClustered="true">
		<SchemeIndexedColumn Column="cfaf0103-604a-01f8-4000-0a2e0d2d81f2" />
	</SchemeIndex>
</SchemeTable>