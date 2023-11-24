<?xml version="1.0" encoding="utf-8"?>
<SchemeTable Partition="54ed79f5-11c0-4f74-b8b0-793d596699d6" ID="ae32aeda-08ed-44f7-8ac4-815e137bcf9e" Name="PnrDepartmentTypes" Group="Pnr">
	<Description>Тип подразделения</Description>
	<SchemePhysicalColumn ID="9edb45c5-b36f-448f-9930-94c5d4b94f20" Name="ID" Type="Int16 Not Null" />
	<SchemePhysicalColumn ID="edaf212a-fb92-44c7-9898-f216cdf09d8d" Name="Name" Type="String(Max) Not Null" />
	<SchemePrimaryKey ID="8ec0b60c-71f8-4db6-bf54-6474fb40e8fa" Name="pk_PnrDepartmentTypes" IsClustered="true">
		<SchemeIndexedColumn Column="9edb45c5-b36f-448f-9930-94c5d4b94f20" />
	</SchemePrimaryKey>
	<SchemeRecord>
		<ID ID="9edb45c5-b36f-448f-9930-94c5d4b94f20">1</ID>
		<Name ID="edaf212a-fb92-44c7-9898-f216cdf09d8d">Подразделение</Name>
	</SchemeRecord>
	<SchemeRecord>
		<ID ID="9edb45c5-b36f-448f-9930-94c5d4b94f20">2</ID>
		<Name ID="edaf212a-fb92-44c7-9898-f216cdf09d8d">Самостоятельное подразделение</Name>
	</SchemeRecord>
</SchemeTable>