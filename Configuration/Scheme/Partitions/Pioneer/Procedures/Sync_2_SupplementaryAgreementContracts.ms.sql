CREATE PROCEDURE [Sync_2_SupplementaryAgreementContracts]
AS
BEGIN

	BEGIN TRANSACTION;

	BEGIN TRY		

		DECLARE @CardType UNIQUEIDENTIFIER = 'f5a33228-32ae-483f-beca-8b2e3453a615' -- PnrSupplementaryAgreement
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
		WHERE [IsContract] = N''False''
			AND [UniqueId] IS NOT NULL
			AND NOT EXISTS (SELECT 1 from [PnrSupplementaryAgreements] WHERE ExtID = [UniqueId])'

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
			
		INSERT INTO [PnrSupplementaryAgreements]
		(
			[ID]
			,[ExternalNumber]
			,[ProjectDate]
			,[Subject]			   
			,[Amount]
			,[AmountSA]
			,[PrepaidExpenseAmount]
			,[VATRateID]
			,[VATRateValue]
			,[FormID]
			,[FormName]
			,[StartDate]
			,[EndDate]
			,[Comment]
			,[DownPaymentID]
			,[DownPaymentValue]
			,[DefermentPaymentID]
			,[DefermentPaymentValue]
			,[GroundConcluding]
			,[IsWarrantyID]
			,[IsWarrantyName]
			,[GuaranteePeriodID]
			,[GuaranteePeriodValue]
			,[GuaranteeDeductionsID]
			,[GuaranteeDeductionsValue]
			,[PhasedImplementationID]
			,[PhasedImplementationName]
			,[PlannedActDate]
			,[LinkCardCRM]
			,[MDMKey]
			,[IsInBudget]
			,[IsTenderHeld]
			,[HyperlinkCard]
			,[ParentMDM]
			,[SettlementCurrencyID]
			,[SettlementCurrencyName]
			,[SettlementCurrencyCode]
			,[TypeID]
			,[TypeName]
			,[ExtID]
			,[PartnerID]
			,[PartnerName]
			,[OrganizationID]
			,[OrganizationName]		
			,[ProjectID]
			,[ProjectName]
			,[CFOID]
			,[CFOName]
			,[CostItemID]
			,[CostItemName]
			,[KindID]
			,[KindName]
		)
		SELECT 
			[obj].[ConID] AS [ID]				  
			,[obj].[OuterNumber] as [ExternalNumber]
			,[obj].[Created] as [ProjectDate]
			,[obj].[DocDisplay] as [Subject]  
			,[obj].[AdditionalContractSum] as [Amount]	
			,[obj].[DocSum] as [AmountSA]
			,[obj].[DocPrepayment] as [PrepaidExpenseAmount]		
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
			,[obj].[Comment]
			,CASE 
				WHEN [obj].[PrepaymentProcent] = 5 THEN 2
				WHEN [obj].[PrepaymentProcent] = 10 THEN 3
				WHEN [obj].[PrepaymentProcent] = 15 THEN 4
				WHEN [obj].[PrepaymentProcent] = 30 THEN 5
				WHEN [obj].[PrepaymentProcent] = 45 THEN 6
				ELSE 1
			END AS [DownPaymentID]
			,CASE 
				WHEN [obj].[PrepaymentProcent] = 5 THEN 5
				WHEN [obj].[PrepaymentProcent] = 10 THEN 10
				WHEN [obj].[PrepaymentProcent] = 15 THEN 15
				WHEN [obj].[PrepaymentProcent] = 30 THEN 30
				WHEN [obj].[PrepaymentProcent] = 45 THEN 45
				ELSE 0
			END AS [DownPaymentValue]
			,CASE 
				WHEN [obj].[DefermentOfPayment] = 0 THEN 1
				WHEN [obj].[DefermentOfPayment] = 10 THEN 2
				WHEN [obj].[DefermentOfPayment] = 15 THEN 3
				WHEN [obj].[DefermentOfPayment] = 20 THEN 4
				WHEN [obj].[DefermentOfPayment] = 25 THEN 5
				WHEN [obj].[DefermentOfPayment] = 30 THEN 6
				WHEN [obj].[DefermentOfPayment] = 40 THEN 7
				WHEN [obj].[DefermentOfPayment] = 50 THEN 8
				WHEN [obj].[DefermentOfPayment] = 70 THEN 9
				WHEN [obj].[DefermentOfPayment] = 100 THEN 10
				ELSE NULL
			END AS [DefermentPaymentID]
			,CASE 
				WHEN [obj].[DefermentOfPayment] = 0 THEN 0
				WHEN [obj].[DefermentOfPayment] = 10 THEN 10
				WHEN [obj].[DefermentOfPayment] = 15 THEN 15
				WHEN [obj].[DefermentOfPayment] = 20 THEN 20
				WHEN [obj].[DefermentOfPayment] = 25 THEN 25
				WHEN [obj].[DefermentOfPayment] = 30 THEN 30
				WHEN [obj].[DefermentOfPayment] = 40 THEN 40
				WHEN [obj].[DefermentOfPayment] = 50 THEN 50
				WHEN [obj].[DefermentOfPayment] = 70 THEN 70
				WHEN [obj].[DefermentOfPayment] = 100 THEN 100
				ELSE NULL
			END AS [DefermentPaymentValue]				   
			,[obj].[ReasonOfConclusion] as [GroundConcluding]
			,CASE 
				WHEN [obj].[WarrantyCommitments] LIKE N'%Да%' THEN 1
				WHEN [obj].[WarrantyCommitments] LIKE N'%Нет%' THEN 0
				ELSE NULL
			END AS [IsWarrantyID]
			,CASE 
				WHEN [obj].[WarrantyCommitments] LIKE N'%Да%' THEN N'Да'
				WHEN [obj].[WarrantyCommitments] LIKE N'%Нет%' THEN N'Нет'
				ELSE NULL
			END AS [IsWarrantyName]
			,CASE 
				WHEN [obj].[Warranty] = 0 THEN 1
				WHEN [obj].[Warranty] = 1 THEN 2
				WHEN [obj].[Warranty] = 2 THEN 3
				WHEN [obj].[Warranty] = 3 THEN 4
				WHEN [obj].[Warranty] = 4 THEN 5
				WHEN [obj].[Warranty] = 5 THEN 6
				ELSE NULL
			END AS [GuaranteePeriodID]
			,CASE 
				WHEN [obj].[Warranty] = 0 THEN 0
				WHEN [obj].[Warranty] = 1 THEN 1
				WHEN [obj].[Warranty] = 2 THEN 2
				WHEN [obj].[Warranty] = 3 THEN 3
				WHEN [obj].[Warranty] = 4 THEN 4
				WHEN [obj].[Warranty] = 5 THEN 5
				ELSE NULL
			END AS [GuaranteePeriodValue]
			,CASE 
				WHEN [obj].[WarrantyRetentionProc] = 0 THEN 1
				WHEN [obj].[WarrantyRetentionProc] = 2 THEN 2
				WHEN [obj].[WarrantyRetentionProc] = 5 THEN 3
				ELSE NULL
			END AS [GuaranteeDeductionsID]
			,CASE 
				WHEN [obj].[WarrantyRetentionProc] = 0 THEN 0
				WHEN [obj].[WarrantyRetentionProc] = 2 THEN 2
				WHEN [obj].[WarrantyRetentionProc] = 5 THEN 5
				ELSE NULL
			END AS [GuaranteeDeductionsValue]		   
			,CASE 
				WHEN [obj].[ProvidedPhasedImplementation] LIKE N'%Да%' THEN 1
				WHEN [obj].[ProvidedPhasedImplementation] LIKE N'%Нет%' THEN 0
				ELSE NULL
			END AS [PhasedImplementationID]
			,CASE 
				WHEN [obj].[ProvidedPhasedImplementation] LIKE N'%Да%' THEN 1
				WHEN [obj].[ProvidedPhasedImplementation] LIKE N'%Нет%' THEN 0
				ELSE NULL
			END AS [PhasedImplementationName]			   			 
			,[obj].[PlanActDate] as [PlannedActDate]			 
			,[obj].[CRM_Url] as [LinkCardCRM]				  
			,[obj].[Doc_MDM_Key] as [MDMKey]
			,[obj].[InBudget] as [IsInBudget]
			,[obj].[TenderCompleted] as [IsTenderHeld]
			,[obj].[SED_Url] as [HyperlinkCard]				  
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
			,COALESCE
			(
				(SELECT TOP(1) ID FROM [PnrContractTypes] where LOWER(REPLACE(Name, ' ', '')) = LOWER(REPLACE([obj].[DocType], ' ', ''))),
				NULL
			) AS [TypeID]
			,COALESCE
			(
				(SELECT TOP(1) Name FROM [PnrContractTypes] where LOWER(REPLACE(Name, ' ', '')) = LOWER(REPLACE([obj].[DocType], ' ', ''))),
				NULL
			) AS [TypeName]									
			,[obj].[UniqueId] as [ExtID]
			,(SELECT TOP(1) [ID] FROM Partners WHERE [MDMKey] = obj.[Contragent_MDM_Key]) AS [PartnerID]
			,(SELECT TOP(1) [Name] FROM Partners WHERE [MDMKey] = obj.[Contragent_MDM_Key]) AS [PartnerName]
			,(SELECT TOP(1) [ID] FROM PnrOrganizations WHERE [MDMKey] = obj.[Org_MDM_Key]) AS [OrganizationID]
			,(SELECT TOP(1) [Name] FROM PnrOrganizations WHERE [MDMKey] = obj.[Org_MDM_Key]) AS [OrganizationName]
			,(SELECT TOP(1) [ID] FROM PnrProjects WHERE [MDMKey] = obj.[Project_MDM_Key]) AS [ProjectID]
			,(SELECT TOP(1) [Name] FROM PnrProjects WHERE [MDMKey] = obj.[Project_MDM_Key]) AS [ProjectName]
			,(SELECT TOP(1) [ID] FROM PnrCFO WHERE [MDMKey] = obj.[CFO_MDM_Key]) AS [CFOID]
			,(SELECT TOP(1) [Name] FROM PnrCFO WHERE [MDMKey] = obj.[CFO_MDM_Key]) AS [CFOName]
			,(SELECT TOP(1) [ID] FROM PnrCostItems WHERE [MDMKey] = obj.[Codifier_MDM_Key]) AS [CostItemID]
			,(SELECT TOP(1) [Name] FROM PnrCostItems WHERE [MDMKey] = obj.[Codifier_MDM_Key]) AS [CostItemName]
			,COALESCE
			(
				(SELECT top(1) ki.ID from PnrContractKinds as ki where Name = LOWER(REPLACE(obj.DocTypeTESSA, ' ', ''))), 
				(case when obj.DocTypeTESSA = N'с покупателем' then '7ede7958-e642-490c-b458-32c034ccb9d6' end),
				(case when obj.DocTypeTESSA LIKE N'%ДУП%' then '2b35c1f5-ebaf-4f70-b030-6dcebf6ce550' end),
				NULL
			) AS [KindID]
			,COALESCE
			(
				(SELECT top(1) ki.Name from PnrContractKinds as ki where Name = LOWER(REPLACE(obj.DocTypeTESSA, ' ', ''))),
				(case when obj.DocTypeTESSA = N'с покупателем' then N'С покупателями' end),
				(case when obj.DocTypeTESSA LIKE N'%ДУП%' then N'ДУП' end), 
				NULL
			) AS [KindName]	 
		FROM
			#ObjectsToUpdate obj
				
		INSERT INTO [DocumentCommonInfo] 
		(
			[ID]
			,[Subject]
			,[FullNumber]
			,[CardTypeID]
			,[CardTypeName]
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
			,'0ba2dd7f-4b31-419b-b02c-830b5608de79'
			,N'Действует'
		FROM #FdStatesObjectsVirtual as t
								
		DROP TABLE #FdStatesObjectsVirtual	
		DROP TABLE #ObjectsToUpdate

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
	return
END