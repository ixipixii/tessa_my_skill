<?xml version="1.0" encoding="utf-8"?>
<SchemeTable Partition="54ed79f5-11c0-4f74-b8b0-793d596699d6" ID="40cd3934-7005-407d-84f6-6e0500874a2a" Name="PnrArchiveKIS" Group="Pnr" InstanceType="Cards" ContentType="Entries">
	<Description>Архив КИС</Description>
	<SchemeComplexColumn IsSystem="true" IsPermanent="true" IsSealed="true" ID="40cd3934-7005-007d-2000-0e0500874a2a" Name="ID" Type="Reference(Typified) Not Null" ReferencedTable="1074eadd-21d7-4925-98c8-40d1e5f0ca0e">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="40cd3934-7005-017d-4000-0e0500874a2a" Name="ID" Type="Guid Not Null" ReferencedColumn="9a58123b-b2e9-4137-9c6c-5dab0ec02747" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn ID="1b7e9c64-09c3-49b8-a8e6-0b31b50c3a21" Name="Number" Type="String(Max) Null">
		<Description>Номер</Description>
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="b6cae0bd-b600-4c1d-ac2f-bc40bfac6c04" Name="Amount" Type="Decimal(20, 2) Null">
		<Description>Сумма</Description>
	</SchemePhysicalColumn>
	<SchemeComplexColumn ID="ef4dbc69-4e65-46ce-aa20-957be7182b35" Name="Organization" Type="Reference(Typified) Null" ReferencedTable="303dd35d-b998-49b3-a10b-ed30d31bd36a">
		<Description>Организация ГК Пионер</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="ef4dbc69-4e65-00ce-4000-057be7182b35" Name="OrganizationID" Type="Guid Null" ReferencedColumn="303dd35d-b998-01b3-4000-0d30d31bd36a" />
		<SchemeReferencingColumn ID="d8aeb914-46a7-4d9b-add4-5eb58a5daf28" Name="OrganizationName" Type="String(Max) Null" ReferencedColumn="d8ecabaa-f192-4546-9af6-1f9712daabdb" />
	</SchemeComplexColumn>
	<SchemeComplexColumn ID="58e1c237-46f3-4041-8de4-538ebfe1da64" Name="Partner" Type="Reference(Typified) Null" ReferencedTable="5d47ef13-b6f4-47ef-9815-3b3d0e6d475a">
		<Description>Контрагент</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="58e1c237-46f3-0041-4000-038ebfe1da64" Name="PartnerID" Type="Guid Null" ReferencedColumn="5d47ef13-b6f4-01ef-4000-0b3d0e6d475a" />
		<SchemeReferencingColumn ID="d4c9af94-aad2-490b-ab65-3729e08426fb" Name="PartnerName" Type="String(255) Null" ReferencedColumn="f1c960e0-951e-4837-8474-bb61d98f40f0" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn ID="4985f465-c09d-41f1-8284-20ccae3fdd2b" Name="ExtID" Type="Guid Null">
		<Description>Уникальный ID записи архива КИС из системы заказчика</Description>
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="275aa104-690a-41ee-953d-4bb5ba29471a" Name="Created" Type="DateTime Null">
		<Description>Дата создания</Description>
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="78710d47-724f-4e8a-aae6-cf021cba84b9" Name="Modified" Type="DateTime Null">
		<Description>Дата последнего изменения</Description>
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="e8254e73-eabd-4bea-b967-eb2503b66f88" Name="AuthorID" Type="Int64 Null">
		<Description>ID автора</Description>
	</SchemePhysicalColumn>
	<SchemeComplexColumn ID="d1f3e8aa-9294-462f-948f-6bfbe12d5e48" Name="AuthorInTessa" Type="Reference(Typified) Null" ReferencedTable="6c977939-bbfc-456f-a133-f1c2244e3cc3">
		<Description>Автор в системе Тесса</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="d1f3e8aa-9294-002f-4000-0bfbe12d5e48" Name="AuthorInTessaID" Type="Guid Null" ReferencedColumn="6c977939-bbfc-016f-4000-01c2244e3cc3" />
		<SchemeReferencingColumn ID="ae2de293-e719-468b-9ebd-46ca3bef1114" Name="AuthorInTessaName" Type="String(128) Null" ReferencedColumn="1782f76a-4743-4aa4-920c-7edaee860964" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn ID="72d41ddb-536d-4327-8160-3dc18a0b46d2" Name="AuthorFio" Type="String(Max) Null" />
	<SchemePhysicalColumn ID="79147cd9-8a54-4f5f-b9ad-eac584e7e383" Name="AuthorLogin" Type="String(Max) Null" />
	<SchemePhysicalColumn ID="bba06d0b-a0a9-4b47-b35e-c59399a7e68d" Name="AuthorEmail" Type="String(Max) Null" />
	<SchemePhysicalColumn ID="282da2cc-b5c2-4eb2-bded-1917e2769bbc" Name="EditorID" Type="Int64 Null">
		<Description>ID редактора</Description>
	</SchemePhysicalColumn>
	<SchemeComplexColumn ID="ac71d1df-5670-41f3-9ad7-a51e81da2e6f" Name="EditorInTessa" Type="Reference(Typified) Null" ReferencedTable="6c977939-bbfc-456f-a133-f1c2244e3cc3">
		<Description>Редактор в системе Тесса</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="ac71d1df-5670-00f3-4000-051e81da2e6f" Name="EditorInTessaID" Type="Guid Null" ReferencedColumn="6c977939-bbfc-016f-4000-01c2244e3cc3" />
		<SchemeReferencingColumn ID="c3ecf0be-d412-4daa-b7b1-c025bee32c9d" Name="EditorInTessaName" Type="String(128) Null" ReferencedColumn="1782f76a-4743-4aa4-920c-7edaee860964" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn ID="50f218da-bc16-423e-97cc-8f2f1d03f7b3" Name="EditorFio" Type="String(Max) Null" />
	<SchemePhysicalColumn ID="7fcff2ba-b457-486f-af23-73b435391f24" Name="EditorLogin" Type="String(Max) Null" />
	<SchemePhysicalColumn ID="28874b17-3add-4bcf-a356-a0ded4885317" Name="EditorEmail" Type="String(Max) Null" />
	<SchemeComplexColumn ID="a80783e8-bec2-4223-8548-afb07bd59d85" Name="Project" Type="Reference(Typified) Null" ReferencedTable="44de4db0-e013-441c-8441-4cb3ef09a649">
		<Description>Проект</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="a80783e8-bec2-0023-4000-0fb07bd59d85" Name="ProjectID" Type="Guid Null" ReferencedColumn="44de4db0-e013-011c-4000-0cb3ef09a649" />
		<SchemeReferencingColumn ID="0729327b-0983-4213-a95c-1225e3cfcfd2" Name="ProjectName" Type="String(Max) Null" ReferencedColumn="6c2fd8b3-e69b-4696-8a8a-2ab2d593af07" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn ID="16a7c934-184f-4ac1-bfa6-fde4cc06eda3" Name="Date" Type="Date Null">
		<Description>Дата</Description>
	</SchemePhysicalColumn>
	<SchemePrimaryKey IsSystem="true" IsPermanent="true" IsSealed="true" ID="40cd3934-7005-007d-5000-0e0500874a2a" Name="pk_PnrArchiveKIS" IsClustered="true">
		<SchemeIndexedColumn Column="40cd3934-7005-017d-4000-0e0500874a2a" />
	</SchemePrimaryKey>
</SchemeTable>