<?xml version="1.0" encoding="utf-8"?>
<SchemeTable Partition="54ed79f5-11c0-4f74-b8b0-793d596699d6" ID="bc28eb42-9be5-4391-84af-80c96ded8b20" Name="PnrApprovingPersons" Group="Pnr" InstanceType="Cards" ContentType="Collections">
	<Description>Согласующие</Description>
	<SchemeComplexColumn IsSystem="true" IsPermanent="true" IsSealed="true" ID="bc28eb42-9be5-0091-2000-00c96ded8b20" Name="ID" Type="Reference(Typified) Not Null" ReferencedTable="1074eadd-21d7-4925-98c8-40d1e5f0ca0e">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="bc28eb42-9be5-0191-4000-00c96ded8b20" Name="ID" Type="Guid Not Null" ReferencedColumn="9a58123b-b2e9-4137-9c6c-5dab0ec02747" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn IsSystem="true" IsPermanent="true" IsSealed="true" ID="bc28eb42-9be5-0091-3100-00c96ded8b20" Name="RowID" Type="Guid Not Null" />
	<SchemeComplexColumn ID="da6aad67-a42f-4d8f-a9bf-769a6be9e68a" Name="User" Type="Reference(Typified) Null" ReferencedTable="6c977939-bbfc-456f-a133-f1c2244e3cc3">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="da6aad67-a42f-008f-4000-069a6be9e68a" Name="UserID" Type="Guid Null" ReferencedColumn="6c977939-bbfc-016f-4000-01c2244e3cc3" />
		<SchemeReferencingColumn ID="307f0dad-2851-4fb3-aa74-68c293765422" Name="UserName" Type="String(128) Null" ReferencedColumn="1782f76a-4743-4aa4-920c-7edaee860964" />
	</SchemeComplexColumn>
	<SchemePrimaryKey IsSystem="true" IsPermanent="true" IsSealed="true" ID="bc28eb42-9be5-0091-5000-00c96ded8b20" Name="pk_PnrApprovingPersons">
		<SchemeIndexedColumn Column="bc28eb42-9be5-0091-3100-00c96ded8b20" />
	</SchemePrimaryKey>
	<SchemeIndex IsSystem="true" IsPermanent="true" IsSealed="true" ID="bc28eb42-9be5-0091-7000-00c96ded8b20" Name="idx_PnrApprovingPersons_ID" IsClustered="true">
		<SchemeIndexedColumn Column="bc28eb42-9be5-0191-4000-00c96ded8b20" />
	</SchemeIndex>
</SchemeTable>