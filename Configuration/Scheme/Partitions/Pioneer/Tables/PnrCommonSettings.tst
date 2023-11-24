<?xml version="1.0" encoding="utf-8"?>
<SchemeTable Partition="54ed79f5-11c0-4f74-b8b0-793d596699d6" ID="5718dc8a-6b0c-4135-a9c8-e31b824fc779" Name="PnrCommonSettings" Group="Pnr Settings" InstanceType="Cards" ContentType="Entries">
	<Description>Общие настройки Пионер</Description>
	<SchemeComplexColumn IsSystem="true" IsPermanent="true" IsSealed="true" ID="5718dc8a-6b0c-0035-2000-031b824fc779" Name="ID" Type="Reference(Typified) Not Null" ReferencedTable="1074eadd-21d7-4925-98c8-40d1e5f0ca0e">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="5718dc8a-6b0c-0135-4000-031b824fc779" Name="ID" Type="Guid Not Null" ReferencedColumn="9a58123b-b2e9-4137-9c6c-5dab0ec02747" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn ID="fbbbfcbb-d0d6-4a73-a018-3280aa7e9962" Name="MdmServiceUrl" Type="String(Max) Not Null">
		<Description>Адрес сервиса для передачи договора и заявки на КА в MDM (НСИ)</Description>
		<SchemeDefaultConstraint IsPermanent="true" ID="0a77abf0-9c39-4406-a77b-cd5f9ca57980" Name="df_PnrCommonSettings_MdmServiceUrl" Value="test" />
	</SchemePhysicalColumn>
	<SchemePrimaryKey IsSystem="true" IsPermanent="true" IsSealed="true" ID="5718dc8a-6b0c-0035-5000-031b824fc779" Name="pk_PnrCommonSettings" IsClustered="true">
		<SchemeIndexedColumn Column="5718dc8a-6b0c-0135-4000-031b824fc779" />
	</SchemePrimaryKey>
</SchemeTable>