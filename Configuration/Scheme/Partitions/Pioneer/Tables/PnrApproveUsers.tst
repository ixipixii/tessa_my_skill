<?xml version="1.0" encoding="utf-8"?>
<SchemeTable Partition="54ed79f5-11c0-4f74-b8b0-793d596699d6" ID="62c530b3-e701-4596-b1ae-126173ed1572" Name="PnrApproveUsers" Group="PnrTask" InstanceType="Tasks" ContentType="Collections">
	<Description>Согласующие</Description>
	<SchemeComplexColumn IsSystem="true" IsPermanent="true" IsSealed="true" ID="62c530b3-e701-0096-2000-026173ed1572" Name="ID" Type="Reference(Typified) Not Null" ReferencedTable="5bfa9936-bb5a-4e8f-89a9-180bfd8f75f8">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="62c530b3-e701-0196-4000-026173ed1572" Name="ID" Type="Guid Not Null" ReferencedColumn="5bfa9936-bb5a-008f-3100-080bfd8f75f8" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn IsSystem="true" IsPermanent="true" IsSealed="true" ID="62c530b3-e701-0096-3100-026173ed1572" Name="RowID" Type="Guid Not Null" />
	<SchemeComplexColumn ID="33fda9c4-f165-4fa3-af12-0c1e8ac6ac6b" Name="User" Type="Reference(Typified) Null" ReferencedTable="6c977939-bbfc-456f-a133-f1c2244e3cc3">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="33fda9c4-f165-00a3-4000-0c1e8ac6ac6b" Name="UserID" Type="Guid Null" ReferencedColumn="6c977939-bbfc-016f-4000-01c2244e3cc3" />
		<SchemeReferencingColumn ID="b4fcd896-ac57-42dc-80eb-909fdba33715" Name="UserName" Type="String(128) Null" ReferencedColumn="1782f76a-4743-4aa4-920c-7edaee860964" />
	</SchemeComplexColumn>
	<SchemePrimaryKey IsSystem="true" IsPermanent="true" IsSealed="true" ID="62c530b3-e701-0096-5000-026173ed1572" Name="pk_PnrApproveUsers">
		<SchemeIndexedColumn Column="62c530b3-e701-0096-3100-026173ed1572" />
	</SchemePrimaryKey>
	<SchemeIndex IsSystem="true" IsPermanent="true" IsSealed="true" ID="62c530b3-e701-0096-7000-026173ed1572" Name="idx_PnrApproveUsers_ID" IsClustered="true">
		<SchemeIndexedColumn Column="62c530b3-e701-0196-4000-026173ed1572" />
	</SchemeIndex>
</SchemeTable>