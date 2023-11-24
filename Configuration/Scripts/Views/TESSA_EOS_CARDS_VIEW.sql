IF OBJECT_ID('TESSA_EOS_CARDS_VIEW', 'V') IS NOT NULL
    DROP VIEW TESSA_EOS_CARDS_VIEW
GO

-- выборка договоров/дс/договоров УК с GUID EOS
CREATE VIEW TESSA_EOS_CARDS_VIEW
AS

select
	t.ID as GUID_TESSA,
	t.ExtID as GUID_EOS,
	'tessa://tessaclient.tessa/?Action=OpenCard&ID=' + cast(lower(t.ID) as nvarchar(36)) as URL_TESSA
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
where
	ExtID is not null -- ссылка на внешний ID карточки
