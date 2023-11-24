<?xml version="1.0" encoding="utf-8"?>
<SchemeTable ID="a161e289-2f99-4699-9e95-6e3336be8527" Partition="d1b372f3-7565-4309-9037-5e5a0969d94e">
	<SchemeComplexColumn Partition="54ed79f5-11c0-4f74-b8b0-793d596699d6" ID="99037f7d-c376-4cb5-a06a-006fb96548d5" Name="CardType" Type="Reference(Typified) Null" ReferencedTable="b0538ece-8468-4d0b-8b4e-5a1d43e024db">
		<SchemeReferencingColumn Partition="54ed79f5-11c0-4f74-b8b0-793d596699d6" ID="30af08c4-2b97-4cf1-bce5-0615bdc40792" Name="CardTypeName" Type="String(128) Null" ReferencedColumn="71181642-0d62-45f9-8ad8-ccec4bd4ce22" />
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="99037f7d-c376-00b5-4000-006fb96548d5" Name="CardTypeID" Type="Guid Null" ReferencedColumn="a628a864-c858-4200-a6b7-da78c8e6e1f4">
			<Description>ID of a type.</Description>
			<SchemeDefaultConstraint IsPermanent="true" ID="a196f380-dafd-43bc-8dfc-7e2e90cf2993" Name="df_DocumentCommonInfo_CardTypeID" Value="6d06c5a0-9687-4f6b-9bed-d3a081d84d9a" />
		</SchemeReferencingColumn>
		<SchemeReferencingColumn ID="b3797c27-de1d-455c-ba8c-ad8f0a605ba0" Name="CardTypeCaption" Type="String(128) Null" ReferencedColumn="0a02451e-2e06-4001-9138-b4805e641afa" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn Partition="54ed79f5-11c0-4f74-b8b0-793d596699d6" ID="cc4329a5-3c39-4200-be26-8b555ded499c" Name="IsMigrated" Type="Boolean Null" />
</SchemeTable>