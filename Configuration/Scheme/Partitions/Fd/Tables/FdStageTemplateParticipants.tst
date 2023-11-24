<?xml version="1.0" encoding="utf-8"?>
<SchemeTable Partition="c3b14969-a553-4005-9a07-e9705c15b1e1" ID="b9d1e0af-06c0-4c17-9e83-0497464e193b" Name="FdStageTemplateParticipants" Group="Fd" InstanceType="Cards" ContentType="Collections">
	<Description>Участники этапа</Description>
	<SchemeComplexColumn IsSystem="true" IsPermanent="true" IsSealed="true" ID="b9d1e0af-06c0-0017-2000-0497464e193b" Name="ID" Type="Reference(Typified) Not Null" ReferencedTable="1074eadd-21d7-4925-98c8-40d1e5f0ca0e">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="b9d1e0af-06c0-0117-4000-0497464e193b" Name="ID" Type="Guid Not Null" ReferencedColumn="9a58123b-b2e9-4137-9c6c-5dab0ec02747" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn IsSystem="true" IsPermanent="true" IsSealed="true" ID="b9d1e0af-06c0-0017-3100-0497464e193b" Name="RowID" Type="Guid Not Null" />
	<SchemeComplexColumn ID="67b5fdc9-f244-4b85-b7a8-9fb038e97b4f" Name="RoleParticipant" Type="Reference(Typified) Null" ReferencedTable="81f6010b-9641-4aa5-8897-b8e8603fbf4b">
		<Description></Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="67b5fdc9-f244-0085-4000-0fb038e97b4f" Name="RoleParticipantID" Type="Guid Null" ReferencedColumn="81f6010b-9641-01a5-4000-08e8603fbf4b" />
		<SchemeReferencingColumn ID="602b66a7-de9b-402f-bfd8-0934ab9acbc1" Name="RoleParticipantName" Type="String(128) Null" ReferencedColumn="616d6b2e-35d5-424d-846b-618eb25962d0" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn ID="8d4a155b-9053-47ac-9015-f87105e23ea2" Name="Order" Type="Int32 Not Null">
		<SchemeDefaultConstraint IsPermanent="true" ID="1929ff77-3f50-4152-84e8-e151c70c05a0" Name="df_FdStageTemplateParticipants_Order" Value="0" />
	</SchemePhysicalColumn>
	<SchemeComplexColumn ID="5331fff2-a14b-4f50-9c08-67590fbc43ca" Name="ParticipantType" Type="Reference(Typified) Not Null" ReferencedTable="11b2bc21-e3b5-4e24-8e55-d07a8bc9b2f5">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="5331fff2-a14b-0050-4000-07590fbc43ca" Name="ParticipantTypeID" Type="Int32 Not Null" ReferencedColumn="ea53336b-92a4-440c-914f-7a536e0c5806">
			<SchemeDefaultConstraint IsPermanent="true" ID="fa856796-b6f7-4301-a923-1ca5e72d20ca" Name="df_FdStageTemplateParticipants_ParticipantTypeID" Value="0" />
		</SchemeReferencingColumn>
		<SchemeReferencingColumn ID="90f0e81f-4d99-4bc7-b4b6-4a9b70919b49" Name="ParticipantTypeName" Type="String(128) Not Null" ReferencedColumn="51fc32bc-cf78-40b3-9d36-3bc97b60ac12">
			<SchemeDefaultConstraint IsPermanent="true" ID="f95cb58c-6267-4dd8-8e91-ac47a2a66f01" Name="df_FdStageTemplateParticipants_ParticipantTypeName" Value="Роль" />
		</SchemeReferencingColumn>
	</SchemeComplexColumn>
	<SchemePhysicalColumn ID="86be1d09-8d74-41cd-990a-45a6eea62a22" Name="ParticipantText" Type="String(128) Null" />
	<SchemePhysicalColumn ID="0365d1a0-3a68-4fff-8fb4-b17587eb6ac6" Name="CreateIndividualTaskForEachUserInRole" Type="Boolean Not Null">
		<Description>Флаг, нужно ли создавать отдельное задание для каждого пользователя внутри роли-участнике</Description>
		<SchemeDefaultConstraint IsPermanent="true" ID="bb4358de-f411-4bfd-8f67-a98391e04dfa" Name="df_FdStageTemplateParticipants_CreateIndividualTaskForEachUserInRole" Value="false" />
	</SchemePhysicalColumn>
	<SchemeComplexColumn ID="ac23a364-440c-48e2-8d4e-e7593ab95504" Name="CardFieldParticipant" Type="Reference(Typified) Null" ReferencedTable="d30606d0-3bdd-4c18-b6ee-5d8d19cfb333">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="ac23a364-440c-00e2-4000-07593ab95504" Name="CardFieldParticipantID" Type="Guid Null" ReferencedColumn="d30606d0-3bdd-0118-4000-0d8d19cfb333" />
		<SchemeReferencingColumn ID="6cda54be-6b66-43cf-85f7-f208440ce33f" Name="CardFieldParticipantName" Type="String(Max) Null" ReferencedColumn="d21b0bc8-ff67-48ee-8d18-cff15549a309" />
		<SchemeReferencingColumn ID="2807a4b8-c1b3-4d8c-b69a-7ed2901e5bee" Name="CardFieldParticipantControlTypeID" Type="Guid Null" ReferencedColumn="70ca101a-e2e3-4ca6-bf91-d8df5cd78ecd" />
		<SchemeReferencingColumn ID="d178c13d-6032-49f5-ab2c-51373b869f04" Name="CardFieldParticipantControlTypeName" Type="String(Max) Null" ReferencedColumn="2e0e0121-ab28-4b59-bb45-4974a54ab3e0" />
		<SchemeReferencingColumn ID="d169a78b-ce82-4915-a169-4ba2e9efe2af" Name="CardFieldParticipantSectionID" Type="Guid Null" ReferencedColumn="3063c6fd-b546-4368-86f1-6d78c7238201" />
		<SchemeReferencingColumn ID="07c31ae7-f532-4d2a-a788-c3acca4a3df1" Name="CardFieldParticipantSectionName" Type="String(Max) Null" ReferencedColumn="fb17f159-853e-461a-849d-47dd60090e8a" />
		<SchemeReferencingColumn ID="9971a132-e1d8-4f5c-a2b7-37f4bccff249" Name="CardFieldParticipantPhysicalColumnID" Type="Guid Null" ReferencedColumn="577c0a40-af09-40f6-b262-dc64ee10649b" />
		<SchemeReferencingColumn ID="c5a02382-fa42-4efd-bf5d-763552b2fb72" Name="CardFieldParticipantComplexColumnID" Type="Guid Null" ReferencedColumn="f65e0789-478e-42d3-bb01-b51ebffabb37" />
		<SchemeReferencingColumn ID="79c4b659-460b-4d6e-9d1a-832f4f730152" Name="CardFieldParticipantColumnName" Type="String(Max) Null" ReferencedColumn="5dbe1a72-b58a-41b5-9d89-6755023f4938" />
		<SchemeReferencingColumn ID="7cff407e-6cfe-4ae2-bbd3-ac497bca87e5" Name="CardFieldParticipantReferencedTableID" Type="Guid Null" ReferencedColumn="4ce6bdbf-395e-418f-a59f-97a3c9daace0" />
		<SchemeReferencingColumn ID="9b85a479-085a-4e5a-bc43-0510f5237b3f" Name="CardFieldParticipantReferencedTableName" Type="String(Max) Null" ReferencedColumn="333d1847-c2da-4fd1-97e0-c4f6426be594" />
		<SchemeReferencingColumn ID="e783f3d4-9541-48c6-b7ff-e7f71224bdd0" Name="CardFieldParticipantSpecialMode" Type="Int32 Null" ReferencedColumn="fc5e528e-67d6-4b8c-80f9-70c08bcbdddd" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn ID="13626d00-7582-4e72-bb89-227476ccd718" Name="CheckAtLeastOneCriteriaFit" Type="Boolean Not Null">
		<SchemeDefaultConstraint IsPermanent="true" ID="1151b2b5-34e2-4326-8fd7-a6adf5f62f24" Name="df_FdStageTemplateParticipants_CheckAtLeastOneCriteriaFit" Value="false" />
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="5338a988-0f97-4b58-9672-4e2b5adc0ee5" Name="TaskTimeLimitationInWorkingDaysNormal" Type="Double Null">
		<Description>Время на одно задание в рабочих днях</Description>
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="5def692d-0b38-4d9e-9ea9-e2c7c921d6e1" Name="TaskTimeLimitationInWorkingHoursNormal" Type="Double Null" />
	<SchemePhysicalColumn ID="adb0c239-dd57-4e5d-a68f-cfdfe79c64e2" Name="TaskTimeLimitationInWorkingDaysUrgent" Type="Double Null" />
	<SchemePhysicalColumn ID="92475208-fee7-4fed-ae2a-b47cb212db6e" Name="TaskTimeLimitationInWorkingHoursUrgent" Type="Double Null" />
	<SchemePhysicalColumn ID="43f856c9-2572-4b0d-b17a-40b4f6106786" Name="IgnoreOtherParticipantsWhenCriteriasFit" Type="Boolean Not Null">
		<Description>Остановить обработку если критерии выполнены успешно</Description>
		<SchemeDefaultConstraint IsPermanent="true" ID="aa1055fc-3e92-4028-8ef0-2c25680cc5b2" Name="df_FdStageTemplateParticipants_IgnoreOtherParticipantsWhenCriteriasFit" Value="false" />
	</SchemePhysicalColumn>
	<SchemeComplexColumn ID="c3589eb2-2715-4c8a-a1a9-33c23fee46c0" Name="Notification" Type="Reference(Typified) Null" ReferencedTable="18145bb5-fd4e-4795-aa1f-9e1cd9b4ee5a">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="c3589eb2-2715-008a-4000-03c23fee46c0" Name="NotificationID" Type="Guid Null" ReferencedColumn="18145bb5-fd4e-0195-4000-0e1cd9b4ee5a" />
		<SchemeReferencingColumn ID="07752531-988f-4728-8dd8-dd36294587f3" Name="NotificationName" Type="String(256) Null" ReferencedColumn="265d4336-6764-4db8-8874-0e5fa92cbd5d" />
	</SchemeComplexColumn>
	<SchemeComplexColumn ID="8efbc012-4cd6-42b3-be5a-6775733324bb" Name="SpecialRoleField" Type="Reference(Typified) Null" ReferencedTable="d30606d0-3bdd-4c18-b6ee-5d8d19cfb333">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="8efbc012-4cd6-00b3-4000-0775733324bb" Name="SpecialRoleFieldID" Type="Guid Null" ReferencedColumn="d30606d0-3bdd-0118-4000-0d8d19cfb333" />
		<SchemeReferencingColumn ID="ab6b2034-c96f-4d39-bbd9-5d4a0193fbdb" Name="SpecialRoleFieldName" Type="String(Max) Null" ReferencedColumn="d21b0bc8-ff67-48ee-8d18-cff15549a309" />
		<SchemeReferencingColumn ID="5e09bd28-b920-45aa-a780-33b0707eedad" Name="SpecialRoleFieldControlTypeID" Type="Guid Null" ReferencedColumn="70ca101a-e2e3-4ca6-bf91-d8df5cd78ecd" />
		<SchemeReferencingColumn ID="b214d54b-7ea8-497a-bb99-e76aff990eb6" Name="SpecialRoleFieldControlTypeName" Type="String(Max) Null" ReferencedColumn="2e0e0121-ab28-4b59-bb45-4974a54ab3e0" />
		<SchemeReferencingColumn ID="f76a5eb2-5cbb-4b77-8475-6134b7572325" Name="SpecialRoleFieldSectionID" Type="Guid Null" ReferencedColumn="3063c6fd-b546-4368-86f1-6d78c7238201" />
		<SchemeReferencingColumn ID="0862d139-df9b-499a-8943-62fb29f74d3e" Name="SpecialRoleFieldSectionName" Type="String(Max) Null" ReferencedColumn="fb17f159-853e-461a-849d-47dd60090e8a" />
		<SchemeReferencingColumn ID="aaa5f585-ef85-4545-a565-21c8b7bf0e53" Name="SpecialRoleFieldPhysicalColumnID" Type="Guid Null" ReferencedColumn="577c0a40-af09-40f6-b262-dc64ee10649b" />
		<SchemeReferencingColumn ID="7ee5bec5-9146-408c-8e7f-d378e7fb967f" Name="SpecialRoleFieldComplexColumnID" Type="Guid Null" ReferencedColumn="f65e0789-478e-42d3-bb01-b51ebffabb37" />
		<SchemeReferencingColumn ID="9aa151d4-2c11-4096-b8e1-ad1ac153bce1" Name="SpecialRoleFieldColumnName" Type="String(Max) Null" ReferencedColumn="5dbe1a72-b58a-41b5-9d89-6755023f4938" />
		<SchemeReferencingColumn ID="46bffdb4-a727-4d74-8f01-d821007e13f0" Name="SpecialRoleFieldReferencedTableID" Type="Guid Null" ReferencedColumn="4ce6bdbf-395e-418f-a59f-97a3c9daace0" />
		<SchemeReferencingColumn ID="92604ce7-be1b-431f-bcd9-ea8ce8a59986" Name="SpecialRoleFieldReferencedTableName" Type="String(Max) Null" ReferencedColumn="333d1847-c2da-4fd1-97e0-c4f6426be594" />
		<SchemeReferencingColumn ID="f9cdb309-b4fa-49fa-822e-940fc9a0bf72" Name="SpecialRoleFieldSpecialMode" Type="Int32 Null" ReferencedColumn="fc5e528e-67d6-4b8c-80f9-70c08bcbdddd" />
	</SchemeComplexColumn>
	<SchemeComplexColumn ID="aff69ad9-3bac-4f07-aff6-f8ba1ee95c0b" Name="SpecialRoleParticipantField" Type="Reference(Typified) Null" ReferencedTable="d30606d0-3bdd-4c18-b6ee-5d8d19cfb333">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="aff69ad9-3bac-0007-4000-08ba1ee95c0b" Name="SpecialRoleParticipantFieldID" Type="Guid Null" ReferencedColumn="d30606d0-3bdd-0118-4000-0d8d19cfb333" />
		<SchemeReferencingColumn ID="29bb417d-144d-4206-bae4-fecff50c9a85" Name="SpecialRoleParticipantFieldName" Type="String(Max) Null" ReferencedColumn="d21b0bc8-ff67-48ee-8d18-cff15549a309" />
		<SchemeReferencingColumn ID="49c2454f-0377-4b25-8f46-1a2dc6d35868" Name="SpecialRoleParticipantFieldControlTypeID" Type="Guid Null" ReferencedColumn="70ca101a-e2e3-4ca6-bf91-d8df5cd78ecd" />
		<SchemeReferencingColumn ID="f240f799-47b8-4177-83c8-a85cb8e8e482" Name="SpecialRoleParticipantFieldControlTypeName" Type="String(Max) Null" ReferencedColumn="2e0e0121-ab28-4b59-bb45-4974a54ab3e0" />
		<SchemeReferencingColumn ID="3aab6623-a05b-4fc0-9221-4234296751c3" Name="SpecialRoleParticipantFieldSectionID" Type="Guid Null" ReferencedColumn="3063c6fd-b546-4368-86f1-6d78c7238201" />
		<SchemeReferencingColumn ID="bffff371-122f-4168-bc98-bcbba9e812c6" Name="SpecialRoleParticipantFieldSectionName" Type="String(Max) Null" ReferencedColumn="fb17f159-853e-461a-849d-47dd60090e8a" />
		<SchemeReferencingColumn ID="9ed1a2ba-8b28-404f-8eea-17331546e2b5" Name="SpecialRoleParticipantFieldPhysicalColumnID" Type="Guid Null" ReferencedColumn="577c0a40-af09-40f6-b262-dc64ee10649b" />
		<SchemeReferencingColumn ID="d0199437-b932-4f0d-b154-7b058c057933" Name="SpecialRoleParticipantFieldComplexColumnID" Type="Guid Null" ReferencedColumn="f65e0789-478e-42d3-bb01-b51ebffabb37" />
		<SchemeReferencingColumn ID="cb5be4a4-c92e-488c-a09c-83439d7768b8" Name="SpecialRoleParticipantFieldColumnName" Type="String(Max) Null" ReferencedColumn="5dbe1a72-b58a-41b5-9d89-6755023f4938" />
		<SchemeReferencingColumn ID="262eae3f-fffc-4b02-a328-f19223039201" Name="SpecialRoleParticipantFieldReferencedTableID" Type="Guid Null" ReferencedColumn="4ce6bdbf-395e-418f-a59f-97a3c9daace0" />
		<SchemeReferencingColumn ID="a01993a5-3bff-4c6f-a9f0-6658bdf95ae3" Name="SpecialRoleParticipantFieldReferencedTableName" Type="String(Max) Null" ReferencedColumn="333d1847-c2da-4fd1-97e0-c4f6426be594" />
		<SchemeReferencingColumn ID="ed7860ce-82b3-4880-a82a-7f0bb20cf069" Name="SpecialRoleParticipantFieldSpecialMode" Type="Int32 Null" ReferencedColumn="fc5e528e-67d6-4b8c-80f9-70c08bcbdddd" />
	</SchemeComplexColumn>
	<SchemeComplexColumn ID="a0a7f670-c4ec-477f-b7c1-15638fb1ea61" Name="SpecialRoleValue" Type="Reference(Typified) Null" ReferencedTable="6e99ace1-78b1-4b05-8e71-f86598de608b">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="a0a7f670-c4ec-007f-4000-05638fb1ea61" Name="SpecialRoleValueID" Type="Guid Null" ReferencedColumn="6e99ace1-78b1-0105-4000-086598de608b" />
		<SchemeReferencingColumn ID="d5af04b3-cc90-41ea-b118-aa032e8a9c4a" Name="SpecialRoleValueName" Type="String(255) Null" ReferencedColumn="bf1d03e3-5964-4648-95ca-9ab82b541191" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn ID="e00f8630-d8a9-400c-8707-9e3cd0d1d03d" Name="SkipEmptyContextRoles" Type="Boolean Null">
		<Description>Исключать из этапа контекстные роли, входящие в участника, которые на момент расчета участников этапа не содержат ни одного сотрудника</Description>
		<SchemeDefaultConstraint IsPermanent="true" ID="c51e4b95-65c0-40c6-a357-a7b34d95dd68" Name="df_FdStageTemplateParticipants_SkipEmptyContextRoles" Value="true" />
	</SchemePhysicalColumn>
	<SchemePrimaryKey IsSystem="true" IsPermanent="true" IsSealed="true" ID="b9d1e0af-06c0-0017-5000-0497464e193b" Name="pk_FdStageTemplateParticipants">
		<SchemeIndexedColumn Column="b9d1e0af-06c0-0017-3100-0497464e193b" />
	</SchemePrimaryKey>
	<SchemeIndex IsSystem="true" IsPermanent="true" IsSealed="true" ID="b9d1e0af-06c0-0017-7000-0497464e193b" Name="idx_FdStageTemplateParticipants_ID" IsClustered="true">
		<SchemeIndexedColumn Column="b9d1e0af-06c0-0117-4000-0497464e193b" />
	</SchemeIndex>
</SchemeTable>