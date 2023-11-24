<?xml version="1.0" encoding="utf-8"?>
<SchemeTable ID="4eaacebc-e201-40d7-8ac7-5c8db3e04c24" Name="FmTopicParticipantsVirtual" Group="Fm" IsVirtual="true" InstanceType="Cards" ContentType="Collections">
	<Description>Виртуальные секции для формы заполнения участниками топика</Description>
	<SchemeComplexColumn IsSystem="true" IsPermanent="true" IsSealed="true" ID="4eaacebc-e201-00d7-2000-0c8db3e04c24" Name="ID" Type="Reference(Typified) Not Null" ReferencedTable="1074eadd-21d7-4925-98c8-40d1e5f0ca0e">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="4eaacebc-e201-01d7-4000-0c8db3e04c24" Name="ID" Type="Guid Not Null" ReferencedColumn="9a58123b-b2e9-4137-9c6c-5dab0ec02747" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn IsSystem="true" IsPermanent="true" IsSealed="true" ID="4eaacebc-e201-00d7-3100-0c8db3e04c24" Name="RowID" Type="Guid Not Null" />
	<SchemeComplexColumn ID="bdea9c1d-91a0-44cd-9a32-87b54b76d058" Name="Participant" Type="Reference(Typified) Not Null" ReferencedTable="81f6010b-9641-4aa5-8897-b8e8603fbf4b">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="bdea9c1d-91a0-00cd-4000-07b54b76d058" Name="ParticipantID" Type="Guid Not Null" ReferencedColumn="81f6010b-9641-01a5-4000-08e8603fbf4b" />
		<SchemeReferencingColumn ID="ac198708-8484-431a-83e7-ba8691eee0c3" Name="ParticipantName" Type="String(128) Not Null" ReferencedColumn="616d6b2e-35d5-424d-846b-618eb25962d0" />
	</SchemeComplexColumn>
	<SchemePrimaryKey IsSystem="true" IsPermanent="true" IsSealed="true" ID="4eaacebc-e201-00d7-5000-0c8db3e04c24" Name="pk_FmTopicParticipantsVirtual">
		<SchemeIndexedColumn Column="4eaacebc-e201-00d7-3100-0c8db3e04c24" />
	</SchemePrimaryKey>
	<SchemeIndex IsSystem="true" IsPermanent="true" IsSealed="true" ID="4eaacebc-e201-00d7-7000-0c8db3e04c24" Name="idx_FmTopicParticipantsVirtual_ID" IsClustered="true">
		<SchemeIndexedColumn Column="4eaacebc-e201-01d7-4000-0c8db3e04c24" />
	</SchemeIndex>
</SchemeTable>