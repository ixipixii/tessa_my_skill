CREATE FUNCTION "ObtainReaderLock"
(
	id uuid,
	retry_count int,
	retry_timeout int
)
RETURNS TABLE
(
	"Result" int,
	"TypeID" uuid
) AS $$
DECLARE
	type_id uuid;
	counter int;
BEGIN
	BEGIN
		SELECT i."TypeID"
		INTO type_id
		FROM "Instances" AS i
		WHERE i."ID" = id
		LIMIT 1;

		IF type_id IS NULL THEN
			RETURN QUERY SELECT 2, NULL::uuid;
			RETURN;
		END IF;

		counter := 0;

		UPDATE "Instances"
		SET "Readers" = "Readers" + 1
		WHERE "ID" = id AND "WritePending" = false;

		WHILE NOT FOUND LOOP
			counter := counter + 1;

			IF counter > retry_count THEN
				RETURN QUERY SELECT 1, NULL::uuid;
				RETURN;
			END IF;

			PERFORM pg_sleep(retry_timeout / 1000.0);
	
			UPDATE "Instances"
			SET "Readers" = "Readers" + 1
			WHERE "ID" = id AND "WritePending" = false;
		END LOOP;
	EXCEPTION
		WHEN SQLSTATE '55P03' THEN
			RETURN QUERY SELECT 3, NULL::uuid;
			RETURN;
	END;

	RETURN QUERY SELECT 0, type_id;
END; $$
LANGUAGE PLPGSQL;