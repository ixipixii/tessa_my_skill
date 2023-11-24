<?xml version="1.0" encoding="utf-8"?>
<SchemeTable Partition="c3b14969-a553-4005-9a07-e9705c15b1e1" ID="850b3e3b-dab3-4e10-b0d3-eccae3f4b7a2" Name="FdRequestComment" Group="Fd" InstanceType="Tasks" ContentType="Entries">
	<SchemeComplexColumn IsSystem="true" IsPermanent="true" IsSealed="true" ID="850b3e3b-dab3-0010-2000-0ccae3f4b7a2" Name="ID" Type="Reference(Typified) Not Null" ReferencedTable="5bfa9936-bb5a-4e8f-89a9-180bfd8f75f8">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="850b3e3b-dab3-0110-4000-0ccae3f4b7a2" Name="ID" Type="Guid Not Null" ReferencedColumn="5bfa9936-bb5a-008f-3100-080bfd8f75f8" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn ID="5eccd3a3-e8cd-4b6c-8e3f-ebc9bbe755a2" Name="Comment" Type="String(4000) Null">
		<Description>Ответ комментирующего на вопрос согласанта</Description>
	</SchemePhysicalColumn>
	<SchemeComplexColumn ID="e235cda0-06e5-49b6-a546-33cf4661ea6e" Name="AuthorRole" Type="Reference(Typified) Not Null" ReferencedTable="81f6010b-9641-4aa5-8897-b8e8603fbf4b" WithForeignKey="false">
		<Description>Первоначальная роль согласанта, который запрашивает комментарий. Требуется для восстановления задания согласования после получения запрошенного комментария.</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="e235cda0-06e5-00b6-4000-03cf4661ea6e" Name="AuthorRoleID" Type="Guid Not Null" ReferencedColumn="81f6010b-9641-01a5-4000-08e8603fbf4b" />
		<SchemeReferencingColumn ID="0601c72f-9994-4608-9957-4128400eb464" Name="AuthorRoleName" Type="String(128) Not Null" ReferencedColumn="616d6b2e-35d5-424d-846b-618eb25962d0" />
	</SchemeComplexColumn>
	<SchemePrimaryKey IsSystem="true" IsPermanent="true" IsSealed="true" ID="850b3e3b-dab3-0010-5000-0ccae3f4b7a2" Name="pk_FdRequestComment" IsClustered="true">
		<SchemeIndexedColumn Column="850b3e3b-dab3-0110-4000-0ccae3f4b7a2" />
	</SchemePrimaryKey>
</SchemeTable>