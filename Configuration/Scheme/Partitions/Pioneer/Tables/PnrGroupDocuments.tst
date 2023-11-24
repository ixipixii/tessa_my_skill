<?xml version="1.0" encoding="utf-8"?>
<SchemeTable Partition="54ed79f5-11c0-4f74-b8b0-793d596699d6" ID="509214b4-3b67-4597-b5ec-d09ad9273a85" Name="PnrGroupDocuments" Group="Pnr">
	<Description>Справочник Группа документов</Description>
	<SchemePhysicalColumn ID="2a8ee9d0-9ebc-4be0-9ff7-b58c66626682" Name="ID" Type="Int16 Not Null" />
	<SchemePhysicalColumn ID="d7c89108-4613-4915-ab8d-4f092abf370f" Name="Name" Type="String(64) Not Null" />
	<SchemePrimaryKey ID="1ec535ce-536e-45e4-9988-1dd294be97f8" Name="pk_PnrGroupDocuments" IsClustered="true">
		<SchemeIndexedColumn Column="2a8ee9d0-9ebc-4be0-9ff7-b58c66626682" />
	</SchemePrimaryKey>
</SchemeTable>