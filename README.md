[![Travis Build Status](https://img.shields.io/travis/manicdigger/manicdigger.svg?style=flat-square)](https://travis-ci.org/manicdigger/manicdigger)
[![GitHub Issues](https://img.shields.io/github/issues/manicdigger/manicdigger.svg?style=flat-square)](https://github.com/manicdigger/manicdigger/issues)
[![Latest GitHub Release](https://img.shields.io/github/release/manicdigger/manicdigger.svg?style=flat-square)](https://github.com/manicdigger/manicdigger/releases/latest)
[![GitHub Downloads](https://img.shields.io/github/downloads/manicdigger/manicdigger/latest/total.svg?style=flat-square)](https://github.com/manicdigger/manicdigger/releases/latest)

Manic Digger
============
Manic Digger is a 3D voxel building game similar to Minecraft.  
Build yourself a home in your own world or connect to an online server to team up with others to create great buildings!

You can download the game directly from the GitHub releases page:  
https://github.com/manicdigger/manicdigger/releases/latest


Features
--------
- Singleplayer and Multiplayer
- Full support for custom textures
- Powerful server side modding API
- Large world: 9984x9984x128 by default
- War game mode - first person shooter


Game modes
-----------
#### Creative Mode

In creative mode there are no limits on the amount of blocks you can place. Build whatever you like without having to worry about collecting resources or crafting.  
Build spaceships, flying islands or cool pixelart - your imagination is the only limit!

[![Spaceship](https://raw.githubusercontent.com/manicdigger/manicdigger-screenshots/master/2014-11-27_18-35-13-thumb.png)](https://raw.githubusercontent.com/manicdigger/manicdigger-screenshots/master/2014-11-27_18-35-13.png)
[![Floating Island](https://raw.githubusercontent.com/manicdigger/manicdigger-screenshots/master/2014-12-19_20-42-13-thumb.png)](https://raw.githubusercontent.com/manicdigger/manicdigger-screenshots/master/2014-12-19_20-42-13.png)
[![Pixelart](https://raw.githubusercontent.com/manicdigger/manicdigger-screenshots/master/2014-12-19_20-43-46-thumb.png)](https://raw.githubusercontent.com/manicdigger/manicdigger-screenshots/master/2014-12-19_20-43-46.png)
[![Mansion](https://raw.githubusercontent.com/manicdigger/manicdigger-screenshots/master/2014-12-25_22-38-53-thumb.png)](https://raw.githubusercontent.com/manicdigger/manicdigger-screenshots/master/2014-12-25_22-38-53.png)

#### Survival Mode

For those of you who like gathering resources and crafting stuff there is a survival mode.  
Please note that this is still in development (no friendly/hostile mobs right now)

[![City](https://raw.githubusercontent.com/manicdigger/manicdigger-screenshots/master/9c1d22eac9aac5f36bf12a5fb5c8a856-300x240.png)](https://raw.githubusercontent.com/manicdigger/manicdigger-screenshots/master/9c1d22eac9aac5f36bf12a5fb5c8a856.png)

#### War Mod

The War Mod is a gamemode that comes bundled with Manic Digger. It transforms the game into a fast-paced first-person shooter.

[![War Mod 1](https://raw.githubusercontent.com/manicdigger/manicdigger-screenshots/master/2012-10-15_02-13-14-300x227.png)](https://raw.githubusercontent.com/manicdigger/manicdigger-screenshots/master/2012-10-15_02-13-14.png)
[![War Mod 2](https://raw.githubusercontent.com/manicdigger/manicdigger-screenshots/master/2012-10-15_02-12-27-300x227.png)](https://raw.githubusercontent.com/manicdigger/manicdigger-screenshots/master/2012-10-15_02-12-27.png)


Links
-----
- Serverlist: http://manicdigger.sourceforge.net/play/
- Forum: http://manicdigger.sourceforge.net/forum/
- Wiki: http://manicdigger.sourceforge.net/wiki/
- IRC: http://chat.mibbit.com/?server=irc.esper.net&channel=%23manicdigger


Code
----
The OpenGL game client is written in a common subset of C# and [Ć programming lanuguage](http://cito.sourceforge.net/).  
It can be transcompiled to Java, C#, JavaScript, ActionScript, Perl and D.
The only external dependency is [GamePlatform interface](ManicDiggerLib/Client/Platform.ci.cs).

Server mods can be implemented in C# or interpreted Javascript.

#### Building

For detailed build instructions, see [BUILD.md](BUILD.md).

Quick start:
- **Windows**: Open `ManicDigger.sln` in Visual Studio and build
- **Linux/Mac**: Install Mono and run `xbuild ManicDigger.sln`

#### Contributing

If you want to help developing Manic Digger feel free to fork this repository on GitHub and submit your changes by sending a pull request.  
Once you've contributed some good patches you can also get direct push access to the repository.


License
-------
Manic Digger is **free and open source software** released into the **public domain** under the [Unlicense](http://unlicense.org).

**This means you can:**
- ✅ Use it to make your own game
- ✅ Modify and add features freely
- ✅ Use it commercially or non-commercially
- ✅ Distribute it in any form
- ✅ Do all of this **without asking permission** or providing attribution

While attribution is not required, it is always appreciated!

For detailed information about third-party libraries and their licenses, see [COPYING.md](COPYING.md)


Frequently Asked Questions
--------------------------

### Can I use this to make my own game?

**Yes, absolutely!** Manic Digger is released into the public domain. You are free to:
- Fork this project and create your own game
- Add your own features and modifications
- Change and mold it to fit your vision
- Release it under your own name
- Use it commercially

You don't need to ask permission or provide attribution (though giving credit is always nice). The code is yours to use however you wish!
