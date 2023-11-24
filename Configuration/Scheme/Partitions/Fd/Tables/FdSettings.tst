<?xml version="1.0" encoding="utf-8"?>
<SchemeTable Partition="c3b14969-a553-4005-9a07-e9705c15b1e1" ID="8847585d-bb60-40df-980f-7fa30593cd3a" Name="FdSettings" Group="Fd" InstanceType="Cards" ContentType="Entries">
	<SchemeComplexColumn IsSystem="true" IsPermanent="true" IsSealed="true" ID="8847585d-bb60-00df-2000-0fa30593cd3a" Name="ID" Type="Reference(Typified) Not Null" ReferencedTable="1074eadd-21d7-4925-98c8-40d1e5f0ca0e">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="8847585d-bb60-01df-4000-0fa30593cd3a" Name="ID" Type="Guid Not Null" ReferencedColumn="9a58123b-b2e9-4137-9c6c-5dab0ec02747" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn ID="f169a0bc-cef7-4c0d-a32a-5b02467333e4" Name="IsTaskNotificationsEnabled" Type="Boolean Not Null">
		<SchemeDefaultConstraint IsPermanent="true" ID="f5fca07d-6cb6-4ef0-b1bb-24aa9942a481" Name="df_FdSettings_IsTaskNotificationsEnabled" Value="true" />
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="94cecdd0-e09c-4d0b-a4c1-917e2b5b0150" Name="ApprovalListRoleUsersLimit" Type="Int32 Not Null">
		<Description>Максимальное число сотрудников, которые отображаются в ролях активных заданий в листе истории</Description>
		<SchemeDefaultConstraint IsPermanent="true" ID="033edae9-908e-4a05-aa4a-ef0ce4b28da2" Name="df_FdSettings_ApprovalListRoleUsersLimit" Value="0" />
	</SchemePhysicalColumn>
	<SchemePrimaryKey IsSystem="true" IsPermanent="true" IsSealed="true" ID="8847585d-bb60-00df-5000-0fa30593cd3a" Name="pk_FdSettings" IsClustered="true">
		<SchemeIndexedColumn Column="8847585d-bb60-01df-4000-0fa30593cd3a" />
	</SchemePrimaryKey>
</SchemeTable>