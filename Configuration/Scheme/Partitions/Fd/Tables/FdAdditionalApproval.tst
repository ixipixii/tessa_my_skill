<?xml version="1.0" encoding="utf-8"?>
<SchemeTable Partition="c3b14969-a553-4005-9a07-e9705c15b1e1" ID="0dbab0cf-9bdf-4e45-b1b3-3436aa5df7a0" Name="FdAdditionalApproval" Group="Fd" InstanceType="Tasks" ContentType="Entries">
	<SchemeComplexColumn IsSystem="true" IsPermanent="true" IsSealed="true" ID="0dbab0cf-9bdf-0045-2000-0436aa5df7a0" Name="ID" Type="Reference(Typified) Not Null" ReferencedTable="5bfa9936-bb5a-4e8f-89a9-180bfd8f75f8">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="0dbab0cf-9bdf-0145-4000-0436aa5df7a0" Name="ID" Type="Guid Not Null" ReferencedColumn="5bfa9936-bb5a-008f-3100-080bfd8f75f8" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn ID="4f7bc4af-6448-49a4-84f5-110b14b18ead" Name="TimeLimitation" Type="Double Null">
		<SchemeDefaultConstraint IsPermanent="true" ID="9f1ae3b2-8c42-486e-b147-b98024a04e7b" Name="df_FdAdditionalApproval_TimeLimitation" Value="1" />
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="6010f718-5272-4191-b9a4-bc84592154ed" Name="FirstIsResponsible" Type="Boolean Not Null">
		<SchemeDefaultConstraint IsPermanent="true" ID="1aba07d5-2c98-4142-a608-54866305ebf0" Name="df_FdAdditionalApproval_FirstIsResponsible" Value="false" />
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="3ff7c7a5-4e3d-4d4b-8a68-9890d8bab1fb" Name="Comment" Type="String(Max) Null" />
	<SchemePhysicalColumn ID="12cd0144-1a22-4e55-955a-d98b19bc2c3e" Name="Planned" Type="DateTime Null">
		<Description>Планируемый срок выполнения подзадач</Description>
	</SchemePhysicalColumn>
	<SchemePrimaryKey IsSystem="true" IsPermanent="true" IsSealed="true" ID="0dbab0cf-9bdf-0045-5000-0436aa5df7a0" Name="pk_FdAdditionalApproval" IsClustered="true">
		<SchemeIndexedColumn Column="0dbab0cf-9bdf-0145-4000-0436aa5df7a0" />
	</SchemePrimaryKey>
</SchemeTable>