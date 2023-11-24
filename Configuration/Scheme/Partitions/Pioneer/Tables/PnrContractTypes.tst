<?xml version="1.0" encoding="utf-8"?>
<SchemeTable Partition="54ed79f5-11c0-4f74-b8b0-793d596699d6" ID="e41cd076-fa8f-4ee2-857c-2f1ecf257eb7" Name="PnrContractTypes" Group="Pnr" InstanceType="Cards" ContentType="Entries">
	<Description>Справочник Типы договора</Description>
	<SchemeComplexColumn IsSystem="true" IsPermanent="true" IsSealed="true" ID="e41cd076-fa8f-00e2-2000-0f1ecf257eb7" Name="ID" Type="Reference(Typified) Not Null" ReferencedTable="1074eadd-21d7-4925-98c8-40d1e5f0ca0e">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="e41cd076-fa8f-01e2-4000-0f1ecf257eb7" Name="ID" Type="Guid Not Null" ReferencedColumn="9a58123b-b2e9-4137-9c6c-5dab0ec02747" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn ID="6fb304a3-3a0c-4184-a9e1-09342acdee0f" Name="Name" Type="String(Max) Null">
		<Description>Название</Description>
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="2bfeddce-f167-4813-b43b-a54261021352" Name="IsContract" Type="Boolean Null">
		<Description>Тип Договора для карточки Договор</Description>
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="01f607ce-6ff1-4fa8-ba5e-37d7e722d257" Name="IsrSupplementaryAgreement" Type="Boolean Null">
		<Description>Тип Договора для карточки Дополнительное соглашение</Description>
	</SchemePhysicalColumn>
	<SchemePrimaryKey IsSystem="true" IsPermanent="true" IsSealed="true" ID="e41cd076-fa8f-00e2-5000-0f1ecf257eb7" Name="pk_PnrContractTypes" IsClustered="true">
		<SchemeIndexedColumn Column="e41cd076-fa8f-01e2-4000-0f1ecf257eb7" />
	</SchemePrimaryKey>
</SchemeTable>