<?xml version="1.0" encoding="utf-8"?>
<SchemeTable Partition="c3b14969-a553-4005-9a07-e9705c15b1e1" ID="24394617-9e08-4b9f-afb7-779852444da7" Name="FdStageTemplateCompletion_NextStagesWithCriterias" Group="Fd" InstanceType="Cards" ContentType="Collections">
	<Description>Коллекция этапов, которые проверяются на соответствие критериям</Description>
	<SchemeComplexColumn IsSystem="true" IsPermanent="true" IsSealed="true" ID="24394617-9e08-009f-2000-079852444da7" Name="ID" Type="Reference(Typified) Not Null" ReferencedTable="1074eadd-21d7-4925-98c8-40d1e5f0ca0e">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="24394617-9e08-019f-4000-079852444da7" Name="ID" Type="Guid Not Null" ReferencedColumn="9a58123b-b2e9-4137-9c6c-5dab0ec02747" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn IsSystem="true" IsPermanent="true" IsSealed="true" ID="24394617-9e08-009f-3100-079852444da7" Name="RowID" Type="Guid Not Null" />
	<SchemeComplexColumn ID="63f1d0a1-546f-404e-b2c1-02317e034e54" Name="StageTemplateCompletion" Type="Reference(Typified) Not Null" ReferencedTable="2b1781ba-09e1-42a0-9969-fe4caa9abb81" IsReferenceToOwner="true">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="63f1d0a1-546f-004e-4000-02317e034e54" Name="StageTemplateCompletionRowID" Type="Guid Not Null" ReferencedColumn="2b1781ba-09e1-00a0-3100-0e4caa9abb81" />
	</SchemeComplexColumn>
	<SchemeComplexColumn ID="15326107-a668-45a3-b18d-4b37377b6e6d" Name="NextStageTemplate" Type="Reference(Typified) Not Null" ReferencedTable="323be76d-516a-4ee9-b6b0-e76391d70426" WithForeignKey="false">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="15326107-a668-00a3-4000-0b37377b6e6d" Name="NextStageTemplateID" Type="Guid Not Null" ReferencedColumn="323be76d-516a-01e9-4000-076391d70426" />
		<SchemeReferencingColumn ID="85e97bd4-3646-4a20-8c7f-9d3238805ad8" Name="NextStageTemplateName" Type="String(255) Not Null" ReferencedColumn="7bc776b2-ac1d-4c81-b4b3-c52ecf5b1937" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn ID="d295b195-d6ee-46eb-ad3b-1411df76feb2" Name="Order" Type="Int32 Not Null">
		<SchemeDefaultConstraint IsPermanent="true" ID="ab98a87a-efb0-45a2-b056-ce1032ccdb88" Name="df_FdStageTemplateCompletion_NextStagesWithCriterias_Order" Value="0" />
	</SchemePhysicalColumn>
	<SchemeComplexColumn ID="8582b7a2-f869-4f6d-95a2-fe07622751c9" Name="TaskTypeRoleField" Type="Reference(Typified) Null" ReferencedTable="d30606d0-3bdd-4c18-b6ee-5d8d19cfb333">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="8582b7a2-f869-006d-4000-0e07622751c9" Name="TaskTypeRoleFieldID" Type="Guid Null" ReferencedColumn="d30606d0-3bdd-0118-4000-0d8d19cfb333" />
		<SchemeReferencingColumn ID="5a2eb273-bfa0-49ef-8e03-ec571cab3f68" Name="TaskTypeRoleFieldName" Type="String(Max) Null" ReferencedColumn="d21b0bc8-ff67-48ee-8d18-cff15549a309" />
		<SchemeReferencingColumn ID="024976f2-5489-4caa-903c-b8874d5d6070" Name="TaskTypeRoleFieldControlTypeID" Type="Guid Null" ReferencedColumn="70ca101a-e2e3-4ca6-bf91-d8df5cd78ecd" />
		<SchemeReferencingColumn ID="309d82bf-c96c-42fc-a1f5-8d345ebb4ecf" Name="TaskTypeRoleFieldControlTypeName" Type="String(Max) Null" ReferencedColumn="2e0e0121-ab28-4b59-bb45-4974a54ab3e0" />
		<SchemeReferencingColumn ID="294fd546-5e78-4f16-a61d-d3efafcdb21d" Name="TaskTypeRoleFieldSectionID" Type="Guid Null" ReferencedColumn="3063c6fd-b546-4368-86f1-6d78c7238201" />
		<SchemeReferencingColumn ID="65aed782-4ed0-49d2-a9ae-fec8d913cc46" Name="TaskTypeRoleFieldSectionName" Type="String(Max) Null" ReferencedColumn="fb17f159-853e-461a-849d-47dd60090e8a" />
		<SchemeReferencingColumn ID="54006990-3461-460a-ab1c-250301b4e074" Name="TaskTypeRoleFieldPhysicalColumnID" Type="Guid Null" ReferencedColumn="577c0a40-af09-40f6-b262-dc64ee10649b" />
		<SchemeReferencingColumn ID="41e1a4be-8e6f-4514-9ade-8ca525403b6f" Name="TaskTypeRoleFieldComplexColumnID" Type="Guid Null" ReferencedColumn="f65e0789-478e-42d3-bb01-b51ebffabb37" />
		<SchemeReferencingColumn ID="d4172aed-5bd1-4c6a-a86b-4e076f1c5bd5" Name="TaskTypeRoleFieldColumnName" Type="String(Max) Null" ReferencedColumn="5dbe1a72-b58a-41b5-9d89-6755023f4938" />
		<SchemeReferencingColumn ID="79e39801-e36e-4705-ae01-7c7982e65d42" Name="TaskTypeRoleFieldReferencedTableID" Type="Guid Null" ReferencedColumn="4ce6bdbf-395e-418f-a59f-97a3c9daace0" />
		<SchemeReferencingColumn ID="20e62fcf-cfb5-4945-8fc9-7e4163908a06" Name="TaskTypeRoleFieldReferencedTableName" Type="String(Max) Null" ReferencedColumn="333d1847-c2da-4fd1-97e0-c4f6426be594" />
		<SchemeReferencingColumn ID="0baf99c9-d6e4-4893-99ea-fd3c3a88826b" Name="TaskTypeRoleFieldSpecialMode" Type="Int32 Null" ReferencedColumn="fc5e528e-67d6-4b8c-80f9-70c08bcbdddd" />
	</SchemeComplexColumn>
	<SchemePrimaryKey IsSystem="true" IsPermanent="true" IsSealed="true" ID="24394617-9e08-009f-5000-079852444da7" Name="pk_FdStageTemplateCompletion_NextStagesWithCriterias">
		<SchemeIndexedColumn Column="24394617-9e08-009f-3100-079852444da7" />
	</SchemePrimaryKey>
	<SchemeIndex IsSystem="true" IsPermanent="true" IsSealed="true" ID="24394617-9e08-009f-7000-079852444da7" Name="idx_FdStageTemplateCompletion_NextStagesWithCriterias_ID" IsClustered="true">
		<SchemeIndexedColumn Column="24394617-9e08-019f-4000-079852444da7" />
	</SchemeIndex>
</SchemeTable>