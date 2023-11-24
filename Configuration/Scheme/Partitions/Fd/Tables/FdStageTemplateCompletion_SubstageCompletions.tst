<?xml version="1.0" encoding="utf-8"?>
<SchemeTable Partition="c3b14969-a553-4005-9a07-e9705c15b1e1" ID="a5b68aa8-d3b9-48df-925d-c151c77d90f0" Name="FdStageTemplateCompletion_SubstageCompletions" Group="Fd" InstanceType="Cards" ContentType="Collections">
	<Description>Параметры завершения подэтапов</Description>
	<SchemeComplexColumn IsSystem="true" IsPermanent="true" IsSealed="true" ID="a5b68aa8-d3b9-00df-2000-0151c77d90f0" Name="ID" Type="Reference(Typified) Not Null" ReferencedTable="1074eadd-21d7-4925-98c8-40d1e5f0ca0e">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="a5b68aa8-d3b9-01df-4000-0151c77d90f0" Name="ID" Type="Guid Not Null" ReferencedColumn="9a58123b-b2e9-4137-9c6c-5dab0ec02747" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn IsSystem="true" IsPermanent="true" IsSealed="true" ID="a5b68aa8-d3b9-00df-3100-0151c77d90f0" Name="RowID" Type="Guid Not Null" />
	<SchemeComplexColumn ID="ad11e67d-ada1-4e1e-9333-5ad2c4f917c3" Name="Parent" Type="Reference(Typified) Not Null" ReferencedTable="f45f2a1a-db2a-450c-99f1-94f61561ce8e" IsReferenceToOwner="true">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="ad11e67d-ada1-001e-4000-0ad2c4f917c3" Name="ParentRowID" Type="Guid Not Null" ReferencedColumn="f45f2a1a-db2a-000c-3100-04f61561ce8e" />
	</SchemeComplexColumn>
	<SchemeComplexColumn ID="3a9cfb4a-2f71-490f-87b7-c7990e624908" Name="SubstageCompletion" Type="Reference(Typified) Not Null" ReferencedTable="2b1781ba-09e1-42a0-9969-fe4caa9abb81" WithForeignKey="false">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="3a9cfb4a-2f71-000f-4000-07990e624908" Name="SubstageCompletionRowID" Type="Guid Not Null" ReferencedColumn="2b1781ba-09e1-00a0-3100-0e4caa9abb81" />
		<SchemeReferencingColumn ID="a8fffc68-3140-440f-b824-ee1af4b1ee27" Name="SubstageCompletionDescription" Type="String(Max) Null" ReferencedColumn="7c922f3b-4db4-4a62-807a-b8e7ba56c4be" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn ID="4d672a8b-5b0b-4b30-9155-61cf527c345e" Name="Order" Type="Int32 Not Null">
		<SchemeDefaultConstraint IsPermanent="true" ID="36670e77-7265-4ec4-aa70-5a94fd654b45" Name="df_FdStageTemplateCompletion_SubstageCompletions_Order" Value="0" />
	</SchemePhysicalColumn>
	<SchemePrimaryKey IsSystem="true" IsPermanent="true" IsSealed="true" ID="a5b68aa8-d3b9-00df-5000-0151c77d90f0" Name="pk_FdStageTemplateCompletion_SubstageCompletions">
		<SchemeIndexedColumn Column="a5b68aa8-d3b9-00df-3100-0151c77d90f0" />
	</SchemePrimaryKey>
	<SchemeIndex IsSystem="true" IsPermanent="true" IsSealed="true" ID="a5b68aa8-d3b9-00df-7000-0151c77d90f0" Name="idx_FdStageTemplateCompletion_SubstageCompletions_ID" IsClustered="true">
		<SchemeIndexedColumn Column="a5b68aa8-d3b9-01df-4000-0151c77d90f0" />
	</SchemeIndex>
</SchemeTable>