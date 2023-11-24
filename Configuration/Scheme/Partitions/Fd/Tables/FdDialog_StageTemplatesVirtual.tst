<?xml version="1.0" encoding="utf-8"?>
<SchemeTable Partition="c3b14969-a553-4005-9a07-e9705c15b1e1" ID="a14aa698-dd3c-49fd-89bb-52a2070d91fb" Name="FdDialog_StageTemplatesVirtual" Group="FdSystem" IsVirtual="true" InstanceType="Cards" ContentType="Collections">
	<SchemeComplexColumn IsSystem="true" IsPermanent="true" IsSealed="true" ID="a14aa698-dd3c-00fd-2000-02a2070d91fb" Name="ID" Type="Reference(Typified) Not Null" ReferencedTable="1074eadd-21d7-4925-98c8-40d1e5f0ca0e">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="a14aa698-dd3c-01fd-4000-02a2070d91fb" Name="ID" Type="Guid Not Null" ReferencedColumn="9a58123b-b2e9-4137-9c6c-5dab0ec02747" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn IsSystem="true" IsPermanent="true" IsSealed="true" ID="a14aa698-dd3c-00fd-3100-02a2070d91fb" Name="RowID" Type="Guid Not Null" />
	<SchemeComplexColumn ID="fb301778-a606-478c-aff5-656daf221fa1" Name="StageTemplate" Type="Reference(Typified) Not Null" ReferencedTable="323be76d-516a-4ee9-b6b0-e76391d70426">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="fb301778-a606-008c-4000-056daf221fa1" Name="StageTemplateID" Type="Guid Not Null" ReferencedColumn="323be76d-516a-01e9-4000-076391d70426" />
		<SchemeReferencingColumn ID="16ae16ee-6c57-4e7c-b797-470558579272" Name="StageTemplateName" Type="String(255) Not Null" ReferencedColumn="7bc776b2-ac1d-4c81-b4b3-c52ecf5b1937" />
		<SchemeReferencingColumn ID="3a5d9af2-4124-4729-ab4a-e953878243d1" Name="StageTemplateDescription" Type="String(Max) Null" ReferencedColumn="9ef86b05-a368-4823-81a8-0bfdd19f8a5f" />
		<SchemePhysicalColumn ID="4f9570af-661e-44b0-9201-a2374d1bf8bc" Name="StageTemplateTaskTypeCaption" Type="String(128) Null" />
		<SchemeReferencingColumn ID="6545742c-a121-4c01-a6f2-6b6e52feb651" Name="StageTemplateParticipantsText" Type="String(Max) Null" ReferencedColumn="f0c85377-8b65-4a49-aa6a-7b55df0af5c3" />
	</SchemeComplexColumn>
	<SchemePrimaryKey IsSystem="true" IsPermanent="true" IsSealed="true" ID="a14aa698-dd3c-00fd-5000-02a2070d91fb" Name="pk_FdDialog_StageTemplatesVirtual">
		<SchemeIndexedColumn Column="a14aa698-dd3c-00fd-3100-02a2070d91fb" />
	</SchemePrimaryKey>
	<SchemeIndex IsSystem="true" IsPermanent="true" IsSealed="true" ID="a14aa698-dd3c-00fd-7000-02a2070d91fb" Name="idx_FdDialog_StageTemplatesVirtual_ID" IsClustered="true">
		<SchemeIndexedColumn Column="a14aa698-dd3c-01fd-4000-02a2070d91fb" />
	</SchemeIndex>
</SchemeTable>