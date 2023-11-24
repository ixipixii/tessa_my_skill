<?xml version="1.0" encoding="utf-8"?>
<SchemeTable Partition="54ed79f5-11c0-4f74-b8b0-793d596699d6" ID="ea65b864-aa58-4014-9359-92bd09d1a377" Name="PnrTemplateTypes" Group="Pnr">
	<Description>Тип шаблона</Description>
	<SchemePhysicalColumn ID="2b91016e-6b21-4fc7-bbaa-78b40a17cf83" Name="ID" Type="Int16 Not Null" />
	<SchemePhysicalColumn ID="3395d08d-0f90-4a5b-8f45-329ca98ea92b" Name="Name" Type="String(Max) Null" />
	<SchemePrimaryKey ID="69d8c79c-b08f-44e0-b468-61818661c727" Name="pk_PnrTemplateTypes" IsClustered="true">
		<SchemeIndexedColumn Column="2b91016e-6b21-4fc7-bbaa-78b40a17cf83" />
	</SchemePrimaryKey>
	<SchemeRecord>
		<ID ID="2b91016e-6b21-4fc7-bbaa-78b40a17cf83">0</ID>
		<Name ID="3395d08d-0f90-4a5b-8f45-329ca98ea92b">К публикации CRM</Name>
	</SchemeRecord>
	<SchemeRecord>
		<ID ID="2b91016e-6b21-4fc7-bbaa-78b40a17cf83">1</ID>
		<Name ID="3395d08d-0f90-4a5b-8f45-329ca98ea92b">Для агентских договоров </Name>
	</SchemeRecord>
</SchemeTable>