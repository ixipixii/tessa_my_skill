<?xml version="1.0" encoding="utf-8"?>
<SchemeTable Partition="54ed79f5-11c0-4f74-b8b0-793d596699d6" ID="fe777a4c-ce51-4fcd-9fbf-d329164998f1" Name="PnrRegulations" Group="Pnr" InstanceType="Cards" ContentType="Entries">
	<Description>Регламентирующий документ (ВНД)</Description>
	<SchemeComplexColumn IsSystem="true" IsPermanent="true" IsSealed="true" ID="fe777a4c-ce51-00cd-2000-0329164998f1" Name="ID" Type="Reference(Typified) Not Null" ReferencedTable="1074eadd-21d7-4925-98c8-40d1e5f0ca0e">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="fe777a4c-ce51-01cd-4000-0329164998f1" Name="ID" Type="Guid Not Null" ReferencedColumn="9a58123b-b2e9-4137-9c6c-5dab0ec02747" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn ID="a4d7390d-44f3-4e69-9106-f906326f6ad2" Name="RegistrationDate" Type="Date Null">
		<Description>Дата регистрации</Description>
	</SchemePhysicalColumn>
	<SchemeComplexColumn ID="cd18819a-60e8-4e2b-89b3-e9f378eba1f5" Name="Direction" Type="Reference(Typified) Null" ReferencedTable="14cf5907-5876-4b3e-8470-41fd6c9abbcf">
		<Description>Направление</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="cd18819a-60e8-002b-4000-09f378eba1f5" Name="DirectionID" Type="Guid Null" ReferencedColumn="14cf5907-5876-013e-4000-01fd6c9abbcf" />
		<SchemeReferencingColumn ID="1978f148-c125-409e-a81a-33ee26b31b9f" Name="DirectionName" Type="String(Max) Null" ReferencedColumn="dce07234-3902-4b2d-a5ea-ef4c0ae3fea2" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn ID="1af07243-087c-4586-b82c-479e0ef810be" Name="Comments" Type="String(Max) Null">
		<Description>Комментарии</Description>
	</SchemePhysicalColumn>
	<SchemeComplexColumn ID="3793e58e-b197-4b30-8008-85a710ac5f78" Name="CFO" Type="Reference(Typified) Null" ReferencedTable="b5e873a7-4f25-4731-b7bf-93586f07b53a">
		<Description>ЦФО</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="3793e58e-b197-0030-4000-05a710ac5f78" Name="CFOID" Type="Guid Null" ReferencedColumn="b5e873a7-4f25-0131-4000-03586f07b53a" />
		<SchemeReferencingColumn ID="fa20c100-1507-40c6-bef6-88e5f0f52100" Name="CFOName" Type="String(Max) Null" ReferencedColumn="20d4f2eb-ce34-4c44-87b8-8b386c283930" />
	</SchemeComplexColumn>
	<SchemeComplexColumn ID="7a49a211-c1c9-45c2-a664-bd28731380d0" Name="WriteOff" Type="Reference(Typified) Null" ReferencedTable="81f6010b-9641-4aa5-8897-b8e8603fbf4b">
		<Description>Списать в дело</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="7a49a211-c1c9-00c2-4000-0d28731380d0" Name="WriteOffID" Type="Guid Null" ReferencedColumn="81f6010b-9641-01a5-4000-08e8603fbf4b" />
		<SchemeReferencingColumn ID="d35f073a-1614-4ae5-958d-513ce52e90d9" Name="WriteOffName" Type="String(128) Null" ReferencedColumn="616d6b2e-35d5-424d-846b-618eb25962d0" />
		<SchemeReferencingColumn ID="3ec9c192-7799-4514-8308-04f6ac98ceee" Name="WriteOffIdx" Type="String(Max) Null" ReferencedColumn="1f7ca0d4-1b64-43f8-b973-d14791af51a0" />
	</SchemeComplexColumn>
	<SchemePrimaryKey IsSystem="true" IsPermanent="true" IsSealed="true" ID="fe777a4c-ce51-00cd-5000-0329164998f1" Name="pk_PnrRegulations" IsClustered="true">
		<SchemeIndexedColumn Column="fe777a4c-ce51-01cd-4000-0329164998f1" />
	</SchemePrimaryKey>
</SchemeTable>