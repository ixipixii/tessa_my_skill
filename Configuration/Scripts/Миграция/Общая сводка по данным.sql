

sp_whoisactive
--kill 57


-- сводка по документам
select i.TypeID, i.TypeCaption, count(*)
from Instances i
group by i.TypeID, i.TypeCaption
order by count(*) desc

-- общая сводка по файлам
select i.TypeID, i.TypeCaption, count(*)
-- select sum(fv.Size)/1024/1024
from Files f with(nolock)
join Instances i with(nolock) on i.ID = f.ID
join FileVersions fv with(nolock) on f.RowID = fv.ID
join FileContent fc with(nolock) on fv.RowID = fc.VersionRowID
group by i.TypeID, i.TypeCaption
order by count(*) desc

-- последние загруженные файлы
select top 1000 dci.FullNumber, f.*
from Files f with(nolock)
join DocumentCommonInfo dci with(nolock) on dci.ID = f.ID
join Instances i with(nolock) on i.ID = f.ID
join FileVersions fv with(nolock) on f.RowID = fv.ID
join FileContent fc with(nolock) on fv.RowID = fc.VersionRowID
order by f.Created desc

-- всего файлов во внешней системе по ВХ
SELECT count(*)
FROM [TST-TESSA-DB].[MigrationSED].[dbo].[IncomingContractsTESSA] t WITH(NOLOCK)
join [TST-TESSA-DB].[MigrationSED].[dbo].[Files] as [fi] with(nolock) on t.UniqueId = fi.[DocUniqueId]
WHERE [GroupTypeTESSA] = N'Входящие документы'

-- всего файлов во внешней системе по Договорам
SELECT count(*)
FROM [TST-TESSA-DB].[MigrationSED].[dbo].[ContractsTESSA] t
join [TST-TESSA-DB].[MigrationSED].[dbo].[Files] as [fi] with(nolock) on t.UniqueId = fi.[DocUniqueId]
WHERE [IsContract] = N'True'
AND [GroupTypeTESSA] = N'Договоры'

SELECT count(*)
FROM [TST-TESSA-DB].[MigrationSED].[dbo].[ContractsTESSA] t
join [TST-TESSA-DB].[MigrationSED].[dbo].[RegisterFiles] as [rfi] with(nolock)  on t.UniqueId = [rfi].[DocUniqueId]
WHERE [IsContract] = N'True'
AND [GroupTypeTESSA] = N'Договоры'

-- всего файлов во внешней системе по Договорам КИС
SELECT count(*)
FROM [TST-TESSA-DB].[MigrationSED].[dbo].[ContractsKIS] t
join [TST-TESSA-DB].[MigrationSED].[dbo].[FilesKIS] as [fk] with(nolock) on t.DocUniqueId = [fk].[ID]
WHERE t.UniqueId is not null and fk.Content is not null