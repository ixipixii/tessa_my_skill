IF OBJECT_ID('TESSA_EOS_FILES_VIEW', 'V') IS NOT NULL
    DROP VIEW TESSA_EOS_FILES_VIEW
GO

-- выборка файлов договоров/дс/договоров УК с GUID EOS
CREATE VIEW TESSA_EOS_FILES_VIEW
AS

select
	t.ID as CARD_GUID_TESSA,
	t.ExtID as CARD_GUID_EOS,
	f.RowID as FILE_GUID_TESSA,
	f.OriginalFileID as FILE_GUID_EOS,
	si.WebAddress
		+ N'api/filelink?cardId=' + cast(t.ID as nvarchar(36))
		+ N'&fileId=' + cast(f.RowID as nvarchar(36))
		+ N'&versionId=' + cast(f.VersionRowID as nvarchar(36))
		+ N'&cardTypeId=' + cast(i.TypeID as nvarchar(36))
		+ N'&fileName=' + f.Name
	as URL_TESSA
from
(
-- Договоры
select t.ID, t.ExtID
from PnrContracts t

union

-- Доп. соглашения
select t.ID, t.ExtID
from PnrSupplementaryAgreements t

union

-- Договоры УК
select t.ID, t.ExtID
from PnrContractsUK t
) t
join Files f with(nolock) on t.ID = f.ID
join Instances i with(nolock) on f.ID = i.ID
cross apply (select top 1 si.WebAddress from ServerInstances si) as si
where t.ExtID is not null -- ссылка на внешний ID карточки
	and f.OriginalFileID is not null -- ссылка на внешний ID файла
