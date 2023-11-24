<?xml version="1.0" encoding="utf-8"?>
<SchemeTable Partition="c3b14969-a553-4005-9a07-e9705c15b1e1" ID="085b8aac-3762-41e6-b4f6-26d1a7ba7eef" Name="FdProcessTemplate_ManageProcessRoles" Group="Fd" InstanceType="Cards" ContentType="Collections">
	<Description>Роли, которым доступно управление процессом</Description>
	<SchemeComplexColumn IsSystem="true" IsPermanent="true" IsSealed="true" ID="085b8aac-3762-00e6-2000-06d1a7ba7eef" Name="ID" Type="Reference(Typified) Not Null" ReferencedTable="1074eadd-21d7-4925-98c8-40d1e5f0ca0e">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="085b8aac-3762-01e6-4000-06d1a7ba7eef" Name="ID" Type="Guid Not Null" ReferencedColumn="9a58123b-b2e9-4137-9c6c-5dab0ec02747" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn IsSystem="true" IsPermanent="true" IsSealed="true" ID="085b8aac-3762-00e6-3100-06d1a7ba7eef" Name="RowID" Type="Guid Not Null" />
	<SchemeComplexColumn ID="397cc78d-a407-41bf-8782-7ed75c6fcc01" Name="Role" Type="Reference(Typified) Not Null" ReferencedTable="81f6010b-9641-4aa5-8897-b8e8603fbf4b">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="397cc78d-a407-00bf-4000-0ed75c6fcc01" Name="RoleID" Type="Guid Not Null" ReferencedColumn="81f6010b-9641-01a5-4000-08e8603fbf4b" />
		<SchemeReferencingColumn ID="e0b6cda5-d345-4887-9e5d-3ba17a1b1576" Name="RoleName" Type="String(128) Not Null" ReferencedColumn="616d6b2e-35d5-424d-846b-618eb25962d0" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn ID="bd97b26a-457b-48f3-8f0a-6db8bcb0120c" Name="CanStartProcess" Type="Boolean Not Null">
		<Description>Флаг, может ли роль запускать процесс</Description>
		<SchemeDefaultConstraint IsPermanent="true" ID="306b341e-60c9-4ea7-9ed1-6236e7f9459f" Name="df_FdProcessTemplate_ManageProcessRoles_CanStartProcess" Value="true" />
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="9900b523-9bda-4c97-88b1-874764531616" Name="CanRevokeProcess" Type="Boolean Not Null">
		<SchemeDefaultConstraint IsPermanent="true" ID="48287c5b-7398-41cd-8ab9-d97884802057" Name="df_FdProcessTemplate_ManageProcessRoles_CanRevokeProcess" Value="true" />
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="6fa3bdec-ee52-4a1c-b27d-0c9107974dca" Name="CanRestartFromStage" Type="Boolean Not Null">
		<SchemeDefaultConstraint IsPermanent="true" ID="43ca1484-9c7e-4cc1-a438-d44956a63766" Name="df_FdProcessTemplate_ManageProcessRoles_CanRestartFromStage" Value="false" />
	</SchemePhysicalColumn>
	<SchemePrimaryKey IsSystem="true" IsPermanent="true" IsSealed="true" ID="085b8aac-3762-00e6-5000-06d1a7ba7eef" Name="pk_FdProcessTemplate_ManageProcessRoles">
		<SchemeIndexedColumn Column="085b8aac-3762-00e6-3100-06d1a7ba7eef" />
	</SchemePrimaryKey>
	<SchemeIndex IsSystem="true" IsPermanent="true" IsSealed="true" ID="085b8aac-3762-00e6-7000-06d1a7ba7eef" Name="idx_FdProcessTemplate_ManageProcessRoles_ID" IsClustered="true">
		<SchemeIndexedColumn Column="085b8aac-3762-01e6-4000-06d1a7ba7eef" />
	</SchemeIndex>
</SchemeTable>