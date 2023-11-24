<?xml version="1.0" encoding="utf-8"?>
<SchemeTable Partition="c3b14969-a553-4005-9a07-e9705c15b1e1" ID="36988d7d-1dc7-4120-b9a8-9c01b678a329" Name="FdCriteriaList_CriteriaValueTableString" Group="Fd Criteria" InstanceType="Cards" ContentType="Collections">
	<SchemeComplexColumn IsSystem="true" IsPermanent="true" IsSealed="true" ID="36988d7d-1dc7-0020-2000-0c01b678a329" Name="ID" Type="Reference(Typified) Not Null" ReferencedTable="1074eadd-21d7-4925-98c8-40d1e5f0ca0e">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="36988d7d-1dc7-0120-4000-0c01b678a329" Name="ID" Type="Guid Not Null" ReferencedColumn="9a58123b-b2e9-4137-9c6c-5dab0ec02747" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn IsSystem="true" IsPermanent="true" IsSealed="true" ID="36988d7d-1dc7-0020-3100-0c01b678a329" Name="RowID" Type="Guid Not Null" />
	<SchemePhysicalColumn ID="a161a803-ccfd-4884-b476-06a09c78a279" Name="Value" Type="String(Max) Not Null" />
	<SchemeComplexColumn ID="7c86a26a-873c-4dc1-9d79-5d9e2be95b7e" Name="CriteriaListRecord" Type="Reference(Typified) Null" ReferencedTable="38639b42-edcb-4e7e-a23e-cd722bf8e0ac" IsReferenceToOwner="true">
		<Description>Ссылка на родительскую коллекционную запись критерия этапа</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="7c86a26a-873c-00c1-4000-0d9e2be95b7e" Name="CriteriaListRecordRowID" Type="Guid Null" ReferencedColumn="38639b42-edcb-007e-3100-0d722bf8e0ac" />
	</SchemeComplexColumn>
	<SchemeComplexColumn ID="2e7a491e-c976-4c5a-8d67-2f1b31a93586" Name="ParticipantCriteriaListRecord" Type="Reference(Typified) Null" ReferencedTable="a7c5b23a-7e28-4f7a-8d1a-0488061003c8" IsReferenceToOwner="true">
		<Description>Ссылка на родительскую коллекционную запись критерия участника этапа</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="2e7a491e-c976-005a-4000-0f1b31a93586" Name="ParticipantCriteriaListRecordRowID" Type="Guid Null" ReferencedColumn="a7c5b23a-7e28-007a-3100-0488061003c8" />
	</SchemeComplexColumn>
	<SchemePrimaryKey IsSystem="true" IsPermanent="true" IsSealed="true" ID="36988d7d-1dc7-0020-5000-0c01b678a329" Name="pk_FdCriteriaList_CriteriaValueTableString">
		<SchemeIndexedColumn Column="36988d7d-1dc7-0020-3100-0c01b678a329" />
	</SchemePrimaryKey>
	<SchemeIndex IsSystem="true" IsPermanent="true" IsSealed="true" ID="36988d7d-1dc7-0020-7000-0c01b678a329" Name="idx_FdCriteriaList_CriteriaValueTableString_ID" IsClustered="true">
		<SchemeIndexedColumn Column="36988d7d-1dc7-0120-4000-0c01b678a329" />
	</SchemeIndex>
</SchemeTable>