CREATE PROCEDURE [Sync_18_OutgoingUK]
AS
BEGIN

	BEGIN TRANSACTION;

	BEGIN TRY		
		
		DECLARE @CardType UNIQUEIDENTIFIER = '10e5967d-8282-4b43-89c6-8d8c9fd9558f' -- PnrOutgoingUK
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
			[OuterNumber] [nvarchar](max) NULL,
			[Author_Login] [nvarchar](max) NULL,
			[Author_ID] [int] NULL,
			[Author_Name] [nvarchar](max) NULL,
			[Author_Mail] [nvarchar](max) NULL,
			[Created] [datetime] NULL,
			[GroupTypeTESSA] [nvarchar](max) NULL,
			[DocState] [nvarchar](max) NULL,
			[RegistrationDate] [datetime] NULL,
			[Title] [nvarchar](max) NULL,
			[Annotation] [nvarchar](max) NULL,
			[Executors] [nvarchar](max) NULL,
			[OrgAddressee] [nvarchar](max) NULL,
			[PioneerFullName] [nvarchar](max) NULL,
			[PioneerGroupDossier] [nvarchar](max) NULL,
			[Project_MDM_Key] [nvarchar](max) NULL,
			[Org_MDM_Key] [nvarchar](max) NULL,
			[Department] [nvarchar](max) NULL,
			[Comment] [nvarchar](max) NULL,
			[Signer] [nvarchar](max) NULL,
			[DocTypeTESSA] [nvarchar](max) NULL,
			[PioneerContact] [nvarchar](max) NULL,
			[FlatNumber] [nvarchar](max) NULL,
			[ComplaintType] [nvarchar](max) NULL,
			[ClaimDelivery] [nvarchar](max) NULL,
			[Org_Title] [nvarchar](max) NULL,
			[Project_Title] [nvarchar](max) NULL
		)
			
		DECLARE @script VARCHAR(max) = 
		'SELECT NEWID() AS [ID]
			,[UniqueId]
			,[OuterNumber]
			,[Author_Login]
			,[Author_ID]
			,[Author_Name]
			,[Author_Mail]
			,[Created]
			,[GroupTypeTESSA]
			,[DocState]
			,[RegistrationDate]
			,[Title]
			,[Annotation]
			,[Executors]
			,[OrgAddressee]
			,[PioneerFullName]
			,[PioneerGroupDossier]
			,[Project_MDM_Key]
			,[Org_MDM_Key]
			,[Department]
			,[Comment]
			,[Signer]
			,[DocTypeTESSA]
			,[PioneerContact]
			,[FlatNumber]
			,[ComplaintType]
			,[ClaimDelivery]
			,[Org_Title]
			,[Project_Title]
		FROM [TST-TESSA-DB].[MigrationSED].[dbo].[OutgoingContractsTESSA]
		WHERE GroupTypeTESSA = N''Исходящие документы УК''
			AND [UniqueId] IS NOT NULL
			AND NOT EXISTS (SELECT 1 from [PnrOutgoingUK] WHERE ExtID = [UniqueId])'

		INSERT INTO #ObjectsToUpdate
		EXECUTE (@script)		
			
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
				
		INSERT INTO [dbo].[PnrOutgoingUK]
		(
			[ID]
			,[RegistrationDate]
			,[Summary]
			,[DestinationID]
			,[DestinationName]
			,[OrganizationID]
			,[OrganizationName]
			,[Comments]
			,[FullName]
			,[DepartmentID]
			,[DepartmentName]
			,[DepartmentIdx]
			,[LegalEntityIndexID]
			,[LegalEntityIndexName]
			,[LegalEntityIndexIdx]
			,[ExtID]
		)
		SELECT 
			[obj].[ID]				  
			,[obj].[RegistrationDate] AS [RegistrationDate]
			,[obj].[Annotation] as [Summary]
			,(SELECT TOP(1) [ID] FROM [Partners] WITH(NOLOCK) WHERE [Name] LIKE N'%' + [obj].[OrgAddressee] + N'%') AS [DestinationID]  
			,(SELECT TOP(1) [Name] FROM [Partners] WITH(NOLOCK) WHERE [Name] LIKE N'%' + [obj].[OrgAddressee] + N'%') AS [DestinationName]  
			,(SELECT TOP(1) [ID] FROM PnrOrganizations WHERE [MDMKey] = obj.[Org_MDM_Key] AND [MDMKey] IS NOT NULL) AS [OrganizationID]
			,(SELECT TOP(1) [Name] FROM PnrOrganizations WHERE [MDMKey] = obj.[Org_MDM_Key] AND [MDMKey] IS NOT NULL) AS [OrganizationName]
			,[obj].[Comment] as [Comments]
			,[obj].[PioneerFullName] as [FullName]
			,(SELECT TOP(1) [ID] FROM [Roles] WITH(NOLOCK) WHERE [TypeID] = 2 AND [Name] LIKE N'%' + [obj].[Department] + N'%') AS [DepartmentID]
			,(SELECT TOP(1) [Name] FROM [Roles] WITH(NOLOCK) WHERE [TypeID] = 2 AND [Name] LIKE N'%' + [obj].[Department] + N'%') AS [DepartmentName]
			,(SELECT TOP(1) [Idx] FROM [Roles] WITH(NOLOCK) WHERE [TypeID] = 2 AND [Name] LIKE N'%' + [obj].[Department] + N'%') AS [DepartmentIdx]
			,(SELECT TOP(1) [ID] FROM [PnrLegalEntitiesIndexes] WITH(NOLOCK) WHERE [Name] LIKE N'%' + [obj].[PioneerGroupDossier] + N'%') AS [LegalEntityIndexID]
			,(SELECT TOP(1) [Name] FROM [PnrLegalEntitiesIndexes] WITH(NOLOCK) WHERE [Name] LIKE N'%' + [obj].[PioneerGroupDossier] + N'%') AS [LegalEntityIndexName]
			,(SELECT TOP(1) [Idx] FROM [PnrLegalEntitiesIndexes] WITH(NOLOCK) WHERE [Name] LIKE N'%' + [obj].[PioneerGroupDossier] + N'%') AS [LegalEntityIndexIdx]
			,[obj].[UniqueId] as [ExtID]
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
			,'77766668-6220-411b-aeaa-1fdedc855699'
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