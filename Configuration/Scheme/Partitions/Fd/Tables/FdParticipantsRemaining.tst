<?xml version="1.0" encoding="utf-8"?>
<SchemeTable Partition="c3b14969-a553-4005-9a07-e9705c15b1e1" ID="94d9f16f-8f98-4307-844e-9959c8627fae" Name="FdParticipantsRemaining" Group="Fd" InstanceType="Cards" ContentType="Collections">
	<Description>Оставшиеся участники текущего этапа</Description>
	<SchemeComplexColumn IsSystem="true" IsPermanent="true" IsSealed="true" ID="94d9f16f-8f98-0007-2000-0959c8627fae" Name="ID" Type="Reference(Typified) Not Null" ReferencedTable="1074eadd-21d7-4925-98c8-40d1e5f0ca0e">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="94d9f16f-8f98-0107-4000-0959c8627fae" Name="ID" Type="Guid Not Null" ReferencedColumn="9a58123b-b2e9-4137-9c6c-5dab0ec02747" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn IsSystem="true" IsPermanent="true" IsSealed="true" ID="94d9f16f-8f98-0007-3100-0959c8627fae" Name="RowID" Type="Guid Not Null" />
	<SchemeComplexColumn ID="91414c11-c592-4531-afa2-8f567d693443" Name="Participant" Type="Reference(Typified) Not Null" ReferencedTable="81f6010b-9641-4aa5-8897-b8e8603fbf4b">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="91414c11-c592-0031-4000-0f567d693443" Name="ParticipantID" Type="Guid Not Null" ReferencedColumn="81f6010b-9641-01a5-4000-08e8603fbf4b" />
		<SchemeReferencingColumn ID="dc919e9f-3cd6-4fff-b2a1-6f1e5172508d" Name="ParticipantName" Type="String(128) Not Null" ReferencedColumn="616d6b2e-35d5-424d-846b-618eb25962d0" />
	</SchemeComplexColumn>
	<SchemeComplexColumn ID="a0f416ac-2a49-42ad-8706-cb528ffe85b2" Name="Process" Type="Reference(Typified) Not Null" ReferencedTable="2114510a-e165-4491-afcc-1756400e30a0">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="a0f416ac-2a49-00ad-4000-0b528ffe85b2" Name="ProcessRowID" Type="Guid Not Null" ReferencedColumn="2114510a-e165-0091-3100-0756400e30a0" />
	</SchemeComplexColumn>
	<SchemeComplexColumn ID="74c639c1-2f09-4197-8895-9a78c30f1892" Name="Stage" Type="Reference(Typified) Not Null" ReferencedTable="ffced5f3-fea9-41e1-bcc6-b156b3e3c054" DeleteReferentialAction="Cascade">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="74c639c1-2f09-0097-4000-0a78c30f1892" Name="StageRowID" Type="Guid Not Null" ReferencedColumn="ffced5f3-fea9-00e1-3100-0156b3e3c054" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn ID="897d11f0-518c-4512-aef0-b330179e0b19" Name="Order" Type="Int32 Not Null">
		<SchemeDefaultConstraint IsPermanent="true" ID="b6600d23-a08b-4f47-a662-7aada9a3e937" Name="df_FdParticipantsRemaining_Order" Value="0" />
	</SchemePhysicalColumn>
	<SchemeComplexColumn ID="a91f7b67-2942-423a-899d-7310934e2efe" Name="StageTemplateParticipant" Type="Reference(Typified) Null" ReferencedTable="b9d1e0af-06c0-4c17-9e83-0497464e193b" WithForeignKey="false">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="a91f7b67-2942-003a-4000-0310934e2efe" Name="StageTemplateParticipantRowID" Type="Guid Null" ReferencedColumn="b9d1e0af-06c0-0017-3100-0497464e193b" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn ID="afce08be-3aac-4764-bf09-2ebcadbc6e7e" Name="TaskTimeLimitationDaysOffset" Type="Double Null" />
	<SchemeComplexColumn ID="2cd5519a-5e94-42b5-8d63-748faec14e3e" Name="Notification" Type="Reference(Typified) Null" ReferencedTable="18145bb5-fd4e-4795-aa1f-9e1cd9b4ee5a">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="2cd5519a-5e94-00b5-4000-048faec14e3e" Name="NotificationID" Type="Guid Null" ReferencedColumn="18145bb5-fd4e-0195-4000-0e1cd9b4ee5a" />
		<SchemeReferencingColumn ID="8aba594b-baa8-4cec-910f-c1dcbb9de90c" Name="NotificationName" Type="String(256) Null" ReferencedColumn="265d4336-6764-4db8-8874-0e5fa92cbd5d" />
	</SchemeComplexColumn>
	<SchemePrimaryKey IsSystem="true" IsPermanent="true" IsSealed="true" ID="94d9f16f-8f98-0007-5000-0959c8627fae" Name="pk_FdParticipantsRemaining">
		<SchemeIndexedColumn Column="94d9f16f-8f98-0007-3100-0959c8627fae" />
	</SchemePrimaryKey>
	<SchemeIndex IsSystem="true" IsPermanent="true" IsSealed="true" ID="94d9f16f-8f98-0007-7000-0959c8627fae" Name="idx_FdParticipantsRemaining_ID" IsClustered="true">
		<SchemeIndexedColumn Column="94d9f16f-8f98-0107-4000-0959c8627fae" />
	</SchemeIndex>
	<SchemeIndex ID="90bb8383-d035-4bae-b01d-b1dd163c13bf" Name="ndx_FdParticipantsRemaining_ParticipantID">
		<SchemeIndexedColumn Column="91414c11-c592-0031-4000-0f567d693443" />
	</SchemeIndex>
</SchemeTable>