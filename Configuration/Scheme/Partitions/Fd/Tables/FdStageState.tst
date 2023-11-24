<?xml version="1.0" encoding="utf-8"?>
<SchemeTable Partition="c3b14969-a553-4005-9a07-e9705c15b1e1" ID="7298c4f3-6c27-439e-a417-bafa3967e40e" Name="FdStageState" Group="Fd">
	<Description>Состояния этапов</Description>
	<SchemePhysicalColumn ID="a9399756-feeb-4507-8b42-f66517400134" Name="ID" Type="Int16 Not Null" />
	<SchemePhysicalColumn ID="31ac86d7-2665-457a-996e-9aee07dc0030" Name="Name" Type="String(128) Not Null" />
	<SchemePrimaryKey ID="81f48aab-0168-4a55-adf9-48b64020b4ed" Name="pk_FdStageState">
		<SchemeIndexedColumn Column="a9399756-feeb-4507-8b42-f66517400134" />
	</SchemePrimaryKey>
	<SchemeRecord>
		<ID ID="a9399756-feeb-4507-8b42-f66517400134">0</ID>
		<Name ID="31ac86d7-2665-457a-996e-9aee07dc0030">Не запущен</Name>
	</SchemeRecord>
	<SchemeRecord>
		<ID ID="a9399756-feeb-4507-8b42-f66517400134">1</ID>
		<Name ID="31ac86d7-2665-457a-996e-9aee07dc0030">Активен</Name>
	</SchemeRecord>
	<SchemeRecord>
		<ID ID="a9399756-feeb-4507-8b42-f66517400134">2</ID>
		<Name ID="31ac86d7-2665-457a-996e-9aee07dc0030">Завершен</Name>
	</SchemeRecord>
</SchemeTable>