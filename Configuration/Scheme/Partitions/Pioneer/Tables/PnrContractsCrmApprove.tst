<?xml version="1.0" encoding="utf-8"?>
<SchemeTable Partition="54ed79f5-11c0-4f74-b8b0-793d596699d6" ID="0df3fb02-10e0-4049-8f88-ed070acaa11e" Name="PnrContractsCrmApprove" Group="Pnr">
	<Description>Согласование бухгалтерией</Description>
	<SchemePhysicalColumn ID="a74a1d03-37a9-4ca0-be09-6db0463cbbb9" Name="ID" Type="Int16 Not Null" />
	<SchemePhysicalColumn ID="39381382-1bd5-499a-b858-47de893eeae0" Name="Name" Type="String(Max) Not Null" />
	<SchemePrimaryKey ID="7a69d28c-ed5a-42fe-b1b4-92f366b60c95" Name="pk_PnrContractsCrmApprove" IsClustered="true">
		<SchemeIndexedColumn Column="a74a1d03-37a9-4ca0-be09-6db0463cbbb9" />
	</SchemePrimaryKey>
	<SchemeRecord>
		<ID ID="a74a1d03-37a9-4ca0-be09-6db0463cbbb9">0</ID>
		<Name ID="39381382-1bd5-499a-b858-47de893eeae0">На согласовании</Name>
	</SchemeRecord>
	<SchemeRecord>
		<ID ID="a74a1d03-37a9-4ca0-be09-6db0463cbbb9">1</ID>
		<Name ID="39381382-1bd5-499a-b858-47de893eeae0">Согласовано</Name>
	</SchemeRecord>
	<SchemeRecord>
		<ID ID="a74a1d03-37a9-4ca0-be09-6db0463cbbb9">2</ID>
		<Name ID="39381382-1bd5-499a-b858-47de893eeae0">Не согласовано</Name>
	</SchemeRecord>
</SchemeTable>