<?xml version="1.0" encoding="utf-8"?>
<SchemeTable Partition="c3b14969-a553-4005-9a07-e9705c15b1e1" ID="65bb59e8-3f1a-4a4a-8a2c-f6c924546bbc" Name="FdStageTemplate_HiddenTaskOptions" Group="Fd" InstanceType="Cards" ContentType="Collections">
	<SchemeComplexColumn IsSystem="true" IsPermanent="true" IsSealed="true" ID="65bb59e8-3f1a-004a-2000-06c924546bbc" Name="ID" Type="Reference(Typified) Not Null" ReferencedTable="1074eadd-21d7-4925-98c8-40d1e5f0ca0e">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="65bb59e8-3f1a-014a-4000-06c924546bbc" Name="ID" Type="Guid Not Null" ReferencedColumn="9a58123b-b2e9-4137-9c6c-5dab0ec02747" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn IsSystem="true" IsPermanent="true" IsSealed="true" ID="65bb59e8-3f1a-004a-3100-06c924546bbc" Name="RowID" Type="Guid Not Null" />
	<SchemeComplexColumn ID="971657f8-7bed-4459-8499-13b22b427898" Name="Option" Type="Reference(Typified) Not Null" ReferencedTable="08cf782d-4130-4377-8a49-3e201a05d496" WithForeignKey="false">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="971657f8-7bed-0059-4000-03b22b427898" Name="OptionID" Type="Guid Not Null" ReferencedColumn="132dc5f5-ce87-4dd0-acce-b4a02acf7715" />
		<SchemeReferencingColumn ID="f3482da7-89d3-44c9-b528-4d7337cd8fb4" Name="OptionName" Type="String(128) Not Null" ReferencedColumn="aa6a7122-8384-4c81-9553-386f2c05e96c" />
		<SchemeReferencingColumn ID="a8342c84-fc61-441f-85bd-0fba03dfc855" Name="OptionCaption" Type="String(128) Not Null" ReferencedColumn="6762309a-b0ff-4b2f-9cce-dd111116e554" />
	</SchemeComplexColumn>
	<SchemeComplexColumn ID="ff36b6e4-1667-4f19-b33e-41aded6b29f0" Name="Mode" Type="Reference(Typified) Not Null" ReferencedTable="09bfb67e-ba6a-49a6-a823-d7dbe99a83a2">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="ff36b6e4-1667-0019-4000-01aded6b29f0" Name="ModeID" Type="Int32 Not Null" ReferencedColumn="15faae15-99f1-486a-8cc4-e149a6c54e6e">
			<SchemeDefaultConstraint IsPermanent="true" ID="191d115b-7e36-4e21-a8d5-b30523cae080" Name="df_FdStageTemplate_HiddenTaskOptions_ModeID" Value="0" />
		</SchemeReferencingColumn>
		<SchemeReferencingColumn ID="545d5263-4416-4ca0-bd60-eb3f2f3e5498" Name="ModeName" Type="String(128) Not Null" ReferencedColumn="b5132bad-3ec7-42f5-b5c4-c6750ce5717f">
			<SchemeDefaultConstraint IsPermanent="true" ID="d921d80e-65fd-40da-83f5-bb0225ede9bb" Name="df_FdStageTemplate_HiddenTaskOptions_ModeName" Value="Скрывать для указанных ролей" />
		</SchemeReferencingColumn>
	</SchemeComplexColumn>
	<SchemePrimaryKey IsSystem="true" IsPermanent="true" IsSealed="true" ID="65bb59e8-3f1a-004a-5000-06c924546bbc" Name="pk_FdStageTemplate_HiddenTaskOptions">
		<SchemeIndexedColumn Column="65bb59e8-3f1a-004a-3100-06c924546bbc" />
	</SchemePrimaryKey>
	<SchemeIndex IsSystem="true" IsPermanent="true" IsSealed="true" ID="65bb59e8-3f1a-004a-7000-06c924546bbc" Name="idx_FdStageTemplate_HiddenTaskOptions_ID" IsClustered="true">
		<SchemeIndexedColumn Column="65bb59e8-3f1a-014a-4000-06c924546bbc" />
	</SchemeIndex>
</SchemeTable>