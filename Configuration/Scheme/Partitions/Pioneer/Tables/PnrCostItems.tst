<?xml version="1.0" encoding="utf-8"?>
<SchemeTable Partition="54ed79f5-11c0-4f74-b8b0-793d596699d6" ID="b2c21f32-7f00-4d58-9a2d-f45cbb5b2308" Name="PnrCostItems" Group="Pnr" InstanceType="Cards" ContentType="Entries">
	<Description>Справочник Статьи затрат</Description>
	<SchemeComplexColumn IsSystem="true" IsPermanent="true" IsSealed="true" ID="b2c21f32-7f00-0058-2000-045cbb5b2308" Name="ID" Type="Reference(Typified) Not Null" ReferencedTable="1074eadd-21d7-4925-98c8-40d1e5f0ca0e">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="b2c21f32-7f00-0158-4000-045cbb5b2308" Name="ID" Type="Guid Not Null" ReferencedColumn="9a58123b-b2e9-4137-9c6c-5dab0ec02747" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn ID="412f3e25-231f-4bf2-a393-103d4e805b38" Name="Code" Type="String(Max) Null">
		<Description>Код</Description>
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="03d21b73-553a-4e8d-947c-c1dbf2ace92c" Name="Name" Type="String(Max) Null">
		<Description>Наименование</Description>
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="c442d89a-a811-4585-a1e8-8c4a2382563e" Name="IsGroup" Type="String(Max) Null">
		<Description>Это группа</Description>
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="2114449e-fbdb-401b-9bd8-2fe871f8b118" Name="Codifier" Type="String(Max) Null">
		<Description>Кодификатор</Description>
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="b2d1519e-81d7-4706-a421-501f4addae10" Name="Used" Type="String(Max) Null">
		<Description>Используется</Description>
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="1923b942-21cc-4060-9400-d543d73fd533" Name="ConsumptionIncome" Type="String(Max) Null">
		<Description>Расход/Доход</Description>
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="167cddc5-7166-4617-96bd-7ebc65e5f19b" Name="ItemType" Type="String(Max) Null">
		<Description>Тип статьи</Description>
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="a65630da-61c3-4649-bd67-90a0e2286d0e" Name="IdentifierTreasury" Type="String(Max) Null">
		<Description>Идентификатор Казна</Description>
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="c25fda75-063b-4255-843f-d98f76fd3014" Name="MDMKey" Type="String(Max) Null">
		<Description>MDM-Key</Description>
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="eac1c02c-8f4b-448a-9ea7-ffcd60d39e8e" Name="IsRemoved" Type="Boolean Null">
		<Description>Удалено</Description>
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="a1bd25c0-f15f-4239-a660-0b125645ea60" Name="Description" Type="String(Max) Null">
		<Description>Описание</Description>
	</SchemePhysicalColumn>
	<SchemeComplexColumn ID="066c7ce0-bd40-40bb-bed8-2cefd65b0caf" Name="ParentCodifier" Type="Reference(Typified) Null" ReferencedTable="b2c21f32-7f00-4d58-9a2d-f45cbb5b2308">
		<Description>Родительская статья</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="066c7ce0-bd40-00bb-4000-0cefd65b0caf" Name="ParentCodifierID" Type="Guid Null" ReferencedColumn="b2c21f32-7f00-0158-4000-045cbb5b2308" />
		<SchemeReferencingColumn ID="c7501f66-0245-4ab0-b297-718a3c6b03d6" Name="ParentCodifierName" Type="String(Max) Null" ReferencedColumn="03d21b73-553a-4e8d-947c-c1dbf2ace92c" />
	</SchemeComplexColumn>
	<SchemePrimaryKey IsSystem="true" IsPermanent="true" IsSealed="true" ID="b2c21f32-7f00-0058-5000-045cbb5b2308" Name="pk_PnrCostItems" IsClustered="true">
		<SchemeIndexedColumn Column="b2c21f32-7f00-0158-4000-045cbb5b2308" />
	</SchemePrimaryKey>
</SchemeTable>