CREATE FUNCTION "GetFileExt"
(
	file_version_id uuid
)
RETURNS text
AS $$
    SELECT (
		CASE WHEN "Name" LIKE '%.%'
		THEN reverse(left(reverse("Name"), position('.' in reverse("Name")) - 1))
		ELSE ''
		END) AS "Extension"
    FROM "FileVersions"
    WHERE "RowID" = file_version_id;
$$
LANGUAGE SQL;