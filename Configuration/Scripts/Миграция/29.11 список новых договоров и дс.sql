-- НОВЫЕ ДОГОВОРЫ
SELECT *
FROM [TST-TESSA-DB].[MigrationSED].[dbo].[ContractsTESSA] with(nolock)
WHERE [IsContract] = N'True'
	AND [GroupTypeTESSA] = N'Договоры'
	AND [UniqueId] IS NOT NULL
	AND NOT EXISTS (SELECT 1 from [PnrContracts] WHERE ExtID = [UniqueId])

-- НОВЫЕ ДС
SELECT *
FROM [TST-TESSA-DB].[MigrationSED].[dbo].[ContractsTESSA] with(nolock)
WHERE [IsContract] = N'False'
	AND [UniqueId] IS NOT NULL
	AND NOT EXISTS (SELECT 1 from [PnrSupplementaryAgreements] WHERE ExtID = [UniqueId])