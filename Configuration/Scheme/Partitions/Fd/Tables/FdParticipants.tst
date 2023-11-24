<?xml version="1.0" encoding="utf-8"?>
<SchemeTable Partition="c3b14969-a553-4005-9a07-e9705c15b1e1" ID="c1da48a2-f92f-4cc5-9652-48ecc7450903" Name="FdParticipants" Group="Fd" InstanceType="Cards" ContentType="Collections">
	<Description>Участники</Description>
	<SchemeComplexColumn IsSystem="true" IsPermanent="true" IsSealed="true" ID="c1da48a2-f92f-00c5-2000-08ecc7450903" Name="ID" Type="Reference(Typified) Not Null" ReferencedTable="1074eadd-21d7-4925-98c8-40d1e5f0ca0e">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="c1da48a2-f92f-01c5-4000-08ecc7450903" Name="ID" Type="Guid Not Null" ReferencedColumn="9a58123b-b2e9-4137-9c6c-5dab0ec02747" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn IsSystem="true" IsPermanent="true" IsSealed="true" ID="c1da48a2-f92f-00c5-3100-08ecc7450903" Name="RowID" Type="Guid Not Null" />
	<SchemeComplexColumn ID="aabac664-fc5e-4e5f-959f-5b84b767273a" Name="Participant" Type="Reference(Typified) Not Null" ReferencedTable="81f6010b-9641-4aa5-8897-b8e8603fbf4b">
		<Description>Участник (роль)</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="aabac664-fc5e-005f-4000-0b84b767273a" Name="ParticipantID" Type="Guid Not Null" ReferencedColumn="81f6010b-9641-01a5-4000-08e8603fbf4b" />
		<SchemeReferencingColumn ID="204db614-c8df-4155-9478-bb8f61bb5b30" Name="ParticipantName" Type="String(128) Not Null" ReferencedColumn="616d6b2e-35d5-424d-846b-618eb25962d0" />
	</SchemeComplexColumn>
	<SchemeComplexColumn ID="483150a9-5a5e-4b95-a554-94809276ea4d" Name="Process" Type="Reference(Typified) Not Null" ReferencedTable="2114510a-e165-4491-afcc-1756400e30a0">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="483150a9-5a5e-0095-4000-04809276ea4d" Name="ProcessRowID" Type="Guid Not Null" ReferencedColumn="2114510a-e165-0091-3100-0756400e30a0" />
	</SchemeComplexColumn>
	<SchemeComplexColumn ID="dcc35e84-7ef8-47b4-9e52-36ad3c57297a" Name="Stage" Type="Reference(Typified) Not Null" ReferencedTable="ffced5f3-fea9-41e1-bcc6-b156b3e3c054" DeleteReferentialAction="Cascade">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="dcc35e84-7ef8-00b4-4000-06ad3c57297a" Name="StageRowID" Type="Guid Not Null" ReferencedColumn="ffced5f3-fea9-00e1-3100-0156b3e3c054" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn ID="fea1781d-8d9f-47a4-8fe6-40b5ecc1d321" Name="Order" Type="Int32 Not Null">
		<SchemeDefaultConstraint IsPermanent="true" ID="99c61d52-44ba-4720-ae90-11e2d87ba646" Name="df_FdParticipants_Order" Value="0" />
	</SchemePhysicalColumn>
	<SchemeComplexColumn ID="0c772e97-e2ff-42ed-aa81-a72247eaaa96" Name="StageTemplateParticipant" Type="Reference(Typified) Null" ReferencedTable="b9d1e0af-06c0-4c17-9e83-0497464e193b" WithForeignKey="false">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="0c772e97-e2ff-00ed-4000-072247eaaa96" Name="StageTemplateParticipantRowID" Type="Guid Null" ReferencedColumn="b9d1e0af-06c0-0017-3100-0497464e193b" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn ID="df63f6a1-0d5b-459a-9ba8-581a19596a65" Name="TaskTimeLimitationDaysOffset" Type="Double Null">
		<Description>Итоговое время на выполнение задания в рабочих днях заданного участника</Description>
	</SchemePhysicalColumn>
	<SchemeComplexColumn ID="f96babb2-5d0a-4166-835e-a34647b84683" Name="Notification" Type="Reference(Typified) Null" ReferencedTable="18145bb5-fd4e-4795-aa1f-9e1cd9b4ee5a">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="f96babb2-5d0a-0066-4000-034647b84683" Name="NotificationID" Type="Guid Null" ReferencedColumn="18145bb5-fd4e-0195-4000-0e1cd9b4ee5a" />
		<SchemeReferencingColumn ID="571e98f6-c743-406a-8f48-b535e7a23373" Name="NotificationName" Type="String(256) Null" ReferencedColumn="265d4336-6764-4db8-8874-0e5fa92cbd5d" />
	</SchemeComplexColumn>
	<SchemePrimaryKey IsSystem="true" IsPermanent="true" IsSealed="true" ID="c1da48a2-f92f-00c5-5000-08ecc7450903" Name="pk_FdParticipants">
		<SchemeIndexedColumn Column="c1da48a2-f92f-00c5-3100-08ecc7450903" />
	</SchemePrimaryKey>
	<SchemeIndex IsSystem="true" IsPermanent="true" IsSealed="true" ID="c1da48a2-f92f-00c5-7000-08ecc7450903" Name="idx_FdParticipants_ID" IsClustered="true">
		<SchemeIndexedColumn Column="c1da48a2-f92f-01c5-4000-08ecc7450903" />
	</SchemeIndex>
	<SchemeIndex ID="cae7edfd-cee1-4aef-ba94-702c51f9c28a" Name="ndx_FdParticipants_ParticipantID">
		<SchemeIndexedColumn Column="aabac664-fc5e-005f-4000-0b84b767273a" />
	</SchemeIndex>
</SchemeTable>