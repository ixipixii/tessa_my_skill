CREATE PROCEDURE [Sync_7_Organizations]
AS
BEGIN

	BEGIN TRANSACTION;

	BEGIN TRY		
		
			DECLARE @CardType UNIQUEIDENTIFIER = 'a668f7ea-efcd-47f0-a3c7-c4d1e7ed0bc8' -- PnrOrganization
			DECLARE @CardTypeCaption NVARCHAR(128) = (SELECT TOP 1 Caption FROM [Types] WITH (NOLOCK) WHERE ID = @CardType)

			DECLARE @CreatedBy UNIQUEIDENTIFIER = '11111111-1111-1111-1111-111111111111' -- System
			DECLARE @CreatedByName NVARCHAR(128) = (SELECT TOP 1 [Name] FROM [PersonalRoles] WITH (NOLOCK) WHERE ID = @CreatedBy)

			DROP TABLE IF EXISTS #ObjectsToUpdate

			CREATE TABLE #ObjectsToUpdate
			(
			     [ID] [uniqueidentifier] NOT NULL
			    ,[Title] [nvarchar](max) NULL
			    ,[Created] [datetime] NULL
			    ,[FullName] [nvarchar](max) NULL
			    ,[WorkPhone] [nvarchar](max) NULL
			    ,[WorkFax] [nvarchar](max) NULL
			    ,[WorkAddress] [nvarchar](max) NULL
			    ,[WorkZip] [nvarchar](max) NULL
			    ,[INN] [nvarchar](max) NULL
		           ,[KPP] [nvarchar](max) NULL
		           ,[Abbreviation] [nvarchar](max) NULL
		           ,[MDM_Key] [nvarchar](max) NULL
			    ,[BankTitle] [nvarchar](max) NULL
			    ,[BIK] [nvarchar](max) NULL
			    ,[CorrAccount] [nvarchar](max) NULL
		           ,[PaymentAccount] [nvarchar](max) NULL
		           ,[CertificateNumber] [nvarchar](max) NULL		           
       		    ,[GenDirPost] [nvarchar](max) NULL
		           ,[TranslationStateWebId] [nvarchar](max) NULL
			    ,[GenDirPost2] [nvarchar](max) NULL
			    ,[GenDir2] [nvarchar](max) NULL
			    ,[LegalForm] [nvarchar](max) NULL
		           ,[OKPO] [nvarchar](max) NULL
		           ,[OrgNamePioneer] [nvarchar](max) NULL
       		    ,[GenDirPost3] [nvarchar](max) NULL
		           ,[GenDir3] [nvarchar](max) NULL
			    ,[OKATO] [nvarchar](max) NULL
			    ,[OKOGU] [nvarchar](max) NULL
			    ,[OKOPF] [nvarchar](max) NULL
		           ,[OKTMO] [nvarchar](max) NULL
		           ,[OKFS] [nvarchar](max) NULL
			    ,[OKVED] [nvarchar](max) NULL
			    ,[RegCard] [nvarchar](max) NULL
		           ,[GenDirIm] [nvarchar](max) NULL
		           ,[OrganizationGroups] [nvarchar](max) NULL
		           ,[isActive] [int] NULL
		           ,[GenDir] [int] NULL
		           ,[GlavBuh] [int] NULL
		           ,[GrossBuh] [int] NULL
			)
			
			DECLARE @script VARCHAR(max) = 
			'SELECT NEWID() AS [ID]
			           ,[Title]
			           ,[Created]
			           ,[FullName]
			           ,[WorkPhone]
			           ,[WorkFax]
			           ,[WorkAddress]
			           ,[WorkZip]
			           ,[INN]
			           ,[KPP]
			           ,[Abbreviation]
			           ,[MDM_Key]
			           ,[BankTitle]
			           ,[BIK]
			           ,[CorrAccount]
			           ,[PaymentAccount]
			           ,[CertificateNumber]
			           ,[GenDirPost]
			           ,[TranslationStateWebId]
			           ,[GenDirPost2]
			           ,[GenDir2]
			           ,[LegalForm]
			           ,[OKPO]
			           ,[OrgNamePioneer]
			           ,[GenDirPost3]
			           ,[GenDir3]
			           ,[OKATO]
			           ,[OKOGU]
			           ,[OKOPF]
			           ,[OKTMO]
			           ,[OKFS]
			           ,[OKVED]
			           ,[RegCard]
			           ,[GenDirIm]
			           ,[OrganizationGroups]
		            	    ,[isActive]
			           ,[GenDir]
			           ,[GlavBuh]
			           ,[GrossBuh]
  			FROM [TST-TESSA-DB].[MigrationSED].[dbo].[Organizations]
  			WHERE [Title] IS NOT NULL'
			INSERT INTO #ObjectsToUpdate
				EXECUTE (@script)		
				
			--обновим существующие записи
			UPDATE [proj] 
				SET
				[proj].[Name] = [obj].[Title]
				,[proj].[FullName] = [obj].[FullName]
				,[proj].[WorkPhone] = [obj].[WorkPhone]
				,[proj].[FaxNumber] = [obj].[WorkFax]
				,[proj].[Address] = [obj].[WorkAddress]
				,[proj].[Idx] = [obj].[WorkZip]
				,[proj].[INN] = [obj].[INN]
				,[proj].[KPP] = [obj].[KPP]
				,[proj].[Abbreviation] = [obj].[Abbreviation]
				,[proj].[Bank] = [obj].[BankTitle]
				,[proj].[BIK] = [obj].[BIK]
				,[proj].[CorrespondentAccount] = [obj].[CorrAccount]
				,[proj].[CertificateSeriesNumber] = [obj].[PaymentAccount]
				,[proj].[PositionHeadLegalEntity] = [obj].[GenDirPost]
				,[proj].[Website] = [obj].[TranslationStateWebId]
				,[proj].[PositionHeadLegalEntityDative] = [obj].[GenDirPost2]
				,[proj].[HeadLegalEntityDative] = [obj].[GenDir2]
				,[proj].[PartnerType] = [obj].[LegalForm]
				,[proj].[OKPO] = [obj].[OKPO]
				,[proj].[PartnerID] = (select top 1 [ID] from [Partners] p where [obj].[OrgNamePioneer] is not null and p.[INN] = [obj].[INN])
				,[proj].[PartnerName] = (select top 1 [Name] from [Partners] p where [obj].[OrgNamePioneer] is not null and p.[INN] = [obj].[INN])
				,[proj].[PositionHeadLegalEntityGenitive] = [obj].[GenDirPost3]
				,[proj].[HeadLegalEntityGenitive] = [obj].[GenDir3]
				,[proj].[OKATO] = [obj].[OKATO]
				,[proj].[OKOGU] = [obj].[OKOGU]
				,[proj].[OKOPFCode] = [obj].[OKOPF]
				,[proj].[OKTMO] = [obj].[OKTMO]
				,[proj].[OKFSCode] = [obj].[OKFS]
				,[proj].[OKVEDCode] = [obj].[OKVED]
				,[proj].[RegistryCard] = [obj].[RegCard]
				,[proj].[GroupDocuments] = [obj].[OrganizationGroups]
				--ген дир
				,[proj].[HeadLegalEntityID] = (SELECT TOP 1 [per].[ID] FROM [PersonalRoles] as [per] 
				INNER JOIN [TST-TESSA-DB].[MigrationSED].[dbo].[Users] as [us] ON [us].[ID] = [obj].[GenDir] AND [us].[UniqueId] = [per].[ExtID])
				,[proj].[HeadLegalEntityName] = (SELECT TOP 1 [per].[Name] FROM [PersonalRoles] as [per] 
				INNER JOIN [TST-TESSA-DB].[MigrationSED].[dbo].[Users] as [us] ON [us].[ID] = [obj].[GenDir] AND [us].[UniqueId] = [per].[ExtID])
				-- глав бух
				,[proj].[ChiefAccountantID] = (SELECT TOP 1 [per].[ID] FROM [PersonalRoles] as [per] 
				INNER JOIN [TST-TESSA-DB].[MigrationSED].[dbo].[Users] as [us] ON [us].[ID] = [obj].[GlavBuh] AND [us].[UniqueId] = [per].[ExtID])
				,[proj].[ChiefAccountantName] = (SELECT TOP 1 [per].[Name] FROM [PersonalRoles] as [per] 
				INNER JOIN [TST-TESSA-DB].[MigrationSED].[dbo].[Users] as [us] ON [us].[ID] = [obj].[GlavBuh] AND [us].[UniqueId] = [per].[ExtID])
				-- глав бух для процессов
				,[proj].[ChiefAccountantProcessID] = (SELECT TOP 1 [per].[ID] FROM [PersonalRoles] as [per] 
				INNER JOIN [TST-TESSA-DB].[MigrationSED].[dbo].[Users] as [us] ON [us].[ID] = [obj].[GrossBuh] AND [us].[UniqueId] = [per].[ExtID])
				,[proj].[ChiefAccountantProcessName] = (SELECT TOP 1 [per].[Name] FROM [PersonalRoles] as [per] 
				INNER JOIN [TST-TESSA-DB].[MigrationSED].[dbo].[Users] as [us] ON [us].[ID] = [obj].[GrossBuh] AND [us].[UniqueId] = [per].[ExtID])
			FROM #ObjectsToUpdate as [obj]
			INNER JOIN [PnrOrganizations] as [proj] ON [proj].[MDMKey] = [obj].[MDM_Key] 
			WHERE [obj].[MDM_Key] IS NOT NULL
			AND [proj].[MDMKey] IS NOT NULL
			
			--очистим выгрузку от существующих записей
			DELETE obj
			from #ObjectsToUpdate obj
			WHERE [MDM_Key] IN (SELECT [MDMKey] from [PnrOrganizations] WHERE [MDMKey] IS NOT NULL)
			-- чтобы не создавать дубли для записей без MDM ключа
			or exists 
			(
				select top 1 1
				from PnrOrganizations o
				where o.FullName = obj.FullName and o.MDMKey is null
			)
				
			--создадим новые карточки
			INSERT INTO [Instances]
			(
				[ID]
				,[TypeID]
				,[TypeCaption]
				,[Created]
				,[Modified]
				,[Version]
				,[WritePending]
				,[Readers]
				,[CreatedByID]
				,[CreatedByName]
				,[ModifiedByID]
				,[ModifiedByName]
			)
			SELECT
				[t].[ID]
				,@CardType
				,@CardTypeCaption
				,GETUTCDATE()			
				,GETUTCDATE()
				,1
				,0
				,0
				,@CreatedBy
				,@CreatedByName
				,@CreatedBy
				,@CreatedByName
			FROM
				#ObjectsToUpdate t
			
			INSERT INTO [PnrOrganizations]
			(
				[ID]
				,[Name]
				,[FullName]
				,[WorkPhone]
				,[FaxNumber]
				,[Address]
				,[Idx]
				,[INN]
				,[KPP]
				,[Abbreviation]
				,[Bank]
				,[BIK]
				,[CorrespondentAccount]
				,[CertificateSeriesNumber]
				,[PositionHeadLegalEntity]
				,[Website]
				,[PositionHeadLegalEntityDative]
				,[HeadLegalEntityDative]
				,[PartnerType]
				,[OKPO]
				,[PartnerID]
				,[PartnerName]
				,[PositionHeadLegalEntityGenitive]
				,[HeadLegalEntityGenitive]
				,[OKATO]
				,[OKOGU]
				,[OKOPFCode]
				,[OKTMO]
				,[OKFSCode]
				,[OKVEDCode]
				,[RegistryCard]
				,[GroupDocuments]
				,[MDMKey]
				,[HeadLegalEntityID]
				,[HeadLegalEntityName]
				,[ChiefAccountantID]
				,[ChiefAccountantName]
				,[ChiefAccountantProcessID]
				,[ChiefAccountantProcessName]
			)
			SELECT
				[obj].[ID]
				,[obj].[Title]
				,[obj].[FullName]
				,[obj].[WorkPhone]
				,[obj].[WorkFax]
				,[obj].[WorkAddress]
				,[obj].[WorkZip]
				,[obj].[INN]
				,[obj].[KPP]
				,[obj].[Abbreviation]
				,[obj].[BankTitle]
				,[obj].[BIK]
				,[obj].[CorrAccount]
				,[obj].[PaymentAccount]
				,[obj].[GenDirPost]
				,[obj].[TranslationStateWebId]
				,[obj].[GenDirPost2]
				,[obj].[GenDir2]
				,[obj].[LegalForm]
				,[obj].[OKPO]
				,(select top 1 [ID] from [Partners] p where [obj].[OrgNamePioneer] is not null and p.[INN] = [obj].[INN])
				,(select top 1 [Name] from [Partners] p where [obj].[OrgNamePioneer] is not null and p.[INN] = [obj].[INN])
				,[obj].[GenDirPost3]
				,[obj].[GenDir3]
				,[obj].[OKATO]
				,[obj].[OKOGU]
				,[obj].[OKOPF]
				,[obj].[OKTMO]
				,[obj].[OKFS]
				,[obj].[OKVED]
				,[obj].[RegCard]
				,[obj].[OrganizationGroups]
			      ,[obj].[MDM_Key]
			      --ген дир
				,(SELECT TOP 1 [per].[ID] FROM [PersonalRoles] as [per] 
				INNER JOIN [TST-TESSA-DB].[MigrationSED].[dbo].[Users] as [us] ON [us].[ID] = [obj].[GenDir] AND [us].[UniqueId] = [per].[ExtID])
				,(SELECT TOP 1 [per].[Name] FROM [PersonalRoles] as [per] 
				INNER JOIN [TST-TESSA-DB].[MigrationSED].[dbo].[Users] as [us] ON [us].[ID] = [obj].[GenDir] AND [us].[UniqueId] = [per].[ExtID])
				-- глав бух
				,(SELECT TOP 1 [per].[ID] FROM [PersonalRoles] as [per] 
				INNER JOIN [TST-TESSA-DB].[MigrationSED].[dbo].[Users] as [us] ON [us].[ID] = [obj].[GlavBuh] AND [us].[UniqueId] = [per].[ExtID])
				,(SELECT TOP 1 [per].[Name] FROM [PersonalRoles] as [per] 
				INNER JOIN [TST-TESSA-DB].[MigrationSED].[dbo].[Users] as [us] ON [us].[ID] = [obj].[GlavBuh] AND [us].[UniqueId] = [per].[ExtID])
				-- глав бух для процессов
				,(SELECT TOP 1 [per].[ID] FROM [PersonalRoles] as [per] 
				INNER JOIN [TST-TESSA-DB].[MigrationSED].[dbo].[Users] as [us] ON [us].[ID] = [obj].[GrossBuh] AND [us].[UniqueId] = [per].[ExtID])
				,(SELECT TOP 1 [per].[Name] FROM [PersonalRoles] as [per] 
				INNER JOIN [TST-TESSA-DB].[MigrationSED].[dbo].[Users] as [us] ON [us].[ID] = [obj].[GrossBuh] AND [us].[UniqueId] = [per].[ExtID])
			FROM
				#ObjectsToUpdate as [obj]
				
			DROP TABLE IF EXISTS #ObjectsToUpdate
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

END