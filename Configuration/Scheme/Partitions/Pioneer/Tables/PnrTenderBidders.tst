<?xml version="1.0" encoding="utf-8"?>
<SchemeTable Partition="54ed79f5-11c0-4f74-b8b0-793d596699d6" ID="bdd19dbd-f963-4f2b-bdd3-719537278982" Name="PnrTenderBidders" Group="Pnr" InstanceType="Cards" ContentType="Collections">
	<Description>Участники тендера</Description>
	<SchemeComplexColumn IsSystem="true" IsPermanent="true" IsSealed="true" ID="bdd19dbd-f963-002b-2000-019537278982" Name="ID" Type="Reference(Typified) Not Null" ReferencedTable="1074eadd-21d7-4925-98c8-40d1e5f0ca0e">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="bdd19dbd-f963-012b-4000-019537278982" Name="ID" Type="Guid Not Null" ReferencedColumn="9a58123b-b2e9-4137-9c6c-5dab0ec02747" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn IsSystem="true" IsPermanent="true" IsSealed="true" ID="bdd19dbd-f963-002b-3100-019537278982" Name="RowID" Type="Guid Not Null" />
	<SchemeComplexColumn ID="9b826f66-80c0-4bdf-b283-9ed058c7e1dc" Name="Partner" Type="Reference(Typified) Null" ReferencedTable="5d47ef13-b6f4-47ef-9815-3b3d0e6d475a">
		<Description>Участник тендера</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="9b826f66-80c0-00df-4000-0ed058c7e1dc" Name="PartnerID" Type="Guid Null" ReferencedColumn="5d47ef13-b6f4-01ef-4000-0b3d0e6d475a" />
		<SchemeReferencingColumn ID="47382fcc-dc20-4790-953c-7a2336f02753" Name="PartnerName" Type="String(255) Null" ReferencedColumn="f1c960e0-951e-4837-8474-bb61d98f40f0" />
	</SchemeComplexColumn>
	<SchemePrimaryKey IsSystem="true" IsPermanent="true" IsSealed="true" ID="bdd19dbd-f963-002b-5000-019537278982" Name="pk_PnrTenderBidders">
		<SchemeIndexedColumn Column="bdd19dbd-f963-002b-3100-019537278982" />
	</SchemePrimaryKey>
	<SchemeIndex IsSystem="true" IsPermanent="true" IsSealed="true" ID="bdd19dbd-f963-002b-7000-019537278982" Name="idx_PnrTenderBidders_ID" IsClustered="true">
		<SchemeIndexedColumn Column="bdd19dbd-f963-012b-4000-019537278982" />
	</SchemeIndex>
</SchemeTable>