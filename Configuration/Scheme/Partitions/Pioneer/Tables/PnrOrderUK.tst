<?xml version="1.0" encoding="utf-8"?>
<SchemeTable Partition="54ed79f5-11c0-4f74-b8b0-793d596699d6" ID="11de9a27-5881-4ad9-b3ff-187fcb89f12b" Name="PnrOrderUK" Group="Pnr" InstanceType="Cards" ContentType="Entries">
	<Description>Приказы и распоряжения УК</Description>
	<SchemeComplexColumn IsSystem="true" IsPermanent="true" IsSealed="true" ID="11de9a27-5881-00d9-2000-087fcb89f12b" Name="ID" Type="Reference(Typified) Not Null" ReferencedTable="1074eadd-21d7-4925-98c8-40d1e5f0ca0e">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="11de9a27-5881-01d9-4000-087fcb89f12b" Name="ID" Type="Guid Not Null" ReferencedColumn="9a58123b-b2e9-4137-9c6c-5dab0ec02747" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn ID="a766dd42-e90e-41d1-8d29-d085fc5cf0c3" Name="RegistrationDate" Type="Date Null">
		<Description>Дата регистрации</Description>
	</SchemePhysicalColumn>
	<SchemeComplexColumn ID="35676b00-0ae9-4f4a-b980-567e57effeca" Name="LegalEntityIndex" Type="Reference(Typified) Null" ReferencedTable="00e3d618-aa1e-4bac-9297-3010b710425c">
		<Description>Индекс ЮЛ УК ПС</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="35676b00-0ae9-004a-4000-067e57effeca" Name="LegalEntityIndexID" Type="Guid Null" ReferencedColumn="00e3d618-aa1e-01ac-4000-0010b710425c" />
		<SchemeReferencingColumn ID="e85011ca-b9e6-41a3-81e6-23c29dfc585d" Name="LegalEntityIndexIdx" Type="String(Max) Null" ReferencedColumn="14c84e81-9e95-4cfa-98cf-62c786ba4bfe" />
	</SchemeComplexColumn>
	<SchemeComplexColumn ID="97ed0aac-acf4-44d6-9a01-59583c97107f" Name="Organization" Type="Reference(Typified) Null" ReferencedTable="303dd35d-b998-49b3-a10b-ed30d31bd36a">
		<Description>Организация ГК Пионер</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="97ed0aac-acf4-00d6-4000-09583c97107f" Name="OrganizationID" Type="Guid Null" ReferencedColumn="303dd35d-b998-01b3-4000-0d30d31bd36a" />
		<SchemeReferencingColumn ID="87693908-f2a0-44f8-81f3-14a8993bc1e2" Name="OrganizationName" Type="String(Max) Null" ReferencedColumn="d8ecabaa-f192-4546-9af6-1f9712daabdb" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn ID="9e09badf-8565-439f-b38b-925ed69648e7" Name="Comments" Type="String(Max) Null">
		<Description>Комментарий</Description>
	</SchemePhysicalColumn>
	<SchemeComplexColumn ID="94f615ec-db13-41a8-8910-d8ef4fb421d4" Name="City" Type="Reference(Typified) Null" ReferencedTable="75aa0d06-9bff-4e01-96b6-2ca62af269ae">
		<Description>Город</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="94f615ec-db13-00a8-4000-08ef4fb421d4" Name="CityID" Type="Guid Null" ReferencedColumn="75aa0d06-9bff-0101-4000-0ca62af269ae" />
		<SchemeReferencingColumn ID="9fd26c84-130e-4872-a94d-6aba6685b7e5" Name="CityName" Type="String(Max) Null" ReferencedColumn="e70abb74-73d9-4232-ac52-a8ff12c357f5" />
	</SchemeComplexColumn>
	<SchemeComplexColumn ID="3f095a24-1991-43ef-83cc-61f2f479d1b5" Name="Department" Type="Reference(Typified) Null" ReferencedTable="81f6010b-9641-4aa5-8897-b8e8603fbf4b">
		<Description>Подразделение</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="3f095a24-1991-00ef-4000-01f2f479d1b5" Name="DepartmentID" Type="Guid Null" ReferencedColumn="81f6010b-9641-01a5-4000-08e8603fbf4b" />
		<SchemeReferencingColumn ID="525e96b3-44ca-4de2-ab13-fe205add0d26" Name="DepartmentName" Type="String(128) Null" ReferencedColumn="616d6b2e-35d5-424d-846b-618eb25962d0" />
		<SchemeReferencingColumn ID="47b63392-ba26-4629-b4dd-d436f51bd5a8" Name="DepartmentIdx" Type="String(Max) Null" ReferencedColumn="1f7ca0d4-1b64-43f8-b973-d14791af51a0" />
	</SchemeComplexColumn>
	<SchemeComplexColumn ID="71918bfa-a47b-472d-a1ff-0f2a5f4ab75d" Name="Approver" Type="Reference(Typified) Null" ReferencedTable="6c977939-bbfc-456f-a133-f1c2244e3cc3">
		<Description>Утверждающий</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="71918bfa-a47b-002d-4000-0f2a5f4ab75d" Name="ApproverID" Type="Guid Null" ReferencedColumn="6c977939-bbfc-016f-4000-01c2244e3cc3" />
		<SchemeReferencingColumn ID="bf9f52e1-aec6-4e4d-8ef5-d6b53921daea" Name="ApproverName" Type="String(128) Null" ReferencedColumn="1782f76a-4743-4aa4-920c-7edaee860964" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn ID="8b04821d-e71f-4165-9400-8d8c76ceaeb4" Name="ExtID" Type="Guid Null">
		<Description>GUID карточки из системы заказчика</Description>
	</SchemePhysicalColumn>
	<SchemePrimaryKey IsSystem="true" IsPermanent="true" IsSealed="true" ID="11de9a27-5881-00d9-5000-087fcb89f12b" Name="pk_PnrOrderUK" IsClustered="true">
		<SchemeIndexedColumn Column="11de9a27-5881-01d9-4000-087fcb89f12b" />
	</SchemePrimaryKey>
</SchemeTable>