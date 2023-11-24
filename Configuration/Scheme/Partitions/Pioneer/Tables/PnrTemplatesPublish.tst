<?xml version="1.0" encoding="utf-8"?>
<SchemeTable Partition="54ed79f5-11c0-4f74-b8b0-793d596699d6" ID="a41f5936-2cb2-4f2c-8c46-f43eba3753e5" Name="PnrTemplatesPublish" Group="Pnr">
	<Description>Вид шаблона</Description>
	<SchemePhysicalColumn ID="1f59643c-873a-4e5b-9f6a-ceac223fce7f" Name="ID" Type="Int16 Not Null" />
	<SchemePhysicalColumn ID="3ab01221-918a-4221-b503-9ac2b4d3e0e1" Name="Name" Type="String(Max) Null" />
	<SchemePrimaryKey ID="eaeee70f-54f1-4d57-91e9-d0664ca1a327" Name="pk_PnrTemplatesPublish" IsClustered="true">
		<SchemeIndexedColumn Column="1f59643c-873a-4e5b-9f6a-ceac223fce7f" />
	</SchemePrimaryKey>
	<SchemeRecord>
		<ID ID="1f59643c-873a-4e5b-9f6a-ceac223fce7f">0</ID>
		<Name ID="3ab01221-918a-4221-b503-9ac2b4d3e0e1">Общие к публикации в CRM</Name>
	</SchemeRecord>
	<SchemeRecord>
		<ID ID="1f59643c-873a-4e5b-9f6a-ceac223fce7f">1</ID>
		<Name ID="3ab01221-918a-4221-b503-9ac2b4d3e0e1">Под передаточные акты к публикации в CRM</Name>
	</SchemeRecord>
	<SchemeRecord>
		<ID ID="1f59643c-873a-4e5b-9f6a-ceac223fce7f">2</ID>
		<Name ID="3ab01221-918a-4221-b503-9ac2b4d3e0e1">Для агентских договоров по жилым помещениям</Name>
	</SchemeRecord>
	<SchemeRecord>
		<ID ID="1f59643c-873a-4e5b-9f6a-ceac223fce7f">3</ID>
		<Name ID="3ab01221-918a-4221-b503-9ac2b4d3e0e1">Для агентских договоров по коммерческим помещениям </Name>
	</SchemeRecord>
</SchemeTable>