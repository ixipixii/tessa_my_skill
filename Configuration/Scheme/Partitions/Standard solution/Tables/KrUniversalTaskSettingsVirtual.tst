﻿<?xml version="1.0" encoding="utf-8"?>
<SchemeTable Partition="d1b372f3-7565-4309-9037-5e5a0969d94e" ID="01db90f2-22ec-4233-a5fd-587a832b1b48" Name="KrUniversalTaskSettingsVirtual" Group="KrStageTypes" IsVirtual="true" InstanceType="Cards" ContentType="Entries">
	<Description>Секция настроек этапа Универсальное задание</Description>
	<SchemeComplexColumn IsSystem="true" IsPermanent="true" IsSealed="true" ID="01db90f2-22ec-0033-2000-087a832b1b48" Name="ID" Type="Reference(Typified) Not Null" ReferencedTable="1074eadd-21d7-4925-98c8-40d1e5f0ca0e">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="01db90f2-22ec-0133-4000-087a832b1b48" Name="ID" Type="Guid Not Null" ReferencedColumn="9a58123b-b2e9-4137-9c6c-5dab0ec02747" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn ID="35c4f8a7-02ea-48ac-a056-35426426dad2" Name="Digest" Type="String(Max) Null">
		<Description>Описание задания</Description>
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="f38836f2-167e-44b9-9c38-d212888ce8ea" Name="CanEditCard" Type="Boolean Not Null">
		<Description>Возможность редактировать файлы при наличии настраиваемого задания</Description>
		<SchemeDefaultConstraint IsPermanent="true" ID="d8dada61-30eb-4673-a2b7-e9ee1fd82c39" Name="df_KrUniversalTaskSettingsVirtual_CanEditCard" Value="false" />
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="e852a138-ce49-4288-99c7-45e5d1526afa" Name="CanEditFiles" Type="Boolean Not Null">
		<Description>Возможность редактировать файлы при наличии настраиваемого задания</Description>
		<SchemeDefaultConstraint IsPermanent="true" ID="69a25b9d-dac1-45a3-9e28-caf53663fa18" Name="df_KrUniversalTaskSettingsVirtual_CanEditFiles" Value="false" />
	</SchemePhysicalColumn>
	<SchemeComplexColumn ID="61895891-82bf-41dc-96b8-0851f224a96c" Name="Kind" Type="Reference(Typified) Null" ReferencedTable="856068b1-0e78-4aa8-8e7a-4f53d91a7298" WithForeignKey="false">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="61895891-82bf-00dc-4000-0851f224a96c" Name="KindID" Type="Guid Null" ReferencedColumn="856068b1-0e78-01a8-4000-0f53d91a7298" />
		<SchemeReferencingColumn ID="41a7832f-1ee8-4bc5-83e8-5ddb7cdf9a89" Name="KindCaption" Type="String(128) Null" ReferencedColumn="63d9110b-7628-4bf9-9dae-750c3035e48d" />
	</SchemeComplexColumn>
	<SchemePrimaryKey IsSystem="true" IsPermanent="true" IsSealed="true" ID="01db90f2-22ec-0033-5000-087a832b1b48" Name="pk_KrUniversalTaskSettingsVirtual" IsClustered="true">
		<SchemeIndexedColumn Column="01db90f2-22ec-0133-4000-087a832b1b48" />
	</SchemePrimaryKey>
</SchemeTable>