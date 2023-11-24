<?xml version="1.0" encoding="utf-8"?>
<SchemeTable Partition="c3b14969-a553-4005-9a07-e9705c15b1e1" ID="0290dd16-eb65-4501-bfc7-119a8352a6e1" Name="FdFieldDataType_CriteriaOperators" Group="Fd Criteria">
	<Description>Связи между типами данных и опреаторами критериев, применимыми к ним</Description>
	<SchemePhysicalColumn ID="0d01306f-6a4b-4a34-83aa-5084c58211d4" Name="ID" Type="Int32 Not Null" />
	<SchemePhysicalColumn ID="1921ced1-d743-435c-b2ea-3cdd714a15ff" Name="DataTypeID" Type="Guid Not Null">
		<Description>Тип данных (из Tessa.Cards.CardControlTypes)</Description>
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="0a40e8c0-5b20-43e4-896b-60c24dc4d543" Name="DataTypeName" Type="String(128) Not Null" />
	<SchemePhysicalColumn ID="9b3f8652-0edc-49ed-8caf-93343de1090f" Name="CriteriaOperatorID" Type="Int16 Not Null">
		<SchemeDefaultConstraint IsPermanent="true" ID="1df23aa7-026e-451c-9559-7e121ed2e375" Name="df_FdFieldDataType_CriteriaOperators_CriteriaOperatorID" Value="0" />
	</SchemePhysicalColumn>
	<SchemePhysicalColumn ID="d52270b7-ab1d-4fbc-b98d-116efb3d3179" Name="CriteriaOperatorName" Type="String(128) Not Null">
		<SchemeDefaultConstraint IsPermanent="true" ID="8ca80dc8-7916-4797-9fe5-85174fba3a82" Name="df_FdFieldDataType_CriteriaOperators_CriteriaOperatorName" Value="String" />
	</SchemePhysicalColumn>
	<SchemePrimaryKey ID="f4ec3e24-595d-482a-a9ab-0b9214780ad8" Name="pk_FdFieldDataType_CriteriaOperators">
		<SchemeIndexedColumn Column="0d01306f-6a4b-4a34-83aa-5084c58211d4" />
	</SchemePrimaryKey>
	<SchemeRecord>
		<ID ID="0d01306f-6a4b-4a34-83aa-5084c58211d4">0</ID>
		<DataTypeID ID="1921ced1-d743-435c-b2ea-3cdd714a15ff">79633881-a684-010a-b1ea-027d714bda5d</DataTypeID>
		<DataTypeName ID="0a40e8c0-5b20-43e4-896b-60c24dc4d543">Boolean</DataTypeName>
		<CriteriaOperatorID ID="9b3f8652-0edc-49ed-8caf-93343de1090f">6</CriteriaOperatorID>
		<CriteriaOperatorName ID="d52270b7-ab1d-4fbc-b98d-116efb3d3179">IsNull</CriteriaOperatorName>
	</SchemeRecord>
	<SchemeRecord>
		<ID ID="0d01306f-6a4b-4a34-83aa-5084c58211d4">1</ID>
		<DataTypeID ID="1921ced1-d743-435c-b2ea-3cdd714a15ff">79633881-a684-010a-b1ea-027d714bda5d</DataTypeID>
		<DataTypeName ID="0a40e8c0-5b20-43e4-896b-60c24dc4d543">Boolean</DataTypeName>
		<CriteriaOperatorID ID="9b3f8652-0edc-49ed-8caf-93343de1090f">7</CriteriaOperatorID>
		<CriteriaOperatorName ID="d52270b7-ab1d-4fbc-b98d-116efb3d3179">IsNotNull</CriteriaOperatorName>
	</SchemeRecord>
	<SchemeRecord>
		<ID ID="0d01306f-6a4b-4a34-83aa-5084c58211d4">2</ID>
		<DataTypeID ID="1921ced1-d743-435c-b2ea-3cdd714a15ff">c08b81f3-ed18-005a-bae6-a61679812709</DataTypeID>
		<DataTypeName ID="0a40e8c0-5b20-43e4-896b-60c24dc4d543">DateTime</DataTypeName>
		<CriteriaOperatorID ID="9b3f8652-0edc-49ed-8caf-93343de1090f">0</CriteriaOperatorID>
		<CriteriaOperatorName ID="d52270b7-ab1d-4fbc-b98d-116efb3d3179">Equals</CriteriaOperatorName>
	</SchemeRecord>
	<SchemeRecord>
		<ID ID="0d01306f-6a4b-4a34-83aa-5084c58211d4">3</ID>
		<DataTypeID ID="1921ced1-d743-435c-b2ea-3cdd714a15ff">c08b81f3-ed18-005a-bae6-a61679812709</DataTypeID>
		<DataTypeName ID="0a40e8c0-5b20-43e4-896b-60c24dc4d543">DateTime</DataTypeName>
		<CriteriaOperatorID ID="9b3f8652-0edc-49ed-8caf-93343de1090f">1</CriteriaOperatorID>
		<CriteriaOperatorName ID="d52270b7-ab1d-4fbc-b98d-116efb3d3179">NonEquals</CriteriaOperatorName>
	</SchemeRecord>
	<SchemeRecord>
		<ID ID="0d01306f-6a4b-4a34-83aa-5084c58211d4">4</ID>
		<DataTypeID ID="1921ced1-d743-435c-b2ea-3cdd714a15ff">c08b81f3-ed18-005a-bae6-a61679812709</DataTypeID>
		<DataTypeName ID="0a40e8c0-5b20-43e4-896b-60c24dc4d543">DateTime</DataTypeName>
		<CriteriaOperatorID ID="9b3f8652-0edc-49ed-8caf-93343de1090f">2</CriteriaOperatorID>
		<CriteriaOperatorName ID="d52270b7-ab1d-4fbc-b98d-116efb3d3179">GreatThan</CriteriaOperatorName>
	</SchemeRecord>
	<SchemeRecord>
		<ID ID="0d01306f-6a4b-4a34-83aa-5084c58211d4">5</ID>
		<DataTypeID ID="1921ced1-d743-435c-b2ea-3cdd714a15ff">c08b81f3-ed18-005a-bae6-a61679812709</DataTypeID>
		<DataTypeName ID="0a40e8c0-5b20-43e4-896b-60c24dc4d543">DateTime</DataTypeName>
		<CriteriaOperatorID ID="9b3f8652-0edc-49ed-8caf-93343de1090f">3</CriteriaOperatorID>
		<CriteriaOperatorName ID="d52270b7-ab1d-4fbc-b98d-116efb3d3179">LessThan</CriteriaOperatorName>
	</SchemeRecord>
	<SchemeRecord>
		<ID ID="0d01306f-6a4b-4a34-83aa-5084c58211d4">6</ID>
		<DataTypeID ID="1921ced1-d743-435c-b2ea-3cdd714a15ff">c08b81f3-ed18-005a-bae6-a61679812709</DataTypeID>
		<DataTypeName ID="0a40e8c0-5b20-43e4-896b-60c24dc4d543">DateTime</DataTypeName>
		<CriteriaOperatorID ID="9b3f8652-0edc-49ed-8caf-93343de1090f">4</CriteriaOperatorID>
		<CriteriaOperatorName ID="d52270b7-ab1d-4fbc-b98d-116efb3d3179">GreatOrEquals</CriteriaOperatorName>
	</SchemeRecord>
	<SchemeRecord>
		<ID ID="0d01306f-6a4b-4a34-83aa-5084c58211d4">7</ID>
		<DataTypeID ID="1921ced1-d743-435c-b2ea-3cdd714a15ff">c08b81f3-ed18-005a-bae6-a61679812709</DataTypeID>
		<DataTypeName ID="0a40e8c0-5b20-43e4-896b-60c24dc4d543">DateTime</DataTypeName>
		<CriteriaOperatorID ID="9b3f8652-0edc-49ed-8caf-93343de1090f">5</CriteriaOperatorID>
		<CriteriaOperatorName ID="d52270b7-ab1d-4fbc-b98d-116efb3d3179">LessOrEquals</CriteriaOperatorName>
	</SchemeRecord>
	<SchemeRecord>
		<ID ID="0d01306f-6a4b-4a34-83aa-5084c58211d4">8</ID>
		<DataTypeID ID="1921ced1-d743-435c-b2ea-3cdd714a15ff">c08b81f3-ed18-005a-bae6-a61679812709</DataTypeID>
		<DataTypeName ID="0a40e8c0-5b20-43e4-896b-60c24dc4d543">DateTime</DataTypeName>
		<CriteriaOperatorID ID="9b3f8652-0edc-49ed-8caf-93343de1090f">6</CriteriaOperatorID>
		<CriteriaOperatorName ID="d52270b7-ab1d-4fbc-b98d-116efb3d3179">IsNull</CriteriaOperatorName>
	</SchemeRecord>
	<SchemeRecord>
		<ID ID="0d01306f-6a4b-4a34-83aa-5084c58211d4">9</ID>
		<DataTypeID ID="1921ced1-d743-435c-b2ea-3cdd714a15ff">c08b81f3-ed18-005a-bae6-a61679812709</DataTypeID>
		<DataTypeName ID="0a40e8c0-5b20-43e4-896b-60c24dc4d543">DateTime</DataTypeName>
		<CriteriaOperatorID ID="9b3f8652-0edc-49ed-8caf-93343de1090f">7</CriteriaOperatorID>
		<CriteriaOperatorName ID="d52270b7-ab1d-4fbc-b98d-116efb3d3179">IsNotNull</CriteriaOperatorName>
	</SchemeRecord>
	<SchemeRecord>
		<ID ID="0d01306f-6a4b-4a34-83aa-5084c58211d4">10</ID>
		<DataTypeID ID="1921ced1-d743-435c-b2ea-3cdd714a15ff">c08b81f3-ed18-005a-bae6-a61679812709</DataTypeID>
		<DataTypeName ID="0a40e8c0-5b20-43e4-896b-60c24dc4d543">DateTime</DataTypeName>
		<CriteriaOperatorID ID="9b3f8652-0edc-49ed-8caf-93343de1090f">8</CriteriaOperatorID>
		<CriteriaOperatorName ID="d52270b7-ab1d-4fbc-b98d-116efb3d3179">Between</CriteriaOperatorName>
	</SchemeRecord>
	<SchemeRecord>
		<ID ID="0d01306f-6a4b-4a34-83aa-5084c58211d4">11</ID>
		<DataTypeID ID="1921ced1-d743-435c-b2ea-3cdd714a15ff">79633881-a684-010a-b1ea-027d714bda5d</DataTypeID>
		<DataTypeName ID="0a40e8c0-5b20-43e4-896b-60c24dc4d543">Boolean</DataTypeName>
		<CriteriaOperatorID ID="9b3f8652-0edc-49ed-8caf-93343de1090f">12</CriteriaOperatorID>
		<CriteriaOperatorName ID="d52270b7-ab1d-4fbc-b98d-116efb3d3179">IsTrue</CriteriaOperatorName>
	</SchemeRecord>
	<SchemeRecord>
		<ID ID="0d01306f-6a4b-4a34-83aa-5084c58211d4">12</ID>
		<DataTypeID ID="1921ced1-d743-435c-b2ea-3cdd714a15ff">79633881-a684-010a-b1ea-027d714bda5d</DataTypeID>
		<DataTypeName ID="0a40e8c0-5b20-43e4-896b-60c24dc4d543">Boolean</DataTypeName>
		<CriteriaOperatorID ID="9b3f8652-0edc-49ed-8caf-93343de1090f">13</CriteriaOperatorID>
		<CriteriaOperatorName ID="d52270b7-ab1d-4fbc-b98d-116efb3d3179">IsFalse</CriteriaOperatorName>
	</SchemeRecord>
	<SchemeRecord>
		<ID ID="0d01306f-6a4b-4a34-83aa-5084c58211d4">13</ID>
		<DataTypeID ID="1921ced1-d743-435c-b2ea-3cdd714a15ff">01c2bb84-6b71-034e-859e-b6e2b64ff2f3</DataTypeID>
		<DataTypeName ID="0a40e8c0-5b20-43e4-896b-60c24dc4d543">Int</DataTypeName>
		<CriteriaOperatorID ID="9b3f8652-0edc-49ed-8caf-93343de1090f">0</CriteriaOperatorID>
		<CriteriaOperatorName ID="d52270b7-ab1d-4fbc-b98d-116efb3d3179">Equals</CriteriaOperatorName>
	</SchemeRecord>
	<SchemeRecord>
		<ID ID="0d01306f-6a4b-4a34-83aa-5084c58211d4">14</ID>
		<DataTypeID ID="1921ced1-d743-435c-b2ea-3cdd714a15ff">01c2bb84-6b71-034e-859e-b6e2b64ff2f3</DataTypeID>
		<DataTypeName ID="0a40e8c0-5b20-43e4-896b-60c24dc4d543">Int</DataTypeName>
		<CriteriaOperatorID ID="9b3f8652-0edc-49ed-8caf-93343de1090f">1</CriteriaOperatorID>
		<CriteriaOperatorName ID="d52270b7-ab1d-4fbc-b98d-116efb3d3179">NonEquals</CriteriaOperatorName>
	</SchemeRecord>
	<SchemeRecord>
		<ID ID="0d01306f-6a4b-4a34-83aa-5084c58211d4">15</ID>
		<DataTypeID ID="1921ced1-d743-435c-b2ea-3cdd714a15ff">01c2bb84-6b71-034e-859e-b6e2b64ff2f3</DataTypeID>
		<DataTypeName ID="0a40e8c0-5b20-43e4-896b-60c24dc4d543">Int</DataTypeName>
		<CriteriaOperatorID ID="9b3f8652-0edc-49ed-8caf-93343de1090f">2</CriteriaOperatorID>
		<CriteriaOperatorName ID="d52270b7-ab1d-4fbc-b98d-116efb3d3179">GreatThan</CriteriaOperatorName>
	</SchemeRecord>
	<SchemeRecord>
		<ID ID="0d01306f-6a4b-4a34-83aa-5084c58211d4">16</ID>
		<DataTypeID ID="1921ced1-d743-435c-b2ea-3cdd714a15ff">01c2bb84-6b71-034e-859e-b6e2b64ff2f3</DataTypeID>
		<DataTypeName ID="0a40e8c0-5b20-43e4-896b-60c24dc4d543">Int</DataTypeName>
		<CriteriaOperatorID ID="9b3f8652-0edc-49ed-8caf-93343de1090f">3</CriteriaOperatorID>
		<CriteriaOperatorName ID="d52270b7-ab1d-4fbc-b98d-116efb3d3179">LessThan</CriteriaOperatorName>
	</SchemeRecord>
	<SchemeRecord>
		<ID ID="0d01306f-6a4b-4a34-83aa-5084c58211d4">17</ID>
		<DataTypeID ID="1921ced1-d743-435c-b2ea-3cdd714a15ff">01c2bb84-6b71-034e-859e-b6e2b64ff2f3</DataTypeID>
		<DataTypeName ID="0a40e8c0-5b20-43e4-896b-60c24dc4d543">Int</DataTypeName>
		<CriteriaOperatorID ID="9b3f8652-0edc-49ed-8caf-93343de1090f">4</CriteriaOperatorID>
		<CriteriaOperatorName ID="d52270b7-ab1d-4fbc-b98d-116efb3d3179">GreatOrEquals</CriteriaOperatorName>
	</SchemeRecord>
	<SchemeRecord>
		<ID ID="0d01306f-6a4b-4a34-83aa-5084c58211d4">18</ID>
		<DataTypeID ID="1921ced1-d743-435c-b2ea-3cdd714a15ff">01c2bb84-6b71-034e-859e-b6e2b64ff2f3</DataTypeID>
		<DataTypeName ID="0a40e8c0-5b20-43e4-896b-60c24dc4d543">Int</DataTypeName>
		<CriteriaOperatorID ID="9b3f8652-0edc-49ed-8caf-93343de1090f">5</CriteriaOperatorID>
		<CriteriaOperatorName ID="d52270b7-ab1d-4fbc-b98d-116efb3d3179">LessOrEquals</CriteriaOperatorName>
	</SchemeRecord>
	<SchemeRecord>
		<ID ID="0d01306f-6a4b-4a34-83aa-5084c58211d4">19</ID>
		<DataTypeID ID="1921ced1-d743-435c-b2ea-3cdd714a15ff">01c2bb84-6b71-034e-859e-b6e2b64ff2f3</DataTypeID>
		<DataTypeName ID="0a40e8c0-5b20-43e4-896b-60c24dc4d543">Int</DataTypeName>
		<CriteriaOperatorID ID="9b3f8652-0edc-49ed-8caf-93343de1090f">6</CriteriaOperatorID>
		<CriteriaOperatorName ID="d52270b7-ab1d-4fbc-b98d-116efb3d3179">IsNull</CriteriaOperatorName>
	</SchemeRecord>
	<SchemeRecord>
		<ID ID="0d01306f-6a4b-4a34-83aa-5084c58211d4">20</ID>
		<DataTypeID ID="1921ced1-d743-435c-b2ea-3cdd714a15ff">01c2bb84-6b71-034e-859e-b6e2b64ff2f3</DataTypeID>
		<DataTypeName ID="0a40e8c0-5b20-43e4-896b-60c24dc4d543">Int</DataTypeName>
		<CriteriaOperatorID ID="9b3f8652-0edc-49ed-8caf-93343de1090f">7</CriteriaOperatorID>
		<CriteriaOperatorName ID="d52270b7-ab1d-4fbc-b98d-116efb3d3179">IsNotNull</CriteriaOperatorName>
	</SchemeRecord>
	<SchemeRecord>
		<ID ID="0d01306f-6a4b-4a34-83aa-5084c58211d4">21</ID>
		<DataTypeID ID="1921ced1-d743-435c-b2ea-3cdd714a15ff">01c2bb84-6b71-034e-859e-b6e2b64ff2f3</DataTypeID>
		<DataTypeName ID="0a40e8c0-5b20-43e4-896b-60c24dc4d543">Int</DataTypeName>
		<CriteriaOperatorID ID="9b3f8652-0edc-49ed-8caf-93343de1090f">8</CriteriaOperatorID>
		<CriteriaOperatorName ID="d52270b7-ab1d-4fbc-b98d-116efb3d3179">Between</CriteriaOperatorName>
	</SchemeRecord>
	<SchemeRecord>
		<ID ID="0d01306f-6a4b-4a34-83aa-5084c58211d4">22</ID>
		<DataTypeID ID="1921ced1-d743-435c-b2ea-3cdd714a15ff">65ad3578-b6a2-045f-9d55-c915a19dbc96</DataTypeID>
		<DataTypeName ID="0a40e8c0-5b20-43e4-896b-60c24dc4d543">Double</DataTypeName>
		<CriteriaOperatorID ID="9b3f8652-0edc-49ed-8caf-93343de1090f">0</CriteriaOperatorID>
		<CriteriaOperatorName ID="d52270b7-ab1d-4fbc-b98d-116efb3d3179">Equals</CriteriaOperatorName>
	</SchemeRecord>
	<SchemeRecord>
		<ID ID="0d01306f-6a4b-4a34-83aa-5084c58211d4">23</ID>
		<DataTypeID ID="1921ced1-d743-435c-b2ea-3cdd714a15ff">65ad3578-b6a2-045f-9d55-c915a19dbc96</DataTypeID>
		<DataTypeName ID="0a40e8c0-5b20-43e4-896b-60c24dc4d543">Double</DataTypeName>
		<CriteriaOperatorID ID="9b3f8652-0edc-49ed-8caf-93343de1090f">1</CriteriaOperatorID>
		<CriteriaOperatorName ID="d52270b7-ab1d-4fbc-b98d-116efb3d3179">NonEquals</CriteriaOperatorName>
	</SchemeRecord>
	<SchemeRecord>
		<ID ID="0d01306f-6a4b-4a34-83aa-5084c58211d4">24</ID>
		<DataTypeID ID="1921ced1-d743-435c-b2ea-3cdd714a15ff">65ad3578-b6a2-045f-9d55-c915a19dbc96</DataTypeID>
		<DataTypeName ID="0a40e8c0-5b20-43e4-896b-60c24dc4d543">Double</DataTypeName>
		<CriteriaOperatorID ID="9b3f8652-0edc-49ed-8caf-93343de1090f">2</CriteriaOperatorID>
		<CriteriaOperatorName ID="d52270b7-ab1d-4fbc-b98d-116efb3d3179">GreatThan</CriteriaOperatorName>
	</SchemeRecord>
	<SchemeRecord>
		<ID ID="0d01306f-6a4b-4a34-83aa-5084c58211d4">25</ID>
		<DataTypeID ID="1921ced1-d743-435c-b2ea-3cdd714a15ff">65ad3578-b6a2-045f-9d55-c915a19dbc96</DataTypeID>
		<DataTypeName ID="0a40e8c0-5b20-43e4-896b-60c24dc4d543">Double</DataTypeName>
		<CriteriaOperatorID ID="9b3f8652-0edc-49ed-8caf-93343de1090f">3</CriteriaOperatorID>
		<CriteriaOperatorName ID="d52270b7-ab1d-4fbc-b98d-116efb3d3179">LessThan</CriteriaOperatorName>
	</SchemeRecord>
	<SchemeRecord>
		<ID ID="0d01306f-6a4b-4a34-83aa-5084c58211d4">26</ID>
		<DataTypeID ID="1921ced1-d743-435c-b2ea-3cdd714a15ff">65ad3578-b6a2-045f-9d55-c915a19dbc96</DataTypeID>
		<DataTypeName ID="0a40e8c0-5b20-43e4-896b-60c24dc4d543">Double</DataTypeName>
		<CriteriaOperatorID ID="9b3f8652-0edc-49ed-8caf-93343de1090f">4</CriteriaOperatorID>
		<CriteriaOperatorName ID="d52270b7-ab1d-4fbc-b98d-116efb3d3179">GreatOrEquals</CriteriaOperatorName>
	</SchemeRecord>
	<SchemeRecord>
		<ID ID="0d01306f-6a4b-4a34-83aa-5084c58211d4">27</ID>
		<DataTypeID ID="1921ced1-d743-435c-b2ea-3cdd714a15ff">65ad3578-b6a2-045f-9d55-c915a19dbc96</DataTypeID>
		<DataTypeName ID="0a40e8c0-5b20-43e4-896b-60c24dc4d543">Double</DataTypeName>
		<CriteriaOperatorID ID="9b3f8652-0edc-49ed-8caf-93343de1090f">5</CriteriaOperatorID>
		<CriteriaOperatorName ID="d52270b7-ab1d-4fbc-b98d-116efb3d3179">LessOrEquals</CriteriaOperatorName>
	</SchemeRecord>
	<SchemeRecord>
		<ID ID="0d01306f-6a4b-4a34-83aa-5084c58211d4">28</ID>
		<DataTypeID ID="1921ced1-d743-435c-b2ea-3cdd714a15ff">65ad3578-b6a2-045f-9d55-c915a19dbc96</DataTypeID>
		<DataTypeName ID="0a40e8c0-5b20-43e4-896b-60c24dc4d543">Double</DataTypeName>
		<CriteriaOperatorID ID="9b3f8652-0edc-49ed-8caf-93343de1090f">6</CriteriaOperatorID>
		<CriteriaOperatorName ID="d52270b7-ab1d-4fbc-b98d-116efb3d3179">IsNull</CriteriaOperatorName>
	</SchemeRecord>
	<SchemeRecord>
		<ID ID="0d01306f-6a4b-4a34-83aa-5084c58211d4">29</ID>
		<DataTypeID ID="1921ced1-d743-435c-b2ea-3cdd714a15ff">65ad3578-b6a2-045f-9d55-c915a19dbc96</DataTypeID>
		<DataTypeName ID="0a40e8c0-5b20-43e4-896b-60c24dc4d543">Double</DataTypeName>
		<CriteriaOperatorID ID="9b3f8652-0edc-49ed-8caf-93343de1090f">7</CriteriaOperatorID>
		<CriteriaOperatorName ID="d52270b7-ab1d-4fbc-b98d-116efb3d3179">IsNotNull</CriteriaOperatorName>
	</SchemeRecord>
	<SchemeRecord>
		<ID ID="0d01306f-6a4b-4a34-83aa-5084c58211d4">30</ID>
		<DataTypeID ID="1921ced1-d743-435c-b2ea-3cdd714a15ff">65ad3578-b6a2-045f-9d55-c915a19dbc96</DataTypeID>
		<DataTypeName ID="0a40e8c0-5b20-43e4-896b-60c24dc4d543">Double</DataTypeName>
		<CriteriaOperatorID ID="9b3f8652-0edc-49ed-8caf-93343de1090f">8</CriteriaOperatorID>
		<CriteriaOperatorName ID="d52270b7-ab1d-4fbc-b98d-116efb3d3179">Between</CriteriaOperatorName>
	</SchemeRecord>
	<SchemeRecord>
		<ID ID="0d01306f-6a4b-4a34-83aa-5084c58211d4">31</ID>
		<DataTypeID ID="1921ced1-d743-435c-b2ea-3cdd714a15ff">2860b9fd-2592-0ca1-ae99-e8051a04d4bb</DataTypeID>
		<DataTypeName ID="0a40e8c0-5b20-43e4-896b-60c24dc4d543">Decimal</DataTypeName>
		<CriteriaOperatorID ID="9b3f8652-0edc-49ed-8caf-93343de1090f">0</CriteriaOperatorID>
		<CriteriaOperatorName ID="d52270b7-ab1d-4fbc-b98d-116efb3d3179">Equals</CriteriaOperatorName>
	</SchemeRecord>
	<SchemeRecord>
		<ID ID="0d01306f-6a4b-4a34-83aa-5084c58211d4">32</ID>
		<DataTypeID ID="1921ced1-d743-435c-b2ea-3cdd714a15ff">2860b9fd-2592-0ca1-ae99-e8051a04d4bb</DataTypeID>
		<DataTypeName ID="0a40e8c0-5b20-43e4-896b-60c24dc4d543">Decimal</DataTypeName>
		<CriteriaOperatorID ID="9b3f8652-0edc-49ed-8caf-93343de1090f">1</CriteriaOperatorID>
		<CriteriaOperatorName ID="d52270b7-ab1d-4fbc-b98d-116efb3d3179">NonEquals</CriteriaOperatorName>
	</SchemeRecord>
	<SchemeRecord>
		<ID ID="0d01306f-6a4b-4a34-83aa-5084c58211d4">33</ID>
		<DataTypeID ID="1921ced1-d743-435c-b2ea-3cdd714a15ff">2860b9fd-2592-0ca1-ae99-e8051a04d4bb</DataTypeID>
		<DataTypeName ID="0a40e8c0-5b20-43e4-896b-60c24dc4d543">Decimal</DataTypeName>
		<CriteriaOperatorID ID="9b3f8652-0edc-49ed-8caf-93343de1090f">2</CriteriaOperatorID>
		<CriteriaOperatorName ID="d52270b7-ab1d-4fbc-b98d-116efb3d3179">GreatThan</CriteriaOperatorName>
	</SchemeRecord>
	<SchemeRecord>
		<ID ID="0d01306f-6a4b-4a34-83aa-5084c58211d4">34</ID>
		<DataTypeID ID="1921ced1-d743-435c-b2ea-3cdd714a15ff">2860b9fd-2592-0ca1-ae99-e8051a04d4bb</DataTypeID>
		<DataTypeName ID="0a40e8c0-5b20-43e4-896b-60c24dc4d543">Decimal</DataTypeName>
		<CriteriaOperatorID ID="9b3f8652-0edc-49ed-8caf-93343de1090f">3</CriteriaOperatorID>
		<CriteriaOperatorName ID="d52270b7-ab1d-4fbc-b98d-116efb3d3179">LessThan</CriteriaOperatorName>
	</SchemeRecord>
	<SchemeRecord>
		<ID ID="0d01306f-6a4b-4a34-83aa-5084c58211d4">35</ID>
		<DataTypeID ID="1921ced1-d743-435c-b2ea-3cdd714a15ff">2860b9fd-2592-0ca1-ae99-e8051a04d4bb</DataTypeID>
		<DataTypeName ID="0a40e8c0-5b20-43e4-896b-60c24dc4d543">Decimal</DataTypeName>
		<CriteriaOperatorID ID="9b3f8652-0edc-49ed-8caf-93343de1090f">4</CriteriaOperatorID>
		<CriteriaOperatorName ID="d52270b7-ab1d-4fbc-b98d-116efb3d3179">GreatOrEquals</CriteriaOperatorName>
	</SchemeRecord>
	<SchemeRecord>
		<ID ID="0d01306f-6a4b-4a34-83aa-5084c58211d4">36</ID>
		<DataTypeID ID="1921ced1-d743-435c-b2ea-3cdd714a15ff">2860b9fd-2592-0ca1-ae99-e8051a04d4bb</DataTypeID>
		<DataTypeName ID="0a40e8c0-5b20-43e4-896b-60c24dc4d543">Decimal</DataTypeName>
		<CriteriaOperatorID ID="9b3f8652-0edc-49ed-8caf-93343de1090f">5</CriteriaOperatorID>
		<CriteriaOperatorName ID="d52270b7-ab1d-4fbc-b98d-116efb3d3179">LessOrEquals</CriteriaOperatorName>
	</SchemeRecord>
	<SchemeRecord>
		<ID ID="0d01306f-6a4b-4a34-83aa-5084c58211d4">37</ID>
		<DataTypeID ID="1921ced1-d743-435c-b2ea-3cdd714a15ff">2860b9fd-2592-0ca1-ae99-e8051a04d4bb</DataTypeID>
		<DataTypeName ID="0a40e8c0-5b20-43e4-896b-60c24dc4d543">Decimal</DataTypeName>
		<CriteriaOperatorID ID="9b3f8652-0edc-49ed-8caf-93343de1090f">6</CriteriaOperatorID>
		<CriteriaOperatorName ID="d52270b7-ab1d-4fbc-b98d-116efb3d3179">IsNull</CriteriaOperatorName>
	</SchemeRecord>
	<SchemeRecord>
		<ID ID="0d01306f-6a4b-4a34-83aa-5084c58211d4">38</ID>
		<DataTypeID ID="1921ced1-d743-435c-b2ea-3cdd714a15ff">2860b9fd-2592-0ca1-ae99-e8051a04d4bb</DataTypeID>
		<DataTypeName ID="0a40e8c0-5b20-43e4-896b-60c24dc4d543">Decimal</DataTypeName>
		<CriteriaOperatorID ID="9b3f8652-0edc-49ed-8caf-93343de1090f">7</CriteriaOperatorID>
		<CriteriaOperatorName ID="d52270b7-ab1d-4fbc-b98d-116efb3d3179">IsNotNull</CriteriaOperatorName>
	</SchemeRecord>
	<SchemeRecord>
		<ID ID="0d01306f-6a4b-4a34-83aa-5084c58211d4">39</ID>
		<DataTypeID ID="1921ced1-d743-435c-b2ea-3cdd714a15ff">2860b9fd-2592-0ca1-ae99-e8051a04d4bb</DataTypeID>
		<DataTypeName ID="0a40e8c0-5b20-43e4-896b-60c24dc4d543">Decimal</DataTypeName>
		<CriteriaOperatorID ID="9b3f8652-0edc-49ed-8caf-93343de1090f">8</CriteriaOperatorID>
		<CriteriaOperatorName ID="d52270b7-ab1d-4fbc-b98d-116efb3d3179">Between</CriteriaOperatorName>
	</SchemeRecord>
	<SchemeRecord>
		<ID ID="0d01306f-6a4b-4a34-83aa-5084c58211d4">40</ID>
		<DataTypeID ID="1921ced1-d743-435c-b2ea-3cdd714a15ff">1962c931-9c23-0eb9-903c-26bac9ef3571</DataTypeID>
		<DataTypeName ID="0a40e8c0-5b20-43e4-896b-60c24dc4d543">String</DataTypeName>
		<CriteriaOperatorID ID="9b3f8652-0edc-49ed-8caf-93343de1090f">9</CriteriaOperatorID>
		<CriteriaOperatorName ID="d52270b7-ab1d-4fbc-b98d-116efb3d3179">Contains</CriteriaOperatorName>
	</SchemeRecord>
	<SchemeRecord>
		<ID ID="0d01306f-6a4b-4a34-83aa-5084c58211d4">41</ID>
		<DataTypeID ID="1921ced1-d743-435c-b2ea-3cdd714a15ff">1962c931-9c23-0eb9-903c-26bac9ef3571</DataTypeID>
		<DataTypeName ID="0a40e8c0-5b20-43e4-896b-60c24dc4d543">String</DataTypeName>
		<CriteriaOperatorID ID="9b3f8652-0edc-49ed-8caf-93343de1090f">0</CriteriaOperatorID>
		<CriteriaOperatorName ID="d52270b7-ab1d-4fbc-b98d-116efb3d3179">Equals</CriteriaOperatorName>
	</SchemeRecord>
	<SchemeRecord>
		<ID ID="0d01306f-6a4b-4a34-83aa-5084c58211d4">42</ID>
		<DataTypeID ID="1921ced1-d743-435c-b2ea-3cdd714a15ff">1962c931-9c23-0eb9-903c-26bac9ef3571</DataTypeID>
		<DataTypeName ID="0a40e8c0-5b20-43e4-896b-60c24dc4d543">String</DataTypeName>
		<CriteriaOperatorID ID="9b3f8652-0edc-49ed-8caf-93343de1090f">1</CriteriaOperatorID>
		<CriteriaOperatorName ID="d52270b7-ab1d-4fbc-b98d-116efb3d3179">NonEquals</CriteriaOperatorName>
	</SchemeRecord>
	<SchemeRecord>
		<ID ID="0d01306f-6a4b-4a34-83aa-5084c58211d4">43</ID>
		<DataTypeID ID="1921ced1-d743-435c-b2ea-3cdd714a15ff">1962c931-9c23-0eb9-903c-26bac9ef3571</DataTypeID>
		<DataTypeName ID="0a40e8c0-5b20-43e4-896b-60c24dc4d543">String</DataTypeName>
		<CriteriaOperatorID ID="9b3f8652-0edc-49ed-8caf-93343de1090f">10</CriteriaOperatorID>
		<CriteriaOperatorName ID="d52270b7-ab1d-4fbc-b98d-116efb3d3179">StartsWith</CriteriaOperatorName>
	</SchemeRecord>
	<SchemeRecord>
		<ID ID="0d01306f-6a4b-4a34-83aa-5084c58211d4">44</ID>
		<DataTypeID ID="1921ced1-d743-435c-b2ea-3cdd714a15ff">1962c931-9c23-0eb9-903c-26bac9ef3571</DataTypeID>
		<DataTypeName ID="0a40e8c0-5b20-43e4-896b-60c24dc4d543">String</DataTypeName>
		<CriteriaOperatorID ID="9b3f8652-0edc-49ed-8caf-93343de1090f">11</CriteriaOperatorID>
		<CriteriaOperatorName ID="d52270b7-ab1d-4fbc-b98d-116efb3d3179">EndsWith</CriteriaOperatorName>
	</SchemeRecord>
	<SchemeRecord>
		<ID ID="0d01306f-6a4b-4a34-83aa-5084c58211d4">45</ID>
		<DataTypeID ID="1921ced1-d743-435c-b2ea-3cdd714a15ff">1962c931-9c23-0eb9-903c-26bac9ef3571</DataTypeID>
		<DataTypeName ID="0a40e8c0-5b20-43e4-896b-60c24dc4d543">String</DataTypeName>
		<CriteriaOperatorID ID="9b3f8652-0edc-49ed-8caf-93343de1090f">6</CriteriaOperatorID>
		<CriteriaOperatorName ID="d52270b7-ab1d-4fbc-b98d-116efb3d3179">IsNull</CriteriaOperatorName>
	</SchemeRecord>
	<SchemeRecord>
		<ID ID="0d01306f-6a4b-4a34-83aa-5084c58211d4">46</ID>
		<DataTypeID ID="1921ced1-d743-435c-b2ea-3cdd714a15ff">1962c931-9c23-0eb9-903c-26bac9ef3571</DataTypeID>
		<DataTypeName ID="0a40e8c0-5b20-43e4-896b-60c24dc4d543">String</DataTypeName>
		<CriteriaOperatorID ID="9b3f8652-0edc-49ed-8caf-93343de1090f">7</CriteriaOperatorID>
		<CriteriaOperatorName ID="d52270b7-ab1d-4fbc-b98d-116efb3d3179">IsNotNull</CriteriaOperatorName>
	</SchemeRecord>
	<SchemeRecord>
		<ID ID="0d01306f-6a4b-4a34-83aa-5084c58211d4">47</ID>
		<DataTypeID ID="1921ced1-d743-435c-b2ea-3cdd714a15ff">208e364e-94fe-0d64-bc27-6b4dc12d039c</DataTypeID>
		<DataTypeName ID="0a40e8c0-5b20-43e4-896b-60c24dc4d543">Entry</DataTypeName>
		<CriteriaOperatorID ID="9b3f8652-0edc-49ed-8caf-93343de1090f">0</CriteriaOperatorID>
		<CriteriaOperatorName ID="d52270b7-ab1d-4fbc-b98d-116efb3d3179">Equals</CriteriaOperatorName>
	</SchemeRecord>
	<SchemeRecord>
		<ID ID="0d01306f-6a4b-4a34-83aa-5084c58211d4">48</ID>
		<DataTypeID ID="1921ced1-d743-435c-b2ea-3cdd714a15ff">208e364e-94fe-0d64-bc27-6b4dc12d039c</DataTypeID>
		<DataTypeName ID="0a40e8c0-5b20-43e4-896b-60c24dc4d543">Entry</DataTypeName>
		<CriteriaOperatorID ID="9b3f8652-0edc-49ed-8caf-93343de1090f">1</CriteriaOperatorID>
		<CriteriaOperatorName ID="d52270b7-ab1d-4fbc-b98d-116efb3d3179">NonEquals</CriteriaOperatorName>
	</SchemeRecord>
	<SchemeRecord>
		<ID ID="0d01306f-6a4b-4a34-83aa-5084c58211d4">49</ID>
		<DataTypeID ID="1921ced1-d743-435c-b2ea-3cdd714a15ff">208e364e-94fe-0d64-bc27-6b4dc12d039c</DataTypeID>
		<DataTypeName ID="0a40e8c0-5b20-43e4-896b-60c24dc4d543">Entry</DataTypeName>
		<CriteriaOperatorID ID="9b3f8652-0edc-49ed-8caf-93343de1090f">6</CriteriaOperatorID>
		<CriteriaOperatorName ID="d52270b7-ab1d-4fbc-b98d-116efb3d3179">IsNull</CriteriaOperatorName>
	</SchemeRecord>
	<SchemeRecord>
		<ID ID="0d01306f-6a4b-4a34-83aa-5084c58211d4">50</ID>
		<DataTypeID ID="1921ced1-d743-435c-b2ea-3cdd714a15ff">208e364e-94fe-0d64-bc27-6b4dc12d039c</DataTypeID>
		<DataTypeName ID="0a40e8c0-5b20-43e4-896b-60c24dc4d543">Entry</DataTypeName>
		<CriteriaOperatorID ID="9b3f8652-0edc-49ed-8caf-93343de1090f">7</CriteriaOperatorID>
		<CriteriaOperatorName ID="d52270b7-ab1d-4fbc-b98d-116efb3d3179">IsNotNull</CriteriaOperatorName>
	</SchemeRecord>
	<SchemeRecord>
		<ID ID="0d01306f-6a4b-4a34-83aa-5084c58211d4">51</ID>
		<DataTypeID ID="1921ced1-d743-435c-b2ea-3cdd714a15ff">208e364e-94fe-0d64-bc27-6b4dc12d039c</DataTypeID>
		<DataTypeName ID="0a40e8c0-5b20-43e4-896b-60c24dc4d543">Entry</DataTypeName>
		<CriteriaOperatorID ID="9b3f8652-0edc-49ed-8caf-93343de1090f">16</CriteriaOperatorID>
		<CriteriaOperatorName ID="d52270b7-ab1d-4fbc-b98d-116efb3d3179">ContainsAny</CriteriaOperatorName>
	</SchemeRecord>
	<SchemeRecord>
		<ID ID="0d01306f-6a4b-4a34-83aa-5084c58211d4">52</ID>
		<DataTypeID ID="1921ced1-d743-435c-b2ea-3cdd714a15ff">208e364e-94fe-0d64-bc27-6b4dc12d039c</DataTypeID>
		<DataTypeName ID="0a40e8c0-5b20-43e4-896b-60c24dc4d543">Entry</DataTypeName>
		<CriteriaOperatorID ID="9b3f8652-0edc-49ed-8caf-93343de1090f">14</CriteriaOperatorID>
		<CriteriaOperatorName ID="d52270b7-ab1d-4fbc-b98d-116efb3d3179">InRole</CriteriaOperatorName>
	</SchemeRecord>
	<SchemeRecord>
		<ID ID="0d01306f-6a4b-4a34-83aa-5084c58211d4">53</ID>
		<DataTypeID ID="1921ced1-d743-435c-b2ea-3cdd714a15ff">5d95340a-0f22-0d14-b9d2-a5471f607261</DataTypeID>
		<DataTypeName ID="0a40e8c0-5b20-43e4-896b-60c24dc4d543">Table</DataTypeName>
		<CriteriaOperatorID ID="9b3f8652-0edc-49ed-8caf-93343de1090f">6</CriteriaOperatorID>
		<CriteriaOperatorName ID="d52270b7-ab1d-4fbc-b98d-116efb3d3179">IsNull</CriteriaOperatorName>
	</SchemeRecord>
	<SchemeRecord>
		<ID ID="0d01306f-6a4b-4a34-83aa-5084c58211d4">54</ID>
		<DataTypeID ID="1921ced1-d743-435c-b2ea-3cdd714a15ff">5d95340a-0f22-0d14-b9d2-a5471f607261</DataTypeID>
		<DataTypeName ID="0a40e8c0-5b20-43e4-896b-60c24dc4d543">Table</DataTypeName>
		<CriteriaOperatorID ID="9b3f8652-0edc-49ed-8caf-93343de1090f">7</CriteriaOperatorID>
		<CriteriaOperatorName ID="d52270b7-ab1d-4fbc-b98d-116efb3d3179">IsNotNull</CriteriaOperatorName>
	</SchemeRecord>
	<SchemeRecord>
		<ID ID="0d01306f-6a4b-4a34-83aa-5084c58211d4">55</ID>
		<DataTypeID ID="1921ced1-d743-435c-b2ea-3cdd714a15ff">5d95340a-0f22-0d14-b9d2-a5471f607261</DataTypeID>
		<DataTypeName ID="0a40e8c0-5b20-43e4-896b-60c24dc4d543">Table</DataTypeName>
		<CriteriaOperatorID ID="9b3f8652-0edc-49ed-8caf-93343de1090f">15</CriteriaOperatorID>
		<CriteriaOperatorName ID="d52270b7-ab1d-4fbc-b98d-116efb3d3179">ContainsAll</CriteriaOperatorName>
	</SchemeRecord>
	<SchemeRecord>
		<ID ID="0d01306f-6a4b-4a34-83aa-5084c58211d4">56</ID>
		<DataTypeID ID="1921ced1-d743-435c-b2ea-3cdd714a15ff">5d95340a-0f22-0d14-b9d2-a5471f607261</DataTypeID>
		<DataTypeName ID="0a40e8c0-5b20-43e4-896b-60c24dc4d543">Table</DataTypeName>
		<CriteriaOperatorID ID="9b3f8652-0edc-49ed-8caf-93343de1090f">16</CriteriaOperatorID>
		<CriteriaOperatorName ID="d52270b7-ab1d-4fbc-b98d-116efb3d3179">ContainsAny</CriteriaOperatorName>
	</SchemeRecord>
	<SchemeRecord>
		<ID ID="0d01306f-6a4b-4a34-83aa-5084c58211d4">57</ID>
		<DataTypeID ID="1921ced1-d743-435c-b2ea-3cdd714a15ff">208e364e-94fe-0d64-bc27-6b4dc12d039c</DataTypeID>
		<DataTypeName ID="0a40e8c0-5b20-43e4-896b-60c24dc4d543">Entry</DataTypeName>
		<CriteriaOperatorID ID="9b3f8652-0edc-49ed-8caf-93343de1090f">17</CriteriaOperatorID>
		<CriteriaOperatorName ID="d52270b7-ab1d-4fbc-b98d-116efb3d3179">NotContainsAny</CriteriaOperatorName>
	</SchemeRecord>
	<SchemeRecord>
		<ID ID="0d01306f-6a4b-4a34-83aa-5084c58211d4">58</ID>
		<DataTypeID ID="1921ced1-d743-435c-b2ea-3cdd714a15ff">5d95340a-0f22-0d14-b9d2-a5471f607261</DataTypeID>
		<DataTypeName ID="0a40e8c0-5b20-43e4-896b-60c24dc4d543">Table</DataTypeName>
		<CriteriaOperatorID ID="9b3f8652-0edc-49ed-8caf-93343de1090f">17</CriteriaOperatorID>
		<CriteriaOperatorName ID="d52270b7-ab1d-4fbc-b98d-116efb3d3179">NotContainsAny</CriteriaOperatorName>
	</SchemeRecord>
	<SchemeRecord>
		<ID ID="0d01306f-6a4b-4a34-83aa-5084c58211d4">59</ID>
		<DataTypeID ID="1921ced1-d743-435c-b2ea-3cdd714a15ff">208e364e-94fe-0d64-bc27-6b4dc12d039c</DataTypeID>
		<DataTypeName ID="0a40e8c0-5b20-43e4-896b-60c24dc4d543">Entry</DataTypeName>
		<CriteriaOperatorID ID="9b3f8652-0edc-49ed-8caf-93343de1090f">18</CriteriaOperatorID>
		<CriteriaOperatorName ID="d52270b7-ab1d-4fbc-b98d-116efb3d3179">NotInRole</CriteriaOperatorName>
	</SchemeRecord>
	<SchemeRecord>
		<ID ID="0d01306f-6a4b-4a34-83aa-5084c58211d4">60</ID>
		<DataTypeID ID="1921ced1-d743-435c-b2ea-3cdd714a15ff">1962c931-9c23-0eb9-903c-26bac9ef3571</DataTypeID>
		<DataTypeName ID="0a40e8c0-5b20-43e4-896b-60c24dc4d543">String</DataTypeName>
		<CriteriaOperatorID ID="9b3f8652-0edc-49ed-8caf-93343de1090f">15</CriteriaOperatorID>
		<CriteriaOperatorName ID="d52270b7-ab1d-4fbc-b98d-116efb3d3179">ContainsAll</CriteriaOperatorName>
	</SchemeRecord>
	<SchemeRecord>
		<ID ID="0d01306f-6a4b-4a34-83aa-5084c58211d4">61</ID>
		<DataTypeID ID="1921ced1-d743-435c-b2ea-3cdd714a15ff">1962c931-9c23-0eb9-903c-26bac9ef3571</DataTypeID>
		<DataTypeName ID="0a40e8c0-5b20-43e4-896b-60c24dc4d543">String</DataTypeName>
		<CriteriaOperatorID ID="9b3f8652-0edc-49ed-8caf-93343de1090f">16</CriteriaOperatorID>
		<CriteriaOperatorName ID="d52270b7-ab1d-4fbc-b98d-116efb3d3179">ContainsAny</CriteriaOperatorName>
	</SchemeRecord>
	<SchemeRecord>
		<ID ID="0d01306f-6a4b-4a34-83aa-5084c58211d4">62</ID>
		<DataTypeID ID="1921ced1-d743-435c-b2ea-3cdd714a15ff">1962c931-9c23-0eb9-903c-26bac9ef3571</DataTypeID>
		<DataTypeName ID="0a40e8c0-5b20-43e4-896b-60c24dc4d543">String</DataTypeName>
		<CriteriaOperatorID ID="9b3f8652-0edc-49ed-8caf-93343de1090f">17</CriteriaOperatorID>
		<CriteriaOperatorName ID="d52270b7-ab1d-4fbc-b98d-116efb3d3179">NotContainsAny</CriteriaOperatorName>
	</SchemeRecord>
</SchemeTable>