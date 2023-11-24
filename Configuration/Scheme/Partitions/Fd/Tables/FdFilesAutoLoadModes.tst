<?xml version="1.0" encoding="utf-8"?>
<SchemeTable Partition="c3b14969-a553-4005-9a07-e9705c15b1e1" ID="35ca731c-cb73-4e14-a0d9-c5f0478c7b58" Name="FdFilesAutoLoadModes" Group="Fd Enum">
	<SchemePhysicalColumn ID="46ca0bdc-cc41-4f11-9d27-e155225ecaf6" Name="ID" Type="Int32 Not Null" />
	<SchemePhysicalColumn ID="d6fedd9e-1c5d-4176-ab4d-2aab042c9345" Name="Name" Type="String(128) Not Null" />
	<SchemePrimaryKey ID="1688b38d-d76b-4a9d-a512-ec165375922d" Name="pk_FdFilesAutoLoadModes">
		<SchemeIndexedColumn Column="46ca0bdc-cc41-4f11-9d27-e155225ecaf6" />
	</SchemePrimaryKey>
	<SchemeRecord>
		<ID ID="46ca0bdc-cc41-4f11-9d27-e155225ecaf6">0</ID>
		<Name ID="d6fedd9e-1c5d-4176-ab4d-2aab042c9345">Не загружать</Name>
	</SchemeRecord>
	<SchemeRecord>
		<ID ID="46ca0bdc-cc41-4f11-9d27-e155225ecaf6">1</ID>
		<Name ID="d6fedd9e-1c5d-4176-ab4d-2aab042c9345">Загружать по кнопке</Name>
	</SchemeRecord>
</SchemeTable>