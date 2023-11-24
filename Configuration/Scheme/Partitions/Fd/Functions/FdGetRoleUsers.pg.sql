CREATE FUNCTION "FdGetRoleUsers"
(
	role_id uuid,
	limit_count int
)
RETURNS text
AS $$
	SELECT string_agg("UserName", ', ')
	FROM (
            SELECT ru."UserName"
            FROM "Roles" r
			join "RoleUsers" ru on ru."ID" = r."ID"
			join "RoleTypes" rt on r."TypeID" = rt."ID"
			where 
				r."ID" = role_id
				and rt."ID" in (0, 2, 3, 4, 5, 6) -- все роли, кроме персональной
				and ru."IsDeputy" = false -- без учета замещений
			order by ru."UserName" ASC
			LIMIT limit_count) as "Users"
$$
LANGUAGE SQL;