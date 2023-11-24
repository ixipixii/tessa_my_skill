<?xml version="1.0" encoding="utf-8"?>
<SchemeTable Partition="54ed79f5-11c0-4f74-b8b0-793d596699d6" ID="fb6e1129-6e9f-4d61-842a-2ad319a4b328" Name="PnrCommentaryBudget" Group="Pnr">
	<Description>Комментарий к бюджету</Description>
	<SchemePhysicalColumn ID="aef4a1d6-e087-4705-b25b-80f15b39ef34" Name="ID" Type="Int16 Not Null" />
	<SchemePhysicalColumn ID="f7189dc8-387e-4cc6-9f98-e1765b544573" Name="Content" Type="String(Max) Null">
		<Description>Комментарий к бюджету </Description>
	</SchemePhysicalColumn>
	<SchemePrimaryKey ID="2809f465-b70d-4305-931d-b4caa09a4455" Name="pk_PnrCommentaryBudget" IsClustered="true">
		<SchemeIndexedColumn Column="aef4a1d6-e087-4705-b25b-80f15b39ef34" />
	</SchemePrimaryKey>
	<SchemeRecord>
		<ID ID="aef4a1d6-e087-4705-b25b-80f15b39ef34">0</ID>
		<Content ID="f7189dc8-387e-4cc6-9f98-e1765b544573">В бюджете</Content>
	</SchemeRecord>
	<SchemeRecord>
		<ID ID="aef4a1d6-e087-4705-b25b-80f15b39ef34">1</ID>
		<Content ID="f7189dc8-387e-4cc6-9f98-e1765b544573">Вне бюджета</Content>
	</SchemeRecord>
	<SchemeRecord>
		<ID ID="aef4a1d6-e087-4705-b25b-80f15b39ef34">2</ID>
		<Content ID="f7189dc8-387e-4cc6-9f98-e1765b544573">Из резерва</Content>
	</SchemeRecord>
</SchemeTable>