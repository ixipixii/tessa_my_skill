<?xml version="1.0" encoding="utf-8"?>
<SchemeTable Partition="54ed79f5-11c0-4f74-b8b0-793d596699d6" ID="194fafa8-1ee6-4bc4-8cfd-d0bb8b205427" Name="PnrConfidantsDepartmentsHeads" Group="Pnr" InstanceType="Cards" ContentType="Collections">
	<Description>Руководители дирекций доверенных лиц</Description>
	<SchemeComplexColumn IsSystem="true" IsPermanent="true" IsSealed="true" ID="194fafa8-1ee6-00c4-2000-00bb8b205427" Name="ID" Type="Reference(Typified) Not Null" ReferencedTable="1074eadd-21d7-4925-98c8-40d1e5f0ca0e">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="194fafa8-1ee6-01c4-4000-00bb8b205427" Name="ID" Type="Guid Not Null" ReferencedColumn="9a58123b-b2e9-4137-9c6c-5dab0ec02747" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn IsSystem="true" IsPermanent="true" IsSealed="true" ID="194fafa8-1ee6-00c4-3100-00bb8b205427" Name="RowID" Type="Guid Not Null" />
	<SchemeComplexColumn ID="2d352ab3-35b7-4e00-ae48-c39e6daf2916" Name="User" Type="Reference(Typified) Not Null" ReferencedTable="6c977939-bbfc-456f-a133-f1c2244e3cc3">
		<Description>Руководитель</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="2d352ab3-35b7-0000-4000-039e6daf2916" Name="UserID" Type="Guid Not Null" ReferencedColumn="6c977939-bbfc-016f-4000-01c2244e3cc3" />
		<SchemeReferencingColumn ID="88c8128b-a511-4e60-8cfa-b41ac58df313" Name="UserName" Type="String(128) Not Null" ReferencedColumn="1782f76a-4743-4aa4-920c-7edaee860964" />
	</SchemeComplexColumn>
	<SchemePrimaryKey IsSystem="true" IsPermanent="true" IsSealed="true" ID="194fafa8-1ee6-00c4-5000-00bb8b205427" Name="pk_PnrConfidantsDepartmentsHeads">
		<SchemeIndexedColumn Column="194fafa8-1ee6-00c4-3100-00bb8b205427" />
	</SchemePrimaryKey>
	<SchemeIndex IsSystem="true" IsPermanent="true" IsSealed="true" ID="194fafa8-1ee6-00c4-7000-00bb8b205427" Name="idx_PnrConfidantsDepartmentsHeads_ID" IsClustered="true">
		<SchemeIndexedColumn Column="194fafa8-1ee6-01c4-4000-00bb8b205427" />
	</SchemeIndex>
</SchemeTable>