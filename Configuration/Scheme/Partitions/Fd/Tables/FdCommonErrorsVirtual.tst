<?xml version="1.0" encoding="utf-8"?>
<SchemeTable Partition="c3b14969-a553-4005-9a07-e9705c15b1e1" ID="f24814d9-b8f1-4320-90bc-2135db2df767" Name="FdCommonErrorsVirtual" Group="FdDiagnostic" IsVirtual="true" InstanceType="Cards" ContentType="Collections">
	<SchemeComplexColumn IsSystem="true" IsPermanent="true" IsSealed="true" ID="f24814d9-b8f1-0020-2000-0135db2df767" Name="ID" Type="Reference(Typified) Not Null" ReferencedTable="1074eadd-21d7-4925-98c8-40d1e5f0ca0e">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="f24814d9-b8f1-0120-4000-0135db2df767" Name="ID" Type="Guid Not Null" ReferencedColumn="9a58123b-b2e9-4137-9c6c-5dab0ec02747" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn IsSystem="true" IsPermanent="true" IsSealed="true" ID="f24814d9-b8f1-0020-3100-0135db2df767" Name="RowID" Type="Guid Not Null" />
	<SchemePhysicalColumn ID="9beffe2d-d334-49b5-b4e3-bc29a20cd954" Name="Description" Type="String(Max) Null" />
	<SchemePhysicalColumn ID="16e74474-d9d1-4d63-9d42-617edd350ae9" Name="FieldName" Type="String(Max) Null" />
	<SchemePhysicalColumn ID="baac10e5-f82b-4010-94f3-a6c01488e930" Name="FieldValue" Type="String(Max) Null" />
	<SchemePrimaryKey IsSystem="true" IsPermanent="true" IsSealed="true" ID="f24814d9-b8f1-0020-5000-0135db2df767" Name="pk_FdCommonErrorsVirtual">
		<SchemeIndexedColumn Column="f24814d9-b8f1-0020-3100-0135db2df767" />
	</SchemePrimaryKey>
	<SchemeIndex IsSystem="true" IsPermanent="true" IsSealed="true" ID="f24814d9-b8f1-0020-7000-0135db2df767" Name="idx_FdCommonErrorsVirtual_ID" IsClustered="true">
		<SchemeIndexedColumn Column="f24814d9-b8f1-0120-4000-0135db2df767" />
	</SchemeIndex>
</SchemeTable>