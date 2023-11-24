/*
��� ���� ������:
������� ���������� � CRM, �� ������� � ����� �������� �������� � ������������
������ � ���������� �������� ����� (������ /web/PnrService/CreateOrUpdateContract/submitProjectFile), 
������� ��������� � �����

��� �������� ������ ����� � CRM ������������ ������ �� ���� ����:
.../web/api/filelink
?cardId=28d775c3-ebf9-4fc4-9467-3da6eb67ab66
&fileId=ea2907d3-2130-4149-a3b1-a0ec5c3411a1
&versionId=cf39f092-6d18-4099-8b28-e4ac69011b1b
&cardTypeId=1c7a5718-09ae-4f65-aa67-e66f23bb7aee
&fileName=%D0%9E%D1%81%D1%82%D0%B0%D1%88%D0%BA%D0%BE%D0%9A.%D0%92._1_20201009_182159.txt

��� ��� � ���� ������ ����� VersionID ������ �����

��� ������ � CRM ��������� � ���� � �������� � ���������� ��� �������� ����� �� �����

�������� � ���������: ���� � ����� ������������ ������� ����� ����, ��������� �� CRM,
�� ��� ����� ����� ������� ����� ������, � �����-�� � ��� ����� ������ RowID ������

�������, ���� � CRM ���������� ��������� ���� �� �������������� ������, �� ��� �������� �� ����� ������, � ����� ������

��� ������� ���� �������� � ������ ������ ������ (������� ���������, ����� ����������� ����� ��� ��������� ����� ��������� RowID ������)

���� ������ ���������� ��� �������� � ������������, � ��� - �����, ������� ���� �������� ����������
����� ��� ���� ����� ������ �������� �������
RowID ������������ ������ ����� (���, ��� ������ � ������ �� ���� � CRM) 
� RowID ��������� ���������� ���������������� ������ �����

������ ������� � ������ ����, ��� ��� ����� ��������� ������������, ����� ��������������� ��� CRM ��������� �� ������ �����
��� ����������� �� ���� ����, ��� ������������ ������ ����� (�� ������) ����������� � ������� Files.OriginalVersionRowID

+ ����� ���� ������ ��������� ���������� ����� � ���������, ������� ���� ����������� ���������� ����� ������ �������� ���������

*/

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

-- ��������� ����� �� ��������� � ������������, ��� ���� ����� ���������������� ������ ������������ ��������� �������������
IF OBJECT_ID('tempdb..#changedFiles') IS NOT NULL DROP TABLE #changedFiles
create table #changedFiles
(
	CardID uniqueidentifier, -- ID ��������/��
	FileID uniqueidentifier, -- ID �����
	OriginalVersionRowID uniqueidentifier, -- RowID ������ �����, ������� ��� ��������� � CRM � ������ �� ����� ��� ����������
	ActualVersionRowID uniqueidentifier,  -- RowID ���������� ������ �����
	NewVersionRowID uniqueidentifier -- ����� RowID, ������� ����� ���������� ��� �������������� ������ ������
)

insert into #changedFiles
select
	t.CardID,
	f.RowID,
	-- ���� � ����� ������ 2 ������, �� ��� ����� ������������ RowID
	-- ���� �� 2, �� ��� ����� RowID ������ ��������� ������
	coalesce(f.OriginalVersionRowID, firstVersion.RowID) as OriginalVersionRowID, 
	actualVersion.RowID,
	newid() as NewVersionRowID
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
cross apply -- ���� ��������� ���������������� ������
(
	select top 1 *
	from FileVersions fv
	where fv.ID = f.RowID
		and fv.Number > 1 -- ������ ����� �� ������
		and fv.RowID = f.VersionRowID -- ��������� ������
		and fv.CreatedByID != '11111111-1111-1111-1111-111111111111' -- ���� ��� ������ �� ��������
		and fv.StateID = 1 -- ���� ������� ��������
) actualVersion
where f.VersionNumber > 1 -- ������ ����� ������
and f.CreatedByID = '11111111-1111-1111-1111-111111111111' -- ���� ��� ������ ��������
and (f.OriginalVersionRowID is null or f.OriginalVersionRowID != f.VersionRowID) -- ���������� ������ ���������� �� ������������

/*
select distinct dci.Subject, dci.CreationDate
from Files f
join DocumentCommonInfo dci on dci.ID = f.ID
--join PnrContracts c on c.ID = f.ID
join #changedFiles cf on f.RowID = cf.FileID
order by dci.CreationDate desc
*/

-- ������� ���� ��������� ��� ��������� ������
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

	-- �������� ������������ ������, ��� �� ���������� ������
	update f
	set f.OriginalVersionRowID = cf.OriginalVersionRowID
	--select f.ID, f.RowID, f.Name, f.OriginalVersionRowID, cf.OriginalVersionRowID
	from Files f
	join #changedFiles cf on f.RowID = cf.FileID
	where f.OriginalVersionRowID is null

	-- � ������������ ������ � �� �������� ������� RowID, ����� ����� ������ ��� ��� ��������� ������
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

	-- � ��������� ������ � �� �������� ������� RowID �� �������������� ������������ RowID ������ ������
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

	-- ������� ���������� ������ � ��������� �����
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

