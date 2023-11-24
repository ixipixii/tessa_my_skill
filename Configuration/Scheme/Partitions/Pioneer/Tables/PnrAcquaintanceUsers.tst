<?xml version="1.0" encoding="utf-8"?>
<SchemeTable Partition="54ed79f5-11c0-4f74-b8b0-793d596699d6" ID="b0776f32-9697-4be0-86b1-2db15d10bd5b" Name="PnrAcquaintanceUsers" Group="PnrTask" InstanceType="Tasks" ContentType="Collections">
	<Description>Пользователи на ознакомление</Description>
	<SchemeComplexColumn IsSystem="true" IsPermanent="true" IsSealed="true" ID="b0776f32-9697-00e0-2000-0db15d10bd5b" Name="ID" Type="Reference(Typified) Not Null" ReferencedTable="5bfa9936-bb5a-4e8f-89a9-180bfd8f75f8">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="b0776f32-9697-01e0-4000-0db15d10bd5b" Name="ID" Type="Guid Not Null" ReferencedColumn="5bfa9936-bb5a-008f-3100-080bfd8f75f8" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn IsSystem="true" IsPermanent="true" IsSealed="true" ID="b0776f32-9697-00e0-3100-0db15d10bd5b" Name="RowID" Type="Guid Not Null" />
	<SchemeComplexColumn ID="04d1f12f-5ffa-4bbd-9294-eb9f1f02caad" Name="User" Type="Reference(Typified) Null" ReferencedTable="6c977939-bbfc-456f-a133-f1c2244e3cc3">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="04d1f12f-5ffa-00bd-4000-0b9f1f02caad" Name="UserID" Type="Guid Null" ReferencedColumn="6c977939-bbfc-016f-4000-01c2244e3cc3" />
		<SchemeReferencingColumn ID="5527540d-09a3-48ed-a979-066fe374c31b" Name="UserName" Type="String(128) Null" ReferencedColumn="1782f76a-4743-4aa4-920c-7edaee860964" />
	</SchemeComplexColumn>
	<SchemePrimaryKey IsSystem="true" IsPermanent="true" IsSealed="true" ID="b0776f32-9697-00e0-5000-0db15d10bd5b" Name="pk_PnrAcquaintanceUsers">
		<SchemeIndexedColumn Column="b0776f32-9697-00e0-3100-0db15d10bd5b" />
	</SchemePrimaryKey>
	<SchemeIndex IsSystem="true" IsPermanent="true" IsSealed="true" ID="b0776f32-9697-00e0-7000-0db15d10bd5b" Name="idx_PnrAcquaintanceUsers_ID" IsClustered="true">
		<SchemeIndexedColumn Column="b0776f32-9697-01e0-4000-0db15d10bd5b" />
	</SchemeIndex>
</SchemeTable>