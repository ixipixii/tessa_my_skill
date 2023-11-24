<?xml version="1.0" encoding="utf-8"?>
<SchemeTable Partition="c3b14969-a553-4005-9a07-e9705c15b1e1" ID="64c5e272-87f8-42f6-b184-1790ac51090f" Name="FdStageTemplateVirtual" Group="Fd" IsVirtual="true" InstanceType="Cards" ContentType="Entries">
	<SchemeComplexColumn IsSystem="true" IsPermanent="true" IsSealed="true" ID="64c5e272-87f8-00f6-2000-0790ac51090f" Name="ID" Type="Reference(Typified) Not Null" ReferencedTable="1074eadd-21d7-4925-98c8-40d1e5f0ca0e">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="64c5e272-87f8-01f6-4000-0790ac51090f" Name="ID" Type="Guid Not Null" ReferencedColumn="9a58123b-b2e9-4137-9c6c-5dab0ec02747" />
	</SchemeComplexColumn>
	<SchemeComplexColumn ID="a04a2d38-1f6a-4ee6-9a65-27f1d878ccac" Name="CardType" Type="Reference(Typified) Null" ReferencedTable="b0538ece-8468-4d0b-8b4e-5a1d43e024db">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="a04a2d38-1f6a-00e6-4000-07f1d878ccac" Name="CardTypeID" Type="Guid Null" ReferencedColumn="a628a864-c858-4200-a6b7-da78c8e6e1f4" />
		<SchemeReferencingColumn ID="9161a47d-a3a8-4e96-88ef-4dddb71db9e3" Name="CardTypeCaption" Type="String(128) Null" ReferencedColumn="0a02451e-2e06-4001-9138-b4805e641afa" />
	</SchemeComplexColumn>
	<SchemePrimaryKey IsSystem="true" IsPermanent="true" IsSealed="true" ID="64c5e272-87f8-00f6-5000-0790ac51090f" Name="pk_FdStageTemplateVirtual" IsClustered="true">
		<SchemeIndexedColumn Column="64c5e272-87f8-01f6-4000-0790ac51090f" />
	</SchemePrimaryKey>
</SchemeTable>