CREATE PROCEDURE [Sync_10_Incoming]
AS
BEGIN

	BEGIN TRANSACTION;

	BEGIN TRY		

		DECLARE @CardType UNIQUEIDENTIFIER = '476fa752-133d-4571-8f28-86002241f2fe' -- PnrIncoming
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
			[Contragent_Title] [nvarchar](max) NULL,
			[Original] [nvarchar](max) NULL,
			[PioneerFullName] [nvarchar](max) NULL,
			[OutgoingDate] [datetime] NULL,
			[DeliveryType] [nvarchar](max) NULL,
			[Project_MDM_Key] [nvarchar](max) NULL,
			[Org_MDM_Key] [nvarchar](max) NULL,
			[OutgoingNumber] [nvarchar](max) NULL,
			[Annotation] [nvarchar](max) NULL,
			[Department] [nvarchar](max) NULL,
			[PioneerGroupDossier] [nvarchar](max) NULL,
			[Comment] [nvarchar](max) NULL,
			[DocTypeTESSA] [nvarchar](max) NULL,
			[MailId] [nvarchar](max) NULL,
			[PioneerContact] [nvarchar](max) NULL,
			[ComplaintType] [nvarchar](max) NULL,
			[FlatNumber] [nvarchar](max) NULL,
			[ClaimDelivery] [nvarchar](max) NULL,
			[Contragent_FullName] [nvarchar](max) NULL,
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
			,[Contragent_Title]
			,[Original]
			,[PioneerFullName]
			,[OutgoingDate]
			,[DeliveryType]
			,[Project_MDM_Key]
			,[Org_MDM_Key]
			,[OutgoingNumber]
			,[Annotation]
			,[Department]
			,[PioneerGroupDossier]
			,[Comment]
			,[DocTypeTESSA]
			,[MailId]
			,[PioneerContact]
			,[ComplaintType]
			,[FlatNumber]
			,[ClaimDelivery]
			,[Contragent_FullName]
			,[Org_Title]
			,[Project_Title]
		FROM [TST-TESSA-DB].[MigrationSED].[dbo].[IncomingContractsTESSA] WITH(NOLOCK)
		WHERE [GroupTypeTESSA] = N''Входящие документы''
			AND [UniqueId] IS NOT NULL'

		INSERT INTO #ObjectsToUpdate
		EXECUTE (@script)		
				
		--обновим существующие записи
		UPDATE [proj] 
		SET
			[proj].[RegistrationDate] = [obj].[RegistrationDate]
			,[proj].[CorrespondentID] =(SELECT TOP(1) [ID] FROM [Partners] as [part] WITH(NOLOCK) WHERE [part].[Name] LIKE N'%' + [obj].[Contragent_Title] + N'%')
			,[proj].[CorrespondentName] =(SELECT TOP(1) [Name] FROM [Partners] as [part] WITH(NOLOCK) WHERE [part].[Name] LIKE N'%' + [obj].[Contragent_Title] + N'%')
			,[proj].[FullName] = [obj].[PioneerFullName]
			,[proj].[ExternalDate] = [obj].[OutgoingDate]
			,[proj].[ExternalNumber] = [obj].[OutgoingNumber]
			,[proj].[Summary] = [obj].[Annotation]
			,[proj].[Comments] = [obj].[Comment]
			,[proj].[MailID] = [obj].[MailId]
			,[proj].[Contacts] = [obj].[PioneerContact]
			,[proj].[ProjectID] = (SELECT TOP(1) [ID] FROM [PnrProjects] WHERE [MDMKey] = [obj].[Project_MDM_Key] AND [MDMKey] IS NOT NULL)
			,[proj].[ProjectName] = (SELECT TOP(1) [Name] FROM [PnrProjects] WHERE [MDMKey] = [obj].[Project_MDM_Key] AND [MDMKey] IS NOT NULL)
			,[proj].[ApartmentNumber] = [obj].[FlatNumber]
			,[proj].[ComplaintKindID] = (SELECT TOP(1) [ID] FROM [PnrComplaintKinds] WITH(NOLOCK) WHERE [Name] LIKE N'%' + [obj].[ComplaintType] + N'%')
			,[proj].[ComplaintKindName] = (SELECT TOP(1) [Name] FROM [PnrComplaintKinds] WITH(NOLOCK) WHERE [Name] LIKE N'%' + [obj].[ComplaintType] + N'%')
			,[proj].[DeliveryTypeID] = (SELECT TOP(1) [ID] FROM [PnrDeliveryTypes] WITH(NOLOCK) WHERE [Name] LIKE N'%' + [obj].[DeliveryType] + N'%')
			,[proj].[DeliveryTypeName] = (SELECT TOP(1) [Name] FROM [PnrDeliveryTypes] WITH(NOLOCK) WHERE [Name] LIKE N'%' + [obj].[DeliveryType] + N'%')
			,[proj].[LegalEntityIndexID] = (SELECT TOP(1) [ID] FROM [PnrLegalEntitiesIndexes] WITH(NOLOCK) WHERE [Name] LIKE N'%' + [obj].[PioneerGroupDossier] + N'%')
			,[proj].[LegalEntityIndexIdx] = (SELECT TOP(1) [Idx] FROM [PnrLegalEntitiesIndexes] WITH(NOLOCK) WHERE [Name] LIKE N'%' + [obj].[PioneerGroupDossier] + N'%')
			,[proj].[DocumentKindID] = COALESCE
			(
				(SELECT TOP(1) [ID] FROM [PnrIncomingDocumentsKinds] WITH(NOLOCK) WHERE [Name] LIKE N'%' + [obj].[DocTypeTESSA] + N'%'),
				CASE
					WHEN [obj].[DocTypeTESSA] = N'общие' THEN '8a58a6be-a235-43b4-bc2b-648a42799895'
					WHEN [obj].[DocTypeTESSA] = N'Рекламации' THEN 'a7e9340b-70f6-46b1-8049-00d9ec91e910'
					ELSE NULL
				END
			)
			,[proj].[DocumentKindName] = COALESCE
			(
				(SELECT TOP(1) [Name] FROM [PnrIncomingDocumentsKinds] WITH(NOLOCK) WHERE [Name] LIKE N'%' + [obj].[DocTypeTESSA] + N'%'),
				CASE
					WHEN [obj].[DocTypeTESSA] = N'общие' THEN N'Входящее письмо'
					WHEN [obj].[DocTypeTESSA] = N'Рекламации' THEN N'Рекламации'
					ELSE NULL
				END
			)
			,[proj].[DocumentKindIdx] = COALESCE
			(
				(SELECT TOP(1) [Idx] FROM [PnrIncomingDocumentsKinds] WITH(NOLOCK) WHERE [Name] LIKE N'%' + [obj].[DocTypeTESSA] + N'%'),
				CASE
					WHEN [obj].[DocTypeTESSA] = N'общие' THEN N'1-01'
					WHEN [obj].[DocTypeTESSA] = N'Рекламации' THEN N'1-02'
					ELSE NULL
				END
			)
			,[proj].[ComplaintFormatID] = (SELECT TOP(1) [ID] FROM [PnrComplaintFormats] WITH(NOLOCK) WHERE  LOWER(REPLACE([Name], ' ', ''))  LIKE N'%' + LOWER(REPLACE([obj].[ClaimDelivery], ' ', '')) + N'%')
			,[proj].[ComplaintFormatName] = (SELECT TOP(1) [Name] FROM [PnrComplaintFormats] WITH(NOLOCK) WHERE  LOWER(REPLACE([Name], ' ', ''))  LIKE N'%' + LOWER(REPLACE([obj].[ClaimDelivery], ' ', '')) + N'%')
			,[proj].[OriginalID] = (SELECT TOP(1) [ID] FROM [PnrOriginalsTypes] WITH(NOLOCK) WHERE  LOWER(REPLACE([Name], ' ', ''))  LIKE N'%' + LOWER(REPLACE([obj].[Original], ' ', '')) + N'%')
			,[proj].[OriginalName] = (SELECT TOP(1) [Name] FROM [PnrOriginalsTypes] WITH(NOLOCK) WHERE  LOWER(REPLACE([Name], ' ', ''))  LIKE N'%' + LOWER(REPLACE([obj].[Original], ' ', '')) + N'%')
			,[proj].[DepartmentID] = (SELECT TOP(1) [ID] FROM [Roles] WITH(NOLOCK) WHERE [TypeID] = 2 AND [Name] LIKE N'%' + [obj].[Department] + N'%') 
			,[proj].[DepartmentName] = (SELECT TOP(1) [Name] FROM [Roles] WITH(NOLOCK) WHERE [TypeID] = 2 AND [Name] LIKE N'%' + [obj].[Department] + N'%')
			,[proj].[DepartmentIdx] = (SELECT TOP(1) [Idx] FROM [Roles] WITH(NOLOCK) WHERE [TypeID] = 2 AND [Name] LIKE N'%' + [obj].[Department] + N'%')
		FROM #ObjectsToUpdate as [obj] WITH(NOLOCK)
		INNER JOIN [PnrIncoming] as [proj] WITH(NOLOCK) ON [proj].[ExtID] = [obj].[UniqueId]
			
		UPDATE [proj] 
		SET
			[proj].[Subject] = [obj].[Title]
			,[proj].[FullNumber] = [obj].[OuterNumber]
		FROM #ObjectsToUpdate as [obj] WITH(NOLOCK)
		INNER JOIN [PnrIncoming] as [inc] WITH(NOLOCK) ON [inc].[ExtID] = [obj].[UniqueId]
		INNER JOIN [DocumentCommonInfo] as [proj] WITH(NOLOCK) ON  [proj].[ID] = [inc].[ID]
			
		--очистим выгрузку от существующих записей
		DELETE #ObjectsToUpdate WHERE [UniqueId] IN (SELECT [ExtID] from [PnrIncoming] WHERE [ExtID] IS NOT NULL)
			
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
				
		INSERT INTO [dbo].[PnrIncoming]
		(
			[ID]
			,[RegistrationDate]
			,[CorrespondentID]
			,[CorrespondentName]
			,[FullName]
			,[ExternalDate]
			,[ExternalNumber]
			,[Summary]
			,[Comments]
			,[MailID]
			,[Contacts]
			,[ProjectID]
			,[ProjectName]
			,[ApartmentNumber]
			,[ComplaintKindID]
			,[ComplaintKindName]
			,[DeliveryTypeID]
			,[DeliveryTypeName]
			,[LegalEntityIndexID]
			,[LegalEntityIndexIdx]
			,[DocumentKindID]
			,[DocumentKindName]
			,[DocumentKindIdx]
			,[ComplaintFormatID]
			,[ComplaintFormatName]
			,[OriginalID]
			,[OriginalName]
			,[DepartmentID]
			,[DepartmentName]
			,[DepartmentIdx]
			,[ExtID]
		)
		SELECT 
			[obj].[ID]				  
			,[obj].[RegistrationDate]
			,(SELECT TOP(1) [ID] FROM [Partners] as [part] WITH(NOLOCK) WHERE [part].[Name] LIKE N'%' + [obj].[Contragent_Title] + N'%') AS [CorrespondentID]
			,(SELECT TOP(1) [Name] FROM [Partners] as [part] WITH(NOLOCK) WHERE [part].[Name] LIKE N'%' + [obj].[Contragent_Title] + N'%') AS [CorrespondentName]
			,[obj].[PioneerFullName] AS [FullName]
			,[obj].[OutgoingDate] AS [ExternalDate]
			,[obj].[OutgoingNumber] AS [ExternalNumber]
			,[obj].[Annotation] AS [Summary]
			,[obj].[Comment] AS [Comments]
			,[obj].[MailId] AS [MailID]
			,[obj].[PioneerContact] AS [Contacts]
			,(SELECT TOP(1) [ID] FROM [PnrProjects] WHERE [MDMKey] = [obj].[Project_MDM_Key] AND [MDMKey] IS NOT NULL) AS [ProjectID]
			,(SELECT TOP(1) [Name] FROM [PnrProjects] WHERE [MDMKey] = [obj].[Project_MDM_Key] AND [MDMKey] IS NOT NULL) AS [ProjectName]
			,[obj].[FlatNumber] AS [ApartmentNumber]
			,(SELECT TOP(1) [ID] FROM [PnrComplaintKinds] WITH(NOLOCK) WHERE [Name] LIKE N'%' + [obj].[ComplaintType] + N'%') AS [ComplaintKindID]
			,(SELECT TOP(1) [Name] FROM [PnrComplaintKinds] WITH(NOLOCK) WHERE [Name] LIKE N'%' + [obj].[ComplaintType] + N'%') AS [ComplaintKindName]
			,(SELECT TOP(1) [ID] FROM [PnrDeliveryTypes] WITH(NOLOCK) WHERE [Name] LIKE N'%' + [obj].[DeliveryType] + N'%') AS [DeliveryTypeID]
			,(SELECT TOP(1) [Name] FROM [PnrDeliveryTypes] WITH(NOLOCK) WHERE [Name] LIKE N'%' + [obj].[DeliveryType] + N'%') AS [DeliveryTypeName]
			,(SELECT TOP(1) [ID] FROM [PnrLegalEntitiesIndexes] WITH(NOLOCK) WHERE [Name] LIKE N'%' + [obj].[PioneerGroupDossier] + N'%') AS [LegalEntityIndexID]
			,(SELECT TOP(1) [Idx] FROM [PnrLegalEntitiesIndexes] WITH(NOLOCK) WHERE [Name] LIKE N'%' + [obj].[PioneerGroupDossier] + N'%') AS [LegalEntityIndexIdx]
			,COALESCE
			(
				(SELECT TOP(1) [ID] FROM [PnrIncomingDocumentsKinds] WITH(NOLOCK) WHERE [Name] LIKE N'%' + [obj].[DocTypeTESSA] + N'%'),
				CASE
					WHEN [obj].[DocTypeTESSA] = N'общие' THEN '8a58a6be-a235-43b4-bc2b-648a42799895'
					WHEN [obj].[DocTypeTESSA] = N'Рекламации' THEN 'a7e9340b-70f6-46b1-8049-00d9ec91e910'
					ELSE NULL
				END
			) AS [DocumentKindID]
			,COALESCE
			(
				(SELECT TOP(1) [Name] FROM [PnrIncomingDocumentsKinds] WITH(NOLOCK) WHERE [Name] LIKE N'%' + [obj].[DocTypeTESSA] + N'%'),
				CASE
					WHEN [obj].[DocTypeTESSA] = N'общие' THEN N'Входящее письмо'
					WHEN [obj].[DocTypeTESSA] = N'Рекламации' THEN N'Рекламации'
					ELSE NULL
				END
			) AS [DocumentKindName]
			,COALESCE
			(
				(SELECT TOP(1) [Idx] FROM [PnrIncomingDocumentsKinds] WITH(NOLOCK) WHERE [Name] LIKE N'%' + [obj].[DocTypeTESSA] + N'%'),
				CASE
					WHEN [obj].[DocTypeTESSA] = N'общие' THEN N'1-01'
					WHEN [obj].[DocTypeTESSA] = N'Рекламации' THEN N'1-02'
					ELSE NULL
				END
			) AS [DocumentKindIdx]
			,(SELECT TOP(1) [ID] FROM [PnrComplaintFormats] WITH(NOLOCK) WHERE  LOWER(REPLACE([Name], ' ', ''))  LIKE N'%' + LOWER(REPLACE([obj].[ClaimDelivery], ' ', '')) + N'%') AS [ComplaintFormatID]
			,(SELECT TOP(1) [Name] FROM [PnrComplaintFormats] WITH(NOLOCK) WHERE  LOWER(REPLACE([Name], ' ', ''))  LIKE N'%' + LOWER(REPLACE([obj].[ClaimDelivery], ' ', '')) + N'%') AS [ComplaintFormatName]
			,(SELECT TOP(1) [ID] FROM [PnrOriginalsTypes] WITH(NOLOCK) WHERE  LOWER(REPLACE([Name], ' ', ''))  LIKE N'%' + LOWER(REPLACE([obj].[Original], ' ', '')) + N'%') AS [OriginalID]
			,(SELECT TOP(1) [Name] FROM [PnrOriginalsTypes] WITH(NOLOCK) WHERE  LOWER(REPLACE([Name], ' ', ''))  LIKE N'%' + LOWER(REPLACE([obj].[Original], ' ', '')) + N'%') AS [OriginalName]
			,(SELECT TOP(1) [ID] FROM [Roles] WITH(NOLOCK) WHERE [TypeID] = 2 AND [Name] LIKE N'%' + [obj].[Department] + N'%') AS [DepartmentID]
			,(SELECT TOP(1) [Name] FROM [Roles] WITH(NOLOCK) WHERE [TypeID] = 2 AND [Name] LIKE N'%' + [obj].[Department] + N'%') AS [DepartmentName]
			,(SELECT TOP(1) [Idx] FROM [Roles] WITH(NOLOCK) WHERE [TypeID] = 2 AND [Name] LIKE N'%' + [obj].[Department] + N'%') AS [DepartmentIdx]
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
			,t.[OuterNumber] as [FullNumber]
			,@CardType
			,@CardTypeCaption
			,@CardTypeCaption
			,@CreatedBy
			,@CreatedByName
			,GETUTCDATE()
		FROM 
			#ObjectsToUpdate t
				
		INSERT INTO [PnrIncomingOrganizations] 
		(
			[ID]
			,[RowID]
			,[OrganizationID]
			,[OrganizationName]
		)
		SELECT 
			t.[ID]
			,NEWID() AS [RowID]
			,[org].[ID] AS [OrganizationID]
			,[org].[Name] AS [OrganizationName]
		FROM
		#ObjectsToUpdate AS [t]
		LEFT JOIN [PnrOrganizations] as [org] WITH(NOLOCK) ON [org].[MDMKey] = [t].[Org_MDM_Key] AND [org].[MDMKey] IS NOT NULL
				
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
			,'b387568e-2c2d-414c-b22a-dc11b252634f'
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