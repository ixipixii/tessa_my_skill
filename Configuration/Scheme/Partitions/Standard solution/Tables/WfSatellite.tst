<?xml version="1.0" encoding="utf-8"?>
<SchemeTable Partition="d1b372f3-7565-4309-9037-5e5a0969d94e" ID="05394727-2b6f-4d59-9900-d95bc8effdc5" Name="WfSatellite" Group="Wf" InstanceType="Cards" ContentType="Entries">
	<Description>Основная секция карточки-сателлита для бизнес-процессов Workflow.</Description>
	<SchemeComplexColumn IsSystem="true" IsPermanent="true" IsSealed="true" ID="05394727-2b6f-0059-2000-095bc8effdc5" Name="ID" Type="Reference(Typified) Not Null" ReferencedTable="1074eadd-21d7-4925-98c8-40d1e5f0ca0e">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="05394727-2b6f-0159-4000-095bc8effdc5" Name="ID" Type="Guid Not Null" ReferencedColumn="9a58123b-b2e9-4137-9c6c-5dab0ec02747" />
	</SchemeComplexColumn>
	<SchemeComplexColumn ID="18feded1-3eb3-46c4-ac26-467c6fdb754c" Name="MainCard" Type="Reference(Abstract) Not Null" WithForeignKey="false">
		<Description>Ссылка на основную карточку, к которой принадлежит карточка-сателлит.</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="18feded1-3eb3-00c4-4000-067c6fdb754c" Name="MainCardID" Type="Guid Not Null" ReferencedColumn="9a58123b-b2e9-4137-9c6c-5dab0ec02747" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn ID="d527ef7a-fc0e-4434-8195-fc6022bd95ef" Name="Data" Type="Binary(Max) Null">
		<Description>Неструктурированные данные по всем бизнес-процессам в сателлите, или Null, если такие данные отсутствуют.</Description>
	</SchemePhysicalColumn>
	<SchemePrimaryKey IsSystem="true" IsPermanent="true" IsSealed="true" ID="05394727-2b6f-0059-5000-095bc8effdc5" Name="pk_WfSatellite" IsClustered="true">
		<SchemeIndexedColumn Column="05394727-2b6f-0159-4000-095bc8effdc5" />
	</SchemePrimaryKey>
	<SchemeIndex ID="0e3a86dd-2813-4d53-af85-82cf3882bd83" Name="ndx_WfSatellite_MainCardID" IsUnique="true">
		<SchemeIndexedColumn Column="18feded1-3eb3-00c4-4000-067c6fdb754c" />
	</SchemeIndex>
</SchemeTable>