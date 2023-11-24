<?xml version="1.0" encoding="utf-8"?>
<SchemeTable Partition="c3b14969-a553-4005-9a07-e9705c15b1e1" ID="547b0f43-bdc4-4173-8338-4a3b08ebfde2" Name="FdActiveTasks" Group="Fd" InstanceType="Cards" ContentType="Collections">
	<Description>Активные задания сателлита</Description>
	<SchemeComplexColumn IsSystem="true" IsPermanent="true" IsSealed="true" ID="547b0f43-bdc4-0073-2000-0a3b08ebfde2" Name="ID" Type="Reference(Typified) Not Null" ReferencedTable="1074eadd-21d7-4925-98c8-40d1e5f0ca0e">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="547b0f43-bdc4-0173-4000-0a3b08ebfde2" Name="ID" Type="Guid Not Null" ReferencedColumn="9a58123b-b2e9-4137-9c6c-5dab0ec02747" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn IsSystem="true" IsPermanent="true" IsSealed="true" ID="547b0f43-bdc4-0073-3100-0a3b08ebfde2" Name="RowID" Type="Guid Not Null" />
	<SchemePhysicalColumn ID="62229750-c1b3-4b7c-8f9e-7dd2dca0f72d" Name="TaskID" Type="Guid Not Null">
		<Description>Ссылка на активное задание</Description>
	</SchemePhysicalColumn>
	<SchemeComplexColumn ID="c2ca22c5-2e89-4a9a-9ae7-e87a5c159b8f" Name="Process" Type="Reference(Typified) Not Null" ReferencedTable="2114510a-e165-4491-afcc-1756400e30a0">
		<Description>Ссылка на экземпляр процесса</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="c2ca22c5-2e89-009a-4000-087a5c159b8f" Name="ProcessRowID" Type="Guid Not Null" ReferencedColumn="2114510a-e165-0091-3100-0756400e30a0" />
	</SchemeComplexColumn>
	<SchemeComplexColumn ID="b78d38d6-7694-40bc-b238-974ff716c06b" Name="Stage" Type="Reference(Typified) Not Null" ReferencedTable="ffced5f3-fea9-41e1-bcc6-b156b3e3c054" DeleteReferentialAction="Cascade">
		<Description>Ссылка на экземпляр этапа</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="b78d38d6-7694-00bc-4000-074ff716c06b" Name="StageRowID" Type="Guid Not Null" ReferencedColumn="ffced5f3-fea9-00e1-3100-0156b3e3c054" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn ID="dd9cebf3-4e47-424f-8aeb-f87477569490" Name="ParticipantID" Type="Guid Not Null" />
	<SchemePrimaryKey IsSystem="true" IsPermanent="true" IsSealed="true" ID="547b0f43-bdc4-0073-5000-0a3b08ebfde2" Name="pk_FdActiveTasks">
		<SchemeIndexedColumn Column="547b0f43-bdc4-0073-3100-0a3b08ebfde2" />
	</SchemePrimaryKey>
	<SchemeIndex IsSystem="true" IsPermanent="true" IsSealed="true" ID="547b0f43-bdc4-0073-7000-0a3b08ebfde2" Name="idx_FdActiveTasks_ID" IsClustered="true">
		<SchemeIndexedColumn Column="547b0f43-bdc4-0173-4000-0a3b08ebfde2" />
	</SchemeIndex>
</SchemeTable>