<?xml version="1.0" encoding="utf-8"?>
<SchemeTable Partition="c3b14969-a553-4005-9a07-e9705c15b1e1" ID="705ae655-75cb-4d86-a7e3-4b07377d98d6" Name="FdCriteriaOperator" Group="Fd Criteria">
	<SchemePhysicalColumn ID="d9a8e964-2e01-46de-a1af-b30dcbf4311f" Name="ID" Type="Int16 Not Null" />
	<SchemePhysicalColumn ID="b40eeda3-15e5-40f6-942a-cfeaa7081c5d" Name="Caption" Type="String(128) Not Null" />
	<SchemePhysicalColumn ID="7f656a90-1cb9-43f0-9244-88dce8ad7c8b" Name="Name" Type="String(128) Not Null" />
	<SchemePrimaryKey ID="370acbaa-8665-4f94-9198-d0572397026a" Name="pk_FdCriteriaOperator">
		<SchemeIndexedColumn Column="d9a8e964-2e01-46de-a1af-b30dcbf4311f" />
	</SchemePrimaryKey>
	<SchemeRecord>
		<ID ID="d9a8e964-2e01-46de-a1af-b30dcbf4311f">0</ID>
		<Caption ID="b40eeda3-15e5-40f6-942a-cfeaa7081c5d">Равен</Caption>
		<Name ID="7f656a90-1cb9-43f0-9244-88dce8ad7c8b">Equals</Name>
	</SchemeRecord>
	<SchemeRecord>
		<ID ID="d9a8e964-2e01-46de-a1af-b30dcbf4311f">1</ID>
		<Caption ID="b40eeda3-15e5-40f6-942a-cfeaa7081c5d">Не равен</Caption>
		<Name ID="7f656a90-1cb9-43f0-9244-88dce8ad7c8b">NonEquals</Name>
	</SchemeRecord>
	<SchemeRecord>
		<ID ID="d9a8e964-2e01-46de-a1af-b30dcbf4311f">2</ID>
		<Caption ID="b40eeda3-15e5-40f6-942a-cfeaa7081c5d">Больше чем</Caption>
		<Name ID="7f656a90-1cb9-43f0-9244-88dce8ad7c8b">GreatThan</Name>
	</SchemeRecord>
	<SchemeRecord>
		<ID ID="d9a8e964-2e01-46de-a1af-b30dcbf4311f">3</ID>
		<Caption ID="b40eeda3-15e5-40f6-942a-cfeaa7081c5d">Меньше чем</Caption>
		<Name ID="7f656a90-1cb9-43f0-9244-88dce8ad7c8b">LessThan</Name>
	</SchemeRecord>
	<SchemeRecord>
		<ID ID="d9a8e964-2e01-46de-a1af-b30dcbf4311f">4</ID>
		<Caption ID="b40eeda3-15e5-40f6-942a-cfeaa7081c5d">Больше или равно</Caption>
		<Name ID="7f656a90-1cb9-43f0-9244-88dce8ad7c8b">GreatOrEquals</Name>
	</SchemeRecord>
	<SchemeRecord>
		<ID ID="d9a8e964-2e01-46de-a1af-b30dcbf4311f">5</ID>
		<Caption ID="b40eeda3-15e5-40f6-942a-cfeaa7081c5d">Меньше или равно</Caption>
		<Name ID="7f656a90-1cb9-43f0-9244-88dce8ad7c8b">LessOrEquals</Name>
	</SchemeRecord>
	<SchemeRecord>
		<ID ID="d9a8e964-2e01-46de-a1af-b30dcbf4311f">6</ID>
		<Caption ID="b40eeda3-15e5-40f6-942a-cfeaa7081c5d">Пусто</Caption>
		<Name ID="7f656a90-1cb9-43f0-9244-88dce8ad7c8b">IsNull</Name>
	</SchemeRecord>
	<SchemeRecord>
		<ID ID="d9a8e964-2e01-46de-a1af-b30dcbf4311f">7</ID>
		<Caption ID="b40eeda3-15e5-40f6-942a-cfeaa7081c5d">Не пусто</Caption>
		<Name ID="7f656a90-1cb9-43f0-9244-88dce8ad7c8b">IsNotNull</Name>
	</SchemeRecord>
	<SchemeRecord>
		<ID ID="d9a8e964-2e01-46de-a1af-b30dcbf4311f">8</ID>
		<Caption ID="b40eeda3-15e5-40f6-942a-cfeaa7081c5d">Между</Caption>
		<Name ID="7f656a90-1cb9-43f0-9244-88dce8ad7c8b">Between</Name>
	</SchemeRecord>
	<SchemeRecord>
		<ID ID="d9a8e964-2e01-46de-a1af-b30dcbf4311f">9</ID>
		<Caption ID="b40eeda3-15e5-40f6-942a-cfeaa7081c5d">Содержит</Caption>
		<Name ID="7f656a90-1cb9-43f0-9244-88dce8ad7c8b">Contains</Name>
	</SchemeRecord>
	<SchemeRecord>
		<ID ID="d9a8e964-2e01-46de-a1af-b30dcbf4311f">10</ID>
		<Caption ID="b40eeda3-15e5-40f6-942a-cfeaa7081c5d">Начинается с</Caption>
		<Name ID="7f656a90-1cb9-43f0-9244-88dce8ad7c8b">StartsWith</Name>
	</SchemeRecord>
	<SchemeRecord>
		<ID ID="d9a8e964-2e01-46de-a1af-b30dcbf4311f">11</ID>
		<Caption ID="b40eeda3-15e5-40f6-942a-cfeaa7081c5d">Оканчивается на</Caption>
		<Name ID="7f656a90-1cb9-43f0-9244-88dce8ad7c8b">EndsWith</Name>
	</SchemeRecord>
	<SchemeRecord>
		<ID ID="d9a8e964-2e01-46de-a1af-b30dcbf4311f">12</ID>
		<Caption ID="b40eeda3-15e5-40f6-942a-cfeaa7081c5d">Равен да</Caption>
		<Name ID="7f656a90-1cb9-43f0-9244-88dce8ad7c8b">IsTrue</Name>
	</SchemeRecord>
	<SchemeRecord>
		<ID ID="d9a8e964-2e01-46de-a1af-b30dcbf4311f">13</ID>
		<Caption ID="b40eeda3-15e5-40f6-942a-cfeaa7081c5d">Равен нет</Caption>
		<Name ID="7f656a90-1cb9-43f0-9244-88dce8ad7c8b">IsFalse</Name>
	</SchemeRecord>
	<SchemeRecord>
		<ID ID="d9a8e964-2e01-46de-a1af-b30dcbf4311f">14</ID>
		<Caption ID="b40eeda3-15e5-40f6-942a-cfeaa7081c5d">Входит в роль</Caption>
		<Name ID="7f656a90-1cb9-43f0-9244-88dce8ad7c8b">InRole</Name>
	</SchemeRecord>
	<SchemeRecord>
		<ID ID="d9a8e964-2e01-46de-a1af-b30dcbf4311f">15</ID>
		<Caption ID="b40eeda3-15e5-40f6-942a-cfeaa7081c5d">Содержит все значения</Caption>
		<Name ID="7f656a90-1cb9-43f0-9244-88dce8ad7c8b">ContainsAll</Name>
	</SchemeRecord>
	<SchemeRecord>
		<ID ID="d9a8e964-2e01-46de-a1af-b30dcbf4311f">16</ID>
		<Caption ID="b40eeda3-15e5-40f6-942a-cfeaa7081c5d">Содержит хотя бы одно значение</Caption>
		<Name ID="7f656a90-1cb9-43f0-9244-88dce8ad7c8b">ContainsAny</Name>
	</SchemeRecord>
	<SchemeRecord>
		<ID ID="d9a8e964-2e01-46de-a1af-b30dcbf4311f">17</ID>
		<Caption ID="b40eeda3-15e5-40f6-942a-cfeaa7081c5d">Не содержит ни одно из перечисленных значений</Caption>
		<Name ID="7f656a90-1cb9-43f0-9244-88dce8ad7c8b">NotContainsAny</Name>
	</SchemeRecord>
	<SchemeRecord>
		<ID ID="d9a8e964-2e01-46de-a1af-b30dcbf4311f">18</ID>
		<Caption ID="b40eeda3-15e5-40f6-942a-cfeaa7081c5d">Не входит в роль</Caption>
		<Name ID="7f656a90-1cb9-43f0-9244-88dce8ad7c8b">NotInRole</Name>
	</SchemeRecord>
</SchemeTable>