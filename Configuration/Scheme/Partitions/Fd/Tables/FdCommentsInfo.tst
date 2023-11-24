<?xml version="1.0" encoding="utf-8"?>
<SchemeTable Partition="c3b14969-a553-4005-9a07-e9705c15b1e1" ID="6fef35d6-2780-4f6e-94b3-b6499057d7f7" Name="FdCommentsInfo" Group="Fd" InstanceType="Tasks" ContentType="Collections">
	<SchemeComplexColumn IsSystem="true" IsPermanent="true" IsSealed="true" ID="6fef35d6-2780-006e-2000-06499057d7f7" Name="ID" Type="Reference(Typified) Not Null" ReferencedTable="5bfa9936-bb5a-4e8f-89a9-180bfd8f75f8">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="6fef35d6-2780-016e-4000-06499057d7f7" Name="ID" Type="Guid Not Null" ReferencedColumn="5bfa9936-bb5a-008f-3100-080bfd8f75f8" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn IsSystem="true" IsPermanent="true" IsSealed="true" ID="6fef35d6-2780-006e-3100-06499057d7f7" Name="RowID" Type="Guid Not Null" />
	<SchemePhysicalColumn ID="4fc7e28c-c0a6-4a66-8994-92404ce8d7cc" Name="Question" Type="String(Max) Null">
		<Description>Вопрос к комментатору</Description>
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="16c8fd14-1123-43e9-9d41-eaa82c2d2694" Name="Answer" Type="String(Max) Null">
		<Description>Ответ комментатора на вопрос</Description>
	</SchemePhysicalColumn>
	<SchemeComplexColumn ID="35f56597-1cd3-4a72-a3e1-53dc5341a611" Name="Commentator" Type="Reference(Typified) Not Null" ReferencedTable="81f6010b-9641-4aa5-8897-b8e8603fbf4b" WithForeignKey="false">
		<Description>Комментатор</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="35f56597-1cd3-0072-4000-03dc5341a611" Name="CommentatorID" Type="Guid Not Null" ReferencedColumn="81f6010b-9641-01a5-4000-08e8603fbf4b" />
		<SchemeReferencingColumn ID="2254c964-d12b-4240-b306-1aee2eb67230" Name="CommentatorName" Type="String(128) Not Null" ReferencedColumn="616d6b2e-35d5-424d-846b-618eb25962d0" />
	</SchemeComplexColumn>
	<SchemePrimaryKey IsSystem="true" IsPermanent="true" IsSealed="true" ID="6fef35d6-2780-006e-5000-06499057d7f7" Name="pk_FdCommentsInfo">
		<SchemeIndexedColumn Column="6fef35d6-2780-006e-3100-06499057d7f7" />
	</SchemePrimaryKey>
	<SchemeIndex IsSystem="true" IsPermanent="true" IsSealed="true" ID="6fef35d6-2780-006e-7000-06499057d7f7" Name="idx_FdCommentsInfo_ID" IsClustered="true">
		<SchemeIndexedColumn Column="6fef35d6-2780-016e-4000-06499057d7f7" />
	</SchemeIndex>
</SchemeTable>