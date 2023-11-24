CREATE PROCEDURE [Sync_3_Projects]
AS
BEGIN

	BEGIN TRANSACTION;

	BEGIN TRY		
		
			DECLARE @CardType UNIQUEIDENTIFIER = 'c17a5031-e7d8-4ea6-b03f-4c88b4bd6063' -- PnrProject
			DECLARE @CardTypeCaption NVARCHAR(128) = (SELECT TOP 1 Caption FROM [Types] WITH (NOLOCK) WHERE ID = @CardType)

			DECLARE @CreatedBy UNIQUEIDENTIFIER = '11111111-1111-1111-1111-111111111111' -- System
			DECLARE @CreatedByName NVARCHAR(128) = (SELECT TOP 1 [Name] FROM PersonalRoles WITH (NOLOCK) WHERE ID = @CreatedBy)

			DROP TABLE IF EXISTS #ObjectsToUpdate

			CREATE TABLE #ObjectsToUpdate
			(
				[ID] [uniqueidentifier] NOT NULL
			    ,[Title] [nvarchar](max) NULL
		           ,[FullName] [nvarchar](max) NULL
		           ,[Created] [datetime] NULL
		           ,[RubricIndex] [nvarchar](max) NULL
		           ,[MDM_Key] [nvarchar](max) NULL
		           ,[ProjectManager] [nvarchar](36) NULL
		           ,[ProjectAdmin] [nvarchar](36) NULL
		           ,[ProjectEconomist] [nvarchar](36) NULL
		           ,[Chief] [nvarchar](36) NULL
		           ,[Estimator] [nvarchar](36) NULL
		           ,[EngineerPTO] [nvarchar](36) NULL
		           ,[ConstructionManager] [nvarchar](36) NULL
		          -- ,[Parent_MDM_Key] [nvarchar](50) NULL
		           ,[RubricLinkStateId] [nvarchar](50) NULL
		           ,[UniqueId] [uniqueidentifier] NULL
			)
			
			DECLARE @script VARCHAR(max) = 
			'SELECT NEWID() AS [ID]
  				       ,[Title]
     					,[FullName]
     					,[Created]
     					,[RubricIndex]
      					,[MDM_Key]
      					,[ProjectManager] 
			           	,[ProjectAdmin] 
			           	,[ProjectEconomist] 
			           	,[Chief] 
			           	,[Estimator] 
			          	,[EngineerPTO] 
			           	,[ConstructionManager] 
			           	--,[Parent_MDM_Key]
			           	,[RubricLinkStateId]
			           	,[UniqueId]
  			FROM [TST-TESSA-DB].[MigrationSED].[dbo].[Projects]
  			WHERE [Title] IS NOT NULL'
			INSERT INTO #ObjectsToUpdate
				EXECUTE (@script)		
				
			--обновим существующие записи
			UPDATE [proj] 
				SET
				[proj].[Name] = [obj].[Title]
				,[proj].[Code] = [obj].[RubricIndex]
				,[proj].[MDMKey] = [obj].[MDM_Key]
				--рук проекта
				,[proj].[ProjectManagerID] = (SELECT TOP 1 [per].[ID] FROM [PersonalRoles] as [per] 
				INNER JOIN [TST-TESSA-DB].[MigrationSED].[dbo].[Users] as [us] ON [us].[ID] = [obj].[ProjectManager] AND [us].[UniqueId] = [per].[ExtID])
				,[proj].[ProjectManagerName] = (SELECT TOP 1 [per].[Name] FROM [PersonalRoles] as [per] 
				INNER JOIN [TST-TESSA-DB].[MigrationSED].[dbo].[Users] as [us] ON [us].[ID] = [obj].[ProjectManager] AND [us].[UniqueId] = [per].[ExtID])
				--админ проекта
				,[proj].[ProjectAdministratorID] = (SELECT TOP 1 [per].[ID] FROM [PersonalRoles] as [per] 
				INNER JOIN [TST-TESSA-DB].[MigrationSED].[dbo].[Users] as [us] ON [us].[ID] = [obj].[ProjectAdmin] AND [us].[UniqueId] = [per].[ExtID])
				,[proj].[ProjectAdministratorName] = (SELECT TOP 1 [per].[Name] FROM [PersonalRoles] as [per] 
				INNER JOIN [TST-TESSA-DB].[MigrationSED].[dbo].[Users] as [us] ON [us].[ID] = [obj].[ProjectAdmin] AND [us].[UniqueId] = [per].[ExtID])
				--экономист проекта
				,[proj].[ProjectEconomistID] = (SELECT TOP 1 [per].[ID] FROM [PersonalRoles] as [per] 
				INNER JOIN [TST-TESSA-DB].[MigrationSED].[dbo].[Users] as [us] ON [us].[ID] = [obj].[ProjectEconomist] AND [us].[UniqueId] = [per].[ExtID])
				,[proj].[ProjectEconomistName] = (SELECT TOP 1 [per].[Name] FROM [PersonalRoles] as [per] 
				INNER JOIN [TST-TESSA-DB].[MigrationSED].[dbo].[Users] as [us] ON [us].[ID] = [obj].[ProjectEconomist] AND [us].[UniqueId] = [per].[ExtID])
				--гип проекта
				,[proj].[GIPID] = (SELECT TOP 1 [per].[ID] FROM [PersonalRoles] as [per] 
				INNER JOIN [TST-TESSA-DB].[MigrationSED].[dbo].[Users] as [us] ON [us].[ID] = [obj].[Chief] AND [us].[UniqueId] = [per].[ExtID])
				,[proj].[GIPName] = (SELECT TOP 1 [per].[Name] FROM [PersonalRoles] as [per] 
				INNER JOIN [TST-TESSA-DB].[MigrationSED].[dbo].[Users] as [us] ON [us].[ID] = [obj].[Chief] AND [us].[UniqueId] = [per].[ExtID])
				--сметчик проекта
				,[proj].[EstimatorID] = (SELECT TOP 1 [per].[ID] FROM [PersonalRoles] as [per] 
				INNER JOIN [TST-TESSA-DB].[MigrationSED].[dbo].[Users] as [us] ON [us].[ID] = [obj].[Estimator] AND [us].[UniqueId] = [per].[ExtID])
				,[proj].[EstimatorName] = (SELECT TOP 1 [per].[Name] FROM [PersonalRoles] as [per] 
				INNER JOIN [TST-TESSA-DB].[MigrationSED].[dbo].[Users] as [us] ON [us].[ID] = [obj].[Estimator] AND [us].[UniqueId] = [per].[ExtID])
				--инж пто
				,[proj].[EngineerPTOID] = (SELECT TOP 1 [per].[ID] FROM [PersonalRoles] as [per] 
				INNER JOIN [TST-TESSA-DB].[MigrationSED].[dbo].[Users] as [us] ON [us].[ID] = [obj].[EngineerPTO] AND [us].[UniqueId] = [per].[ExtID])
				,[proj].[EngineerPTOName] = (SELECT TOP 1 [per].[Name] FROM [PersonalRoles] as [per] 
				INNER JOIN [TST-TESSA-DB].[MigrationSED].[dbo].[Users] as [us] ON [us].[ID] = [obj].[EngineerPTO] AND [us].[UniqueId] = [per].[ExtID])
				--руководитель строительства 
				,[proj].[ConstructionManagerID] = (SELECT TOP 1 [per].[ID] FROM [PersonalRoles] as [per] 
				INNER JOIN [TST-TESSA-DB].[MigrationSED].[dbo].[Users] as [us] ON [us].[ID] = [obj].[ConstructionManager] AND [us].[UniqueId] = [per].[ExtID])
				,[proj].[ConstructionManagerName] = (SELECT TOP 1 [per].[Name] FROM [PersonalRoles] as [per] 
				INNER JOIN [TST-TESSA-DB].[MigrationSED].[dbo].[Users] as [us] ON [us].[ID] = [obj].[ConstructionManager] AND [us].[UniqueId] = [per].[ExtID])
				,[proj].[InArchive] = CASE WHEN [obj].[RubricLinkStateId] = N'Завершен' THEN 1 ELSE 0 END
			FROM #ObjectsToUpdate as [obj]
			INNER JOIN [PnrProjects] as [proj] ON [proj].[ExtID] = [obj].[UniqueId]
			/*
			--обновим родительские записи
			UPDATE [pnr] SET
			[pnr].[ParentProjectID] = (select top 1 [ID] from [PnrProjects] as [t] where [t].[MDMKey] = [obj].[Parent_MDM_Key]),
			[pnr].[ParentProjectName] = (select top 1 [Name] from [PnrProjects] as [t] where [t].[MDMKey] = [obj].[Parent_MDM_Key])
			from [PnrProjects] as [pnr]
			inner join #ObjectsToUpdate as [obj] on [obj].[MDM_Key] = [pnr].[MDMKey]
			*/
			
			--очистим выгрузку от существующих записей
			DELETE #ObjectsToUpdate WHERE [UniqueId] IN (SELECT [ExtID] from [PnrProjects] WHERE [ExtID] IS NOT NULL)
				
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
			
			INSERT INTO [PnrProjects]
			(
				[ID]
				,[Name]
				,[Code]
				,[MDMKey]
				,[ProjectManagerID]
				,[ProjectManagerName]
				,[ProjectAdministratorID]
				,[ProjectAdministratorName]
				,[ProjectEconomistID]
				,[ProjectEconomistName]
				,[GIPID]
				,[GIPName]
				,[EstimatorID]
				,[EstimatorName]
				,[EngineerPTOID]
				,[EngineerPTOName]
				,[ConstructionManagerID]
				,[ConstructionManagerName]
				,[InArchive]
				,[ExtID]
			)
			SELECT
				[obj].[ID]
				,[obj].[Title]
				,[obj].[RubricIndex]
				,[obj].[MDM_Key]
				--рук проекта
				,(SELECT TOP 1 [per].[ID] FROM [PersonalRoles] as [per] 
				INNER JOIN [TST-TESSA-DB].[MigrationSED].[dbo].[Users] as [us] ON [us].[ID] = [obj].[ProjectManager] AND [us].[UniqueId] = [per].[ExtID])
				,(SELECT TOP 1 [per].[Name] FROM [PersonalRoles] as [per] 
				INNER JOIN [TST-TESSA-DB].[MigrationSED].[dbo].[Users] as [us] ON [us].[ID] = [obj].[ProjectManager] AND [us].[UniqueId] = [per].[ExtID])
				--админ проекта
				,(SELECT TOP 1 [per].[ID] FROM [PersonalRoles] as [per] 
				INNER JOIN [TST-TESSA-DB].[MigrationSED].[dbo].[Users] as [us] ON [us].[ID] = [obj].[ProjectAdmin] AND [us].[UniqueId] = [per].[ExtID])
				,(SELECT TOP 1 [per].[Name] FROM [PersonalRoles] as [per] 
				INNER JOIN [TST-TESSA-DB].[MigrationSED].[dbo].[Users] as [us] ON [us].[ID] = [obj].[ProjectAdmin] AND [us].[UniqueId] = [per].[ExtID])
				--экономист проекта
				,(SELECT TOP 1 [per].[ID] FROM [PersonalRoles] as [per] 
				INNER JOIN [TST-TESSA-DB].[MigrationSED].[dbo].[Users] as [us] ON [us].[ID] = [obj].[ProjectEconomist] AND [us].[UniqueId] = [per].[ExtID])
				,(SELECT TOP 1 [per].[Name] FROM [PersonalRoles] as [per] 
				INNER JOIN [TST-TESSA-DB].[MigrationSED].[dbo].[Users] as [us] ON [us].[ID] = [obj].[ProjectEconomist] AND [us].[UniqueId] = [per].[ExtID])
				--гип проекта
				,(SELECT TOP 1 [per].[ID] FROM [PersonalRoles] as [per] 
				INNER JOIN [TST-TESSA-DB].[MigrationSED].[dbo].[Users] as [us] ON [us].[ID] = [obj].[Chief] AND [us].[UniqueId] = [per].[ExtID])
				,(SELECT TOP 1 [per].[Name] FROM [PersonalRoles] as [per] 
				INNER JOIN [TST-TESSA-DB].[MigrationSED].[dbo].[Users] as [us] ON [us].[ID] = [obj].[Chief] AND [us].[UniqueId] = [per].[ExtID])
				--сметчик проекта
				,(SELECT TOP 1 [per].[ID] FROM [PersonalRoles] as [per] 
				INNER JOIN [TST-TESSA-DB].[MigrationSED].[dbo].[Users] as [us] ON [us].[ID] = [obj].[Estimator] AND [us].[UniqueId] = [per].[ExtID])
				,(SELECT TOP 1 [per].[Name] FROM [PersonalRoles] as [per] 
				INNER JOIN [TST-TESSA-DB].[MigrationSED].[dbo].[Users] as [us] ON [us].[ID] = [obj].[Estimator] AND [us].[UniqueId] = [per].[ExtID])
				--инж пто
				,(SELECT TOP 1 [per].[ID] FROM [PersonalRoles] as [per] 
				INNER JOIN [TST-TESSA-DB].[MigrationSED].[dbo].[Users] as [us] ON [us].[ID] = [obj].[EngineerPTO] AND [us].[UniqueId] = [per].[ExtID])
				,(SELECT TOP 1 [per].[Name] FROM [PersonalRoles] as [per] 
				INNER JOIN [TST-TESSA-DB].[MigrationSED].[dbo].[Users] as [us] ON [us].[ID] = [obj].[EngineerPTO] AND [us].[UniqueId] = [per].[ExtID])
				--руководитель строительства 
				,(SELECT TOP 1 [per].[ID] FROM [PersonalRoles] as [per] 
				INNER JOIN [TST-TESSA-DB].[MigrationSED].[dbo].[Users] as [us] ON [us].[ID] = [obj].[ConstructionManager] AND [us].[UniqueId] = [per].[ExtID])
				,(SELECT TOP 1 [per].[Name] FROM [PersonalRoles] as [per] 
				INNER JOIN [TST-TESSA-DB].[MigrationSED].[dbo].[Users] as [us] ON [us].[ID] = [obj].[ConstructionManager] AND [us].[UniqueId] = [per].[ExtID])
				,CASE WHEN [obj].[RubricLinkStateId] = N'Завершен' THEN 1 ELSE 0 END
				,[obj].[UniqueId]
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