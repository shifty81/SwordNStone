# Sword & Stone - World Simulation Design

**Last Updated:** February 2026  
**Status:** Design Phase  
**Goal:** Create interconnected world systems (seasons, weather, crops, soil, NPCs, trade, story) that produce emergent narrative through simulation rather than scripting.

---

## 🌍 Design Philosophy

> *"The world is a simulation first. Visuals and story are projections of that simulation."*

Every visible change in the world should trace back to a simulated cause. Weather affects crops. Crops affect villages. Villages affect trade. Trade affects history. History becomes myth.

---

## 1️⃣ World Calendar & Seasons

### Calendar Structure

```
TICKS_PER_DAY = 24000
DAYS_PER_YEAR = 96
DAYS_PER_SEASON = 24
```

### Season Effects

| Season | Temp Bias | Rainfall | Crops | Snow |
|--------|-----------|----------|-------|------|
| Spring | +0 → +10 | High | Planting | Melting |
| Summer | +10 → +25 | Medium | Growth | None |
| Autumn | +5 → -5 | Medium | Harvest | None |
| Winter | -10 → -30 | Snow | Dormant | Accumulation |

### Temperature Model

Temperature is a function of:
- Season
- Latitude
- Elevation
- Ocean proximity

This is **not global** — different areas have different weather at the same time.

---

## 2️⃣ Weather System

### Weather Cells (Chunk-Based)

Weather is spatial and local, not a global toggle.

```
WeatherCell:
    temperature: float (°C)
    rainfall: float (0–1)
    wind: float (0–1)
    cloud: float (0–1)
```

Resolution: 1 cell per 8–16 chunks, updated slowly (minutes, not frames).

### Weather Types (Derived, Not Stored)

| Condition | Result |
|-----------|--------|
| Low cloud | Clear |
| High cloud + warm | Rain |
| High cloud + cold | Snow |
| Very high cloud | Storm |
| High humidity + low wind | Fog |

### Gameplay Effects

| Weather | Effect |
|---------|--------|
| Rain | Soil hydration ↑, reduced visibility |
| Snow | Movement slow, water freezes |
| Storm | Crop damage, vision loss |
| Fog | Reduced view distance |
| Winter | Hunger drain ↑ |

---

## 3️⃣ Soil & Terrain Materials

### Soil Types

| Material | Fertility | Drainage | Erosion Resistance |
|----------|-----------|----------|--------------------|
| Soil | 0.6 | 0.5 | 0.4 |
| Loam | 0.9 | 0.7 | 0.6 |
| Clay | 0.5 | 0.2 | 0.8 |
| Peat | 0.7 | 0.9 | 0.2 |

### Soil drives gameplay
- **Loam** → best farming → population growth
- **Clay** → pottery → trade specialization
- **Peat** → fuel → winter survival
- **Soil depletion** → migration → conflict

### Erosion

```
erosionRate = waterFlow × slope × (1 - soilResistance)
```

- Rivers carve valleys over time
- Floodplains form from sediment deposits
- Spring thaw creates mud, which erodes on slopes
- Player-made trails form from repeated traffic

### Snow → Mud → Erosion Pass

1. Winter: Snow accumulates based on temperature and precipitation
2. Spring: Snow melts into mud when temperature rises above 2°C
3. Mud erodes on slopes during rainfall
4. Long-term erosion permanently lowers terrain height

---

## 4️⃣ Crop Growth & Farming

### Growth Factors

```
growthRate = baseCrop × soilFertility × soilMoisture × temperatureCurve × farmerSkillMultiplier
```

### Seasonal Rules
- Spring → planting season
- Summer → growth season
- Autumn → harvest bonus
- Winter → growth paused completely

### Soil Depletion
- Replanting same crop → fertility drops
- Crop rotation → maintains fertility
- Composting → restores nutrients
- Flooding → fertility boost from sediment

### Farming Tool Tiers
Higher tool tiers increase farming speed (see [GAME_DESIGN.md](GAME_DESIGN.md) for tier table).

---

## 5️⃣ Food Preservation & Storage

### Spoilage System

```
spoilageRate = baseSpoilRate × preservationModifier × environmentModifier × deltaTime
```

| Preservation | Modifier |
|-------------|----------|
| None | 1.0× |
| Dried | 0.4× |
| Smoked | 0.25× |
| Salted | 0.15× |
| Fermented | 0.05× |

### Environmental Factors
- Temperature increases spoilage
- Humidity increases spoilage
- Sealed containers reduce spoilage ×0.5

### Food States

| Freshness | State | Effect |
|-----------|-------|--------|
| >0.7 | Fresh | Full nutrition |
| 0.3–0.7 | Stale | Reduced nutrition |
| <0.3 | Rotten | Risk of illness |
| 0 | Toxic | Guaranteed illness |

### Salt as Strategic Resource
- Salt is required for long-term meat preservation
- Salt deposits are geographically limited → natural trade monopolies
- No salt = no winter food stores = village death

---

## 6️⃣ Livestock & Grazing

### Grazing Pressure
- Livestock consume soil fertility while grazing
- Overgrazing degrades pasture (compaction, reduced moisture)
- Livestock weight increases with adequate grazing, decreases during starvation
- Pasture rotation prevents degradation

### Disease & Parasites
- Overcrowding increases infection spread
- Low hygiene × high density = disease outbreak
- Infected animals lose weight and grazing efficiency
- Rotten food contaminates healthy livestock

---

## 7️⃣ NPC Settlements

### Settlement Types

| Type | Size | Purpose | Failure Mode |
|------|------|---------|--------------|
| Village | 10–60 | Food survival | Winter famine |
| Town | 100–400 | Preservation + tools | Disease & unrest |
| City | 1000+ | Trade & labor control | Siege, collapse |
| Outpost | 5–20 | Resource extraction | Abandonment |

### Settlement Placement (System-Driven)

Settlements are placed based on a scoring function:

```
score = fertility × 5 + moisture × 2 + nearRiver × 4
      - floodRisk × 3 - slope × 2 - distanceToStone × 1.5
```

Rivers birth towns. Clay spawns pottery villages. Loam feeds breadbasket settlements.

### Social Classes & Labor

| Class | Productivity | Food Need |
|-------|-------------|-----------|
| Peasant | 1.0× | Low |
| Artisan | 1.4× | Medium |
| Merchant | 1.2× | Medium |
| Soldier | 0.3× | High |
| Elite | 0.1× | High |

- Food surplus → population growth → class specialization
- Food deficit → demotions, emigration
- Armies cost food — standing military is expensive

### Village Specialization & Tech Drift
- Villages drift toward what they do most (farming, pottery, trade)
- Neglected skills decay over time
- No tech tree menus — history shapes technology

---

## 8️⃣ Trade Routes & Caravans

### Route Generation
- Trade routes follow road networks
- Routes pruned by mountains, snow, river ice
- Routes re-evaluated seasonally

### Caravan Mechanics
- Caravans carry food, salt, and goods
- Speed affected by snow depth and temperature
- Winter can stall or kill caravans
- Road erosion forces reroutes

### Seasonal Trade Patterns
- Summer → boat-dominated logistics
- Winter → frozen rivers become trade corridors
- Spring → flooding disrupts land routes

---

## 9️⃣ Roads & Infrastructure

### Organic Road Formation
- Player/NPC traffic gradually wears paths into terrain
- Cart weight and traffic density deepen ruts
- Mud interaction worsens ruts during rain

### Road Speed Bonus
- Worn paths provide movement speed bonus
- Deep ruts eventually slow traffic (maintenance needed)
- Stone roads become vital for heavy trade

---

## 🏰 Siege Logistics

### Core Mechanics
- Siege = food supply vs. time
- Daily food consumption per population
- Winter multiplies food drain
- Disease escalates when food runs out

### Outcomes (No Arbitrary Timers)
- Short siege → defenders survive
- Winter siege → mass starvation
- No salt → stored food spoils → collapse
- Disease breaks armies before swords do

---

## 🐾 Animal Migration

### Seasonal Migration
- Animals migrate based on temperature vs. species tolerance
- Cold-intolerant species move south in winter
- Heat-intolerant species move north in summer
- Migration distance varies by species

### Climate Effects on AI
- Rain/fog reduces AI sight range
- Cold increases AI stamina drain
- Blizzards cause shelter-seeking behavior
- Storms increase predator aggression

---

## 📖 Story & Narrative

### Core Premise: "The Long White"

The world is recovering from a long climate collapse. Winters grew longer, rivers froze unpredictably, trade routes died, cities starved, knowledge fragmented.

The player is not "the chosen one" — they are a persistent survivor whose life threads through many worlds.

### Narrative Layers

1. **World History** — Generated at world creation (years of collapse, salt wars, migration patterns)
2. **Settlement Stories** — Dynamic, driven by simulation (famine, prosperity, disease)
3. **NPC Life Arcs** — Personal, reactive (farmers become herders, merchants turn bandit)
4. **Player Legacy** — Persistent across worlds (skills, reputation echoes)

### NPC Memory System

NPCs remember events from their lifetime and village history:
- "We lost many mouths that winter. I still count them when it snows."
- "The flood took half the fields that year."
- "You came when others turned away. That matters."

Dialogue shifts based on season, settlement stress, and player reputation.

### Belief Systems

Beliefs form from harvest cause-and-effect misattribution:
- Good harvest after ritual → belief strengthens
- Bad harvest after ritual → belief weakens
- Examples: "The River Must Be Fed", "Salt Offends the Earth"
- Beliefs affect NPC trust, farming choices, and resistance to player advice

### Multi-Generation NPC Lineage

- NPC children inherit skills from parents (with decay)
- Knowledge loss during famine (skilled lineages die)
- Dead lineages erase techniques forever
- Player intervention can save or doom family lines

### Myth Generation

When settlements die, myths are generated:
- "They say the river swallowed the old town whole." (Flood death)
- "The soil itself betrayed them." (Famine death)
- "No fire burned bright enough that year." (Winter death)
- Myths spread via traders, mutate over generations, become legends

---

## 🔗 Master System Loop

```
Climate
  → Harvest
    → Preservation (salt bottleneck)
      → Village survival
        → Trade & roads
          → Class structure
            → Military capacity
              → Siege outcome
                → World history
                  → Myths
```

Every year reshapes the world. Nothing is isolated. Nothing is cosmetic.

---

## 🚀 Implementation Phases

### Phase 1 — Calendar & Weather
- [ ] World calendar with season progression
- [ ] Weather cells (temperature, rainfall, wind)
- [ ] Temperature model (season + latitude + elevation)

### Phase 2 — Terrain & Soil
- [ ] Soil type system (soil, loam, clay, peat)
- [ ] Erosion from water flow
- [ ] Snow accumulation and melt

### Phase 3 — Farming
- [ ] Crop growth tied to soil + season + skill
- [ ] Soil fertility depletion and recovery
- [ ] Food spoilage and preservation

### Phase 4 — NPC Settlements
- [ ] Settlement placement scoring
- [ ] Population growth/decline
- [ ] Social class system

### Phase 5 — Trade & Economy
- [ ] Trade route generation
- [ ] Caravan mechanics
- [ ] Salt as strategic resource

### Phase 6 — Story & Memory
- [ ] NPC memory system
- [ ] Procedural dialogue
- [ ] Belief systems
- [ ] Myth generation from extinct settlements

---

## 📚 Related Documents

- [GAME_DESIGN.md](GAME_DESIGN.md) — Core design vision and character systems
- [WATER_SYSTEM_DESIGN.md](WATER_SYSTEM_DESIGN.md) — Water physics and rendering
- [ROADMAP.md](../../ROADMAP.md) — Development timeline
- [COMBAT_SYSTEM_IMPLEMENTATION.md](COMBAT_SYSTEM_IMPLEMENTATION.md) — Combat mechanics

---

*This document captures the world simulation design distilled from project brainstorming. It is a living document that will evolve as implementation progresses.*
