<?xml version="1.0" encoding="utf-8"?>
<SchemeTable ID="6af4bc6f-4853-4134-b8af-33fa05209ad1" Name="FmTopicParticipantsInfoVirtual" Group="Fm" IsVirtual="true" InstanceType="Cards" ContentType="Entries">
	<Description>Виртуальные секции для формы заполнения информции по участникам топика</Description>
	<SchemeComplexColumn IsSystem="true" IsPermanent="true" IsSealed="true" ID="6af4bc6f-4853-0034-2000-03fa05209ad1" Name="ID" Type="Reference(Typified) Not Null" ReferencedTable="1074eadd-21d7-4925-98c8-40d1e5f0ca0e">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="6af4bc6f-4853-0134-4000-03fa05209ad1" Name="ID" Type="Guid Not Null" ReferencedColumn="9a58123b-b2e9-4137-9c6c-5dab0ec02747" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn ID="c33aec76-dd27-4c31-8711-341cd51d0638" Name="ReadOnly" Type="Boolean Not Null">
		<SchemeDefaultConstraint IsPermanent="true" ID="16a77387-d62b-479b-ba4c-3364b2e2eaab" Name="df_FmTopicParticipantsInfoVirtual_ReadOnly" Value="false" />
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="c94f9359-c108-484d-9b98-8593631f6452" Name="IsModerator" Type="Boolean Not Null">
		<SchemeDefaultConstraint IsPermanent="true" ID="9beba5fa-dfc5-4a11-8693-eac9173344a2" Name="df_FmTopicParticipantsInfoVirtual_IsModerator" Value="false" />
	</SchemePhysicalColumn>
	<SchemePrimaryKey IsSystem="true" IsPermanent="true" IsSealed="true" ID="6af4bc6f-4853-0034-5000-03fa05209ad1" Name="pk_FmTopicParticipantsInfoVirtual" IsClustered="true">
		<SchemeIndexedColumn Column="6af4bc6f-4853-0134-4000-03fa05209ad1" />
	</SchemePrimaryKey>
</SchemeTable>