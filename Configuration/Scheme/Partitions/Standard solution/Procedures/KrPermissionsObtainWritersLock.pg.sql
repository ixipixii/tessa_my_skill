CREATE FUNCTION "KrPermissionsObtainWritersLock"
(
	retry_count int,
	retry_timeout int
)
RETURNS TABLE
(
	"Result" int
) AS $$
DECLARE
	counter int;
BEGIN
	BEGIN
		counter := 0;

		UPDATE "KrPermissionsSystem"
		SET "Writers" = "Writers" + 1
		WHERE "Readers" = 0;

		WHILE NOT FOUND LOOP
			counter := counter + 1;

			IF counter > retry_count THEN
				RETURN QUERY SELECT 1;
				RETURN;
			END IF;

			PERFORM pg_sleep(retry_timeout / 1000.0);
	
			UPDATE "KrPermissionsSystem"
			SET "Writers" = "Writers" + 1
			WHERE "Readers" = 0;
		END LOOP;
	EXCEPTION
		WHEN SQLSTATE '55P03' THEN
			RETURN QUERY SELECT 1;
			RETURN;
	END;

	RETURN QUERY SELECT 0;
END; $$
LANGUAGE PLPGSQL;