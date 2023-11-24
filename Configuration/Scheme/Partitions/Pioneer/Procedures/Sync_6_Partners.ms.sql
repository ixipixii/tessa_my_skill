CREATE PROCEDURE [Sync_6_Partners]
AS
BEGIN

	BEGIN TRANSACTION;

	BEGIN TRY		
		
			DECLARE @CardType UNIQUEIDENTIFIER = 'b9a1f125-ab1d-4cff-929f-5ad8351bda4f' -- Partner
			DECLARE @CardTypeCaption NVARCHAR(128) = (SELECT TOP 1 Caption FROM [Types] WITH (NOLOCK) WHERE ID = @CardType)

			DECLARE @CreatedBy UNIQUEIDENTIFIER = '11111111-1111-1111-1111-111111111111' -- System
			DECLARE @CreatedByName NVARCHAR(128) = (SELECT TOP 1 [Name] FROM PersonalRoles WITH (NOLOCK) WHERE ID = @CreatedBy)

			DROP TABLE IF EXISTS #ObjectsToUpdate

			CREATE TABLE #ObjectsToUpdate
			(
			     [ID] [uniqueidentifier] NOT NULL
			    ,[Title] [nvarchar](max) NULL
			    ,[FullName] [nvarchar](max) NULL
			    ,[ContactType] [nvarchar](max) NULL
			    ,[Created] [datetime] NULL
			    ,[Email] [nvarchar](max) NULL
			    ,[INN] [nvarchar](max) NULL
			    ,[KPP] [nvarchar](max) NULL
			    ,[Comments] [nvarchar](max) NULL
		           ,[MDM_Key] [nvarchar](max) NULL
		           ,[BlackList] [nvarchar](max) NULL
			)
			
			DECLARE @script VARCHAR(max) = 
			'SELECT NEWID() AS [ID]
				      ,[Title]
				      ,[FullName]
				      ,[ContactType]
				      ,[Created]
				      ,[Email]
				      ,[INN]
				      ,[KPP]
				      ,[Comments]
				      ,[MDM_Key]
				      ,[BlackList]
  			FROM [TST-TESSA-DB].[MigrationSED].[dbo].[Contractors]
  			WHERE [Title] IS NOT NULL and MDM_Key is not null'
			INSERT INTO #ObjectsToUpdate
				EXECUTE (@script)		
				
			--обновим существующие записи
			UPDATE [proj] 
				SET
				[proj].[Name] = [obj].[Title]
				,[proj].[FullName] = [obj].[FullName]
				,[proj].[DateCreation] = [obj].[Created]
				,[proj].[Email] = [obj].[Email]
				,[proj].[INN] = [obj].[INN]
				,[proj].[KPP] = [obj].[KPP]
				,[proj].[MDMKey] = [obj].[MDM_Key]
				,[proj].[StatusID] = CASE WHEN [obj].[BlackList] = N'Согласован' THEN 0
				WHEN [obj].[BlackList] = N'Не согласован/В черном списке' THEN 1 END
				,[proj].[StatusName] = CASE WHEN [obj].[BlackList] = N'Согласован' THEN N'Согласован'
				WHEN [obj].[BlackList] = N'Не согласован/В черном списке' THEN N'Не согласован' END
			FROM #ObjectsToUpdate as [obj]
			INNER JOIN [Partners] as [proj] ON [proj].[MDMKey] = [obj].[MDM_Key] 
			WHERE [obj].[MDM_Key] IS NOT NULL
			AND [proj].[MDMKey] IS NOT NULL
			
			--очистим выгрузку от существующих записей
			DELETE #ObjectsToUpdate WHERE [MDM_Key] IN (SELECT [MDMKey] from [Partners] WHERE [MDMKey] IS NOT NULL)
				
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
			
			INSERT INTO [Partners]
			(
				[ID]
				,[Name]
				,[FullName]
				,[DateCreation]
				,[Email]
				,[INN]
				,[KPP]
				,[MDMKey]
				,[StatusID]
				,[StatusName]
			)
			SELECT
				[obj].[ID]
				,[obj].[Title]
			      ,[obj].[FullName]
			      ,[obj].[Created]
			      ,[obj].[Email]
			      ,[obj].[INN]
			      ,[obj].[KPP]
			      ,[obj].[MDM_Key]
			      ,CASE WHEN [obj].[BlackList] = N'Согласован' THEN 0
				WHEN [obj].[BlackList] = N'Не согласован/В черном списке' THEN 1 END AS [StatusID]
				,CASE WHEN [obj].[BlackList] = N'Согласован' THEN N'Согласован'
				WHEN [obj].[BlackList] = N'Не согласован/В черном списке' THEN N'Не согласован' END AS [StatusName]
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