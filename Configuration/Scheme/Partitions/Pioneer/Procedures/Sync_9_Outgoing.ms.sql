CREATE PROCEDURE [Sync_9_Outgoing]
AS
BEGIN

	BEGIN TRANSACTION;

	BEGIN TRY		

		DECLARE @CardType UNIQUEIDENTIFIER = '40dab24a-0b6f-4609-947c-f1916348a540' -- PnrOutgoing
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
		WHERE GroupTypeTESSA = N''Исходящие документы''
			AND [UniqueId] IS NOT NULL
			AND NOT EXISTS (SELECT 1 from [PnrOutgoing] WHERE ExtID = [UniqueId])'

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
				
		INSERT INTO [dbo].[PnrOutgoing]
		(
			[ID]
			,[RegistrationDate]
			,[Summary]
			,[DestinationID]
			,[DestinationName]
			,[ProjectID]
			,[ProjectName]
			,[OrganizationID]
			,[OrganizationName]
			,[Comments]
			,[Contacts]
			,[ApartmentNumber]
			,[FullName]
			,[ComplaintKindID]
			,[ComplaintKindName]
			,[DepartmentID]
			,[DepartmentName]
			,[DepartmentIdx]
			,[DocumentKindID]
			,[DocumentKindName]
			,[DocumentKindIdx]
			,[ComplaintFormatID]
			,[ComplaintFormatName]
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
			,(SELECT TOP(1) [ID] FROM PnrProjects WHERE [MDMKey] = obj.[Project_MDM_Key] AND [MDMKey] IS NOT NULL) AS [ProjectID]
			,(SELECT TOP(1) [Name] FROM PnrProjects WHERE [MDMKey] = obj.[Project_MDM_Key] AND [MDMKey] IS NOT NULL) AS [ProjectName]
			,(SELECT TOP(1) [ID] FROM PnrOrganizations WHERE [MDMKey] = obj.[Org_MDM_Key] AND [MDMKey] IS NOT NULL) AS [OrganizationID]
			,(SELECT TOP(1) [Name] FROM PnrOrganizations WHERE [MDMKey] = obj.[Org_MDM_Key] AND [MDMKey] IS NOT NULL) AS [OrganizationName]
			,[obj].[Comment] as [Comments]
			,[obj].[PioneerContact] as [Contacts]
			,[obj].[FlatNumber] as [ApartmentNumber]
			,[obj].[PioneerFullName] as [FullName]
			,(SELECT TOP(1) [ID] FROM PnrComplaintKinds WITH(NOLOCK) WHERE [Name] LIKE N'%' + [obj].[ComplaintType] + N'%') AS [ComplaintKindID]
			,(SELECT TOP(1) [Name] FROM PnrComplaintKinds WITH(NOLOCK) WHERE [Name] LIKE N'%' + [obj].[ComplaintType] + N'%') AS [ComplaintKindName]
			,(SELECT TOP(1) [ID] FROM [Roles] WITH(NOLOCK) WHERE [TypeID] = 2 AND [Name] LIKE N'%' + [obj].[Department] + N'%') AS [DepartmentID]
			,(SELECT TOP(1) [Name] FROM [Roles] WITH(NOLOCK) WHERE [TypeID] = 2 AND [Name] LIKE N'%' + [obj].[Department] + N'%') AS [DepartmentName]
			,(SELECT TOP(1) [Idx] FROM [Roles] WITH(NOLOCK) WHERE [TypeID] = 2 AND [Name] LIKE N'%' + [obj].[Department] + N'%') AS [DepartmentIdx]
			,COALESCE
			(
				(SELECT TOP(1) [ID] FROM [PnrOutgoingDocumentsKinds] WITH(NOLOCK) WHERE [Name] LIKE N'%' + [obj].[DocTypeTESSA] + N'%'),
				CASE WHEN [obj].[DocTypeTESSA] = N'Исходящие документы' THEN 'f5d461c5-2237-4e68-b500-de6b17fc6b38'
				WHEN [obj].[DocTypeTESSA] = N'Ответы на рекламации' THEN '9eab450e-6c37-4542-8d0a-2da0f91c80c4'
				ELSE NULL END
			) AS [DocumentKindID]
			,COALESCE
			(
				(SELECT TOP(1) [Name] FROM [PnrOutgoingDocumentsKinds] WITH(NOLOCK) WHERE [Name] LIKE N'%' + [obj].[DocTypeTESSA] + N'%'),
				CASE WHEN [obj].[DocTypeTESSA] = N'Исходящие документы' THEN N'Исходящее письмо'
				WHEN [obj].[DocTypeTESSA] = N'Ответы на рекламации' THEN N'Ответы на рекламации'
				ELSE NULL END
			) AS [DocumentKindName]
			,COALESCE
			(
				(SELECT TOP(1) [Idx] FROM [PnrOutgoingDocumentsKinds] WITH(NOLOCK) WHERE [Name] LIKE N'%' + [obj].[DocTypeTESSA] + N'%'),
				CASE WHEN [obj].[DocTypeTESSA] = N'Исходящие документы' THEN N'2-01'
				WHEN [obj].[DocTypeTESSA] = N'Ответы на рекламации' THEN N'2-02'
				ELSE NULL END
			) AS [DocumentKindIdx]
			,(SELECT TOP(1) [ID] FROM [PnrComplaintFormats] WITH(NOLOCK) WHERE  LOWER(REPLACE([Name], ' ', ''))  LIKE N'%' + LOWER(REPLACE([obj].[ClaimDelivery], ' ', '')) + N'%') AS [ComplaintFormatID]
			,(SELECT TOP(1) [Name] FROM [PnrComplaintFormats] WITH(NOLOCK) WHERE LOWER(REPLACE([Name], ' ', ''))  LIKE N'%' + LOWER(REPLACE([obj].[ClaimDelivery], ' ', '')) + N'%') AS [ComplaintFormatName]
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
			,'91e9bca6-c547-40fe-96fe-0f1f3e32e6d2'
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