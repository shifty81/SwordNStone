# Project Status — Sword & Stone

**Last Updated:** March 2026  
**Current Version:** Alpha 0.0.4-dev  
**Modernization Phase:** Milestone 2 In Progress

---

## Current State

Sword & Stone has completed its build system modernization (Milestone 1) and is partway through code architecture cleanup (Milestone 2). The Server.cs monolith has been split into focused files, all compiler warnings have been eliminated, and deprecated APIs have been replaced with modern equivalents.

### Build System
| Item | Status | Notes |
|------|--------|-------|
| Solution structure | ✅ Working | 6 projects in `SwordAndStone.sln` |
| Target framework | ✅ Modern | .NET 8.0 (MonsterEditor: net8.0-windows) |
| Project file format | ✅ Modern | SDK-style `.csproj` files |
| Windows build | ✅ Working | `dotnet build` with .NET 8.0 SDK |
| Linux/Mac build | ✅ Working | `dotnet build` (except MonsterEditor: Windows-only) |
| CI/CD | ✅ Active | GitHub Actions — build + test on every push/PR |
| NuGet packages | ✅ Current | OpenTK 4.9.4, protobuf-net 3.2.56, NUnit 3.14.0 |

### Projects
| Project | Type | Status |
|---------|------|--------|
| `SwordAndStone` | Game Client (WinExe) | ✅ Compiles |
| `SwordAndStoneLib` | Core Library (DLL) | ✅ Compiles |
| `SwordAndStoneServer` | Dedicated Server (Console) | ✅ Compiles |
| `ScriptingApi` | Modding API (DLL) | ✅ Compiles |
| `SwordAndStoneMonsterEditor` | Model Editor (WinForms) | ✅ Compiles |
| `SwordAndStone.Tests` | Unit Tests (NUnit) | ✅ Compiles |

### Code Quality
| Area | Status | Details |
|------|--------|---------|
| Dead code | ✅ Cleaned | Removed `Server/Mods/Unused/` (4 files, 721 lines) |
| Bare exceptions | ✅ Fixed | All `throw new Exception()` replaced with descriptive messages |
| Stale TODOs | ✅ Resolved | ServerWorldManager.cs "TODO: wrong?" resolved (5 locations) |
| Duplicate code | ✅ Refactored | Inventory `MoveToInventory` extracted to `TryPlaceItemInMainArea()` |
| Compiler warnings | ✅ Fixed | 302 → 0 warnings (deprecated APIs fixed, platform warnings suppressed) |
| Remaining TODOs | ⚠️ ~40 | Mostly performance notes, not missing features |
| Server architecture | ✅ Improved | Server.cs split from 4,434 → 808 lines; ServerCommand.cs split from 2,191 → 712 lines; NetworkBackendFactory |
| Test coverage | ⚠️ Growing | 11 test files, 108 tests — expanding |

### Feature Status
| Feature | Status | Notes |
|---------|--------|-------|
| Voxel building | ✅ Working | Place/break blocks, multiple types, large worlds |
| Creative mode | ✅ Working | Unlimited blocks |
| Multiplayer | ✅ Working | Dedicated server, ENet/TCP/WebSocket backends |
| Day/night cycle | ✅ Working | Sky sphere, lighting system |
| Golden UI system | ✅ Working | WoW-inspired interface, action bar, minimap |
| Character animation | ✅ Working | Walk, idle, head rotation |
| Character customization | 🔄 Partial | UI exists, not fully wired to rendering |
| Pixel art skin editor | 🔄 Partial | Editor exists, integration incomplete |
| Survival mode | 🔄 Partial | Basic mechanics, no crafting loop |
| Combat system | 🔄 Partial | Animations defined, damage not wired |
| War mode | ✅ Working | FPS game mode via server mod |
| World generation | ✅ Working | Noise terrain, caves, ores |
| Modding API | ✅ Working | C# and JavaScript server mods |
| VOIP | ⏳ Design only | Design doc exists, no implementation |
| Water system | ⏳ Design only | Design doc exists, basic fluid rendering only |
| World simulation | ⏳ Design only | Seasons/weather/NPC — design doc only |

## Next Steps

See [ROADMAP.md](ROADMAP.md) for the full modernization plan. Current progress:

**Milestone 2: Code Architecture Cleanup** (in progress)
- ✅ Server.cs split into focused partial class files
- ✅ ServerCommand.cs split into focused partial class files (Chat, Admin, Gameplay)
- ✅ Network backends consolidated behind NetworkBackendFactory
- ✅ All compiler warnings fixed (302 → 0)
- Remaining: namespace alignment, client architecture cleanup

### Known Issues
1. **ENet on Linux/Mono** — Native library compatibility issues. Workaround: use TCP backend.
2. **~40 remaining TODOs** — Mostly performance notes, not missing features.

---

## Platform Support

| Platform | Build | Runtime | Notes |
|----------|-------|---------|-------|
| Windows | ✅ Full | ✅ Full | Visual Studio or MSBuild |
| Linux | ⚠️ Mono | ⚠️ Partial | ENet issues, X11 required for client |
| macOS | ⚠️ Mono | ⚠️ Untested | Likely similar to Linux |

---

## Community Resources

- **GitHub**: [github.com/shifty81/SwordNStone](https://github.com/shifty81/SwordNStone)
- **License**: Public domain ([Unlicense](http://unlicense.org))
