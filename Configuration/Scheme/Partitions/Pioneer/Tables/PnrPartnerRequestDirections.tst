<?xml version="1.0" encoding="utf-8"?>
<SchemeTable Partition="54ed79f5-11c0-4f74-b8b0-793d596699d6" ID="41d0d309-7f0b-4f14-a0b4-0f55c50fed7b" Name="PnrPartnerRequestDirections" Group="Pnr">
	<Description>Направление заявки на контрагента</Description>
	<SchemePhysicalColumn ID="33080600-7fdf-4c78-a8a4-d7dcf5e5d5cc" Name="ID" Type="Int16 Not Null" />
	<SchemePhysicalColumn ID="c8bed3cc-fc2a-4232-b43a-8559fd5fa4d3" Name="Name" Type="String(Max) Null" />
	<SchemePrimaryKey ID="003d5db6-8c45-43a9-859b-c0e2067b62ad" Name="pk_PnrPartnerRequestDirections" IsClustered="true">
		<SchemeIndexedColumn Column="33080600-7fdf-4c78-a8a4-d7dcf5e5d5cc" />
	</SchemePrimaryKey>
	<SchemeRecord>
		<ID ID="33080600-7fdf-4c78-a8a4-d7dcf5e5d5cc">0</ID>
		<Name ID="c8bed3cc-fc2a-4232-b43a-8559fd5fa4d3">Группа компаний Пионер</Name>
	</SchemeRecord>
	<SchemeRecord>
		<ID ID="33080600-7fdf-4c78-a8a4-d7dcf5e5d5cc">1</ID>
		<Name ID="c8bed3cc-fc2a-4232-b43a-8559fd5fa4d3">Управляющая компания</Name>
	</SchemeRecord>
</SchemeTable>