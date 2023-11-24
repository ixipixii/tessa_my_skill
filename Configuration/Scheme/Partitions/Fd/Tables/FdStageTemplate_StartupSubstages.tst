<?xml version="1.0" encoding="utf-8"?>
<SchemeTable Partition="c3b14969-a553-4005-9a07-e9705c15b1e1" ID="828ac111-48a2-43a8-ab9d-89d6d320562c" Name="FdStageTemplate_StartupSubstages" Group="Fd" InstanceType="Cards" ContentType="Collections">
	<Description>Запускаемые подэтапы</Description>
	<SchemeComplexColumn IsSystem="true" IsPermanent="true" IsSealed="true" ID="828ac111-48a2-00a8-2000-09d6d320562c" Name="ID" Type="Reference(Typified) Not Null" ReferencedTable="1074eadd-21d7-4925-98c8-40d1e5f0ca0e">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="828ac111-48a2-01a8-4000-09d6d320562c" Name="ID" Type="Guid Not Null" ReferencedColumn="9a58123b-b2e9-4137-9c6c-5dab0ec02747" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn IsSystem="true" IsPermanent="true" IsSealed="true" ID="828ac111-48a2-00a8-3100-09d6d320562c" Name="RowID" Type="Guid Not Null" />
	<SchemeComplexColumn ID="40012a08-b1e1-4068-b8ef-53f5622dad29" Name="SubstageTemplate" Type="Reference(Typified) Not Null" ReferencedTable="323be76d-516a-4ee9-b6b0-e76391d70426" WithForeignKey="false">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="40012a08-b1e1-0068-4000-03f5622dad29" Name="SubstageTemplateID" Type="Guid Not Null" ReferencedColumn="323be76d-516a-01e9-4000-076391d70426" />
		<SchemeReferencingColumn ID="a9e2355b-43bb-4b84-a09f-441ab1242297" Name="SubstageTemplateName" Type="String(255) Not Null" ReferencedColumn="7bc776b2-ac1d-4c81-b4b3-c52ecf5b1937" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn ID="97b6fa6d-b549-4326-beea-e67263e8314f" Name="Order" Type="Int32 Not Null">
		<SchemeDefaultConstraint IsPermanent="true" ID="93304514-be63-4c8c-a50d-a268034a09d6" Name="df_FdStageTemplate_StartupSubstages_Order" Value="0" />
	</SchemePhysicalColumn>
	<SchemePrimaryKey IsSystem="true" IsPermanent="true" IsSealed="true" ID="828ac111-48a2-00a8-5000-09d6d320562c" Name="pk_FdStageTemplate_StartupSubstages">
		<SchemeIndexedColumn Column="828ac111-48a2-00a8-3100-09d6d320562c" />
	</SchemePrimaryKey>
	<SchemeIndex IsSystem="true" IsPermanent="true" IsSealed="true" ID="828ac111-48a2-00a8-7000-09d6d320562c" Name="idx_FdStageTemplate_StartupSubstages_ID" IsClustered="true">
		<SchemeIndexedColumn Column="828ac111-48a2-01a8-4000-09d6d320562c" />
	</SchemeIndex>
</SchemeTable>