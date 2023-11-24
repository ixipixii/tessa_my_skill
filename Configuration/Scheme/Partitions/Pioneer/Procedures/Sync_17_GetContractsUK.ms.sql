CREATE PROCEDURE [Sync_17_GetContractsUK]
AS
BEGIN

	BEGIN TRANSACTION;

	BEGIN TRY		
		
		DECLARE @CardType UNIQUEIDENTIFIER = '25ea1e75-6ff9-4fd1-94e3-f6bc266d6544' -- PnrContractUK
		DECLARE @CardTypeCaption NVARCHAR(128) = (SELECT TOP 1 Caption FROM [Types] WITH (NOLOCK) WHERE ID = @CardType)
		
		DECLARE @CreatedBy UNIQUEIDENTIFIER = '11111111-1111-1111-1111-111111111111' -- System
		DECLARE @CreatedByName NVARCHAR(128) = (SELECT TOP 1 [Name] FROM PersonalRoles WITH (NOLOCK) WHERE ID = @CreatedBy)
		
		DECLARE @CardTypeState UNIQUEIDENTIFIER = 'D819B56B-D6A3-447C-B257-CE53930E7ABE' 
		DECLARE @CardTypeStateCaption NVARCHAR(128) = (SELECT TOP 1 Caption FROM [Types] WITH (NOLOCK) WHERE ID = @CardTypeState)		
		
		DECLARE @CreationDate datetime2 = CONVERT(datetime2, '2020-11-26 00:00:00.000', 121)
		
		DROP TABLE IF EXISTS #ObjectsToUpdate

		CREATE TABLE #ObjectsToUpdate
		(
			[ConID] [uniqueidentifier] NOT NULL,
			[UniqueId] [uniqueidentifier] NOT NULL,
			[DocNumber] [nvarchar](max) NULL,
			[IsContract] [nvarchar](max) NULL,
			[Author_ID] [int] NULL,
			[DocState] [nvarchar](max) NULL,
			[OuterNumber] [nvarchar](max) NULL,
			[Created] [datetime] NULL,
			[DocDisplay] [nvarchar](max) NULL,
			[Title] [nvarchar](max) NULL,
			[Contragent_MDM_Key] [nvarchar](max) NULL,
			[Org_MDM_Key] [nvarchar](max) NULL,
			[CFO_MDM_Key] [nvarchar](max) NULL,
			[Project_MDM_Key] [nvarchar](max) NULL,
			[Codifier_MDM_Key] [nvarchar](max) NULL,
			[DocSum] [decimal](20, 2) NULL,
			[DocPrepayment] [decimal](20, 2) NULL,
			[AdditionalContractSum] [decimal](20, 2) NULL,
			[Currency] [nvarchar](max) NULL,
			[InBudget] [bit] NULL,
			[TenderCompleted] [bit] NULL,
			[NDS_Type] [nvarchar](max) NULL,
			[DefermentOfPayment] [int] NULL,
			[DocForm] [nvarchar](max) NULL,
			[DocType] [nvarchar](max) NULL,
			[MDM_DocType] [nvarchar](max) NULL,
			[StartDate] [datetime] NULL,
			[EndDate] [datetime] NULL,
			[Comment] [nvarchar](max) NULL,
			[PrepaymentProcent] [nvarchar](max) NULL,
			[ReasonOfConclusion] [nvarchar](max) NULL,
			[WarrantyCommitments] [nvarchar](max) NULL,
			[DocTypeDUP] [nvarchar](max) NULL,
			[StartWorkConditions] [nvarchar](max) NULL,
			[Warranty] [nvarchar](max) NULL,
			[WarrantyRetentionProc] [nvarchar](max) NULL,
			[ProvidedPhasedImplementation] [nvarchar](max) NULL,
			[PlanActDate] [datetime] NULL,
			[CRM_Url] [nvarchar](max) NULL,
			[SED_Url] [nvarchar](max) NULL,
			[Doc_MDM_Key] [nvarchar](max) NULL,
			[ParentDoc_MDM_Key] [nvarchar](max) NULL,
			[GroupTypeTESSA] [nvarchar](max) NULL,
			[DocTypeTESSA] [nvarchar](max) NULL,
			[Contragent_FullName] [nvarchar](max) NULL,
			[Org_Title] [nvarchar](max) NULL,
			[Project_Title] [nvarchar](max) NULL,
			[CFO_Title] [nvarchar](max) NULL	,
			[Codifier_Title] [nvarchar](max) NULL
		)
			
		DECLARE @script VARCHAR(max) = 
		'SELECT NEWID() AS [ConID]
			,[UniqueId]
			,[DocNumber]
			,[IsContract]
			,[Author_ID]
			,[DocState]
			,[OuterNumber]
			,[Created]
			,[DocDisplay]
			,[Title]
			,[Contragent_MDM_Key]
			,[Org_MDM_Key]
			,[CFO_MDM_Key]
			,[Project_MDM_Key]
			,[Codifier_MDM_Key]
			,[DocSum]
			,[DocPrepayment]
			,[AdditionalContractSum]
			,[Currency]
			,[InBudget]
			,[TenderCompleted]
			,[NDS_Type]
			,[DefermentOfPayment]
			,[DocForm]
			,[DocType]
			,[MDM_DocType]
			,[StartDate]
			,[EndDate]
			,[Comment]
			,[PrepaymentProcent]
			,[ReasonOfConclusion]
			,[WarrantyCommitments]
			,[DocTypeDUP]
			,[StartWorkConditions]
			,[Warranty]
			,[WarrantyRetentionProc]
			,[ProvidedPhasedImplementation]
			,[PlanActDate]
			,[CRM_Url]
			,[SED_Url]
			,[Doc_MDM_Key]
			,[ParentDoc_MDM_Key]
			,[GroupTypeTESSA]
			,[DocTypeTESSA]
			,[Contragent_FullName]
			,[Org_Title]
			,[Project_Title]
			,[CFO_Title]
			,[Codifier_Title]
		FROM [TST-TESSA-DB].[MigrationSED].[dbo].[ContractsTESSA]
		WHERE [IsContract] = N''True'' 
			AND [GroupTypeTESSA] = N''Договоры УК''
			AND [UniqueId] IS NOT NULL
			AND NOT EXISTS (SELECT 1 from [PnrContractsUK] WHERE ExtID = [UniqueId])'

		INSERT INTO #ObjectsToUpdate
		EXECUTE (@script)			

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
			t.ConID
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
			
		INSERT INTO [PnrContractsUK]
		(
			[ID]
			,[ExternalNumber]
			,[RegistrationDate]
			,[Subject]
			,[Amount]
			,[VATRateID]
			,[VATRateValue]
			,[FormID]
			,[FormName]
			,[StartDate]
			,[EndDate]
			,[PlannedActDate]
			,[MDMKey]
			,[ParentMDM]
			,[SettlementCurrencyID]
			,[SettlementCurrencyName]
			,[SettlementCurrencyCode]
			,[TypeID]
			,[TypeName]
			,[Kind1CID]
			,[Kind1CName]
			,[ExtID]
			,[PartnerID]
			,[PartnerName]
			,[OrganizationID]
			,[OrganizationName]
			,[CFOID]
			,[CFOName]
			,[DevelopmentID]
			,[DevelopmentName]
		)
		SELECT 
			[obj].[ConID] AS [ID]				  
			,[obj].[OuterNumber] as [ExternalNumber]
			,[obj].[Created] as [RegistrationDate]
			,[obj].[DocDisplay] as [Subject]  
			,[obj].[DocSum] as [Amount]			  
			,CASE 
				WHEN [obj].[NDS_Type] = 0 THEN 0 
				WHEN [obj].[NDS_Type] = 10 THEN 2
				WHEN [obj].[NDS_Type] = 18 THEN 3
				ELSE 4
			END AS [VATRateID]
			,CASE 
				WHEN [obj].[NDS_Type] = 0 THEN N'Без НДС' 
				WHEN [obj].[NDS_Type] = 10 THEN N'10%' 
				WHEN [obj].[NDS_Type] = 18 THEN N'18%' 
				ELSE N'20%' 
			END AS [VATRateValue]	
			,CASE 
				WHEN [obj].[DocForm] LIKE N'%Нетип%' THEN '547ad4da-cb7f-478d-8b17-9746cc81ce01' 
				WHEN [obj].[DocForm] LIKE N'%Тип%' THEN 'e8550e9e-b25c-4602-8590-8aec29f2b6b1'
				WHEN [obj].[DocForm] LIKE N'%моно%' THEN '933afcc6-865d-4951-b5e6-2f73de05c65c'
				ELSE NULL
			END AS [FormID]
			,CASE 
				WHEN [obj].[DocForm] LIKE N'%Нетип%' THEN N'Нетиповой' 
				WHEN [obj].[DocForm] LIKE N'%Тип%' THEN N'Типовой'
				WHEN [obj].[DocForm] LIKE N'%моно%' THEN N'С монополистом'
				ELSE NULL
			END AS [FormName]
			,[obj].[StartDate] AS [StartDate]
			,[obj].[EndDate] AS [EndDate]
			,[obj].[PlanActDate] as [PlannedActDate]			 
			,[obj].[Doc_MDM_Key] as [MDMKey]
			,[obj].[ParentDoc_MDM_Key] as [ParentMDM]
			,CASE 
				WHEN [obj].[Currency] LIKE N'%RUR%' THEN 'acaabdfc-2c88-472f-a861-1813bfafe49d'
				ELSE NULL
			END AS [SettlementCurrencyID]
			,CASE 
				WHEN [obj].[Currency] LIKE N'%RUR%' THEN N'RUB'
				ELSE NULL
			END AS [SettlementCurrencyName]
			,CASE 
				WHEN [obj].[Currency] LIKE N'%RUR%' THEN '643'
				ELSE NULL
			END AS [SettlementCurrencyCode]
			,COALESCE(
			(
				SELECT TOP(1) ID FROM [PnrContractTypes] where LOWER(REPLACE(Name, ' ', '')) = LOWER(REPLACE([obj].[DocType], ' ', ''))),
				NULL
			) AS [TypeID]
			,COALESCE
			(
				(SELECT TOP(1) Name FROM [PnrContractTypes] where LOWER(REPLACE(Name, ' ', '')) = LOWER(REPLACE([obj].[DocType], ' ', ''))),
				NULL
			) AS [TypeName]					
			,CASE 
				WHEN LOWER(REPLACE([obj].[MDM_DocType], ' ', '')) = LOWER(REPLACE(N'Прочее', ' ', '')) THEN 'fb00dc0b-903c-46ca-9d6c-53f917a236ee'
				WHEN LOWER(REPLACE([obj].[MDM_DocType], ' ', '')) = LOWER(REPLACE(N'С комиссионером', ' ', '')) THEN '7a1eb607-788c-441a-bcd4-1e43f36cffc0'
				WHEN LOWER(REPLACE([obj].[MDM_DocType], ' ', '')) = LOWER(REPLACE(N'С комиссионером на закупку', ' ', '')) THEN '5ee52c1f-1c36-4b72-a4e1-c96e4dfae62e'
				WHEN LOWER(REPLACE([obj].[MDM_DocType], ' ', '')) = LOWER(REPLACE(N'С комитентом', ' ', '')) THEN 'da7be241-b7a9-4227-a2f1-c732e3429b76'
				WHEN LOWER(REPLACE([obj].[MDM_DocType], ' ', '')) = LOWER(REPLACE(N'С комитентом на закупку', ' ', '')) THEN 'f70eb773-90d2-4857-87fa-e41cb7609a0e'
				WHEN LOWER(REPLACE([obj].[MDM_DocType], ' ', '')) = LOWER(REPLACE(N'С поставщиком', ' ', '')) THEN '8a045f3a-448a-4af1-a6c5-50ddbbe559de'
				ELSE NULL
			END AS [Kind1CID]
			,CASE 
				WHEN LOWER(REPLACE([obj].[MDM_DocType], ' ', '')) = LOWER(REPLACE(N'Прочее', ' ', '')) THEN N'Прочее'
				WHEN LOWER(REPLACE([obj].[MDM_DocType], ' ', '')) = LOWER(REPLACE(N'С комиссионером', ' ', '')) THEN N'С комиссионером'
				WHEN LOWER(REPLACE([obj].[MDM_DocType], ' ', '')) = LOWER(REPLACE(N'С комиссионером на закупку', ' ', '')) THEN N'С комиссионером на закупку'
				WHEN LOWER(REPLACE([obj].[MDM_DocType], ' ', '')) = LOWER(REPLACE(N'С комитентом', ' ', '')) THEN N'С комитентом'
				WHEN LOWER(REPLACE([obj].[MDM_DocType], ' ', '')) = LOWER(REPLACE(N'С комитентом на закупку', ' ', '')) THEN N'С комитентом на закупку'
				WHEN LOWER(REPLACE([obj].[MDM_DocType], ' ', '')) = LOWER(REPLACE(N'С поставщиком', ' ', '')) THEN N'С поставщиком'
				ELSE NULL
			END AS [Kind1CName]					
			,[obj].[UniqueId] as [ExtID]
			,(SELECT TOP(1) [ID] FROM Partners WHERE [MDMKey] = obj.[Contragent_MDM_Key] AND [MDMKey] IS NOT NULL) AS [PartnerID]
			,(SELECT TOP(1) [Name] FROM Partners WHERE [MDMKey] = obj.[Contragent_MDM_Key] AND [MDMKey] IS NOT NULL) AS [PartnerName]
			,(SELECT TOP(1) [ID] FROM PnrOrganizations WHERE [MDMKey] = obj.[Org_MDM_Key] AND [MDMKey] IS NOT NULL) AS [OrganizationID]
			,(SELECT TOP(1) [Name] FROM PnrOrganizations WHERE [MDMKey] = obj.[Org_MDM_Key] AND [MDMKey] IS NOT NULL) AS [OrganizationName]
			,(SELECT TOP(1) [ID] FROM PnrCFO WHERE [MDMKey] = obj.[CFO_MDM_Key] AND [MDMKey] IS NOT NULL) AS [CFOID]
			,(SELECT TOP(1) [Name] FROM PnrCFO WHERE [MDMKey] = obj.[CFO_MDM_Key] AND [MDMKey] IS NOT NULL) AS [CFOName]
			,0 AS [DevelopmentID]
			,N'Не требуется' AS [DevelopmentName]
		FROM
			#ObjectsToUpdate obj
				
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
			t.[ConID]
			,t.[Title] as [Subject]
			,t.[DocNumber] as [FullNumber]
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
		SELECT NEWID() as [ID], t.[ConID] FROM #ObjectsToUpdate as t
				
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
			,'706d9b03-cd05-4963-a28a-e8e50b58dc76'
			,N'Действует'
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