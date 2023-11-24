<?xml version="1.0" encoding="utf-8"?>
<SchemeTable Partition="c3b14969-a553-4005-9a07-e9705c15b1e1" ID="323be76d-516a-4ee9-b6b0-e76391d70426" Name="FdStageTemplate" Group="Fd" InstanceType="Cards" ContentType="Entries">
	<SchemeComplexColumn IsSystem="true" IsPermanent="true" IsSealed="true" ID="323be76d-516a-00e9-2000-076391d70426" Name="ID" Type="Reference(Typified) Not Null" ReferencedTable="1074eadd-21d7-4925-98c8-40d1e5f0ca0e">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="323be76d-516a-01e9-4000-076391d70426" Name="ID" Type="Guid Not Null" ReferencedColumn="9a58123b-b2e9-4137-9c6c-5dab0ec02747" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn ID="7bc776b2-ac1d-4c81-b4b3-c52ecf5b1937" Name="Name" Type="String(255) Not Null" />
	<SchemePhysicalColumn ID="187dda2f-baff-41c0-b056-78778a5a82ce" Name="Order" Type="Int32 Not Null">
		<SchemeDefaultConstraint IsPermanent="true" ID="a7e1bf23-b9ff-470e-9775-95e7b528c3b2" Name="df_FdStageTemplate_Order" Value="0" />
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="b03bac95-801a-4490-87e1-4f7a3643ae68" Name="IsParallel" Type="Boolean Not Null">
		<SchemeDefaultConstraint IsPermanent="true" ID="711b35a4-62a4-4171-8683-62d5d24c21de" Name="df_FdStageTemplate_IsParallel" Value="false" />
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="9ef86b05-a368-4823-81a8-0bfdd19f8a5f" Name="Description" Type="String(Max) Null">
		<Description>Описание</Description>
	</SchemePhysicalColumn>
	<SchemeComplexColumn ID="8f0d723b-1c56-4a6d-91a9-5f3ca0908494" Name="ProcessTemplate" Type="Reference(Typified) Not Null" ReferencedTable="7d88534f-8bce-4084-a150-64e9594d77c8">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="8f0d723b-1c56-006d-4000-0f3ca0908494" Name="ProcessTemplateID" Type="Guid Not Null" ReferencedColumn="7d88534f-8bce-0184-4000-04e9594d77c8" />
		<SchemeReferencingColumn ID="e10ed52b-44e9-4d21-bcec-7f1cfbf888eb" Name="ProcessTemplateName" Type="String(255) Not Null" ReferencedColumn="40395010-1055-40c2-bc1e-a095c16b3517" />
	</SchemeComplexColumn>
	<SchemeComplexColumn ID="899fbf71-c364-4f05-b909-cf2495eaed11" Name="TaskType" Type="Reference(Typified) Null" ReferencedTable="b0538ece-8468-4d0b-8b4e-5a1d43e024db">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="899fbf71-c364-0005-4000-0f2495eaed11" Name="TaskTypeID" Type="Guid Null" ReferencedColumn="a628a864-c858-4200-a6b7-da78c8e6e1f4" />
		<SchemeReferencingColumn ID="2691ed9b-e4d5-40da-9244-1ea1b168b4ed" Name="TaskTypeCaption" Type="String(128) Null" ReferencedColumn="0a02451e-2e06-4001-9138-b4805e641afa" />
		<SchemeReferencingColumn ID="450fcd45-2e7b-4573-b2c0-f66d35ccf165" Name="TaskTypeName" Type="String(128) Null" ReferencedColumn="71181642-0d62-45f9-8ad8-ccec4bd4ce22">
			<SchemeDefaultConstraint IsPermanent="true" ID="b884c11b-fce2-407a-9ccd-1268e37fa80c" Name="df_FdStageTemplate_TaskTypeName" Value="-" />
		</SchemeReferencingColumn>
	</SchemeComplexColumn>
	<SchemePhysicalColumn ID="4e38b6b4-9659-41cb-a67e-dfe5788a7980" Name="TaskDigest" Type="String(512) Null" />
	<SchemePhysicalColumn ID="508d24aa-347c-4298-a579-a467a58a1c9b" Name="TaskTimeLimitationInWorkingDaysNormal" Type="Double Not Null">
		<Description>Время на одно задание в рабочих днях (обычный срок)</Description>
		<SchemeDefaultConstraint IsPermanent="true" ID="adf41998-8d3c-4bb6-bdd7-33c0f3a06e9d" Name="df_FdStageTemplate_TaskTimeLimitationInWorkingDaysNormal" Value="1" />
	</SchemePhysicalColumn>
	<SchemeComplexColumn ID="191e9e66-279c-4f92-a2f7-a44f7287fee2" Name="NewCardStateWhenStageStart" Type="Reference(Typified) Null" ReferencedTable="c5e015fe-1b55-4a18-8787-074b5d2ec80c">
		<Description>Состояние, которое нужно перевести карточку после в момент запуска этапа</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="191e9e66-279c-0092-4000-044f7287fee2" Name="NewCardStateWhenStageStartID" Type="Guid Null" ReferencedColumn="c5e015fe-1b55-0118-4000-074b5d2ec80c" />
		<SchemeReferencingColumn ID="38453cce-d130-4efd-9813-9f228dacd274" Name="NewCardStateWhenStageStartName" Type="String(128) Null" ReferencedColumn="09d1dfdb-262f-4b6e-b14d-64e5f51b7fa7" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn ID="f0c85377-8b65-4a49-aa6a-7b55df0af5c3" Name="ParticipantsText" Type="String(Max) Null" />
	<SchemeComplexColumn ID="7891dfa3-87be-4939-8a2d-913720e26eb9" Name="StageType" Type="Reference(Typified) Not Null" ReferencedTable="09be6b67-cf61-48b1-beeb-588e0505ad5b">
		<Description>Тип этапа</Description>
		<SchemeReferencingColumn IsPermanent="true" ID="461bc378-8021-4792-b8d0-3899c45bb48c" Name="StageTypeID" Type="Int32 Not Null" ReferencedColumn="c7b30411-85cf-4d3c-83a2-e8c0fa4a40bd" />
		<SchemeReferencingColumn ID="41adb4ef-054c-4be4-9d04-121106e25a67" Name="StageTypeName" Type="String(256) Not Null" ReferencedColumn="039f7f8a-cd50-4c49-b9ee-d5a92986794c" />
	</SchemeComplexColumn>
	<SchemeComplexColumn ID="dedec50b-fa90-4a6b-9467-7f0325b9130b" Name="FilesAutoLoadMode" Type="Reference(Typified) Null" ReferencedTable="35ca731c-cb73-4e14-a0d9-c5f0478c7b58">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="dedec50b-fa90-006b-4000-0f0325b9130b" Name="FilesAutoLoadModeID" Type="Int32 Null" ReferencedColumn="46ca0bdc-cc41-4f11-9d27-e155225ecaf6" />
		<SchemeReferencingColumn ID="ba3019ee-1ce9-4187-be15-97f34420d8c7" Name="FilesAutoLoadModeName" Type="String(128) Null" ReferencedColumn="d6fedd9e-1c5d-4176-ab4d-2aab042c9345" />
	</SchemeComplexColumn>
	<SchemeComplexColumn ID="89c4fa1c-9694-4233-92ba-5bed7023ef60" Name="TaskTimePlannedFromCardField" Type="Reference(Typified) Null" ReferencedTable="d30606d0-3bdd-4c18-b6ee-5d8d19cfb333">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="89c4fa1c-9694-0033-4000-0bed7023ef60" Name="TaskTimePlannedFromCardFieldID" Type="Guid Null" ReferencedColumn="d30606d0-3bdd-0118-4000-0d8d19cfb333" />
		<SchemeReferencingColumn ID="176e3ff6-970c-47b1-aeee-688477d278ff" Name="TaskTimePlannedFromCardFieldName" Type="String(Max) Null" ReferencedColumn="d21b0bc8-ff67-48ee-8d18-cff15549a309" />
		<SchemeReferencingColumn ID="af128881-aae6-4db0-b22a-16c48c804fe6" Name="TaskTimePlannedFromCardFieldControlTypeID" Type="Guid Null" ReferencedColumn="70ca101a-e2e3-4ca6-bf91-d8df5cd78ecd" />
		<SchemeReferencingColumn ID="8ca10a95-dff9-4a68-99c7-fa10c9adbb9f" Name="TaskTimePlannedFromCardFieldControlTypeName" Type="String(Max) Null" ReferencedColumn="2e0e0121-ab28-4b59-bb45-4974a54ab3e0" />
		<SchemeReferencingColumn ID="bcdbc2b9-a7aa-4fb6-ba19-a2734fa8e88d" Name="TaskTimePlannedFromCardFieldSectionID" Type="Guid Null" ReferencedColumn="3063c6fd-b546-4368-86f1-6d78c7238201" />
		<SchemeReferencingColumn ID="d01118b5-457d-4647-a669-5c33ad824f3c" Name="TaskTimePlannedFromCardFieldSectionName" Type="String(Max) Null" ReferencedColumn="fb17f159-853e-461a-849d-47dd60090e8a" />
		<SchemeReferencingColumn ID="942db7bb-c967-4097-9000-efeafb2218c3" Name="TaskTimePlannedFromCardFieldPhysicalColumnID" Type="Guid Null" ReferencedColumn="577c0a40-af09-40f6-b262-dc64ee10649b" />
		<SchemeReferencingColumn ID="f872800b-8cc9-404c-b5ff-eb3b1f3f5c27" Name="TaskTimePlannedFromCardFieldComplexColumnID" Type="Guid Null" ReferencedColumn="f65e0789-478e-42d3-bb01-b51ebffabb37" />
		<SchemeReferencingColumn ID="538c31b8-2e3a-401c-93c3-5301e293a4bb" Name="TaskTimePlannedFromCardFieldColumnName" Type="String(Max) Null" ReferencedColumn="5dbe1a72-b58a-41b5-9d89-6755023f4938" />
		<SchemeReferencingColumn ID="05cfb5e8-219c-463c-8589-cb396fdb9da3" Name="TaskTimePlannedFromCardFieldReferencedTableID" Type="Guid Null" ReferencedColumn="4ce6bdbf-395e-418f-a59f-97a3c9daace0" />
		<SchemeReferencingColumn ID="f799133a-7a7c-4e53-8407-cfdb9693594b" Name="TaskTimePlannedFromCardFieldReferencedTableName" Type="String(Max) Null" ReferencedColumn="333d1847-c2da-4fd1-97e0-c4f6426be594" />
		<SchemeReferencingColumn ID="edb1bfd4-2c42-4f9a-b051-357ff3f04b63" Name="TaskTimePlannedFromCardFieldSpecialMode" Type="Int32 Null" ReferencedColumn="fc5e528e-67d6-4b8c-80f9-70c08bcbdddd" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn ID="c0c24b8b-6eff-4120-9745-f3ec80437cf8" Name="CheckAtLeastOneCriteriaFit" Type="Boolean Not Null">
		<SchemeDefaultConstraint IsPermanent="true" ID="e0a5d370-d217-4409-89da-5773faecf2e6" Name="df_FdStageTemplate_CheckAtLeastOneCriteriaFit" Value="false" />
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="4567c082-4c05-467f-9698-d31e92858118" Name="TaskTimeLimitationInWorkingHoursNormal" Type="Double Not Null">
		<Description>Время на одно задание в рабочих часах (обычный срок)</Description>
		<SchemeDefaultConstraint IsPermanent="true" ID="e88af73e-5e77-4bd8-b26c-04cabbf42523" Name="df_FdStageTemplate_TaskTimeLimitationInWorkingHoursNormal" Value="0" />
	</SchemePhysicalColumn>
	<SchemeComplexColumn ID="e3ff7d32-c6c5-43b2-bf4f-ab46625b54d6" Name="UrgentFlagCardField" Type="Reference(Typified) Null" ReferencedTable="d30606d0-3bdd-4c18-b6ee-5d8d19cfb333">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="e3ff7d32-c6c5-00b2-4000-0b46625b54d6" Name="UrgentFlagCardFieldID" Type="Guid Null" ReferencedColumn="d30606d0-3bdd-0118-4000-0d8d19cfb333" />
		<SchemeReferencingColumn ID="aa429cd8-2229-46ef-aa85-3fb703bbd3cb" Name="UrgentFlagCardFieldName" Type="String(Max) Null" ReferencedColumn="d21b0bc8-ff67-48ee-8d18-cff15549a309" />
		<SchemeReferencingColumn ID="5f555977-39cc-4a83-96b1-a544287bb7bd" Name="UrgentFlagCardFieldControlTypeID" Type="Guid Null" ReferencedColumn="70ca101a-e2e3-4ca6-bf91-d8df5cd78ecd" />
		<SchemeReferencingColumn ID="4fbac847-598c-45fd-b9df-298ad278af61" Name="UrgentFlagCardFieldControlTypeName" Type="String(Max) Null" ReferencedColumn="2e0e0121-ab28-4b59-bb45-4974a54ab3e0" />
		<SchemeReferencingColumn ID="c8216c35-97c3-434c-b18a-5767abbee68c" Name="UrgentFlagCardFieldSectionID" Type="Guid Null" ReferencedColumn="3063c6fd-b546-4368-86f1-6d78c7238201" />
		<SchemeReferencingColumn ID="78752a68-a25c-4f6b-9e2b-9cd427fe937d" Name="UrgentFlagCardFieldSectionName" Type="String(Max) Null" ReferencedColumn="fb17f159-853e-461a-849d-47dd60090e8a" />
		<SchemeReferencingColumn ID="f97517fb-8b81-4fa6-928e-62cc544746fe" Name="UrgentFlagCardFieldPhysicalColumnID" Type="Guid Null" ReferencedColumn="577c0a40-af09-40f6-b262-dc64ee10649b" />
		<SchemeReferencingColumn ID="6b8d0922-4071-41da-96b7-9ad699c748cc" Name="UrgentFlagCardFieldComplexColumnID" Type="Guid Null" ReferencedColumn="f65e0789-478e-42d3-bb01-b51ebffabb37" />
		<SchemeReferencingColumn ID="c6a8394d-c77d-450c-a1dd-4659a2800ea3" Name="UrgentFlagCardFieldColumnName" Type="String(Max) Null" ReferencedColumn="5dbe1a72-b58a-41b5-9d89-6755023f4938" />
		<SchemeReferencingColumn ID="16edcf68-bbb0-4df5-b86e-4c271c09153c" Name="UrgentFlagCardFieldReferencedTableID" Type="Guid Null" ReferencedColumn="4ce6bdbf-395e-418f-a59f-97a3c9daace0" />
		<SchemeReferencingColumn ID="5d70d93e-10a5-411f-8f81-61a85c2ac893" Name="UrgentFlagCardFieldReferencedTableName" Type="String(Max) Null" ReferencedColumn="333d1847-c2da-4fd1-97e0-c4f6426be594" />
		<SchemeReferencingColumn ID="6629a5ab-1a2d-47e3-b7ea-802d89ee83d4" Name="UrgentFlagCardFieldSpecialMode" Type="Int32 Null" ReferencedColumn="fc5e528e-67d6-4b8c-80f9-70c08bcbdddd" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn ID="af853508-3d5e-4523-9f57-54e297d7479c" Name="TaskTimeLimitationInWorkingDaysUrgent" Type="Double Null">
		<Description>Время на одно задание в рабочих днях (срочный)</Description>
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="e1697435-5f18-47a1-a896-b77dd9471324" Name="TaskTimeLimitationInWorkingHoursUrgent" Type="Double Null">
		<Description>Время на одно задание в рабочих часах (срочный)</Description>
	</SchemePhysicalColumn>
	<SchemeComplexColumn ID="223a9ad1-938f-4850-bb7b-b462c2bb490e" Name="ParentStageTemplate" Type="Reference(Typified) Null" ReferencedTable="323be76d-516a-4ee9-b6b0-e76391d70426">
		<Description>Родительский шаблон этапа</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="223a9ad1-938f-0050-4000-0462c2bb490e" Name="ParentStageTemplateID" Type="Guid Null" ReferencedColumn="323be76d-516a-01e9-4000-076391d70426" />
		<SchemeReferencingColumn ID="98315f74-422a-4f88-b5ea-282b19e6273f" Name="ParentStageTemplateName" Type="String(255) Null" ReferencedColumn="7bc776b2-ac1d-4c81-b4b3-c52ecf5b1937" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn ID="32c5b403-b72e-4eef-880f-127c83700a3e" Name="IsAggregate" Type="Boolean Not Null">
		<SchemeDefaultConstraint IsPermanent="true" ID="9a373258-c894-4a36-9087-d96d45958f50" Name="df_FdStageTemplate_IsAggregate" Value="false" />
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="f1d1a7d6-87df-4f08-a0ce-161b162f527e" Name="AppropriateCompletionIsRequired" Type="Boolean Not Null">
		<Description>Выводить ошибку, если не найден параметр завершения</Description>
		<SchemeDefaultConstraint IsPermanent="true" ID="0fcd1963-18aa-4d5c-9839-b7d87b4a3bcd" Name="df_FdStageTemplate_AppropriateCompletionIsRequired" Value="false" />
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="34854180-1b9c-45f1-b34e-dcfff038f141" Name="IsEmailNotificationsEnabled" Type="Boolean Not Null">
		<Description>Отправлять участникам уведомления по заданиям на e-mail</Description>
		<SchemeDefaultConstraint IsPermanent="true" ID="ff3c7221-b63c-415d-b4e5-6ad4b0cc0678" Name="df_FdStageTemplate_IsEmailNotificationsEnabled" Value="true" />
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="bf2a9e38-94fb-44c3-b6c5-798e24f99308" Name="IsDistinctParticipantRoles" Type="Boolean Not Null">
		<SchemeDefaultConstraint IsPermanent="true" ID="f2cd98be-6982-4dd4-be6c-ebff0aa2fdc7" Name="df_FdStageTemplate_IsDistinctParticipantRoles" Value="true" />
	</SchemePhysicalColumn>
	<SchemePrimaryKey IsSystem="true" IsPermanent="true" IsSealed="true" ID="323be76d-516a-00e9-5000-076391d70426" Name="pk_FdStageTemplate" IsClustered="true">
		<SchemeIndexedColumn Column="323be76d-516a-01e9-4000-076391d70426" />
	</SchemePrimaryKey>
</SchemeTable>