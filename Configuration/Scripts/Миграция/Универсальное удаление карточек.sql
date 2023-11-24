-- ПЕРЕД УДАЛЕНИЕМ СДЕЛАТЬ БЭКАП БД
-- Т.К СКРИПТ БЕЗ ТРАНЗАКЦИИ, ТО МОЖЕТ УДАЛИТЬСЯ НЕ ВСЁ, И ОТКАТИТЬ УДАЛЕНИЕ УЖЕ НЕ ПОЛУЧИТСЯ


-- !! тут указывается тип карточки, экземпляры которого нужно удалить
declare @cardTypeID uniqueidentifier = ''

/* ID нужного типа карточки можно глянуть тут

-- сводка по документам
select t.ID, t.Name, dbo.Localize(t.Caption, 25) as TypeCaption, count(*) as Count
from Instances i
join [Types] t on i.TypeID = t.ID
group by t.ID, t.Name, t.Caption
order by t.Name

*/

-- число карточек, которые хотим удалить за раз
declare @CardsDeleteCount int = 5000
-- ИСКЛЮЧЕНИЕ, число карточек, которые обязательно хотим оставить (если нужно)
declare @CardsKeepCount int = 0

declare @cardsToKeep table (ID uniqueidentifier)

IF OBJECT_ID('tempdb..#tempCardsToDelete') IS NULL
BEGIN
	create table #tempCardsToDelete (ID uniqueidentifier)
END 

-- если в #tempCardsToDelete еще остались карточки, то повторно формироваться список для удаления не будет,
-- будет повторная попытка удалить сначала указанные карточки (например при повторном запуске после возникновения ошибок при первом запуске)
declare @tempCardsToDeleteCount int = (select count(*) from #tempCardsToDelete)

if (@tempCardsToDeleteCount = 0)
begin
	-- выберем N самых новых карточек
	;WITH goners AS (
		SELECT ROW_NUMBER() OVER(ORDER BY Created DESC) AS rn, Instances.*
		FROM Instances
		where TypeID = @cardTypeID
	)
	insert into @cardsToKeep (ID)
	select ID
	FROM goners
	WHERE rn <= @CardsKeepCount

	-- выберем N самых старых карточек (кроме N карточек, которые надо оставить)
	;WITH goners AS (
		SELECT ROW_NUMBER() OVER(ORDER BY Created ASC) AS rn, i.*
		FROM Instances i
		-- join PnrContracts t on t.ID = i.ID 
		where i.TypeID = @cardTypeID
			--and i.CreatedByID = '11111111-1111-1111-1111-111111111111' -- System
			--and i.Created = cast('26.11.2020' as datetime) -- дата миграции
			--and t.ProjectDate >= cast('01.09.2020' as date) -- документ создан начиная с 1 сентября
	)
	insert into #tempCardsToDelete (ID)
	select g.ID
	FROM goners g
	left join @cardsToKeep k on g.ID = k.ID
	WHERE rn <= @CardsDeleteCount
		and k.ID is null
end

-- список всех таблиц (достаем из настроек типа карточки - то есть из секций)
declare @tableNames table (TableName nvarchar(256))
insert into @tableNames(TableName)
select t.TableName
from
(
	SELECT  t.ID, t.Name, tbl.Name as TableName
	from [Types] as t
	outer apply t.Definition.nodes('/cardType[1]/*') n(c)
	inner join [Tables] tbl
	on n.c.value('(./@section)', 'uniqueidentifier') = tbl.ID
	where n.c.value('(./@section)', 'uniqueidentifier') is not null
		-- игнорим виртуальные таблицы
		and cast(tbl.Definition as xml).value('(/SchemeTable[1]/@IsVirtual)', 'nvarchar(max)' ) is null
) as t
where t.ID = @cardTypeID
order by t.TableName

-- удалим данные из каждой связанной таблицы карточки
DECLARE @tableName varchar(255)

DECLARE cur cursor for select TableName from @tableNames

OPEN CUR
	FETCH NEXT FROM cur into @tableName

	WHILE @@FETCH_STATUS = 0
		BEGIN
			exec('delete t1 from ' + @tableName + ' t1 join #tempCardsToDelete t2 on t1.ID = t2.ID')
			FETCH NEXT FROM cur into @tableName 
		END
CLOSE cur
DEALLOCATE cur

-- пример: удалим данные в доп. таблицах, которые не указаны в карточке, но имеют внешние ключи
-- пример: delete t1 from PkContracts t1 join #tempCardsToDelete t2 on t1.ID = t2.ID

-- удалим файлы
delete fc 
from Files f 
join FileVersions fv on f.RowID = fv.ID
join FileContent fc on fv.RowID = fc.VersionRowID
join #tempCardsToDelete t2 on f.ID = t2.ID

delete fv 
from Files f 
join FileVersions fv on f.RowID = fv.ID
join #tempCardsToDelete t2 on f.ID = t2.ID

delete f
from Files f 
join #tempCardsToDelete t2 on f.ID = t2.ID

-- удалим задания
delete t2 from Tasks t join 
FdTask t2 on t2.ID = t.RowID join #tempCardsToDelete d on t.ID = d.ID

delete t2 from Tasks t join
FdAdditionalApproval t2 on t2.ID = t.RowID join #tempCardsToDelete d on t.ID = d.ID

delete t2 from Tasks t join
FdAdditionalApprovalInfo t2 on t2.ID = t.RowID join #tempCardsToDelete d on t.ID = d.ID

delete t2 from Tasks t join
FdAdditionalApprovalTaskInfo t2 on t2.ID = t.RowID join #tempCardsToDelete d on t.ID = d.ID

delete t2 from Tasks t join
FdCommentators t2 on t2.ID = t.RowID join #tempCardsToDelete d on t.ID = d.ID

delete t2 from Tasks t join
FdCommentsInfo t2 on t2.ID = t.RowID join #tempCardsToDelete d on t.ID = d.ID

delete t2 from Tasks t join
TaskCommonInfo t2 on t2.ID = t.RowID join #tempCardsToDelete d on t.ID = d.ID

delete t2 from Tasks t join
WfResolutions t2 on t2.ID = t.RowID join #tempCardsToDelete d on t.ID = d.ID

delete t2 from Tasks t join
WfResolutionPerformers t2 on t2.ID = t.RowID join #tempCardsToDelete d on t.ID = d.ID

delete t2 from Tasks t join
PnrAcquaintanceUsers t2 on t2.ID = t.RowID join #tempCardsToDelete d on t.ID = d.ID

delete t from Tasks t join #tempCardsToDelete d on t.ID = d.ID

-- история заданий
delete t from TaskHistory t join #tempCardsToDelete d on t.ID = d.ID
delete t from TaskHistoryGroups t join #tempCardsToDelete d on t.ID = d.ID

-- удалим осн. запись в последний момент
delete t1 from Instances t1 join #tempCardsToDelete t2 on t1.ID = t2.ID

-- удалим из очереди успешно удаленные карточки
delete d
from #tempCardsToDelete d
left join Instances i on i.ID = d.ID
where i.ID is null

-- если в итоге запуска скрипта была ошибка,
-- то её нужно исправить (добавив в скрипт удаление данных из связанной таблицы из ошибки, как правило такое бывает только для заданий)
-- и перезапустить скрипт повторно

--checkpoint -- в БД с простой моделью восстановления бывает полезно, чтобы лог транзакций сильно не рос
