<?xml version="1.0" encoding="utf-8"?>
<SchemeTable Partition="c3b14969-a553-4005-9a07-e9705c15b1e1" ID="1f767351-3356-47e6-ae68-878c4e1bb832" Name="FdCommentators" Group="Fd" InstanceType="Tasks" ContentType="Collections">
	<SchemeComplexColumn IsSystem="true" IsPermanent="true" IsSealed="true" ID="1f767351-3356-00e6-2000-078c4e1bb832" Name="ID" Type="Reference(Typified) Not Null" ReferencedTable="5bfa9936-bb5a-4e8f-89a9-180bfd8f75f8">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="1f767351-3356-01e6-4000-078c4e1bb832" Name="ID" Type="Guid Not Null" ReferencedColumn="5bfa9936-bb5a-008f-3100-080bfd8f75f8" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn IsSystem="true" IsPermanent="true" IsSealed="true" ID="1f767351-3356-00e6-3100-078c4e1bb832" Name="RowID" Type="Guid Not Null" />
	<SchemeComplexColumn ID="bcc7d4e1-9e0b-4d8c-98a3-c1324665c97f" Name="Commentator" Type="Reference(Typified) Not Null" ReferencedTable="6c977939-bbfc-456f-a133-f1c2244e3cc3">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="bcc7d4e1-9e0b-008c-4000-01324665c97f" Name="CommentatorID" Type="Guid Not Null" ReferencedColumn="6c977939-bbfc-016f-4000-01c2244e3cc3" />
		<SchemeReferencingColumn ID="05bf3196-8505-45d6-869d-aeb88b0d2f68" Name="CommentatorName" Type="String(128) Not Null" ReferencedColumn="1782f76a-4743-4aa4-920c-7edaee860964" />
	</SchemeComplexColumn>
	<SchemePrimaryKey IsSystem="true" IsPermanent="true" IsSealed="true" ID="1f767351-3356-00e6-5000-078c4e1bb832" Name="pk_FdCommentators">
		<SchemeIndexedColumn Column="1f767351-3356-00e6-3100-078c4e1bb832" />
	</SchemePrimaryKey>
	<SchemeIndex IsSystem="true" IsPermanent="true" IsSealed="true" ID="1f767351-3356-00e6-7000-078c4e1bb832" Name="idx_FdCommentators_ID" IsClustered="true">
		<SchemeIndexedColumn Column="1f767351-3356-01e6-4000-078c4e1bb832" />
	</SchemeIndex>
</SchemeTable>