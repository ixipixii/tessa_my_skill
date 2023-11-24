<?xml version="1.0" encoding="utf-8"?>
<SchemeTable Partition="c3b14969-a553-4005-9a07-e9705c15b1e1" ID="b5b0aba7-681e-4196-ac18-ee636eb6e4ac" Name="FdCriteriaList_CriteriaValueTableGuid" Group="Fd Criteria" InstanceType="Cards" ContentType="Collections">
	<SchemeComplexColumn IsSystem="true" IsPermanent="true" IsSealed="true" ID="b5b0aba7-681e-0096-2000-0e636eb6e4ac" Name="ID" Type="Reference(Typified) Not Null" ReferencedTable="1074eadd-21d7-4925-98c8-40d1e5f0ca0e">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="b5b0aba7-681e-0196-4000-0e636eb6e4ac" Name="ID" Type="Guid Not Null" ReferencedColumn="9a58123b-b2e9-4137-9c6c-5dab0ec02747" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn IsSystem="true" IsPermanent="true" IsSealed="true" ID="b5b0aba7-681e-0096-3100-0e636eb6e4ac" Name="RowID" Type="Guid Not Null" />
	<SchemeComplexColumn ID="8e0ba1e2-74ea-4a9a-a888-9b0990948737" Name="Value" Type="Reference(Abstract) Not Null" WithForeignKey="false">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="8e0ba1e2-74ea-009a-4000-0b0990948737" Name="ValueID" Type="Guid Not Null" ReferencedColumn="9a58123b-b2e9-4137-9c6c-5dab0ec02747" />
		<SchemePhysicalColumn ID="ab252960-3275-4084-bd17-7938acc75387" Name="ValueName" Type="String(128) Not Null" />
	</SchemeComplexColumn>
	<SchemeComplexColumn ID="805c3f86-92fa-4cea-b599-9b84c7b056c1" Name="CriteriaListRecord" Type="Reference(Typified) Null" ReferencedTable="38639b42-edcb-4e7e-a23e-cd722bf8e0ac" IsReferenceToOwner="true">
		<Description>Ссылка на родительскую коллекционную запись критерия этапа</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="805c3f86-92fa-00ea-4000-0b84c7b056c1" Name="CriteriaListRecordRowID" Type="Guid Null" ReferencedColumn="38639b42-edcb-007e-3100-0d722bf8e0ac" />
	</SchemeComplexColumn>
	<SchemeComplexColumn ID="24c3b6d9-6e05-4685-9a02-8c4c71fd0b62" Name="ParticipantCriteriaListRecord" Type="Reference(Typified) Null" ReferencedTable="a7c5b23a-7e28-4f7a-8d1a-0488061003c8" IsReferenceToOwner="true">
		<Description>Ссылка на родительскую коллекционную запись критерия участника этапа</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="24c3b6d9-6e05-0085-4000-0c4c71fd0b62" Name="ParticipantCriteriaListRecordRowID" Type="Guid Null" ReferencedColumn="a7c5b23a-7e28-007a-3100-0488061003c8" />
	</SchemeComplexColumn>
	<SchemePrimaryKey IsSystem="true" IsPermanent="true" IsSealed="true" ID="b5b0aba7-681e-0096-5000-0e636eb6e4ac" Name="pk_FdCriteriaList_CriteriaValueTableGuid">
		<SchemeIndexedColumn Column="b5b0aba7-681e-0096-3100-0e636eb6e4ac" />
	</SchemePrimaryKey>
	<SchemeIndex IsSystem="true" IsPermanent="true" IsSealed="true" ID="b5b0aba7-681e-0096-7000-0e636eb6e4ac" Name="idx_FdCriteriaList_CriteriaValueTableGuid_ID" IsClustered="true">
		<SchemeIndexedColumn Column="b5b0aba7-681e-0196-4000-0e636eb6e4ac" />
	</SchemeIndex>
</SchemeTable>