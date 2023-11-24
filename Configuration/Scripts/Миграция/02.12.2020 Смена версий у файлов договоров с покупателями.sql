/*
для чего скрипт:
имеется интеграция с CRM, по которой в тессу приходят договоры с покупателями
вместе с карточками приходят файлы (сервис /web/PnrService/CreateOrUpdateContract/submitProjectFile), 
которые создаются в тесса

при создании такого файла в CRM возвращается ссылка на файл вида:
.../web/api/filelink
?cardId=28d775c3-ebf9-4fc4-9467-3da6eb67ab66
&fileId=ea2907d3-2130-4149-a3b1-a0ec5c3411a1
&versionId=cf39f092-6d18-4099-8b28-e4ac69011b1b
&cardTypeId=1c7a5718-09ae-4f65-aa67-e66f23bb7aee
&fileName=%D0%9E%D1%81%D1%82%D0%B0%D1%88%D0%BA%D0%BE%D0%9A.%D0%92._1_20201009_182159.txt

так вот в этой ссылке зашит VersionID версии файла

эту ссылку в CRM сохраняют у себя в карточке и используют для загрузки файла из тессы

проблема в следующем: если в ТЕССА пользователь изменит такой файл, пришедший из CRM,
то для файла будет создана новая версия, и соотв-но у нее будет другой RowID версии

поэтому, если в CRM попытаются загрузить файл по первоначальной ссылке, то они загрузят не новую версию, а сумую первую

для решения этой проблемы и создан данный скрипт (решение временное, далее планируется сразу при изменении файла обновлять RowID версий)

этот скрипт определяет все договоры с покупателями, в них - файлы, которые были изменены пользаками
затем для всех таких файлов меняются местами
RowID оригинальной версии файла (той, что зашита в ссылке на файл в CRM) 
и RowID последней актуальной пользовательской версии файла

скрипт написан с учетом того, что его можно запускать периодически, чтобы актуализировать для CRM доступные по ссылке файлы
это реализовано за счет того, что оригинальная версия файла (из ссылки) сохраняется в колонке Files.OriginalVersionRowID

+ также этот скрипт учитывает измененные файлы в договорах, которые были мигрированы одноразово через скрипт миграции договоров

*/

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

-- определим файлы из договоров с покупателями, где есть новая пользовательская версия относительно первичной мигрированной
IF OBJECT_ID('tempdb..#changedFiles') IS NOT NULL DROP TABLE #changedFiles
create table #changedFiles
(
	CardID uniqueidentifier, -- ID договора/дс
	FileID uniqueidentifier, -- ID файла
	OriginalVersionRowID uniqueidentifier, -- RowID версии файла, который был возвращен в CRM в ссылке на файле при интеграции
	ActualVersionRowID uniqueidentifier,  -- RowID актуальной версии файла
	NewVersionRowID uniqueidentifier -- новый RowID, который будет проставлен для первоначальных версий файлов
)

insert into #changedFiles
select
	t.CardID,
	f.RowID,
	-- если у файла больше 2 версий, то тут будет оригинальный RowID
	-- если их 2, то тут будет RowID первой системной версии
	coalesce(f.OriginalVersionRowID, firstVersion.RowID) as OriginalVersionRowID, 
	actualVersion.RowID,
	newid() as NewVersionRowID
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
cross apply -- ищем последнюю пользовательскую версию
(
	select top 1 *
	from FileVersions fv
	where fv.ID = f.RowID
		and fv.Number > 1 -- версия точно не первая
		and fv.RowID = f.VersionRowID -- последняя версия
		and fv.CreatedByID != '11111111-1111-1111-1111-111111111111' -- файл был создан не системой
		and fv.StateID = 1 -- файл успешно загружен
) actualVersion
where f.VersionNumber > 1 -- больше одной версии
and f.CreatedByID = '11111111-1111-1111-1111-111111111111' -- файл был создан системой
and (f.OriginalVersionRowID is null or f.OriginalVersionRowID != f.VersionRowID) -- актуальная вресия отличается от оригинальной

/*
select distinct dci.Subject, dci.CreationDate
from Files f
join DocumentCommonInfo dci on dci.ID = f.ID
--join PnrContracts c on c.ID = f.ID
join #changedFiles cf on f.RowID = cf.FileID
order by dci.CreationDate desc
*/

-- выборка всех найденных для изменения файлов
select dci.FullNumber, dci.CreationDate, f.*, originalVersion.Number, actualVersion.Number, originalVersion.CreatedByName, actualVersion.CreatedByName,
	originalVersion.Created, actualVersion.Created
from Files f
join DocumentCommonInfo dci on dci.ID = f.ID
join #changedFiles cf on f.RowID = cf.FileID
cross apply
(
	select top 1 *
	from FileVersions fv
	where fv.RowID = cf.OriginalVersionRowID
) as originalVersion
cross apply
(
	select top 1 *
	from FileVersions fv
	where fv.RowID = cf.ActualVersionRowID
) as actualVersion
order by f.Modified desc

BEGIN TRANSACTION;  
  
BEGIN TRY  

	-- запомним оригинальную версию, где не запоминали раньше
	update f
	set f.OriginalVersionRowID = cf.OriginalVersionRowID
	--select f.ID, f.RowID, f.Name, f.OriginalVersionRowID, cf.OriginalVersionRowID
	from Files f
	join #changedFiles cf on f.RowID = cf.FileID
	where f.OriginalVersionRowID is null

	-- у оригинальных версий и их контента обновим RowID, чтобы потом задать его для последних версий
	update fv
	set fv.RowID = cf.NewVersionRowID
	--select fv.RowID, cf.NewVersionRowID
	from FileVersions fv
	join #changedFiles cf on fv.RowID = cf.OriginalVersionRowID

	update fc
	set fc.VersionRowID = cf.NewVersionRowID
	--select fc.VersionRowID, cf.NewVersionRowID
	from FileContent fc
	join #changedFiles cf on fc.VersionRowID = cf.OriginalVersionRowID

	-- у последних версий и их контента обновим RowID на освободившийся оригинальный RowID первой версии
	update fv
	set fv.RowID = cf.OriginalVersionRowID
	--select fv.RowID, cf.OriginalVersionRowID
	from FileVersions fv
	join #changedFiles cf on fv.RowID = cf.ActualVersionRowID

	update fc
	set fc.VersionRowID = cf.OriginalVersionRowID
	--select fc.VersionRowID, cf.OriginalVersionRowID
	from FileContent fc
	join #changedFiles cf on fc.VersionRowID = cf.ActualVersionRowID

	-- обновим актуальную версию у основного файла
	--select f.VersionRowID, cf.OriginalVersionRowID
	update f
	set f.VersionRowID = cf.OriginalVersionRowID
	from Files f
	join #changedFiles cf on f.RowID = cf.FileID

END TRY  
BEGIN CATCH  
    SELECT   
        ERROR_NUMBER() AS ErrorNumber  
        ,ERROR_SEVERITY() AS ErrorSeverity  
        ,ERROR_STATE() AS ErrorState  
        ,ERROR_PROCEDURE() AS ErrorProcedure  
        ,ERROR_LINE() AS ErrorLine  
        ,ERROR_MESSAGE() AS ErrorMessage;  
  
    IF @@TRANCOUNT > 0  
        ROLLBACK TRANSACTION;  
END CATCH;  
  
IF @@TRANCOUNT > 0  
    COMMIT TRANSACTION;  
GO  

