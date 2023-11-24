<?xml version="1.0" encoding="utf-8"?>
<SchemeTable Partition="54ed79f5-11c0-4f74-b8b0-793d596699d6" ID="f0475bbd-e18d-42ad-859a-605612052dc8" Name="PnrFileCategories_PartnerType" Group="Pnr" InstanceType="Cards" ContentType="Collections">
	<Description>Привязка файловых категорий к типам контрагентов</Description>
	<SchemeComplexColumn IsSystem="true" IsPermanent="true" IsSealed="true" ID="f0475bbd-e18d-00ad-2000-005612052dc8" Name="ID" Type="Reference(Typified) Not Null" ReferencedTable="1074eadd-21d7-4925-98c8-40d1e5f0ca0e">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="f0475bbd-e18d-01ad-4000-005612052dc8" Name="ID" Type="Guid Not Null" ReferencedColumn="9a58123b-b2e9-4137-9c6c-5dab0ec02747" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn IsSystem="true" IsPermanent="true" IsSealed="true" ID="f0475bbd-e18d-00ad-3100-005612052dc8" Name="RowID" Type="Guid Not Null" />
	<SchemeComplexColumn ID="ff0bb05f-7aa0-4c07-8ab2-d550ab176317" Name="PartnerType" Type="Reference(Typified) Null" ReferencedTable="354e4f5a-e50c-4a11-84d0-6e0a98a81ca5">
		<Description>Тип контрагента</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="ff0bb05f-7aa0-0007-4000-0550ab176317" Name="PartnerTypeID" Type="Int32 Null" ReferencedColumn="876c8cd8-505f-40f4-ba4a-65ae78b22945" />
		<SchemeReferencingColumn ID="9a946e17-fcbd-4f72-bddc-295cffb1f9e6" Name="PartnerTypeName" Type="String(256) Null" ReferencedColumn="695e6069-4bde-406a-b880-a0a27c87117e" />
	</SchemeComplexColumn>
	<SchemePrimaryKey IsSystem="true" IsPermanent="true" IsSealed="true" ID="f0475bbd-e18d-00ad-5000-005612052dc8" Name="pk_PnrFileCategories_PartnerType">
		<SchemeIndexedColumn Column="f0475bbd-e18d-00ad-3100-005612052dc8" />
	</SchemePrimaryKey>
	<SchemeIndex IsSystem="true" IsPermanent="true" IsSealed="true" ID="f0475bbd-e18d-00ad-7000-005612052dc8" Name="idx_PnrFileCategories_PartnerType_ID" IsClustered="true">
		<SchemeIndexedColumn Column="f0475bbd-e18d-01ad-4000-005612052dc8" />
	</SchemeIndex>
</SchemeTable>