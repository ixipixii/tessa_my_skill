<?xml version="1.0" encoding="utf-8"?>
<SchemeTable Partition="c3b14969-a553-4005-9a07-e9705c15b1e1" ID="5f852b9f-69ae-456b-8060-5b1715976f01" Name="FdCriteriaList_CriteriaValueTableRoles" Group="Fd Criteria" InstanceType="Cards" ContentType="Collections">
	<Description>Список ролей</Description>
	<SchemeComplexColumn IsSystem="true" IsPermanent="true" IsSealed="true" ID="5f852b9f-69ae-006b-2000-0b1715976f01" Name="ID" Type="Reference(Typified) Not Null" ReferencedTable="1074eadd-21d7-4925-98c8-40d1e5f0ca0e">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="5f852b9f-69ae-016b-4000-0b1715976f01" Name="ID" Type="Guid Not Null" ReferencedColumn="9a58123b-b2e9-4137-9c6c-5dab0ec02747" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn IsSystem="true" IsPermanent="true" IsSealed="true" ID="5f852b9f-69ae-006b-3100-0b1715976f01" Name="RowID" Type="Guid Not Null" />
	<SchemeComplexColumn ID="b7cab4dc-6c79-4791-8ca3-447871b8afdd" Name="Role" Type="Reference(Typified) Not Null" ReferencedTable="81f6010b-9641-4aa5-8897-b8e8603fbf4b">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="b7cab4dc-6c79-0091-4000-047871b8afdd" Name="RoleID" Type="Guid Not Null" ReferencedColumn="81f6010b-9641-01a5-4000-08e8603fbf4b" />
		<SchemeReferencingColumn ID="499e1f9f-d59a-4425-ad69-50ec2aa64911" Name="RoleName" Type="String(128) Not Null" ReferencedColumn="616d6b2e-35d5-424d-846b-618eb25962d0" />
	</SchemeComplexColumn>
	<SchemeComplexColumn ID="f27f4393-4da5-4a5f-b57a-5809cb5577db" Name="CriteriaListRecord" Type="Reference(Typified) Null" ReferencedTable="38639b42-edcb-4e7e-a23e-cd722bf8e0ac" IsReferenceToOwner="true">
		<Description>Ссылка на родительскую коллекционную запись критерия этапа</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="f27f4393-4da5-005f-4000-0809cb5577db" Name="CriteriaListRecordRowID" Type="Guid Null" ReferencedColumn="38639b42-edcb-007e-3100-0d722bf8e0ac" />
	</SchemeComplexColumn>
	<SchemeComplexColumn ID="9d99cd9c-580e-4f6d-be9d-6e6d18261239" Name="ParticipantCriteriaListRecord" Type="Reference(Typified) Null" ReferencedTable="a7c5b23a-7e28-4f7a-8d1a-0488061003c8" IsReferenceToOwner="true">
		<Description>Ссылка на родительскую коллекционную запись критерия участника этапа</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="9d99cd9c-580e-006d-4000-0e6d18261239" Name="ParticipantCriteriaListRecordRowID" Type="Guid Null" ReferencedColumn="a7c5b23a-7e28-007a-3100-0488061003c8" />
	</SchemeComplexColumn>
	<SchemePrimaryKey IsSystem="true" IsPermanent="true" IsSealed="true" ID="5f852b9f-69ae-006b-5000-0b1715976f01" Name="pk_FdCriteriaList_CriteriaValueTableRoles">
		<SchemeIndexedColumn Column="5f852b9f-69ae-006b-3100-0b1715976f01" />
	</SchemePrimaryKey>
	<SchemeIndex IsSystem="true" IsPermanent="true" IsSealed="true" ID="5f852b9f-69ae-006b-7000-0b1715976f01" Name="idx_FdCriteriaList_CriteriaValueTableRoles_ID" IsClustered="true">
		<SchemeIndexedColumn Column="5f852b9f-69ae-016b-4000-0b1715976f01" />
	</SchemeIndex>
</SchemeTable>