<?xml version="1.0" encoding="utf-8"?>
<SchemeTable ID="301215dc-be92-4ea6-8099-453f2478b9bd" Name="NotificationPostponement" Group="System" InstanceType="Cards" ContentType="Entries">
	<Description>Настройки для уведомлений по отсрочке заданий.</Description>
	<SchemeComplexColumn IsSystem="true" IsPermanent="true" IsSealed="true" ID="301215dc-be92-00a6-2000-053f2478b9bd" Name="ID" Type="Reference(Typified) Not Null" ReferencedTable="1074eadd-21d7-4925-98c8-40d1e5f0ca0e">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="301215dc-be92-01a6-4000-053f2478b9bd" Name="ID" Type="Guid Not Null" ReferencedColumn="9a58123b-b2e9-4137-9c6c-5dab0ec02747" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn ID="d11bd319-4117-4b25-b045-0fe02f197923" Name="MailSubject" Type="String(255) Null">
		<Description>Заголовок письма с уведомлением о том, что отложенное задание было возвращено в работу, поскольку время откладывания истекло.</Description>
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="614565e7-b211-49b8-9f03-194294def1f3" Name="MailBody" Type="String(Max) Null">
		<Description>Тело письма с уведомлением о том, что отложенное задание было возвращено в работу, поскольку время откладывания истекло.&#xD;
Тело может содержать специальные плейсхолдеры, сообщающие информацию по заданию.</Description>
	</SchemePhysicalColumn>
	<SchemePrimaryKey IsSystem="true" IsPermanent="true" IsSealed="true" ID="301215dc-be92-00a6-5000-053f2478b9bd" Name="pk_NotificationPostponement" IsClustered="true">
		<SchemeIndexedColumn Column="301215dc-be92-01a6-4000-053f2478b9bd" />
	</SchemePrimaryKey>
</SchemeTable>