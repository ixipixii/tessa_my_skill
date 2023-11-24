CREATE FUNCTION [GetFileExt](@fileVersionID uniqueidentifier)
RETURNS nvarchar(100)
AS
BEGIN
RETURN
(
    select
        (case when Name like N'%.%'
            then reverse(left(reverse(Name), charindex(N'.', reverse(Name)) - 1))
            else N'' end) as Extension
    from FileVersions with(nolock)
    where RowID = @fileVersionID
)
END