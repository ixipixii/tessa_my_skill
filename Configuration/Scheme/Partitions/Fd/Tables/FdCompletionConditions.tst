<?xml version="1.0" encoding="utf-8"?>
<SchemeTable Partition="c3b14969-a553-4005-9a07-e9705c15b1e1" ID="4f3b337b-42cf-454f-9010-fee2d827338d" Name="FdCompletionConditions" Group="Fd">
	<SchemePhysicalColumn ID="614130b3-a0fb-423c-ab7e-bfa863c5121e" Name="ID" Type="Int32 Not Null" />
	<SchemePhysicalColumn ID="3f8b60eb-e7a0-43cf-9df2-0862fd54d4c6" Name="Name" Type="String(128) Not Null" />
	<SchemePhysicalColumn ID="a3c60540-fb02-45c9-8bc6-8106cbd5ed77" Name="Caption" Type="String(128) Not Null" />
	<SchemePrimaryKey ID="d7adfe42-50df-40ab-b220-827c6cd428f7" Name="pk_FdCompletionConditions">
		<SchemeIndexedColumn Column="614130b3-a0fb-423c-ab7e-bfa863c5121e" />
	</SchemePrimaryKey>
	<SchemeRecord>
		<ID ID="614130b3-a0fb-423c-ab7e-bfa863c5121e">0</ID>
		<Name ID="3f8b60eb-e7a0-43cf-9df2-0862fd54d4c6">All</Name>
		<Caption ID="a3c60540-fb02-45c9-8bc6-8106cbd5ed77">Все</Caption>
	</SchemeRecord>
	<SchemeRecord>
		<ID ID="614130b3-a0fb-423c-ab7e-bfa863c5121e">1</ID>
		<Name ID="3f8b60eb-e7a0-43cf-9df2-0862fd54d4c6">AtLeastOne</Name>
		<Caption ID="a3c60540-fb02-45c9-8bc6-8106cbd5ed77">Хотя бы один</Caption>
	</SchemeRecord>
</SchemeTable>