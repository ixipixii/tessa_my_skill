<?xml version="1.0" encoding="utf-8"?>
<SchemeTable Partition="54ed79f5-11c0-4f74-b8b0-793d596699d6" ID="431d3734-0c5e-443f-963e-88bbc673afc2" Name="PnrOriginal" Group="Pnr">
	<Description>Оригинал</Description>
	<SchemePhysicalColumn ID="c0b1086b-4fc7-42c8-9e15-620caa6b4657" Name="ID" Type="Int16 Not Null" />
	<SchemePhysicalColumn ID="e14ded34-3b12-4538-9c1b-7f25e8b769ad" Name="Name" Type="String(4) Not Null" />
	<SchemePrimaryKey ID="29a20807-2679-4523-8401-d6399005cf2e" Name="pk_PnrOriginal" IsClustered="true">
		<SchemeIndexedColumn Column="c0b1086b-4fc7-42c8-9e15-620caa6b4657" />
	</SchemePrimaryKey>
	<SchemeRecord>
		<ID ID="c0b1086b-4fc7-42c8-9e15-620caa6b4657">0</ID>
		<Name ID="e14ded34-3b12-4538-9c1b-7f25e8b769ad">Нет</Name>
	</SchemeRecord>
	<SchemeRecord>
		<ID ID="c0b1086b-4fc7-42c8-9e15-620caa6b4657">1</ID>
		<Name ID="e14ded34-3b12-4538-9c1b-7f25e8b769ad">Да</Name>
	</SchemeRecord>
</SchemeTable>