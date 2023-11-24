CREATE PROCEDURE [Sync_15_ContractsKIS]
AS
BEGIN

	BEGIN TRANSACTION;

	BEGIN TRY		
		
		DECLARE @CardType UNIQUEIDENTIFIER = '9c8da932-22bc-45d1-9cb7-6edb7d97698b' -- PnrArchiveKIS
		DECLARE @CardTypeCaption NVARCHAR(128) = (SELECT TOP 1 Caption FROM [Types] WITH (NOLOCK) WHERE ID = @CardType)

		DECLARE @CreatedBy UNIQUEIDENTIFIER = '11111111-1111-1111-1111-111111111111' -- System
		DECLARE @CreatedByName NVARCHAR(128) = (SELECT TOP 1 [Name] FROM PersonalRoles WITH (NOLOCK) WHERE ID = @CreatedBy)
		
		DECLARE @CardTypeState UNIQUEIDENTIFIER = 'D819B56B-D6A3-447C-B257-CE53930E7ABE' 
		DECLARE @CardTypeStateCaption NVARCHAR(128) = (SELECT TOP 1 Caption FROM [Types] WITH (NOLOCK) WHERE ID = @CardTypeState)		

		DECLARE @CreationDate datetime2 = CONVERT(varchar(23), '2020-11-26 00:00:00.000', 121)

		DROP TABLE IF EXISTS #ObjectsToUpdate

		CREATE TABLE #ObjectsToUpdate
		(
			[ID] [uniqueidentifier] NOT NULL,
			[UniqueId] [uniqueidentifier] NOT NULL,
			[DocUniqueId] [uniqueidentifier] NULL,
			[DocNumber] [nvarchar](max) NULL,
			[Sum] [real] NULL,
			[Entity_Name] [nvarchar](max) NULL,
			[Contractor_Name] [nvarchar](max) NULL,
			[Title] [nvarchar](max) NULL,
			[Created] [datetime] NULL,
			[Modified] [datetime] NULL,
			[Author_ID] [int] NULL,
			[Author_Name] [nvarchar](max) NULL,
			[Author_Login] [nvarchar](max) NULL,
			[Author_Mail] [nvarchar](max) NULL,
			[Editor_ID] [int] NULL,
			[Editor_Name] [nvarchar](max) NULL,
			[Editor_Login] [nvarchar](max) NULL,
			[Editor_Mail] [nvarchar](max) NULL
		)
			
		DECLARE @script VARCHAR(max) = 
		'SELECT NEWID() AS [ID], t.*
			from
			(
				select distinct [UniqueId]
						,[DocUniqueId]
						,[DocNumber]
						,[Sum]
						,[Entity_Name]
						,[Contractor_Name]
						,[Title]
						,[Created]
						,[Modified]
						,[Author_ID]
						,[Author_Name]
						,[Author_Login]
						,[Author_Mail]
						,[Editor_ID]
						,[Editor_Name]
						,[Editor_Login]
						,[Editor_Mail]
					FROM [TST-TESSA-DB].[MigrationSED].[dbo].[ContractsKIS] WITH(NOLOCK)
					WHERE [UniqueId] IS NOT NULL
			) t'
		
		INSERT INTO #ObjectsToUpdate
		EXECUTE (@script)		
				
		--обновим существующие записи
		UPDATE [proj] 
		SET
			[proj].[Amount] = [obj].[Sum]
			,[proj].[Number] = [obj].[DocNumber]
			,[proj].[Created] = [obj].[Created]
			,[proj].[Modified] = [obj].[Modified]
			,[proj].[AuthorID] = [obj].[Author_ID]
			,[proj].[AuthorFio] = [obj].[Author_Name]
			,[proj].[AuthorLogin] = [obj].[Author_Login]
			,[proj].[AuthorEmail] = [obj].[Author_Mail]
			,[proj].[EditorID] = [obj].[Editor_ID]
			,[proj].[EditorFio] = [obj].[Editor_Name]
			,[proj].[EditorLogin] = [obj].[Editor_Login]
			,[proj].[EditorEmail] = [obj].[Editor_Mail]
			,[proj].[AuthorInTessaID] = CASE WHEN [obj].[Author_Login] LIKE N'%system%' THEN @CreatedBy ELSE (select top(1) t.ID from PersonalRoles as t where [obj].[Author_Login] COLLATE Cyrillic_General_CI_AS = t.Login) END
			,[proj].[AuthorInTessaName] = CASE WHEN [obj].[Author_Login] LIKE N'%system%' THEN @CreatedByName ELSE (select top(1) t.Name from PersonalRoles as t where [obj].[Author_Login] COLLATE Cyrillic_General_CI_AS = t.Login) END
			,[proj].[EditorInTessaID] = CASE WHEN [obj].[Editor_Login] LIKE N'%system%' THEN @CreatedBy ELSE (select top(1) t.ID from PersonalRoles as t where [obj].[Editor_Login] COLLATE Cyrillic_General_CI_AS = t.Login) END
			,[proj].[EditorInTessaName] = CASE WHEN [obj].[Editor_Login] LIKE N'%system%' THEN @CreatedByName ELSE (select top(1) t.Name from PersonalRoles as t where [obj].[Editor_Login] COLLATE Cyrillic_General_CI_AS = t.Login) END
		FROM #ObjectsToUpdate as [obj] WITH(NOLOCK)
		INNER JOIN [PnrArchiveKIS] as [proj] WITH(NOLOCK) ON [proj].[ExtID] = [obj].[UniqueId]
			
		UPDATE [proj] 
		SET
			[proj].[Subject] = [obj].[Title]
			,[proj].[FullNumber] = [obj].[DocNumber]
		FROM #ObjectsToUpdate as [obj] WITH(NOLOCK)
		INNER JOIN [PnrArchiveKIS] as [inc] WITH(NOLOCK) ON [inc].[ExtID] = [obj].[UniqueId]
		INNER JOIN [DocumentCommonInfo] as [proj] WITH(NOLOCK) ON  [proj].[ID] = [inc].[ID]
			
		--очистим выгрузку от существующих записей
		DELETE #ObjectsToUpdate WHERE [UniqueId] IN (SELECT [ExtID] from [PnrArchiveKIS] WHERE [ExtID] IS NOT NULL)
			
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
				
		INSERT INTO [dbo].[PnrArchiveKIS]
		(
			[ID]
			,[Amount]
			,[ExtID]
			,[Created]
			,[Modified]
			,[AuthorID]
			,[AuthorFio]
			,[AuthorLogin]
			,[AuthorEmail]
			,[EditorID]
			,[EditorFio]
			,[EditorLogin]
			,[EditorEmail]
			,[AuthorInTessaID]
			,[AuthorInTessaName]
			,[EditorInTessaID]
			,[EditorInTessaName]
		)
		SELECT 
			[obj].[ID]				  
			,[obj].[Sum] AS [Amount]
			,[obj].[UniqueId] AS [ExtID]
			,[obj].[Created] AS [Created]
			,[obj].[Modified] AS [Modified]
			,[obj].[Author_ID] AS [AuthorID]
			,[obj].[Author_Name] AS [AuthorFio]
			,[obj].[Author_Login] AS [AuthorLogin]
			,[obj].[Author_Mail] AS [AuthorEmail]
			,[obj].[Editor_ID] AS [EditorID]
			,[obj].[Editor_Name] AS [EditorFio]
			,[obj].[Editor_Login] AS [EditorLogin]
			,[obj].[Editor_Mail] AS [EditorEmail]
		,COALESCE
		(
			(select top(1) t.ID from PersonalRoles as t where [obj].[Author_Login] COLLATE Cyrillic_General_CI_AS = t.Login),
			NULL
		) AS [AuthorInTessaID]
		,COALESCE
		(
			(select top(1) t.Name from PersonalRoles as t where [obj].[Author_Login] COLLATE Cyrillic_General_CI_AS = t.Login),
			NULL
		) AS [AuthorInTessaName]
		,COALESCE
		(
			(select top(1) t.ID from PersonalRoles as t where [obj].[Editor_Login] COLLATE Cyrillic_General_CI_AS = t.Login),
			NULL
		) AS [EditorInTessaID]
		,COALESCE
		(
			(select top(1) t.Name from PersonalRoles as t where [obj].[Editor_Login] COLLATE Cyrillic_General_CI_AS = t.Login),
			NULL
		) AS [EditorInTessaName]
		FROM #ObjectsToUpdate AS [obj]
				
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
			,ISNULL(SUBSTRING(t.[Title],0, 440) COLLATE Cyrillic_General_CI_AS, N' ')  as [Subject]
			,t.[DocNumber] as [FullNumber]
			,@CardType
			,@CardTypeCaption
			,@CardTypeCaption
			,@CreatedBy
			,@CreatedByName
			,@CreationDate
		FROM 
			#ObjectsToUpdate t
				
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