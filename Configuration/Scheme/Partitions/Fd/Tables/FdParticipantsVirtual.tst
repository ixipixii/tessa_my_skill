<?xml version="1.0" encoding="utf-8"?>
<SchemeTable Partition="c3b14969-a553-4005-9a07-e9705c15b1e1" ID="a04cebc9-49ed-4663-81bf-4da5e7bff630" Name="FdParticipantsVirtual" Group="Fd" IsVirtual="true" InstanceType="Cards" ContentType="Collections">
	<SchemeComplexColumn IsSystem="true" IsPermanent="true" IsSealed="true" ID="a04cebc9-49ed-0063-2000-0da5e7bff630" Name="ID" Type="Reference(Typified) Not Null" ReferencedTable="1074eadd-21d7-4925-98c8-40d1e5f0ca0e">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="a04cebc9-49ed-0163-4000-0da5e7bff630" Name="ID" Type="Guid Not Null" ReferencedColumn="9a58123b-b2e9-4137-9c6c-5dab0ec02747" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn IsSystem="true" IsPermanent="true" IsSealed="true" ID="a04cebc9-49ed-0063-3100-0da5e7bff630" Name="RowID" Type="Guid Not Null" />
	<SchemeComplexColumn ID="4719eb36-9a95-4086-b2f9-5cd04afb3f9c" Name="Participant" Type="Reference(Typified) Not Null" ReferencedTable="81f6010b-9641-4aa5-8897-b8e8603fbf4b">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="4719eb36-9a95-0086-4000-0cd04afb3f9c" Name="ParticipantID" Type="Guid Not Null" ReferencedColumn="81f6010b-9641-01a5-4000-08e8603fbf4b" />
		<SchemeReferencingColumn ID="aa5dd2ff-0bdc-45c7-8f45-f532acd6b338" Name="ParticipantName" Type="String(128) Not Null" ReferencedColumn="616d6b2e-35d5-424d-846b-618eb25962d0" />
	</SchemeComplexColumn>
	<SchemeComplexColumn ID="71eedacc-2be9-48cc-945b-48d509ccdda6" Name="Process" Type="Reference(Typified) Not Null" ReferencedTable="ce0e5c9d-a8d2-498a-b8a0-d9bed35d5735">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="71eedacc-2be9-00cc-4000-08d509ccdda6" Name="ProcessRowID" Type="Guid Not Null" ReferencedColumn="ce0e5c9d-a8d2-008a-3100-09bed35d5735" />
	</SchemeComplexColumn>
	<SchemeComplexColumn ID="96868be2-0a98-4542-836b-c8ffa44c3388" Name="Stage" Type="Reference(Typified) Not Null" ReferencedTable="ba857d60-e905-4e9f-bb44-e2d21dd25027" IsReferenceToOwner="true">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="96868be2-0a98-0042-4000-08ffa44c3388" Name="StageRowID" Type="Guid Not Null" ReferencedColumn="ba857d60-e905-009f-3100-02d21dd25027" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn ID="52e5b350-7b7f-4092-a613-d88fbedb0faa" Name="Order" Type="Int32 Not Null">
		<SchemeDefaultConstraint IsPermanent="true" ID="ccd50b16-f9ec-4eb8-9e87-7c8f74b9c2f7" Name="df_FdParticipantsVirtual_Order" Value="0" />
	</SchemePhysicalColumn>
	<SchemeComplexColumn ID="a0d49f91-51ce-48cb-80a8-452f5041a47b" Name="StageTemplateParticipant" Type="Reference(Typified) Null" ReferencedTable="b9d1e0af-06c0-4c17-9e83-0497464e193b" WithForeignKey="false">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="a0d49f91-51ce-00cb-4000-052f5041a47b" Name="StageTemplateParticipantRowID" Type="Guid Null" ReferencedColumn="b9d1e0af-06c0-0017-3100-0497464e193b" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn ID="a7c362e2-7fc0-4aed-9fbe-135ae94ee00a" Name="TaskTimeLimitationDaysOffset" Type="Double Null" />
	<SchemeComplexColumn ID="743f0dca-e5e2-4b20-9f73-b863c1b07ad3" Name="Notification" Type="Reference(Typified) Null" ReferencedTable="18145bb5-fd4e-4795-aa1f-9e1cd9b4ee5a">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="743f0dca-e5e2-0020-4000-0863c1b07ad3" Name="NotificationID" Type="Guid Null" ReferencedColumn="18145bb5-fd4e-0195-4000-0e1cd9b4ee5a" />
		<SchemeReferencingColumn ID="d3bce3bd-ca50-411c-972d-0d66f209ef89" Name="NotificationName" Type="String(256) Null" ReferencedColumn="265d4336-6764-4db8-8874-0e5fa92cbd5d" />
	</SchemeComplexColumn>
	<SchemePrimaryKey IsSystem="true" IsPermanent="true" IsSealed="true" ID="a04cebc9-49ed-0063-5000-0da5e7bff630" Name="pk_FdParticipantsVirtual">
		<SchemeIndexedColumn Column="a04cebc9-49ed-0063-3100-0da5e7bff630" />
	</SchemePrimaryKey>
	<SchemeIndex IsSystem="true" IsPermanent="true" IsSealed="true" ID="a04cebc9-49ed-0063-7000-0da5e7bff630" Name="idx_FdParticipantsVirtual_ID" IsClustered="true">
		<SchemeIndexedColumn Column="a04cebc9-49ed-0163-4000-0da5e7bff630" />
	</SchemeIndex>
</SchemeTable>