@echo off
REM Post-Build Validation Script for SwordNStone
REM This script verifies the build completed successfully and all required outputs exist
REM Run this after building to verify metadata and DLLs are properly generated

setlocal enabledelayedexpansion
echo ========================================
echo SwordNStone Post-Build Validation
echo ========================================
echo.

set ERRORS=0
set WARNINGS=0
set CONFIGURATION=%1
if "%CONFIGURATION%"=="" set CONFIGURATION=Debug

echo Validating build for Configuration: %CONFIGURATION%
echo.

REM 1. Check SwordAndStoneLib output
echo [1/6] Checking SwordAndStoneLib output...
if exist "SwordAndStoneLib\bin\%CONFIGURATION%\SwordAndStoneLib.dll" (
    echo   OK: SwordAndStoneLib.dll found
) else (
    echo   ERROR: SwordAndStoneLib.dll not found
    echo   Path: SwordAndStoneLib\bin\%CONFIGURATION%\SwordAndStoneLib.dll
    set /a ERRORS+=1
)

if exist "SwordAndStoneLib\bin\%CONFIGURATION%\SwordAndStoneLib.pdb" (
    echo   OK: SwordAndStoneLib.pdb found
) else (
    echo   WARNING: SwordAndStoneLib.pdb not found (debug symbols)
    set /a WARNINGS+=1
)
echo.

REM 2. Check ScriptingApi output
echo [2/6] Checking ScriptingApi output...
if exist "ScriptingApi\bin\%CONFIGURATION%\ScriptingApi.dll" (
    echo   OK: ScriptingApi.dll found
) else (
    echo   ERROR: ScriptingApi.dll not found
    set /a ERRORS+=1
)
echo.

REM 3. Check SwordAndStone main executable
echo [3/6] Checking SwordAndStone executable...
if exist "SwordAndStone\bin\%CONFIGURATION%\SwordAndStone.exe" (
    echo   OK: SwordAndStone.exe found
) else (
    echo   ERROR: SwordAndStone.exe not found
    echo   Path: SwordAndStone\bin\%CONFIGURATION%\SwordAndStone.exe
    set /a ERRORS+=1
)
echo.

REM 4. Check NuGet package DLLs are copied
echo [4/6] Checking NuGet package dependencies...
if exist "SwordAndStoneLib\bin\%CONFIGURATION%\protobuf-net.dll" (
    echo   OK: protobuf-net.dll copied to output
) else (
    echo   WARNING: protobuf-net.dll not in output directory
    echo   May cause runtime errors if not in GAC
    set /a WARNINGS+=1
)

if exist "SwordAndStone\bin\%CONFIGURATION%\OpenTK.dll" (
    echo   OK: OpenTK.dll copied to output
) else (
    echo   WARNING: OpenTK.dll not in output directory
    echo   May cause runtime errors
    set /a WARNINGS+=1
)
echo.

REM 5. Check generated files
echo [5/6] Checking generated files...
if exist "Packet.Serializer.ci.cs" (
    echo   OK: Packet.Serializer.ci.cs generated
    
    REM Check for CS0108 fix
    findstr /C:"public new int GetType" "Packet.Serializer.ci.cs" >nul 2>&1
    if !ERRORLEVEL! EQU 0 (
        echo   OK: CS0108 fix applied
    ) else (
        echo   WARNING: CS0108 fix not applied
        echo   You may see CS0108 warnings in build output
        set /a WARNINGS+=1
    )
) else (
    echo   WARNING: Packet.Serializer.ci.cs not found
    echo   May be generated during build
    set /a WARNINGS+=1
)

if exist "cito\output\JsTa\SwordAndStone.js" (
    echo   OK: JavaScript transpilation output found
) else (
    echo   INFO: JavaScript output not found (may be disabled)
)
echo.

REM 6. Check for common build issues in output
echo [6/6] Checking for common build warnings...
REM This would ideally parse the build log, but we'll do basic checks

REM Check if obj directories exist (successful compilation)
if exist "SwordAndStoneLib\obj\%CONFIGURATION%" (
    echo   OK: SwordAndStoneLib compiled (obj directory exists)
) else (
    echo   WARNING: SwordAndStoneLib obj directory not found
    set /a WARNINGS+=1
)

if exist "SwordAndStone\obj\%CONFIGURATION%" (
    echo   OK: SwordAndStone compiled (obj directory exists)
) else (
    echo   WARNING: SwordAndStone obj directory not found
    set /a WARNINGS+=1
)
echo.

REM Summary
echo ========================================
echo Post-Build Validation Summary
echo ========================================
if %ERRORS% GTR 0 (
    echo ERRORS: %ERRORS%
    echo Build did NOT complete successfully!
)
if %WARNINGS% GTR 0 (
    echo WARNINGS: %WARNINGS%
    echo Build completed but with issues
)

if %ERRORS% EQU 0 (
    if %WARNINGS% EQU 0 (
        echo All checks passed! Build successful.
        echo.
        echo Output locations:
        echo   Library: SwordAndStoneLib\bin\%CONFIGURATION%\SwordAndStoneLib.dll
        echo   Executable: SwordAndStone\bin\%CONFIGURATION%\SwordAndStone.exe
    ) else (
        echo Build completed with warnings
    )
) else (
    echo Build validation FAILED!
    echo Common causes:
    echo   1. Pre-build event failed (BuildCito.bat)
    echo   2. Missing NuGet packages
    echo   3. Compilation errors
    echo   4. Missing project references
    echo.
    echo Run pre-build-validation.bat to diagnose
)

echo.
if %ERRORS% GTR 0 (
    exit /b 1
)
exit /b 0
