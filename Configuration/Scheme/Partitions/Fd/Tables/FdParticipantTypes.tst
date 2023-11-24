<?xml version="1.0" encoding="utf-8"?>
<SchemeTable Partition="c3b14969-a553-4005-9a07-e9705c15b1e1" ID="11b2bc21-e3b5-4e24-8e55-d07a8bc9b2f5" Name="FdParticipantTypes" Group="Fd StageTemplate">
	<SchemePhysicalColumn ID="ea53336b-92a4-440c-914f-7a536e0c5806" Name="ID" Type="Int32 Not Null" />
	<SchemePhysicalColumn ID="51fc32bc-cf78-40b3-9d36-3bc97b60ac12" Name="Name" Type="String(128) Not Null" />
	<SchemePrimaryKey ID="94d8a464-a9cd-45bf-b287-686b265900d3" Name="pk_FdParticipantTypes">
		<SchemeIndexedColumn Column="ea53336b-92a4-440c-914f-7a536e0c5806" />
	</SchemePrimaryKey>
	<SchemeRecord>
		<ID ID="ea53336b-92a4-440c-914f-7a536e0c5806">0</ID>
		<Name ID="51fc32bc-cf78-40b3-9d36-3bc97b60ac12">Роль</Name>
	</SchemeRecord>
	<SchemeRecord>
		<ID ID="ea53336b-92a4-440c-914f-7a536e0c5806">1</ID>
		<Name ID="51fc32bc-cf78-40b3-9d36-3bc97b60ac12">Поле карточки</Name>
	</SchemeRecord>
	<SchemeRecord>
		<ID ID="ea53336b-92a4-440c-914f-7a536e0c5806">2</ID>
		<Name ID="51fc32bc-cf78-40b3-9d36-3bc97b60ac12">Текущий пользователь</Name>
	</SchemeRecord>
	<SchemeRecord>
		<ID ID="ea53336b-92a4-440c-914f-7a536e0c5806">3</ID>
		<Name ID="51fc32bc-cf78-40b3-9d36-3bc97b60ac12">Роль из предыдущего задания</Name>
	</SchemeRecord>
	<SchemeRecord>
		<ID ID="ea53336b-92a4-440c-914f-7a536e0c5806">4</ID>
		<Name ID="51fc32bc-cf78-40b3-9d36-3bc97b60ac12">Особая роль (поле из строки таблицы карточки)</Name>
	</SchemeRecord>
</SchemeTable>