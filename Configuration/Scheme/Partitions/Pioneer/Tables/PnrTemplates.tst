<?xml version="1.0" encoding="utf-8"?>
<SchemeTable Partition="54ed79f5-11c0-4f74-b8b0-793d596699d6" ID="38164c0d-014b-4a7a-aaa7-94256608d29d" Name="PnrTemplates" Group="Pnr" InstanceType="Cards" ContentType="Entries">
	<Description>Шаблоны</Description>
	<SchemeComplexColumn IsSystem="true" IsPermanent="true" IsSealed="true" ID="38164c0d-014b-007a-2000-04256608d29d" Name="ID" Type="Reference(Typified) Not Null" ReferencedTable="1074eadd-21d7-4925-98c8-40d1e5f0ca0e">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="38164c0d-014b-017a-4000-04256608d29d" Name="ID" Type="Guid Not Null" ReferencedColumn="9a58123b-b2e9-4137-9c6c-5dab0ec02747" />
	</SchemeComplexColumn>
	<SchemeComplexColumn ID="054064dd-8776-45cd-a5de-6ffb3b43213d" Name="Template" Type="Reference(Typified) Null" ReferencedTable="ea65b864-aa58-4014-9359-92bd09d1a377">
		<Description>Шаблон</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="054064dd-8776-00cd-4000-0ffb3b43213d" Name="TemplateID" Type="Int16 Null" ReferencedColumn="2b91016e-6b21-4fc7-bbaa-78b40a17cf83" />
		<SchemeReferencingColumn ID="30370d29-8b15-4735-a43d-27bb5452379a" Name="TemplateName" Type="String(Max) Null" ReferencedColumn="3395d08d-0f90-4a5b-8f45-329ca98ea92b" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn ID="640e7da8-34e1-4d08-a4d2-3b823850dfb0" Name="ProjectNo" Type="String(Max) Null">
		<Description>Номер проекта</Description>
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="09f8a8aa-92f0-4e2d-8968-69c9b311d57f" Name="ProjectDate" Type="Date Null">
		<Description>Дата проекта</Description>
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="77f34c0f-81f9-4be4-8ad1-a17fd2645840" Name="SubjectContract" Type="String(Max) Null">
		<Description>Предмет договора</Description>
	</SchemePhysicalColumn>
	<SchemeComplexColumn ID="418e4342-60fa-440a-bb1e-1cceaf1884cb" Name="Project" Type="Reference(Typified) Null" ReferencedTable="44de4db0-e013-441c-8441-4cb3ef09a649">
		<Description>Проект</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="418e4342-60fa-000a-4000-0cceaf1884cb" Name="ProjectID" Type="Guid Null" ReferencedColumn="44de4db0-e013-011c-4000-0cb3ef09a649" />
		<SchemeReferencingColumn ID="b126f297-7b05-4dff-bd4a-3046871b29b5" Name="ProjectName" Type="String(Max) Null" ReferencedColumn="6c2fd8b3-e69b-4696-8a8a-2ab2d593af07" />
	</SchemeComplexColumn>
	<SchemeComplexColumn ID="b7a4af54-5b50-4b75-8cec-0d549811d67a" Name="Organization" Type="Reference(Typified) Null" ReferencedTable="303dd35d-b998-49b3-a10b-ed30d31bd36a">
		<Description>Организация ГК Пионер</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="b7a4af54-5b50-0075-4000-0d549811d67a" Name="OrganizationID" Type="Guid Null" ReferencedColumn="303dd35d-b998-01b3-4000-0d30d31bd36a" />
		<SchemeReferencingColumn ID="287491b8-e8e0-4d64-aaae-b7b2a0d9296b" Name="OrganizationName" Type="String(Max) Null" ReferencedColumn="d8ecabaa-f192-4546-9af6-1f9712daabdb" />
	</SchemeComplexColumn>
	<SchemeComplexColumn ID="61cb9345-c612-4e40-82b6-a7fc155a1467" Name="CFO" Type="Reference(Typified) Null" ReferencedTable="b5e873a7-4f25-4731-b7bf-93586f07b53a">
		<Description>ЦФО</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="61cb9345-c612-0040-4000-07fc155a1467" Name="CFOID" Type="Guid Null" ReferencedColumn="b5e873a7-4f25-0131-4000-03586f07b53a" />
		<SchemeReferencingColumn ID="815b57db-9b7b-4f51-816d-3a1a3c508bb9" Name="CFOName" Type="String(Max) Null" ReferencedColumn="20d4f2eb-ce34-4c44-87b8-8b386c283930" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn ID="4cea41ed-fa14-4e50-ae5d-7fe38c141951" Name="Content" Type="String(Max) Null">
		<Description>Содержание</Description>
	</SchemePhysicalColumn>
	<SchemeComplexColumn ID="ead748f4-b21d-4334-81c4-eaa2a4594c05" Name="Publish" Type="Reference(Typified) Null" ReferencedTable="a41f5936-2cb2-4f2c-8c46-f43eba3753e5">
		<Description>Шаблоны к публикации в CRM</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="ead748f4-b21d-0034-4000-0aa2a4594c05" Name="PublishID" Type="Int16 Null" ReferencedColumn="1f59643c-873a-4e5b-9f6a-ceac223fce7f" />
		<SchemeReferencingColumn ID="30db004b-486b-45ae-9f22-f29d3c1b3deb" Name="PublishName" Type="String(Max) Null" ReferencedColumn="3ab01221-918a-4221-b503-9ac2b4d3e0e1" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn ID="2b0ce223-207b-4076-a711-113ac9e98de7" Name="StartDate" Type="Date Null">
		<Description>Дата начала</Description>
	</SchemePhysicalColumn>
	<SchemeComplexColumn ID="80d8bdd4-8b89-4a12-9d69-39f359b1131d" Name="Responsible" Type="Reference(Typified) Null" ReferencedTable="6c977939-bbfc-456f-a133-f1c2244e3cc3">
		<Description>Ответственный</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="80d8bdd4-8b89-0012-4000-09f359b1131d" Name="ResponsibleID" Type="Guid Null" ReferencedColumn="6c977939-bbfc-016f-4000-01c2244e3cc3" />
		<SchemeReferencingColumn ID="48a59dcc-b85c-4c48-851c-eac1b7350114" Name="ResponsibleName" Type="String(128) Null" ReferencedColumn="1782f76a-4743-4aa4-920c-7edaee860964" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn ID="c8626315-d486-4cdc-afcc-60a1523dcfde" Name="ExtID" Type="Guid Null">
		<Description>Guid карточки из системы заказчика</Description>
	</SchemePhysicalColumn>
	<SchemePrimaryKey IsSystem="true" IsPermanent="true" IsSealed="true" ID="38164c0d-014b-007a-5000-04256608d29d" Name="pk_PnrTemplates" IsClustered="true">
		<SchemeIndexedColumn Column="38164c0d-014b-017a-4000-04256608d29d" />
	</SchemePrimaryKey>
</SchemeTable>