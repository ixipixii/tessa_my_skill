CREATE FUNCTION [FdGetRoleUsers]
(
	@RoleID uniqueidentifier,
	@LimitCount int
)
RETURNS nvarchar(max)
AS
BEGIN
	RETURN (STUFF((
            SELECT top (@LimitCount) ', ' + ru.UserName
            FROM Roles r with(nolock)
			join RoleUsers ru with(nolock) on ru.ID = r.ID
			join RoleTypes rt with(nolock) on r.TypeID = rt.ID
			where 
				r.ID = @RoleID
				and rt.ID in (0, 2, 3, 4, 5, 6) -- все роли, кроме персональной
				and ru.IsDeputy = 0 -- без учета замещений
			order by ru.UserName ASC
            FOR XML PATH('')
            ), 1, 2, ''))
END