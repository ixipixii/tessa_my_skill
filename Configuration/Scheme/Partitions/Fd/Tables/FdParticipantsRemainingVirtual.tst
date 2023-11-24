<?xml version="1.0" encoding="utf-8"?>
<SchemeTable Partition="c3b14969-a553-4005-9a07-e9705c15b1e1" ID="cbcd5b37-7092-4dc7-accc-24be9c0e158d" Name="FdParticipantsRemainingVirtual" Group="Fd" IsVirtual="true" InstanceType="Cards" ContentType="Collections">
	<SchemeComplexColumn IsSystem="true" IsPermanent="true" IsSealed="true" ID="cbcd5b37-7092-00c7-2000-04be9c0e158d" Name="ID" Type="Reference(Typified) Not Null" ReferencedTable="1074eadd-21d7-4925-98c8-40d1e5f0ca0e">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="cbcd5b37-7092-01c7-4000-04be9c0e158d" Name="ID" Type="Guid Not Null" ReferencedColumn="9a58123b-b2e9-4137-9c6c-5dab0ec02747" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn IsSystem="true" IsPermanent="true" IsSealed="true" ID="cbcd5b37-7092-00c7-3100-04be9c0e158d" Name="RowID" Type="Guid Not Null" />
	<SchemeComplexColumn ID="3d202f88-64b2-481e-b2cd-58348ac7d7ad" Name="Participant" Type="Reference(Typified) Not Null" ReferencedTable="81f6010b-9641-4aa5-8897-b8e8603fbf4b">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="3d202f88-64b2-001e-4000-08348ac7d7ad" Name="ParticipantID" Type="Guid Not Null" ReferencedColumn="81f6010b-9641-01a5-4000-08e8603fbf4b" />
		<SchemeReferencingColumn ID="a8e856aa-dfc7-4d98-aa83-da9ef1c7813e" Name="ParticipantName" Type="String(128) Not Null" ReferencedColumn="616d6b2e-35d5-424d-846b-618eb25962d0" />
	</SchemeComplexColumn>
	<SchemeComplexColumn ID="8bfe24ed-18fe-4fd5-8eb0-a9a55ff65a73" Name="Process" Type="Reference(Typified) Not Null" ReferencedTable="ce0e5c9d-a8d2-498a-b8a0-d9bed35d5735">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="8bfe24ed-18fe-00d5-4000-09a55ff65a73" Name="ProcessRowID" Type="Guid Not Null" ReferencedColumn="ce0e5c9d-a8d2-008a-3100-09bed35d5735" />
	</SchemeComplexColumn>
	<SchemeComplexColumn ID="e82c4a5d-637f-4c68-b662-4ee7b852c079" Name="Stage" Type="Reference(Typified) Not Null" ReferencedTable="ba857d60-e905-4e9f-bb44-e2d21dd25027" IsReferenceToOwner="true">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="e82c4a5d-637f-0068-4000-0ee7b852c079" Name="StageRowID" Type="Guid Not Null" ReferencedColumn="ba857d60-e905-009f-3100-02d21dd25027" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn ID="f74a2953-7163-48b1-ac4c-8f2844597feb" Name="Order" Type="Int32 Not Null">
		<SchemeDefaultConstraint IsPermanent="true" ID="b5e94829-6c2d-45fe-b18a-d2f58cad9706" Name="df_FdParticipantsRemainingVirtual_Order" Value="0" />
	</SchemePhysicalColumn>
	<SchemeComplexColumn ID="596ce5a3-47fd-4bcb-b63f-be2fea85d103" Name="StageTemplateParticipant" Type="Reference(Typified) Null" ReferencedTable="b9d1e0af-06c0-4c17-9e83-0497464e193b" WithForeignKey="false">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="596ce5a3-47fd-00cb-4000-0e2fea85d103" Name="StageTemplateParticipantRowID" Type="Guid Null" ReferencedColumn="b9d1e0af-06c0-0017-3100-0497464e193b" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn ID="4794567e-3b62-4b49-8b5c-d40ca60416ea" Name="TaskTimeLimitationDaysOffset" Type="Double Null" />
	<SchemeComplexColumn ID="08baaf8e-8729-45bd-ac85-b5af5621a691" Name="Notification" Type="Reference(Typified) Null" ReferencedTable="18145bb5-fd4e-4795-aa1f-9e1cd9b4ee5a">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="08baaf8e-8729-00bd-4000-05af5621a691" Name="NotificationID" Type="Guid Null" ReferencedColumn="18145bb5-fd4e-0195-4000-0e1cd9b4ee5a" />
		<SchemeReferencingColumn ID="60e817f8-8b38-494d-abe4-fc037e416d3c" Name="NotificationName" Type="String(256) Null" ReferencedColumn="265d4336-6764-4db8-8874-0e5fa92cbd5d" />
	</SchemeComplexColumn>
	<SchemePrimaryKey IsSystem="true" IsPermanent="true" IsSealed="true" ID="cbcd5b37-7092-00c7-5000-04be9c0e158d" Name="pk_FdParticipantsRemainingVirtual">
		<SchemeIndexedColumn Column="cbcd5b37-7092-00c7-3100-04be9c0e158d" />
	</SchemePrimaryKey>
	<SchemeIndex IsSystem="true" IsPermanent="true" IsSealed="true" ID="cbcd5b37-7092-00c7-7000-04be9c0e158d" Name="idx_FdParticipantsRemainingVirtual_ID" IsClustered="true">
		<SchemeIndexedColumn Column="cbcd5b37-7092-01c7-4000-04be9c0e158d" />
	</SchemeIndex>
</SchemeTable>