<?xml version="1.0" encoding="utf-8"?>
<SchemeTable Partition="c3b14969-a553-4005-9a07-e9705c15b1e1" ID="8ed6e919-48a8-4561-b35c-1582d447e780" Name="FdProcessTemplateStagesVirtual" Group="Fd" IsVirtual="true" InstanceType="Cards" ContentType="Collections">
	<SchemeComplexColumn IsSystem="true" IsPermanent="true" IsSealed="true" ID="8ed6e919-48a8-0061-2000-0582d447e780" Name="ID" Type="Reference(Typified) Not Null" ReferencedTable="1074eadd-21d7-4925-98c8-40d1e5f0ca0e">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="8ed6e919-48a8-0161-4000-0582d447e780" Name="ID" Type="Guid Not Null" ReferencedColumn="9a58123b-b2e9-4137-9c6c-5dab0ec02747" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn IsSystem="true" IsPermanent="true" IsSealed="true" ID="8ed6e919-48a8-0061-3100-0582d447e780" Name="RowID" Type="Guid Not Null" />
	<SchemePhysicalColumn ID="abf4b807-9377-4e04-bd7b-b78a257db0e8" Name="StageID" Type="Guid Not Null">
		<Description>ID этапа-шаблона</Description>
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="9910ce4a-f0b6-412b-b424-b789ce1c2d39" Name="StageName" Type="String(255) Not Null" />
	<SchemePhysicalColumn ID="a9477771-5169-48b4-b316-cd382791fee0" Name="Order" Type="Int32 Not Null">
		<SchemeDefaultConstraint IsPermanent="true" ID="28823dc4-7ab7-4a2c-b200-9504f2e2c747" Name="df_FdProcessTemplateStagesVirtual_Order" Value="0" />
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="dd1d6005-5982-4e32-bebf-15938638518f" Name="Description" Type="String(Max) Null" />
	<SchemePhysicalColumn ID="ada1852d-4d59-474f-8bd7-39bf416da4e9" Name="TaskDigest" Type="String(512) Null" />
	<SchemePhysicalColumn ID="83ef0f3a-4ab8-480c-9e4c-4e1bcc6c904c" Name="ParticipantsText" Type="String(Max) Null" />
	<SchemePhysicalColumn ID="0d75099d-47d2-47c2-af33-0b75e386474b" Name="IsParallel" Type="Boolean Not Null">
		<SchemeDefaultConstraint IsPermanent="true" ID="6ceb0944-cf94-41df-b532-f1b62fad0590" Name="df_FdProcessTemplateStagesVirtual_IsParallel" Value="false" />
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="842995e4-5a8f-4cbb-abee-cd545db12bc2" Name="StageTypeName" Type="String(256) Null" />
	<SchemePhysicalColumn ID="9c88c662-f224-4bb1-8ed0-bee9278cbf8e" Name="NextStage" Type="String(2000) Null" />
	<SchemePrimaryKey IsSystem="true" IsPermanent="true" IsSealed="true" ID="8ed6e919-48a8-0061-5000-0582d447e780" Name="pk_FdProcessTemplateStagesVirtual">
		<SchemeIndexedColumn Column="8ed6e919-48a8-0061-3100-0582d447e780" />
	</SchemePrimaryKey>
	<SchemeIndex IsSystem="true" IsPermanent="true" IsSealed="true" ID="8ed6e919-48a8-0061-7000-0582d447e780" Name="idx_FdProcessTemplateStagesVirtual_ID" IsClustered="true">
		<SchemeIndexedColumn Column="8ed6e919-48a8-0161-4000-0582d447e780" />
	</SchemeIndex>
</SchemeTable>