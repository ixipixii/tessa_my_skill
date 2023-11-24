<?xml version="1.0" encoding="utf-8"?>
<SchemeTable Partition="54ed79f5-11c0-4f74-b8b0-793d596699d6" ID="04c172c3-8fcd-469d-8442-77e028d7016f" Name="PnrErrandsControllers" Group="Pnr" InstanceType="Cards" ContentType="Collections">
	<SchemeComplexColumn IsSystem="true" IsPermanent="true" IsSealed="true" ID="04c172c3-8fcd-009d-2000-07e028d7016f" Name="ID" Type="Reference(Typified) Not Null" ReferencedTable="1074eadd-21d7-4925-98c8-40d1e5f0ca0e">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="04c172c3-8fcd-019d-4000-07e028d7016f" Name="ID" Type="Guid Not Null" ReferencedColumn="9a58123b-b2e9-4137-9c6c-5dab0ec02747" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn IsSystem="true" IsPermanent="true" IsSealed="true" ID="04c172c3-8fcd-009d-3100-07e028d7016f" Name="RowID" Type="Guid Not Null" />
	<SchemeComplexColumn ID="255de9bc-14ee-4ce2-aba2-3897c4e2379f" Name="Controllers" Type="Reference(Typified) Not Null" ReferencedTable="6c977939-bbfc-456f-a133-f1c2244e3cc3">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="255de9bc-14ee-00e2-4000-0897c4e2379f" Name="ControllersID" Type="Guid Not Null" ReferencedColumn="6c977939-bbfc-016f-4000-01c2244e3cc3" />
		<SchemeReferencingColumn ID="840946ee-28ed-4e8c-95ba-bd4a118c8132" Name="ControllersName" Type="String(128) Not Null" ReferencedColumn="1782f76a-4743-4aa4-920c-7edaee860964" />
	</SchemeComplexColumn>
	<SchemePrimaryKey IsSystem="true" IsPermanent="true" IsSealed="true" ID="04c172c3-8fcd-009d-5000-07e028d7016f" Name="pk_PnrErrandsControllers">
		<SchemeIndexedColumn Column="04c172c3-8fcd-009d-3100-07e028d7016f" />
	</SchemePrimaryKey>
	<SchemeIndex IsSystem="true" IsPermanent="true" IsSealed="true" ID="04c172c3-8fcd-009d-7000-07e028d7016f" Name="idx_PnrErrandsControllers_ID" IsClustered="true">
		<SchemeIndexedColumn Column="04c172c3-8fcd-019d-4000-07e028d7016f" />
	</SchemeIndex>
</SchemeTable>