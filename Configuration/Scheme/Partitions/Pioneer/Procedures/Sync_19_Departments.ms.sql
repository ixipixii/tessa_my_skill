CREATE PROCEDURE [Sync_19_Departments]
AS
BEGIN

	BEGIN TRANSACTION;

	BEGIN TRY		

			DECLARE @CardType UNIQUEIDENTIFIER = 'abe57cb7-e1cb-06f6-b7ca-ad1668bebd72' -- DepartmentRole
			DECLARE @CardTypeCaption NVARCHAR(128) = (SELECT TOP 1 Caption FROM [Types] WITH (NOLOCK) WHERE ID = @CardType)

			DECLARE @CreatedBy UNIQUEIDENTIFIER = '11111111-1111-1111-1111-111111111111' -- System
			DECLARE @CreatedByName NVARCHAR(128) = (SELECT TOP 1 [Name] FROM PersonalRoles WITH (NOLOCK) WHERE ID = @CreatedBy)

			DROP TABLE IF EXISTS #ObjectsToUpdate

			CREATE TABLE #ObjectsToUpdate
			(
			     [ID] [uniqueidentifier] NOT NULL
			    ,[ExtID] [int] NOT NULL
			    ,[Title] [nvarchar](max) NULL
			    ,[DepartmentIndex] [nvarchar](255) NULL
			    ,[UserId] [int] NULL
			    ,[FileDirref] [nvarchar](max)  NULL
			    ,[DepartmentLink] [int] NULL
			    ,[Chief] [int] NULL
			    ,[Supervisor] [int] NULL
			)
			
			DECLARE @script VARCHAR(max) = 
			'SELECT NEWID() AS [ID]
				      ,[Id] AS [ExtID]
				      ,[Title]
				      ,[DepartmentIndex]
				      ,[UserId]
				      ,[FileDirref]
				      ,[DepartmentLink]
				      ,[Chief]
				      ,[Supervisor]
  			FROM [TST-TESSA-DB].[MigrationSED].[dbo].[Departments]
  			WHERE [DepartmentIndex] IS NOT NULL'
			INSERT INTO #ObjectsToUpdate
				EXECUTE (@script)

			--обновим существующие записи
			UPDATE [pr] 
				SET
				[pr].[HeadUserID] = (SELECT TOP(1) [p].[ID] FROM [PersonalRoles] as [p]
				INNER JOIN [TST-TESSA-DB].[MigrationSED].[dbo].[Users] as [u] ON [u].[UniqueId] = [p].[ExtID] AND [u].[ID] = [obj].[Chief])
				,[pr].[HeadUserName] = (SELECT TOP(1) [p].[Name] FROM [PersonalRoles] as [p]
				INNER JOIN [TST-TESSA-DB].[MigrationSED].[dbo].[Users] as [u] ON [u].[UniqueId] = [p].[ExtID] AND [u].[ID] = [obj].[Chief])
				,[pr].[CuratorID] = (SELECT TOP(1) [p].[ID] FROM [PersonalRoles] as [p]
				INNER JOIN [TST-TESSA-DB].[MigrationSED].[dbo].[Users] as [u] ON [u].[UniqueId] = [p].[ExtID] AND [u].[ID] = [obj].[Supervisor])
				,[pr].[CuratorName] = (SELECT TOP(1) [p].[Name] FROM [PersonalRoles] as [p]
				INNER JOIN [TST-TESSA-DB].[MigrationSED].[dbo].[Users] as [u] ON [u].[UniqueId] = [p].[ExtID] AND [u].[ID] = [obj].[Supervisor])			
			FROM #ObjectsToUpdate as [obj]
			INNER JOIN [DepartmentRoles] as [pr] ON [pr].[ExtID] = [obj].[ExtID]
			
			UPDATE [pr] 
				SET
				[pr].[Name] = [obj].[Title]
				,[pr].[Idx] = [obj].[DepartmentIndex]
			FROM #ObjectsToUpdate as [obj]
			INNER JOIN [DepartmentRoles] as [dep] ON [dep].[ExtID] = [obj].[ExtID]
			INNER JOIN [Roles] as [pr] ON [pr].[ID] = [dep].[ID]
			
			--очистим выгрузку от существующих записей
			DELETE #ObjectsToUpdate WHERE [ExtID] IN (SELECT [ExtID] from [DepartmentRoles] WHERE [ExtID] IS NOT NULL)
				
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
			
			INSERT INTO [DepartmentRoles]
			(
				[ID]
				,[HeadUserID]
				,[HeadUserName]
				,[CuratorID]
				,[CuratorName]
                		,[ExtID]
			)
			SELECT
				[obj].[ID]
				,(SELECT TOP(1) [p].[ID] FROM [PersonalRoles] as [p]
				INNER JOIN [TST-TESSA-DB].[MigrationSED].[dbo].[Users] as [u] ON [u].[UniqueId] = [p].[ExtID] AND [u].[ID] = [obj].[Chief]) AS [HeadUserID]
				,(SELECT TOP(1) [p].[Name] FROM [PersonalRoles] as [p]
				INNER JOIN [TST-TESSA-DB].[MigrationSED].[dbo].[Users] as [u] ON [u].[UniqueId] = [p].[ExtID] AND [u].[ID] = [obj].[Chief]) AS [HeadUserName]
				,(SELECT TOP(1) [p].[ID] FROM [PersonalRoles] as [p]
				INNER JOIN [TST-TESSA-DB].[MigrationSED].[dbo].[Users] as [u] ON [u].[UniqueId] = [p].[ExtID] AND [u].[ID] = [obj].[Supervisor]) AS [CuratorID]
				,(SELECT TOP(1) [p].[Name] FROM [PersonalRoles] as [p]
				INNER JOIN [TST-TESSA-DB].[MigrationSED].[dbo].[Users] as [u] ON [u].[UniqueId] = [p].[ExtID] AND [u].[ID] = [obj].[Supervisor]) AS [CuratorName]
				,[obj].[ExtID]
			FROM
				#ObjectsToUpdate as [obj]
				
			INSERT INTO [Roles]
			(
				ID, 
				Name, 
				Hidden, 
				AdSyncIndependent, 
				InheritTimeZone, 
				TypeID,
				Idx
			)
			SELECT 
				[obj].[ID]
				,[obj].[Title]
				,0
				,0
				,1
				,2
				,[obj].[DepartmentIndex]
			FROM #ObjectsToUpdate as [obj]
			
			--сотрудники подразделения
			INSERT INTO [RoleUsers] 
			(
				[ID]
				,[RowID]
				,[TypeID]
				,[UserID]
				,[UserName]
				,[IsDeputy]
			)
			SELECT 
				[obj].[ID] as [ID]
				,NEWID() AS [RowID]
				,2 AS [TypeID]
				,[pr].[ID] as [UserID]
				,[pr].[Name] as [UserName]
				,0 AS [IsDeputy]
			FROM  	#ObjectsToUpdate as [obj]
			INNER JOIN [DepartmentRoles] as [dep] ON [dep].[ExtID] = [obj].[ExtID]
			INNER JOIN [TST-TESSA-DB].[MigrationSED].[dbo].[Users] as [u] ON [obj].[ExtID] = [u].[Department_ID] AND [u].[Department_ID] IS NOT NULL
			INNER JOIN [PersonalRoles] as [pr] ON  [u].[UniqueId] = [pr].[ExtID]
			WHERE NOT EXISTS (SELECT * FROM [RoleUsers] WHERE [ID]=[obj].[ID] AND [TypeID] = 2)
			
			-- проставим связи между подразделениями (родитель-потомок)
			update roles
			set roles.ParentID = parentRoles.ID, roles.ParentName = parentRoles.Name
			from DepartmentRoles dep
			join Roles roles on dep.ID = roles.ID
			join MigrationSED.dbo.Departments src on src.Id = dep.ExtID
			join DepartmentRoles parentDep on parentDep.ExtID = src.ParentId
			join Roles parentRoles on parentRoles.ID = parentDep.ID
			WHERE src.[DepartmentIndex] IS NOT NULL -- это подразделение
				and src.ParentId is not null -- есть ссылка на родителя
				and (roles.ParentID is null or roles.ParentID != parentRoles.ID) -- не будем обновлять ссылку уже у связанных подразделений

				
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