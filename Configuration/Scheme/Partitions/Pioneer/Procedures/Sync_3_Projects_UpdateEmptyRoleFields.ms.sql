CREATE PROCEDURE [Sync_3_Projects_UpdateEmptyRoleFields]
AS
BEGIN

	BEGIN TRANSACTION;

	BEGIN TRY		
		
		-- Руководитель проекта
		update t set t.ProjectManagerID = pm.ProjectManagerID, t.ProjectManagerName = pm.ProjectManagerName
		--select *
		from PnrProjects t
		outer apply
		(
			select top 1 p.ProjectManagerID, p.ProjectManagerName	
			from PnrGetProjectWithAllParents(t.ID) p
			where p.ProjectManagerID is not null
			order by p.Level
		) as pm
		where t.ParentProjectID is not null
			and t.ProjectManagerID is null
			and pm.ProjectManagerID is not null
		
		-- Сметчик
		update t set t.EstimatorID = es.EstimatorID, t.EstimatorName = es.EstimatorName
		--select *
		from PnrProjects t
		outer apply
		(
			select top 1 p.EstimatorID, p.EstimatorName
			from PnrGetProjectWithAllParents(t.ID) p
			where p.EstimatorID is not null
			order by p.Level
		) as es
		where t.ParentProjectID is not null
			and t.EstimatorID is null
			and es.EstimatorID is not null
		
		-- ГИП
		update t set t.GIPID = gip.GIPID, t.GIPName = gip.GIPName
		--select *
		from PnrProjects t
		outer apply
		(
			select top 1 p.GIPID, p.GIPName
			from PnrGetProjectWithAllParents(t.ID) p
			where p.GIPID is not null
			order by p.Level
		) as gip
		where t.ParentProjectID is not null
			and t.GIPID is null
			and gip.GIPID is not null 
		
		-- Администратор проекта
		update t set t.ProjectAdministratorID = pa.ProjectAdministratorID, t.ProjectAdministratorName = pa.ProjectAdministratorName
		--select *
		from PnrProjects t
		outer apply
		(
			select top 1 p.ProjectAdministratorID, p.ProjectAdministratorName
			from PnrGetProjectWithAllParents(t.ID) p
			where p.ProjectAdministratorID is not null
			order by p.Level
		) as pa
		where t.ParentProjectID is not null
		and t.ProjectAdministratorID is null and pa.ProjectAdministratorID is not null
		
		
		-- Экономист
		update t set t.ProjectEconomistID = pe.ProjectEconomistID, t.ProjectEconomistName = pe.ProjectEconomistName
		--select *
		from PnrProjects t
		outer apply
		(
			select top 1 p.ProjectEconomistID, p.ProjectEconomistName
			from PnrGetProjectWithAllParents(t.ID) p
			where p.ProjectEconomistID is not null
			order by p.Level
		) as pe
		where t.ParentProjectID is not null
		and t.ProjectEconomistID is null and pe.ProjectEconomistID is not null

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