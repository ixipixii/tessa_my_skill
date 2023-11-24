<?xml version="1.0" encoding="utf-8"?>
<SchemeTable Partition="54ed79f5-11c0-4f74-b8b0-793d596699d6" ID="9ab27608-ec6a-4c55-badb-b53e915e665f" Name="PnrTenderReviewers" Group="Pnr" InstanceType="Cards" ContentType="Collections">
	<Description>Рецензирующие (Тендер)</Description>
	<SchemeComplexColumn IsSystem="true" IsPermanent="true" IsSealed="true" ID="9ab27608-ec6a-0055-2000-053e915e665f" Name="ID" Type="Reference(Typified) Not Null" ReferencedTable="1074eadd-21d7-4925-98c8-40d1e5f0ca0e">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="9ab27608-ec6a-0155-4000-053e915e665f" Name="ID" Type="Guid Not Null" ReferencedColumn="9a58123b-b2e9-4137-9c6c-5dab0ec02747" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn IsSystem="true" IsPermanent="true" IsSealed="true" ID="9ab27608-ec6a-0055-3100-053e915e665f" Name="RowID" Type="Guid Not Null" />
	<SchemeComplexColumn ID="ecf36317-7766-4739-9ac0-7246a89cfd4e" Name="Reviewer" Type="Reference(Typified) Null" ReferencedTable="6c977939-bbfc-456f-a133-f1c2244e3cc3">
		<Description>Рецензент</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="ecf36317-7766-0039-4000-0246a89cfd4e" Name="ReviewerID" Type="Guid Null" ReferencedColumn="6c977939-bbfc-016f-4000-01c2244e3cc3" />
		<SchemeReferencingColumn ID="9db62311-9e50-44f7-aabb-dbba9f725317" Name="ReviewerName" Type="String(128) Null" ReferencedColumn="1782f76a-4743-4aa4-920c-7edaee860964" />
	</SchemeComplexColumn>
	<SchemePrimaryKey IsSystem="true" IsPermanent="true" IsSealed="true" ID="9ab27608-ec6a-0055-5000-053e915e665f" Name="pk_PnrTenderReviewers">
		<SchemeIndexedColumn Column="9ab27608-ec6a-0055-3100-053e915e665f" />
	</SchemePrimaryKey>
	<SchemeIndex IsSystem="true" IsPermanent="true" IsSealed="true" ID="9ab27608-ec6a-0055-7000-053e915e665f" Name="idx_PnrTenderReviewers_ID" IsClustered="true">
		<SchemeIndexedColumn Column="9ab27608-ec6a-0155-4000-053e915e665f" />
	</SchemeIndex>
</SchemeTable>