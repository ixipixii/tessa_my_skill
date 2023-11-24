<?xml version="1.0" encoding="utf-8"?>
<SchemeTable Partition="d1b372f3-7565-4309-9037-5e5a0969d94e" ID="663dcabe-f9d8-4a52-a235-6c407e683810" Name="KrDialogStageTypeSettingsVirtual" Group="KrStageTypes" IsVirtual="true" InstanceType="Cards" ContentType="Entries">
	<SchemeComplexColumn IsSystem="true" IsPermanent="true" IsSealed="true" ID="663dcabe-f9d8-0052-2000-0c407e683810" Name="ID" Type="Reference(Typified) Not Null" ReferencedTable="1074eadd-21d7-4925-98c8-40d1e5f0ca0e">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="663dcabe-f9d8-0152-4000-0c407e683810" Name="ID" Type="Guid Not Null" ReferencedColumn="9a58123b-b2e9-4137-9c6c-5dab0ec02747" />
	</SchemeComplexColumn>
	<SchemeComplexColumn ID="4598e7e3-9513-4f27-9463-ce7584ef64c4" Name="DialogType" Type="Reference(Typified) Not Null" ReferencedTable="a90baecf-c9ce-4cba-8bb0-150a13666266" WithForeignKey="false">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="4598e7e3-9513-0027-4000-0e7584ef64c4" Name="DialogTypeID" Type="Guid Not Null" ReferencedColumn="a90baecf-c9ce-01ba-4000-050a13666266" />
		<SchemePhysicalColumn ID="7082e3f3-1de8-47be-9206-38c197f17ed8" Name="DialogTypeName" Type="String(Max) Not Null">
			<Description>Название типа карточки.</Description>
		</SchemePhysicalColumn>
		<SchemeReferencingColumn ID="126da579-90c7-41a8-a8be-0dd8c04f63b4" Name="DialogTypeCaption" Type="String(128) Not Null" ReferencedColumn="447f7cb1-76ae-4703-b3bb-16a57d4e7ab1" />
	</SchemeComplexColumn>
	<SchemeComplexColumn ID="3a5dd5cc-59cf-4616-b67a-58b3803914b1" Name="CardStoreMode" Type="Reference(Typified) Null" ReferencedTable="f383bf09-2ec9-4fe5-aa50-f3b14898c976">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="3a5dd5cc-59cf-0016-4000-08b3803914b1" Name="CardStoreModeID" Type="Int32 Null" ReferencedColumn="c3ebd27e-4fd3-40d9-9bed-13716ba05342" />
		<SchemeReferencingColumn ID="4bf74c0e-49ed-406b-bc6f-1b042cff9fab" Name="CardStoreModeName" Type="String(Max) Null" ReferencedColumn="a0c0f93e-a43c-4949-9216-e3b1f8de1b3a" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn ID="2bd9cb3d-4180-49e5-9351-0d8f89202950" Name="DialogActionScript" Type="String(Max) Null" />
	<SchemePhysicalColumn ID="5b3ff1c2-5a11-490b-9a65-c30592f878c8" Name="ButtonName" Type="String(Max) Null" />
	<SchemePhysicalColumn ID="6c5c46fa-87a5-410d-b1f0-2f988c807d58" Name="DialogName" Type="String(Max) Null" />
	<SchemePhysicalColumn ID="97ab50ca-fd49-4560-af24-8329e98ddce7" Name="DialogAlias" Type="String(Max) Null" />
	<SchemeComplexColumn ID="0965aae9-b0a2-4608-a07e-9c102e6fc9b3" Name="OpenMode" Type="Reference(Typified) Not Null" ReferencedTable="b1827f66-89bd-4269-b2ce-ea27337616fd">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="0965aae9-b0a2-0008-4000-0c102e6fc9b3" Name="OpenModeID" Type="Int32 Not Null" ReferencedColumn="fca3e61d-c404-4dd1-8980-e069f10512ac" />
		<SchemeReferencingColumn ID="b7e3cea2-bd40-446d-a4f9-3474709ca3c3" Name="OpenModeName" Type="String(Max) Not Null" ReferencedColumn="915af02d-d52b-40b6-9d35-34ce36491731" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn ID="94f20bc5-040e-46ae-9b6c-8eabe35ba9e6" Name="TaskDigest" Type="String(Max) Null" />
	<SchemePhysicalColumn ID="25fb7a37-c5fa-4854-bbaa-46c855753f8f" Name="DialogCardSavingScript" Type="String(Max) Null" />
	<SchemePrimaryKey IsSystem="true" IsPermanent="true" IsSealed="true" ID="663dcabe-f9d8-0052-5000-0c407e683810" Name="pk_KrDialogStageTypeSettingsVirtual" IsClustered="true">
		<SchemeIndexedColumn Column="663dcabe-f9d8-0152-4000-0c407e683810" />
	</SchemePrimaryKey>
</SchemeTable>