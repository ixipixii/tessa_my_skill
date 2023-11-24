-- скрипт находит файлы по договорам с покупателями, которые пришли через миграцию/интеграцию,
-- и где еще не сохранена оригинальная версия (в интеграции изначально это не сохранялось, но теперь должно сохрнаяться автоматом)

IF OBJECT_ID('tempdb..#cardIDs') IS NOT NULL DROP TABLE #cardIDs
create table #cardIDs (CardID uniqueidentifier)

--договоры и ДС с покупателями
insert into #cardIDs
select * from
(
	select ID
	from PnrContracts t
	where t.KindID = '7ede7958-e642-490c-b458-32c034ccb9d6'
	union
	select ID
	from PnrSupplementaryAgreements t
	where t.KindID = '7ede7958-e642-490c-b458-32c034ccb9d6'
) t

-- определим файлы из договоров с покупателями, где не проставлена ссылка на оригинальную версию
IF OBJECT_ID('tempdb..#FilesWithoutOriginalVersion') IS NOT NULL DROP TABLE #FilesWithoutOriginalVersion
create table #FilesWithoutOriginalVersion
(
	CardID uniqueidentifier, -- ID договора/дс
	FileID uniqueidentifier, -- ID файла
	OriginalVersionRowID uniqueidentifier, -- RowID версии файла, который был возвращен в CRM в ссылке на файл при интеграции
)

insert into #FilesWithoutOriginalVersion
select
	t.CardID,
	f.RowID,
	firstVersion.RowID as OriginalVersionRowID
from #cardIDs t
join Files f on f.ID = t.CardID
cross apply -- ищем первую системную версию, которая была создана при интеграции
(
	select top 1 *
	from FileVersions fv
	where fv.ID = f.RowID
		and fv.Number = 1 -- первая версия
		and fv.CreatedByID = '11111111-1111-1111-1111-111111111111' -- файл был создан системой
		and fv.StateID = 1 -- файл успешно загружен
) firstVersion
where f.CreatedByID = '11111111-1111-1111-1111-111111111111' -- файл был создан системой
	and f.OriginalVersionRowID is null -- оригинальная версия не была еще сохранена

-- всего файлов
select count(*)
from #cardIDs t
join Files f on f.ID = t.CardID
where f.CreatedByID = '11111111-1111-1111-1111-111111111111'

-- файлов, у которых оригинальная версия уже задана
select count(*)
from #cardIDs t
join Files f on f.ID = t.CardID
where f.CreatedByID = '11111111-1111-1111-1111-111111111111' and f.OriginalVersionRowID is not null

-- файлов, у которых надо задать оригинальную версию
select count(*)
from #FilesWithoutOriginalVersion t

-- обновим такие файлы
select count(*)
--update f set f.OriginalVersionRowID = t.OriginalVersionRowID
from Files f
join #FilesWithoutOriginalVersion t on f.RowID = t.FileID
where f.OriginalVersionRowID is null or f.OriginalVersionRowID != t.OriginalVersionRowID