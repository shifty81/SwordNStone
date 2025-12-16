Sword&Stone
============
Sword&Stone is a 3D voxel building game similar to Minecraft.  
Build yourself a home in your own world or connect to an online server to team up with others to create great buildings!


Features
--------
- Singleplayer and Multiplayer
- **Character Customization** - Customize your character's appearance with different hairstyles, beards, outfits, and genders
- Full support for custom textures
- Powerful server side modding API
- Large world: 9984x9984x128 by default
- War game mode - first person shooter


Game modes
-----------
#### Creative Mode

In creative mode there are no limits on the amount of blocks you can place. Build whatever you like without having to worry about collecting resources or crafting.  
Build spaceships, flying islands or cool pixelart - your imagination is the only limit!

#### Survival Mode

For those of you who like gathering resources and crafting stuff there is a survival mode.  
Please note that this is still in development (no friendly/hostile mobs right now)

#### War Mod

The War Mod is a gamemode that transforms the game into a fast-paced first-person shooter.


Character Customization
-----------------------

Customize your character's appearance to make them truly your own!

**Features:**
- **Gender Selection**: Choose between male and female character models
- **Hairstyles**: 5 different hairstyle options (Short, Medium, Long, Bald, Ponytail)
- **Facial Hair**: 4 beard options for that perfect look (None, Short, Long, Goatee)
- **Outfits**: 4 outfit styles to match your playstyle (Default, Armor, Robe, Casual)
- **🎨 Pixel Art Skin Editor**: Create fully custom character skins with an integrated in-game editor!

**How to Use:**
1. From the main menu, select **Singleplayer**
2. Click the **Character...** button to open the Character Creator
3. Use the arrow buttons to cycle through options for each category
4. Click **Confirm** to save your customization
5. Your character will appear with your chosen appearance in both first-person and third-person views

**Pixel Art Skin Editor:**
Create your own unique character skins from scratch!
- **Access:** Character Creator → **Skin Editor** button
- **Tools:** Brush, Eraser, Fill Bucket, Color Picker
- **Features:** RGB color picker, dual layers, real-time 3D preview
- **Templates:** Start with blank templates or pre-made example skins
- See [PIXEL_ART_SKIN_SYSTEM.md](PIXEL_ART_SKIN_SYSTEM.md) for complete guide

**For Content Creators:**
The system supports custom character textures! See [CHARACTER_CUSTOMIZATION.md](CHARACTER_CUSTOMIZATION.md), [PIXEL_ART_SKIN_SYSTEM.md](PIXEL_ART_SKIN_SYSTEM.md), and [EXAMPLE_SKINS_GUIDE.md](EXAMPLE_SKINS_GUIDE.md) for details on creating your own character skins.


Code
----
The OpenGL game client is written in a common subset of C# and [Ć programming language](http://cito.sourceforge.net/).  
It can be transcompiled to Java, C#, JavaScript, ActionScript, Perl and D.
The only external dependency is [GamePlatform interface](SwordAndStoneLib/Client/Platform.ci.cs).

Server mods can be implemented in C# or interpreted Javascript.

#### Building

For detailed build instructions, see [BUILD.md](BUILD.md).

Quick start:
- **Windows**: Open `SwordAndStone.sln` in Visual Studio and build
- **Linux/Mac**: Install Mono and run `xbuild SwordAndStone.sln`

#### Contributing

If you want to help developing Sword&Stone feel free to fork this repository on GitHub and submit your changes by sending a pull request.


Credits
-------
**Sword&Stone** is a fork and evolution of the original **Manic Digger** project.

### Original Project
This project is based on **Manic Digger**, an open source voxel building game released into the public domain.
- Original repository: https://github.com/manicdigger/manicdigger
- The original Manic Digger was created by the Manic Digger development team

### Procedural Generation
The procedural world generation code in this project is derived from the original Manic Digger implementation and has been heavily modified and extended. The noise-based terrain generation algorithms (Noise2DWorldGenerator and related code) originated from the Manic Digger codebase.

### Third-Party Libraries
For detailed information about third-party libraries and their licenses, see [COPYING.md](COPYING.md)


License
-------
Sword&Stone is **free and open source software** released into the **public domain** under the [Unlicense](http://unlicense.org).

**This means you can:**
- ✅ Use it to make your own game
- ✅ Modify and add features freely
- ✅ Use it commercially or non-commercially
- ✅ Distribute it in any form
- ✅ Do all of this **without asking permission** or providing attribution

While attribution is not required, it is always appreciated!


Frequently Asked Questions
--------------------------

### Can I use this to make my own game?

**Yes, absolutely!** Sword&Stone is released into the public domain. You are free to:
- Fork this project and create your own game
- Add your own features and modifications
- Change and mold it to fit your vision
- Release it under your own name
- Use it commercially

You don't need to ask permission or provide attribution (though giving credit is always nice). The code is yours to use however you wish!
