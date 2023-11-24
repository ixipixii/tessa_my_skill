<?xml version="1.0" encoding="utf-8"?>
<SchemeTable Partition="c3b14969-a553-4005-9a07-e9705c15b1e1" ID="e89bb104-52dd-4d27-9abc-76a9bbaac832" Name="FdSettings_ApproveCategoryTaskTypes" Group="Fd" InstanceType="Cards" ContentType="Collections">
	<Description>Дополнительно включить возможность запроса комментария и доп. согл./исполнения для след. типов заданий</Description>
	<SchemeComplexColumn IsSystem="true" IsPermanent="true" IsSealed="true" ID="e89bb104-52dd-0027-2000-06a9bbaac832" Name="ID" Type="Reference(Typified) Not Null" ReferencedTable="1074eadd-21d7-4925-98c8-40d1e5f0ca0e">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="e89bb104-52dd-0127-4000-06a9bbaac832" Name="ID" Type="Guid Not Null" ReferencedColumn="9a58123b-b2e9-4137-9c6c-5dab0ec02747" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn IsSystem="true" IsPermanent="true" IsSealed="true" ID="e89bb104-52dd-0027-3100-06a9bbaac832" Name="RowID" Type="Guid Not Null" />
	<SchemeComplexColumn ID="7e192c89-3427-4162-8e0e-e134f16cfe61" Name="TaskType" Type="Reference(Typified) Not Null" ReferencedTable="b0538ece-8468-4d0b-8b4e-5a1d43e024db">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="7e192c89-3427-0062-4000-0134f16cfe61" Name="TaskTypeID" Type="Guid Not Null" ReferencedColumn="a628a864-c858-4200-a6b7-da78c8e6e1f4" />
		<SchemeReferencingColumn ID="c882da2b-9169-4499-bae3-db5fb993f717" Name="TaskTypeCaption" Type="String(128) Not Null" ReferencedColumn="0a02451e-2e06-4001-9138-b4805e641afa" />
		<SchemeReferencingColumn ID="0aac7d62-4ab5-4d45-8697-88eb0d022a67" Name="TaskTypeName" Type="String(128) Not Null" ReferencedColumn="71181642-0d62-45f9-8ad8-ccec4bd4ce22" />
	</SchemeComplexColumn>
	<SchemePrimaryKey IsSystem="true" IsPermanent="true" IsSealed="true" ID="e89bb104-52dd-0027-5000-06a9bbaac832" Name="pk_FdSettings_ApproveCategoryTaskTypes">
		<SchemeIndexedColumn Column="e89bb104-52dd-0027-3100-06a9bbaac832" />
	</SchemePrimaryKey>
	<SchemeIndex IsSystem="true" IsPermanent="true" IsSealed="true" ID="e89bb104-52dd-0027-7000-06a9bbaac832" Name="idx_FdSettings_ApproveCategoryTaskTypes_ID" IsClustered="true">
		<SchemeIndexedColumn Column="e89bb104-52dd-0127-4000-06a9bbaac832" />
	</SchemeIndex>
</SchemeTable>