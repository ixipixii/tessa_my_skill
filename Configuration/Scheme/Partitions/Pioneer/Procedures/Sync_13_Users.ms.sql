CREATE PROCEDURE [Sync_13_Users]
AS
BEGIN

	BEGIN TRANSACTION;

	BEGIN TRY		
		
			DECLARE @CardType UNIQUEIDENTIFIER = '929ad23c-8a22-09aa-9000-398bf13979b2' -- PersonalRole
			DECLARE @CardTypeCaption NVARCHAR(128) = (SELECT TOP 1 Caption FROM [Types] WITH (NOLOCK) WHERE ID = @CardType)

			DECLARE @CreatedBy UNIQUEIDENTIFIER = '11111111-1111-1111-1111-111111111111' -- System
			DECLARE @CreatedByName NVARCHAR(128) = (SELECT TOP 1 [Name] FROM PersonalRoles WITH (NOLOCK) WHERE ID = @CreatedBy)

			DROP TABLE IF EXISTS #ObjectsToUpdate

			CREATE TABLE #ObjectsToUpdate
			(
			     [ID] [uniqueidentifier] NOT NULL
			    ,[ExtID] [uniqueidentifier] NOT NULL
			    ,[Title] [nvarchar](max) NULL
			    ,[Email] [nvarchar](max) NULL
			    ,[FirstName] [nvarchar](max) NULL
			    ,[LastName] [nvarchar](max)  NULL
			    ,[MiddleName] [nvarchar](max) NULL
			    ,[JobTitle] [nvarchar](max) NULL
			    ,[MobilePhone] [nvarchar](max) NULL
			    ,[Login] [nvarchar](max) NULL
		           ,[OfficialDisplay] [nvarchar](max) NULL
		           ,[UserDisplay] [nvarchar](max) NULL
		           ,[DepartmentID] [int] NULL
		           ,[DepartmentTitle] [nvarchar](255) NULL
		           ,[isActive] [bit] NULL
			)
			
			DECLARE @script VARCHAR(max) = 
			'SELECT NEWID() AS [ID]
				      ,t2.[UniqueId] AS [ExtID]
				      ,[Title]
				      ,[Email]
				      ,[FirstName]
				      ,[LastName]
				      ,[MiddleName]
				      ,[JobTitle]
				      ,[MobilePhone]
				      ,t2.[Login]
				      ,[OfficialDisplay]
      				      ,[UserDisplay]
      				      ,[Department_ID]
      				      ,[DepartmentTitle]
      				      ,[isActive]
  			FROM
			(
				SELECT 
					Min([UniqueId]) as [UniqueId]
					,[Login]
				FROM [TST-TESSA-DB].[MigrationSED].[dbo].[Users]
				GROUP BY [Login]
			) as t1
			INNER JOIN [TST-TESSA-DB].[MigrationSED].[dbo].[Users] as t2 ON t2.[UniqueId] = t1.[UniqueId]
  			WHERE [FirstName] IS NOT NULL'
			INSERT INTO #ObjectsToUpdate
				EXECUTE (@script)

			--обновим существующие записи
			UPDATE [pr] 
				SET
				[pr].[FirstName] = [obj].[FirstName]
				,[pr].[Name] = [obj].[UserDisplay]
				,[pr].[FullName] = [obj].[Title]
				,[pr].[LastName] = [obj].[LastName]
				,[pr].[MiddleName] = [obj].[MiddleName]
				,[pr].[Email] = [obj].[Email]
				,[pr].[Position] = [obj].[JobTitle]
				,[pr].[MobilePhone] = [obj].[MobilePhone]
				,[pr].[Login] = [obj].[Login]
				,[pr].[Blocked] = COALESCE(~[obj].[isActive], 0)
			FROM #ObjectsToUpdate as [obj]
			INNER JOIN [PersonalRoles] as [pr] ON [pr].[ExtID] = [obj].[ExtID]
			
			--очистим выгрузку от существующих записей
			DELETE #ObjectsToUpdate WHERE [ExtID] IN (SELECT [ExtID] from [PersonalRoles] WHERE [ExtID] IS NOT NULL)
				
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
			
			INSERT INTO [PersonalRoles]
			(
				[ID]
				,[FirstName]
				,[Name]
				,[FullName]
				,[LastName]
				,[MiddleName]
				,[Email]
				,[Position]
				,[MobilePhone]
				,[Login]
				,[AccessLevelID]
                		,[AccessLevelName]
                		,[Blocked]
                		,[LoginTypeID]
                		,[LoginTypeName]
                		,[SecurityLock]
                		,[ExtID]
			)
			SELECT
				[obj].[ID]
				,[obj].[FirstName] AS [FirstName]
				,[obj].[UserDisplay] AS [Name]
				,[obj].[Title]
				,[obj].[LastName]
				,[obj].[MiddleName]
				,[obj].[Email]
				,[obj].[JobTitle]
				,[obj].[MobilePhone]
				,[obj].[Login]
				,0
				,N'$Enum_AccessLevels_Regular'
				,COALESCE(~[obj].[isActive], 0)
				,2
				,N'$Enum_LoginTypes_Windows'
				,0
				,[obj].[ExtID]
			FROM
				#ObjectsToUpdate as [obj]
				
			INSERT INTO [Roles]
			(ID, Name, Hidden, AdSyncIndependent, InheritTimeZone, TypeID)
			SELECT ID, UserDisplay, 0, 0, 1, 1
			FROM #ObjectsToUpdate as [obj]
			
			--включение сотрудника в состав собственной же роли
			INSERT INTO RoleUsers (ID, RowID, TypeID, UserID, UserName, IsDeputy)
			SELECT pr.ID, newid(), 1, pr.ID, pr.Name, 0
			FROM PersonalRoles pr
			WHERE NOT EXISTS
			(
				SELECT top 1 1
				FROM RoleUsers ru
				WHERE ru.ID = pr.ID and ru.UserID = pr.ID
			)
			
			--подразделения
			INSERT INTO [RoleUsers] 
			(
			[ID]
			,[RowID]
			,[TypeID]
			,[UserID]
			,[UserName]
			,[IsDeputy]
			)
			SELECT [rol].[ID] as [ID]
			,NEWID() AS [RowID]
			,2 AS [TypeID]
			,[pr].[ID] as [UserID]
			,[pr].[Name] as [UserName]
			,0 AS [IsDeputy]
			from #ObjectsToUpdate as [obj]
			INNER JOIN [Roles] as [rol] on ISNULL([rol].[Name] COLLATE Cyrillic_General_CI_AS, 1) = ISNULL([obj].[DepartmentTitle] COLLATE Cyrillic_General_CI_AS, 1)
			AND [rol].[TypeID] = 2
			INNER JOIN [PersonalRoles] as [pr] ON [pr].[ExtID] = [obj].[ExtID]			
				
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