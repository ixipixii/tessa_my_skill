-- ДС на удаление
SELECT  *
FROM Instances i
-- пример: если надо удалить карточки по условию (пример - удаление всех КА, у котороых не задано поле MDMKey)
-- пример: join Partners p on p.ID = i.ID 
join PnrSupplementaryAgreements t on t.ID = i.ID 
where i.TypeID = 'F5A33228-32AE-483F-BECA-8B2E3453A615'
-- пример: and p.MDMKey is null
	and i.CreatedByID = '11111111-1111-1111-1111-111111111111' -- System
	and i.Created = cast('26.11.2020' as datetime) -- дата миграции
	and t.ProjectDate >= cast('01.09.2020' as date) -- документ создан начиная с 1 сентября
order by t.ProjectDate desc


 -- Договор на удаление
SELECT *
FROM Instances i
-- пример: если надо удалить карточки по условию (пример - удаление всех КА, у котороых не задано поле MDMKey)
-- пример: join Partners p on p.ID = i.ID 
join PnrContracts t on t.ID = i.ID 
where i.TypeID = '1C7A5718-09AE-4F65-AA67-E66F23BB7AEE'
-- пример: and p.MDMKey is null
	and i.CreatedByID = '11111111-1111-1111-1111-111111111111' -- System
	and i.Created = cast('26.11.2020' as datetime) -- дата миграции
	and t.ProjectDate >= cast('01.09.2020' as date) -- документ создан начиная с 1 сентября
order by i.Created desc