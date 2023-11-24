-- ��������� ������ �� ������������ �������� � ��������� � ��

BEGIN TRANSACTION;

BEGIN TRY		
		

	declare @refTable table (ID uniqueidentifier, ParentID uniqueidentifier, ParentDescription nvarchar(250))

	-- ��������� ��� �������� � ��, ������� ����� ������ �� �������� � ��������
	-- � ������� ���������� � �����
	insert into @refTable (ID, ParentID, ParentDescription)
	select coalesce(destContract.ID, destSA.ID), parent.ID, coalesce (srcParent.DocNumber, srcParent.DocDisplay)
	FROM [MigrationSED].[dbo].[ContractsTESSA] src
	join [MigrationSED].[dbo].[ContractsTESSA] srcParent on src.ParentUniqueId = srcParent.UniqueId
	join [PnrContracts] parent on parent.ExtID = src.ParentUniqueId
	left join [PnrContracts] destContract on src.UniqueId = destContract.ExtID
	left join [PnrSupplementaryAgreements] destSA on src.UniqueId = destSA.ExtID
	where
		destContract.ID is not null	-- ������� � ��������
		or destSA.ID is not null	-- �� � ��������

			
	-- ������� ������ � ��������� ������
	insert into OutgoingRefDocs (ID, RowID, DocID, DocDescription)
	select rt.ID, newid() as RowID, rt.ParentID, rt.ParentDescription
	from @refTable rt
	where not exists
	(
		select top 1 1
		from OutgoingRefDocs ord
		where ord.ID = rt.ID
			and ord.DocID = rt.ParentID
	)

	-- ����� ��������� ����� �� �������� ������� � ��
	update psa
	set psa.MainContractID = rt.ParentID, psa.MainContractSubject = rt.ParentDescription
	from PnrSupplementaryAgreements psa
	join @refTable rt on psa.ID = rt.ID
	where psa.MainContractID is null

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