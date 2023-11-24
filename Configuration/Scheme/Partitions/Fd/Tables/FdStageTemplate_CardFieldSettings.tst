<?xml version="1.0" encoding="utf-8"?>
<SchemeTable Partition="c3b14969-a553-4005-9a07-e9705c15b1e1" ID="74bc5189-b634-41db-89b7-0f453e41e69c" Name="FdStageTemplate_CardFieldSettings" Group="Fd Fields" InstanceType="Cards" ContentType="Collections">
	<Description>Какие поля карточки может редактировать исполнитель и какие из них обязательные для заполнения</Description>
	<SchemeComplexColumn IsSystem="true" IsPermanent="true" IsSealed="true" ID="74bc5189-b634-00db-2000-0f453e41e69c" Name="ID" Type="Reference(Typified) Not Null" ReferencedTable="1074eadd-21d7-4925-98c8-40d1e5f0ca0e">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="74bc5189-b634-01db-4000-0f453e41e69c" Name="ID" Type="Guid Not Null" ReferencedColumn="9a58123b-b2e9-4137-9c6c-5dab0ec02747" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn IsSystem="true" IsPermanent="true" IsSealed="true" ID="74bc5189-b634-00db-3100-0f453e41e69c" Name="RowID" Type="Guid Not Null" />
	<SchemeComplexColumn ID="5e552b8f-b25a-4840-9f73-c7cc688f3fd4" Name="CardField" Type="Reference(Typified) Not Null" ReferencedTable="9a51283d-02ff-4a86-9d88-144addad3805">
		<Description>Поле карточки</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="5e552b8f-b25a-0040-4000-07cc688f3fd4" Name="CardFieldID" Type="Guid Not Null" ReferencedColumn="9a51283d-02ff-0186-4000-044addad3805" />
		<SchemeReferencingColumn ID="f1b597bc-da68-4565-9758-cb56f1a9e368" Name="CardFieldCaption" Type="String(128) Not Null" ReferencedColumn="09b3454b-d9d3-4c6b-9ebf-12cfc1d911d5" />
		<SchemeReferencingColumn ID="a15efcdb-2ec2-4c5c-b859-f33596bd5af6" Name="CardFieldDataTypeID" Type="Guid Not Null" ReferencedColumn="cb758997-e875-489a-9854-89398204eb1e" />
		<SchemeReferencingColumn ID="0c005c54-5264-4fc1-8dd0-94014175dc4c" Name="CardFieldDataTypeCaption" Type="String(128) Not Null" ReferencedColumn="87f02e9b-5552-433b-8862-35dcd83de844" />
		<SchemeReferencingColumn ID="aa29b011-33dc-497f-91a8-8611c766d0ad" Name="CardFieldSectionName" Type="String(128) Not Null" ReferencedColumn="65d67953-e0ef-4c96-9a70-cbfc42518985" />
		<SchemeReferencingColumn ID="04797010-7b9f-48c7-8566-764826ef8492" Name="CardFieldPhysicalColumnName" Type="String(128) Null" ReferencedColumn="16570f16-6fb3-4b9f-926d-2645823096fe" />
		<SchemeReferencingColumn ID="e1a29fbd-750c-4ef1-ac56-38af09208fe4" Name="CardFieldComplexColumnName" Type="String(128) Null" ReferencedColumn="30af168c-855d-467c-b867-4f324c68c760" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn ID="4ff7f134-6eef-45bc-a4a2-57a73888ec3e" Name="IsReadOnly" Type="Boolean Not Null">
		<Description>Является ли поле доступным только для чтения</Description>
		<SchemeDefaultConstraint IsPermanent="true" ID="b8a354ae-48b4-497b-a031-43272aa0390d" Name="df_FdStageTemplate_CardFieldSettings_IsReadOnly" Value="false" />
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="2117421d-6a83-41ae-8b2c-15f4139d0b86" Name="IsRequired" Type="Boolean Not Null">
		<Description>Является ли поле обязательным для заполнения</Description>
		<SchemeDefaultConstraint IsPermanent="true" ID="f9fb9431-f5a1-4800-b6c8-a352bc0bf855" Name="df_FdStageTemplate_CardFieldSettings_IsRequired" Value="false" />
	</SchemePhysicalColumn>
	<SchemeComplexColumn ID="6043e082-b539-40fa-ac5a-05fcfc32216e" Name="CompletionOption" Type="Reference(Typified) Null" ReferencedTable="08cf782d-4130-4377-8a49-3e201a05d496">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="6043e082-b539-00fa-4000-05fcfc32216e" Name="CompletionOptionID" Type="Guid Null" ReferencedColumn="132dc5f5-ce87-4dd0-acce-b4a02acf7715" />
		<SchemeReferencingColumn ID="6a1cfd40-51ac-404f-806b-de8465f8a0a9" Name="CompletionOptionCaption" Type="String(128) Null" ReferencedColumn="6762309a-b0ff-4b2f-9cce-dd111116e554" />
	</SchemeComplexColumn>
	<SchemePrimaryKey IsSystem="true" IsPermanent="true" IsSealed="true" ID="74bc5189-b634-00db-5000-0f453e41e69c" Name="pk_FdStageTemplate_CardFieldSettings">
		<SchemeIndexedColumn Column="74bc5189-b634-00db-3100-0f453e41e69c" />
	</SchemePrimaryKey>
	<SchemeIndex IsSystem="true" IsPermanent="true" IsSealed="true" ID="74bc5189-b634-00db-7000-0f453e41e69c" Name="idx_FdStageTemplate_CardFieldSettings_ID" IsClustered="true">
		<SchemeIndexedColumn Column="74bc5189-b634-01db-4000-0f453e41e69c" />
	</SchemeIndex>
</SchemeTable>