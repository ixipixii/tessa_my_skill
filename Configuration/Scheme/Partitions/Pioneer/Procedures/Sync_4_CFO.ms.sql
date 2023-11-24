CREATE PROCEDURE [Sync_4_CFO]
AS
BEGIN

	BEGIN TRANSACTION;

	BEGIN TRY		
		
			DECLARE @CardType UNIQUEIDENTIFIER = '38bbd7ed-ab6f-4a12-81c2-ea0069da316f' -- PnrCFO
			DECLARE @CardTypeCaption NVARCHAR(128) = (SELECT TOP 1 Caption FROM [Types] WITH (NOLOCK) WHERE ID = @CardType)

			DECLARE @CreatedBy UNIQUEIDENTIFIER = '11111111-1111-1111-1111-111111111111' -- System
			DECLARE @CreatedByName NVARCHAR(128) = (SELECT TOP 1 [Name] FROM PersonalRoles WITH (NOLOCK) WHERE ID = @CreatedBy)

			DROP TABLE IF EXISTS #ObjectsToUpdate

			CREATE TABLE #ObjectsToUpdate
			(
				[ID] [uniqueidentifier] NOT NULL
			    ,[Title] [nvarchar](max) NULL
		           ,[MDM_Key] [nvarchar](max) NULL
		           ,[Parent_MDM_Key] [nvarchar](max) NULL
			)
			
			DECLARE @script VARCHAR(max) = 
			'SELECT NEWID() AS [ID]
  				       ,[Title]
      					,[MDM_Key]
      					,[Parent_MDM_Key]
  			FROM [TST-TESSA-DB].[MigrationSED].[dbo].[CFO]
  			WHERE [Title] IS NOT NULL'
			INSERT INTO #ObjectsToUpdate
				EXECUTE (@script)		
				
			--обновим существующие записи
			UPDATE [proj] 
				SET
				[proj].[Name] = [obj].[Title]
				,[proj].[MDMKey] = [obj].[MDM_Key]
			FROM #ObjectsToUpdate as [obj]
			INNER JOIN [PnrCFO] as [proj] ON [proj].[MDMKey] = [obj].[MDM_Key]
			
			--обновим родительские записи
			UPDATE [pnr] SET
			[pnr].[ParentCFOID] = (select top 1 [ID] from [PnrCFO] as [t] where [t].[MDMKey] = [obj].[Parent_MDM_Key]),
			[pnr].[ParentCFOName] = (select top 1 [Name] from [PnrCFO] as [t] where [t].[MDMKey] = [obj].[Parent_MDM_Key])
			from [PnrCFO] as [pnr]
			inner join #ObjectsToUpdate as [obj] on [obj].[MDM_Key] = [pnr].[MDMKey]
			
			--очистим выгрузку от существующих записей
			DELETE #ObjectsToUpdate WHERE [MDM_Key] IN (SELECT [MDMKey] from [PnrCFO] WHERE [MDMKey] IS NOT NULL)
				
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
			
			INSERT INTO [PnrCFO]
			(
				[ID]
				,[Name]
				,[MDMKey]
			)
			SELECT
				[obj].[ID]
				,[obj].[Title]
				,[obj].[MDM_Key]
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