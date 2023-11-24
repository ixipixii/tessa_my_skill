<?xml version="1.0" encoding="utf-8"?>
<SchemeTable Partition="c3b14969-a553-4005-9a07-e9705c15b1e1" ID="09be6b67-cf61-48b1-beeb-588e0505ad5b" Name="FdStageTemplateTypes" Group="Fd">
	<Description>Типы шаблонов этапа</Description>
	<SchemePhysicalColumn ID="c7b30411-85cf-4d3c-83a2-e8c0fa4a40bd" Name="ID" Type="Int32 Not Null" />
	<SchemePhysicalColumn ID="039f7f8a-cd50-4c49-b9ee-d5a92986794c" Name="Name" Type="String(256) Not Null" />
	<SchemeUniqueKey ID="e07fdf61-9219-4959-a172-4f8495b2fa26" Name="ndx_FdStageTemplateTypes_ID">
		<SchemeIndexedColumn Column="c7b30411-85cf-4d3c-83a2-e8c0fa4a40bd" />
	</SchemeUniqueKey>
	<SchemePrimaryKey ID="63c38d66-6e59-4d41-b615-af48289a7302" Name="pk_FdStageTemplateTypes">
		<SchemeIndexedColumn Column="c7b30411-85cf-4d3c-83a2-e8c0fa4a40bd" />
	</SchemePrimaryKey>
	<SchemeRecord>
		<ID ID="c7b30411-85cf-4d3c-83a2-e8c0fa4a40bd">0</ID>
		<Name ID="039f7f8a-cd50-4c49-b9ee-d5a92986794c">Запуск процесса</Name>
	</SchemeRecord>
	<SchemeRecord>
		<ID ID="c7b30411-85cf-4d3c-83a2-e8c0fa4a40bd">1</ID>
		<Name ID="039f7f8a-cd50-4c49-b9ee-d5a92986794c">Стандартный этап</Name>
	</SchemeRecord>
</SchemeTable>