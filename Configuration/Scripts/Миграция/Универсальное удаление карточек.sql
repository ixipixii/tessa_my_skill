-- ����� ��������� ������� ����� ��
-- �.� ������ ��� ����������, �� ����� ��������� �� �Ѩ, � �������� �������� ��� �� ���������


-- !! ��� ����������� ��� ��������, ���������� �������� ����� �������
declare @cardTypeID uniqueidentifier = ''

/* ID ������� ���� �������� ����� ������� ���

-- ������ �� ����������
select t.ID, t.Name, dbo.Localize(t.Caption, 25) as TypeCaption, count(*) as Count
from Instances i
join [Types] t on i.TypeID = t.ID
group by t.ID, t.Name, t.Caption
order by t.Name

*/

-- ����� ��������, ������� ����� ������� �� ���
declare @CardsDeleteCount int = 5000
-- ����������, ����� ��������, ������� ����������� ����� �������� (���� �����)
declare @CardsKeepCount int = 0

declare @cardsToKeep table (ID uniqueidentifier)

IF OBJECT_ID('tempdb..#tempCardsToDelete') IS NULL
BEGIN
	create table #tempCardsToDelete (ID uniqueidentifier)
END 

-- ���� � #tempCardsToDelete ��� �������� ��������, �� �������� ������������� ������ ��� �������� �� �����,
-- ����� ��������� ������� ������� ������� ��������� �������� (�������� ��� ��������� ������� ����� ������������� ������ ��� ������ �������)
declare @tempCardsToDeleteCount int = (select count(*) from #tempCardsToDelete)

if (@tempCardsToDeleteCount = 0)
begin
	-- ������� N ����� ����� ��������
	;WITH goners AS (
		SELECT ROW_NUMBER() OVER(ORDER BY Created DESC) AS rn, Instances.*
		FROM Instances
		where TypeID = @cardTypeID
	)
	insert into @cardsToKeep (ID)
	select ID
	FROM goners
	WHERE rn <= @CardsKeepCount

	-- ������� N ����� ������ �������� (����� N ��������, ������� ���� ��������)
	;WITH goners AS (
		SELECT ROW_NUMBER() OVER(ORDER BY Created ASC) AS rn, i.*
		FROM Instances i
		-- join PnrContracts t on t.ID = i.ID 
		where i.TypeID = @cardTypeID
			--and i.CreatedByID = '11111111-1111-1111-1111-111111111111' -- System
			--and i.Created = cast('26.11.2020' as datetime) -- ���� ��������
			--and t.ProjectDate >= cast('01.09.2020' as date) -- �������� ������ ������� � 1 ��������
	)
	insert into #tempCardsToDelete (ID)
	select g.ID
	FROM goners g
	left join @cardsToKeep k on g.ID = k.ID
	WHERE rn <= @CardsDeleteCount
		and k.ID is null
end

-- ������ ���� ������ (������� �� �������� ���� �������� - �� ���� �� ������)
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
		-- ������� ����������� �������
		and cast(tbl.Definition as xml).value('(/SchemeTable[1]/@IsVirtual)', 'nvarchar(max)' ) is null
) as t
where t.ID = @cardTypeID
order by t.TableName

-- ������ ������ �� ������ ��������� ������� ��������
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

-- ������: ������ ������ � ���. ��������, ������� �� ������� � ��������, �� ����� ������� �����
-- ������: delete t1 from PkContracts t1 join #tempCardsToDelete t2 on t1.ID = t2.ID

-- ������ �����
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

-- ������ �������
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

-- ������� �������
delete t from TaskHistory t join #tempCardsToDelete d on t.ID = d.ID
delete t from TaskHistoryGroups t join #tempCardsToDelete d on t.ID = d.ID

-- ������ ���. ������ � ��������� ������
delete t1 from Instances t1 join #tempCardsToDelete t2 on t1.ID = t2.ID

-- ������ �� ������� ������� ��������� ��������
delete d
from #tempCardsToDelete d
left join Instances i on i.ID = d.ID
where i.ID is null

-- ���� � ����� ������� ������� ���� ������,
-- �� � ����� ��������� (������� � ������ �������� ������ �� ��������� ������� �� ������, ��� ������� ����� ������ ������ ��� �������)
-- � ������������� ������ ��������

--checkpoint -- � �� � ������� ������� �������������� ������ �������, ����� ��� ���������� ������ �� ���
