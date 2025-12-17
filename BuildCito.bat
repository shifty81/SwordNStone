@echo off
REM Enhanced BuildCito.bat with error handling and validation
echo ========================================
echo BuildCito - CiTo Transpilation Script
echo ========================================

REM 1. Validate prerequisites
echo [1/5] Checking prerequisites...
if not exist "CodeGenerator.exe" (
    echo ERROR: CodeGenerator.exe not found
    exit /b 1
)
if not exist "CiTo.exe" (
    echo ERROR: CiTo.exe not found
    exit /b 1
)
if not exist "Packet.proto" (
    echo ERROR: Packet.proto not found
    exit /b 1
)
echo OK: All required tools found

REM 2. Generate ProtoBuf files
echo [2/5] Generating ProtoBuf files...
CodeGenerator Packet.proto
if %ERRORLEVEL% NEQ 0 (
    echo ERROR: CodeGenerator failed with exit code %ERRORLEVEL%
    exit /b 1
)
echo OK: ProtoBuf files generated

REM 3. Clean cito output directory
echo [3/5] Cleaning output directory...
if exist "cito\output" (
    del /q /s cito\output >nul 2>&1
    rmdir /s /q cito\output >nul 2>&1
)

REM Create output directories
if not exist "cito\output" mkdir cito\output
if not exist "cito\output\JsTa" mkdir cito\output\JsTa
echo OK: Output directories prepared

REM 4. Create list of input files
echo [4/5] Collecting source files...
setlocal enabledelayedexpansion enableextensions
set LIST=
set FILE_COUNT=0
for %%x in (SwordAndStoneLib\Client\*.ci.cs) do (
    set LIST=!LIST! %%x
    set /a FILE_COUNT+=1
)
for %%x in (SwordAndStoneLib\Client\MainMenu\*.ci.cs) do (
    set LIST=!LIST! %%x
    set /a FILE_COUNT+=1
)
for %%x in (SwordAndStoneLib\Client\Mods\*.ci.cs) do (
    set LIST=!LIST! %%x
    set /a FILE_COUNT+=1
)
for %%x in (SwordAndStoneLib\Client\Misc\*.ci.cs) do (
    set LIST=!LIST! %%x
    set /a FILE_COUNT+=1
)
for %%x in (SwordAndStoneLib\Client\SimpleServer\*.ci.cs) do (
    set LIST=!LIST! %%x
    set /a FILE_COUNT+=1
)
for %%x in (SwordAndStoneLib\Common\*.ci.cs) do (
    set LIST=!LIST! %%x
    set /a FILE_COUNT+=1
)
set LIST=%LIST:~1%
echo OK: Found %FILE_COUNT% source files

REM 5. Compile JavaScript files
echo [5/5] Running CiTo transpilation...
IF NOT "%1"=="fast" (
    echo   - Generating Assets...
    CitoAssets data Assets.ci.cs
    if !ERRORLEVEL! NEQ 0 (
        echo ERROR: CitoAssets failed with exit code !ERRORLEVEL!
        exit /b 1
    )
    
    echo   - Transpiling Assets.js...
    cito -D CITO -D JS -D JSTA -l js-ta -o cito\output\JsTa\Assets.js Assets.ci.cs
    if !ERRORLEVEL! NEQ 0 (
        echo ERROR: CiTo transpilation of Assets.js failed with exit code !ERRORLEVEL!
        echo Check Assets.ci.cs for syntax errors
        exit /b 1
    )
    
    echo   - Transpiling SwordAndStone.js...
    cito -D CITO -D JS -D JSTA -l js-ta -o cito\output\JsTa\SwordAndStone.js %LIST% Packet.Serializer.ci.cs
    if !ERRORLEVEL! NEQ 0 (
        echo ERROR: CiTo transpilation of SwordAndStone.js failed with exit code !ERRORLEVEL!
        echo Check .ci.cs files for syntax errors
        echo Common issues:
        echo   - Complex nested expressions in IntRef.Create
        echo   - Unsupported 'ref' parameter usage
        echo   - Missing semicolons or mismatched braces
        exit /b 1
    )
    echo OK: CiTo transpilation successful
)

REM Copy skeleton files
echo Copying platform skeleton files...
copy cito\platform\JsTa\* cito\output\JsTa\* >nul 2>&1

REM Fix CS0108 warnings in generated file by adding 'new' keyword to GetType() methods
REM This is done AFTER CiTo transpilation because CiTo doesn't support the 'new' keyword
echo Applying CS0108 fix to Packet.Serializer.ci.cs...
powershell -Command "(Get-Content Packet.Serializer.ci.cs) -replace '(\s+)public int GetType\(\)', '$1public new int GetType()' | Set-Content Packet.Serializer.ci.cs"
if %ERRORLEVEL% NEQ 0 (
    echo WARNING: CS0108 fix failed - you may see compiler warnings
)

REM Additional transpilation targets (currently disabled)
REM mkdir cito\output\C
REM mkdir cito\output\Java
REM mkdir cito\output\Cs
REM IF NOT "%1"=="fast" cito -D CITO -D C -l c -o cito\output\C\ManicDigger.c %LIST% Packet.Serializer.ci.cs
REM IF NOT "%1"=="fast" cito -D CITO -D JAVA -l java -o cito\output\Java\ManicDigger.java -n manicdigger.lib  %LIST% Packet.Serializer.ci.cs
REM IF NOT "%1"=="fast" cito -D CITO -D CS -l cs -o cito\output\Cs\ManicDigger.cs %LIST% Packet.Serializer.ci.cs
REM copy cito\platform\C\* cito\output\C\*
REM copy cito\platform\Java\* cito\output\Java\*
REM copy cito\platform\Cs\* cito\output\Cs\*

echo ========================================
echo BuildCito completed successfully
echo ========================================
exit /b 0