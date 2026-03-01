# Sword & Stone — Modernization Roadmap

**Last Updated:** March 2026  
**Current Version:** Alpha 0.0.3-dev  
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

## ✅ Milestone 1: Build System Modernization

> Make the project build reliably on modern toolchains and CI.

### 1a. Migrate to SDK-style Project Files ✅
All 6 projects migrated from legacy verbose `.csproj` to SDK-style:

- [x] `ScriptingApi/ScriptingApi.csproj`
- [x] `SwordAndStoneLib/SwordAndStoneLib.csproj`
- [x] `SwordAndStone.Tests/SwordAndStone.Tests.csproj`
- [x] `SwordAndStoneServer/SwordAndStoneServer.csproj`
- [x] `SwordAndStone/SwordAndStone.csproj`
- [x] `SwordAndStoneMonsterEditor/SwordAndStoneMonsterEditor.csproj`

### 1b. Target Modern .NET ✅
- [x] Retargeted from `.NET Framework 4.8` → `net8.0` (LTS)
- [x] Added `System.Drawing.Common` NuGet for cross-platform System.Drawing
- [x] Updated NuGet packages:
  - `OpenTK 2.0.0` → `OpenTK 4.9.4` (with full API migration)
  - `protobuf-net 2.1.0` → `protobuf-net 3.2.56`
  - `NUnit 3.13.3` → `NUnit 3.14.0` + NUnit3TestAdapter + Microsoft.NET.Test.Sdk
- [x] Added `System.CodeDom` NuGet for runtime compilation support
- [x] Replaced `System.Windows.Forms` usage with cross-platform alternatives
- [x] Replaced `System.Web.HttpUtility` with `System.Net.WebUtility`
- [x] Vendored DLLs retained in Lib/ for ENet, Jint, LibNoise, websocket-sharp, SQLite, csogg/csvorbis

### 1c. Add GitHub Actions CI ✅
- [x] Created `.github/workflows/build-validation.yml` — build + test on push/PR
- [x] Added build status badge to README.md
- [x] Removed `.travis.yml` (legacy CI)

### 1d. Clean Up Build Scripts ✅
- [x] Removed `packages.config` files (replaced by PackageReference in csproj)
- [x] Updated solution file (removed FastBuild config, modern project type GUIDs)
- [x] Primary build commands: `dotnet build` / `dotnet test`

---

## ✅ Milestone 2: Code Architecture Cleanup (Partial)

> Untangle the spaghetti. Make the code navigable and maintainable.

### 2b. Server Architecture (Partial) ✅
- [x] Split `Server.cs` partial class (4,434 → 808 lines) into focused files:
  - `ServerPlayerManager.cs` — player spawn/despawn/tracking, inventory notifications, stats
  - `ServerNetworkProcessor.cs` — network packet dispatch (TryReadPacket)
  - `ServerBuildCraft.cs` — build/craft/privileges, fill area operations
  - `ServerNetworkSend.cs` — all Send* methods, messaging, entity management
  - `ServerTypes.cs` — supporting types (GameTime, Vector3i, ServerSystem, etc.)

### 2d. Fix Remaining Compiler Warnings ✅
- [x] Reduced compiler warnings from **302 → 0**:
  - Fixed `SYSLIB0021`: `MD5CryptoServiceProvider` → `MD5.Create()`
  - Fixed `SYSLIB0014`: `WebClient`/`WebRequest` → `HttpClient`
  - Suppressed `CA1416` platform compatibility warnings (MonsterEditor already `net8.0-windows`, System.Drawing usage in Client/Lib)
  - Suppressed `CS0649` in Lib (94 warnings, all in auto-generated `.ci.cs` files)

### 2a. Namespace & Project Alignment
- [ ] Align namespaces with directory structure consistently
- [ ] Move Cito-transpiled `.ci.cs` files into proper namespace directories
- [ ] Evaluate whether Cito transpilation is still needed (if not targeting Java/JS, remove it)

### 2b. Server Architecture (Remaining)
- [ ] Consolidate network backends (ENet/TCP/WebSocket/Dummy) behind clean interface
- [ ] Clean up `ServerCommand.cs` — extract command handlers into separate classes

### 2c. Client Architecture
- [ ] Extract `Game.ci.cs` rendering subsystems into focused modules
- [ ] Consolidate GUI mod system — single consistent UI framework
- [ ] Remove commented-out code blocks throughout Client/

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
| Alpha 0.0.3 | Mar 2026 | Build system modernization: .NET 8.0, SDK-style csproj, GitHub Actions CI (Milestone 1) |
| Alpha 0.0.4-dev | Mar 2026 | Server.cs split, all compiler warnings fixed (Milestone 2 partial) |

---

*This is a living document. Updated as milestones are completed.*
