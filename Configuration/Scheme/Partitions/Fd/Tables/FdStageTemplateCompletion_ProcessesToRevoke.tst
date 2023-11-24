<?xml version="1.0" encoding="utf-8"?>
<SchemeTable Partition="c3b14969-a553-4005-9a07-e9705c15b1e1" ID="1448c4cc-2e22-4962-b04b-9198efb27aa7" Name="FdStageTemplateCompletion_ProcessesToRevoke" Group="Fd" InstanceType="Cards" ContentType="Collections">
	<Description>Отзываемые процессы</Description>
	<SchemeComplexColumn IsSystem="true" IsPermanent="true" IsSealed="true" ID="1448c4cc-2e22-0062-2000-0198efb27aa7" Name="ID" Type="Reference(Typified) Not Null" ReferencedTable="1074eadd-21d7-4925-98c8-40d1e5f0ca0e">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="1448c4cc-2e22-0162-4000-0198efb27aa7" Name="ID" Type="Guid Not Null" ReferencedColumn="9a58123b-b2e9-4137-9c6c-5dab0ec02747" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn IsSystem="true" IsPermanent="true" IsSealed="true" ID="1448c4cc-2e22-0062-3100-0198efb27aa7" Name="RowID" Type="Guid Not Null" />
	<SchemeComplexColumn ID="98d8420a-c465-4cef-98ae-8b31ae6cc60a" Name="StageTemplateCompletion" Type="Reference(Typified) Not Null" ReferencedTable="2b1781ba-09e1-42a0-9969-fe4caa9abb81" IsReferenceToOwner="true">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="98d8420a-c465-00ef-4000-0b31ae6cc60a" Name="StageTemplateCompletionRowID" Type="Guid Not Null" ReferencedColumn="2b1781ba-09e1-00a0-3100-0e4caa9abb81" />
	</SchemeComplexColumn>
	<SchemeComplexColumn ID="5c65820d-a59d-44a7-bd95-bb70fbeddf06" Name="ProcessTemplate" Type="Reference(Typified) Not Null" ReferencedTable="7d88534f-8bce-4084-a150-64e9594d77c8" WithForeignKey="false">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="5c65820d-a59d-00a7-4000-0b70fbeddf06" Name="ProcessTemplateID" Type="Guid Not Null" ReferencedColumn="7d88534f-8bce-0184-4000-04e9594d77c8" />
		<SchemeReferencingColumn ID="6191bc0d-f12f-4bb1-abb4-3f5c2cba2017" Name="ProcessTemplateName" Type="String(255) Not Null" ReferencedColumn="40395010-1055-40c2-bc1e-a095c16b3517" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn ID="f26d787e-f5e3-42a7-a6af-cf1f3aad5509" Name="Order" Type="Int32 Not Null">
		<SchemeDefaultConstraint IsPermanent="true" ID="d9366ad2-c008-4ebc-997a-4a44632d8336" Name="df_FdStageTemplateCompletion_ProcessesToRevoke_Order" Value="0" />
	</SchemePhysicalColumn>
	<SchemePrimaryKey IsSystem="true" IsPermanent="true" IsSealed="true" ID="1448c4cc-2e22-0062-5000-0198efb27aa7" Name="pk_FdStageTemplateCompletion_ProcessesToRevoke">
		<SchemeIndexedColumn Column="1448c4cc-2e22-0062-3100-0198efb27aa7" />
	</SchemePrimaryKey>
	<SchemeIndex IsSystem="true" IsPermanent="true" IsSealed="true" ID="1448c4cc-2e22-0062-7000-0198efb27aa7" Name="idx_FdStageTemplateCompletion_ProcessesToRevoke_ID" IsClustered="true">
		<SchemeIndexedColumn Column="1448c4cc-2e22-0162-4000-0198efb27aa7" />
	</SchemeIndex>
</SchemeTable>