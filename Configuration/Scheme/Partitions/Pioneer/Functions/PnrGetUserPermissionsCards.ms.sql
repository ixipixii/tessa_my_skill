CREATE FUNCTION [PnrGetUserPermissionsCards] (@UserID uniqueidentifier, @TypeID uniqueidentifier)
RETURNS @RESULT TABLE(ID uniqueidentifier)
AS
BEGIN

	--эти права влияют на видимость карточек в представлениях, пока захардкожены
	--для них можно сделать настроечную карточку с соответствием Тип документа - Роли
	
	declare @contractTypeID uniqueidentifier = '1c7a5718-09ae-4f65-aa67-e66f23bb7aee'
	declare @contractUKTypeID uniqueidentifier = '25ea1e75-6ff9-4fd1-94e3-f6bc266d6544'
	declare @supplementaryAgreementTypeID uniqueidentifier = 'f5a33228-32ae-483f-beca-8b2e3453a615'
	declare @supplementaryAgreementUKTypeID uniqueidentifier = '87adb0cb-7c5f-4c82-974f-5d4e3c4a050f'	
	declare @attorneyTypeID uniqueidentifier = 'f9c07ae1-4e87-4cfe-8229-26ce6af5c326'
	declare @actTypeID uniqueidentifier = '156df436-74e3-4e08-aba8-cbc609c6c1c7'
	declare @outgoingTypeID uniqueidentifier = '40dab24a-0b6f-4609-947c-f1916348a540'
	declare @outgoingUKTypeID uniqueidentifier = '10e5967d-8282-4b43-89c6-8d8c9fd9558f'	
	declare @errandTypeID uniqueidentifier = '531e41ec-639f-41a9-9313-94f3eada0427'
	declare @incomingTypeID uniqueidentifier = '476fa752-133d-4571-8f28-86002241f2fe'
	declare @incomingUKTypeID uniqueidentifier = '42eb6143-d431-4bb9-b4bf-19a521205ca5'	
	declare @PnrOrder uniqueidentifier = 'df141f0f-7e73-48fb-9cdb-6d46665cc0fb'
	declare @PnrOrderUK uniqueidentifier = '8d8d1098-3b12-4a77-a988-4278f11d9039'
	declare @archiveKISTypeID uniqueidentifier = '9c8da932-22bc-45d1-9cb7-6edb7d97698b'
	declare @tenderTypeID uniqueidentifier = '78cc3cc5-6314-45c1-bafc-5d41d7da7640'
	declare @partnerRequestTypeID uniqueidentifier = 'ca76dbb5-e4f0-46b7-b5fa-2f7f77c7cae2'
	declare @requestTypeID uniqueidentifier = '3b454ce1-4d5d-4909-ac0a-256b3ebead9c'
	declare @vndTypeID uniqueidentifier = 'cd45788f-1576-4836-83c1-c55714eba28a'
	declare @serviceNoteTypeID uniqueidentifier = 'dceb0c7e-4147-410c-8a6c-f30781226007'
	declare @templateTypeID uniqueidentifier = 'dc10a79d-4bb2-4aad-acb8-82d5838408a9'
	
	declare @UserUkRoleID uniqueidentifier = 'b620333e-0fcb-4b69-9576-02208bc8d0d4'
	declare @AccountantRoleID uniqueidentifier = '0412fedc-35bf-4554-ae01-b084afeb8289' 
	declare @AuditRoleID uniqueidentifier = 'a5cbce1b-8879-4d8a-bc9b-9f8ccb61ad58'
	declare @LawyerRoleID uniqueidentifier = 'b5433694-87e3-486e-a30c-3088036ccc30'
	declare @SBRoleID uniqueidentifier = '9ab797a2-4a9b-451e-b590-5fb40b4e4afc'
	declare @FESRoleID uniqueidentifier = '6e98dbf1-0df9-4ef5-9cad-c67cb630f00a'
	declare @UserGkRoleID uniqueidentifier = 'c365e148-71f9-4731-8025-3aeb5225af9f'
	declare @UserDYPRoleID uniqueidentifier = 'eb858c20-5e2a-4350-a772-57d4f475851e'
	declare @UserYESRoleID uniqueidentifier = '14de0eda-d8ac-4204-a3a7-f817824b7413'
	declare @DirectorDeputyRoleID uniqueidentifier = '940d6e3f-cf66-4059-a778-5d700a7904f1'
	
	--админу права на все карточки
	--архив КИС виден всем
	If (@UserID IN (SELECT ID FROM PersonalRoles WHERE AccessLevelID = 1))
	BEGIN
		INSERT INTO @RESULT (ID)		
		select distinct ID from Instances where @TypeID is null or TypeID = @TypeID
		RETURN
	END
	
	IF (@TypeID is null or @TypeID in (@archiveKISTypeID))
	BEGIN
		INSERT INTO @RESULT (ID)		
		select distinct ID from Instances where TypeID in (@archiveKISTypeID)		
	END
	
	--ДОГОВОРЫ
	IF (@TypeID is null or @TypeID in (@contractTypeID, @supplementaryAgreementTypeID))
	BEGIN
		
		INSERT INTO @RESULT (ID)
		
		/*
		Все исполнители заданий
		*/
		select distinct d.ID from 
		DocumentCommonInfo d with(nolock) 
		inner join TaskHistory th with(nolock)  on th.ID = d.ID 
		inner join RoleUsers ru with(nolock)  on th.RoleID = ru.ID and ru.UserID =  @UserID
		where d.CardTypeID in (@contractTypeID, @supplementaryAgreementTypeID)
		
		-- ДУП и ЦФО: 
		UNION		
		-- +сотрудники соответствующей дирекции\подразделения 	
		select distinct d.ID from 
		DocumentCommonInfo d with(nolock) 
		inner join PnrContracts ctr with(nolock)  on ctr.ID = d.ID and ctr.KindID in ('2b35c1f5-ebaf-4f70-b030-6dcebf6ce550','252a93b2-9fe5-4ee7-b284-6268714db5ee')
		inner join  (
		select UserID from [PnrGetUserDirectionEmployees] (@UserID) 
		union select '11111111-1111-1111-1111-111111111111' -- чтобы мигрированные карточки были видны всем
		) as t  on t.UserID  = d.AuthorID		  
		where d.CardTypeID in (@contractTypeID, @supplementaryAgreementTypeID)
		
		union 
		-- +Бухгалтерия, Внутренний аудит, Юристы, СБ, ФЭС, Замы директора
		select distinct d.ID from 
		DocumentCommonInfo d with(nolock) 
		inner join PnrContracts ctr with(nolock)  on ctr.ID = d.ID and ctr.KindID in ('2b35c1f5-ebaf-4f70-b030-6dcebf6ce550','252a93b2-9fe5-4ee7-b284-6268714db5ee')	
		inner join RoleUsers ru with(nolock) on ru.UserID = @UserID and ru.ID in 
		(@AccountantRoleID,	
		@AuditRoleID,
		@LawyerRoleID,	
		@SBRoleID,
		@FESRoleID,
		@DirectorDeputyRoleID)
		where d.CardTypeID in (@contractTypeID, @supplementaryAgreementTypeID)

		--с покупателями:
		UNION 		
		-- +сотрудники соответствующей дирекции\подразделения		
		select distinct d.ID from 
		DocumentCommonInfo d with(nolock) 
		inner join PnrContracts ctr with(nolock)  on ctr.ID = d.ID and ctr.KindID = '7ede7958-e642-490c-b458-32c034ccb9d6'	 
		inner join  (
		select UserID from [PnrGetUserDirectionEmployees] (@UserID) 
		union select '11111111-1111-1111-1111-111111111111' -- чтобы мигрированные карточки были видны всем
		) as t on t.UserID  = d.AuthorID	
		where d.CardTypeID in (@contractTypeID, @supplementaryAgreementTypeID)
		
		union 
		-- +Бухгалтерия, СБ
		select distinct d.ID from 
		DocumentCommonInfo d with(nolock) 
		inner join PnrContracts ctr with(nolock)  on ctr.ID = d.ID and ctr.KindID = '7ede7958-e642-490c-b458-32c034ccb9d6'	
		inner join RoleUsers ru with(nolock) on ru.UserID = @UserID and ru.ID in 
		(@AccountantRoleID,			
		@SBRoleID)
		where d.CardTypeID in (@contractTypeID, @supplementaryAgreementTypeID)
		
		--внутрихолдинговые:
		UNION		
		-- +видят все сотрудники ГК		
		select distinct d.ID from 
		DocumentCommonInfo d with(nolock) 
		inner join PnrContracts ctr with(nolock)  on ctr.ID = d.ID and ctr.KindID = '5d232548-c3fa-414e-94fa-9ccfd187dd4e'	
		inner join RoleUsers ru with(nolock) on ru.UserID = @UserID and ru.ID in 
		(@UserGkRoleID)
		where d.CardTypeID in (@contractTypeID, @supplementaryAgreementTypeID)

		
	END
	--АКТЫ
	IF (@TypeID is null or @TypeID in (@actTypeID))
	BEGIN
		insert into @result (ID)
		-- исполнители заданий
		select distinct d.ID from 
		DocumentCommonInfo d with(nolock) 
		inner join TaskHistory th with(nolock)  on th.ID = d.ID 
		inner join RoleUsers ru with(nolock)  on th.RoleID = ru.ID and ru.UserID =  @UserID
		where d.CardTypeID in (@actTypeID)
		
		union
		-- Сотрудники ДУП, Бухгалтерия, Юристы, СБ, ФЭС 
		select distinct d.ID from 
		DocumentCommonInfo d with(nolock) 		
		inner join RoleUsers ru with(nolock) on ru.UserID = @UserID and ru.ID in 
		(@UserDYPRoleID,
		@AccountantRoleID,
		@LawyerRoleID,	
		@SBRoleID,
		@FESRoleID)
		where d.CardTypeID in (@actTypeID)
		
		--
		union
		--+ автор
		select distinct d.ID from 
		DocumentCommonInfo d with(nolock) 
		where d.CardTypeID in (@actTypeID) and d.AuthorID = @UserID
			
	END
	--ТЕНДЕР
	IF (@TypeID is null or @TypeID in (@tenderTypeID))
	BEGIN
		insert into @result (ID)
		-- исполнители заданий
		select distinct d.ID from 
		DocumentCommonInfo d with(nolock) 
		inner join TaskHistory th with(nolock)  on th.ID = d.ID 
		inner join RoleUsers ru with(nolock)  on th.RoleID = ru.ID and ru.UserID =  @UserID
		where d.CardTypeID in (@tenderTypeID)
		
		union
		-- Сотрудники ДУП, Юристы, СБ
		select distinct d.ID from 
		DocumentCommonInfo d with(nolock) 		
		inner join RoleUsers ru with(nolock) on ru.UserID = @UserID and ru.ID in 
		(@UserDYPRoleID,		
		@LawyerRoleID,	
		@SBRoleID)
		where d.CardTypeID in (@tenderTypeID)	
		
		--
		union
		--+ автор
		select distinct d.ID from 
		DocumentCommonInfo d with(nolock) 
		where d.CardTypeID in (@tenderTypeID) and d.AuthorID = @UserID
			
	END
	--ИСХОДЯЩИЕ, ВХОДЯЩИЕ, ПРИКАЗЫ УК
	IF (@TypeID is null or  @TypeID in (@PnrOrderUK, @incomingUKTypeID, @outgoingUKTypeID))
	BEGIN
		insert into @result (ID)
		-- исполнители заданий
		select distinct d.ID from 
		DocumentCommonInfo d with(nolock) 
		inner join TaskHistory th with(nolock)  on th.ID = d.ID 
		inner join RoleUsers ru with(nolock)  on th.RoleID = ru.ID and ru.UserID =  @UserID
		where d.CardTypeID in (@PnrOrderUK, @incomingUKTypeID, @outgoingUKTypeID)
		
		union
		-- Сотрудники УК
		select distinct d.ID from 
		DocumentCommonInfo d with(nolock) 		
		inner join RoleUsers ru with(nolock) on ru.UserID = @UserID and ru.ID in 
		(@UserUkRoleID)
		where d.CardTypeID in (@PnrOrderUK, @incomingUKTypeID, @outgoingUKTypeID)	
		
		--
		union
		--+ автор
		select distinct d.ID from 
		DocumentCommonInfo d with(nolock) 
		where d.CardTypeID in (@PnrOrderUK, @incomingUKTypeID, @outgoingUKTypeID) and d.AuthorID = @UserID
			
	END
	--ДОГОВОРЫ УК, ДОП. СОГЛАШЕНИЯ УК
	IF (@TypeID is null or @TypeID in (@contractUKTypeID, @supplementaryAgreementUKTypeID))
	BEGIN
		insert into @result (ID)
		-- исполнители заданий
		select distinct d.ID from 
		DocumentCommonInfo d with(nolock) 
		inner join TaskHistory th with(nolock)  on th.ID = d.ID 
		inner join RoleUsers ru with(nolock)  on th.RoleID = ru.ID and ru.UserID =  @UserID
		where d.CardTypeID in (@contractUKTypeID, @supplementaryAgreementUKTypeID)
		
		union
		-- Сотрудники УК, Бухгалтерия, Внутренний аудит, СБ
		select distinct d.ID from 
		DocumentCommonInfo d with(nolock) 		
		inner join RoleUsers ru with(nolock) on ru.UserID = @UserID and ru.ID in 
		(@UserUkRoleID,
		@AccountantRoleID,
		@AuditRoleID,
		@SBRoleID)	
		where d.CardTypeID in (@contractUKTypeID, @supplementaryAgreementUKTypeID)
		
		--
		union
		--+ автор
		select distinct d.ID from 
		DocumentCommonInfo d with(nolock) 
		where d.CardTypeID in (@contractUKTypeID, @supplementaryAgreementUKTypeID) and d.AuthorID = @UserID
			
	END
	--ЗАЯВКИ НА КА
	IF (@TypeID is null or @TypeID in (@partnerRequestTypeID))
	BEGIN
		insert into @result (ID)
		-- исполнители заданий
		select distinct d.ID from 
		DocumentCommonInfo d with(nolock) 
		inner join TaskHistory th with(nolock)  on th.ID = d.ID 
		inner join RoleUsers ru with(nolock)  on th.RoleID = ru.ID and ru.UserID =  @UserID
		where d.CardTypeID in (@partnerRequestTypeID)
		
		union
		-- Сотрудники УК, Сотрудники ГК, Сотрудники YES
		select distinct d.ID from 
		DocumentCommonInfo d with(nolock) 		
		inner join RoleUsers ru with(nolock) on ru.UserID = @UserID and ru.ID in 
		(@UserUkRoleID,
		@UserGkRoleID,
		@UserYESRoleID)	
		where d.CardTypeID in (@partnerRequestTypeID)	
		
		--
		union
		--+ автор
		select distinct d.ID from 
		DocumentCommonInfo d with(nolock) 
		where d.CardTypeID in (@partnerRequestTypeID) and d.AuthorID = @UserID
			
	END
	--для остальных типов по умолчанию
	IF (@TypeID is null or @TypeID in (@attorneyTypeID, @incomingTypeID, @outgoingTypeID, @vndTypeID, @serviceNoteTypeID, @PnrOrder, @templateTypeID ))	
	BEGIN
	
		insert into @result (ID)
		-- исполнители заданий
		select distinct d.ID from 
		DocumentCommonInfo d with(nolock) 
		inner join TaskHistory th with(nolock)  on th.ID = d.ID 
		inner join RoleUsers ru with(nolock)  on th.RoleID = ru.ID and ru.UserID =  @UserID
		where d.CardTypeID in (@attorneyTypeID, @incomingTypeID, @outgoingTypeID, @vndTypeID, @serviceNoteTypeID, @PnrOrder, @templateTypeID )
		
		union
		--+ все сотрудники ГК		
		select distinct d.ID from 
		DocumentCommonInfo d with(nolock) 		
		inner join RoleUsers ru with(nolock) on ru.UserID = @UserID and ru.ID in 
		(@UserGkRoleID)
		where d.CardTypeID in (@attorneyTypeID, @incomingTypeID, @outgoingTypeID, @vndTypeID, @serviceNoteTypeID, @PnrOrder, @templateTypeID )
		
		union
		--+ автор
		select distinct d.ID from 
		DocumentCommonInfo d with(nolock) 
		where d.CardTypeID in (@attorneyTypeID, @incomingTypeID, @outgoingTypeID, @vndTypeID, @serviceNoteTypeID, @PnrOrder, @templateTypeID ) and d.AuthorID = @UserID
	END
	RETURN
END