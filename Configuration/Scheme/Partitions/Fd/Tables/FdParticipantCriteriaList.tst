<?xml version="1.0" encoding="utf-8"?>
<SchemeTable Partition="c3b14969-a553-4005-9a07-e9705c15b1e1" ID="a7c5b23a-7e28-4f7a-8d1a-0488061003c8" Name="FdParticipantCriteriaList" Group="Fd Criteria" InstanceType="Cards" ContentType="Collections">
	<Description>Критерии участника в шаблоне этапа</Description>
	<SchemeComplexColumn IsSystem="true" IsPermanent="true" IsSealed="true" ID="a7c5b23a-7e28-007a-2000-0488061003c8" Name="ID" Type="Reference(Typified) Not Null" ReferencedTable="1074eadd-21d7-4925-98c8-40d1e5f0ca0e">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="a7c5b23a-7e28-017a-4000-0488061003c8" Name="ID" Type="Guid Not Null" ReferencedColumn="9a58123b-b2e9-4137-9c6c-5dab0ec02747" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn IsSystem="true" IsPermanent="true" IsSealed="true" ID="a7c5b23a-7e28-007a-3100-0488061003c8" Name="RowID" Type="Guid Not Null" />
	<SchemeComplexColumn ID="e234e8ca-8ada-41a9-bac0-508d0726ae69" Name="Field" Type="Reference(Typified) Not Null" ReferencedTable="9a51283d-02ff-4a86-9d88-144addad3805">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="e234e8ca-8ada-00a9-4000-008d0726ae69" Name="FieldID" Type="Guid Not Null" ReferencedColumn="9a51283d-02ff-0186-4000-044addad3805" />
		<SchemeReferencingColumn ID="fd0b97b8-29f2-409e-a223-e287d1349508" Name="FieldCaption" Type="String(128) Not Null" ReferencedColumn="09b3454b-d9d3-4c6b-9ebf-12cfc1d911d5" />
		<SchemeReferencingColumn ID="2063bdb1-6cf0-4601-8141-a3ef85de11e2" Name="FieldSectionTypeName" Type="String(128) Not Null" ReferencedColumn="5268e016-ff47-4a2b-ae09-631d68ae6455" />
		<SchemeReferencingColumn ID="db219f90-88b8-4e0f-a805-4498b509eee7" Name="FieldDataTypeID" Type="Guid Not Null" ReferencedColumn="cb758997-e875-489a-9854-89398204eb1e" />
		<SchemeReferencingColumn ID="b135b865-2228-4852-be6c-f35de6f492ea" Name="FieldDataTypeCaption" Type="String(128) Not Null" ReferencedColumn="87f02e9b-5552-433b-8862-35dcd83de844" />
		<SchemeReferencingColumn ID="85b1b538-1d45-47f1-ae0d-c20d7a9524f3" Name="FieldSectionID" Type="Guid Not Null" ReferencedColumn="c1764def-bad2-4a9d-81b1-384c56013aa1" />
		<SchemeReferencingColumn ID="cebd50c1-6e86-4587-bbfe-4f66f2b5cc62" Name="FieldSectionName" Type="String(128) Not Null" ReferencedColumn="65d67953-e0ef-4c96-9a70-cbfc42518985" />
		<SchemeReferencingColumn ID="7e4595d7-e28c-4f8d-a8f8-1b55e58b1b42" Name="FieldComplexColumnID" Type="Guid Null" ReferencedColumn="d245f7a9-a177-4622-a71e-f703bcb8aaae" />
		<SchemeReferencingColumn ID="1a1e1414-f9a2-469e-b4a4-b247af08f6f1" Name="FieldPhysicalColumnID" Type="Guid Null" ReferencedColumn="01ceb5cc-dcba-4bdd-a748-d46092bcb0e4" />
		<SchemeReferencingColumn ID="c4322cac-ab46-4b82-8242-88566d20ec31" Name="FieldPhysicalColumnName" Type="String(128) Null" ReferencedColumn="16570f16-6fb3-4b9f-926d-2645823096fe" />
		<SchemeReferencingColumn ID="5b7a9343-d986-46c2-b459-c8f9361f1844" Name="FieldComplexColumnName" Type="String(128) Null" ReferencedColumn="30af168c-855d-467c-b867-4f324c68c760" />
		<SchemeReferencingColumn ID="029b2266-6ed8-48db-9226-de135276eed9" Name="FieldComplexColumnReferencedTableID" Type="Guid Null" ReferencedColumn="494ec73f-bef9-45eb-bef3-77fa2a0a8e9b" />
		<SchemeReferencingColumn ID="ec989f54-c5ea-4ca7-9e8d-6e251430970f" Name="FieldComplexColumnReferencedTableName" Type="String(128) Null" ReferencedColumn="711f51a2-4d38-41bd-b7b1-a0dd5be05505" />
	</SchemeComplexColumn>
	<SchemeComplexColumn ID="d4f90b24-6dd0-4580-9f19-8ddd5b0b742c" Name="CriteriaOperator" Type="Reference(Typified) Not Null" ReferencedTable="705ae655-75cb-4d86-a7e3-4b07377d98d6">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="d4f90b24-6dd0-0080-4000-0ddd5b0b742c" Name="CriteriaOperatorID" Type="Int16 Not Null" ReferencedColumn="d9a8e964-2e01-46de-a1af-b30dcbf4311f" />
		<SchemeReferencingColumn ID="e1a41851-3a2c-437d-a001-5d293431563c" Name="CriteriaOperatorCaption" Type="String(128) Not Null" ReferencedColumn="b40eeda3-15e5-40f6-942a-cfeaa7081c5d" />
		<SchemeReferencingColumn ID="609a6f79-12c1-4033-8dfd-614ad902b85b" Name="CriteriaOperatorName" Type="String(128) Not Null" ReferencedColumn="7f656a90-1cb9-43f0-9244-88dce8ad7c8b" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn ID="58f1ed98-0f0f-4c43-b03b-9b5729af0075" Name="CriteriaValueResult" Type="String(Max) Null" />
	<SchemePhysicalColumn ID="433768fb-a327-4ff3-ae66-cee2a99fb23e" Name="CriteriaValueString" Type="String(Max) Null" />
	<SchemePhysicalColumn ID="cad152ce-d414-4594-a07e-9ce06a733265" Name="CriteriaValueDouble" Type="Double Null" />
	<SchemePhysicalColumn ID="e7cf7ce6-78c7-45db-bc74-576c6db77b97" Name="CriteriaValueDouble2" Type="Double Null" />
	<SchemePhysicalColumn ID="5eee22bb-def7-4daa-9627-190dc0748a12" Name="CriteriaValueDecimal" Type="Decimal(15, 4) Null" />
	<SchemePhysicalColumn ID="aeeb7f77-c9fb-4662-a7c5-1bf95914df6e" Name="CriteriaValueDecimal2" Type="Decimal(15, 4) Null" />
	<SchemePhysicalColumn ID="07a019ef-93b0-4b5e-9c93-7ab47468f617" Name="CriteriaValueInt" Type="Int64 Null" />
	<SchemePhysicalColumn ID="e04c921d-2cd6-483e-ac56-a3bcbe112340" Name="CriteriaValueInt2" Type="Int64 Null" />
	<SchemePhysicalColumn ID="3437d499-8339-4b86-a255-0fece64a20f2" Name="CriteriaValueDateTime" Type="DateTime Null" />
	<SchemePhysicalColumn ID="6a288c33-c010-4c2f-ba23-a8a21435070c" Name="CriteriaValueDateTime2" Type="DateTime Null" />
	<SchemeComplexColumn ID="bbfb58fa-794b-4e89-a6e7-3bf24f9bf187" Name="CriteriaValueComplexGuid" Type="Reference(Abstract) Null" WithForeignKey="false">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="bbfb58fa-794b-0089-4000-0bf24f9bf187" Name="CriteriaValueComplexGuidID" Type="Guid Null" ReferencedColumn="9a58123b-b2e9-4137-9c6c-5dab0ec02747" />
		<SchemePhysicalColumn ID="3ce74448-c34c-4679-8520-43c557ff2625" Name="CriteriaValueComplexGuidName" Type="String(128) Null" />
	</SchemeComplexColumn>
	<SchemeComplexColumn ID="b9c3775b-7915-4b7e-b52b-197e5adce277" Name="CriteriaValueComplexInt32" Type="Reference(Typified) Null" ReferencedTable="dc9fab0e-4c54-4769-8187-173c3ea68f9d">
		<SchemeReferencingColumn ID="29b5cb27-c4e2-41f1-9c64-d923b6a63d36" Name="CriteriaValueComplexInt32ID" Type="Int32 Null" ReferencedColumn="96b37085-0f20-4c69-9253-d8fae2eebeaf" />
		<SchemeReferencingColumn ID="17ec5e9d-6a96-46fb-a9ec-f8194f34228c" Name="CriteriaValueComplexInt32Name" Type="String(128) Null" ReferencedColumn="7fc4182e-1003-4a4b-a01d-44baf05c23e4" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn ID="3d01476e-0c07-4d89-88eb-c6d3dc5878a5" Name="Order" Type="Int32 Not Null">
		<SchemeDefaultConstraint IsPermanent="true" ID="998c6976-19fc-49e4-a17a-f76155316b3b" Name="df_FdParticipantCriteriaList_Order" Value="1" />
	</SchemePhysicalColumn>
	<SchemeComplexColumn ID="059d29b9-f3c5-46d0-ba91-7e94a9106eae" Name="Participant" Type="Reference(Typified) Not Null" ReferencedTable="b9d1e0af-06c0-4c17-9e83-0497464e193b" IsReferenceToOwner="true">
		<Description>Ссылка на родительскую коллекционную запись участника этапа</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="059d29b9-f3c5-00d0-4000-0e94a9106eae" Name="ParticipantRowID" Type="Guid Not Null" ReferencedColumn="b9d1e0af-06c0-0017-3100-0497464e193b" />
	</SchemeComplexColumn>
	<SchemePrimaryKey IsSystem="true" IsPermanent="true" IsSealed="true" ID="a7c5b23a-7e28-007a-5000-0488061003c8" Name="pk_FdParticipantCriteriaList">
		<SchemeIndexedColumn Column="a7c5b23a-7e28-007a-3100-0488061003c8" />
	</SchemePrimaryKey>
	<SchemeIndex IsSystem="true" IsPermanent="true" IsSealed="true" ID="a7c5b23a-7e28-007a-7000-0488061003c8" Name="idx_FdParticipantCriteriaList_ID" IsClustered="true">
		<SchemeIndexedColumn Column="a7c5b23a-7e28-017a-4000-0488061003c8" />
	</SchemeIndex>
</SchemeTable>