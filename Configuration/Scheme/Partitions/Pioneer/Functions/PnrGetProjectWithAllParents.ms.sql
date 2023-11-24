CREATE FUNCTION [PnrGetProjectWithAllParents]
(
	@ProjectID uniqueidentifier
)
RETURNS TABLE 
AS
RETURN
(
    WITH items AS
	(
		SELECT p.*, 0 AS Level
		FROM PnrProjects p with(nolock)
		WHERE p.ID = @ProjectID

		UNION ALL

		SELECT p2.*, Level + 1
		FROM PnrProjects p2 with(nolock)
		INNER JOIN items i ON i.ParentProjectID = p2.ID
	)
	SELECT * 
	FROM items
)