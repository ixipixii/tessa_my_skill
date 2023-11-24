<?xml version="1.0" encoding="utf-8"?>
<SchemeTable Partition="c3b14969-a553-4005-9a07-e9705c15b1e1" ID="14e83033-151c-4e29-942c-98e6c956c75b" Name="FdCompletedTasksVirtual" Group="Fd" IsVirtual="true" InstanceType="Cards" ContentType="Collections">
	<SchemeComplexColumn IsSystem="true" IsPermanent="true" IsSealed="true" ID="14e83033-151c-0029-2000-08e6c956c75b" Name="ID" Type="Reference(Typified) Not Null" ReferencedTable="1074eadd-21d7-4925-98c8-40d1e5f0ca0e">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="14e83033-151c-0129-4000-08e6c956c75b" Name="ID" Type="Guid Not Null" ReferencedColumn="9a58123b-b2e9-4137-9c6c-5dab0ec02747" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn IsSystem="true" IsPermanent="true" IsSealed="true" ID="14e83033-151c-0029-3100-08e6c956c75b" Name="RowID" Type="Guid Not Null" />
	<SchemeComplexColumn ID="6fd7e1d3-8938-47d4-9a8b-9eaf55720549" Name="Process" Type="Reference(Typified) Not Null" ReferencedTable="2114510a-e165-4491-afcc-1756400e30a0">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="6fd7e1d3-8938-00d4-4000-0eaf55720549" Name="ProcessRowID" Type="Guid Not Null" ReferencedColumn="2114510a-e165-0091-3100-0756400e30a0" />
	</SchemeComplexColumn>
	<SchemeComplexColumn ID="167419e2-ce42-4d82-a625-ed7367223b27" Name="Stage" Type="Reference(Typified) Not Null" ReferencedTable="ffced5f3-fea9-41e1-bcc6-b156b3e3c054">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="167419e2-ce42-0082-4000-0d7367223b27" Name="StageRowID" Type="Guid Not Null" ReferencedColumn="ffced5f3-fea9-00e1-3100-0156b3e3c054" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn ID="c15266e2-1bdc-43b3-aa30-087fd911121a" Name="TaskID" Type="Guid Not Null" />
	<SchemeComplexColumn ID="049e3513-986a-4e55-ab41-c137ad84f70c" Name="Option" Type="Reference(Typified) Not Null" ReferencedTable="08cf782d-4130-4377-8a49-3e201a05d496">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="049e3513-986a-0055-4000-0137ad84f70c" Name="OptionID" Type="Guid Not Null" ReferencedColumn="132dc5f5-ce87-4dd0-acce-b4a02acf7715" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn ID="32a81605-0a6c-480b-afd3-731fdf6cdd67" Name="IsMain" Type="Boolean Not Null">
		<SchemeDefaultConstraint IsPermanent="true" ID="b967a685-6010-4a07-8eb9-59fe62c00152" Name="df_FdCompletedTasksVirtual_IsMain" Value="true" />
	</SchemePhysicalColumn>
	<SchemePrimaryKey IsSystem="true" IsPermanent="true" IsSealed="true" ID="14e83033-151c-0029-5000-08e6c956c75b" Name="pk_FdCompletedTasksVirtual">
		<SchemeIndexedColumn Column="14e83033-151c-0029-3100-08e6c956c75b" />
	</SchemePrimaryKey>
	<SchemeIndex IsSystem="true" IsPermanent="true" IsSealed="true" ID="14e83033-151c-0029-7000-08e6c956c75b" Name="idx_FdCompletedTasksVirtual_ID" IsClustered="true">
		<SchemeIndexedColumn Column="14e83033-151c-0129-4000-08e6c956c75b" />
	</SchemeIndex>
</SchemeTable>