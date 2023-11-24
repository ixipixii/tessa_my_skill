<?xml version="1.0" encoding="utf-8"?>
<SchemeTable Partition="c3b14969-a553-4005-9a07-e9705c15b1e1" ID="ba857d60-e905-4e9f-bb44-e2d21dd25027" Name="FdStageInstancesVirtual" Group="Fd" IsVirtual="true" InstanceType="Cards" ContentType="Collections">
	<SchemeComplexColumn IsSystem="true" IsPermanent="true" IsSealed="true" ID="ba857d60-e905-009f-2000-02d21dd25027" Name="ID" Type="Reference(Typified) Not Null" ReferencedTable="1074eadd-21d7-4925-98c8-40d1e5f0ca0e">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="ba857d60-e905-019f-4000-02d21dd25027" Name="ID" Type="Guid Not Null" ReferencedColumn="9a58123b-b2e9-4137-9c6c-5dab0ec02747" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn IsSystem="true" IsPermanent="true" IsSealed="true" ID="ba857d60-e905-009f-3100-02d21dd25027" Name="RowID" Type="Guid Not Null" />
	<SchemeComplexColumn ID="03cea5ac-6254-4dab-86bc-600375274687" Name="ProcessInstance" Type="Reference(Typified) Not Null" ReferencedTable="ce0e5c9d-a8d2-498a-b8a0-d9bed35d5735">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="03cea5ac-6254-00ab-4000-000375274687" Name="ProcessInstanceRowID" Type="Guid Not Null" ReferencedColumn="ce0e5c9d-a8d2-008a-3100-09bed35d5735" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn ID="bedeafb9-5c6d-4115-adc2-12b49d6459e6" Name="Name" Type="String(255) Not Null" />
	<SchemePhysicalColumn ID="077b25da-6fc8-4835-8543-ea4d52d35996" Name="Order" Type="Int32 Not Null">
		<SchemeDefaultConstraint IsPermanent="true" ID="b264616a-f026-4f8b-9fee-7c827e8a14de" Name="df_FdStageInstancesVirtual_Order" Value="0" />
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="a1e52bd3-7ef7-4aba-b869-c20f8dbed038" Name="IsParallel" Type="Boolean Not Null">
		<SchemeDefaultConstraint IsPermanent="true" ID="d1a5df16-f76e-43c5-8495-2d3c40cec8fc" Name="df_FdStageInstancesVirtual_IsParallel" Value="false" />
	</SchemePhysicalColumn>
	<SchemeComplexColumn ID="313d753e-76ec-4cea-adb2-e94566160e0a" Name="BasedOnStageTemplate" Type="Reference(Typified) Not Null" ReferencedTable="323be76d-516a-4ee9-b6b0-e76391d70426">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="313d753e-76ec-00ea-4000-094566160e0a" Name="BasedOnStageTemplateID" Type="Guid Not Null" ReferencedColumn="323be76d-516a-01e9-4000-076391d70426" />
		<SchemeReferencingColumn ID="4b928659-ad69-440b-b569-81b1ea812f5c" Name="BasedOnStageTemplateName" Type="String(255) Not Null" ReferencedColumn="7bc776b2-ac1d-4c81-b4b3-c52ecf5b1937" />
	</SchemeComplexColumn>
	<SchemeComplexColumn ID="4a0d67eb-a8ea-4a78-a66d-6cee57f6de35" Name="State" Type="Reference(Typified) Not Null" ReferencedTable="7298c4f3-6c27-439e-a417-bafa3967e40e">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="4a0d67eb-a8ea-0078-4000-0cee57f6de35" Name="StateID" Type="Int16 Not Null" ReferencedColumn="a9399756-feeb-4507-8b42-f66517400134" />
		<SchemeReferencingColumn ID="7e18e41f-2935-49d4-a436-177ad5c9a4ba" Name="StateName" Type="String(128) Not Null" ReferencedColumn="31ac86d7-2665-457a-996e-9aee07dc0030" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn ID="108e7632-2b6e-42b7-a0b9-be10addc5b1a" Name="StartDate" Type="DateTime Not Null" />
	<SchemeComplexColumn ID="97517d41-5796-44a5-88df-dee88ce050d0" Name="TaskType" Type="Reference(Typified) Null" ReferencedTable="b0538ece-8468-4d0b-8b4e-5a1d43e024db">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="97517d41-5796-00a5-4000-0ee88ce050d0" Name="TaskTypeID" Type="Guid Null" ReferencedColumn="a628a864-c858-4200-a6b7-da78c8e6e1f4" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn ID="923e601d-de3f-492d-9690-aa23de274873" Name="TaskDigest" Type="String(512) Null" />
	<SchemeComplexColumn ID="045b46d5-36f7-4b9e-ac2c-bec9cfc44a9d" Name="CompletionResult" Type="Reference(Typified) Null" ReferencedTable="2b1781ba-09e1-42a0-9969-fe4caa9abb81" WithForeignKey="false">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="045b46d5-36f7-009e-4000-0ec9cfc44a9d" Name="CompletionResultRowID" Type="Guid Null" ReferencedColumn="2b1781ba-09e1-00a0-3100-0e4caa9abb81" />
		<SchemeReferencingColumn ID="23ad2bf9-5c72-420b-b799-7e1da3f061f3" Name="CompletionResultCompletionText" Type="String(Max) Null" ReferencedColumn="97a09aa3-c465-4284-b474-8df1823c4774" />
	</SchemeComplexColumn>
	<SchemeComplexColumn ID="9870c9a1-b5d5-45f5-a4d2-315245af13ef" Name="ParentStageInstance" Type="Reference(Typified) Null" ReferencedTable="ffced5f3-fea9-41e1-bcc6-b156b3e3c054" WithForeignKey="false">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="9870c9a1-b5d5-00f5-4000-015245af13ef" Name="ParentStageInstanceRowID" Type="Guid Null" ReferencedColumn="ffced5f3-fea9-00e1-3100-0156b3e3c054" />
		<SchemeReferencingColumn ID="8d9f3fe9-a973-43ec-aa88-b788b0662db5" Name="ParentStageInstanceName" Type="String(255) Null" ReferencedColumn="a3683ea2-ba85-45a1-84a9-eb803f8ea9ed" />
	</SchemeComplexColumn>
	<SchemePrimaryKey IsSystem="true" IsPermanent="true" IsSealed="true" ID="ba857d60-e905-009f-5000-02d21dd25027" Name="pk_FdStageInstancesVirtual">
		<SchemeIndexedColumn Column="ba857d60-e905-009f-3100-02d21dd25027" />
	</SchemePrimaryKey>
	<SchemeIndex IsSystem="true" IsPermanent="true" IsSealed="true" ID="ba857d60-e905-009f-7000-02d21dd25027" Name="idx_FdStageInstancesVirtual_ID" IsClustered="true">
		<SchemeIndexedColumn Column="ba857d60-e905-019f-4000-02d21dd25027" />
	</SchemeIndex>
</SchemeTable>