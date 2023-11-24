<?xml version="1.0" encoding="utf-8"?>
<SchemeTable Partition="c3b14969-a553-4005-9a07-e9705c15b1e1" ID="f45f2a1a-db2a-450c-99f1-94f61561ce8e" Name="FdStageTemplateCompletion_Substages" Group="Fd" InstanceType="Cards" ContentType="Collections">
	<SchemeComplexColumn IsSystem="true" IsPermanent="true" IsSealed="true" ID="f45f2a1a-db2a-000c-2000-04f61561ce8e" Name="ID" Type="Reference(Typified) Not Null" ReferencedTable="1074eadd-21d7-4925-98c8-40d1e5f0ca0e">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="f45f2a1a-db2a-010c-4000-04f61561ce8e" Name="ID" Type="Guid Not Null" ReferencedColumn="9a58123b-b2e9-4137-9c6c-5dab0ec02747" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn IsSystem="true" IsPermanent="true" IsSealed="true" ID="f45f2a1a-db2a-000c-3100-04f61561ce8e" Name="RowID" Type="Guid Not Null" />
	<SchemeComplexColumn ID="877cc3dd-c12b-4ce3-8ef2-1a127376f893" Name="Parent" Type="Reference(Typified) Not Null" ReferencedTable="2b1781ba-09e1-42a0-9969-fe4caa9abb81" IsReferenceToOwner="true">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="877cc3dd-c12b-00e3-4000-0a127376f893" Name="ParentRowID" Type="Guid Not Null" ReferencedColumn="2b1781ba-09e1-00a0-3100-0e4caa9abb81" />
	</SchemeComplexColumn>
	<SchemeComplexColumn ID="0d27db55-c42a-489f-847f-e71e2041768b" Name="SubstageTemplate" Type="Reference(Typified) Not Null" ReferencedTable="323be76d-516a-4ee9-b6b0-e76391d70426" WithForeignKey="false">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="0d27db55-c42a-009f-4000-071e2041768b" Name="SubstageTemplateID" Type="Guid Not Null" ReferencedColumn="323be76d-516a-01e9-4000-076391d70426" />
		<SchemeReferencingColumn ID="f47e4130-0107-40f6-a3a3-a0eb870829cb" Name="SubstageTemplateName" Type="String(255) Not Null" ReferencedColumn="7bc776b2-ac1d-4c81-b4b3-c52ecf5b1937" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn ID="43ff67e0-041b-4a27-a8f5-2c3740c4331b" Name="Order" Type="Int32 Not Null">
		<SchemeDefaultConstraint IsPermanent="true" ID="e9585681-8d6d-45a3-ac63-1406a8361f14" Name="df_FdStageTemplateCompletion_Substages_Order" Value="0" />
	</SchemePhysicalColumn>
	<SchemePrimaryKey IsSystem="true" IsPermanent="true" IsSealed="true" ID="f45f2a1a-db2a-000c-5000-04f61561ce8e" Name="pk_FdStageTemplateCompletion_Substages">
		<SchemeIndexedColumn Column="f45f2a1a-db2a-000c-3100-04f61561ce8e" />
	</SchemePrimaryKey>
	<SchemeIndex IsSystem="true" IsPermanent="true" IsSealed="true" ID="f45f2a1a-db2a-000c-7000-04f61561ce8e" Name="idx_FdStageTemplateCompletion_Substages_ID" IsClustered="true">
		<SchemeIndexedColumn Column="f45f2a1a-db2a-010c-4000-04f61561ce8e" />
	</SchemeIndex>
</SchemeTable>