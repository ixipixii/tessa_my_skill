<?xml version="1.0" encoding="utf-8"?>
<SchemeTable Partition="c3b14969-a553-4005-9a07-e9705c15b1e1" ID="38639b42-edcb-4e7e-a23e-cd722bf8e0ac" Name="FdCriteriaList" Group="Fd Criteria" InstanceType="Cards" ContentType="Collections">
	<Description>критерии шаблонов этапов</Description>
	<SchemeComplexColumn IsSystem="true" IsPermanent="true" IsSealed="true" ID="38639b42-edcb-007e-2000-0d722bf8e0ac" Name="ID" Type="Reference(Typified) Not Null" ReferencedTable="1074eadd-21d7-4925-98c8-40d1e5f0ca0e">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="38639b42-edcb-017e-4000-0d722bf8e0ac" Name="ID" Type="Guid Not Null" ReferencedColumn="9a58123b-b2e9-4137-9c6c-5dab0ec02747" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn IsSystem="true" IsPermanent="true" IsSealed="true" ID="38639b42-edcb-007e-3100-0d722bf8e0ac" Name="RowID" Type="Guid Not Null" />
	<SchemeComplexColumn ID="befeacd1-234c-4b7f-bb32-6c4c87ceae03" Name="Field" Type="Reference(Typified) Not Null" ReferencedTable="9a51283d-02ff-4a86-9d88-144addad3805">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="befeacd1-234c-007f-4000-0c4c87ceae03" Name="FieldID" Type="Guid Not Null" ReferencedColumn="9a51283d-02ff-0186-4000-044addad3805" />
		<SchemeReferencingColumn ID="6a959b10-b163-4071-9d7c-13f29f9d9697" Name="FieldCaption" Type="String(128) Not Null" ReferencedColumn="09b3454b-d9d3-4c6b-9ebf-12cfc1d911d5" />
		<SchemeReferencingColumn ID="cfba66c9-641d-4164-bc85-0bcd4f60d137" Name="FieldSectionTypeName" Type="String(128) Not Null" ReferencedColumn="5268e016-ff47-4a2b-ae09-631d68ae6455" />
		<SchemeReferencingColumn ID="b8c3714b-ccaf-4532-88df-0a55d16076f3" Name="FieldDataTypeID" Type="Guid Not Null" ReferencedColumn="cb758997-e875-489a-9854-89398204eb1e" />
		<SchemeReferencingColumn ID="a610b8db-0149-4f3f-ae38-f8f0b591b040" Name="FieldDataTypeCaption" Type="String(128) Not Null" ReferencedColumn="87f02e9b-5552-433b-8862-35dcd83de844" />
		<SchemeReferencingColumn ID="24ddbdef-1da1-4c82-9b5e-25aaf2a51f92" Name="FieldSectionID" Type="Guid Not Null" ReferencedColumn="c1764def-bad2-4a9d-81b1-384c56013aa1" />
		<SchemeReferencingColumn ID="85d3de22-40cd-402f-8840-6cbbd8616fee" Name="FieldSectionName" Type="String(128) Not Null" ReferencedColumn="65d67953-e0ef-4c96-9a70-cbfc42518985" />
		<SchemeReferencingColumn ID="757529a5-3781-4371-b621-3cb9a30ac9bd" Name="FieldComplexColumnID" Type="Guid Null" ReferencedColumn="d245f7a9-a177-4622-a71e-f703bcb8aaae" />
		<SchemeReferencingColumn ID="e7f26e25-27e2-436a-9319-050e9cb40d11" Name="FieldPhysicalColumnID" Type="Guid Null" ReferencedColumn="01ceb5cc-dcba-4bdd-a748-d46092bcb0e4" />
		<SchemeReferencingColumn ID="386a8835-7c41-4d83-9216-862887cf67ed" Name="FieldPhysicalColumnName" Type="String(128) Null" ReferencedColumn="16570f16-6fb3-4b9f-926d-2645823096fe" />
		<SchemeReferencingColumn ID="9c3e147d-c05e-4f4a-b423-558ee8758407" Name="FieldComplexColumnName" Type="String(128) Null" ReferencedColumn="30af168c-855d-467c-b867-4f324c68c760" />
		<SchemeReferencingColumn ID="1cca6eb2-7455-4b9b-b3f2-315a94ca6e6a" Name="FieldComplexColumnReferencedTableID" Type="Guid Null" ReferencedColumn="494ec73f-bef9-45eb-bef3-77fa2a0a8e9b" />
		<SchemeReferencingColumn ID="d2e293f2-40a6-4e53-8945-5662b087cdf5" Name="FieldComplexColumnReferencedTableName" Type="String(128) Null" ReferencedColumn="711f51a2-4d38-41bd-b7b1-a0dd5be05505" />
	</SchemeComplexColumn>
	<SchemeComplexColumn ID="28a78610-13ef-41ae-98a9-74cc30c6d6de" Name="CriteriaOperator" Type="Reference(Typified) Not Null" ReferencedTable="705ae655-75cb-4d86-a7e3-4b07377d98d6">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="28a78610-13ef-00ae-4000-04cc30c6d6de" Name="CriteriaOperatorID" Type="Int16 Not Null" ReferencedColumn="d9a8e964-2e01-46de-a1af-b30dcbf4311f" />
		<SchemeReferencingColumn ID="96de200e-f6a4-4293-b327-9f7196302499" Name="CriteriaOperatorCaption" Type="String(128) Not Null" ReferencedColumn="b40eeda3-15e5-40f6-942a-cfeaa7081c5d" />
		<SchemeReferencingColumn ID="2b4cf898-730c-410c-bd98-f877da3ecac4" Name="CriteriaOperatorName" Type="String(128) Not Null" ReferencedColumn="7f656a90-1cb9-43f0-9244-88dce8ad7c8b" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn ID="8b995901-f0d9-4fb9-b738-caebe8515d83" Name="CriteriaValueResult" Type="String(Max) Null">
		<Description>Результирующее значение</Description>
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="774fae00-ea48-429b-a350-49cec7c54146" Name="CriteriaValueString" Type="String(Max) Null">
		<Description>Строка</Description>
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="70c008bb-a373-44ac-8ce8-ba02d0e10554" Name="CriteriaValueDouble" Type="Double Null">
		<Description>Вещественное число</Description>
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="4c60ab0d-419f-4558-b585-0141b24df2ab" Name="CriteriaValueDouble2" Type="Double Null" />
	<SchemePhysicalColumn ID="6c236a07-d6cd-423c-8208-c2c0298e3b1d" Name="CriteriaValueDecimal" Type="Decimal(15, 4) Null">
		<Description>Десятичное число</Description>
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="bfaa74a8-a4d0-4bbb-898a-027b612299cb" Name="CriteriaValueDecimal2" Type="Decimal(15, 4) Null" />
	<SchemePhysicalColumn ID="9719505c-47de-4808-89e8-0104d4967eb1" Name="CriteriaValueInt" Type="Int64 Null">
		<Description>Целое число</Description>
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="e4316ad2-4f3d-4f37-99cc-0824ec9bb152" Name="CriteriaValueInt2" Type="Int64 Null" />
	<SchemePhysicalColumn ID="19c30115-51a9-4cfb-b6cb-234abd38e963" Name="CriteriaValueDateTime" Type="DateTime Null">
		<Description>Дата и время</Description>
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="1da174b4-431a-4914-b3fe-e94e863f0453" Name="CriteriaValueDateTime2" Type="DateTime Null" />
	<SchemeComplexColumn ID="c1b8c7ea-91b3-4329-a651-a5efc5b4f6c0" Name="CriteriaValueComplexGuid" Type="Reference(Abstract) Null" WithForeignKey="false">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="c1b8c7ea-91b3-0029-4000-05efc5b4f6c0" Name="CriteriaValueComplexGuidID" Type="Guid Null" ReferencedColumn="9a58123b-b2e9-4137-9c6c-5dab0ec02747" />
		<SchemePhysicalColumn ID="011b54a1-b491-468a-94b4-4f0838dcae76" Name="CriteriaValueComplexGuidName" Type="String(128) Null" />
	</SchemeComplexColumn>
	<SchemeComplexColumn ID="b7c1428d-dfea-4024-ae2d-4bfa2750440b" Name="CriteriaValueComplexInt32" Type="Reference(Typified) Null" ReferencedTable="dc9fab0e-4c54-4769-8187-173c3ea68f9d">
		<SchemeReferencingColumn ID="ca673790-a279-4776-a0a3-cd753c5cfc2c" Name="CriteriaValueComplexInt32ID" Type="Int32 Null" ReferencedColumn="96b37085-0f20-4c69-9253-d8fae2eebeaf" />
		<SchemeReferencingColumn ID="4b839150-500c-41bb-9d5a-b92476ad2451" Name="CriteriaValueComplexInt32Name" Type="String(128) Null" ReferencedColumn="7fc4182e-1003-4a4b-a01d-44baf05c23e4" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn ID="de37c217-c412-43ff-af7f-0a49fa2a183c" Name="Order" Type="Int32 Not Null">
		<SchemeDefaultConstraint IsPermanent="true" ID="bfe2e077-6373-4f3d-9c64-4335c11c35b6" Name="df_FdCriteriaList_Order" Value="1" />
	</SchemePhysicalColumn>
	<SchemePrimaryKey IsSystem="true" IsPermanent="true" IsSealed="true" ID="38639b42-edcb-007e-5000-0d722bf8e0ac" Name="pk_FdCriteriaList">
		<SchemeIndexedColumn Column="38639b42-edcb-007e-3100-0d722bf8e0ac" />
	</SchemePrimaryKey>
	<SchemeIndex IsSystem="true" IsPermanent="true" IsSealed="true" ID="38639b42-edcb-007e-7000-0d722bf8e0ac" Name="idx_FdCriteriaList_ID" IsClustered="true">
		<SchemeIndexedColumn Column="38639b42-edcb-017e-4000-0d722bf8e0ac" />
	</SchemeIndex>
</SchemeTable>