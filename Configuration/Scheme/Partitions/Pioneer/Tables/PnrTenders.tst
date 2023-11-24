<?xml version="1.0" encoding="utf-8"?>
<SchemeTable Partition="54ed79f5-11c0-4f74-b8b0-793d596699d6" ID="45c89001-961e-4c1c-b424-b2821820aebd" Name="PnrTenders" Group="Pnr" InstanceType="Cards" ContentType="Entries">
	<Description>Тендеры</Description>
	<SchemeComplexColumn IsSystem="true" IsPermanent="true" IsSealed="true" ID="45c89001-961e-001c-2000-02821820aebd" Name="ID" Type="Reference(Typified) Not Null" ReferencedTable="1074eadd-21d7-4925-98c8-40d1e5f0ca0e">
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="45c89001-961e-011c-4000-02821820aebd" Name="ID" Type="Guid Not Null" ReferencedColumn="9a58123b-b2e9-4137-9c6c-5dab0ec02747" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn ID="3f509113-a004-4f07-b99f-0e5b5bbc9668" Name="RegistrationNo" Type="String(Max) Null">
		<Description>Рег. №</Description>
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="75371171-69c1-427c-a6c9-269eb8002f59" Name="RegistrationDate" Type="Date Null">
		<Description>Дата регистрации</Description>
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="bc55dd82-57b1-4c6b-be03-d0ee2144d5c0" Name="ProjectDate" Type="Date Null">
		<Description>Дата проекта</Description>
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="5574312e-a237-42be-8edc-b7e6f8ff37e8" Name="ProjectNo" Type="String(Max) Null">
		<Description>Номер проекта</Description>
	</SchemePhysicalColumn>
	<SchemeComplexColumn ID="53423ce8-81a8-442c-9c9e-9591ab1b464c" Name="Project" Type="Reference(Typified) Null" ReferencedTable="44de4db0-e013-441c-8441-4cb3ef09a649">
		<Description>Проект</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="53423ce8-81a8-002c-4000-0591ab1b464c" Name="ProjectID" Type="Guid Null" ReferencedColumn="44de4db0-e013-011c-4000-0cb3ef09a649" />
		<SchemeReferencingColumn ID="b09968aa-cac4-4b9a-a1be-97d4e6641b3f" Name="ProjectName" Type="String(Max) Null" ReferencedColumn="6c2fd8b3-e69b-4696-8a8a-2ab2d593af07" />
	</SchemeComplexColumn>
	<SchemeComplexColumn ID="5400dc34-edf1-4e1f-addc-a6e5d53b5773" Name="CFO" Type="Reference(Typified) Null" ReferencedTable="b5e873a7-4f25-4731-b7bf-93586f07b53a">
		<Description>ЦФО</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="5400dc34-edf1-001f-4000-06e5d53b5773" Name="CFOID" Type="Guid Null" ReferencedColumn="b5e873a7-4f25-0131-4000-03586f07b53a" />
		<SchemeReferencingColumn ID="42fdcc7f-9c84-45eb-9206-d183593ba7b8" Name="CFOName" Type="String(Max) Null" ReferencedColumn="20d4f2eb-ce34-4c44-87b8-8b386c283930" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn ID="d7aabecd-d9a4-4dbc-87bb-b9ad103ea8cf" Name="EstimatedCost" Type="Decimal(20, 2) Null">
		<Description>Ориентировочная стоимость тендера (руб.)</Description>
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="e43c7476-7746-4743-b40c-e79101407318" Name="StartDate" Type="Date Null">
		<Description>Дата начала</Description>
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="b3b606ec-f7b4-4dbd-94c0-ea4855032d48" Name="ProvidingTDDate" Type="Date Null">
		<Description>Дата предоставления ТД</Description>
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="a6e741c6-d7eb-458e-91b6-5e59fb4deaac" Name="PlannedDate" Type="Date Null">
		<Description>Планируемая дата тендера</Description>
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="b0eed227-1548-40b2-92ce-58e0f77619cb" Name="RemainderBudgetFunds" Type="Decimal(20, 2) Null">
		<Description>Остаток бюджетных средств (руб.)</Description>
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="907b602f-3c90-4376-9bdb-8867191c0187" Name="PriceReferenceProposal" Type="Decimal(20, 2) Null">
		<Description>Стоимость по эталонному предложению (руб.)</Description>
	</SchemePhysicalColumn>
	<SchemeComplexColumn ID="ddf6eeff-962d-4230-b7d2-7cd2ba8319cf" Name="Initiator" Type="Reference(Typified) Null" ReferencedTable="6c977939-bbfc-456f-a133-f1c2244e3cc3">
		<Description>Инициатор</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="ddf6eeff-962d-0030-4000-0cd2ba8319cf" Name="InitiatorID" Type="Guid Null" ReferencedColumn="6c977939-bbfc-016f-4000-01c2244e3cc3" />
		<SchemeReferencingColumn ID="8d4ee053-e6c5-47ea-94fb-cecaf990d928" Name="InitiatorName" Type="String(128) Null" ReferencedColumn="1782f76a-4743-4aa4-920c-7edaee860964" />
	</SchemeComplexColumn>
	<SchemeComplexColumn ID="93623bcd-5c25-4637-b0b1-24b717b397f6" Name="Winner" Type="Reference(Typified) Null" ReferencedTable="5d47ef13-b6f4-47ef-9815-3b3d0e6d475a">
		<Description>Победитель тендера</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="93623bcd-5c25-0037-4000-04b717b397f6" Name="WinnerID" Type="Guid Null" ReferencedColumn="5d47ef13-b6f4-01ef-4000-0b3d0e6d475a" />
		<SchemeReferencingColumn ID="1ccea0d2-eff6-4cd5-abd4-e298ec87fa6e" Name="WinnerName" Type="String(255) Null" ReferencedColumn="f1c960e0-951e-4837-8474-bb61d98f40f0" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn ID="6814731e-2d6e-40f4-8774-8d42ce5c511d" Name="ReceiptDate" Type="Date Null">
		<Description>Дата поступления</Description>
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="5a86d6fa-226d-42bf-9fe8-08e4b662b9c0" Name="IssueStatus" Type="String(Max) Null">
		<Description>Статус вопроса</Description>
	</SchemePhysicalColumn>
	<SchemeComplexColumn ID="58274cf3-e3b2-4a93-91b3-b7705c772ffa" Name="Status" Type="Reference(Typified) Null" ReferencedTable="60efb9ae-b38b-4568-b44d-dca30e74dc17">
		<Description>Статус тендера</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="58274cf3-e3b2-0093-4000-07705c772ffa" Name="StatusID" Type="Int16 Null" ReferencedColumn="88bfaf6d-36e4-492c-9595-ffa4e2119ae9" />
		<SchemeReferencingColumn ID="ca623027-c1f2-4230-8b55-53d1b7ac72e9" Name="StatusName" Type="String(Max) Null" ReferencedColumn="bd173e71-ebda-4a27-be54-0171037c05b7" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn ID="eb2815c5-3784-4f20-80df-d2abe66c3ef1" Name="Comment" Type="String(Max) Null">
		<Description>Комментарий</Description>
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="cb36d234-df80-4481-a20c-aef16072117c" Name="IsSVISApprovalRequired" Type="Boolean Null">
		<Description>Требуется согласование СВИС</Description>
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="0f8d6d3d-b055-4918-9cb5-b3402d1e39e6" Name="IsSPiAApprovalRequired" Type="Boolean Null">
		<Description>Требуется согласование СПиА</Description>
	</SchemePhysicalColumn>
	<SchemeComplexColumn ID="3c91022a-3334-41be-a40d-2f83eb2cc9c0" Name="GroupDocuments" Type="Reference(Typified) Null" ReferencedTable="6c977939-bbfc-456f-a133-f1c2244e3cc3">
		<Description>Группа документов</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="3c91022a-3334-00be-4000-0f83eb2cc9c0" Name="GroupDocumentsID" Type="Guid Null" ReferencedColumn="6c977939-bbfc-016f-4000-01c2244e3cc3" />
		<SchemeReferencingColumn ID="e86573ac-0795-4ae3-9091-b983c5442566" Name="GroupDocumentsName" Type="String(128) Null" ReferencedColumn="1782f76a-4743-4aa4-920c-7edaee860964" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn ID="8bac9845-7264-4e1b-aad4-cdeaa7c5d127" Name="ProtocolNo" Type="String(Max) Null">
		<Description>Протокол №</Description>
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="f9f1a288-335c-4968-9e59-f70da16ae37f" Name="BudgetSize" Type="Decimal(20, 2) Null">
		<Description>Размер бюджета (руб.) </Description>
	</SchemePhysicalColumn>
	<SchemeComplexColumn ID="b10d6e20-9d90-473d-9abb-2a4f7c670ad8" Name="BudgetComment" Type="Reference(Typified) Null" ReferencedTable="fb6e1129-6e9f-4d61-842a-2ad319a4b328">
		<Description>Комментарий к бюджету</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="b10d6e20-9d90-003d-4000-0a4f7c670ad8" Name="BudgetCommentID" Type="Int16 Null" ReferencedColumn="aef4a1d6-e087-4705-b25b-80f15b39ef34" />
		<SchemeReferencingColumn ID="6355f256-448b-4817-ae1a-a6b493a820af" Name="BudgetCommentContent" Type="String(Max) Null" ReferencedColumn="f7189dc8-387e-4cc6-9f98-e1765b544573" />
	</SchemeComplexColumn>
	<SchemePhysicalColumn ID="ffa8f207-a7b9-42eb-a824-efb8b28e9258" Name="FinalCost" Type="Decimal(20, 2) Null">
		<Description>Итоговая стоимость тендера (руб.)</Description>
	</SchemePhysicalColumn>
	<SchemeComplexColumn ID="46a121ec-6d97-4393-85c5-451fdf14f752" Name="Stage" Type="Reference(Typified) Null" ReferencedTable="acb2c154-f00a-4dbd-adb0-8b77ca861ae2">
		<Description>Стадия реализации</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="46a121ec-6d97-0093-4000-051fdf14f752" Name="StageID" Type="Guid Null" ReferencedColumn="acb2c154-f00a-01bd-4000-0b77ca861ae2" />
		<SchemeReferencingColumn ID="665dd730-c7d7-4558-9134-18fa9ae7c042" Name="StageName" Type="String(Max) Null" ReferencedColumn="32b4bb15-8adb-4851-b9c6-d990cb2e3396" />
		<SchemeReferencingColumn ID="b816db66-72c7-4331-8251-39e1695811fd" Name="StageIsExcludeFromSelection" Type="Boolean Null" ReferencedColumn="711d08ff-544b-42a6-a733-570d83ccb7a6" />
	</SchemeComplexColumn>
	<SchemeComplexColumn ID="453fba9f-66b7-4375-a24e-b4669ef46fd9" Name="ContractType" Type="Reference(Typified) Null" ReferencedTable="e41cd076-fa8f-4ee2-857c-2f1ecf257eb7">
		<Description>Тип договора</Description>
		<SchemeReferencingColumn IsSystem="true" IsPermanent="true" ID="453fba9f-66b7-0075-4000-04669ef46fd9" Name="ContractTypeID" Type="Guid Null" ReferencedColumn="e41cd076-fa8f-01e2-4000-0f1ecf257eb7" />
		<SchemeReferencingColumn ID="c138442b-052f-4030-878a-f2c86c91fae0" Name="ContractTypeName" Type="String(Max) Null" ReferencedColumn="6fb304a3-3a0c-4184-a9e1-09342acdee0f" />
	</SchemeComplexColumn>
	<SchemePrimaryKey IsSystem="true" IsPermanent="true" IsSealed="true" ID="45c89001-961e-001c-5000-02821820aebd" Name="pk_PnrTenders" IsClustered="true">
		<SchemeIndexedColumn Column="45c89001-961e-011c-4000-02821820aebd" />
	</SchemePrimaryKey>
</SchemeTable>