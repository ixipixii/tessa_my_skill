-- ������ ������� ����� �� ��������� � ������������, ������� ������ ����� ��������/����������,
-- � ��� ��� �� ��������� ������������ ������ (� ���������� ���������� ��� �� �����������, �� ������ ������ ����������� ���������)

IF OBJECT_ID('tempdb..#cardIDs') IS NOT NULL DROP TABLE #cardIDs
create table #cardIDs (CardID uniqueidentifier)

--�������� � �� � ������������
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

-- ��������� ����� �� ��������� � ������������, ��� �� ����������� ������ �� ������������ ������
IF OBJECT_ID('tempdb..#FilesWithoutOriginalVersion') IS NOT NULL DROP TABLE #FilesWithoutOriginalVersion
create table #FilesWithoutOriginalVersion
(
	CardID uniqueidentifier, -- ID ��������/��
	FileID uniqueidentifier, -- ID �����
	OriginalVersionRowID uniqueidentifier, -- RowID ������ �����, ������� ��� ��������� � CRM � ������ �� ���� ��� ����������
)

insert into #FilesWithoutOriginalVersion
select
	t.CardID,
	f.RowID,
	firstVersion.RowID as OriginalVersionRowID
from #cardIDs t
join Files f on f.ID = t.CardID
cross apply -- ���� ������ ��������� ������, ������� ���� ������� ��� ����������
(
	select top 1 *
	from FileVersions fv
	where fv.ID = f.RowID
		and fv.Number = 1 -- ������ ������
		and fv.CreatedByID = '11111111-1111-1111-1111-111111111111' -- ���� ��� ������ ��������
		and fv.StateID = 1 -- ���� ������� ��������
) firstVersion
where f.CreatedByID = '11111111-1111-1111-1111-111111111111' -- ���� ��� ������ ��������
	and f.OriginalVersionRowID is null -- ������������ ������ �� ���� ��� ���������

-- ����� ������
select count(*)
from #cardIDs t
join Files f on f.ID = t.CardID
where f.CreatedByID = '11111111-1111-1111-1111-111111111111'

-- ������, � ������� ������������ ������ ��� ������
select count(*)
from #cardIDs t
join Files f on f.ID = t.CardID
where f.CreatedByID = '11111111-1111-1111-1111-111111111111' and f.OriginalVersionRowID is not null

-- ������, � ������� ���� ������ ������������ ������
select count(*)
from #FilesWithoutOriginalVersion t

-- ������� ����� �����
select count(*)
--update f set f.OriginalVersionRowID = t.OriginalVersionRowID
from Files f
join #FilesWithoutOriginalVersion t on f.RowID = t.FileID
where f.OriginalVersionRowID is null or f.OriginalVersionRowID != t.OriginalVersionRowID