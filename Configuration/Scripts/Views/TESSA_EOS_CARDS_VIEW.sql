IF OBJECT_ID('TESSA_EOS_CARDS_VIEW', 'V') IS NOT NULL
    DROP VIEW TESSA_EOS_CARDS_VIEW
GO

-- ������� ���������/��/��������� �� � GUID EOS
CREATE VIEW TESSA_EOS_CARDS_VIEW
AS

select
	t.ID as GUID_TESSA,
	t.ExtID as GUID_EOS,
	'tessa://tessaclient.tessa/?Action=OpenCard&ID=' + cast(lower(t.ID) as nvarchar(36)) as URL_TESSA
from
(
-- ��������
select t.ID, t.ExtID
from PnrContracts t

union

-- ���. ����������
select t.ID, t.ExtID
from PnrSupplementaryAgreements t

union

-- �������� ��
select t.ID, t.ExtID
from PnrContractsUK t
) t
where
	ExtID is not null -- ������ �� ������� ID ��������
