<?xml version="1.0" encoding="utf-8"?>
<SchemeTable Partition="c3b14969-a553-4005-9a07-e9705c15b1e1" ID="7d88534f-8bce-4084-a150-64e9594d77c8" Name="FdProcessTemplate" Group="Fd" InstanceType="Cards" ContentType="Entries">
	<SchemeComplexColumn IsSystem="true" IsPermanent="true" IsSealed="true" ID="7d88534f-8bce-0084-2000-04e9594d77c8" Name="ID" Type="Reference(Typified) Not Null" ReferencedTable="1074eadd-21d7-4925-98c8-40d1e5f0ca0e">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="7d88534f-8bce-0184-4000-04e9594d77c8" Name="ID" Type="Guid Not Null" ReferencedColumn="9a58123b-b2e9-4137-9c6c-5dab0ec02747" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn ID="40395010-1055-40c2-bc1e-a095c16b3517" Name="Name" Type="String(255) Not Null" />
	<SchemePhysicalColumn ID="de984343-fa76-423d-ab78-797c0c1b8b36" Name="Description" Type="String(Max) Null">
		<Description>Описание</Description>
	</SchemePhysicalColumn>
	<SchemeComplexColumn ID="c681e9dc-f071-46fa-b91c-b09bc3e04763" Name="CardType" Type="Reference(Typified) Not Null" ReferencedTable="b0538ece-8468-4d0b-8b4e-5a1d43e024db">
		<Description>Тип карточки</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="c681e9dc-f071-00fa-4000-009bc3e04763" Name="CardTypeID" Type="Guid Not Null" ReferencedColumn="a628a864-c858-4200-a6b7-da78c8e6e1f4" />
		<SchemeReferencingColumn ID="1aac9ef7-619c-4a3c-bc35-3d0d4fcbfb11" Name="CardTypeCaption" Type="String(128) Not Null" ReferencedColumn="0a02451e-2e06-4001-9138-b4805e641afa" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn ID="4d1cc841-6e98-4872-aaaf-43f2cc55605d" Name="StartTileCaption" Type="String(128) Not Null">
		<Description>Текст тайла запуска процесса</Description>
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="01761863-22fd-416a-9aab-809111bf34fc" Name="IsActive" Type="Boolean Not Null">
		<Description>Является ли процесс автивным.</Description>
		<SchemeDefaultConstraint IsPermanent="true" ID="f88d395e-ea96-4c44-8c3c-4e1b42c6b6c0" Name="df_FdProcessTemplate_IsActive" Value="true" />
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="8fc9b185-acb8-4439-808f-d45a74c4a3ac" Name="AutoStartOnCardStore" Type="Boolean Not Null">
		<Description>Автоматически запускать процесс при сохранении карточки</Description>
		<SchemeDefaultConstraint IsPermanent="true" ID="50e4ad48-f1ad-4781-83d5-308eeef06bd9" Name="df_FdProcessTemplate_AutoStartOnCardStore" Value="false" />
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="434ec10b-71e2-412f-ac24-898dd2a4c6c7" Name="NameIcon" Type="String(128) Not Null">
		<SchemeDefaultConstraint IsPermanent="true" ID="5ff3f856-a2d0-453a-872d-a03ec36028c3" Name="df_FdProcessTemplate_NameIcon" Value="Thin360" />
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="e6a6dc5b-2b25-4343-86f7-e1f8de25643b" Name="StartTileIcon" Type="String(128) Not Null">
		<SchemeDefaultConstraint IsPermanent="true" ID="f628fba3-846a-4fee-b038-fa220d6f937f" Name="df_FdProcessTemplate_StartTileIcon" Value="Thin258" />
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="e68c41c2-41e0-4e09-a9c9-89e063acaed5" Name="UnlimitedActiveInstancesCount" Type="Boolean Not Null">
		<SchemeDefaultConstraint IsPermanent="true" ID="9d305ca1-10fc-44cb-ab90-e17d8e6a5b5c" Name="df_FdProcessTemplate_UnlimitedActiveInstancesCount" Value="false" />
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="c6d6cec0-8bee-455b-b7ea-58f44121849c" Name="CancelProcessStartWhenParticipantsEmpty" Type="Boolean Not Null">
		<SchemeDefaultConstraint IsPermanent="true" ID="114db090-3e51-4673-bffb-17220f1e0948" Name="df_FdProcessTemplate_CancelProcessStartWhenParticipantsEmpty" Value="false" />
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="0a2ea7fa-3ef3-4be8-bfba-c2074165ac07" Name="AllowCompleteExistingTasks" Type="Boolean Not Null">
		<Description>Разрешено ли завершать существующие задачи (используется для случая, когда шаблон процесс не является активным)</Description>
		<SchemeDefaultConstraint IsPermanent="true" ID="3c150c75-e2ad-40d2-ba9c-8e33fd6e4bc3" Name="df_FdProcessTemplate_AllowCompleteExistingTasks" Value="false" />
	</SchemePhysicalColumn>
	<SchemeComplexColumn ID="1eef2f98-6e56-47fd-ae9a-00c3e486cee6" Name="WhenProcessRevokedNewState" Type="Reference(Typified) Null" ReferencedTable="c5e015fe-1b55-4a18-8787-074b5d2ec80c">
		<Description>Состояние, в которое переводится карточка при отзыве процесса</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="1eef2f98-6e56-00fd-4000-00c3e486cee6" Name="WhenProcessRevokedNewStateID" Type="Guid Null" ReferencedColumn="c5e015fe-1b55-0118-4000-074b5d2ec80c" />
		<SchemeReferencingColumn ID="35d8c115-1338-4e09-96da-0cc6fc836ae7" Name="WhenProcessRevokedNewStateName" Type="String(128) Null" ReferencedColumn="09d1dfdb-262f-4b6e-b14d-64e5f51b7fa7" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn ID="fd6440ec-9c9c-4085-9d1e-74d090cd4fa8" Name="CheckInitialStageFound" Type="Boolean Not Null">
		<Description>Учитывать наличие подходящего стартового этапа при расчете видимости тайла запуска процесса</Description>
		<SchemeDefaultConstraint IsPermanent="true" ID="075c92f3-2f98-466c-9343-281c13f02169" Name="df_FdProcessTemplate_CheckInitialStageFound" Value="false" />
	</SchemePhysicalColumn>
	<SchemePrimaryKey IsSystem="true" IsPermanent="true" IsSealed="true" ID="7d88534f-8bce-0084-5000-04e9594d77c8" Name="pk_FdProcessTemplate" IsClustered="true">
		<SchemeIndexedColumn Column="7d88534f-8bce-0184-4000-04e9594d77c8" />
	</SchemePrimaryKey>
</SchemeTable>