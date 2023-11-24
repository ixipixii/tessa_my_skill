<?xml version="1.0" encoding="utf-8"?>
<SchemeTable Partition="c3b14969-a553-4005-9a07-e9705c15b1e1" ID="2114510a-e165-4491-afcc-1756400e30a0" Name="FdProcessInstances" Group="Fd" InstanceType="Cards" ContentType="Collections">
	<Description>Экземпляры процессов сателлита</Description>
	<SchemeComplexColumn IsSystem="true" IsPermanent="true" IsSealed="true" ID="2114510a-e165-0091-2000-0756400e30a0" Name="ID" Type="Reference(Typified) Not Null" ReferencedTable="1074eadd-21d7-4925-98c8-40d1e5f0ca0e">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="2114510a-e165-0191-4000-0756400e30a0" Name="ID" Type="Guid Not Null" ReferencedColumn="9a58123b-b2e9-4137-9c6c-5dab0ec02747" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn IsSystem="true" IsPermanent="true" IsSealed="true" ID="2114510a-e165-0091-3100-0756400e30a0" Name="RowID" Type="Guid Not Null" />
	<SchemeComplexColumn ID="a1cca276-8afe-43fc-abf1-5b612cbcc7ed" Name="BasedOnProcessTemplate" Type="Reference(Typified) Not Null" ReferencedTable="7d88534f-8bce-4084-a150-64e9594d77c8" WithForeignKey="false">
		<Description>Шаблон процесса, к которому относится экземпляр</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="a1cca276-8afe-00fc-4000-0b612cbcc7ed" Name="BasedOnProcessTemplateID" Type="Guid Not Null" ReferencedColumn="7d88534f-8bce-0184-4000-04e9594d77c8" />
		<SchemeReferencingColumn ID="7943982a-8043-473b-b3da-52885cefb0d1" Name="BasedOnProcessTemplateName" Type="String(255) Not Null" ReferencedColumn="40395010-1055-40c2-bc1e-a095c16b3517" />
	</SchemeComplexColumn>
	<SchemeComplexColumn ID="9801e763-775f-42ca-8f32-328f1a0edc82" Name="Author" Type="Reference(Typified) Null" ReferencedTable="6c977939-bbfc-456f-a133-f1c2244e3cc3">
		<Description>Инициатор процесса</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="9801e763-775f-00ca-4000-028f1a0edc82" Name="AuthorID" Type="Guid Null" ReferencedColumn="6c977939-bbfc-016f-4000-01c2244e3cc3" />
		<SchemeReferencingColumn ID="e1e20a33-72b8-4b7e-a928-26bf6abade6e" Name="AuthorName" Type="String(128) Null" ReferencedColumn="1782f76a-4743-4aa4-920c-7edaee860964" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn ID="b8c73c1c-5189-441f-b5ce-e18ec8e5f818" Name="Cycle" Type="Int16 Not Null">
		<Description>Номер цикла</Description>
		<SchemeDefaultConstraint IsPermanent="true" ID="ae0933d1-7104-4bb8-bb4d-c105757b06be" Name="df_FdProcessInstances_Cycle" Value="0" />
	</SchemePhysicalColumn>
	<SchemeComplexColumn ID="4f65ecb5-1516-44c6-83f5-a0de5b46ca25" Name="State" Type="Reference(Typified) Not Null" ReferencedTable="44c3a9b3-d245-445c-a1fe-2a21ce021e1e">
		<Description>Текущее состояние процесса</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="4f65ecb5-1516-00c6-4000-00de5b46ca25" Name="StateID" Type="Int16 Not Null" ReferencedColumn="3ba53faf-6661-4352-8c57-35e0b6c0dc3b" />
		<SchemeReferencingColumn ID="6fd93849-71a4-48ff-94d9-a85e8367016c" Name="StateName" Type="String(128) Not Null" ReferencedColumn="13bffd8e-b32f-4243-b8f9-ec07da0ef5c1" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn ID="93ad68c7-849d-43e9-8b18-3d74473243ff" Name="Order" Type="Int32 Not Null">
		<SchemeDefaultConstraint IsPermanent="true" ID="95763620-7645-4f18-ada9-a4b592a28be5" Name="df_FdProcessInstances_Order" Value="0" />
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="1279d223-16d3-45ae-95ed-be8da5b802d9" Name="Name" Type="String(255) Not Null" />
	<SchemePhysicalColumn ID="6ebe7a04-f407-4cf8-a4ad-7af8732595c9" Name="StartDate" Type="DateTime Not Null">
		<Description>Дата запуска процесса</Description>
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="503b46b5-308b-4329-b949-abe14c2a8649" Name="NeedRebuild" Type="Boolean Not Null">
		<Description>Отметка о том, что этап уже содержит несогласования и документ необходимо вернуть на доработку по завершении этого параллельного этапа (если этап последовательный, то документ возвращается не дожидаясь завершения этапа, а сразу после несогласования)</Description>
		<SchemeDefaultConstraint IsPermanent="true" ID="e68ecc5f-49d8-47b5-87e9-4c7c117e433a" Name="df_FdProcessInstances_NeedRebuild" Value="false" />
	</SchemePhysicalColumn>
	<SchemeComplexColumn ID="573f9785-c841-4308-a74f-f47c9a9fd4ea" Name="ParentProcessInstance" Type="Reference(Typified) Null" ReferencedTable="2114510a-e165-4491-afcc-1756400e30a0" WithForeignKey="false">
		<Description>Ссылка на родительский экземпляр процесса</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="573f9785-c841-0008-4000-047c9a9fd4ea" Name="ParentProcessInstanceRowID" Type="Guid Null" ReferencedColumn="2114510a-e165-0091-3100-0756400e30a0" />
		<SchemeReferencingColumn ID="c9acf7c8-6941-4785-8d1f-d3d9b992bac8" Name="ParentProcessInstanceName" Type="String(255) Null" ReferencedColumn="1279d223-16d3-45ae-95ed-be8da5b802d9" />
	</SchemeComplexColumn>
	<SchemePrimaryKey IsSystem="true" IsPermanent="true" IsSealed="true" ID="2114510a-e165-0091-5000-0756400e30a0" Name="pk_FdProcessInstances">
		<SchemeIndexedColumn Column="2114510a-e165-0091-3100-0756400e30a0" />
	</SchemePrimaryKey>
	<SchemeIndex IsSystem="true" IsPermanent="true" IsSealed="true" ID="2114510a-e165-0091-7000-0756400e30a0" Name="idx_FdProcessInstances_ID" IsClustered="true">
		<SchemeIndexedColumn Column="2114510a-e165-0191-4000-0756400e30a0" />
	</SchemeIndex>
</SchemeTable>