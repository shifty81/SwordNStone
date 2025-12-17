# Generate ProtoBuf files
mono CodeGenerator.exe Packet.proto

# Clean cito output directory
rm -r cito/output

# Create output directories
mkdir cito/output
mkdir cito/output/JsTa

# Compile JavaScript files
mono CitoAssets.exe data Assets.ci.cs
mono CiTo.exe -D CITO -D JS -D JSTA -l js-ta -o cito/output/JsTa/Assets.js Assets.ci.cs
mono CiTo.exe -D CITO -D JS -D JSTA -l js-ta -o cito/output/JsTa/SwordAndStone.js $(ls SwordAndStoneLib/Client/*.ci.cs) $(ls SwordAndStoneLib/Client/Mods/*.ci.cs) $(ls SwordAndStoneLib/Client/MainMenu/*.ci.cs) $(ls SwordAndStoneLib/Client/Misc/*.ci.cs) $(ls SwordAndStoneLib/Client/SimpleServer/*.ci.cs) $(ls SwordAndStoneLib/Common/*.ci.cs) Packet.Serializer.ci.cs

# Copy skeleton files
cp -r cito/platform/JsTa/* cito/output/JsTa/

# Fix CS0108 warnings in generated file by adding 'new' keyword to GetType() methods
# This is done AFTER CiTo transpilation because CiTo doesn't support the 'new' keyword
sed -i 's/^\([[:space:]]*\)public int GetType()/\1public new int GetType()/' Packet.Serializer.ci.cs
