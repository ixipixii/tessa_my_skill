<?xml version="1.0" encoding="utf-8"?>
<SchemeTable Partition="c3b14969-a553-4005-9a07-e9705c15b1e1" ID="60fe0749-c5de-4b53-b01d-d651290768f1" Name="FdProcessTemplate_StageTemplateErrorsVirtual" Group="FdDiagnostic" IsVirtual="true" InstanceType="Cards" ContentType="Collections">
	<SchemeComplexColumn IsSystem="true" IsPermanent="true" IsSealed="true" ID="60fe0749-c5de-0053-2000-0651290768f1" Name="ID" Type="Reference(Typified) Not Null" ReferencedTable="1074eadd-21d7-4925-98c8-40d1e5f0ca0e">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="60fe0749-c5de-0153-4000-0651290768f1" Name="ID" Type="Guid Not Null" ReferencedColumn="9a58123b-b2e9-4137-9c6c-5dab0ec02747" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn IsSystem="true" IsPermanent="true" IsSealed="true" ID="60fe0749-c5de-0053-3100-0651290768f1" Name="RowID" Type="Guid Not Null" />
	<SchemeComplexColumn ID="845e6f24-70ae-4aa1-b778-e2ae707c66c0" Name="StageTemplate" Type="Reference(Typified) Not Null" ReferencedTable="323be76d-516a-4ee9-b6b0-e76391d70426" WithForeignKey="false">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="845e6f24-70ae-00a1-4000-02ae707c66c0" Name="StageTemplateID" Type="Guid Not Null" ReferencedColumn="323be76d-516a-01e9-4000-076391d70426" />
		<SchemeReferencingColumn ID="e376b926-410c-40d6-a51d-cd5bdf63c7c4" Name="StageTemplateName" Type="String(255) Not Null" ReferencedColumn="7bc776b2-ac1d-4c81-b4b3-c52ecf5b1937" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn ID="0833fa90-ddcc-4083-a734-3b6f28582be1" Name="Order" Type="Int32 Not Null" />
	<SchemePhysicalColumn ID="b91986d2-7f7f-4e84-9041-68795568e6c6" Name="Description" Type="String(Max) Null" />
	<SchemePrimaryKey IsSystem="true" IsPermanent="true" IsSealed="true" ID="60fe0749-c5de-0053-5000-0651290768f1" Name="pk_FdProcessTemplate_StageTemplateErrorsVirtual">
		<SchemeIndexedColumn Column="60fe0749-c5de-0053-3100-0651290768f1" />
	</SchemePrimaryKey>
	<SchemeIndex IsSystem="true" IsPermanent="true" IsSealed="true" ID="60fe0749-c5de-0053-7000-0651290768f1" Name="idx_FdProcessTemplate_StageTemplateErrorsVirtual_ID" IsClustered="true">
		<SchemeIndexedColumn Column="60fe0749-c5de-0153-4000-0651290768f1" />
	</SchemeIndex>
</SchemeTable>