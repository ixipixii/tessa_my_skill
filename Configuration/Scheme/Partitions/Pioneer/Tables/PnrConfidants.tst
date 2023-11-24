<?xml version="1.0" encoding="utf-8"?>
<SchemeTable Partition="54ed79f5-11c0-4f74-b8b0-793d596699d6" ID="b49a1639-2158-4117-933d-02a42fe6c7b8" Name="PnrConfidants" Group="Pnr" InstanceType="Cards" ContentType="Collections">
	<Description>Доверенные лица</Description>
	<SchemeComplexColumn IsSystem="true" IsPermanent="true" IsSealed="true" ID="b49a1639-2158-0017-2000-02a42fe6c7b8" Name="ID" Type="Reference(Typified) Not Null" ReferencedTable="1074eadd-21d7-4925-98c8-40d1e5f0ca0e">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="b49a1639-2158-0117-4000-02a42fe6c7b8" Name="ID" Type="Guid Not Null" ReferencedColumn="9a58123b-b2e9-4137-9c6c-5dab0ec02747" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn IsSystem="true" IsPermanent="true" IsSealed="true" ID="b49a1639-2158-0017-3100-02a42fe6c7b8" Name="RowID" Type="Guid Not Null" />
	<SchemeComplexColumn ID="c636f834-6a7a-402d-8bf9-15cc563223b1" Name="User" Type="Reference(Typified) Not Null" ReferencedTable="6c977939-bbfc-456f-a133-f1c2244e3cc3">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="c636f834-6a7a-002d-4000-05cc563223b1" Name="UserID" Type="Guid Not Null" ReferencedColumn="6c977939-bbfc-016f-4000-01c2244e3cc3" />
		<SchemeReferencingColumn ID="dc8f8aef-20e8-4c03-b05d-10d25fdedcfb" Name="UserName" Type="String(128) Not Null" ReferencedColumn="1782f76a-4743-4aa4-920c-7edaee860964" />
	</SchemeComplexColumn>
	<SchemePrimaryKey IsSystem="true" IsPermanent="true" IsSealed="true" ID="b49a1639-2158-0017-5000-02a42fe6c7b8" Name="pk_PnrConfidants">
		<SchemeIndexedColumn Column="b49a1639-2158-0017-3100-02a42fe6c7b8" />
	</SchemePrimaryKey>
	<SchemeIndex IsSystem="true" IsPermanent="true" IsSealed="true" ID="b49a1639-2158-0017-7000-02a42fe6c7b8" Name="idx_PnrConfidants_ID" IsClustered="true">
		<SchemeIndexedColumn Column="b49a1639-2158-0117-4000-02a42fe6c7b8" />
	</SchemeIndex>
</SchemeTable>