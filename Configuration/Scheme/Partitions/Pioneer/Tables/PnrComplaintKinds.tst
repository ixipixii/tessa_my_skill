<?xml version="1.0" encoding="utf-8"?>
<SchemeTable Partition="54ed79f5-11c0-4f74-b8b0-793d596699d6" ID="ea8131ae-ac8c-4f9c-814a-1698efd714ee" Name="PnrComplaintKinds" Group="Pnr" InstanceType="Cards" ContentType="Entries">
	<Description>Справочник Виды рекламации</Description>
	<SchemeComplexColumn IsSystem="true" IsPermanent="true" IsSealed="true" ID="ea8131ae-ac8c-009c-2000-0698efd714ee" Name="ID" Type="Reference(Typified) Not Null" ReferencedTable="1074eadd-21d7-4925-98c8-40d1e5f0ca0e">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="ea8131ae-ac8c-019c-4000-0698efd714ee" Name="ID" Type="Guid Not Null" ReferencedColumn="9a58123b-b2e9-4137-9c6c-5dab0ec02747" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn ID="f5e4f06c-41ba-4654-8ead-f142dc6bda82" Name="Name" Type="String(Max) Null">
		<Description>Название</Description>
	</SchemePhysicalColumn>
	<SchemeComplexColumn ID="b88735af-e12f-4839-b62b-55e7efec1193" Name="Responsible" Type="Reference(Typified) Null" ReferencedTable="6c977939-bbfc-456f-a133-f1c2244e3cc3">
		<Description>Ответственный МСК</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="b88735af-e12f-0039-4000-05e7efec1193" Name="ResponsibleID" Type="Guid Null" ReferencedColumn="6c977939-bbfc-016f-4000-01c2244e3cc3" />
		<SchemeReferencingColumn ID="f7059a9d-41c0-4953-b9de-172c338e33ec" Name="ResponsibleName" Type="String(128) Null" ReferencedColumn="1782f76a-4743-4aa4-920c-7edaee860964" />
	</SchemeComplexColumn>
	<SchemeComplexColumn ID="3a4b495a-e64a-4791-8584-ea73c20833a9" Name="ResponsibleRole" Type="Reference(Typified) Null" ReferencedTable="81f6010b-9641-4aa5-8897-b8e8603fbf4b">
		<Description>Ответственный</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="3a4b495a-e64a-0091-4000-0a73c20833a9" Name="ResponsibleRoleID" Type="Guid Null" ReferencedColumn="81f6010b-9641-01a5-4000-08e8603fbf4b" />
		<SchemeReferencingColumn ID="f6929b10-98fd-4aff-a18c-63d1dc732780" Name="ResponsibleRoleName" Type="String(128) Null" ReferencedColumn="616d6b2e-35d5-424d-846b-618eb25962d0" />
	</SchemeComplexColumn>
	<SchemePrimaryKey IsSystem="true" IsPermanent="true" IsSealed="true" ID="ea8131ae-ac8c-009c-5000-0698efd714ee" Name="pk_PnrComplaintKinds" IsClustered="true">
		<SchemeIndexedColumn Column="ea8131ae-ac8c-019c-4000-0698efd714ee" />
	</SchemePrimaryKey>
</SchemeTable>