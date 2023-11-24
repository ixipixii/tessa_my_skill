<?xml version="1.0" encoding="utf-8"?>
<SchemeTable Partition="54ed79f5-11c0-4f74-b8b0-793d596699d6" ID="e960e5a2-1b04-4d14-9455-73428558a2e3" Name="PnrCategoryCard" Group="Pnr" InstanceType="Cards" ContentType="Collections">
	<SchemeComplexColumn IsSystem="true" IsPermanent="true" IsSealed="true" ID="e960e5a2-1b04-0014-2000-03428558a2e3" Name="ID" Type="Reference(Typified) Not Null" ReferencedTable="1074eadd-21d7-4925-98c8-40d1e5f0ca0e">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="e960e5a2-1b04-0114-4000-03428558a2e3" Name="ID" Type="Guid Not Null" ReferencedColumn="9a58123b-b2e9-4137-9c6c-5dab0ec02747" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn IsSystem="true" IsPermanent="true" IsSealed="true" ID="e960e5a2-1b04-0014-3100-03428558a2e3" Name="RowID" Type="Guid Not Null" />
	<SchemeComplexColumn ID="064ff2dc-90d2-46c0-9a03-6514d7bae213" Name="Category" Type="Reference(Typified) Not Null" ReferencedTable="e1599715-02d4-4ca9-b63e-b4b1ce642c7a">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="064ff2dc-90d2-00c0-4000-0514d7bae213" Name="CategoryID" Type="Guid Not Null" ReferencedColumn="e1599715-02d4-01a9-4000-04b1ce642c7a" />
		<SchemeReferencingColumn ID="6da280f2-cd25-4797-8465-d260521ee641" Name="CategoryName" Type="String(255) Not Null" ReferencedColumn="e2598c40-038d-4af4-9907-f2514170cc4d" />
	</SchemeComplexColumn>
	<SchemePrimaryKey IsSystem="true" IsPermanent="true" IsSealed="true" ID="e960e5a2-1b04-0014-5000-03428558a2e3" Name="pk_PnrCategoryCard">
		<SchemeIndexedColumn Column="e960e5a2-1b04-0014-3100-03428558a2e3" />
	</SchemePrimaryKey>
	<SchemeIndex IsSystem="true" IsPermanent="true" IsSealed="true" ID="e960e5a2-1b04-0014-7000-03428558a2e3" Name="idx_PnrCategoryCard_ID" IsClustered="true">
		<SchemeIndexedColumn Column="e960e5a2-1b04-0114-4000-03428558a2e3" />
	</SchemeIndex>
</SchemeTable>