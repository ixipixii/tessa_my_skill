<?xml version="1.0" encoding="utf-8"?>
<SchemeTable Partition="54ed79f5-11c0-4f74-b8b0-793d596699d6" ID="44de4db0-e013-441c-8441-4cb3ef09a649" Name="PnrProjects" Group="Pnr" InstanceType="Cards" ContentType="Entries">
	<Description>Справочник Проекты</Description>
	<SchemeComplexColumn IsSystem="true" IsPermanent="true" IsSealed="true" ID="44de4db0-e013-001c-2000-0cb3ef09a649" Name="ID" Type="Reference(Typified) Not Null" ReferencedTable="1074eadd-21d7-4925-98c8-40d1e5f0ca0e">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="44de4db0-e013-011c-4000-0cb3ef09a649" Name="ID" Type="Guid Not Null" ReferencedColumn="9a58123b-b2e9-4137-9c6c-5dab0ec02747" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn ID="49c662a7-c625-4fac-903e-11413f5b54f2" Name="Code" Type="String(Max) Null">
		<Description>Код</Description>
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="6c2fd8b3-e69b-4696-8a8a-2ab2d593af07" Name="Name" Type="String(Max) Null">
		<Description>Название</Description>
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="b8bb6b08-adc9-4fa6-97c9-2fdfb56c5226" Name="Description" Type="String(Max) Null">
		<Description>Описание</Description>
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="ed894d1d-0228-4d1d-8635-53542829f4d0" Name="EndDate" Type="Date Null">
		<Description>Дата окончания</Description>
	</SchemePhysicalColumn>
	<SchemeComplexColumn ID="84438acd-6a02-44b6-bb41-540e645f4924" Name="ParentProject" Type="Reference(Typified) Null" ReferencedTable="44de4db0-e013-441c-8441-4cb3ef09a649">
		<Description>Родительский проект</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="84438acd-6a02-00b6-4000-040e645f4924" Name="ParentProjectID" Type="Guid Null" ReferencedColumn="44de4db0-e013-011c-4000-0cb3ef09a649" />
		<SchemeReferencingColumn ID="0e5caead-4c45-4148-9c0c-04d65df63192" Name="ParentProjectName" Type="String(Max) Null" ReferencedColumn="6c2fd8b3-e69b-4696-8a8a-2ab2d593af07" />
	</SchemeComplexColumn>
	<SchemeComplexColumn ID="76927d99-1d98-414c-aee7-cd2b3692c8e6" Name="ProjectManager" Type="Reference(Typified) Null" ReferencedTable="6c977939-bbfc-456f-a133-f1c2244e3cc3">
		<Description>Руководитель проекта</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="76927d99-1d98-004c-4000-0d2b3692c8e6" Name="ProjectManagerID" Type="Guid Null" ReferencedColumn="6c977939-bbfc-016f-4000-01c2244e3cc3" />
		<SchemeReferencingColumn ID="dfc34f46-b5d6-48b5-9844-7ed4ac1f6c05" Name="ProjectManagerName" Type="String(128) Null" ReferencedColumn="1782f76a-4743-4aa4-920c-7edaee860964" />
	</SchemeComplexColumn>
	<SchemeComplexColumn ID="7b7041bc-bc2d-44a1-891a-f4672b159a0d" Name="Estimator" Type="Reference(Typified) Null" ReferencedTable="6c977939-bbfc-456f-a133-f1c2244e3cc3">
		<Description>Сметчик</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="7b7041bc-bc2d-00a1-4000-04672b159a0d" Name="EstimatorID" Type="Guid Null" ReferencedColumn="6c977939-bbfc-016f-4000-01c2244e3cc3" />
		<SchemeReferencingColumn ID="1432a6d1-75cd-4ec3-8224-4a6c81db41f0" Name="EstimatorName" Type="String(128) Null" ReferencedColumn="1782f76a-4743-4aa4-920c-7edaee860964" />
	</SchemeComplexColumn>
	<SchemeComplexColumn ID="8f7a3b46-32c8-413e-8d17-09e9e9b0ee64" Name="GIP" Type="Reference(Typified) Null" ReferencedTable="6c977939-bbfc-456f-a133-f1c2244e3cc3">
		<Description>ГИП</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="8f7a3b46-32c8-003e-4000-09e9e9b0ee64" Name="GIPID" Type="Guid Null" ReferencedColumn="6c977939-bbfc-016f-4000-01c2244e3cc3" />
		<SchemeReferencingColumn ID="32729c7e-b5cb-4c29-be9e-e8c9b950abbf" Name="GIPName" Type="String(128) Null" ReferencedColumn="1782f76a-4743-4aa4-920c-7edaee860964" />
	</SchemeComplexColumn>
	<SchemeComplexColumn ID="3da5ce90-7618-4d6f-97e2-57a900d677e8" Name="ProjectAdministrator" Type="Reference(Typified) Null" ReferencedTable="6c977939-bbfc-456f-a133-f1c2244e3cc3">
		<Description>Администратор проекта</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="3da5ce90-7618-006f-4000-07a900d677e8" Name="ProjectAdministratorID" Type="Guid Null" ReferencedColumn="6c977939-bbfc-016f-4000-01c2244e3cc3" />
		<SchemeReferencingColumn ID="7044375b-ed5e-479b-8ca9-4cd7bb64c722" Name="ProjectAdministratorName" Type="String(128) Null" ReferencedColumn="1782f76a-4743-4aa4-920c-7edaee860964" />
	</SchemeComplexColumn>
	<SchemeComplexColumn ID="83c51ba1-3464-4aea-9f48-36e292ad569d" Name="ProjectEconomist" Type="Reference(Typified) Null" ReferencedTable="6c977939-bbfc-456f-a133-f1c2244e3cc3">
		<Description>Экономист проекта</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="83c51ba1-3464-00ea-4000-06e292ad569d" Name="ProjectEconomistID" Type="Guid Null" ReferencedColumn="6c977939-bbfc-016f-4000-01c2244e3cc3" />
		<SchemeReferencingColumn ID="bf6e2292-48ce-4fef-8246-1a245547a90e" Name="ProjectEconomistName" Type="String(128) Null" ReferencedColumn="1782f76a-4743-4aa4-920c-7edaee860964" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn ID="d75f95d8-2729-4ec7-8dd4-06cfc6165480" Name="MDMKey" Type="String(Max) Null">
		<Description>MDM-Key</Description>
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="aa8c2f04-090c-4dc0-83ee-782e3f47f3fa" Name="InArchive" Type="Boolean Null">
		<Description>В архиве</Description>
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="e04ff668-ee8e-4f28-8149-03103335d121" Name="IsHidden" Type="Boolean Null" />
	<SchemeComplexColumn ID="fa26a421-7305-4339-af03-7b5b1a43c10a" Name="EngineerPTO" Type="Reference(Typified) Null" ReferencedTable="6c977939-bbfc-456f-a133-f1c2244e3cc3">
		<Description>Инженер Производственно-Технического Отдела</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="fa26a421-7305-0039-4000-0b5b1a43c10a" Name="EngineerPTOID" Type="Guid Null" ReferencedColumn="6c977939-bbfc-016f-4000-01c2244e3cc3" />
		<SchemeReferencingColumn ID="f674e6b0-ab79-4412-9031-92114502147e" Name="EngineerPTOName" Type="String(128) Null" ReferencedColumn="1782f76a-4743-4aa4-920c-7edaee860964" />
	</SchemeComplexColumn>
	<SchemeComplexColumn ID="a4afdbaa-6255-4a9d-85db-65fd4f720cf6" Name="ConstructionManager" Type="Reference(Typified) Null" ReferencedTable="6c977939-bbfc-456f-a133-f1c2244e3cc3">
		<Description>Руководитель строительства</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="a4afdbaa-6255-009d-4000-05fd4f720cf6" Name="ConstructionManagerID" Type="Guid Null" ReferencedColumn="6c977939-bbfc-016f-4000-01c2244e3cc3" />
		<SchemeReferencingColumn ID="afddd0e7-dd9d-405b-9875-b753d1ba249c" Name="ConstructionManagerName" Type="String(128) Null" ReferencedColumn="1782f76a-4743-4aa4-920c-7edaee860964" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn ID="cecfefb7-d10b-4972-aae8-d49b381ea2da" Name="ExtID" Type="Guid Null">
		<Description>Guid в системе заказчика</Description>
	</SchemePhysicalColumn>
	<SchemeComplexColumn ID="5df7c550-b52a-4f9e-8907-c3e3e7979efc" Name="ProjectResponsible" Type="Reference(Typified) Null" ReferencedTable="6c977939-bbfc-456f-a133-f1c2244e3cc3">
		<Description>Ответственный проекта</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="5df7c550-b52a-009e-4000-03e3e7979efc" Name="ProjectResponsibleID" Type="Guid Null" ReferencedColumn="6c977939-bbfc-016f-4000-01c2244e3cc3" />
		<SchemeReferencingColumn ID="36901b1e-18c9-4713-8fd5-73c1a1699a00" Name="ProjectResponsibleName" Type="String(128) Null" ReferencedColumn="1782f76a-4743-4aa4-920c-7edaee860964" />
	</SchemeComplexColumn>
	<SchemePrimaryKey IsSystem="true" IsPermanent="true" IsSealed="true" ID="44de4db0-e013-001c-5000-0cb3ef09a649" Name="pk_PnrProjects" IsClustered="true">
		<SchemeIndexedColumn Column="44de4db0-e013-011c-4000-0cb3ef09a649" />
	</SchemePrimaryKey>
</SchemeTable>