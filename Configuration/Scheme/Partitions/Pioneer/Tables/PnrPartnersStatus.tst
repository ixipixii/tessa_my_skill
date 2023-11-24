<?xml version="1.0" encoding="utf-8"?>
<SchemeTable Partition="54ed79f5-11c0-4f74-b8b0-793d596699d6" ID="65cd42e4-d0f7-4bab-a256-45cfec7b286a" Name="PnrPartnersStatus" Group="PnrEnums">
	<SchemePhysicalColumn ID="29458ef8-7c58-4f9f-8f3b-a2098f54fd13" Name="ID" Type="Int16 Not Null" />
	<SchemePhysicalColumn ID="fb59bbdc-5ec5-4fbf-ab31-19756af8b406" Name="Name" Type="String(100) Null" />
	<SchemePrimaryKey ID="65b1769d-7311-4aab-9808-d187b3d88ec9" Name="pk_PnrPartnersStatus">
		<SchemeIndexedColumn Column="29458ef8-7c58-4f9f-8f3b-a2098f54fd13" />
	</SchemePrimaryKey>
	<SchemeRecord>
		<ID ID="29458ef8-7c58-4f9f-8f3b-a2098f54fd13">0</ID>
		<Name ID="fb59bbdc-5ec5-4fbf-ab31-19756af8b406">Согласован</Name>
	</SchemeRecord>
	<SchemeRecord>
		<ID ID="29458ef8-7c58-4f9f-8f3b-a2098f54fd13">1</ID>
		<Name ID="fb59bbdc-5ec5-4fbf-ab31-19756af8b406">Не согласован</Name>
	</SchemeRecord>
	<SchemeRecord>
		<ID ID="29458ef8-7c58-4f9f-8f3b-a2098f54fd13">2</ID>
		<Name ID="fb59bbdc-5ec5-4fbf-ab31-19756af8b406">В черном списке</Name>
	</SchemeRecord>
</SchemeTable>