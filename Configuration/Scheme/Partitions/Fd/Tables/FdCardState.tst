<?xml version="1.0" encoding="utf-8"?>
<SchemeTable Partition="c3b14969-a553-4005-9a07-e9705c15b1e1" ID="c5e015fe-1b55-4a18-8787-074b5d2ec80c" Name="FdCardState" Group="Fd" InstanceType="Cards" ContentType="Entries">
	<SchemeComplexColumn IsSystem="true" IsPermanent="true" IsSealed="true" ID="c5e015fe-1b55-0018-2000-074b5d2ec80c" Name="ID" Type="Reference(Typified) Not Null" ReferencedTable="1074eadd-21d7-4925-98c8-40d1e5f0ca0e">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="c5e015fe-1b55-0118-4000-074b5d2ec80c" Name="ID" Type="Guid Not Null" ReferencedColumn="9a58123b-b2e9-4137-9c6c-5dab0ec02747" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn ID="09d1dfdb-262f-4b6e-b14d-64e5f51b7fa7" Name="Name" Type="String(128) Not Null" />
	<SchemeComplexColumn ID="8713c822-d30c-40be-8b90-32c2d88a4dc4" Name="CardType" Type="Reference(Typified) Not Null" ReferencedTable="b0538ece-8468-4d0b-8b4e-5a1d43e024db">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="8713c822-d30c-00be-4000-02c2d88a4dc4" Name="CardTypeID" Type="Guid Not Null" ReferencedColumn="a628a864-c858-4200-a6b7-da78c8e6e1f4" />
		<SchemeReferencingColumn ID="8c9bd56b-1628-4b86-a8a9-272fadb8b844" Name="CardTypeCaption" Type="String(128) Not Null" ReferencedColumn="0a02451e-2e06-4001-9138-b4805e641afa" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn ID="79c935da-8a55-4d0d-81d6-2e0b56f9d930" Name="IsDefaultState" Type="Boolean Not Null">
		<SchemeDefaultConstraint IsPermanent="true" ID="8d6dc6d1-425a-414f-9dcb-b21c7b91eefd" Name="df_FdCardState_IsDefaultState" Value="false" />
	</SchemePhysicalColumn>
	<SchemePrimaryKey IsSystem="true" IsPermanent="true" IsSealed="true" ID="c5e015fe-1b55-0018-5000-074b5d2ec80c" Name="pk_FdCardState" IsClustered="true">
		<SchemeIndexedColumn Column="c5e015fe-1b55-0118-4000-074b5d2ec80c" />
	</SchemePrimaryKey>
</SchemeTable>