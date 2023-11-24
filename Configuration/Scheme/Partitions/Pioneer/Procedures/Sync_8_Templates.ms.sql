CREATE PROCEDURE [Sync_8_Templates]
AS
BEGIN

	BEGIN TRANSACTION;

	BEGIN TRY		

		DECLARE @CardType UNIQUEIDENTIFIER = 'dc10a79d-4bb2-4aad-acb8-82d5838408a9' -- PnrTemplate
		DECLARE @CardTypeCaption NVARCHAR(128) = (SELECT TOP 1 Caption FROM [Types] WITH (NOLOCK) WHERE ID = @CardType)

		DECLARE @CreatedBy UNIQUEIDENTIFIER = '11111111-1111-1111-1111-111111111111' -- System
		DECLARE @CreatedByName NVARCHAR(128) = (SELECT TOP 1 [Name] FROM [PersonalRoles] WITH (NOLOCK) WHERE ID = @CreatedBy)

		DECLARE @CardTypeState UNIQUEIDENTIFIER = 'D819B56B-D6A3-447C-B257-CE53930E7ABE' 
		DECLARE @CardTypeStateCaption NVARCHAR(128) = (SELECT TOP 1 Caption FROM [Types] WITH (NOLOCK) WHERE ID = @CardTypeState)

		DECLARE @CreationDate datetime2 = CONVERT(datetime2, '2020-11-26 00:00:00.000', 121)

		DROP TABLE IF EXISTS #ObjectsToUpdate

		CREATE TABLE #ObjectsToUpdate
		(
			[ID] [uniqueidentifier] NOT NULL
			,[UniqueId] [uniqueidentifier] NOT NULL
			,[DocProjectNumber] [nvarchar](max) NULL
			,[DocProjectDate] [datetime] NULL
			,[DocDisplay] [nvarchar](max) NULL
			,[Annotation] [nvarchar](max) NULL
			,[Org_MDM_Key] [nvarchar](max) NULL
			,[TemplateCRM] [nvarchar](max) NULL
			,[DocDate] [datetime] NULL
			,[Project_MDM_Key] [nvarchar](max) NULL
			,[DocTypeTESSA] [nvarchar](max) NULL
			,[GroupTypeTESSA] [nvarchar](max) NULL
			,[Org_Title] [nvarchar](max) NULL
			,[Project_Title] [nvarchar](max) NULL
		)
			
		DECLARE @script VARCHAR(max) = 
		'SELECT NEWID() AS [ID]
			,[UniqueId]
			,[DocProjectNumber]
			,[DocProjectDate]
			,[DocDisplay]
			,[Annotation]
			,[Org_MDM_Key]
			,[TemplateCRM]
			,[DocDate]
			,[Project_MDM_Key]
			,[DocTypeTESSA]
			,[GroupTypeTESSA]
			,[Org_Title]
			,[Project_Title]
		FROM [TST-TESSA-DB].[MigrationSED].[dbo].[TemplatesTESSA]
		WHERE [UniqueId] IS NOT NULL'

		INSERT INTO #ObjectsToUpdate
		EXECUTE (@script)		
				
		--обновим существующие записи
		UPDATE [proj] 
		SET
			[proj].[ProjectNo] = [obj].[DocProjectNumber]
			,[proj].[ProjectDate] = [obj].[DocProjectDate]
			,[proj].[SubjectContract] = [obj].[DocDisplay]
			,[proj].[Content] = [obj].[Annotation]
			,[proj].[OrganizationID] = [pnrorg].[ID]
			,[proj].[OrganizationName] = [pnrorg].[Name]
			,[proj].[PublishID] = (CASE WHEN [obj].[TemplateCRM] LIKE N'%Общие%' THEN 0
								    	   WHEN [obj].[TemplateCRM] LIKE N'%акт%' THEN 1
								    	   ELSE NULL END)
			,[proj].[PublishName] = (CASE WHEN [obj].[TemplateCRM] LIKE N'%Общие%' THEN N'Общие'
								    	   WHEN [obj].[TemplateCRM] LIKE N'%акт%' THEN N'Под передаточные акты'
								    	   ELSE NULL END)
			,[proj].[ProjectID] = [pnrpr].[ID]
			,[proj].[ProjectName] = [pnrpr].[Name]
		FROM #ObjectsToUpdate as [obj]
		INNER JOIN [PnrTemplates] as [proj] WITH(NOLOCK) ON [proj].[ExtID] = [obj].[UniqueId] 
		LEFT JOIN [PnrProjects] as [pnrpr] WITH(NOLOCK) ON [pnrpr].[MDMKey] = [obj].[Project_MDM_Key] AND [pnrpr].[MDMKey] IS NOT NULL
		LEFT JOIN [PnrOrganizations] as [pnrorg] WITH(NOLOCK) ON [pnrorg].[MDMKey] = [obj].[Org_MDM_Key] AND [pnrorg].[MDMKey] IS NOT NULL
		WHERE [proj].[ExtID] = [obj].[UniqueId]
			
		--очистим выгрузку от существующих записей
		DELETE #ObjectsToUpdate WHERE [UniqueId] IN (SELECT [ExtID] from [PnrTemplates] WHERE [ExtID] IS NOT NULL)
				
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
			#ObjectsToUpdate t
			
		INSERT INTO [PnrTemplates]
		(
			[ID]
			,[ProjectNo]
			,[ProjectDate] 
			,[SubjectContract]
			,[Content]
			,[OrganizationID]
			,[OrganizationName]
			,[PublishID]
			,[PublishName]
			,[ProjectID]
			,[ProjectName]
			,[ExtID]
			,[TemplateID]
			,[TemplateName]
		)
		SELECT
			[obj].[ID]
			,[obj].[DocProjectNumber]
			,[obj].[DocProjectDate]
			,[obj].[DocDisplay]
			,[obj].[Annotation]
			,[pnrorg].[ID] AS [OrganizationID]
			,[pnrorg].[Name] AS [OrganizationName]
			,CASE WHEN [obj].[TemplateCRM] LIKE N'%Общие%' THEN 0
								    	   WHEN [obj].[TemplateCRM] LIKE N'%акт%' THEN 1
								    	   ELSE NULL END AS [PublishID]
			,CASE WHEN [obj].[TemplateCRM] LIKE N'%Общие%' THEN N'Общие'
								    	   WHEN [obj].[TemplateCRM] LIKE N'%акт%' THEN N'Под передаточные акты'
								    	   ELSE NULL END AS [PublishName]
			,[pnrpr].[ID] AS [ProjectID]
			,[pnrpr].[Name] AS [ProjectName]
			,[obj].[UniqueId]
			,0
			,N'К публикации CRM'
		FROM
			#ObjectsToUpdate as [obj]
		LEFT JOIN [PnrProjects] as [pnrpr] WITH(NOLOCK) ON [pnrpr].[MDMKey] = [obj].[Project_MDM_Key] AND [pnrpr].[MDMKey] IS NOT NULL
		LEFT JOIN [PnrOrganizations] as [pnrorg] WITH(NOLOCK) ON [pnrorg].[MDMKey] = [obj].[Org_MDM_Key] AND [pnrorg].[MDMKey] IS NOT NULL
			
		INSERT INTO [DocumentCommonInfo] 
		(
			[ID]
			,[CardTypeID]
			,[CardTypeName]
			,[AuthorID]
			,[AuthorName]
			,[CreationDate]				
		)
		SELECT 
			[t].[ID]
			,@CardType
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
			,'2db198dd-6a58-4b1d-a90b-19be1b1e53f5'
			,N'Проект'
		FROM #FdStatesObjectsVirtual as t
						
		DROP TABLE #FdStatesObjectsVirtual
		DROP TABLE #ObjectsToUpdate
				
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