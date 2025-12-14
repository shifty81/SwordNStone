# Sword & Stone - Development Roadmap

**Last Updated:** December 2025  
**Project Status:** Active Development

---

## 🎯 Project Vision

Sword & Stone aims to be a comprehensive voxel-based adventure game combining the creativity of building games with RPG elements, multiplayer cooperation, and combat mechanics. Our goal is to create an immersive world where players can build, explore, fight, and socialize together.

---

## ✅ Current Features (Implemented)

### Core Gameplay
- **Voxel Building System**
  - Place and break blocks
  - Multiple block types (stone, dirt, wood, grass, sand, water, etc.)
  - Large worlds (9984×9984×128 blocks by default)
  - Chunk-based rendering and loading

- **Game Modes**
  - Creative Mode: Unlimited blocks for building
  - Survival Mode: Resource gathering (in development)
  - War Mode: First-person shooter mechanics

- **Multiplayer**
  - Dedicated server support
  - Online multiplayer with server browser
  - Player synchronization
  - Local and remote server support

### Graphics & UI
- **3D Rendering**
  - OpenGL-based graphics engine
  - Day/night cycle
  - Sky sphere and celestial bodies
  - Lighting system (torch light, sunlight, block light)
  - Frustum culling for performance

- **Golden UI System (Enhanced)**
  - WoW-inspired immersive interface
  - Action bar with 10 hotkey slots
  - Unit frames (player and target)
  - Minimap with terrain visualization
  - Consistent golden aesthetic across all UI elements
  - Inventory system with visual feedback
  - Chat system

### Player Systems
- **Character Animation**
  - Walk animation with leg and arm movement
  - Idle animation with breathing
  - Head rotation following camera
  - Animated player models
  - Support for custom player skins

- **Player Physics**
  - Gravity and jumping
  - Collision detection
  - Swimming and oxygen system
  - Fall damage

### World Generation
- **Procedural Terrain**
  - Noise-based terrain generation
  - Multiple biomes
  - Caves and underground systems
  - Ore distribution

### Modding & Extensibility
- **Server-Side Modding**
  - C# mod support
  - JavaScript mod support (interpreted)
  - Powerful modding API (ScriptingApi)
  - Custom game modes possible

- **Asset Customization**
  - Custom textures support
  - Custom player models
  - Custom block definitions

---

## 🚧 In Progress

### Enhanced Character System
- **Advanced Animations** (PRIORITY)
  - ✅ Chopping/mining animation (tool swinging)
  - ✅ Bow drawing and firing animation
  - ✅ Sword and shield combat animations
  - ✅ Emote system (wave, point, cheer, talk)
  - 🔄 Animation state machine implementation
  - 🔄 Equipment-specific animation triggers
  - 🔄 Smooth animation blending

- **Equipment System**
  - 🔄 Visible equipped items on character
  - 🔄 Tool/weapon switching with visual feedback
  - 🔄 Armor rendering on player model
  - ⏳ Attachment points for items (hands, back, belt)

### VOIP & Social Features (Planned Foundation)
- **Voice Chat Integration**
  - ⏳ VOIP library integration (Opus codec recommended)
  - ⏳ Voice indicator UI (speaking animation trigger)
  - ⏳ Proximity-based voice chat
  - ⏳ Push-to-talk keybind
  - ⏳ Voice volume controls

- **Emote System**
  - ✅ Basic emote animations created
  - 🔄 Emote command interface (/wave, /point, etc.)
  - ⏳ Emote wheel/radial menu
  - ⏳ Custom emotes support
  - ⏳ Synchronized emote playback in multiplayer

- **Character Expression**
  - ⏳ Talking animation synced with VOIP
  - ⏳ Lip-sync approximation (volume-based)
  - ⏳ Facial expression system (if higher-res models added)
  - ⏳ Hand gestures during speech

### Survival Mode Enhancements
- **Crafting System**
  - 🔄 Enhanced crafting UI using golden frames
  - ⏳ Recipe discovery system
  - ⏳ Crafting stations (workbench, furnace, anvil)
  - ⏳ Tool durability

- **Resource Gathering**
  - 🔄 Block breaking with tools
  - ⏳ Different tool efficiencies
  - ⏳ Block drop system
  - ⏳ Inventory management improvements

---

## 📋 Planned Features

### Short Term (Next 3-6 Months)

#### 1. Combat System
- **Melee Combat**
  - Sword swing hitboxes
  - Shield blocking mechanics
  - Damage calculation
  - Weapon durability
  - Different weapon types (sword, axe, mace)

- **Ranged Combat**
  - Bow and arrow projectiles
  - Arrow physics and collision
  - Crossbow alternative
  - Throwable items

- **Enemy AI**
  - Basic hostile mobs (zombies, skeletons)
  - Passive mobs (animals for resources)
  - Pathfinding
  - Day/night spawn mechanics
  - Boss encounters

#### 2. Enhanced Building
- **Advanced Placement**
  - Rotation of placed blocks
  - Half-blocks and stairs
  - Slope blocks
  - Different block shapes

- **Building Tools**
  - Copy/paste regions
  - Fill tool for large areas
  - Symmetry mode
  - Brush sizes

#### 3. Player Progression
- **Experience System**
  - XP from mining, building, combat
  - Level up system
  - Stat improvements

- **Skills/Abilities**
  - Skill trees (mining, building, combat, magic)
  - Special abilities unlockable
  - Cooldown-based powers
  - Skill UI integration

### Medium Term (6-12 Months)

#### 4. World Expansion
- **More Biomes**
  - Desert, snow, jungle, swamp
  - Biome-specific blocks and resources
  - Weather effects per biome
  - Temperature system

- **Structures**
  - Villages
  - Dungeons
  - Temples and ruins
  - Generated loot

- **Dimensions**
  - Underground world (deeper caves)
  - Sky islands
  - Nether/hell dimension
  - Dimension portals

#### 5. Advanced Multiplayer
- **Guilds/Clans**
  - Guild creation and management
  - Guild territory/claims
  - Guild bank/shared storage
  - Guild chat channel

- **Economy**
  - Trading system
  - Currency
  - Shops (NPC and player-run)
  - Auction house

- **PvP Systems**
  - Arena mode
  - Territorial control
  - Faction wars
  - Ranked combat

#### 6. Quality of Life
- **UI Improvements**
  - Quest log
  - Achievement system
  - Statistics tracking
  - Settings menu redesign with golden UI

- **Controls**
  - Gamepad support
  - Customizable keybinds
  - Mouse sensitivity options
  - Touchscreen improvements (mobile)

### Long Term (12+ Months)

#### 7. Magic System
- **Spellcasting**
  - Spell book interface
  - Mana system
  - Elemental magic (fire, ice, lightning)
  - Spell crafting

- **Enchanting**
  - Enchantment table
  - Item enchantments
  - Magical effects
  - Rune system

#### 8. Farming & Automation
- **Agriculture**
  - Crop planting and growth
  - Animal husbandry
  - Food cooking system
  - Hunger/saturation mechanics

- **Redstone/Logic**
  - Logic blocks (AND, OR, NOT gates)
  - Pistons and moving blocks
  - Automated doors and traps
  - Minecarts and rails

#### 9. Companions & Pets
- **Pet System**
  - Tameable animals
  - Pet commands (follow, stay, attack)
  - Pet inventory
  - Pet leveling

- **Summons**
  - Magic summons
  - Temporary companions
  - Necromancy (skeleton warriors)

#### 10. Story & Quests
- **Quest System**
  - NPC quest givers
  - Quest chains
  - Story progression
  - Quest rewards

- **Lore & World Building**
  - Readable books and scrolls
  - Environmental storytelling
  - Hidden secrets
  - Achievement lore

---

## 🔧 Technical Improvements

### Performance Optimization (Ongoing)
- **Rendering**
  - ✅ Chunk-based frustum culling implemented
  - 🔄 Occlusion culling for hidden chunks
  - ⏳ Level of detail (LOD) for distant terrain
  - ⏳ Mesh batching optimization
  - ⏳ Texture atlas optimization

- **Networking**
  - 🔄 Bandwidth optimization
  - ⏳ Delta compression for position updates
  - ⏳ Chunk streaming improvements
  - ⏳ Server performance profiling

- **Memory Management**
  - ⏳ Chunk unloading improvements
  - ⏳ Texture memory pooling
  - ⏳ Asset streaming

### Platform Support
- **Current Support**
  - ✅ Windows (full support)
  - ✅ Linux (build support, limited runtime)
  - ⏳ macOS (untested, likely works)

- **Future Targets**
  - ⏳ Mobile (Android/iOS) - major undertaking
  - ⏳ Web (WebGL/WebAssembly)
  - ⏳ Console (if feasible)

### Code Quality
- **Testing**
  - ✅ Unit test framework (NUnit) set up
  - 🔄 Expand test coverage
  - ⏳ Integration tests
  - ⏳ Automated UI tests

- **Documentation**
  - ✅ Build documentation (BUILD.md)
  - ✅ Testing guide (TESTING.md)
  - ✅ WoW GUI documentation (WOW_GUI_README.md)
  - ✅ This roadmap
  - 🔄 API documentation
  - ⏳ Modding tutorials
  - ⏳ Video tutorials

---

## 🎨 Art & Audio (Future)

### Visual Enhancements
- **Graphics**
  - ⏳ Shader effects (water, glass, reflections)
  - ⏳ Particle effects (smoke, fire, magic)
  - ⏳ Weather effects (rain, snow, fog)
  - ⏳ Dynamic shadows
  - ⏳ Post-processing (bloom, SSAO)

- **Models**
  - ✅ Enhanced player model with more animations
  - ⏳ Higher resolution player models (optional)
  - ⏳ More mob models
  - ⏳ Animated decorative blocks

### Audio System
- **Music**
  - ⏳ Background music system
  - ⏳ Biome-specific music
  - ⏳ Combat music
  - ⏳ Menu/ambient tracks

- **Sound Effects**
  - ⏳ Block breaking sounds
  - ⏳ Footstep sounds (different surfaces)
  - ⏳ Combat sounds
  - ⏳ Environmental ambience
  - ⏳ UI sound effects

- **Voice**
  - ✅ VOIP system (planned)
  - ⏳ NPC dialogue (text-to-speech or recordings)
  - ⏳ Player voice commands

---

## 🤝 Community & Contribution

### Open Source Development
- **Current State**
  - ✅ Public domain license (Unlicense)
  - ✅ GitHub repository open
  - ✅ Fork-friendly for game creation
  - ⏳ Contribution guidelines

- **Community Features** (Future)
  - ⏳ Discord server
  - ⏳ Official forums
  - ⏳ Mod repository/workshop
  - ⏳ Community showcase
  - ⏳ Regular development updates

### Modding Support
- **Current**
  - ✅ Server-side C# mods
  - ✅ JavaScript mod support
  - ✅ Custom texture packs
  - ✅ Custom models

- **Planned**
  - ⏳ Mod loader/manager
  - ⏳ Client-side mods
  - ⏳ Mod configuration UI
  - ⏳ Mod compatibility checking
  - ⏳ Hot-reload for development

---

## 📊 Priority Matrix

### Critical (Must Have)
1. ✅ Core building mechanics
2. ✅ Multiplayer functionality
3. 🔄 Combat system (in progress)
4. 🔄 Enhanced animations (in progress)
5. ⏳ Survival mode completion
6. ⏳ Performance optimization

### High Priority (Should Have)
1. ⏳ Enemy mobs and AI
2. ⏳ Crafting system improvements
3. ⏳ VOIP integration
4. ⏳ Quest system
5. ⏳ More biomes
6. ⏳ Achievement system

### Medium Priority (Nice to Have)
1. ⏳ Magic system
2. ⏳ Farming and automation
3. ⏳ Guild/clan system
4. ⏳ Pet system
5. ⏳ Weather effects
6. ⏳ Audio system

### Low Priority (Future Enhancement)
1. ⏳ Advanced shaders
2. ⏳ Mobile port
3. ⏳ Story mode
4. ⏳ Additional dimensions
5. ⏳ Console support

---

## 📈 Development Methodology

### Version Scheme
- **Major.Minor.Patch** (e.g., 1.2.3)
  - **Major:** Significant feature additions or breaking changes
  - **Minor:** New features, non-breaking improvements
  - **Patch:** Bug fixes, small tweaks

### Release Cycle
- **Development Builds:** Continuous (main branch)
- **Beta Releases:** Monthly (feature complete but testing)
- **Stable Releases:** Quarterly (fully tested)

### Testing Philosophy
1. Unit tests for core systems
2. Integration tests for multiplayer
3. Manual playtesting for gameplay
4. Community beta testing before stable release

---

## 💡 Optimization Suggestions

### Current Optimizations Implemented
1. ✅ Chunk-based rendering (only visible chunks drawn)
2. ✅ Frustum culling (objects outside camera view ignored)
3. ✅ Texture atlasing for blocks
4. ✅ VBO/VAO for efficient mesh rendering
5. ✅ Delta time for frame-independent animation

### Recommended Future Optimizations

#### Rendering
1. **Greedy Meshing**
   - Combine adjacent faces of same block type
   - Reduces polygon count by 50-80%
   - Priority: High

2. **Occlusion Culling**
   - Don't render chunks completely hidden by others
   - Use bounding box tests
   - Priority: High

3. **Level of Detail (LOD)**
   - Distant chunks use simplified meshes
   - Reduce detail beyond certain distance
   - Priority: Medium

4. **Instanced Rendering**
   - For repeated models (trees, grass, items)
   - Single draw call for many objects
   - Priority: Medium

#### Networking
1. **Chunk Compression**
   - Compress chunk data before sending
   - Use run-length encoding for sparse data
   - Priority: High

2. **Delta Updates**
   - Only send changed blocks, not entire chunks
   - Track dirty chunks
   - Priority: High

3. **Entity Interpolation**
   - Smooth player movement between updates
   - Reduce network update frequency
   - Priority: Medium

4. **Interest Management**
   - Only send updates for nearby entities/chunks
   - Reduce bandwidth per player
   - Priority: High

#### Memory
1. **Texture Streaming**
   - Load high-res textures on demand
   - Keep low-res in memory
   - Priority: Low (mobile priority: High)

2. **Chunk Pooling**
   - Reuse chunk objects instead of GC
   - Reduces memory allocation overhead
   - Priority: Medium

3. **Asset Bundling**
   - Bundle related assets together
   - Reduce file I/O operations
   - Priority: Low

#### Gameplay
1. **AI Optimization**
   - Limit pathfinding updates
   - Use spatial partitioning for entity queries
   - Sleep distant entities
   - Priority: High (when mobs added)

2. **Physics Optimization**
   - Spatial hashing for collision detection
   - Sleep static entities
   - Reduce physics update frequency for distant entities
   - Priority: Medium

---

## 🐛 Known Issues & Limitations

### Current Known Issues
1. **Linux/Mono Networking**
   - ENet native library compatibility issues
   - Workaround: Use Windows for server hosting
   - Priority: Medium

2. **GUI Scaling**
   - Some UI elements may not scale perfectly on all resolutions
   - Mostly affects ultra-wide monitors
   - Priority: Low

3. **Build Warnings**
   - ~107 compiler warnings (unused variables, obsolete APIs)
   - Don't affect functionality
   - Priority: Low (cleanup)

### Limitations
1. **World Height:** Fixed at 128 blocks (expandable with code changes)
2. **World Size:** 9984×9984 default (customizable but limited by memory)
3. **Max Players:** Server-dependent (typically 50-100)
4. **Mobile:** Not currently supported (major undertaking)

---

## 📞 Getting Involved

### For Players
- Download and play the game
- Report bugs on GitHub Issues
- Share your creations
- Join the community (when established)

### For Developers
- Fork the repository for your own game
- Contribute bug fixes and features
- Create mods and share them
- Improve documentation
- Write tests

### For Content Creators
- Create texture packs
- Design player models
- Build showcase worlds
- Make tutorial videos
- Stream gameplay

---

## 📚 Additional Resources

### Documentation
- [README.md](README.md) - Project overview
- [BUILD.md](BUILD.md) - Build instructions
- [TESTING.md](TESTING.md) - Testing guide
- [WOW_GUI_README.md](WOW_GUI_README.md) - UI system documentation
- [COPYING.md](COPYING.md) - License information

### External Resources
- **Cito Language:** [cito.sourceforge.net](http://cito.sourceforge.net/)
- **Original Manic Digger:** [github.com/manicdigger/manicdigger](https://github.com/manicdigger/manicdigger)

---

## 🎯 2025 Goals

### Q1 2025 (Jan-Mar)
- ✅ Enhanced character animations
- ✅ Comprehensive roadmap
- 🔄 VOIP system design and foundation
- ⏳ Combat system alpha

### Q2 2025 (Apr-Jun)
- ⏳ Enemy mobs implementation
- ⏳ Basic crafting expansion
- ⏳ Performance optimization pass
- ⏳ Achievement system

### Q3 2025 (Jul-Sep)
- ⏳ Magic system foundation
- ⏳ More biomes
- ⏳ Quest system alpha
- ⏳ Guild system

### Q4 2025 (Oct-Dec)
- ⏳ Survival mode 1.0
- ⏳ Audio system implementation
- ⏳ Community features
- ⏳ Major stable release

---

## 🏆 Long-term Vision (2026+)

By the end of 2026, Sword & Stone aims to be:
- A feature-complete voxel adventure game
- A thriving multiplayer community
- A platform for creative builders and modders
- An example of successful open-source game development
- A game that players can truly call their own

**The journey continues...**

---

*Legend:*
- ✅ Completed
- 🔄 In Progress
- ⏳ Planned/Not Started

---

**This roadmap is a living document and will be updated regularly as development progresses.**
