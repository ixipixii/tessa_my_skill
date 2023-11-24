<?xml version="1.0" encoding="utf-8"?>
<SchemeTable Partition="c3b14969-a553-4005-9a07-e9705c15b1e1" ID="876678c4-d74f-4500-b56c-dbc80d57e4cf" Name="FdAdditionalApprovalInfoVirtual" Group="Fd" IsVirtual="true" InstanceType="Tasks" ContentType="Collections">
	<SchemeComplexColumn IsSystem="true" IsPermanent="true" IsSealed="true" ID="876678c4-d74f-0000-2000-0bc80d57e4cf" Name="ID" Type="Reference(Typified) Not Null" ReferencedTable="5bfa9936-bb5a-4e8f-89a9-180bfd8f75f8">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="876678c4-d74f-0100-4000-0bc80d57e4cf" Name="ID" Type="Guid Not Null" ReferencedColumn="5bfa9936-bb5a-008f-3100-080bfd8f75f8" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn IsSystem="true" IsPermanent="true" IsSealed="true" ID="876678c4-d74f-0000-3100-0bc80d57e4cf" Name="RowID" Type="Guid Not Null" />
	<SchemeComplexColumn ID="21814c6e-d3dc-4500-afdf-9d98063fc1bf" Name="Performer" Type="Reference(Typified) Not Null" ReferencedTable="81f6010b-9641-4aa5-8897-b8e8603fbf4b" WithForeignKey="false">
		<Description>Роль, на которую назначено задание.
Может быть временной ролью, которая удалится после завершения задания.</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="21814c6e-d3dc-0000-4000-0d98063fc1bf" Name="PerformerID" Type="Guid Not Null" ReferencedColumn="81f6010b-9641-01a5-4000-08e8603fbf4b" />
		<SchemeReferencingColumn ID="87a3e224-adbd-41b7-b322-b8a6d632b533" Name="PerformerName" Type="String(128) Not Null" ReferencedColumn="616d6b2e-35d5-424d-846b-618eb25962d0" />
	</SchemeComplexColumn>
	<SchemeComplexColumn ID="94a51ff7-efc4-42e5-ae94-c4db13d5e6f6" Name="User" Type="Reference(Typified) Null" ReferencedTable="6c977939-bbfc-456f-a133-f1c2244e3cc3">
		<Description>Пользователь, который взял задание в работу или завершил его, или Null, если задание было создано, но не было взято в работу.</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="94a51ff7-efc4-00e5-4000-04db13d5e6f6" Name="UserID" Type="Guid Null" ReferencedColumn="6c977939-bbfc-016f-4000-01c2244e3cc3" />
		<SchemeReferencingColumn ID="53510b14-57ff-46d5-bad3-fe780526c46f" Name="UserName" Type="String(128) Null" ReferencedColumn="1782f76a-4743-4aa4-920c-7edaee860964" />
	</SchemeComplexColumn>
	<SchemeComplexColumn ID="a2a72041-149a-4700-a743-ebe83c6f925b" Name="Option" Type="Reference(Typified) Null" ReferencedTable="08cf782d-4130-4377-8a49-3e201a05d496">
		<Description>Вариант завершения задания или Null, если задание ещё не было завершено.</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="a2a72041-149a-0000-4000-0be83c6f925b" Name="OptionID" Type="Guid Null" ReferencedColumn="132dc5f5-ce87-4dd0-acce-b4a02acf7715" />
		<SchemeReferencingColumn ID="97644624-efc5-439f-9e67-493f158cff9a" Name="OptionCaption" Type="String(128) Null" ReferencedColumn="6762309a-b0ff-4b2f-9cce-dd111116e554" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn ID="1e28aa04-e166-4e0c-889a-31bd08f4bf01" Name="Comment" Type="String(Max) Null">
		<Description>Комментарий к заданию доп. согласования. Например, вопрос, который был задан.</Description>
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="2787b357-78f0-4c11-8fef-1b293b84a50d" Name="Answer" Type="String(Max) Null">
		<Description>Ответный комментарий на задание доп. согласования или Null, если комментарий ещё не был указан.</Description>
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="fc6f9550-129d-48ba-b0a1-852fa62e6137" Name="Created" Type="DateTime Not Null">
		<Description>Дата и время, когда отправлено задание.</Description>
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="969ce9cf-a36b-469d-a1d5-56d55ca217b7" Name="InProgress" Type="DateTime Null">
		<Description>Дата взятия задания в работу или Null, если резолюция ещё не была взята в работу.</Description>
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="dc1a6390-4e27-4bc5-923a-3aab1e337c67" Name="Planned" Type="DateTime Not Null">
		<Description>Дата и время запланированного завершения задания.</Description>
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="93a33bad-eac3-43a8-87f1-94f791d34c77" Name="Completed" Type="DateTime Null">
		<Description>Дата завершения задания или Null, если задание ещё не было завершено.</Description>
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="d9434b1f-f452-42b2-a0e9-3d34e0966fde" Name="ColumnComment" Type="String(Max) Null">
		<Description>Краткий комментарий к заданию доп. согласования.
Выводится в колонке таблицы для заданий доп. согласования.</Description>
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="e6cd162e-f209-4b80-988f-1d1aab226938" Name="ColumnState" Type="String(Max) Null">
		<Description>Краткая информация по текущему заданию доп. согласования.
Выводится в колонке таблицы для заданий доп. согласования.</Description>
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="cf2e62f7-f57c-4474-a8d5-7bab10232427" Name="IsResponsible" Type="Boolean Not Null">
		<Description>Признак, что исполнитель установлен, как "Первый - ответсвенный".</Description>
		<SchemeDefaultConstraint IsPermanent="true" ID="abe485e1-8a7a-4f26-8eb5-6bfc5a8925e6" Name="df_FdAdditionalApprovalInfoVirtual_IsResponsible" Value="false" />
	</SchemePhysicalColumn>
	<SchemePrimaryKey IsSystem="true" IsPermanent="true" IsSealed="true" ID="876678c4-d74f-0000-5000-0bc80d57e4cf" Name="pk_FdAdditionalApprovalInfoVirtual">
		<SchemeIndexedColumn Column="876678c4-d74f-0000-3100-0bc80d57e4cf" />
	</SchemePrimaryKey>
	<SchemeIndex IsSystem="true" IsPermanent="true" IsSealed="true" ID="876678c4-d74f-0000-7000-0bc80d57e4cf" Name="idx_FdAdditionalApprovalInfoVirtual_ID" IsClustered="true">
		<SchemeIndexedColumn Column="876678c4-d74f-0100-4000-0bc80d57e4cf" />
	</SchemeIndex>
</SchemeTable>