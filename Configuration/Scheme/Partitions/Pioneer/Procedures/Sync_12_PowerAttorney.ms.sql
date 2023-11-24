CREATE PROCEDURE [Sync_12_PowerAttorney]
AS
BEGIN

	BEGIN TRANSACTION;

	BEGIN TRY		
		
		DECLARE @CardType UNIQUEIDENTIFIER = 'f9c07ae1-4e87-4cfe-8229-26ce6af5c326' -- PnrPowerAttorney
		DECLARE @CardTypeCaption NVARCHAR(128) = (SELECT TOP 1 Caption FROM [Types] WITH (NOLOCK) WHERE ID = @CardType)
		
		DECLARE @CreatedBy UNIQUEIDENTIFIER = '11111111-1111-1111-1111-111111111111' -- System
		DECLARE @CreatedByName NVARCHAR(128) = (SELECT TOP 1 [Name] FROM PersonalRoles WITH (NOLOCK) WHERE ID = @CreatedBy)
		
		DECLARE @CardTypeState UNIQUEIDENTIFIER = 'D819B56B-D6A3-447C-B257-CE53930E7ABE' 
		DECLARE @CardTypeStateCaption NVARCHAR(128) = (SELECT TOP 1 Caption FROM [Types] WITH (NOLOCK) WHERE ID = @CardTypeState)		
		
		DROP TABLE IF EXISTS #ObjectsToUpdate
		
		CREATE TABLE #ObjectsToUpdate
		(
			[ID] [uniqueidentifier] NOT NULL,
			[UniqueId] [uniqueidentifier] NOT NULL,
			[DocNumber] [nvarchar](max) NULL,
			[DocProjectDate] [datetime] NULL,
			[DocState] [nvarchar](max) NULL,
			[Title] [nvarchar](max) NULL,
			[Executors] [nvarchar](max) NULL,
			[Org_MDM_Key] [nvarchar](max) NULL,
			[IsEmployee] [nvarchar](max) NULL,
			[Dovlico] [nvarchar](max) NULL,
			[Authority] [nvarchar](max) NULL,
			[OrgAddressee] [nvarchar](max) NULL,
			[DovType] [nvarchar](max) NULL,
			[StartDate] [datetime] NULL,
			[DovEndDate] [datetime] NULL,
			[Comment] [nvarchar](max) NULL,
			[DocTypeTESSA] [nvarchar](max) NULL,
			[GroupTypeTESSA] [nvarchar](max) NULL,
			[Org_Title] [nvarchar](max) NULL         
		)
			
		DECLARE @script VARCHAR(max) = 
		'SELECT NEWID() AS [ID]
			,[UniqueId]
			,[DocNumber]
			,[DocProjectDate]
			,[DocState]
			,[Title]
			,[Executors]
			,[Org_MDM_Key]
			,[IsEmployee]
			,[Dovlico]
			,[Authority]
			,[OrgAddressee]
			,[DovType]
			,[StartDate]
			,[DovEndDate]
			,[Comment]
			,[DocTypeTESSA]
			,[GroupTypeTESSA]
			,[Org_Title]
		FROM [TST-TESSA-DB].[MigrationSED].[dbo].[ProxyTESSA] WITH(NOLOCK)
		WHERE [UniqueId] IS NOT NULL'
		
		INSERT INTO #ObjectsToUpdate
		EXECUTE (@script)		
				
		--обновим существующие записи
		UPDATE [proj] 
		SET
			[proj].[ProjectDate] = [obj].[DocProjectDate]
			,[proj].[OrganizationID] = (SELECT TOP(1) [ID] FROM [PnrOrganizations] WHERE [MDMKey] = obj.[Org_MDM_Key] AND [MDMKey] IS NOT NULL)
			,[proj].[OrganizationName] = (SELECT TOP(1) [Name] FROM [PnrOrganizations] WHERE [MDMKey] = obj.[Org_MDM_Key] AND [MDMKey] IS NOT NULL)
			,[proj].[DestinationID] = (SELECT TOP(1) [ID] FROM [Partners] WITH(NOLOCK) WHERE [Name] LIKE N'%' + [obj].[OrgAddressee] + N'%')
			,[proj].[DestinationName] = (SELECT TOP(1) [Name] FROM [Partners] WITH(NOLOCK) WHERE [Name] LIKE N'%' + [obj].[OrgAddressee] + N'%')
			,[proj].[Credentials] =[obj].[Authority]
			,[proj].[StartDate] =[obj].[StartDate]
			,[proj].[EndDate] = [obj].[DovEndDate]
			,[proj].[TypeID] = (SELECT TOP(1) [ID] FROM [PnrPowerAttorneyTypes] WITH(NOLOCK) WHERE [Name] LIKE N'%' + [obj].[DovType] + N'%')
			,[proj].[TypeName] = (SELECT TOP(1) [Name] FROM [PnrPowerAttorneyTypes] WITH(NOLOCK) WHERE [Name] LIKE N'%' + [obj].[DovType] + N'%')
			,[proj].[EmployeeID] = CASE WHEN [obj].[IsEmployee] =N'Да' THEN 1 WHEN [obj].[IsEmployee] =N'Нет' THEN 0 ELSE NULL END
			,[proj].[EmployeeName] = CASE WHEN [obj].[IsEmployee] =N'Да' THEN N'Да' WHEN [obj].[IsEmployee] =N'Нет' THEN N'Нет' ELSE NULL END
		FROM #ObjectsToUpdate as [obj] WITH(NOLOCK)
		INNER JOIN [PnrPowerAttorney] as [proj] WITH(NOLOCK) ON [proj].[ExtID] = [obj].[UniqueId]
			
		UPDATE [proj] 
		SET
			[proj].[Subject] = [obj].[Title]
			,[proj].[FullNumber] = [obj].[DocNumber]
		FROM #ObjectsToUpdate as [obj] WITH(NOLOCK)
		INNER JOIN [PnrPowerAttorney] as [inc] WITH(NOLOCK) ON [inc].[ExtID] = [obj].[UniqueId]
		INNER JOIN [DocumentCommonInfo] as [proj] WITH(NOLOCK) ON  [proj].[ID] = [inc].[ID]
			
		--очистим выгрузку от существующих записей
		DELETE #ObjectsToUpdate WHERE [UniqueId] IN (SELECT [ExtID] from [PnrPowerAttorney] WHERE [ExtID] IS NOT NULL)
			
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
			#ObjectsToUpdate AS [t]
				
		INSERT INTO [dbo].[PnrPowerAttorney]
		(
			[ID]
			,[ProjectDate]
			,[OrganizationID]
			,[OrganizationName]
			,[DestinationID]
			,[DestinationName]
			,[Credentials]
			,[StartDate]
			,[EndDate]
			,[TypeID]
			,[TypeName]
			,[EmployeeID]
			,[EmployeeName]
			,[ExtID]
		)
		SELECT 
			[obj].[ID]				  
			,[obj].[DocProjectDate] AS [ProjectDate]
			,(SELECT TOP(1) [ID] FROM [PnrOrganizations] WHERE [MDMKey] = obj.[Org_MDM_Key] AND [MDMKey] IS NOT NULL) AS [OrganizationID]
			,(SELECT TOP(1) [Name] FROM [PnrOrganizations] WHERE [MDMKey] = obj.[Org_MDM_Key] AND [MDMKey] IS NOT NULL) AS [OrganizationName]
			,(SELECT TOP(1) [ID] FROM [Partners] WITH(NOLOCK) WHERE [Name] LIKE N'%' + [obj].[OrgAddressee] + N'%') AS [DestinationID]
			,(SELECT TOP(1) [Name] FROM [Partners] WITH(NOLOCK) WHERE [Name] LIKE N'%' + [obj].[OrgAddressee] + N'%') AS [DestinationName]
			,[obj].[Authority] AS [Credentials]
			,[obj].[StartDate] AS [StartDate]
			,[obj].[DovEndDate] AS [EndDate]
			,(SELECT TOP(1) [ID] FROM [PnrPowerAttorneyTypes] WITH(NOLOCK) WHERE [Name] LIKE N'%' + [obj].[DovType] + N'%') AS [TypeID]
			,(SELECT TOP(1) [Name] FROM [PnrPowerAttorneyTypes] WITH(NOLOCK) WHERE [Name] LIKE N'%' + [obj].[DovType] + N'%') AS [TypeName]
			,CASE
				WHEN [obj].[IsEmployee] =N'Да' THEN 1
				WHEN [obj].[IsEmployee] =N'Нет' THEN 0
				ELSE NULL
			END AS [EmployeeID]
			,CASE
				WHEN [obj].[IsEmployee] =N'Да' THEN N'Да'
				WHEN [obj].[IsEmployee] =N'Нет' THEN N'Нет'
				ELSE NULL
			END AS [EmployeeName]
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
			,t.[DocNumber] as [FullNumber]
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
			,'4b03094a-3170-49a4-830b-6d761e37740f'
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