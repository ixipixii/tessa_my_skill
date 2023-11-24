<?xml version="1.0" encoding="utf-8"?>
<SchemeTable ID="8b409265-3873-4519-9857-2f0e03e82b03" Name="FmAddTopicInfoVirtual" Group="Fm" IsVirtual="true" InstanceType="Cards" ContentType="Entries">
	<Description>Виртуальная секция для заголовка и описания топики</Description>
	<SchemeComplexColumn IsSystem="true" IsPermanent="true" IsSealed="true" ID="8b409265-3873-0019-2000-0f0e03e82b03" Name="ID" Type="Reference(Typified) Not Null" ReferencedTable="1074eadd-21d7-4925-98c8-40d1e5f0ca0e">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="8b409265-3873-0119-4000-0f0e03e82b03" Name="ID" Type="Guid Not Null" ReferencedColumn="9a58123b-b2e9-4137-9c6c-5dab0ec02747" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn ID="65435953-0606-41c6-bb91-f80650fa1069" Name="Title" Type="String(1024) Not Null">
		<Description>Залоговок</Description>
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="a6f2d8e4-144d-42e9-906c-b90be7cf4c15" Name="Description" Type="String(Max) Null">
		<Description>Описания </Description>
	</SchemePhysicalColumn>
	<SchemePrimaryKey IsSystem="true" IsPermanent="true" IsSealed="true" ID="8b409265-3873-0019-5000-0f0e03e82b03" Name="pk_FmAddTopicInfoVirtual" IsClustered="true">
		<SchemeIndexedColumn Column="8b409265-3873-0119-4000-0f0e03e82b03" />
	</SchemePrimaryKey>
</SchemeTable>