# Sword & Stone - Game Design Document

**Last Updated:** February 2026  
**Status:** Design Phase  
**Inspiration:** Vintage Story (simulation depth) + Hytale (character feel & art style) + Valheim (character persistence)

---

## 🎯 Core Vision

> *"A grounded, tactile survival world where the character—not the world—is the constant."*

Sword & Stone aims to combine deep survival simulation with expressive character movement and persistent progression across worlds. The world should feel alive through interconnected systems — not scripted events.

---

## 🧍 Character System (Persistent Across Worlds)

### Key Design Rule
- **Worlds are disposable. Characters are sacred.**
- Characters exist independently of any world
- Inventory, skills, injuries, and appearance travel with the character

### Data Separation

```
/characters/
   player01.char
   player02.char

/worlds/
   world_seed_81273/
   world_seed_99120/
```

**Character owns:** Inventory, Equipment, Skills, Health/Status, Appearance, Progression  
**World owns:** Terrain, Structures, NPCs, Containers, Time/Weather, Local Quests

### Character Creation (Vintage Story DNA)

**UI Style:**
- Earth tones, linen textures, subtle grain/noise
- Handcrafted feel — no neon, no gradients
- Discrete presets, not sliders

**Creation Flow:**
1. **Physical Identity** — Body type, height, gender expression
2. **Face & Hair** — Preset sculpts, beards (affect warmth), hair length (affects helmet compatibility)
3. **Origin Background** — Soft skill biases (not classes)
   - Woodsman → faster axe learning
   - Potter → clay recipes unlocked
   - Nomad → hunger drains slower

### Movement & Feel (Hytale-Inspired)

**Character Shape:**
- Slightly oversized hands/feet for clear silhouettes
- Large head for readable yaw direction
- Torso twist independent of legs
- Capsule collision (not AABB)

**Movement Physics:**
- Acceleration-based, not velocity snapping
- Turn inertia for natural-feeling rotation
- Head tracks camera yaw/pitch
- Torso lags behind yaw for expressiveness

**Movement Modes:**
- Ground, Swimming, Wading, Climbing, Falling
- Explicit state machine with proper transitions

---

## 🎒 Inventory & Weight (Valheim-Style)

### Core Rules
- Weight-based system, not pure slot-based
- Inventory travels with character between worlds
- World containers do NOT transfer

### Carry Rules
- ✅ Tools, armor, food — allowed cross-world
- ❌ Quest items — world-bound
- ❌ Heavy machines — world-bound

### Soft Exploit Control
- Portals apply fatigue
- Encumbrance slows world entry
- Magical items decay cross-world
- World difficulty scales subtly

---

## 🧠 Learn-By-Doing Skill System

### Core Philosophy
- Skills improve by usage, not XP allocation
- Failure teaches faster than success (1.5× gain)
- No XP bars on screen — skills improve quietly
- Slow mastery: early tools feel weak, mastery feels earned

### Skill Progression
| Action | Skill |
|--------|-------|
| Chopping wood | Woodworking |
| Missing a sword swing | Combat |
| Cooking food | Cooking |
| Surviving starvation | Endurance |

### Farming Quality Tie-In
- Skilled farmers produce higher quality/yield
- Tool tier multiplies farming speed
- Climate sensitivity affects output

---

## ⚔️ Combat & Injury Persistence

### Death Is Not a Reset
- Inventory drops into a world-bound corpse container
- Skills are reduced softly (×0.97)
- Injuries persist after respawn
- Corpse must be retrieved

### Injury System
| Injury | Effect |
|--------|--------|
| Broken Leg | Movement speed ×0.6 |
| Concussion | Stamina regen ×0.5 |
| Infection | Health decay over time |
| Deep Cut | Reduced combat effectiveness |

### Crafting Quality
- Crafted item quality depends on skill level (0.5–1.5 range)
- Higher quality = better durability
- No RNG dice rolls — deterministic skill-based output

---

## 🔧 Tool & Equipment Tiers

| Tier | Material | Efficiency |
|------|----------|------------|
| 1 | Stone | 0.6× |
| 2 | Copper | 0.8× |
| 3 | Bronze | 1.0× |
| 4 | Iron | 1.2× |
| 5 | Steel | 1.4× |

Tool progression drives era advancement (Stone Age → Bronze Age → Iron Age → Steel Age).

---

## 🎨 Visual Style Target

### Direction: "Vintage Story + Hytale Lite"

| Element | Target |
|---------|--------|
| Water | Smooth surface, stylized, readable |
| Lighting | Soft, directional, smooth |
| Fog | Distance-based, sky-tinted |
| Colors | Earthy, desaturated |
| Terrain | Slight randomness, AO depth |
| Vibe | Cozy survival, not plastic |

### Avoid
- ❌ Hyper-real PBR
- ❌ Harsh outlines
- ❌ Flat color blocks

### Aim For
- ✅ Warm light
- ✅ Depth cues
- ✅ Motion everywhere

### Terrain Rendering Goals
- Triplanar mapping (no UV seams on cliffs)
- Material blending driven by soil composition
- Moisture darkening
- Seasonal tinting
- Macro noise overlay (kills repetition)

---

## 🏗️ Server-Authoritative Design

### Golden Rule
> Client never sends character state — only intents.

**Client MAY send:** Input actions, craft requests, equip requests, world travel requests  
**Client MAY NOT send:** Inventory contents, skill values, health values, injuries

### Character Validation
- Characters saved server-side only
- Skills never client-authored
- Crafting quality deterministic
- Any client/server mismatch → server overwrites client

---

## 🎯 Design Principles (Hard Rules)

1. **Characters persist, worlds do not**
2. **Visuals favor warmth over sharpness**
3. **Systems must feel physical**
4. **UI never breaks immersion**
5. **Modding is assumed, not optional**
6. **No scripted story events** — narrative emerges from simulation
7. **Death has permanent consequences**
8. **Worlds never own character data**

---

## 📚 Related Documents

- [ROADMAP.md](ROADMAP.md) — Development timeline and priorities
- [WATER_SYSTEM_DESIGN.md](WATER_SYSTEM_DESIGN.md) — Water physics, rendering, boats, and ice
- [WORLD_SIMULATION_DESIGN.md](WORLD_SIMULATION_DESIGN.md) — Seasons, weather, crops, NPCs, trade, and story
- [COMBAT_SYSTEM_IMPLEMENTATION.md](COMBAT_SYSTEM_IMPLEMENTATION.md) — Combat mechanics (Phase 1)
- [CHARACTER_CUSTOMIZATION.md](CHARACTER_CUSTOMIZATION.md) — Current character system

---

*This document captures the design vision distilled from project brainstorming. It is a living document that will evolve as development progresses.*
