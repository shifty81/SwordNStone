@echo off
REM Build validation script for SwordNStone project (Windows)
REM Run this before committing to catch common issues

echo ==========================================
echo SwordNStone Build Validation
echo ==========================================
echo.

set ERRORS=0
set WARNINGS=0

REM 1. Check for required tools
echo 1. Checking required tools...
where msbuild >nul 2>&1
if %ERRORLEVEL% NEQ 0 (
    echo WARNING: MSBuild not found in PATH
    set /a WARNINGS+=1
) else (
    echo OK: MSBuild found
)

where nuget >nul 2>&1
if %ERRORLEVEL% NEQ 0 (
    echo WARNING: NuGet not found in PATH
    set /a WARNINGS+=1
) else (
    echo OK: NuGet found
)
echo.

REM 2. Check for solution file
echo 2. Checking solution file...
if exist "SwordAndStone.sln" (
    echo OK: Solution file found
) else (
    echo ERROR: SwordAndStone.sln not found
    set /a ERRORS+=1
)
echo.

REM 3. Check for packages directory
echo 3. Checking NuGet packages...
if exist "packages" (
    echo OK: Packages directory exists
) else (
    echo WARNING: Packages directory not found
    echo Run: nuget restore SwordAndStone.sln
    set /a WARNINGS+=1
)
echo.

REM 4. Check for important configuration files
echo 4. Checking configuration files...
if exist ".editorconfig" (
    echo OK: .editorconfig found
) else (
    echo WARNING: .editorconfig not found
    set /a WARNINGS+=1
)

if exist ".gitignore" (
    echo OK: .gitignore found
) else (
    echo WARNING: .gitignore not found
    set /a WARNINGS+=1
)

if exist "README.md" (
    echo OK: README.md found
) else (
    echo WARNING: README.md not found
    set /a WARNINGS+=1
)
echo.

REM 5. Try to restore packages if nuget is available
where nuget >nul 2>&1
if %ERRORLEVEL% EQU 0 (
    echo 5. Attempting to restore NuGet packages...
    nuget restore SwordAndStone.sln
    if %ERRORLEVEL% NEQ 0 (
        echo WARNING: Package restore had issues
        set /a WARNINGS+=1
    ) else (
        echo OK: Packages restored
    )
    echo.
)

REM 6. Try to build if msbuild is available
where msbuild >nul 2>&1
if %ERRORLEVEL% EQU 0 (
    echo 6. Attempting to build solution...
    echo This may take a while...
    
    REM Build the library first
    msbuild SwordAndStoneLib\SwordAndStoneLib.csproj /p:Configuration=Debug /v:quiet /nologo
    if %ERRORLEVEL% NEQ 0 (
        echo ERROR: Build failed
        set /a ERRORS+=1
    ) else (
        echo OK: Library build succeeded
    )
    echo.
)

REM Summary
echo ==========================================
echo Validation Summary
echo ==========================================
if %ERRORS% GTR 0 (
    echo ERRORS: %ERRORS%
)
if %WARNINGS% GTR 0 (
    echo WARNINGS: %WARNINGS%
)

if %ERRORS% EQU 0 (
    if %WARNINGS% EQU 0 (
        echo All checks passed!
    ) else (
        echo Passed with warnings
    )
) else (
    echo Validation failed with errors!
)

echo.
echo Next steps:
echo   1. Fix any errors before committing
echo   2. Review warnings and address if needed
echo   3. Run full build in Visual Studio
echo   4. Run tests to ensure functionality

if %ERRORS% GTR 0 (
    exit /b 1
)
exit /b 0
