<?xml version="1.0" encoding="utf-8"?>
<SchemeTable Partition="c3b14969-a553-4005-9a07-e9705c15b1e1" ID="18cfec7a-f5db-48d3-b669-aa6ff51b6ae9" Name="FdCompletedTasks" Group="Fd" InstanceType="Cards" ContentType="Collections">
	<Description>Хранятся результаты завершения всех заданий, используется для проверки критериев при переходах между этапами</Description>
	<SchemeComplexColumn IsSystem="true" IsPermanent="true" IsSealed="true" ID="18cfec7a-f5db-00d3-2000-0a6ff51b6ae9" Name="ID" Type="Reference(Typified) Not Null" ReferencedTable="1074eadd-21d7-4925-98c8-40d1e5f0ca0e">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="18cfec7a-f5db-01d3-4000-0a6ff51b6ae9" Name="ID" Type="Guid Not Null" ReferencedColumn="9a58123b-b2e9-4137-9c6c-5dab0ec02747" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn IsSystem="true" IsPermanent="true" IsSealed="true" ID="18cfec7a-f5db-00d3-3100-0a6ff51b6ae9" Name="RowID" Type="Guid Not Null" />
	<SchemeComplexColumn ID="e71654aa-ced6-4edf-9545-723b6e8e262e" Name="Process" Type="Reference(Typified) Not Null" ReferencedTable="2114510a-e165-4491-afcc-1756400e30a0">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="e71654aa-ced6-00df-4000-023b6e8e262e" Name="ProcessRowID" Type="Guid Not Null" ReferencedColumn="2114510a-e165-0091-3100-0756400e30a0" />
	</SchemeComplexColumn>
	<SchemeComplexColumn ID="8ce8a99f-8b47-4ce7-b901-c35914566177" Name="Stage" Type="Reference(Typified) Not Null" ReferencedTable="ffced5f3-fea9-41e1-bcc6-b156b3e3c054" DeleteReferentialAction="Cascade">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="8ce8a99f-8b47-00e7-4000-035914566177" Name="StageRowID" Type="Guid Not Null" ReferencedColumn="ffced5f3-fea9-00e1-3100-0156b3e3c054" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn ID="c076d29f-335e-44a7-9f53-0cdff3e1df98" Name="TaskID" Type="Guid Not Null" />
	<SchemeComplexColumn ID="9afc1e4f-ba7f-4594-bcf0-9f7123579d9a" Name="Option" Type="Reference(Typified) Not Null" ReferencedTable="08cf782d-4130-4377-8a49-3e201a05d496">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="9afc1e4f-ba7f-0094-4000-0f7123579d9a" Name="OptionID" Type="Guid Not Null" ReferencedColumn="132dc5f5-ce87-4dd0-acce-b4a02acf7715" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn ID="42e5b804-33e4-4ba9-8e97-999f56d44ff9" Name="IsMain" Type="Boolean Not Null">
		<Description>Является ли задание основным, т.е. учитываться при завершении этапа</Description>
		<SchemeDefaultConstraint IsPermanent="true" ID="910ba223-3fe4-4042-8ef6-7ac6f72c3190" Name="df_FdCompletedTasks_IsMain" Value="true" />
	</SchemePhysicalColumn>
	<SchemePrimaryKey IsSystem="true" IsPermanent="true" IsSealed="true" ID="18cfec7a-f5db-00d3-5000-0a6ff51b6ae9" Name="pk_FdCompletedTasks">
		<SchemeIndexedColumn Column="18cfec7a-f5db-00d3-3100-0a6ff51b6ae9" />
	</SchemePrimaryKey>
	<SchemeIndex IsSystem="true" IsPermanent="true" IsSealed="true" ID="18cfec7a-f5db-00d3-7000-0a6ff51b6ae9" Name="idx_FdCompletedTasks_ID" IsClustered="true">
		<SchemeIndexedColumn Column="18cfec7a-f5db-01d3-4000-0a6ff51b6ae9" />
	</SchemeIndex>
</SchemeTable>