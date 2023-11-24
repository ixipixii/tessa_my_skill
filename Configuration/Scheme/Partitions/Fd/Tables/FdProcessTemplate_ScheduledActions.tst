<?xml version="1.0" encoding="utf-8"?>
<SchemeTable Partition="c3b14969-a553-4005-9a07-e9705c15b1e1" ID="87ce3cbf-1311-4f36-9167-5881a0685608" Name="FdProcessTemplate_ScheduledActions" Group="Fd" InstanceType="Cards" ContentType="Collections">
	<Description>Настройка действий по заданному шаблону процесса по расписанию</Description>
	<SchemeComplexColumn IsSystem="true" IsPermanent="true" IsSealed="true" ID="87ce3cbf-1311-0036-2000-0881a0685608" Name="ID" Type="Reference(Typified) Not Null" ReferencedTable="1074eadd-21d7-4925-98c8-40d1e5f0ca0e">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="87ce3cbf-1311-0136-4000-0881a0685608" Name="ID" Type="Guid Not Null" ReferencedColumn="9a58123b-b2e9-4137-9c6c-5dab0ec02747" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn IsSystem="true" IsPermanent="true" IsSealed="true" ID="87ce3cbf-1311-0036-3100-0881a0685608" Name="RowID" Type="Guid Not Null" />
	<SchemePhysicalColumn ID="bfdac18b-083c-47df-bb57-424761ca61e5" Name="Name" Type="String(128) Null" />
	<SchemePhysicalColumn ID="ed2ffa71-4ba4-44fa-b72c-d5399ef8e30b" Name="SqlCommand" Type="String(Max) Not Null" />
	<SchemePhysicalColumn ID="ce84528b-d901-49ec-9afb-8f7adc4741c3" Name="LastExecutionDate" Type="DateTime Null">
		<Description>Дата последнего запуска планировщиком</Description>
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="4a8fdd00-952a-49bb-912e-61c070c76acb" Name="LimitCount" Type="Int32 Not Null">
		<Description>Максимальное кол-во запусков процесса планировщиком по каждой карточке</Description>
		<SchemeDefaultConstraint IsPermanent="true" ID="4c9fbb7c-7203-45f6-a51a-7a97f6cf390a" Name="df_FdProcessTemplate_ScheduledActions_LimitCount" Value="1" />
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="564c5673-a9f2-40e7-b024-2202ce5a28f4" Name="StartDate" Type="DateTime Not Null" />
	<SchemePhysicalColumn ID="4242ee4f-1687-403a-a5e2-a15004e73201" Name="PeriodInMinutes" Type="Int16 Not Null">
		<SchemeDefaultConstraint IsPermanent="true" ID="c007c67c-a128-4ea8-8e15-3aff974e71f6" Name="df_FdProcessTemplate_ScheduledActions_PeriodInMinutes" Value="0" />
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="9dc762bc-8a28-41ec-b83b-1d0ad53804b8" Name="PeriodInHours" Type="Int16 Not Null">
		<SchemeDefaultConstraint IsPermanent="true" ID="cafa3ad1-2667-4784-a7e4-3049cb026f3d" Name="df_FdProcessTemplate_ScheduledActions_PeriodInHours" Value="0" />
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="3dc813ea-8047-4b1a-a153-dd91346406ff" Name="PeriodInDays" Type="Int16 Not Null">
		<SchemeDefaultConstraint IsPermanent="true" ID="60b5aa29-d5c7-4268-bf5e-b79c69ca108f" Name="df_FdProcessTemplate_ScheduledActions_PeriodInDays" Value="0" />
	</SchemePhysicalColumn>
	<SchemePrimaryKey IsSystem="true" IsPermanent="true" IsSealed="true" ID="87ce3cbf-1311-0036-5000-0881a0685608" Name="pk_FdProcessTemplate_ScheduledActions">
		<SchemeIndexedColumn Column="87ce3cbf-1311-0036-3100-0881a0685608" />
	</SchemePrimaryKey>
	<SchemeIndex IsSystem="true" IsPermanent="true" IsSealed="true" ID="87ce3cbf-1311-0036-7000-0881a0685608" Name="idx_FdProcessTemplate_ScheduledActions_ID" IsClustered="true">
		<SchemeIndexedColumn Column="87ce3cbf-1311-0136-4000-0881a0685608" />
	</SchemeIndex>
</SchemeTable>