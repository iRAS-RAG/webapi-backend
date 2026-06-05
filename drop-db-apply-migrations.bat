@echo off
setlocal

echo Running Entity Framework Core commands...
echo.

REM --- Pre-migration Cleanup ---
REM Delete the existing Migrations folder to ensure a clean start for new migrations.
echo Deleting old migrations folder: .\IRasRag.Infrastructure\Migrations...
if exist ".\IRasRag.Infrastructure\Migrations" (
    rmdir /s /q ".\IRasRag.Infrastructure\Migrations"
    if %errorlevel% neq 0 (
        echo Error deleting migrations folder. Please check permissions or if it's in use.
        goto :eof
    ) else (
        echo Old migrations folder deleted successfully.
    )
) else (
    echo Migrations folder not found, skipping deletion.
)
echo.

REM --- 1. Run Migrations ---
echo Adding new migration "InitialCreate"...
dotnet ef migrations add InitialCreate --project IRasRag.Infrastructure --startup-project IRasRag.API
if %errorlevel% neq 0 (
    echo Error adding migration. Exiting.
    goto :eof
)
echo Migration "InitialCreate" added successfully.
echo.

REM --- 2. Drop Database ---
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

REM --- 3. Update Database ---
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
