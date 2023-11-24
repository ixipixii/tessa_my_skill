-- ����� ��������
SELECT *
FROM [TST-TESSA-DB].[MigrationSED].[dbo].[ContractsTESSA] with(nolock)
WHERE [IsContract] = N'True'
	AND [GroupTypeTESSA] = N'��������'
	AND [UniqueId] IS NOT NULL
	AND NOT EXISTS (SELECT 1 from [PnrContracts] WHERE ExtID = [UniqueId])

-- ����� ��
SELECT *
FROM [TST-TESSA-DB].[MigrationSED].[dbo].[ContractsTESSA] with(nolock)
WHERE [IsContract] = N'False'
	AND [UniqueId] IS NOT NULL
	AND NOT EXISTS (SELECT 1 from [PnrSupplementaryAgreements] WHERE ExtID = [UniqueId])