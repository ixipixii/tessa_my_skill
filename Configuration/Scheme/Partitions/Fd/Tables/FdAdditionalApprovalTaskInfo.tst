<?xml version="1.0" encoding="utf-8"?>
<SchemeTable Partition="c3b14969-a553-4005-9a07-e9705c15b1e1" ID="82137f18-2daf-4f86-a8bf-9c2867b75628" Name="FdAdditionalApprovalTaskInfo" Group="Fd" InstanceType="Tasks" ContentType="Entries">
	<SchemeComplexColumn IsSystem="true" IsPermanent="true" IsSealed="true" ID="82137f18-2daf-0086-2000-0c2867b75628" Name="ID" Type="Reference(Typified) Not Null" ReferencedTable="5bfa9936-bb5a-4e8f-89a9-180bfd8f75f8">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="82137f18-2daf-0186-4000-0c2867b75628" Name="ID" Type="Guid Not Null" ReferencedColumn="5bfa9936-bb5a-008f-3100-080bfd8f75f8" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn ID="af71e5f4-3115-4e7e-96e8-8a34eccbbd90" Name="Comment" Type="String(Max) Null" />
	<SchemeComplexColumn ID="046edfb4-fac1-486a-a687-c4feae9209bc" Name="AuthorRole" Type="Reference(Typified) Not Null" ReferencedTable="81f6010b-9641-4aa5-8897-b8e8603fbf4b" WithForeignKey="false">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="046edfb4-fac1-006a-4000-04feae9209bc" Name="AuthorRoleID" Type="Guid Not Null" ReferencedColumn="81f6010b-9641-01a5-4000-08e8603fbf4b" />
		<SchemeReferencingColumn ID="a4760dc7-9b45-4b6e-b976-3d948d9b2825" Name="AuthorRoleName" Type="String(128) Not Null" ReferencedColumn="616d6b2e-35d5-424d-846b-618eb25962d0" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn ID="072b222d-cd2f-4e08-bbe6-729429e3d2e1" Name="IsResponsible" Type="Boolean Not Null">
		<Description>Признак, что исполнитель установлен, как "Первый - ответсвенный".</Description>
		<SchemeDefaultConstraint IsPermanent="true" ID="de0bee9c-93fd-43a5-ace8-07492d5ab11f" Name="df_FdAdditionalApprovalTaskInfo_IsResponsible" Value="false" />
	</SchemePhysicalColumn>
	<SchemePrimaryKey IsSystem="true" IsPermanent="true" IsSealed="true" ID="82137f18-2daf-0086-5000-0c2867b75628" Name="pk_FdAdditionalApprovalTaskInfo" IsClustered="true">
		<SchemeIndexedColumn Column="82137f18-2daf-0186-4000-0c2867b75628" />
	</SchemePrimaryKey>
</SchemeTable>