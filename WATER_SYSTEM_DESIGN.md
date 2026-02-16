# Sword & Stone - Water System Design

**Last Updated:** February 2026  
**Status:** Design Phase  
**Goal:** Replace per-voxel water rendering with a unified water system supporting smooth surfaces, boats, swimming, ice, and seasonal freezing.

---

## 🌊 Overview

The water system is split into three decoupled layers:

1. **Water Simulation** — Tile-based physics (cheap, deterministic)
2. **Water Mesh** — Chunk-based smooth surface rendering
3. **Water Interactions** — Boats, swimming, ice physics

This separation allows the simulation to remain simple while the visuals look smooth and continuous.

---

## 1️⃣ Water Simulation (Physics Layer)

### Data Model

Each water voxel stores level and flow data, not just "is water":

```
WaterCell:
    level: float (0.0–1.0, fraction of block filled)
    flowX: float (horizontal flow direction)
    flowZ: float (horizontal flow direction)
    isSource: bool
    frozen: bool
```

### Update Rules (Dirty-Queue Driven)

**Performance key:** Only update cells that changed. Maintain a dirty queue.

1. **Gravity first** — Water flows down if block below has space
2. **Lateral equalization** — Compare neighbors, flow toward lower levels (max delta = 1 per tick)
3. **Source regeneration** — Cell surrounded by ≥2 source neighbors becomes a source

### River Flow

Rivers follow downhill paths from rainfall basins to oceans:

1. **Rainfall map** — Perlin noise determines source locations
2. **Flow field** — Steepest-descent tracing from sources to sea level
3. **River carving** — Accumulated flow widens and deepens rivers
4. **Erosion** — River speed × slope × soil resistance = terrain loss

### Ocean Generation

- Sea level constant (e.g., block height 64)
- Everything below sea level fills with water
- Gentle seabed noise for underwater terrain
- Coastlines define biome transitions

---

## 2️⃣ Water Mesh (Rendering Layer)

### Core Principle
> Never render water as solid cubes. Render a smooth surface mesh per chunk.

### Mesh Generation Rules

- **One mesh per chunk** — Scan topmost water cells, generate continuous surface
- **Shared vertices** — Adjacent tiles share edge vertices for smooth interpolation
- **Height smoothing** — Average water level at each vertex from 4 neighboring cells
- **Chunk-edge stitching** — Vertices at chunk borders sample from neighboring chunks

### LOD System

| Distance | Detail |
|----------|--------|
| Near (<40m) | Full mesh (chunkSize vertices) |
| Mid (40–120m) | Reduced mesh (half resolution) |
| Far (>120m) | Single quad per chunk |

The shader animates waves at all LOD levels.

### Water Shader

**Vertex shader features:**
- Subtle sine/cosine wave displacement on Y axis
- UV scrolling for flow animation

**Fragment shader features:**
- Fresnel reflection (edge highlighting)
- Depth-based color blending (shallow → deep)
- Ice overlay blend based on temperature
- Flow normal maps for rivers

```
// Pseudocode
waterColor = mix(shallowColor, deepColor, depth)
waterColor += fresnel * 0.15
freeze = clamp(-temperature * 0.1, 0.0, 1.0)
finalColor = mix(waterColor, iceColor, freeze)
alpha = 0.85
```

---

## 3️⃣ Water Interactions

### Swimming

- **Wading** — Water below waist: slight speed reduction, splash effects
- **Swimming** — Water above chest: switch to swim movement mode
  - Reduced gravity (buoyancy)
  - Horizontal drag
  - Vertical movement via input
  - Stamina drain in cold water

### Boat Physics

- **Buoyancy** — Sample water height at boat position, apply upward force proportional to submersion
- **Steering** — Throttle applies force along forward vector, steer rotates yaw
- **River current** — Water flow forces push boats downstream
- **Ice interaction** — Frozen water stops boats; heavy boats break thin ice

### River Current Forces

Water flow affects all entities in contact:

```
flowForce = cell.flow * cell.speed * depthFactor
entity.velocity += flowForce * deltaTime
```

- Shallow water barely moves entities
- Deep rivers carry entities downstream
- Depth factor = submersion / maxDepth (clamped 0–1)

### Animal Swimming & Drowning

| Swim Ability | Effect |
|-------------|--------|
| None | Stamina drains rapidly, drowns when empty |
| Weak | Can swim short distances, stamina drains ×2 |
| Strong | Comfortable swimmer, stamina drains ×0.5 |

- Cold water (winter) drains stamina faster
- Frozen rivers become migration corridors

---

## ❄️ Ice System

### Freeze/Melt Logic

- Temperature below 0°C → ice thickness grows
- Temperature above 0°C → ice thickness shrinks
- Thickness determines walkability and weight capacity

### Weight Load

```
capacity = iceThickness * 1200
if (loadKg > capacity) → crack/break-through
```

- Thin ice cracks audibly (warning)
- Heavy carts/boats break through
- Players on thin ice risk falling in

### Seasonal Integration

- **Winter** — Rivers and lakes freeze, become walkable paths
- **Spring** — Ice melts, rivers swell with meltwater
- **Frozen rivers** = natural roads for caravans and migration

---

## 🔗 System Integration

### Tick Order (Server)

```
1. WorldCalendar.Advance()
2. Weather update
3. Water simulation (dirty queue)
4. Ice freeze/melt
5. River current update
6. Entity water interactions
7. Snapshot to clients
```

### Client Responsibilities
- Visual effects (waves, reflections, particles)
- Sound (splash, ice crack, flowing water)
- Fog (underwater depth fog)
- Shader parameters

### Server Responsibilities
- Water level simulation
- Ice state
- Flow field
- Buoyancy calculations
- Entity water interactions

---

## 🚀 Implementation Phases

### Phase 1 — Foundation
- [ ] Dirty-queue water solver
- [ ] Water levels (0–15 or 0.0–1.0)
- [ ] Gravity + lateral flow rules

### Phase 2 — Rendering
- [ ] Surface-only water mesh per chunk
- [ ] Animated water shader
- [ ] Smooth vertex height interpolation

### Phase 3 — Interactions
- [ ] Swimming movement mode
- [ ] Boat buoyancy controller
- [ ] River current forces on entities

### Phase 4 — Seasonal
- [ ] Ice freeze/melt system
- [ ] Seasonal water level changes
- [ ] Ice weight/cracking mechanics

### Phase 5 — Polish
- [ ] LOD water meshes for distance
- [ ] Underwater fog effect
- [ ] Ice-cracking sound/visual feedback

---

## 📚 Related Documents

- [GAME_DESIGN.md](GAME_DESIGN.md) — Core design vision
- [WORLD_SIMULATION_DESIGN.md](WORLD_SIMULATION_DESIGN.md) — Seasons, weather, and world systems
- [ROADMAP.md](ROADMAP.md) — Development timeline

---

*This document captures the water system design distilled from project brainstorming. It is a living document that will evolve as implementation progresses.*
