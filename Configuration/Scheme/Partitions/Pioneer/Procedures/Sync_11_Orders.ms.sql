CREATE PROCEDURE [Sync_11_Orders]
AS
BEGIN

	BEGIN TRANSACTION;

	BEGIN TRY		
		
		DECLARE @CardType UNIQUEIDENTIFIER = 'df141f0f-7e73-48fb-9cdb-6d46665cc0fb' -- PnrOrder
		DECLARE @CardTypeCaption NVARCHAR(128) = (SELECT TOP 1 Caption FROM [Types] WITH (NOLOCK) WHERE ID = @CardType)
		
		DECLARE @CreatedBy UNIQUEIDENTIFIER = '11111111-1111-1111-1111-111111111111' -- System
		DECLARE @CreatedByName NVARCHAR(128) = (SELECT TOP 1 [Name] FROM PersonalRoles WITH (NOLOCK) WHERE ID = @CreatedBy)
		
		DECLARE @CardTypeState UNIQUEIDENTIFIER = 'D819B56B-D6A3-447C-B257-CE53930E7ABE' 
		DECLARE @CardTypeStateCaption NVARCHAR(128) = (SELECT TOP 1 Caption FROM [Types] WITH (NOLOCK) WHERE ID = @CardTypeState)		
		
		DECLARE @CreationDate datetime2 = CONVERT(datetime2, '2020-11-26 00:00:00.000', 121)
		
		DROP TABLE IF EXISTS #ObjectsToUpdate

		CREATE TABLE #ObjectsToUpdate
		(
			[ID] [uniqueidentifier] NOT NULL,
			[UniqueId] [uniqueidentifier] NOT NULL,
			[DocNumber] [nvarchar](max) NULL,
			[OuterNumber] [nvarchar](max) NULL,
			[Created] [datetime] NULL,
			[Executors] [nvarchar](max) NULL,
			[DocState] [nvarchar](max) NULL,
			[DocProjectDate] [datetime] NULL,
			[Title] [nvarchar](max) NULL,
			[Project_MDM_Key] [nvarchar](max) NULL,
			[Comment] [nvarchar](max) NULL,
			[DocTypeTESSA] [nvarchar](max) NULL,
			[GroupTypeTESSA] [nvarchar](max) NULL,
			[Contragent_FullName] [nvarchar](max) NULL,
			[Project_Title] [nvarchar](max) NULL
		)
			
		DECLARE @script VARCHAR(max) = 
		'SELECT NEWID() AS [ID]
			,[UniqueId]
			,[DocNumber]
			,[OuterNumber]
			,[Created]
			,[Executors]
			,[DocState]
			,[DocProjectDate]
			,[Title]
			,[Project_MDM_Key]
			,[Comment]
			,[DocTypeTESSA]
			,[GroupTypeTESSA]
			,[Contragent_FullName]
			,[Project_Title]
		FROM [TST-TESSA-DB].[MigrationSED].[dbo].[OrdersTESSA] WITH(NOLOCK)
		where [GroupTypeTESSA] like N''Приказы''
			AND [UniqueId] IS NOT NULL'
		
		INSERT INTO #ObjectsToUpdate
		EXECUTE (@script)		
				
		--обновим существующие записи
		UPDATE [proj] 
		SET
			[proj].[RegistrationDate] = [obj].[DocProjectDate]
			,[proj].[ProjectID] = (SELECT TOP(1) [ID] FROM [PnrProjects] WHERE [MDMKey] = [obj].[Project_MDM_Key] AND [MDMKey] IS NOT NULL)
			,[proj].[ProjectName] = (SELECT TOP(1) [Name] FROM [PnrProjects] WHERE [MDMKey] = [obj].[Project_MDM_Key] AND [MDMKey] IS NOT NULL)
			,[proj].[Comments] = [obj].[Comment]
			,[proj].[DocumentKindID] = 
			CASE
				WHEN [obj].[DocTypeTESSA] = N'по обеспечению корпоративной мобильной связи' THEN '42e6e6af-c176-4b37-9490-44d4d256c028'
				ELSE (SELECT TOP(1) [ID] FROM [PnrDocumentKinds] WHERE LOWER([Name]) = LOWER([obj].[DocTypeTESSA]))
			END
			,[proj].[DocumentKindName] =
			CASE
				WHEN [obj].[DocTypeTESSA] = N'по обеспечению корпоративной мобильной связи' THEN N'По обеспечению корп.моб.связи'
				ELSE (SELECT TOP(1) [Name] FROM [PnrDocumentKinds] WHERE LOWER([Name]) = LOWER([obj].[DocTypeTESSA]))
			END
		FROM #ObjectsToUpdate as [obj] WITH(NOLOCK)
		INNER JOIN [PnrOrder] as [proj] WITH(NOLOCK) ON [proj].[ExtID] = [obj].[UniqueId]
			
		UPDATE [proj] 
		SET
			[proj].[Subject] = [obj].[Title]
			,[proj].[FullNumber] = [obj].[OuterNumber]
		FROM #ObjectsToUpdate as [obj] WITH(NOLOCK)
		INNER JOIN [PnrOrder] as [inc] WITH(NOLOCK) ON [inc].[ExtID] = [obj].[UniqueId]
		INNER JOIN [DocumentCommonInfo] as [proj] WITH(NOLOCK) ON  [proj].[ID] = [inc].[ID]
			
		--очистим выгрузку от существующих записей
		DELETE #ObjectsToUpdate WHERE [UniqueId] IN (SELECT [ExtID] from [PnrOrder] WHERE [ExtID] IS NOT NULL)
			
		-- Создание записей
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
			t.ID
			,@CardType
			,@CardTypeCaption
			,@CreationDate		
			,GETUTCDATE()
			,1
			,0
			,0
			,@CreatedBy
			,@CreatedByName
			,@CreatedBy
			,@CreatedByName
		FROM
			#ObjectsToUpdate AS [t]
				
		INSERT INTO [dbo].[PnrOrder]
		(
			[ID]
			,[RegistrationDate]
			,[ProjectID]
			,[ProjectName]
			,[Comments]
			,[DocumentKindID]
			,[DocumentKindName]
			,[ExtID]
		)
		SELECT 
			[obj].[ID]				  
			,[obj].[DocProjectDate] AS [RegistrationDate]
			,(SELECT TOP(1) [ID] FROM [PnrProjects] WHERE [MDMKey] = [obj].[Project_MDM_Key] AND [MDMKey] IS NOT NULL) AS [ProjectID]
			,(SELECT TOP(1) [Name] FROM [PnrProjects] WHERE [MDMKey] = [obj].[Project_MDM_Key] AND [MDMKey] IS NOT NULL) AS [ProjectName]
			,[obj].[Comment] AS [Comments]
			,CASE
				WHEN [obj].[DocTypeTESSA] = N'по обеспечению корпоративной мобильной связи' THEN '42e6e6af-c176-4b37-9490-44d4d256c028'
				ELSE (SELECT TOP(1) [ID] FROM [PnrDocumentKinds] WHERE LOWER([Name]) = LOWER([obj].[DocTypeTESSA]))
			END AS [DocumentKindID]
			,CASE
				WHEN [obj].[DocTypeTESSA] = N'по обеспечению корпоративной мобильной связи' THEN N'По обеспечению корп.моб.связи'
				ELSE (SELECT TOP(1) [Name] FROM [PnrDocumentKinds] WHERE LOWER([Name]) = LOWER([obj].[DocTypeTESSA]))
			END AS [DocumentKindName]
			,[obj].[UniqueId] AS [ExtID]
		FROM
			#ObjectsToUpdate AS [obj]
				
		INSERT INTO [DocumentCommonInfo] 
		(
			[ID]
			,[Subject]
			,[FullNumber]
			,[CardTypeID]
			,[CardTypeName]
			,[CardTypeCaption]
			,[AuthorID]
			,[AuthorName]
			,[CreationDate]				
		)
		SELECT 
			t.[ID]
			,t.[Title] as [Subject]
			,t.[OuterNumber] as [FullNumber]
			,@CardType
			,@CardTypeCaption
			,@CardTypeCaption
			,@CreatedBy
			,@CreatedByName
			,GETUTCDATE()
		FROM 
			#ObjectsToUpdate t
				
		--Состояния
		DROP TABLE IF EXISTS #FdStatesObjectsVirtual
		
		CREATE TABLE #FdStatesObjectsVirtual(
			[ID] [uniqueidentifier] NOT NULL,
			[MainCardId] [uniqueidentifier] NOT NULL
		)
		
		INSERT INTO #FdStatesObjectsVirtual ([ID], [MainCardId])
		SELECT NEWID() as [ID], t.[ID] FROM #ObjectsToUpdate as t
				
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
			t.[ID]
			,@CardTypeState
			,@CardTypeStateCaption
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
			#FdStatesObjectsVirtual as t
					
		INSERT INTO [FdSatelliteCommonInfo]
		(
			[ID]
			,[MainCardId]
			,[StateID]
			,[StateName]
		)
		SELECT 
			t.[ID]
			,t.[MainCardId]
			,'2ea7eaf3-7a9e-4e50-a031-b49a5a056338'
			,N'Зарегистрирован'
		FROM #FdStatesObjectsVirtual as t
						
		DROP TABLE IF EXISTS #FdStatesObjectsVirtual
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