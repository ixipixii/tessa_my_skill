<?xml version="1.0" encoding="utf-8"?>
<SchemeTable Partition="c3b14969-a553-4005-9a07-e9705c15b1e1" ID="44c3a9b3-d245-445c-a1fe-2a21ce021e1e" Name="FdProcessState" Group="Fd">
	<SchemePhysicalColumn ID="3ba53faf-6661-4352-8c57-35e0b6c0dc3b" Name="ID" Type="Int16 Not Null" />
	<SchemePhysicalColumn ID="13bffd8e-b32f-4243-b8f9-ec07da0ef5c1" Name="Name" Type="String(128) Not Null" />
	<SchemePrimaryKey ID="f3059f50-0167-4f9c-9a6f-24dd33803583" Name="pk_FdProcessState">
		<SchemeIndexedColumn Column="3ba53faf-6661-4352-8c57-35e0b6c0dc3b" />
	</SchemePrimaryKey>
	<SchemeRecord>
		<ID ID="3ba53faf-6661-4352-8c57-35e0b6c0dc3b">0</ID>
		<Name ID="13bffd8e-b32f-4243-b8f9-ec07da0ef5c1">Не запущен</Name>
	</SchemeRecord>
	<SchemeRecord>
		<ID ID="3ba53faf-6661-4352-8c57-35e0b6c0dc3b">1</ID>
		<Name ID="13bffd8e-b32f-4243-b8f9-ec07da0ef5c1">Активен</Name>
	</SchemeRecord>
	<SchemeRecord>
		<ID ID="3ba53faf-6661-4352-8c57-35e0b6c0dc3b">2</ID>
		<Name ID="13bffd8e-b32f-4243-b8f9-ec07da0ef5c1">Завершен</Name>
	</SchemeRecord>
	<SchemeRecord>
		<ID ID="3ba53faf-6661-4352-8c57-35e0b6c0dc3b">3</ID>
		<Name ID="13bffd8e-b32f-4243-b8f9-ec07da0ef5c1">Запуск</Name>
	</SchemeRecord>
</SchemeTable>