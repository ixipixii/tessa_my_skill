<?xml version="1.0" encoding="utf-8"?>
<SchemeTable ID="10214795-ee2a-4868-98f3-09c237e42492" Name="FmTopicRoleParticipantsInfoVirtual" Group="Fm" IsVirtual="true" InstanceType="Cards" ContentType="Entries">
	<Description>Виртуальные секции для формы заполнения ролями - участниками топика</Description>
	<SchemeComplexColumn IsSystem="true" IsPermanent="true" IsSealed="true" ID="10214795-ee2a-0068-2000-09c237e42492" Name="ID" Type="Reference(Typified) Not Null" ReferencedTable="1074eadd-21d7-4925-98c8-40d1e5f0ca0e">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="10214795-ee2a-0168-4000-09c237e42492" Name="ID" Type="Guid Not Null" ReferencedColumn="9a58123b-b2e9-4137-9c6c-5dab0ec02747" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn ID="90412202-a7b4-473d-9d8b-c7defe58f737" Name="ReadOnly" Type="Boolean Not Null">
		<SchemeDefaultConstraint IsPermanent="true" ID="570f54ac-1d4e-4f8f-8d83-da28986e7b2a" Name="df_FmTopicRoleParticipantsInfoVirtual_ReadOnly" Value="false" />
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="d0411db0-3475-4642-966c-8cc31ae4e934" Name="IsSubscribe" Type="Boolean Not Null">
		<SchemeDefaultConstraint IsPermanent="true" ID="f5198981-6f55-4b4c-a4df-2dfb57b8e753" Name="df_FmTopicRoleParticipantsInfoVirtual_IsSubscribe" Value="false" />
	</SchemePhysicalColumn>
	<SchemePrimaryKey IsSystem="true" IsPermanent="true" IsSealed="true" ID="10214795-ee2a-0068-5000-09c237e42492" Name="pk_FmTopicRoleParticipantsInfoVirtual" IsClustered="true">
		<SchemeIndexedColumn Column="10214795-ee2a-0168-4000-09c237e42492" />
	</SchemePrimaryKey>
</SchemeTable>