/*
    HiSubmit SQL Server maintenance

    Safe scope:
    - refreshes statistics for the application tables;
    - rebuilds/reorganizes existing indexes based on fragmentation;
    - keeps the SIMPLE recovery model used by the local database;
    - normalizes log growth to fixed 64 MB increments;
    - shrinks the log only when it is mostly unused.

    Do not run this as a substitute for a production backup/restore plan.
    Execute against the intended database and review the printed measurements.
*/

SET NOCOUNT ON;
SET XACT_ABORT ON;

DECLARE @DatabaseName sysname = DB_NAME();
DECLARE @LogLogicalName sysname;
DECLARE @LogSizeMb decimal(18, 2);
DECLARE @LogUsedPercent decimal(18, 2);
DECLARE @sql nvarchar(max);

SELECT
    @LogLogicalName = name,
    @LogSizeMb = size * 8.0 / 1024
FROM sys.database_files
WHERE type_desc = N'LOG';

SELECT @LogUsedPercent = CAST(used_log_space_in_percent AS decimal(18, 2))
FROM sys.dm_db_log_space_usage;

PRINT CONCAT(
    N'HiSubmit maintenance: ', @DatabaseName,
    N'; log=', @LogSizeMb, N' MB; used=', @LogUsedPercent, N'%');

IF EXISTS
(
    SELECT 1
    FROM sys.databases
    WHERE name = @DatabaseName
      AND recovery_model_desc <> N'SIMPLE'
)
BEGIN
    PRINT N'Recovery model is not SIMPLE; no recovery-model change was made.';
END;

DECLARE @table sysname;
DECLARE @schema sysname;
DECLARE table_cursor CURSOR LOCAL FAST_FORWARD FOR
SELECT s.name, t.name
FROM sys.tables AS t
INNER JOIN sys.schemas AS s ON s.schema_id = t.schema_id
WHERE t.is_ms_shipped = 0;

OPEN table_cursor;
FETCH NEXT FROM table_cursor INTO @schema, @table;

WHILE @@FETCH_STATUS = 0
BEGIN
    SET @sql = N'UPDATE STATISTICS '
        + QUOTENAME(@schema) + N'.' + QUOTENAME(@table)
        + N' WITH RESAMPLE;';
    EXEC sys.sp_executesql @sql;

    FETCH NEXT FROM table_cursor INTO @schema, @table;
END;

CLOSE table_cursor;
DEALLOCATE table_cursor;

DECLARE @objectId int;
DECLARE @indexId int;
DECLARE @fragmentation decimal(18, 2);
DECLARE index_cursor CURSOR LOCAL FAST_FORWARD FOR
SELECT object_id, index_id, avg_fragmentation_in_percent
FROM sys.dm_db_index_physical_stats(DB_ID(), NULL, NULL, NULL, 'LIMITED')
WHERE index_id > 0
  AND page_count >= 32
  AND avg_fragmentation_in_percent >= 10;

OPEN index_cursor;
FETCH NEXT FROM index_cursor INTO @objectId, @indexId, @fragmentation;

WHILE @@FETCH_STATUS = 0
BEGIN
    SELECT
        @schema = s.name,
        @table = t.name
    FROM sys.tables AS t
    INNER JOIN sys.schemas AS s ON s.schema_id = t.schema_id
    WHERE t.object_id = @objectId;

    SELECT @sql = N'ALTER INDEX ' + QUOTENAME(i.name)
        + N' ON ' + QUOTENAME(@schema) + N'.' + QUOTENAME(@table)
        + CASE WHEN @fragmentation >= 30
               THEN N' REBUILD;'
               ELSE N' REORGANIZE;'
          END
    FROM sys.indexes AS i
    WHERE i.object_id = @objectId
      AND i.index_id = @indexId
      AND i.name IS NOT NULL;

    IF @sql IS NOT NULL
        EXEC sys.sp_executesql @sql;

    SET @sql = NULL;
    FETCH NEXT FROM index_cursor INTO @objectId, @indexId, @fragmentation;
END;

CLOSE index_cursor;
DEALLOCATE index_cursor;

IF @LogLogicalName IS NOT NULL
BEGIN
    SET @sql = N'ALTER DATABASE ' + QUOTENAME(@DatabaseName)
        + N' MODIFY FILE (NAME = ' + QUOTENAME(@LogLogicalName, '''')
        + N', FILEGROWTH = 64MB);';
    EXEC sys.sp_executesql @sql;
END;

IF @LogUsedPercent < 20 AND @LogSizeMb > 128
BEGIN
    DBCC SHRINKFILE (@LogLogicalName, 128) WITH NO_INFOMSGS;
    PRINT N'Unused transaction-log space was reduced to approximately 128 MB.';
END
ELSE
BEGIN
    PRINT N'Log shrink skipped because the log is active or already small.';
END;

SELECT
    name,
    type_desc,
    size * 8.0 / 1024 AS SizeMB,
    growth * 8.0 / 1024 AS GrowthMB
FROM sys.database_files;
