@echo off
REM Pre-Build Validation Script for SwordNStone
REM This script validates the project state before build to catch common metadata errors
REM Run this before building or committing changes

setlocal enabledelayedexpansion
echo ========================================
echo SwordNStone Pre-Build Validation
echo ========================================
echo.

set ERRORS=0
set WARNINGS=0

REM 1. Validate all .csproj files are well-formed XML
echo [1/8] Validating project files...
for %%f in (SwordAndStoneLib\SwordAndStoneLib.csproj SwordAndStone\SwordAndStone.csproj SwordAndStoneServer\SwordAndStoneServer.csproj ScriptingApi\ScriptingApi.csproj) do (
    if exist "%%f" (
        REM Basic check - file should contain both opening and closing Project tags
        findstr /C:"<Project" "%%f" >nul 2>&1
        if !ERRORLEVEL! NEQ 0 (
            echo   ERROR: %%f is missing opening Project tag
            set /a ERRORS+=1
        ) else (
            findstr /C:"</Project>" "%%f" >nul 2>&1
            if !ERRORLEVEL! NEQ 0 (
                echo   ERROR: %%f is missing closing Project tag
                set /a ERRORS+=1
            ) else (
                echo   OK: %%f
            )
        )
    ) else (
        echo   ERROR: %%f not found
        set /a ERRORS+=1
    )
)
echo.

REM 2. Check for NuGet packages
echo [2/8] Checking NuGet packages...
if exist "packages\protobuf-net.2.1.0" (
    echo   OK: protobuf-net package found
) else (
    echo   WARNING: protobuf-net package not found
    echo   Please run: nuget restore SwordAndStone.sln
    set /a WARNINGS+=1
)

if exist "packages\OpenTK.2.0.0" (
    echo   OK: OpenTK package found
) else (
    echo   WARNING: OpenTK package not found
    echo   Please run: nuget restore SwordAndStone.sln
    set /a WARNINGS+=1
)
echo.

REM 3. Check for required build tools
echo [3/8] Checking required build tools...
if exist "CiTo.exe" (
    echo   OK: CiTo.exe found
) else (
    echo   ERROR: CiTo.exe not found in root directory
    set /a ERRORS+=1
)

if exist "CodeGenerator.exe" (
    echo   OK: CodeGenerator.exe found
) else (
    echo   ERROR: CodeGenerator.exe not found in root directory
    set /a ERRORS+=1
)

if exist "CitoAssets.exe" (
    echo   OK: CitoAssets.exe found
) else (
    echo   WARNING: CitoAssets.exe not found in root directory
    set /a WARNINGS+=1
)
echo.

REM 4. Check for critical source files
echo [4/8] Checking critical source files...
if exist "Packet.proto" (
    echo   OK: Packet.proto found
) else (
    echo   ERROR: Packet.proto not found
    set /a ERRORS+=1
)

if exist "BuildCito.bat" (
    echo   OK: BuildCito.bat found
) else (
    echo   ERROR: BuildCito.bat not found
    set /a ERRORS+=1
)
echo.

REM 5. Validate that all .ci.cs files are included in SwordAndStoneLib.csproj
echo [5/8] Validating .ci.cs file references in project...
set CI_FILES_MISSING=0
for /r "SwordAndStoneLib" %%f in (*.ci.cs) do (
    set "filename=%%~nxf"
    REM Skip Packet.Serializer.ci.cs as it's generated
    if not "!filename!"=="Packet.Serializer.ci.cs" (
        findstr /C:"!filename!" "SwordAndStoneLib\SwordAndStoneLib.csproj" >nul 2>&1
        if !ERRORLEVEL! NEQ 0 (
            echo   WARNING: !filename! not found in SwordAndStoneLib.csproj
            set /a WARNINGS+=1
            set CI_FILES_MISSING=1
        )
    )
)
if %CI_FILES_MISSING%==0 (
    echo   OK: All .ci.cs files referenced in project
)
echo.

REM 6. Check for common CiTo syntax issues
echo [6/8] Scanning for common CiTo syntax issues...
set CITO_ISSUES=0
REM Check for array indexing within IntRef.Create (may cause transpilation errors)
REM Note: Basic pattern match - may have some false positives
findstr /S /N /R "IntRef\.Create.*\[.*\]" "SwordAndStoneLib\Client\*.ci.cs" >nul 2>&1
if !ERRORLEVEL! EQU 0 (
    echo   WARNING: Found potential array indexing within IntRef.Create
    echo   Consider extracting to intermediate variables
    echo   Example: int val = array[index]; IntRef.Create(val);
    set /a WARNINGS+=1
    set CITO_ISSUES=1
)
if %CITO_ISSUES%==0 (
    echo   OK: No obvious CiTo syntax issues found
)
echo.

REM 7. Check if previous build artifacts exist
echo [7/8] Checking for previous build artifacts...
if exist "SwordAndStoneLib\bin\Debug\SwordAndStoneLib.dll" (
    echo   OK: Previous build found - incremental build possible
) else (
    echo   INFO: No previous build found - full build required
)
echo.

REM 8. Validate Packet.Serializer.ci.cs if it exists (should be generated)
echo [8/8] Checking generated files...
if exist "Packet.Serializer.ci.cs" (
    REM Check if it has the 'new' keyword fix applied
    findstr /C:"public new int GetType" "Packet.Serializer.ci.cs" >nul 2>&1
    if !ERRORLEVEL! EQU 0 (
        echo   OK: Packet.Serializer.ci.cs exists and has CS0108 fix
    ) else (
        echo   WARNING: Packet.Serializer.ci.cs exists but may need CS0108 fix
        set /a WARNINGS+=1
    )
) else (
    echo   INFO: Packet.Serializer.ci.cs not found - will be generated during build
)
echo.

REM Summary
echo ========================================
echo Validation Summary
echo ========================================
if %ERRORS% GTR 0 (
    echo ERRORS: %ERRORS%
    echo Build will likely FAIL - please fix errors first
)
if %WARNINGS% GTR 0 (
    echo WARNINGS: %WARNINGS%
    echo Build may have issues - review warnings
)

if %ERRORS% EQU 0 (
    if %WARNINGS% EQU 0 (
        echo All checks passed! Ready to build.
    ) else (
        echo Validation passed with warnings
        echo Consider running: nuget restore SwordAndStone.sln
    )
)

echo.
echo Next steps:
echo   1. Fix any ERRORS before building
echo   2. Address WARNINGS if possible
echo   3. Run: msbuild SwordAndStone.sln /p:Configuration=Debug
echo   4. Or build in Visual Studio
echo.

if %ERRORS% GTR 0 (
    exit /b 1
)
exit /b 0
