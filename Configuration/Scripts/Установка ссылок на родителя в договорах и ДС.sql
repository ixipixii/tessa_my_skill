-- установка ссылок на родительский документ в Договорах и ДС

BEGIN TRANSACTION;

BEGIN TRY		
		

	declare @refTable table (ID uniqueidentifier, ParentID uniqueidentifier, ParentDescription nvarchar(250))

	-- определим все договоры и ДС, которые имеют ссылки на родителя в выгрузке
	-- и которые существуют в Тесса
	insert into @refTable (ID, ParentID, ParentDescription)
	select coalesce(destContract.ID, destSA.ID), parent.ID, coalesce (srcParent.DocNumber, srcParent.DocDisplay)
	FROM [MigrationSED].[dbo].[ContractsTESSA] src
	join [MigrationSED].[dbo].[ContractsTESSA] srcParent on src.ParentUniqueId = srcParent.UniqueId
	join [PnrContracts] parent on parent.ExtID = src.ParentUniqueId
	left join [PnrContracts] destContract on src.UniqueId = destContract.ExtID
	left join [PnrSupplementaryAgreements] destSA on src.UniqueId = destSA.ExtID
	where
		destContract.ID is not null	-- договор к договору
		or destSA.ID is not null	-- ДС к договору

			
	-- добавим запись в коллекцию Ссылки
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

	-- также проставим сылку на основной договор в ДС
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