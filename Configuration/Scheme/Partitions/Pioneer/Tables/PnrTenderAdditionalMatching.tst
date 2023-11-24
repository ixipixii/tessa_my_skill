<?xml version="1.0" encoding="utf-8"?>
<SchemeTable Partition="54ed79f5-11c0-4f74-b8b0-793d596699d6" ID="11a4e2e1-6297-40b4-9a92-7bd20a981081" Name="PnrTenderAdditionalMatching" Group="Pnr" InstanceType="Cards" ContentType="Collections">
	<Description>Дополнительные согласующие (Тендер)</Description>
	<SchemeComplexColumn IsSystem="true" IsPermanent="true" IsSealed="true" ID="11a4e2e1-6297-00b4-2000-0bd20a981081" Name="ID" Type="Reference(Typified) Not Null" ReferencedTable="1074eadd-21d7-4925-98c8-40d1e5f0ca0e">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="11a4e2e1-6297-01b4-4000-0bd20a981081" Name="ID" Type="Guid Not Null" ReferencedColumn="9a58123b-b2e9-4137-9c6c-5dab0ec02747" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn IsSystem="true" IsPermanent="true" IsSealed="true" ID="11a4e2e1-6297-00b4-3100-0bd20a981081" Name="RowID" Type="Guid Not Null" />
	<SchemeComplexColumn ID="7e7bc363-6c0c-4e68-902b-fba4f6d824f2" Name="User" Type="Reference(Typified) Null" ReferencedTable="6c977939-bbfc-456f-a133-f1c2244e3cc3">
		<Description>Согласующий</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="7e7bc363-6c0c-0068-4000-0ba4f6d824f2" Name="UserID" Type="Guid Null" ReferencedColumn="6c977939-bbfc-016f-4000-01c2244e3cc3" />
		<SchemeReferencingColumn ID="4b1efd7a-7ebf-49e9-b323-81d96ce8df1e" Name="UserName" Type="String(128) Null" ReferencedColumn="1782f76a-4743-4aa4-920c-7edaee860964" />
	</SchemeComplexColumn>
	<SchemePrimaryKey IsSystem="true" IsPermanent="true" IsSealed="true" ID="11a4e2e1-6297-00b4-5000-0bd20a981081" Name="pk_PnrTenderAdditionalMatching">
		<SchemeIndexedColumn Column="11a4e2e1-6297-00b4-3100-0bd20a981081" />
	</SchemePrimaryKey>
	<SchemeIndex IsSystem="true" IsPermanent="true" IsSealed="true" ID="11a4e2e1-6297-00b4-7000-0bd20a981081" Name="idx_PnrTenderAdditionalMatching_ID" IsClustered="true">
		<SchemeIndexedColumn Column="11a4e2e1-6297-01b4-4000-0bd20a981081" />
	</SchemeIndex>
</SchemeTable>