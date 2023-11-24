<?xml version="1.0" encoding="utf-8"?>
<SchemeTable Partition="c3b14969-a553-4005-9a07-e9705c15b1e1" ID="f071a859-de4d-4f81-9c4f-d57dfefb5a9d" Name="FdStageTemplateCompletion_NextProcesses" Group="Fd" InstanceType="Cards" ContentType="Collections">
	<Description>Список процессов, которые нужно запустить</Description>
	<SchemeComplexColumn IsSystem="true" IsPermanent="true" IsSealed="true" ID="f071a859-de4d-0081-2000-057dfefb5a9d" Name="ID" Type="Reference(Typified) Not Null" ReferencedTable="1074eadd-21d7-4925-98c8-40d1e5f0ca0e">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="f071a859-de4d-0181-4000-057dfefb5a9d" Name="ID" Type="Guid Not Null" ReferencedColumn="9a58123b-b2e9-4137-9c6c-5dab0ec02747" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn IsSystem="true" IsPermanent="true" IsSealed="true" ID="f071a859-de4d-0081-3100-057dfefb5a9d" Name="RowID" Type="Guid Not Null" />
	<SchemeComplexColumn ID="42094637-6548-40af-bf37-baa92a035cce" Name="StageTemplateCompletion" Type="Reference(Typified) Not Null" ReferencedTable="2b1781ba-09e1-42a0-9969-fe4caa9abb81" IsReferenceToOwner="true">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="42094637-6548-00af-4000-0aa92a035cce" Name="StageTemplateCompletionRowID" Type="Guid Not Null" ReferencedColumn="2b1781ba-09e1-00a0-3100-0e4caa9abb81" />
	</SchemeComplexColumn>
	<SchemeComplexColumn ID="c2c9345d-149c-4f0d-8fba-9ac5306e5ccb" Name="ProcessTemplate" Type="Reference(Typified) Not Null" ReferencedTable="7d88534f-8bce-4084-a150-64e9594d77c8" WithForeignKey="false">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="c2c9345d-149c-000d-4000-0ac5306e5ccb" Name="ProcessTemplateID" Type="Guid Not Null" ReferencedColumn="7d88534f-8bce-0184-4000-04e9594d77c8" />
		<SchemeReferencingColumn ID="42fb3cce-8c57-4cb4-a2ca-76b7b1a37348" Name="ProcessTemplateName" Type="String(255) Not Null" ReferencedColumn="40395010-1055-40c2-bc1e-a095c16b3517" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn ID="93a6ed63-4a4f-4be8-a3c8-1defc0d367e9" Name="Order" Type="Int32 Not Null">
		<SchemeDefaultConstraint IsPermanent="true" ID="bd7303ff-09eb-484f-9463-8cd6a3f803fa" Name="df_FdStageTemplateCompletion_NextProcesses_Order" Value="0" />
	</SchemePhysicalColumn>
	<SchemeComplexColumn ID="9cd32e9f-0179-40f0-8ffc-200df54e0f17" Name="TaskTypeRoleField" Type="Reference(Typified) Null" ReferencedTable="d30606d0-3bdd-4c18-b6ee-5d8d19cfb333">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="9cd32e9f-0179-00f0-4000-000df54e0f17" Name="TaskTypeRoleFieldID" Type="Guid Null" ReferencedColumn="d30606d0-3bdd-0118-4000-0d8d19cfb333" />
		<SchemeReferencingColumn ID="c10e3b0c-f773-4655-8b88-3a3b56c4c56d" Name="TaskTypeRoleFieldName" Type="String(Max) Null" ReferencedColumn="d21b0bc8-ff67-48ee-8d18-cff15549a309" />
		<SchemeReferencingColumn ID="02a2fdd6-4cfa-4467-acb6-1737cbb9700e" Name="TaskTypeRoleFieldControlTypeID" Type="Guid Null" ReferencedColumn="70ca101a-e2e3-4ca6-bf91-d8df5cd78ecd" />
		<SchemeReferencingColumn ID="787132a0-9bed-4aea-b2e4-e368bfb9c96e" Name="TaskTypeRoleFieldControlTypeName" Type="String(Max) Null" ReferencedColumn="2e0e0121-ab28-4b59-bb45-4974a54ab3e0" />
		<SchemeReferencingColumn ID="bae16ad1-7580-419a-8f27-a411ac74db1d" Name="TaskTypeRoleFieldSectionID" Type="Guid Null" ReferencedColumn="3063c6fd-b546-4368-86f1-6d78c7238201" />
		<SchemeReferencingColumn ID="6bc2f46b-795e-4006-8b76-4e20e8961efb" Name="TaskTypeRoleFieldSectionName" Type="String(Max) Null" ReferencedColumn="fb17f159-853e-461a-849d-47dd60090e8a" />
		<SchemeReferencingColumn ID="47ceda9f-1133-4d56-958a-e3d706de6300" Name="TaskTypeRoleFieldPhysicalColumnID" Type="Guid Null" ReferencedColumn="577c0a40-af09-40f6-b262-dc64ee10649b" />
		<SchemeReferencingColumn ID="0be45935-a071-467f-be17-75665754f0c0" Name="TaskTypeRoleFieldComplexColumnID" Type="Guid Null" ReferencedColumn="f65e0789-478e-42d3-bb01-b51ebffabb37" />
		<SchemeReferencingColumn ID="6bb06a25-497a-4c96-a1ab-9002d70f4ae4" Name="TaskTypeRoleFieldColumnName" Type="String(Max) Null" ReferencedColumn="5dbe1a72-b58a-41b5-9d89-6755023f4938" />
		<SchemeReferencingColumn ID="4a80d001-4efc-42ce-af4a-a6569ad840c4" Name="TaskTypeRoleFieldReferencedTableID" Type="Guid Null" ReferencedColumn="4ce6bdbf-395e-418f-a59f-97a3c9daace0" />
		<SchemeReferencingColumn ID="8e9303cc-b072-4f36-a838-34aac65892d5" Name="TaskTypeRoleFieldReferencedTableName" Type="String(Max) Null" ReferencedColumn="333d1847-c2da-4fd1-97e0-c4f6426be594" />
		<SchemeReferencingColumn ID="44aa7c73-0408-47f2-bba3-4c7a6eb08ab6" Name="TaskTypeRoleFieldSpecialMode" Type="Int32 Null" ReferencedColumn="fc5e528e-67d6-4b8c-80f9-70c08bcbdddd" />
	</SchemeComplexColumn>
	<SchemePrimaryKey IsSystem="true" IsPermanent="true" IsSealed="true" ID="f071a859-de4d-0081-5000-057dfefb5a9d" Name="pk_FdStageTemplateCompletion_NextProcesses">
		<SchemeIndexedColumn Column="f071a859-de4d-0081-3100-057dfefb5a9d" />
	</SchemePrimaryKey>
	<SchemeIndex IsSystem="true" IsPermanent="true" IsSealed="true" ID="f071a859-de4d-0081-7000-057dfefb5a9d" Name="idx_FdStageTemplateCompletion_NextProcesses_ID" IsClustered="true">
		<SchemeIndexedColumn Column="f071a859-de4d-0181-4000-057dfefb5a9d" />
	</SchemeIndex>
</SchemeTable>