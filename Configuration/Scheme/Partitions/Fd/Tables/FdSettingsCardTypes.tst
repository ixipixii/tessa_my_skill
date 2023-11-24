<?xml version="1.0" encoding="utf-8"?>
<SchemeTable Partition="c3b14969-a553-4005-9a07-e9705c15b1e1" ID="88fb6d3f-adef-433c-9e5b-88a884ee92d1" Name="FdSettingsCardTypes" Group="Fd" InstanceType="Cards" ContentType="Collections">
	<SchemeComplexColumn IsSystem="true" IsPermanent="true" IsSealed="true" ID="88fb6d3f-adef-003c-2000-08a884ee92d1" Name="ID" Type="Reference(Typified) Not Null" ReferencedTable="1074eadd-21d7-4925-98c8-40d1e5f0ca0e">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="88fb6d3f-adef-013c-4000-08a884ee92d1" Name="ID" Type="Guid Not Null" ReferencedColumn="9a58123b-b2e9-4137-9c6c-5dab0ec02747" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn IsSystem="true" IsPermanent="true" IsSealed="true" ID="88fb6d3f-adef-003c-3100-08a884ee92d1" Name="RowID" Type="Guid Not Null" />
	<SchemeComplexColumn ID="0c4353bd-7c99-4885-a97d-41a61892593a" Name="CardType" Type="Reference(Typified) Not Null" ReferencedTable="b0538ece-8468-4d0b-8b4e-5a1d43e024db">
		<Description>Тип карточки, включенный в типовое решение Fd.</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="0c4353bd-7c99-0085-4000-01a61892593a" Name="CardTypeID" Type="Guid Not Null" ReferencedColumn="a628a864-c858-4200-a6b7-da78c8e6e1f4" />
		<SchemeReferencingColumn ID="e5a9be5c-a1ca-425d-afc0-e724ad316ff3" Name="CardTypeCaption" Type="String(128) Not Null" ReferencedColumn="0a02451e-2e06-4001-9138-b4805e641afa" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn ID="e257885c-4156-46f2-ae2a-a1b91fdd52cf" Name="UseProcesses" Type="Boolean Not Null">
		<Description>Использовать процессы.</Description>
		<SchemeDefaultConstraint IsPermanent="true" ID="0dfa0f8b-1496-42a0-84ef-a8f548977c0a" Name="df_FdSettingsCardTypes_UseProcesses" Value="false" />
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="42b880da-9955-40fb-b304-9cead87428e6" Name="HideFdHistoryList" Type="Boolean Not Null">
		<Description>Скрывать лист истории для карточки</Description>
		<SchemeDefaultConstraint IsPermanent="true" ID="97258c60-bdbe-4f0e-8a41-b47b4313f879" Name="df_FdSettingsCardTypes_HideFdHistoryList" Value="false" />
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="b43f2057-4be3-4866-bb55-796cc4c18f97" Name="HideKrHistoryList" Type="Boolean Not Null">
		<Description>Скрывать типовой (Kr) лист истории для карточки</Description>
		<SchemeDefaultConstraint IsPermanent="true" ID="b5caddb8-4078-4519-9cbd-1300cfad594c" Name="df_FdSettingsCardTypes_HideKrHistoryList" Value="false" />
	</SchemePhysicalColumn>
	<SchemePrimaryKey IsSystem="true" IsPermanent="true" IsSealed="true" ID="88fb6d3f-adef-003c-5000-08a884ee92d1" Name="pk_FdSettingsCardTypes">
		<SchemeIndexedColumn Column="88fb6d3f-adef-003c-3100-08a884ee92d1" />
	</SchemePrimaryKey>
	<SchemeIndex IsSystem="true" IsPermanent="true" IsSealed="true" ID="88fb6d3f-adef-003c-7000-08a884ee92d1" Name="idx_FdSettingsCardTypes_ID" IsClustered="true">
		<SchemeIndexedColumn Column="88fb6d3f-adef-013c-4000-08a884ee92d1" />
	</SchemeIndex>
</SchemeTable>