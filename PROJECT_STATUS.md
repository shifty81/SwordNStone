# Project Status — Sword & Stone

**Last Updated:** March 2026  
**Current Version:** Alpha 0.0.2-dev  
**Modernization Phase:** Milestone 0 Complete

---

## Current State

Sword & Stone has completed its code audit and initial cleanup (Milestone 0 of the modernization roadmap). The project compiles on Windows with Visual Studio / Mono and has a clean foundation for further modernization.

### Build System
| Item | Status | Notes |
|------|--------|-------|
| Solution structure | ✅ Working | 6 projects in `SwordAndStone.sln` |
| Target framework | ⚠️ Legacy | .NET Framework 4.8 — needs migration to .NET 8.0 |
| Project file format | ⚠️ Legacy | Old-style verbose `.csproj` — needs SDK-style migration |
| Windows build | ✅ Working | Visual Studio 2012+ or MSBuild |
| Linux/Mac build | ⚠️ Partial | Requires Mono — no .NET 8+ SDK support yet |
| CI/CD | ❌ None | `.travis.yml` exists but outdated; needs GitHub Actions |
| NuGet packages | ⚠️ Outdated | OpenTK 2.0, protobuf-net 2.1, NUnit 3.13.3 |

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
| Compiler warnings | ⚠️ ~107 | Unused variables, obsolete APIs — non-blocking |
| Remaining TODOs | ⚠️ ~40 | Mostly performance notes, not missing features |
| Test coverage | ⚠️ Minimal | 9 test files with basic coverage — needs expansion |

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

### Known Issues
1. **ENet on Linux/Mono** — Native library compatibility issues. Workaround: use TCP backend.
2. **No CI pipeline** — Builds not automatically validated on push/PR.
3. **Legacy .NET Framework** — Can't build with modern `dotnet` CLI without targeting packs.
4. **~107 compiler warnings** — Non-blocking but noisy.

---

## What Was Done (Milestone 0)

### Code Fixes
- Replaced bare `throw new Exception()` with `ArgumentOutOfRangeException` and descriptive messages in:
  - `Inventory.cs` — wear place validation (4 locations)
  - `ChunkDb.cs` — chunk count validation (2 locations)  
  - `War.cs` — team color lookup (1 location)
- Resolved 5 stale "TODO: wrong?" comments in `ServerWorldManager.cs` — the `Array.Clear()` calls were correct; removed dead commented-out `.Clear()` calls
- Extracted `TryPlaceItemInMainArea()` from `MoveToInventory()` in `Inventory.cs` — eliminated duplicate code with `GrabItem()`
- Removed dead commented-out `ChunkDbPlainFile` class stub and `SetChunkToFile` method from `ChunkDb.cs`

### Dead Code Removal
- Deleted `Server/Mods/Unused/` directory: `BlockId.cs`, `Fluids.cs`, `PermissionBlock.cs`, `Sign.cs` (721 lines)
- Removed corresponding commented-out `<Compile>` entries from `SwordAndStoneLib.csproj`

### Documentation Consolidation
- Moved 57 markdown files from repository root into organized `docs/` directory:
  - `docs/design/` — Design specs (game design, water, world sim, combat, VOIP)
  - `docs/guides/` — How-to guides (character customization, skin editor, UI, animations)
  - `docs/build/` — Build troubleshooting and validation
  - `docs/implementation/` — Historical implementation notes and summaries
- Created `docs/README.md` index
- Updated all cross-references in `README.md`
- Root now contains only essential docs: README, LICENSE, BUILD, ROADMAP, etc.

---

## Next Steps

See [ROADMAP.md](ROADMAP.md) for the full modernization plan. Next milestone:

**Milestone 1: Build System Modernization**
- Migrate to SDK-style `.csproj` files
- Retarget to .NET 8.0
- Add GitHub Actions CI
- Update NuGet dependencies

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
