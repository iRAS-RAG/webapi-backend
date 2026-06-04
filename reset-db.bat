@echo off
setlocal

echo Running Entity Framework Core commands...
echo.

REM --- 1. Drop Database ---
REM This command drops the database based on your DbContext's connection string.
REM !!! WARNING: This will delete all data in your database. Use with caution. !!!
echo Dropping existing database...
dotnet ef database drop --project IRasRag.Infrastructure --startup-project IRasRag.API --force
if %errorlevel% neq 0 (
    echo Error dropping database. It might not exist, or you might not have permissions.
    REM Continue if error is just because db doesn't exist.
) else (
    echo Database dropped successfully.
)
echo.

REM --- 2. Update Database ---
echo Updating database with latest migrations...
dotnet ef database update --project IRasRag.Infrastructure --startup-project IRasRag.API
if %errorlevel% neq 0 (
    echo Error updating database.
    goto :eof
)
echo Database updated successfully.
echo.

echo All commands completed.
pause
endlocal
