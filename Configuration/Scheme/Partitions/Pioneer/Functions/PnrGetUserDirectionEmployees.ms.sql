create function [PnrGetUserDirectionEmployees] (@UserID uniqueidentifier)
returns table
as
RETURN
(
	--получаем дирекции со всеми дочерними подразделениям
	with directionsDepartments
	as 
	(
		SELECT 
		DR.ID as DirectionID, 
		R.[Name] as DirectionName, 	
		DR.ID as DepartmentID, 
		R.[Name] as DepartmentName, 	
		0 as Lvl
		FROM DepartmentRoles as DR WITH(NOLOCK) 
		INNER JOIN Roles as R WITH(NOLOCK) ON DR.ID = R.ID 
		where R.[Name] like N'%Дирекция%'
	
		UNION ALL

		SELECT 
		du.DirectionID, 
		du.DirectionName, 
		DR.ID as DepartmentID, 
		R.[Name] as DepartmentName, 	
		(Lvl + 1) as Lvl
		FROM 
		Roles ChildDepartments
		INNER JOIN directionsDepartments du on ChildDepartments.ParentID = du.DepartmentID
		INNER JOIN DepartmentRoles as DR WITH(NOLOCK) ON DR.ID = ChildDepartments.ID  	
		INNER JOIN Roles as R WITH(NOLOCK) ON DR.ID = R.ID 	
	)

	--получаем всех сотрудников дочерних подразделений дирекции + всех сотрудников подразделения сотрудника, если оно вне дирекции + самого сотрудника
	SELECT DISTINCT allUserDepartmentEmployees.UserID 
	FROM RoleUsers ru WITH(NOLOCK) 
	INNER JOIN directionsDepartments dd on dd.DepartmentID = ru.ID
	INNER JOIN directionsDepartments allUserDeparments on allUserDeparments.DirectionID = dd.DirectionID
	INNER JOIN RoleUsers allUserDepartmentEmployees on allUserDeparments.DepartmentID = allUserDepartmentEmployees.ID
	where ru.UserID = @UserID
	
	UNION
	
	SELECT DISTINCT allru.UserID 
	FROM DepartmentRoles DR WITH(NOLOCK) 
	INNER JOIN RoleUsers ru WITH(NOLOCK) on ru.ID = DR.ID
	INNER JOIN RoleUsers allru WITH(NOLOCK) on allru.ID = DR.ID	
	where ru.UserID = @UserID	
	
	UNION
	
	SELECT  @UserID
	

)

