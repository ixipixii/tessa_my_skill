<?xml version="1.0" encoding="utf-8"?>
<SchemeTable Partition="d1b372f3-7565-4309-9037-5e5a0969d94e" ID="342d81f0-90da-4b37-b0b4-69498d54f99e" Name="WfTaskCards" Group="Wf" InstanceType="Cards" ContentType="Entries">
	<Description>Строковая секция для карточек-сателлитов для задач.</Description>
	<SchemeComplexColumn IsSystem="true" IsPermanent="true" IsSealed="true" ID="342d81f0-90da-0037-2000-09498d54f99e" Name="ID" Type="Reference(Typified) Not Null" ReferencedTable="1074eadd-21d7-4925-98c8-40d1e5f0ca0e">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="342d81f0-90da-0137-4000-09498d54f99e" Name="ID" Type="Guid Not Null" ReferencedColumn="9a58123b-b2e9-4137-9c6c-5dab0ec02747" />
	</SchemeComplexColumn>
	<SchemeComplexColumn ID="abf60d41-8408-41c1-a5a5-4b7e205b4bd6" Name="MainCard" Type="Reference(Abstract) Not Null" WithForeignKey="false">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="abf60d41-8408-00c1-4000-0b7e205b4bd6" Name="MainCardID" Type="Guid Not Null" ReferencedColumn="9a58123b-b2e9-4137-9c6c-5dab0ec02747" />
	</SchemeComplexColumn>
	<SchemeComplexColumn ID="cadccee9-7312-4db1-9969-b27dbb950da9" Name="Task" Type="Reference(Typified) Not Null" ReferencedTable="f8deab4c-fa9d-404a-8abc-b570cd81820e" WithForeignKey="false">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="cadccee9-7312-00b1-4000-027dbb950da9" Name="TaskRowID" Type="Guid Not Null" ReferencedColumn="f8deab4c-fa9d-004a-3100-0570cd81820e" />
	</SchemeComplexColumn>
	<SchemePrimaryKey IsSystem="true" IsPermanent="true" IsSealed="true" ID="342d81f0-90da-0037-5000-09498d54f99e" Name="pk_WfTaskCards" IsClustered="true">
		<SchemeIndexedColumn Column="342d81f0-90da-0137-4000-09498d54f99e" />
	</SchemePrimaryKey>
	<SchemeIndex ID="31852cee-0b42-48e1-a994-f43eacacaad0" Name="ndx_WfTaskCards_TaskRowID" IsUnique="true">
		<SchemeIndexedColumn Column="cadccee9-7312-00b1-4000-027dbb950da9" />
	</SchemeIndex>
	<SchemeIndex ID="91403362-b3c0-40a9-a264-1f2b1de74615" Name="ndx_WfTaskCards_MainCardIDTaskRowID" IsUnique="true">
		<SchemeIndexedColumn Column="abf60d41-8408-00c1-4000-0b7e205b4bd6" />
		<SchemeIndexedColumn Column="cadccee9-7312-00b1-4000-027dbb950da9" />
	</SchemeIndex>
</SchemeTable>