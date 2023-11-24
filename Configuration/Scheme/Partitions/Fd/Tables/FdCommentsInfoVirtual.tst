<?xml version="1.0" encoding="utf-8"?>
<SchemeTable Partition="c3b14969-a553-4005-9a07-e9705c15b1e1" ID="2af02df8-fe68-4c6b-8a9c-7ee515f81117" Name="FdCommentsInfoVirtual" Group="Fd" IsVirtual="true" InstanceType="Tasks" ContentType="Collections">
	<SchemeComplexColumn IsSystem="true" IsPermanent="true" IsSealed="true" ID="2af02df8-fe68-006b-2000-0ee515f81117" Name="ID" Type="Reference(Typified) Not Null" ReferencedTable="5bfa9936-bb5a-4e8f-89a9-180bfd8f75f8">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="2af02df8-fe68-016b-4000-0ee515f81117" Name="ID" Type="Guid Not Null" ReferencedColumn="5bfa9936-bb5a-008f-3100-080bfd8f75f8" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn IsSystem="true" IsPermanent="true" IsSealed="true" ID="2af02df8-fe68-006b-3100-0ee515f81117" Name="RowID" Type="Guid Not Null" />
	<SchemePhysicalColumn ID="caa30b46-06d2-4bfe-a207-67c466ca93e9" Name="QuestionShort" Type="String(32) Null" />
	<SchemePhysicalColumn ID="e270f237-3e51-474d-99ee-a06b3d0f06cc" Name="AnswerShort" Type="String(32) Null" />
	<SchemePhysicalColumn ID="10619bb1-4623-486e-856f-cea32e04b660" Name="CommentatorNameShort" Type="String(32) Null" />
	<SchemePhysicalColumn ID="210b2d97-a9b8-4641-b724-60e210528aaa" Name="QuestionFull" Type="String(Max) Null" />
	<SchemePhysicalColumn ID="4d0996fd-37aa-472c-909e-a7956c1cbab9" Name="AnswerFull" Type="String(Max) Null" />
	<SchemePhysicalColumn ID="2ff86d3e-f7fe-4747-aaa9-154a7de4b0d4" Name="CommentatorNameFull" Type="String(Max) Null" />
	<SchemePhysicalColumn ID="baa14f2a-8fff-4274-82f7-3bda68b35d46" Name="Completed" Type="DateTime Null">
		<Description>Время завершения задания комментирования</Description>
	</SchemePhysicalColumn>
	<SchemePrimaryKey IsSystem="true" IsPermanent="true" IsSealed="true" ID="2af02df8-fe68-006b-5000-0ee515f81117" Name="pk_FdCommentsInfoVirtual">
		<SchemeIndexedColumn Column="2af02df8-fe68-006b-3100-0ee515f81117" />
	</SchemePrimaryKey>
	<SchemeIndex IsSystem="true" IsPermanent="true" IsSealed="true" ID="2af02df8-fe68-006b-7000-0ee515f81117" Name="idx_FdCommentsInfoVirtual_ID" IsClustered="true">
		<SchemeIndexedColumn Column="2af02df8-fe68-016b-4000-0ee515f81117" />
	</SchemeIndex>
</SchemeTable>