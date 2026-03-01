# Sword & Stone — Modernization Roadmap

**Last Updated:** March 2026  
**Current Version:** Alpha 0.0.2-dev  
**Project Status:** Active Development — Modernization Phase

---

## Overview

This roadmap defines the concrete path to transform Sword & Stone from its Manic Digger heritage into a clean, modern, standalone game. Each milestone builds on the previous one. Work items are ordered by dependency — complete them in sequence.

**Guiding Principles:**
- Fix the foundation before adding features
- Every system must be integrated, not bolted on
- If code isn't called, it's deleted
- If a feature isn't finished, it's either completed or removed

---

## ✅ Milestone 0: Code Cleanup & Foundation (This Release)

> Clean house. Remove dead code, fix bare exceptions, consolidate docs.

- [x] **Audit** — Full codebase audit identifying stubs, dead code, spaghetti patterns
- [x] **Exception hygiene** — Replace all bare `throw new Exception()` with descriptive messages
  - Inventory.cs: `ArgumentOutOfRangeException` for invalid wear places
  - ChunkDb.cs: Descriptive messages with coordinates
  - War.cs: `ArgumentOutOfRangeException` for unknown team types
- [x] **Dead code removal** — Delete `Server/Mods/Unused/` directory (4 files, 721 lines)
- [x] **Stale TODO cleanup** — Resolve 5 "TODO: wrong?" in ServerWorldManager.cs (Array.Clear was correct)
- [x] **Inventory refactor** — Extract `TryPlaceItemInMainArea()` helper, eliminate duplicate code
- [x] **Documentation consolidation** — Move 57 scattered .md files from root into organized `docs/` directory
- [x] **Remove dead stubs** — ChunkDbPlainFile commented-out class, SetChunkToFile dead code

---

## 🔲 Milestone 1: Build System Modernization

> Make the project build reliably on modern toolchains and CI.

### 1a. Migrate to SDK-style Project Files
The current `.csproj` files use the legacy verbose XML format (VS2012 era). Migrate to SDK-style:

```xml
<!-- Before (legacy) -->
<Project ToolsVersion="4.0" DefaultTargets="Build" xmlns="...">
  <PropertyGroup>
    <TargetFrameworkVersion>v4.8</TargetFrameworkVersion>
    <!-- 50+ lines of boilerplate -->
  </PropertyGroup>
  <ItemGroup>
    <Compile Include="Server\File1.cs" />
    <Compile Include="Server\File2.cs" />
    <!-- every file listed explicitly -->
  </ItemGroup>
</Project>

<!-- After (SDK-style) -->
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
  </PropertyGroup>
</Project>
```

**Files to migrate:**
- [ ] `ScriptingApi/ScriptingApi.csproj` (simplest — no native deps, start here)
- [ ] `SwordAndStoneLib/SwordAndStoneLib.csproj` (core library)
- [ ] `SwordAndStone.Tests/SwordAndStone.Tests.csproj` (test project)
- [ ] `SwordAndStoneServer/SwordAndStoneServer.csproj` (console app)
- [ ] `SwordAndStone/SwordAndStone.csproj` (game client — most complex, OpenTK)
- [ ] `SwordAndStoneMonsterEditor/SwordAndStoneMonsterEditor.csproj` (WinForms tool)

### 1b. Target Modern .NET
- [ ] Retarget from `.NET Framework 4.8` → `net8.0` (LTS)
- [ ] Replace `System.Drawing` usage with cross-platform alternative where needed
- [ ] Update NuGet packages:
  - `OpenTK 2.0.0` → `OpenTK 4.x` (major API changes — plan for this)
  - `protobuf-net 2.1.0` → `protobuf-net 3.x`
  - `NUnit 3.13.3` → latest 3.x or 4.x
- [ ] Handle native library loading (ENet, OpenAL, SQLite) for cross-platform
- [ ] Remove Lib/ directory vendored DLLs in favor of NuGet packages

### 1c. Add GitHub Actions CI
- [ ] Create `.github/workflows/build.yml` — build on push/PR
- [ ] Create `.github/workflows/test.yml` — run NUnit tests
- [ ] Add build status badge to README.md
- [ ] Remove `.travis.yml` (legacy CI)

### 1d. Clean Up Build Scripts
- [ ] Remove or update legacy build scripts (`build.bat`, `build.sh`, `BuildCito.*`)
- [ ] Remove legacy validation scripts (`pre-build-validation.*`, `post-build-validation.*`, `validate-build.*`)
- [ ] Consolidate to `dotnet build` / `dotnet test` / `dotnet publish`

---

## 🔲 Milestone 2: Code Architecture Cleanup

> Untangle the spaghetti. Make the code navigable and maintainable.

### 2a. Namespace & Project Alignment
- [ ] Align namespaces with directory structure consistently
- [ ] Move Cito-transpiled `.ci.cs` files into proper namespace directories
- [ ] Evaluate whether Cito transpilation is still needed (if not targeting Java/JS, remove it)

### 2b. Server Architecture
- [ ] Split `Server.cs` partial class into focused files:
  - `ServerChunkManager.cs` — chunk loading/sending
  - `ServerPlayerManager.cs` — player spawn/despawn/tracking
  - `ServerInventoryManager.cs` — inventory operations
  - `ServerLightingManager.cs` — lighting calculations
- [ ] Consolidate network backends (ENet/TCP/WebSocket/Dummy) behind clean interface
- [ ] Clean up `ServerCommand.cs` — extract command handlers into separate classes

### 2c. Client Architecture
- [ ] Extract `Game.ci.cs` rendering subsystems into focused modules
- [ ] Consolidate GUI mod system — single consistent UI framework
- [ ] Remove commented-out code blocks throughout Client/

### 2d. Fix Remaining Compiler Warnings
- [ ] Address ~107 compiler warnings systematically:
  - Unused variables → remove or use
  - Obsolete API calls → update to modern equivalents
  - Missing XML doc comments → add where public API

---

## 🔲 Milestone 3: Feature Integration

> Wire up the features that exist but aren't connected.

### 3a. Combat System Integration
- [ ] Wire up `CombatSystem.cs` and `Weapons.cs` in Fortress mod to game loop
- [ ] Connect combat animations to actual damage calculations
- [ ] Implement health/damage display in UI unit frames
- [ ] Complete War.cs `//TODO: medic subclass` and `//TODO: support subclass`

### 3b. Character System Integration
- [ ] Connect character customization selections to actual rendered models
- [ ] Wire up pixel art skin editor output to character rendering
- [ ] Implement character persistence (save/load character data independently of world)
- [ ] Connect equipment visuals to inventory system

### 3c. Survival Mode Completion
- [ ] Wire up crafting recipes to actual block/item creation
- [ ] Connect tool durability to block breaking system
- [ ] Implement hunger/health regeneration loop
- [ ] Connect resource drops to block destruction

### 3d. UI Consolidation
- [ ] Merge overlapping GUI implementations (golden UI + standard UI)
- [ ] Wire up minimap to actual terrain data
- [ ] Connect action bar slots to inventory hotbar
- [ ] Implement settings menu that persists preferences

---

## 🔲 Milestone 4: Testing & Quality

> Build confidence that changes don't break things.

### 4a. Expand Unit Test Coverage
Current test files cover basic scenarios. Target coverage areas:
- [ ] Inventory system — stacking, overflow, wear places, move-to-inventory
- [ ] Chunk database — read/write/delete operations
- [ ] Server commands — command parsing and execution
- [ ] Network packet serialization — round-trip tests
- [ ] World generation — deterministic seed tests
- [ ] Combat damage calculation

### 4b. Integration Tests
- [ ] Single-player game loop — start game, place block, verify
- [ ] Client-server connection — connect, authenticate, receive chunks
- [ ] Mod loading — load mod, verify hooks fire

### 4c. Automated Quality Gates
- [ ] Require tests pass before merge (CI)
- [ ] Add code coverage reporting
- [ ] Add linting / code style enforcement

---

## 🔲 Milestone 5: Standalone Game Polish

> Make it feel like a real game, not a tech demo.

### 5a. New Player Experience
- [ ] Main menu → single player → world generation → gameplay loop (seamless)
- [ ] Tutorial hints for first-time players
- [ ] Default key bindings that make sense
- [ ] Loading screen with progress indicator

### 5b. Audio
- [ ] Block place/break sounds
- [ ] Footstep sounds (surface-dependent)
- [ ] Ambient background audio
- [ ] UI click/hover sounds

### 5c. Visual Polish
- [ ] Consistent block textures
- [ ] Particle effects (block breaking, torch flames)
- [ ] Smooth camera transitions
- [ ] Day/night cycle lighting improvements

### 5d. Packaging & Distribution
- [ ] Single executable or installer
- [ ] Bundled assets (no external file dependencies)
- [ ] Platform-specific builds (Windows, Linux, macOS)
- [ ] Version number in title bar and about screen

---

## Future: Feature Roadmap (Post-Modernization)

Once the foundation is solid, these features can be developed cleanly:

| Priority | Feature | Design Doc |
|----------|---------|------------|
| High | Water system overhaul | [docs/design/WATER_SYSTEM_DESIGN.md](docs/design/WATER_SYSTEM_DESIGN.md) |
| High | World simulation (seasons, weather, farming) | [docs/design/WORLD_SIMULATION_DESIGN.md](docs/design/WORLD_SIMULATION_DESIGN.md) |
| High | Enemy mobs and AI pathfinding | — |
| Medium | VOIP and emote system | [docs/design/VOIP_AND_EMOTE_SYSTEM.md](docs/design/VOIP_AND_EMOTE_SYSTEM.md) |
| Medium | Magic system | — |
| Medium | Quest and NPC dialogue system | — |
| Low | Mobile port | — |
| Low | Advanced shaders and post-processing | — |

See [docs/design/GAME_DESIGN.md](docs/design/GAME_DESIGN.md) for the full design vision.

---

## Architecture Reference

```
SwordAndStone.sln
├── ScriptingApi/           # Server-side modding API (public interface for mods)
├── SwordAndStoneLib/       # Core shared library
│   ├── Client/             # Client game logic, rendering, UI
│   │   ├── MainMenu/       # Menu screens, editors
│   │   ├── Mods/           # Client-side mod hooks (GUI, HUD)
│   │   └── Misc/           # Utilities, math, helpers
│   ├── Server/             # Server game logic
│   │   ├── Mods/           # Game modes (Fortress, War)
│   │   └── Systems/        # Server subsystems (loading, heartbeat, etc.)
│   └── Common/             # Shared code (network, assets, compression)
├── SwordAndStone/          # Game client executable
├── SwordAndStoneServer/    # Dedicated server executable
├── SwordAndStoneMonsterEditor/ # Model editor tool
├── SwordAndStone.Tests/    # NUnit test project
└── docs/                   # Organized documentation
    ├── design/             # Design specs for planned features
    ├── guides/             # How-to guides
    ├── build/              # Build troubleshooting
    └── implementation/     # Historical implementation notes
```

---

## Version History

| Version | Date | Milestone |
|---------|------|-----------|
| Alpha 0.0.1 | Dec 2025 | Complete rebrand from Manic Digger |
| Alpha 0.0.2 | Mar 2026 | Code cleanup, dead code removal, doc consolidation (Milestone 0) |

---

*This is a living document. Updated as milestones are completed.*
