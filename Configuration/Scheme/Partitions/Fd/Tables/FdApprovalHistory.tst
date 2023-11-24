<?xml version="1.0" encoding="utf-8"?>
<SchemeTable Partition="c3b14969-a553-4005-9a07-e9705c15b1e1" ID="f927200e-b67a-4e4e-b321-f985d5f4c62b" Name="FdApprovalHistory" Group="Fd" InstanceType="Cards" ContentType="Collections">
	<Description>Сопоставление истории заданий с историей согласования (с учетом циклов согласования)</Description>
	<SchemeComplexColumn IsSystem="true" IsPermanent="true" IsSealed="true" ID="f927200e-b67a-004e-2000-0985d5f4c62b" Name="ID" Type="Reference(Typified) Not Null" ReferencedTable="1074eadd-21d7-4925-98c8-40d1e5f0ca0e">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="f927200e-b67a-014e-4000-0985d5f4c62b" Name="ID" Type="Guid Not Null" ReferencedColumn="9a58123b-b2e9-4137-9c6c-5dab0ec02747" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn IsSystem="true" IsPermanent="true" IsSealed="true" ID="f927200e-b67a-004e-3100-0985d5f4c62b" Name="RowID" Type="Guid Not Null" />
	<SchemePhysicalColumn ID="27a01c1a-3159-4273-9031-67c1ea0fa33f" Name="Cycle" Type="Int16 Not Null">
		<Description>Порядковый номер цикла согласования</Description>
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="ee52465e-0b5b-464c-99a2-11991bf90e9d" Name="HistoryRecord" Type="Guid Null">
		<Description>Ссылка на задание согласования в общей истории заданий</Description>
	</SchemePhysicalColumn>
	<SchemePrimaryKey IsSystem="true" IsPermanent="true" IsSealed="true" ID="f927200e-b67a-004e-5000-0985d5f4c62b" Name="pk_FdApprovalHistory">
		<SchemeIndexedColumn Column="f927200e-b67a-004e-3100-0985d5f4c62b" />
	</SchemePrimaryKey>
	<SchemeIndex IsSystem="true" IsPermanent="true" IsSealed="true" ID="f927200e-b67a-004e-7000-0985d5f4c62b" Name="idx_FdApprovalHistory_ID" IsClustered="true">
		<SchemeIndexedColumn Column="f927200e-b67a-014e-4000-0985d5f4c62b" />
	</SchemeIndex>
</SchemeTable>