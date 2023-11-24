<?xml version="1.0" encoding="utf-8"?>
<SchemeTable Partition="c3b14969-a553-4005-9a07-e9705c15b1e1" ID="2b1781ba-09e1-42a0-9969-fe4caa9abb81" Name="FdStageTemplateCompletions" Group="Fd" InstanceType="Cards" ContentType="Collections">
	<Description>Параметры завершения этапа</Description>
	<SchemeComplexColumn IsSystem="true" IsPermanent="true" IsSealed="true" ID="2b1781ba-09e1-00a0-2000-0e4caa9abb81" Name="ID" Type="Reference(Typified) Not Null" ReferencedTable="1074eadd-21d7-4925-98c8-40d1e5f0ca0e">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="2b1781ba-09e1-01a0-4000-0e4caa9abb81" Name="ID" Type="Guid Not Null" ReferencedColumn="9a58123b-b2e9-4137-9c6c-5dab0ec02747" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn IsSystem="true" IsPermanent="true" IsSealed="true" ID="2b1781ba-09e1-00a0-3100-0e4caa9abb81" Name="RowID" Type="Guid Not Null" />
	<SchemeComplexColumn ID="58c3e905-28c4-45ea-acdb-7ed359652c7a" Name="NextStageTemplate" Type="Reference(Typified) Null" ReferencedTable="323be76d-516a-4ee9-b6b0-e76391d70426" WithForeignKey="false">
		<Description>Следующий этап</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="58c3e905-28c4-00ea-4000-0ed359652c7a" Name="NextStageTemplateID" Type="Guid Null" ReferencedColumn="323be76d-516a-01e9-4000-076391d70426" />
		<SchemeReferencingColumn ID="5d35bb04-5402-48e0-8d4b-a10c1096a4f7" Name="NextStageTemplateName" Type="String(255) Null" ReferencedColumn="7bc776b2-ac1d-4c81-b4b3-c52ecf5b1937" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn ID="50076eca-3e8d-4646-95c0-047134b00b66" Name="Order" Type="Int32 Not Null">
		<SchemeDefaultConstraint IsPermanent="true" ID="eb695aa2-d7b1-403a-afa1-e79f36cfcaed" Name="df_FdStageTemplateCompletions_Order" Value="0" />
	</SchemePhysicalColumn>
	<SchemeComplexColumn ID="ead51b5b-9394-428c-aa1a-8cf2d7b17d76" Name="CompletionOption" Type="Reference(Typified) Null" ReferencedTable="08cf782d-4130-4377-8a49-3e201a05d496">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="ead51b5b-9394-008c-4000-0cf2d7b17d76" Name="CompletionOptionID" Type="Guid Null" ReferencedColumn="132dc5f5-ce87-4dd0-acce-b4a02acf7715" />
		<SchemeReferencingColumn ID="3b61b4ee-162c-4d63-a906-99bde913e0ed" Name="CompletionOptionName" Type="String(128) Null" ReferencedColumn="aa6a7122-8384-4c81-9553-386f2c05e96c" />
		<SchemeReferencingColumn ID="ac724238-7bab-464b-b539-7ca31bc50a31" Name="CompletionOptionCaption" Type="String(128) Null" ReferencedColumn="6762309a-b0ff-4b2f-9cce-dd111116e554" />
	</SchemeComplexColumn>
	<SchemeComplexColumn ID="0c4caf72-e784-4873-9910-8634a8116a31" Name="CompletionCondition" Type="Reference(Typified) Null" ReferencedTable="4f3b337b-42cf-454f-9010-fee2d827338d">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="0c4caf72-e784-0073-4000-0634a8116a31" Name="CompletionConditionID" Type="Int32 Null" ReferencedColumn="614130b3-a0fb-423c-ab7e-bfa863c5121e" />
		<SchemeReferencingColumn ID="85732a97-6fe1-4be9-bffa-d5af76058a2b" Name="CompletionConditionName" Type="String(128) Null" ReferencedColumn="3f8b60eb-e7a0-43cf-9df2-0862fd54d4c6" />
		<SchemeReferencingColumn ID="75987729-d1b3-464c-b4dd-2671d681a035" Name="CompletionConditionCaption" Type="String(128) Null" ReferencedColumn="a3c60540-fb02-45c9-8bc6-8106cbd5ed77" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn ID="47c442c8-2483-4c36-a653-67a84dc46851" Name="NeedToStartNextStage" Type="Boolean Not Null">
		<Description>Флаг необходимости запуска следующего этапа</Description>
		<SchemeDefaultConstraint IsPermanent="true" ID="2e5eabd0-0335-4309-90b9-d5ccd77e8e6e" Name="df_FdStageTemplateCompletions_NeedToStartNextStage" Value="false" />
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="eff3b29c-97ae-4587-9091-5745dd0c5e2c" Name="NeedToCompleteProcess" Type="Boolean Not Null">
		<Description>Флаг необходимости завершения процесса</Description>
		<SchemeDefaultConstraint IsPermanent="true" ID="d5977109-5cc1-4bc2-995f-8161a0b12568" Name="df_FdStageTemplateCompletions_NeedToCompleteProcess" Value="false" />
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="27396ba9-d0e1-41b8-9a0d-81d6a6e6df2a" Name="NeedToStartNextProcesses" Type="Boolean Not Null">
		<Description>Флаг необходимости запуска новых процессов</Description>
		<SchemeDefaultConstraint IsPermanent="true" ID="3b91ec9e-15bf-4793-8356-a78efacac96d" Name="df_FdStageTemplateCompletions_NeedToStartNextProcesses" Value="false" />
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="cd05ae43-2692-4d85-8150-f0fc05f8f875" Name="NeedToChangeCardState" Type="Boolean Not Null">
		<Description>Флаг необходимости смены состояния карточки</Description>
		<SchemeDefaultConstraint IsPermanent="true" ID="d5c49b90-6696-4b9c-b241-174829ceb33f" Name="df_FdStageTemplateCompletions_NeedToChangeCardState" Value="false" />
	</SchemePhysicalColumn>
	<SchemeComplexColumn ID="b204d1c9-a2fa-4b7f-86f8-d5baac030193" Name="NextCardState" Type="Reference(Typified) Null" ReferencedTable="c5e015fe-1b55-4a18-8787-074b5d2ec80c">
		<Description>Следующее состояние карточки</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="b204d1c9-a2fa-007f-4000-05baac030193" Name="NextCardStateID" Type="Guid Null" ReferencedColumn="c5e015fe-1b55-0118-4000-074b5d2ec80c" />
		<SchemeReferencingColumn ID="733b5d41-8111-4f09-a9e0-d79c5b7f95f0" Name="NextCardStateName" Type="String(128) Null" ReferencedColumn="09d1dfdb-262f-4b6e-b14d-64e5f51b7fa7" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn ID="b5f431db-079a-4aa3-a983-b2d7eee39f83" Name="NeedToStartNextStageByCriterias" Type="Boolean Not Null">
		<SchemeDefaultConstraint IsPermanent="true" ID="9b82fdc9-2397-46c1-aa23-ffda292d7213" Name="df_FdStageTemplateCompletions_NeedToStartNextStageByCriterias" Value="false" />
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="97a09aa3-c465-4284-b474-8df1823c4774" Name="CompletionText" Type="String(Max) Null">
		<Description>Общее текстовое представление параметра завершения (для вывода пользователю)</Description>
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="70e441bb-4f9e-4ec9-9b24-1c36ad523bb4" Name="NeedToCompleteProcessWhenAllStagesCompleted" Type="Boolean Not Null">
		<Description>Завершение процесса в случае, если нет незавершенных этапов</Description>
		<SchemeDefaultConstraint IsPermanent="true" ID="61e5ee94-20ad-4d16-9c07-7cdbae51d508" Name="df_FdStageTemplateCompletions_NeedToCompleteProcessWhenAllStagesCompleted" Value="false" />
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="a02f4160-234e-465b-8529-d0c76b1353c8" Name="NeedToCancelProcessStart" Type="Boolean Not Null">
		<Description>Отмена запуска процесса (имеет смысл для типа этапа "Запуск процесса")</Description>
		<SchemeDefaultConstraint IsPermanent="true" ID="373243de-8abc-485d-82f2-9c6ee76ab9bc" Name="df_FdStageTemplateCompletions_NeedToCancelProcessStart" Value="false" />
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="6c57197c-48c8-4a58-a6ae-56ebe55ad24a" Name="NeedToRevokeActiveProcesses" Type="Boolean Not Null">
		<SchemeDefaultConstraint IsPermanent="true" ID="e5d744e4-3b06-43f2-918e-5413363578c6" Name="df_FdStageTemplateCompletions_NeedToRevokeActiveProcesses" Value="false" />
	</SchemePhysicalColumn>
	<SchemeComplexColumn ID="85a15eb5-a26a-4556-9804-bf2f6ad02de4" Name="TaskTypeRoleField" Type="Reference(Typified) Null" ReferencedTable="d30606d0-3bdd-4c18-b6ee-5d8d19cfb333">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="85a15eb5-a26a-0056-4000-0f2f6ad02de4" Name="TaskTypeRoleFieldID" Type="Guid Null" ReferencedColumn="d30606d0-3bdd-0118-4000-0d8d19cfb333" />
		<SchemeReferencingColumn ID="243465de-b50b-459d-88cd-d6ab87689f6b" Name="TaskTypeRoleFieldName" Type="String(Max) Null" ReferencedColumn="d21b0bc8-ff67-48ee-8d18-cff15549a309" />
		<SchemeReferencingColumn ID="8fa3435e-ceaa-449d-954e-15546f79eb8d" Name="TaskTypeRoleFieldControlTypeID" Type="Guid Null" ReferencedColumn="70ca101a-e2e3-4ca6-bf91-d8df5cd78ecd" />
		<SchemeReferencingColumn ID="25a0e02b-1b8a-47fe-8cf5-df36817e6f6c" Name="TaskTypeRoleFieldControlTypeName" Type="String(Max) Null" ReferencedColumn="2e0e0121-ab28-4b59-bb45-4974a54ab3e0" />
		<SchemeReferencingColumn ID="04cd589a-b731-49cb-bff7-b9d8fd0b04f4" Name="TaskTypeRoleFieldSectionID" Type="Guid Null" ReferencedColumn="3063c6fd-b546-4368-86f1-6d78c7238201" />
		<SchemeReferencingColumn ID="99c76d7b-5b2a-422e-abec-120c84fc43ad" Name="TaskTypeRoleFieldSectionName" Type="String(Max) Null" ReferencedColumn="fb17f159-853e-461a-849d-47dd60090e8a" />
		<SchemeReferencingColumn ID="0c14bb4a-1420-490a-95ab-426fde185e46" Name="TaskTypeRoleFieldPhysicalColumnID" Type="Guid Null" ReferencedColumn="577c0a40-af09-40f6-b262-dc64ee10649b" />
		<SchemeReferencingColumn ID="06495d50-4d21-4652-8381-200b998407e6" Name="TaskTypeRoleFieldComplexColumnID" Type="Guid Null" ReferencedColumn="f65e0789-478e-42d3-bb01-b51ebffabb37" />
		<SchemeReferencingColumn ID="226c9c1f-760d-4f0c-938f-0301c100521c" Name="TaskTypeRoleFieldColumnName" Type="String(Max) Null" ReferencedColumn="5dbe1a72-b58a-41b5-9d89-6755023f4938" />
		<SchemeReferencingColumn ID="1ba5637e-37f1-4f9b-9e1c-60c27326a910" Name="TaskTypeRoleFieldReferencedTableID" Type="Guid Null" ReferencedColumn="4ce6bdbf-395e-418f-a59f-97a3c9daace0" />
		<SchemeReferencingColumn ID="e547eadc-c8e8-446c-8aba-d059a05edaac" Name="TaskTypeRoleFieldReferencedTableName" Type="String(Max) Null" ReferencedColumn="333d1847-c2da-4fd1-97e0-c4f6426be594" />
		<SchemeReferencingColumn ID="da865e9e-9137-4cfc-8981-c14058f6edde" Name="TaskTypeRoleFieldSpecialMode" Type="Int32 Null" ReferencedColumn="fc5e528e-67d6-4b8c-80f9-70c08bcbdddd" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn ID="e9e0014f-3054-44d2-8410-5225d3375137" Name="PerformCheckAfterEachTaskOrSubstageComplete" Type="Boolean Not Null">
		<Description>Нужно ли выполнять проверку параметра завершения после завершения каждого задания или подэтапа внутри этапа</Description>
		<SchemeDefaultConstraint IsPermanent="true" ID="80738ba8-d6e9-4e34-8b58-b3ca06a72646" Name="df_FdStageTemplateCompletions_PerformCheckAfterEachTaskOrSubstageComplete" Value="false" />
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="7c922f3b-4db4-4a62-807a-b8e7ba56c4be" Name="Description" Type="String(Max) Null" />
	<SchemePrimaryKey IsSystem="true" IsPermanent="true" IsSealed="true" ID="2b1781ba-09e1-00a0-5000-0e4caa9abb81" Name="pk_FdStageTemplateCompletions">
		<SchemeIndexedColumn Column="2b1781ba-09e1-00a0-3100-0e4caa9abb81" />
	</SchemePrimaryKey>
	<SchemeIndex IsSystem="true" IsPermanent="true" IsSealed="true" ID="2b1781ba-09e1-00a0-7000-0e4caa9abb81" Name="idx_FdStageTemplateCompletions_ID" IsClustered="true">
		<SchemeIndexedColumn Column="2b1781ba-09e1-01a0-4000-0e4caa9abb81" />
	</SchemeIndex>
</SchemeTable>