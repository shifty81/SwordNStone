REM Windows build script

del /s /q output
mkdir output

xcopy /s data output\data\

REM Dll
xcopy /s SwordAndStoneLib\bin\release\SwordAndStoneLib.dll output\

REM Scripting API
xcopy /s ScriptingApi\bin\release\ScriptingApi.dll output\

REM Game Client
xcopy /s /y SwordAndStone\bin\release\*.exe output\

REM Server
xcopy /s /y SwordAndStoneServer\bin\release\*.exe output\

REM Monster editor
xcopy /s /y MdMonsterEditor\bin\Release\*.exe output\

REM Server Mods
mkdir output\Mods
xcopy /s SwordAndStoneLib\Server\Mods output\Mods\

REM Third-party libraries
xcopy /y /s Lib\*.* output\

REM NuGet packages
xcopy /s /y packages\OpenTK.2.0.0\lib\net20\OpenTK.dll output\
xcopy /s /y packages\OpenTK.2.0.0\content\OpenTK.dll.config output\
xcopy /s /y packages\protobuf-net.2.1.0\lib\net45\protobuf-net.dll output\

del output\*vshost.exe
copy COPYING.md output\credits.txt

REM pause
