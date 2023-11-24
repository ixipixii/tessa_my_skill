<?xml version="1.0" encoding="utf-8"?>
<SchemeTable Partition="c3b14969-a553-4005-9a07-e9705c15b1e1" ID="ffced5f3-fea9-41e1-bcc6-b156b3e3c054" Name="FdStageInstances" Group="Fd" InstanceType="Cards" ContentType="Collections">
	<Description>Экземпляры этапов процесса</Description>
	<SchemeComplexColumn IsSystem="true" IsPermanent="true" IsSealed="true" ID="ffced5f3-fea9-00e1-2000-0156b3e3c054" Name="ID" Type="Reference(Typified) Not Null" ReferencedTable="1074eadd-21d7-4925-98c8-40d1e5f0ca0e">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="ffced5f3-fea9-01e1-4000-0156b3e3c054" Name="ID" Type="Guid Not Null" ReferencedColumn="9a58123b-b2e9-4137-9c6c-5dab0ec02747" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn IsSystem="true" IsPermanent="true" IsSealed="true" ID="ffced5f3-fea9-00e1-3100-0156b3e3c054" Name="RowID" Type="Guid Not Null" />
	<SchemeComplexColumn ID="93550178-f394-4e85-9986-8d3b6b6b3d64" Name="ProcessInstance" Type="Reference(Typified) Not Null" ReferencedTable="2114510a-e165-4491-afcc-1756400e30a0" DeleteReferentialAction="Cascade">
		<Description>Экземпляр процесса, к которому относится этап</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="93550178-f394-0085-4000-0d3b6b6b3d64" Name="ProcessInstanceRowID" Type="Guid Not Null" ReferencedColumn="2114510a-e165-0091-3100-0756400e30a0" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn ID="a3683ea2-ba85-45a1-84a9-eb803f8ea9ed" Name="Name" Type="String(255) Not Null">
		<Description>Название этапа</Description>
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="2f933ec5-23da-4fc9-bd63-de34758b81e1" Name="Order" Type="Int32 Not Null">
		<Description>Порядковый номер</Description>
		<SchemeDefaultConstraint IsPermanent="true" ID="c9ab9c35-dfec-4ac1-b8ed-a870cedec317" Name="df_FdStageInstances_Order" Value="0" />
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="01e7a446-e92d-4799-a7d7-f0048fe1c1bb" Name="IsParallel" Type="Boolean Not Null">
		<Description>Является ли этап параллельным</Description>
		<SchemeDefaultConstraint IsPermanent="true" ID="18d48643-26e1-4cd3-a857-fe7ca0e01cf1" Name="df_FdStageInstances_IsParallel" Value="false" />
	</SchemePhysicalColumn>
	<SchemeComplexColumn ID="44279533-5501-48d6-bb42-829cffc43d00" Name="BasedOnStageTemplate" Type="Reference(Typified) Not Null" ReferencedTable="323be76d-516a-4ee9-b6b0-e76391d70426" WithForeignKey="false">
		<Description>Шаблон этапа, к которому относится экземпляр</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="44279533-5501-00d6-4000-029cffc43d00" Name="BasedOnStageTemplateID" Type="Guid Not Null" ReferencedColumn="323be76d-516a-01e9-4000-076391d70426" />
		<SchemeReferencingColumn ID="241cafda-5363-4eb6-b3de-f5167da81b64" Name="BasedOnStageTemplateName" Type="String(255) Not Null" ReferencedColumn="7bc776b2-ac1d-4c81-b4b3-c52ecf5b1937" />
	</SchemeComplexColumn>
	<SchemeComplexColumn ID="0e549359-f584-4d8d-8a98-517a7f90117c" Name="State" Type="Reference(Typified) Not Null" ReferencedTable="7298c4f3-6c27-439e-a417-bafa3967e40e">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="0e549359-f584-008d-4000-017a7f90117c" Name="StateID" Type="Int16 Not Null" ReferencedColumn="a9399756-feeb-4507-8b42-f66517400134" />
		<SchemeReferencingColumn ID="03ec8451-cc90-4031-a2b7-41fccfac6c28" Name="StateName" Type="String(128) Not Null" ReferencedColumn="31ac86d7-2665-457a-996e-9aee07dc0030" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn ID="8e6b8ad7-a5e4-4443-8f5b-328775cac16f" Name="StartDate" Type="DateTime Not Null">
		<Description>Дата запуска процесса</Description>
	</SchemePhysicalColumn>
	<SchemeComplexColumn ID="14194b64-98da-4930-bb0e-018b9d080d1e" Name="TaskType" Type="Reference(Typified) Null" ReferencedTable="b0538ece-8468-4d0b-8b4e-5a1d43e024db">
		<Description>Тип создаваемого задания в рамках этапа</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="14194b64-98da-0030-4000-018b9d080d1e" Name="TaskTypeID" Type="Guid Null" ReferencedColumn="a628a864-c858-4200-a6b7-da78c8e6e1f4" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn ID="9244c1f7-85bf-4dad-abe2-4847581a5efe" Name="TaskDigest" Type="String(512) Null" />
	<SchemeComplexColumn ID="688cfd3a-8859-45f9-a491-a44fa491d125" Name="CompletionResult" Type="Reference(Typified) Null" ReferencedTable="2b1781ba-09e1-42a0-9969-fe4caa9abb81" WithForeignKey="false">
		<Description>Результат завершения из шаблона этапа, который был выбран при завершении этапа</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="688cfd3a-8859-00f9-4000-044fa491d125" Name="CompletionResultRowID" Type="Guid Null" ReferencedColumn="2b1781ba-09e1-00a0-3100-0e4caa9abb81" />
		<SchemeReferencingColumn ID="a08c477c-422d-458c-8704-2cbedeeb0b7a" Name="CompletionResultCompletionText" Type="String(Max) Null" ReferencedColumn="97a09aa3-c465-4284-b474-8df1823c4774" />
	</SchemeComplexColumn>
	<SchemeComplexColumn ID="1b518e18-3ac8-47a0-8ce9-87864012fa5f" Name="ParentStageInstance" Type="Reference(Typified) Null" ReferencedTable="ffced5f3-fea9-41e1-bcc6-b156b3e3c054" WithForeignKey="false">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="1b518e18-3ac8-00a0-4000-07864012fa5f" Name="ParentStageInstanceRowID" Type="Guid Null" ReferencedColumn="ffced5f3-fea9-00e1-3100-0156b3e3c054" />
		<SchemeReferencingColumn ID="2c9c7bc2-fff8-4753-8387-d7bfa0130ff6" Name="ParentStageInstanceName" Type="String(255) Null" ReferencedColumn="a3683ea2-ba85-45a1-84a9-eb803f8ea9ed" />
	</SchemeComplexColumn>
	<SchemePrimaryKey IsSystem="true" IsPermanent="true" IsSealed="true" ID="ffced5f3-fea9-00e1-5000-0156b3e3c054" Name="pk_FdStageInstances">
		<SchemeIndexedColumn Column="ffced5f3-fea9-00e1-3100-0156b3e3c054" />
	</SchemePrimaryKey>
	<SchemeIndex IsSystem="true" IsPermanent="true" IsSealed="true" ID="ffced5f3-fea9-00e1-7000-0156b3e3c054" Name="idx_FdStageInstances_ID" IsClustered="true">
		<SchemeIndexedColumn Column="ffced5f3-fea9-01e1-4000-0156b3e3c054" />
	</SchemeIndex>
</SchemeTable>