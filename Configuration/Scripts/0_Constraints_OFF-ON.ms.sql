﻿EXEC sp_msforeachtable "ALTER TABLE ? NOCHECK CONSTRAINT ALL"
GO

EXEC sp_msforeachtable "ALTER TABLE ? WITH CHECK CHECK CONSTRAINT ALL"
GO
