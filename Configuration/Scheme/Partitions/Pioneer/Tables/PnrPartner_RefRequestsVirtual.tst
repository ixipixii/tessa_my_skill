<?xml version="1.0" encoding="utf-8"?>
<SchemeTable Partition="54ed79f5-11c0-4f74-b8b0-793d596699d6" ID="41bbbfe8-7db1-4510-a6b6-85f44ec353f3" Name="PnrPartner_RefRequestsVirtual" Group="Pnr" IsVirtual="true" InstanceType="Cards" ContentType="Collections">
	<Description>Вирт. таблица со связанными заявками на КА</Description>
	<SchemeComplexColumn IsSystem="true" IsPermanent="true" IsSealed="true" ID="41bbbfe8-7db1-0010-2000-05f44ec353f3" Name="ID" Type="Reference(Typified) Not Null" ReferencedTable="1074eadd-21d7-4925-98c8-40d1e5f0ca0e">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="41bbbfe8-7db1-0110-4000-05f44ec353f3" Name="ID" Type="Guid Not Null" ReferencedColumn="9a58123b-b2e9-4137-9c6c-5dab0ec02747" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn IsSystem="true" IsPermanent="true" IsSealed="true" ID="41bbbfe8-7db1-0010-3100-05f44ec353f3" Name="RowID" Type="Guid Not Null" />
	<SchemeComplexColumn ID="8e4a98e6-1a4b-4724-83d3-f68622194222" Name="Doc" Type="Reference(Typified) Not Null" ReferencedTable="a161e289-2f99-4699-9e95-6e3336be8527" WithForeignKey="false">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="8e4a98e6-1a4b-0024-4000-068622194222" Name="DocID" Type="Guid Not Null" ReferencedColumn="a161e289-2f99-0199-4000-0e3336be8527" />
		<SchemeReferencingColumn ID="450a8f0d-eb84-4624-ace4-a893b6633f87" Name="DocFullNumber" Type="String(200) Null" ReferencedColumn="eeb023b5-f473-4a8b-bef2-88e07b0d0688" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn ID="bdb9b3ce-6dcc-421b-8364-f4ef316e81e1" Name="RequestType" Type="String(256) Not Null" />
	<SchemePhysicalColumn ID="4c473b13-36a2-4798-ac0e-3cbebb99dc49" Name="CreateDate" Type="DateTime Not Null" />
	<SchemePhysicalColumn ID="c01fd858-5ecb-4480-b46e-8edf824e3018" Name="State" Type="String(256) Not Null" />
	<SchemePrimaryKey IsSystem="true" IsPermanent="true" IsSealed="true" ID="41bbbfe8-7db1-0010-5000-05f44ec353f3" Name="pk_PnrPartner_RefRequestsVirtual">
		<SchemeIndexedColumn Column="41bbbfe8-7db1-0010-3100-05f44ec353f3" />
	</SchemePrimaryKey>
	<SchemeIndex IsSystem="true" IsPermanent="true" IsSealed="true" ID="41bbbfe8-7db1-0010-7000-05f44ec353f3" Name="idx_PnrPartner_RefRequestsVirtual_ID" IsClustered="true">
		<SchemeIndexedColumn Column="41bbbfe8-7db1-0110-4000-05f44ec353f3" />
	</SchemeIndex>
</SchemeTable>