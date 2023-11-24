﻿CREATE FUNCTION [Localize]
(
	@name nvarchar(max),
	@culture int
)
RETURNS nvarchar(max)
WITH RETURNS NULL ON NULL INPUT
AS
BEGIN
	if left(@Name, 1) <> '$' return @name

	declare @result nvarchar(max)

	select top 1 @result = ls.Value
	from LocalizationEntries le with(nolock)
	cross apply (
		select top 1 ls.Value
		from LocalizationStrings ls with(nolock)
		where ls.EntryID = le.ID
		  and ls.Culture = @culture
	) ls
	where le.Name = right(@name, len(@name) - 1)
	    and le.Overridden = 0

	return isnull(@result, @name)
END;