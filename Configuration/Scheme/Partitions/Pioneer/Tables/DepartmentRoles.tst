<?xml version="1.0" encoding="utf-8"?>
<SchemeTable ID="d43dace1-536f-4c9f-af15-49a8892a7427" Partition="29f90c69-c1ef-4cbf-b9d5-7fc91cd68c67">
	<SchemeComplexColumn Partition="54ed79f5-11c0-4f74-b8b0-793d596699d6" ID="72461d9c-fac7-45a0-9317-a0cd518047e6" Name="DepartmentType" Type="Reference(Typified) Null" ReferencedTable="ae32aeda-08ed-44f7-8ac4-815e137bcf9e">
		<Description>Тип подразделения</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="72461d9c-fac7-00a0-4000-00cd518047e6" Name="DepartmentTypeID" Type="Int16 Null" ReferencedColumn="9edb45c5-b36f-448f-9930-94c5d4b94f20" />
		<SchemeReferencingColumn ID="8ee0afe0-507a-46cb-b262-51d28c35d7dc" Name="DepartmentTypeName" Type="String(Max) Null" ReferencedColumn="edaf212a-fb92-44c7-9898-f216cdf09d8d" />
	</SchemeComplexColumn>
	<SchemeComplexColumn Partition="54ed79f5-11c0-4f74-b8b0-793d596699d6" ID="3bf052ed-eebd-49af-915e-2b5e2d2517cd" Name="CFO" Type="Reference(Typified) Null" ReferencedTable="b5e873a7-4f25-4731-b7bf-93586f07b53a">
		<Description>ЦФО</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="3bf052ed-eebd-00af-4000-0b5e2d2517cd" Name="CFOID" Type="Guid Null" ReferencedColumn="b5e873a7-4f25-0131-4000-03586f07b53a" />
		<SchemeReferencingColumn ID="ff06061d-9207-48ac-976d-aa961d8bc854" Name="CFOName" Type="String(Max) Null" ReferencedColumn="20d4f2eb-ce34-4c44-87b8-8b386c283930" />
	</SchemeComplexColumn>
	<SchemeComplexColumn Partition="54ed79f5-11c0-4f74-b8b0-793d596699d6" ID="853a0f5b-be9e-4e57-98e8-5ddbd7947646" Name="Curator" Type="Reference(Typified) Null" ReferencedTable="6c977939-bbfc-456f-a133-f1c2244e3cc3">
		<Description>Куратор</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="853a0f5b-be9e-0057-4000-0ddbd7947646" Name="CuratorID" Type="Guid Null" ReferencedColumn="6c977939-bbfc-016f-4000-01c2244e3cc3" />
		<SchemeReferencingColumn ID="8323cd71-c8bc-4360-9a62-8e12ceaa6959" Name="CuratorName" Type="String(128) Null" ReferencedColumn="1782f76a-4743-4aa4-920c-7edaee860964" />
	</SchemeComplexColumn>
	<SchemeComplexColumn Partition="54ed79f5-11c0-4f74-b8b0-793d596699d6" ID="d5ce55dc-04dd-4260-8dbe-cc8142d9acdb" Name="ActionStatus" Type="Reference(Typified) Null" ReferencedTable="00262199-e52d-4275-b7af-3925446e3fdd">
		<Description>Статус действия</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="d5ce55dc-04dd-0060-4000-0c8142d9acdb" Name="ActionStatusID" Type="Int16 Null" ReferencedColumn="3e012001-e372-46b0-9029-8d3301313d67" />
		<SchemeReferencingColumn ID="57771916-a22b-4a84-8d7e-c5048b89a507" Name="ActionStatusName" Type="String(Max) Null" ReferencedColumn="421cb3f3-5523-47fd-8704-5d758c385efb" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn Partition="54ed79f5-11c0-4f74-b8b0-793d596699d6" ID="65de71f6-6e28-4e70-b131-ef8d28b38ea8" Name="ExtID" Type="Int32 Null">
		<Description>ID мигрированной карточки</Description>
	</SchemePhysicalColumn>
</SchemeTable>