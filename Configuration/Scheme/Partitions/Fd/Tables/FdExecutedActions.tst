<?xml version="1.0" encoding="utf-8"?>
<SchemeTable Partition="c3b14969-a553-4005-9a07-e9705c15b1e1" ID="a28d1089-e490-43ed-b10a-9a32dddcecdc" Name="FdExecutedActions" Group="Fd" InstanceType="Cards" ContentType="Collections">
	<SchemeComplexColumn IsSystem="true" IsPermanent="true" IsSealed="true" ID="a28d1089-e490-00ed-2000-0a32dddcecdc" Name="ID" Type="Reference(Typified) Not Null" ReferencedTable="1074eadd-21d7-4925-98c8-40d1e5f0ca0e">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="a28d1089-e490-01ed-4000-0a32dddcecdc" Name="ID" Type="Guid Not Null" ReferencedColumn="9a58123b-b2e9-4137-9c6c-5dab0ec02747" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn IsSystem="true" IsPermanent="true" IsSealed="true" ID="a28d1089-e490-00ed-3100-0a32dddcecdc" Name="RowID" Type="Guid Not Null" />
	<SchemeComplexColumn ID="4c88f7fb-f05d-4634-86e6-b0d14b9b5ce2" Name="ScheduledAction" Type="Reference(Typified) Not Null" ReferencedTable="87ce3cbf-1311-4f36-9167-5881a0685608" WithForeignKey="false">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="4c88f7fb-f05d-0034-4000-00d14b9b5ce2" Name="ScheduledActionRowID" Type="Guid Not Null" ReferencedColumn="87ce3cbf-1311-0036-3100-0881a0685608" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn ID="d92d9ec8-2717-4f75-8dbc-483992053517" Name="Count" Type="Int32 Null" />
	<SchemePrimaryKey IsSystem="true" IsPermanent="true" IsSealed="true" ID="a28d1089-e490-00ed-5000-0a32dddcecdc" Name="pk_FdExecutedActions">
		<SchemeIndexedColumn Column="a28d1089-e490-00ed-3100-0a32dddcecdc" />
	</SchemePrimaryKey>
	<SchemeIndex IsSystem="true" IsPermanent="true" IsSealed="true" ID="a28d1089-e490-00ed-7000-0a32dddcecdc" Name="idx_FdExecutedActions_ID" IsClustered="true">
		<SchemeIndexedColumn Column="a28d1089-e490-01ed-4000-0a32dddcecdc" />
	</SchemeIndex>
</SchemeTable>