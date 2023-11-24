<?xml version="1.0" encoding="utf-8"?>
<SchemeTable Partition="54ed79f5-11c0-4f74-b8b0-793d596699d6" ID="8f713827-fbea-447d-b030-b53751507c58" Name="PnrCategory" Group="Pnr" InstanceType="Cards" ContentType="Entries">
	<SchemeComplexColumn IsSystem="true" IsPermanent="true" IsSealed="true" ID="8f713827-fbea-007d-2000-053751507c58" Name="ID" Type="Reference(Typified) Not Null" ReferencedTable="1074eadd-21d7-4925-98c8-40d1e5f0ca0e">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="8f713827-fbea-017d-4000-053751507c58" Name="ID" Type="Guid Not Null" ReferencedColumn="9a58123b-b2e9-4137-9c6c-5dab0ec02747" />
	</SchemeComplexColumn>
	<SchemeComplexColumn ID="35abb923-baa0-456c-abcd-8289405ecfa4" Name="CardType" Type="Reference(Typified) Not Null" ReferencedTable="b0538ece-8468-4d0b-8b4e-5a1d43e024db">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="35abb923-baa0-006c-4000-0289405ecfa4" Name="CardTypeID" Type="Guid Not Null" ReferencedColumn="a628a864-c858-4200-a6b7-da78c8e6e1f4" />
		<SchemeReferencingColumn ID="74d61279-cb2e-4f8d-a0ca-3138edba32ae" Name="CardTypeCaption" Type="String(128) Not Null" ReferencedColumn="0a02451e-2e06-4001-9138-b4805e641afa" />
	</SchemeComplexColumn>
	<SchemePrimaryKey IsSystem="true" IsPermanent="true" IsSealed="true" ID="8f713827-fbea-007d-5000-053751507c58" Name="pk_PnrCategory" IsClustered="true">
		<SchemeIndexedColumn Column="8f713827-fbea-017d-4000-053751507c58" />
	</SchemePrimaryKey>
</SchemeTable>