<?xml version="1.0" encoding="utf-8"?>
<SchemeTable Partition="c3b14969-a553-4005-9a07-e9705c15b1e1" ID="d711ff15-3ef7-431d-92f8-82cd2ce2b79c" Name="FdApprovalHistoryVirtual" Group="Fd" IsVirtual="true" InstanceType="Cards" ContentType="Collections">
	<SchemeComplexColumn IsSystem="true" IsPermanent="true" IsSealed="true" ID="d711ff15-3ef7-001d-2000-02cd2ce2b79c" Name="ID" Type="Reference(Typified) Not Null" ReferencedTable="1074eadd-21d7-4925-98c8-40d1e5f0ca0e">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="d711ff15-3ef7-011d-4000-02cd2ce2b79c" Name="ID" Type="Guid Not Null" ReferencedColumn="9a58123b-b2e9-4137-9c6c-5dab0ec02747" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn IsSystem="true" IsPermanent="true" IsSealed="true" ID="d711ff15-3ef7-001d-3100-02cd2ce2b79c" Name="RowID" Type="Guid Not Null" />
	<SchemePhysicalColumn ID="77cad123-5e46-4ca5-b53e-02efc7efce4a" Name="Cycle" Type="Int16 Not Null" />
	<SchemePhysicalColumn ID="e350a2df-ebbe-4363-a1df-f303f56721e6" Name="HistoryRecord" Type="Guid Null" />
	<SchemePrimaryKey IsSystem="true" IsPermanent="true" IsSealed="true" ID="d711ff15-3ef7-001d-5000-02cd2ce2b79c" Name="pk_FdApprovalHistoryVirtual">
		<SchemeIndexedColumn Column="d711ff15-3ef7-001d-3100-02cd2ce2b79c" />
	</SchemePrimaryKey>
	<SchemeIndex IsSystem="true" IsPermanent="true" IsSealed="true" ID="d711ff15-3ef7-001d-7000-02cd2ce2b79c" Name="idx_FdApprovalHistoryVirtual_ID" IsClustered="true">
		<SchemeIndexedColumn Column="d711ff15-3ef7-011d-4000-02cd2ce2b79c" />
	</SchemeIndex>
</SchemeTable>