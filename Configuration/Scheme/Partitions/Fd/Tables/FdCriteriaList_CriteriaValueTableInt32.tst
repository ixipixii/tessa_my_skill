<?xml version="1.0" encoding="utf-8"?>
<SchemeTable Partition="c3b14969-a553-4005-9a07-e9705c15b1e1" ID="034fec4c-f106-4b66-b850-e05852224a28" Name="FdCriteriaList_CriteriaValueTableInt32" Group="Fd Criteria" InstanceType="Cards" ContentType="Collections">
	<SchemeComplexColumn IsSystem="true" IsPermanent="true" IsSealed="true" ID="034fec4c-f106-0066-2000-005852224a28" Name="ID" Type="Reference(Typified) Not Null" ReferencedTable="1074eadd-21d7-4925-98c8-40d1e5f0ca0e">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="034fec4c-f106-0166-4000-005852224a28" Name="ID" Type="Guid Not Null" ReferencedColumn="9a58123b-b2e9-4137-9c6c-5dab0ec02747" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn IsSystem="true" IsPermanent="true" IsSealed="true" ID="034fec4c-f106-0066-3100-005852224a28" Name="RowID" Type="Guid Not Null" />
	<SchemeComplexColumn ID="fe5948ef-ff45-46aa-a9a4-e0d544ed5d5c" Name="Value" Type="Reference(Typified) Not Null" ReferencedTable="dc9fab0e-4c54-4769-8187-173c3ea68f9d">
		<SchemeReferencingColumn ID="fe5f5ac5-e0af-405a-9ebe-0d80e5d035e0" Name="ValueID" Type="Int32 Not Null" ReferencedColumn="96b37085-0f20-4c69-9253-d8fae2eebeaf" />
		<SchemeReferencingColumn ID="54568db5-2421-48cf-9f7c-cc05188355cb" Name="ValueName" Type="String(128) Null" ReferencedColumn="7fc4182e-1003-4a4b-a01d-44baf05c23e4" />
	</SchemeComplexColumn>
	<SchemeComplexColumn ID="409fe007-62ea-4d7f-afee-b459e6499cef" Name="CriteriaListRecord" Type="Reference(Typified) Null" ReferencedTable="38639b42-edcb-4e7e-a23e-cd722bf8e0ac" IsReferenceToOwner="true">
		<Description>Ссылка на родительскую коллекционную запись критерия этапа</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="409fe007-62ea-007f-4000-0459e6499cef" Name="CriteriaListRecordRowID" Type="Guid Null" ReferencedColumn="38639b42-edcb-007e-3100-0d722bf8e0ac" />
	</SchemeComplexColumn>
	<SchemeComplexColumn ID="61be9661-58dd-45cf-9b67-c20ec214f299" Name="ParticipantCriteriaListRecord" Type="Reference(Typified) Null" ReferencedTable="a7c5b23a-7e28-4f7a-8d1a-0488061003c8" IsReferenceToOwner="true">
		<Description>Ссылка на родительскую коллекционную запись критерия участника этапа</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="61be9661-58dd-00cf-4000-020ec214f299" Name="ParticipantCriteriaListRecordRowID" Type="Guid Null" ReferencedColumn="a7c5b23a-7e28-007a-3100-0488061003c8" />
	</SchemeComplexColumn>
	<SchemePrimaryKey IsSystem="true" IsPermanent="true" IsSealed="true" ID="034fec4c-f106-0066-5000-005852224a28" Name="pk_FdCriteriaList_CriteriaValueTableInt32">
		<SchemeIndexedColumn Column="034fec4c-f106-0066-3100-005852224a28" />
	</SchemePrimaryKey>
	<SchemeIndex IsSystem="true" IsPermanent="true" IsSealed="true" ID="034fec4c-f106-0066-7000-005852224a28" Name="idx_FdCriteriaList_CriteriaValueTableInt32_ID" IsClustered="true">
		<SchemeIndexedColumn Column="034fec4c-f106-0166-4000-005852224a28" />
	</SchemeIndex>
</SchemeTable>