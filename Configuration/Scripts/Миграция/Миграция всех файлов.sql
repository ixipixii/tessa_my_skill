-- скрипт догружает связанные с карточками файлы порциями (только те, которые еще не были перенесены)
-- ЕСЛИ СКРИПТ БЫЛ ПРЕРВАН, НУЖНО НЕ ЗАБЫТЬ ОТМЕНИТЬ ТРАНЗАКЦИЮ
-- ROLLBACK TRANSACTION;


/* В БД миграции в таблице Files сздать Индексы для более быстрой миграции

CREATE NONCLUSTERED INDEX [ix_Files_DocUniqueId] ON [dbo].[Files]
(
	[DocUniqueId] ASC
)
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = OFF, ALLOW_PAGE_LOCKS = OFF) ON [PRIMARY]

CREATE NONCLUSTERED INDEX [ix_Files_UniqueId_DocUniqueId] ON [dbo].[Files]
(
	[UniqueId] ASC,
	[DocUniqueId] ASC
)
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = OFF, ALLOW_PAGE_LOCKS = OFF) ON [PRIMARY]

CREATE NONCLUSTERED INDEX [ix_Files_ID] ON [dbo].[FilesKIS]
(
	[ID] ASC
)
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = OFF, ALLOW_PAGE_LOCKS = OFF) ON [PRIMARY]

также добавить колонку Size INT NULL,  и заполнить:

update Files
set Size = DATALENGTH(Content)
where Size is null

update RegisterFiles
set Size = DATALENGTH(Content)
where Size is null

update FilesKIS
set Size = DATALENGTH(Content)
where Size is null

*/

SET NOCOUNT ON

DROP TABLE IF EXISTS #CardExtIDs
-- в [Priority] задается приоритет загрузки файлов по каждому типу карточки
create table #CardExtIDs (ID uniqueidentifier NOT NULL, ExtID uniqueidentifier NOT NULL, [Priority] int NOT NULL)
create nonclustered index ix_ID on #CardExtIDs(ID)
create nonclustered index ix_ExtID on #CardExtIDs(ExtID)
create nonclustered index ix_ID_ExtID on #CardExtIDs(ID, ExtID)

-- соберем вместе все внешние ID карточек в системе
insert into #CardExtIDs
select distinct t.ID, ExtID, [Priority]
from
(
	-- пример: только договоры за ноябрь-декабрь 2019
	--select ID, ExtID, -1 as [Priority] from [PnrContracts]
	--where ProjectDate >= cast('01.11.2019' as date) and ProjectDate < cast('01.01.2020' as date)
	--union
	-- все договоры
	select ID, ExtID, 0 as [Priority] from [PnrContracts]
	union
	-- ДС
	select ID, ExtID, 1 from [PnrSupplementaryAgreements]
	union
	-- ИСХ
	select ID, ExtID, 2 from [PnrOutgoing]
	union
	-- ВХ
	select ID, ExtID, 3 from [PnrIncoming]
	union
	-- Приказы
	select ID, ExtID, 4 from [PnrOrder]
	union
	-- Доверенности
	select ID, ExtID, 5 from [PnrPowerAttorney]
	union
	-- Шаблоны
	select ID, ExtID, 6 from [PnrTemplates]
	union
	-- договоры УК
	select ID, ExtID, 7 from [PnrContractsUK]
	union
	-- ИСХ УК
	select ID, ExtID, 8 from [PnrOutgoingUK]
	union
	-- ВХ УК
	select ID, ExtID, 9 from [PnrIncomingUK]
	union
	-- Приказы УК
	select ID, ExtID, 10 from [PnrOrderUK]
	--union
	---- Архив КИС
	--select ID, ExtID, 11 as [Priority] from [PnrArchiveKIS]
) t
join Instances i on t.ID = i.ID
where t.ExtID is not null -- у карточки должен быть задан внешний ID

-- временная таблица, куда сначала перенесем инфу о еще незагруженных файлах
DROP TABLE IF EXISTS #AllFilesToUpdate
CREATE TABLE #AllFilesToUpdate(
	[CardID] [uniqueidentifier] NOT NULL,
	[CardExternalID] [uniqueidentifier] NOT NULL, -- внешний ID карточки
	[FileRowID] [uniqueidentifier] NOT NULL,
	[FileName] nvarchar(max) NOT NULL,
	[FileVersionRowID] [uniqueidentifier] NOT NULL,
	[FileExternalID] [uniqueidentifier] NOT NULL, -- внешний ID файла
	[Size] bigint NOT NULL,
	[SourceTable] int NOT NULL, -- 0 = [Files], 1 = [RegisterFiles]
	[FileExt] nvarchar(100) null,
	[Order] int not null, -- порядок, по которому будут грузиться файлы
	[IsLoading] bit not null -- 1, если файл будет загружен в текущей итерации
)
create nonclustered index ix_CardExternalID_FileExternalID on #AllFilesToUpdate([CardExternalID], [FileExternalID])
create nonclustered index ix_Order on #AllFilesToUpdate([Order])
create nonclustered index ix_IsLoading on #AllFilesToUpdate([IsLoading])
	
-- определим список файлов для миграции
INSERT INTO #AllFilesToUpdate ([CardID], [CardExternalID], [FileRowID], [FileName], [FileVersionRowID], [FileExternalID], [Size], [SourceTable], [FileExt], [Order], [IsLoading])
SELECT
	[t].[ID]			as [CardID],
	[fi].[DocUniqueId]	as [CardExternalID],
	newid()				as [FileRowID],
	[fi].[Name]			as [FileName],
	newid()				as [FileVersionRowID],
	[fi].[UniqueId]		as [FileExternalID],
	[fi].[Size]			as [Size],
	[fi].[SourceTable]	as [SourceTable],
	REVERSE(SUBSTRING(REVERSE([fi].[Name]), 0, CHARINDEX('.',REVERSE([fi].[Name])))) as [FileExt],
	ROW_NUMBER() OVER (ORDER BY [t].[Priority])	as [Order], -- расчитаем порядок по приоритету
	0					as [IsLoading]
FROM #CardExtIDs t
cross apply
(
	-- обычные файлы
	select distinct [fi].[DocUniqueId], [fi].[UniqueId], [fi].[Name], [fi].[Size], 0 as SourceTable
	from [TST-TESSA-DB].[MigrationSED].[dbo].[Files] as [fi] with(nolock)
	where [fi].[DocUniqueId] = [t].[ExtID]
	union
	-- листы согласования
	select distinct [rfi].[DocUniqueId], [rfi].[UniqueId], [rfi].[Name], [rfi].[Size], 1 as SourceTable
	from [TST-TESSA-DB].[MigrationSED].[dbo].[RegisterFiles] as [rfi] with(nolock)
	where [rfi].[DocUniqueId] = [t].[ExtID]
	--union
	---- файлы КИС (тут какая-то путаница, в ТЕССЕ в ExtID запоминается UniqueId, но файлы связываются по DocUniqueId
	--select distinct [ck].[UniqueId] as [DocUniqueId], [fk].[ID] as [UniqueId], [fk].[FileName] COLLATE Cyrillic_General_CI_AS as [Name], [fk].[Size], 2 as SourceTable
	--from [TST-TESSA-DB].[MigrationSED].[dbo].[FilesKIS] as [fk] with(nolock)
	--join [TST-TESSA-DB].[MigrationSED].[dbo].[ContractsKIS] as [ck] with(nolock) on [ck].[DocUniqueId] = [fk].[ID]
	--where [ck].[UniqueId] = [t].[ExtID]
	--	and [fk].[Content] is not null -- в файлах КИС есть файлы с пустым контентом
) [fi]
where
	not exists -- не будем учитывать файлы, которые уже есть в ТЕССА
	(
		select top 1 1
		from [Files] [f] with(nolock)
		where [f].[ID] = [t].[ID]
			-- проверим явно по внешнему ID
			and [f].[OriginalFileID] = [fi].[UniqueId]
	)

-- выведем общий список файлов, которые будут загружены
select *
from #AllFilesToUpdate t
order by t.[Order]

DECLARE @CreatedBy UNIQUEIDENTIFIER = '11111111-1111-1111-1111-111111111111' -- System
DECLARE @CreatedByName NVARCHAR(128) = (SELECT TOP 1 [Name] FROM PersonalRoles WITH (NOLOCK) WHERE ID = @CreatedBy)
			
DECLARE @FileTypeID UNIQUEIDENTIFIER = 'AB387C69-FD62-0655-BBC3-B879E433A143';
DECLARE @FileTypeCaption NVARCHAR(128) = (SELECT TOP 1 Caption FROM [Types] WITH (NOLOCK) WHERE ID = @FileTypeID)

-- общее число файлов для загрузки
declare @filesTotalCount int = (select count(*) from #AllFilesToUpdate t)
-- общий размер файлов для загрузки в МБ
declare @filesTotalMB bigint = (select sum(t.Size) from #AllFilesToUpdate t) / 1024 / 1024
-- оставшийся размер для загрузки в МБ
declare @filesLeftMB bigint = @filesTotalMB
-- число загруженных файлов
declare @filesLoadedCount int = 0
-- загружено файлов в последнее итерации
declare @lastLoadedFilesCount int = -1
-- время начала загрузки
declare @startDate datetime = getdate()

-- число загружаемых файлов за одну итерацию
declare @filesLimitPerIteration int = 100

print(N'Общее число файлов для загрузки: ' + cast(@filesTotalCount as nvarchar) + N', Общий размер: ' + cast(@filesTotalMB as nvarchar) + N' MB')

-- в цикле будем мигрировать файлы порциями, плюс в том, что можно остановить скрипт и сохранить при этом мигрированные файлы
-- но в таком случае надо вручную откатить транзакцию: ROLLBACK TRANSACTION;
while (@lastLoadedFilesCount != 0)
begin

	if (@filesLoadedCount > 0)
	begin
		-- оставшийся объем файлов в МБ
		set @filesLeftMB = (select isnull(sum(t.Size), 0) from #AllFilesToUpdate t) / 1024 / 1024

		-- загружено в МБ
		declare @filesLoadedMB bigint = @filesTotalMB - @filesLeftMB

		print N'Файлов загружено: ' + cast(@filesLoadedCount as nvarchar) + N'/' + cast(@filesTotalCount as nvarchar) + N', ' 
			+ cast(@filesLoadedMB as nvarchar) + N' MB/' + cast(@filesTotalMB as nvarchar) + N' MB'

		-- потраченное время в сек
		declare @spentSec int = datediff(SECOND, @startDate, getdate())

		if (@spentSec > 0
			and @filesLoadedMB > 0
			and @filesLoadedCount < @filesTotalCount )
		begin
			-- средняя скорость загрузки MB/msec
			declare @avgLoadSpeed float = cast(@filesLoadedMB as float) / @spentSec

			-- подсчитаем примерное оставшееся время = оставшийся объем / средняя скорость загрузки
			declare @leftSec bigint = @filesLeftMB / @avgLoadSpeed
			declare @leftTime nvarchar(128) = CONVERT(nvarchar, DATEADD(SECOND, @leftSec, 0), 114)

			print N'Осталось времени: ' + cast(@leftTime as nvarchar)
		end
		RAISERROR(N'', 0, 1) WITH NOWAIT -- без этого сообщение из print не будет нормально отображаться в цикле
	end
	
	BEGIN TRANSACTION;

	BEGIN TRY

		-- отметим файлы, которые будут грузиться в текущей итерации
		with cte as (
		   select top (@filesLimitPerIteration) -- берем только часть файлов
			   t.IsLoading
		   from #AllFilesToUpdate t
		   order by t.[Order] -- файлы берем по порядку
		)
		update cte
		set Isloading = 1 -- далее по этому признаку будем определять, какие файлы надо загрузить в текущей итерации

		-- создание файлов
		INSERT INTO [Files] (
			[ID]
			,[RowID]
			,[Name]
			,[Created]
			,[Modified]
			,[TypeID]
			,[TypeCaption]
			,[CreatedByID]
			,[CreatedByName]
			,[ModifiedByID]
			,[ModifiedByName]
			,[VersionRowID]
			,[VersionNumber]
			,[OriginalFileID]) -- тут запомним внешний ID файла
		SELECT
			t.[CardID]
			,[t].[FileRowID]
			,[t].[FileName]
			,GETUTCDATE()			
			,GETUTCDATE()
			,@FileTypeID
			,@FileTypeCaption
			,@CreatedBy
			,@CreatedByName
			,@CreatedBy
			,@CreatedByName
			,[t].[FileVersionRowID]
			,1
			,[t].[FileExternalID]
		FROM #AllFilesToUpdate t
		where t.[IsLoading] = 1

		-- создание версий файлов
		INSERT INTO [FileVersions] (
			[ID]
			,[RowID]
			,[Number]
			,[Name]
			,[Size]
			,[Created]
			,[CreatedByID]
			,[CreatedByName]
			,[SourceID]
			,[StateID]
			)
		SELECT
			[t].[FileRowID]
			,[t].[FileVersionRowID]
			,1
			,[t].[FileName]
			,[t].[Size]
			,GETUTCDATE()
			,@CreatedBy
			,@CreatedByName
			,1
			,1
		FROM #AllFilesToUpdate t
		where t.[IsLoading] = 1

		-- создание контента файлов
		INSERT INTO [FileContent] ([VersionRowID], [Content], [Ext])
		SELECT [t].[FileVersionRowID], [fi].[Content], [t].[FileExt]
		FROM #AllFilesToUpdate t
		cross apply
		(
			-- обычные файлы
			select top 1 [fi].[Content]
			from [TST-TESSA-DB].[MigrationSED].[dbo].[Files] as [fi]
			where [t].[SourceTable] = 0
				and [fi].[DocUniqueId] = [t].[CardExternalID]
				and [fi].[UniqueId] = [t].[FileExternalID]
			union
			-- листы согласования
			select top 1 [rfi].[Content]
			from [TST-TESSA-DB].[MigrationSED].[dbo].[RegisterFiles] as [rfi]
			where [t].[SourceTable] = 1
				and [rfi].[DocUniqueId] = [t].[CardExternalID]
				and [rfi].[UniqueId] = [t].[FileExternalID]
			--union
			---- файлы КИС
			--select top 1 [fk].[Content]
			--from [TST-TESSA-DB].[MigrationSED].[dbo].[FilesKIS] as [fk]
			--where [t].[SourceTable] = 2
			--	and [fk].[ID] = [t].[FileExternalID]
		) [fi]
		where t.[IsLoading] = 1

		-- число только что загруженных файлов
		set @lastLoadedFilesCount = @@ROWCOUNT

		-- обновим число загруженных файлов
		set @filesLoadedCount = @filesLoadedCount + @lastLoadedFilesCount

		-- удалим из общей очереди только что загруженные файлы
		DELETE t
		FROM #AllFilesToUpdate t
		where t.[IsLoading] = 1
	
	END TRY
	BEGIN CATCH
		IF @@TRANCOUNT > 0
			ROLLBACK TRANSACTION;

			DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
			DECLARE @ErrorSeverity INT = ERROR_SEVERITY();
			DECLARE @ErrorState INT = ERROR_STATE();
			RAISERROR (@ErrorMessage, @ErrorSeverity, @ErrorState);
	END CATCH;

	IF @@TRANCOUNT > 0
		COMMIT TRANSACTION;
end

DROP TABLE IF EXISTS #FilesToUpdate
DROP TABLE IF EXISTS #CardExtIDs