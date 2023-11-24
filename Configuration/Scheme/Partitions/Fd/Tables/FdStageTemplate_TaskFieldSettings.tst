<?xml version="1.0" encoding="utf-8"?>
<SchemeTable Partition="c3b14969-a553-4005-9a07-e9705c15b1e1" ID="d86d8f69-59a4-4115-b5eb-6c6dc6bfbaab" Name="FdStageTemplate_TaskFieldSettings" Group="Fd Fields" InstanceType="Cards" ContentType="Collections">
	<SchemeComplexColumn IsSystem="true" IsPermanent="true" IsSealed="true" ID="d86d8f69-59a4-0015-2000-0c6dc6bfbaab" Name="ID" Type="Reference(Typified) Not Null" ReferencedTable="1074eadd-21d7-4925-98c8-40d1e5f0ca0e">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="d86d8f69-59a4-0115-4000-0c6dc6bfbaab" Name="ID" Type="Guid Not Null" ReferencedColumn="9a58123b-b2e9-4137-9c6c-5dab0ec02747" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn IsSystem="true" IsPermanent="true" IsSealed="true" ID="d86d8f69-59a4-0015-3100-0c6dc6bfbaab" Name="RowID" Type="Guid Not Null" />
	<SchemeComplexColumn ID="0ea3d41f-914c-444c-84b5-d87a011ba945" Name="StoreInCardTypeField" Type="Reference(Typified) Null" ReferencedTable="d30606d0-3bdd-4c18-b6ee-5d8d19cfb333" WithForeignKey="false">
		<Description>Поле карточки, в которое запишется значение из поля задания</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="0ea3d41f-914c-004c-4000-087a011ba945" Name="StoreInCardTypeFieldID" Type="Guid Null" ReferencedColumn="d30606d0-3bdd-0118-4000-0d8d19cfb333" />
		<SchemeReferencingColumn ID="d835420f-c4f0-4839-82ac-924247691faf" Name="StoreInCardTypeFieldName" Type="String(Max) Null" ReferencedColumn="d21b0bc8-ff67-48ee-8d18-cff15549a309" />
		<SchemeReferencingColumn ID="cd18df82-2d6f-4a33-a84f-2c3e6b135239" Name="StoreInCardTypeFieldControlTypeID" Type="Guid Null" ReferencedColumn="70ca101a-e2e3-4ca6-bf91-d8df5cd78ecd" />
		<SchemeReferencingColumn ID="ac697e9b-252a-4bf1-b2fe-9c77d0dd8289" Name="StoreInCardTypeFieldControlTypeName" Type="String(Max) Null" ReferencedColumn="2e0e0121-ab28-4b59-bb45-4974a54ab3e0" />
		<SchemeReferencingColumn ID="758b1180-1018-4f41-945f-dbf3027dd5f1" Name="StoreInCardTypeFieldSectionID" Type="Guid Null" ReferencedColumn="3063c6fd-b546-4368-86f1-6d78c7238201" />
		<SchemeReferencingColumn ID="ddd06561-ca2a-4564-8d10-40eac867c7df" Name="StoreInCardTypeFieldSectionName" Type="String(Max) Null" ReferencedColumn="fb17f159-853e-461a-849d-47dd60090e8a" />
		<SchemeReferencingColumn ID="2980f674-c4ad-4b3a-8ac7-1daf4ab2d678" Name="StoreInCardTypeFieldPhysicalColumnID" Type="Guid Null" ReferencedColumn="577c0a40-af09-40f6-b262-dc64ee10649b" />
		<SchemeReferencingColumn ID="d62840c7-d46d-4c02-966e-1cc256d9f4e5" Name="StoreInCardTypeFieldComplexColumnID" Type="Guid Null" ReferencedColumn="f65e0789-478e-42d3-bb01-b51ebffabb37" />
		<SchemeReferencingColumn ID="55deeccc-b1f0-4319-b6d1-a8d0d882fe46" Name="StoreInCardTypeFieldColumnName" Type="String(Max) Null" ReferencedColumn="5dbe1a72-b58a-41b5-9d89-6755023f4938" />
		<SchemeReferencingColumn ID="08d0d95f-9d49-4370-93a0-b73b0efbf810" Name="StoreInCardTypeFieldReferencedTableID" Type="Guid Null" ReferencedColumn="4ce6bdbf-395e-418f-a59f-97a3c9daace0" />
		<SchemeReferencingColumn ID="5ac6cc9f-c7f6-41b5-9f6b-08d1bbb6c1ef" Name="StoreInCardTypeFieldReferencedTableName" Type="String(Max) Null" ReferencedColumn="333d1847-c2da-4fd1-97e0-c4f6426be594" />
		<SchemeReferencingColumn ID="de8f16ad-58bc-4080-8380-030d289fba18" Name="StoreInCardTypeFieldSpecialMode" Type="Int32 Null" ReferencedColumn="fc5e528e-67d6-4b8c-80f9-70c08bcbdddd" />
	</SchemeComplexColumn>
	<SchemeComplexColumn ID="4bce1172-f99a-48d1-92a6-7f7c17e7e94e" Name="TaskTypeField" Type="Reference(Typified) Not Null" ReferencedTable="d30606d0-3bdd-4c18-b6ee-5d8d19cfb333" WithForeignKey="false">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="4bce1172-f99a-00d1-4000-0f7c17e7e94e" Name="TaskTypeFieldID" Type="Guid Not Null" ReferencedColumn="d30606d0-3bdd-0118-4000-0d8d19cfb333" />
		<SchemeReferencingColumn ID="7ecb0484-e597-4c03-885e-6ed92a03a0b9" Name="TaskTypeFieldName" Type="String(Max) Not Null" ReferencedColumn="d21b0bc8-ff67-48ee-8d18-cff15549a309" />
		<SchemeReferencingColumn ID="0da2570a-0b0e-4d91-898b-934ab8788026" Name="TaskTypeFieldControlTypeID" Type="Guid Null" ReferencedColumn="70ca101a-e2e3-4ca6-bf91-d8df5cd78ecd" />
		<SchemeReferencingColumn ID="a91c7214-80da-414e-a5fe-ecdf495fedff" Name="TaskTypeFieldControlTypeName" Type="String(Max) Null" ReferencedColumn="2e0e0121-ab28-4b59-bb45-4974a54ab3e0" />
		<SchemeReferencingColumn ID="0adf9701-544a-42b0-9349-6bb290bf68be" Name="TaskTypeFieldSectionID" Type="Guid Null" ReferencedColumn="3063c6fd-b546-4368-86f1-6d78c7238201" />
		<SchemeReferencingColumn ID="68dc5ff0-f181-4a42-996e-807c00ee0840" Name="TaskTypeFieldSectionName" Type="String(Max) Null" ReferencedColumn="fb17f159-853e-461a-849d-47dd60090e8a" />
		<SchemeReferencingColumn ID="ff9b543c-7d95-4d2c-bed9-2d438db9ad2e" Name="TaskTypeFieldPhysicalColumnID" Type="Guid Null" ReferencedColumn="577c0a40-af09-40f6-b262-dc64ee10649b" />
		<SchemeReferencingColumn ID="b0a93521-5040-40b1-8a26-0e141d6c7160" Name="TaskTypeFieldComplexColumnID" Type="Guid Null" ReferencedColumn="f65e0789-478e-42d3-bb01-b51ebffabb37" />
		<SchemeReferencingColumn ID="eaec1659-b943-44b3-a9a1-5f450f53170c" Name="TaskTypeFieldColumnName" Type="String(Max) Null" ReferencedColumn="5dbe1a72-b58a-41b5-9d89-6755023f4938" />
		<SchemeReferencingColumn ID="5a739697-0785-4358-996a-e5b6d8f974ad" Name="TaskTypeFieldReferencedTableID" Type="Guid Null" ReferencedColumn="4ce6bdbf-395e-418f-a59f-97a3c9daace0" />
		<SchemeReferencingColumn ID="328c63a6-37c6-4e6d-a8b8-ff5de5681271" Name="TaskTypeFieldReferencedTableName" Type="String(Max) Null" ReferencedColumn="333d1847-c2da-4fd1-97e0-c4f6426be594" />
		<SchemeReferencingColumn ID="99afe7c1-bd5f-4d8c-908f-9bd67729cbf3" Name="TaskTypeFieldSpecialMode" Type="Int32 Null" ReferencedColumn="fc5e528e-67d6-4b8c-80f9-70c08bcbdddd" />
	</SchemeComplexColumn>
	<SchemeComplexColumn ID="28167710-98b1-45cb-8215-79cdc20a8c56" Name="CompletionOption" Type="Reference(Typified) Null" ReferencedTable="08cf782d-4130-4377-8a49-3e201a05d496">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="28167710-98b1-00cb-4000-09cdc20a8c56" Name="CompletionOptionID" Type="Guid Null" ReferencedColumn="132dc5f5-ce87-4dd0-acce-b4a02acf7715" />
		<SchemeReferencingColumn ID="28befa8b-a79a-4104-b6e2-c99b1587461c" Name="CompletionOptionCaption" Type="String(128) Null" ReferencedColumn="6762309a-b0ff-4b2f-9cce-dd111116e554" />
	</SchemeComplexColumn>
	<SchemeComplexColumn ID="0a0171aa-2e9c-4cd3-bdd5-3c53e0a23e3b" Name="InitFromCardTypeField" Type="Reference(Typified) Null" ReferencedTable="d30606d0-3bdd-4c18-b6ee-5d8d19cfb333" WithForeignKey="false">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="0a0171aa-2e9c-00d3-4000-0c53e0a23e3b" Name="InitFromCardTypeFieldID" Type="Guid Null" ReferencedColumn="d30606d0-3bdd-0118-4000-0d8d19cfb333" />
		<SchemeReferencingColumn ID="b3966054-c5d2-4be3-b6a7-a52ab5e74cc5" Name="InitFromCardTypeFieldName" Type="String(Max) Null" ReferencedColumn="d21b0bc8-ff67-48ee-8d18-cff15549a309" />
		<SchemeReferencingColumn ID="7e19bf9f-61cd-477c-971a-3e6c3a7940f4" Name="InitFromCardTypeFieldControlTypeID" Type="Guid Null" ReferencedColumn="70ca101a-e2e3-4ca6-bf91-d8df5cd78ecd" />
		<SchemeReferencingColumn ID="ada0c616-497c-450d-90a2-7dbbc6617b3e" Name="InitFromCardTypeFieldControlTypeName" Type="String(Max) Null" ReferencedColumn="2e0e0121-ab28-4b59-bb45-4974a54ab3e0" />
		<SchemeReferencingColumn ID="70b03897-bb71-4606-b6bb-a8b86d04be6c" Name="InitFromCardTypeFieldSectionID" Type="Guid Null" ReferencedColumn="3063c6fd-b546-4368-86f1-6d78c7238201" />
		<SchemeReferencingColumn ID="eaa14338-578b-48d9-bd74-854aa3f00273" Name="InitFromCardTypeFieldSectionName" Type="String(Max) Null" ReferencedColumn="fb17f159-853e-461a-849d-47dd60090e8a" />
		<SchemeReferencingColumn ID="af7edf2d-74d4-4cde-888f-9518592b8ee1" Name="InitFromCardTypeFieldPhysicalColumnID" Type="Guid Null" ReferencedColumn="577c0a40-af09-40f6-b262-dc64ee10649b" />
		<SchemeReferencingColumn ID="cad18815-6fe3-421d-b9d1-6a65979d1707" Name="InitFromCardTypeFieldComplexColumnID" Type="Guid Null" ReferencedColumn="f65e0789-478e-42d3-bb01-b51ebffabb37" />
		<SchemeReferencingColumn ID="7a912374-4b73-43e6-8306-a50e0a0fc117" Name="InitFromCardTypeFieldColumnName" Type="String(Max) Null" ReferencedColumn="5dbe1a72-b58a-41b5-9d89-6755023f4938" />
		<SchemeReferencingColumn ID="c2d172a0-853a-45a4-a10f-79504ed24462" Name="InitFromCardTypeFieldReferencedTableID" Type="Guid Null" ReferencedColumn="4ce6bdbf-395e-418f-a59f-97a3c9daace0" />
		<SchemeReferencingColumn ID="74f8209a-009c-4351-8783-23133f0d075e" Name="InitFromCardTypeFieldReferencedTableName" Type="String(Max) Null" ReferencedColumn="333d1847-c2da-4fd1-97e0-c4f6426be594" />
		<SchemeReferencingColumn ID="09e0be5d-fb81-4d11-968f-1befd1b42ccf" Name="InitFromCardTypeFieldSpecialMode" Type="Int32 Null" ReferencedColumn="fc5e528e-67d6-4b8c-80f9-70c08bcbdddd" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn ID="205396f8-d96e-4250-b011-27e93551e076" Name="ClearCardCollectionSectionBeforeFilling" Type="Boolean Not Null">
		<Description>Нужно ли очистить коллекционную секцию карточки перед добавлением туда данных из поля задания</Description>
		<SchemeDefaultConstraint IsPermanent="true" ID="15e98413-ddcd-4891-84da-ffa1e41082f7" Name="df_FdStageTemplate_TaskFieldSettings_ClearCardCollectionSectionBeforeFilling" Value="false" />
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="83fbb137-db5c-42e7-8881-19e8089b0d8b" Name="IsRequired" Type="Boolean Not Null">
		<Description>Является ли поле обязательным для заполнения</Description>
		<SchemeDefaultConstraint IsPermanent="true" ID="3a7d121f-5dfb-4c13-9934-5cadeab93603" Name="df_FdStageTemplate_TaskFieldSettings_IsRequired" Value="false" />
	</SchemePhysicalColumn>
	<SchemePrimaryKey IsSystem="true" IsPermanent="true" IsSealed="true" ID="d86d8f69-59a4-0015-5000-0c6dc6bfbaab" Name="pk_FdStageTemplate_TaskFieldSettings">
		<SchemeIndexedColumn Column="d86d8f69-59a4-0015-3100-0c6dc6bfbaab" />
	</SchemePrimaryKey>
	<SchemeIndex IsSystem="true" IsPermanent="true" IsSealed="true" ID="d86d8f69-59a4-0015-7000-0c6dc6bfbaab" Name="idx_FdStageTemplate_TaskFieldSettings_ID" IsClustered="true">
		<SchemeIndexedColumn Column="d86d8f69-59a4-0115-4000-0c6dc6bfbaab" />
	</SchemeIndex>
</SchemeTable>