<?xml version="1.0" encoding="utf-8"?>
<SchemeTable Partition="c3b14969-a553-4005-9a07-e9705c15b1e1" ID="f540c029-ab17-461c-b6c0-736020f51221" Name="FdAdditionalApprovalInfo" Group="Fd" InstanceType="Tasks" ContentType="Collections">
	<SchemeComplexColumn IsSystem="true" IsPermanent="true" IsSealed="true" ID="f540c029-ab17-001c-2000-036020f51221" Name="ID" Type="Reference(Typified) Not Null" ReferencedTable="5bfa9936-bb5a-4e8f-89a9-180bfd8f75f8">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="f540c029-ab17-011c-4000-036020f51221" Name="ID" Type="Guid Not Null" ReferencedColumn="5bfa9936-bb5a-008f-3100-080bfd8f75f8" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn IsSystem="true" IsPermanent="true" IsSealed="true" ID="f540c029-ab17-001c-3100-036020f51221" Name="RowID" Type="Guid Not Null" />
	<SchemeComplexColumn ID="e0a5b537-798d-433c-8cac-15deacc1e04a" Name="Performer" Type="Reference(Typified) Not Null" ReferencedTable="81f6010b-9641-4aa5-8897-b8e8603fbf4b" WithForeignKey="false">
		<Description>Роль, на которую назначено задание.
Может быть временной ролью, которая удалится после завершения задания.</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="e0a5b537-798d-003c-4000-05deacc1e04a" Name="PerformerID" Type="Guid Not Null" ReferencedColumn="81f6010b-9641-01a5-4000-08e8603fbf4b" />
		<SchemeReferencingColumn ID="1e2b45cd-efd3-4f8f-9318-19e97a7ba5b9" Name="PerformerName" Type="String(128) Not Null" ReferencedColumn="616d6b2e-35d5-424d-846b-618eb25962d0" />
	</SchemeComplexColumn>
	<SchemeComplexColumn ID="fc285ed3-4c52-4b70-9190-a939776de555" Name="User" Type="Reference(Typified) Null" ReferencedTable="6c977939-bbfc-456f-a133-f1c2244e3cc3">
		<Description>Пользователь, который взял задание в работу или завершил его, или Null, если задание было создано, но не было взято в работу.</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="fc285ed3-4c52-0070-4000-0939776de555" Name="UserID" Type="Guid Null" ReferencedColumn="6c977939-bbfc-016f-4000-01c2244e3cc3" />
		<SchemeReferencingColumn ID="e9795087-47e6-4f0c-8a5f-515cae4bd12e" Name="UserName" Type="String(128) Null" ReferencedColumn="1782f76a-4743-4aa4-920c-7edaee860964" />
	</SchemeComplexColumn>
	<SchemeComplexColumn ID="f6caab98-1bfa-408d-93cc-a629be8ecaf9" Name="Option" Type="Reference(Typified) Null" ReferencedTable="08cf782d-4130-4377-8a49-3e201a05d496">
		<Description>Вариант завершения задания или Null, если задание ещё не было завершено.</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="f6caab98-1bfa-008d-4000-0629be8ecaf9" Name="OptionID" Type="Guid Null" ReferencedColumn="132dc5f5-ce87-4dd0-acce-b4a02acf7715" />
		<SchemeReferencingColumn ID="269842e3-6997-437f-8954-271efb810e64" Name="OptionCaption" Type="String(128) Null" ReferencedColumn="6762309a-b0ff-4b2f-9cce-dd111116e554" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn ID="d7694478-6f2e-40a9-86db-74f66d80454e" Name="Comment" Type="String(Max) Null">
		<Description>Комментарий к заданию доп. согласования. Например, вопрос, который был задан.</Description>
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="e5d8774c-8ae7-4f08-ac7c-f26184f0fd63" Name="Answer" Type="String(Max) Null">
		<Description>Ответный комментарий на задание доп. согласования или Null, если комментарий ещё не был указан.</Description>
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="513fa0da-ee02-414e-b4d1-331cdeb94bbe" Name="Created" Type="DateTime Not Null">
		<Description>Дата и время, когда отправлено задание.</Description>
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="8e91b25e-e199-49ba-b74b-6e27237f4fae" Name="InProgress" Type="DateTime Null">
		<Description>Дата взятия задания в работу или Null, если резолюция ещё не была взята в работу.</Description>
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="e88ef709-7ccb-420c-8813-cfebdb4417dc" Name="Planned" Type="DateTime Not Null">
		<Description>Дата и время запланированного завершения задания.</Description>
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="e2dbf129-b386-466b-84c3-a8312cbf933c" Name="Completed" Type="DateTime Null">
		<Description>Дата завершения задания или Null, если задание ещё не было завершено.</Description>
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="6b0bc79f-1e4c-410e-b246-763784b9063e" Name="IsResponsible" Type="Boolean Not Null">
		<Description>Признак, что исполнитель установлен, как "Первый - ответсвенный".</Description>
		<SchemeDefaultConstraint IsPermanent="true" ID="2da08575-a26d-486e-8c48-1ccc16b547e0" Name="df_FdAdditionalApprovalInfo_IsResponsible" Value="false" />
	</SchemePhysicalColumn>
	<SchemePrimaryKey IsSystem="true" IsPermanent="true" IsSealed="true" ID="f540c029-ab17-001c-5000-036020f51221" Name="pk_FdAdditionalApprovalInfo">
		<SchemeIndexedColumn Column="f540c029-ab17-001c-3100-036020f51221" />
	</SchemePrimaryKey>
	<SchemeIndex IsSystem="true" IsPermanent="true" IsSealed="true" ID="f540c029-ab17-001c-7000-036020f51221" Name="idx_FdAdditionalApprovalInfo_ID" IsClustered="true">
		<SchemeIndexedColumn Column="f540c029-ab17-011c-4000-036020f51221" />
	</SchemeIndex>
</SchemeTable>