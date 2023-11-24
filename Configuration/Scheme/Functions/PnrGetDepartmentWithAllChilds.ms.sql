CREATE FUNCTION [PnrGetDepartmentWithAllChilds]
(
	@DepartmentIdx varchar(5)
)
RETURNS TABLE 
AS
RETURN
(
   WITH UnderTreeElements (ID, ParentID) AS -- Элементы иерархии вниз по дереву
   (
   	SELECT ID, ParentID
   	FROM Roles
   	WHERE Idx = @DepartmentIdx
   	UNION ALL
   	SELECT Roles.ID, Roles.ParentID
   	FROM Roles
   	JOIN UnderTreeElements 
   	ON Roles.ParentID = UnderTreeElements.ID
   )
   
   SELECT DISTINCT ID FROM UnderTreeElements
)
