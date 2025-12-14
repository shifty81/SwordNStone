#! /bin/bash
# Linux build script

rm -R -f output
mkdir output

cp -R data output

# Dll
cp SwordAndStoneLib/bin/Release/SwordAndStoneLib.dll output

# Scripting API
cp ScriptingApi/bin/Release/ScriptingApi.dll output

# Game Client
cp SwordAndStone/bin/Release/*.exe output

# Server
cp SwordAndStoneServer/bin/Release/*.exe output

# Monster editor
cp SwordAndStoneMonsterEditor/bin/Release/*.exe output

# Server Mods
cp -R SwordAndStoneLib/Server/Mods output

# Third-party libraries
cp Lib/* output

# NuGet packages
cp packages/OpenTK.2.0.0/lib/net20/OpenTK.dll output
cp packages/OpenTK.2.0.0/content/OpenTK.dll.config output
cp packages/protobuf-net.2.1.0/lib/net45/protobuf-net.dll output

rm -f output/*vshost.exe
cp COPYING.md output/credits.txt

# pause
