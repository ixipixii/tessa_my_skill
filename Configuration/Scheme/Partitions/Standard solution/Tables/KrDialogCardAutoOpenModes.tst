<?xml version="1.0" encoding="utf-8"?>
<SchemeTable Partition="d1b372f3-7565-4309-9037-5e5a0969d94e" ID="b1827f66-89bd-4269-b2ce-ea27337616fd" Name="KrDialogCardAutoOpenModes" Group="KrStageTypes">
	<SchemePhysicalColumn ID="fca3e61d-c404-4dd1-8980-e069f10512ac" Name="ID" Type="Int32 Not Null" />
	<SchemePhysicalColumn ID="915af02d-d52b-40b6-9d35-34ce36491731" Name="Name" Type="String(Max) Not Null" />
	<SchemePrimaryKey ID="5848ef29-677e-4e29-a364-5f70eae011ac" Name="pk_KrDialogCardAutoOpenModes">
		<SchemeIndexedColumn Column="fca3e61d-c404-4dd1-8980-e069f10512ac" />
	</SchemePrimaryKey>
	<SchemeRecord>
		<ID ID="fca3e61d-c404-4dd1-8980-e069f10512ac">0</ID>
		<Name ID="915af02d-d52b-40b6-9d35-34ce36491731">$KrStages_Dialog_AlwaysOpen</Name>
	</SchemeRecord>
	<SchemeRecord>
		<ID ID="fca3e61d-c404-4dd1-8980-e069f10512ac">1</ID>
		<Name ID="915af02d-d52b-40b6-9d35-34ce36491731">$KrStages_Dialog_ButtonClickOpen</Name>
	</SchemeRecord>
</SchemeTable>