<?xml version="1.0" encoding="utf-8"?>
<SchemeTable Partition="c3b14969-a553-4005-9a07-e9705c15b1e1" ID="09bfb67e-ba6a-49a6-a823-d7dbe99a83a2" Name="FdTaskOptionVisibilityModes" Group="Fd Enum">
	<SchemePhysicalColumn ID="15faae15-99f1-486a-8cc4-e149a6c54e6e" Name="ID" Type="Int32 Not Null" />
	<SchemePhysicalColumn ID="b5132bad-3ec7-42f5-b5c4-c6750ce5717f" Name="Name" Type="String(128) Not Null" />
	<SchemePrimaryKey ID="fd7a8dd4-dcc8-4a8a-8439-e1273e50b4f0" Name="pk_FdTaskOptionVisibilityModes">
		<SchemeIndexedColumn Column="15faae15-99f1-486a-8cc4-e149a6c54e6e" />
	</SchemePrimaryKey>
	<SchemeRecord>
		<ID ID="15faae15-99f1-486a-8cc4-e149a6c54e6e">0</ID>
		<Name ID="b5132bad-3ec7-42f5-b5c4-c6750ce5717f">Скрывать для указанных ролей</Name>
	</SchemeRecord>
	<SchemeRecord>
		<ID ID="15faae15-99f1-486a-8cc4-e149a6c54e6e">1</ID>
		<Name ID="b5132bad-3ec7-42f5-b5c4-c6750ce5717f">Показывать для указанных ролей</Name>
	</SchemeRecord>
</SchemeTable>