# Combat System Implementation

## Overview

This document describes the basic melee combat system implementation for Sword & Stone, including weapon items, damage calculation, attack cooldown, and shield blocking mechanics.

## Status: Phase 1 Complete ✅

### Implemented Features

1. **Weapon Items** ✅
   - Stone Sword (damage: 4)
   - Iron Sword (damage: 6)
   - Diamond Sword (damage: 8)
   - Wooden Shield (damage reduction)
   - Iron Shield (better reduction)
   - Bow (placeholder for future ranged combat)

2. **Combat System Module** ✅
   - Attack cooldown (0.5 seconds between attacks)
   - Melee range checking (3 block radius)
   - Weapon damage calculation
   - Shield blocking mechanic (50% damage reduction)
   - Player combat state tracking
   - Kill/death event handling

3. **Infrastructure Integration** ✅
   - Uses existing health system API
   - Automatic mod loading through Fortress game mode
   - Server-side validation and processing

## Architecture

### File Structure

```
ManicDiggerLib/Server/Mods/Fortress/
├── Weapons.cs         - Weapon and shield item definitions
├── CombatSystem.cs    - Combat logic and state management
├── Tools.cs           - Existing tool definitions (pickaxe, axe, shovel)
└── Core.cs            - Core game mod
```

### Combat Flow

```
1. Player clicks on entity (client)
   ↓
2. Client sends HitEntity packet
   ↓
3. Server receives EntityInteraction packet
   ↓
4. Server calls OnEntityHit event handlers
   ↓
5. CombatSystem.ApplyMeleeDamage() processes attack
   ↓
6. Damage calculated and applied to target
   ↓
7. Health updated and broadcasted to clients
```

## Technical Details

### Weapon Damage Values

| Weapon | Damage | Notes |
|--------|--------|-------|
| Fist (no weapon) | 1 | Base punch damage |
| Stone Sword | 4 | Basic melee weapon |
| Iron Sword | 6 | Mid-tier weapon |
| Diamond Sword | 8 | High-tier weapon |

### Shield Protection

| Shield | Damage Reduction |
|--------|------------------|
| Wooden Shield | 50% |
| Iron Shield | 50% (future: better) |

### Combat Parameters

- **Attack Cooldown**: 0.5 seconds
- **Melee Range**: 3 blocks
- **Minimum Damage**: 1 (even when blocked)

### Server-Side Combat State

The `CombatSystem` mod tracks:
- `lastAttackTime` - Dictionary mapping player ID to last attack timestamp
- `playerBlocking` - Dictionary tracking which players are blocking with shields

## Code Examples

### Creating a New Weapon

```csharp
m.SetBlockType("IronSword", new BlockType()
{
    Name = "Iron Sword",
    IsTool = true,
    ToolType = ToolType.Sword,
    Strength = 6,  // Damage value
    TextureIdForInventory = "IronSword",
    AllTextures = "IronSword",
});
```

### Applying Melee Damage

```csharp
public void ApplyMeleeDamage(int attackerId, int targetId)
{
    // Cooldown check
    if (!CanAttack(attackerId)) return;
    
    // Range check
    if (!IsInMeleeRange(attackerId, targetId)) return;
    
    // Calculate damage with shield reduction
    int weaponDamage = 4;  // Get from player's equipped weapon
    float shieldReduction = GetShieldDamageReduction(targetId);
    int finalDamage = (int)(weaponDamage * (1.0f - shieldReduction));
    
    // Apply damage
    int newHealth = m.GetPlayerHealth(targetId) - finalDamage;
    m.SetPlayerHealth(targetId, newHealth, m.GetPlayerMaxHealth(targetId));
    
    // Update cooldown
    lastAttackTime[attackerId] = DateTime.UtcNow;
}
```

## Current Limitations

### Not Yet Implemented

1. **Entity Hit Event Integration**
   - CombatSystem needs to register `OnEntityHit` handler
   - Need to wire up packet handling to call `ApplyMeleeDamage()`
   - Currently only has framework in place

2. **Active Weapon Detection**
   - System assumes stone sword (damage 4)
   - Need to query player's active inventory slot
   - TODO: Integrate with inventory system

3. **Shield Blocking Activation**
   - `SetPlayerBlocking()` method exists but not called
   - Need client-side input (right-click to block)
   - No packet for blocking state yet

4. **Combat Animations**
   - Sword attack animation exists but not triggered
   - Need to send animation packet on hit
   - Shield block animation not implemented

5. **Visual/Audio Feedback**
   - No hit particles
   - No combat sound effects
   - No damage numbers

6. **Monster Combat**
   - Only player-vs-player framework
   - Monster health system needs integration
   - Monster AI needs combat behavior

## Next Implementation Steps

### Priority 1: Wire Up Entity Hits

```csharp
public void Start(ModManager manager)
{
    m = manager;
    m.RegisterOnPlayerJoin(OnPlayerJoin);
    m.RegisterOnPlayerLeave(OnPlayerLeave);
    m.RegisterOnEntityHit(OnEntityHit);  // <-- Add this
}

void OnEntityHit(int playerId, ServerEntityId entityId)
{
    // Convert entity ID to player ID (if applicable)
    // Call ApplyMeleeDamage()
}
```

### Priority 2: Get Active Weapon

```csharp
int GetPlayerWeaponDamage(int playerId)
{
    // Get active material slot
    // Query inventory for item in that slot
    // Return BlockType.Strength or BASE_PUNCH_DAMAGE
}
```

### Priority 3: Trigger Combat Animations

```csharp
void TriggerAttackAnimation(int playerId)
{
    // Send packet to play "sword_attack" animation
    // Duration based on attack cooldown
}
```

### Priority 4: Add Combat Feedback

```csharp
void OnPlayerHit(int attackerId, int targetId, int damage)
{
    // Spawn hit particles at target position
    // Play hit sound effect
    // Show damage number (client-side mod)
}
```

## Testing Procedures

### Manual Testing Steps

1. **Start Server**
   ```
   cd ManicDiggerServer/bin/Release
   ./ManicDiggerServer.exe
   ```

2. **Start Client and Connect**
   - Open ManicDigger client
   - Connect to localhost

3. **Check Inventory**
   - Player should spawn with Stone Sword in inventory
   - Check creative inventory for all weapons

4. **Test Weapon Equipping**
   - Select sword from inventory
   - Verify it appears in hand

5. **Test Attack (Once Integrated)**
   - Target another player or entity
   - Click to attack
   - Verify damage is applied
   - Check cooldown prevents spam

### Expected Behavior

- ✅ Weapons appear in creative inventory
- ✅ Weapons added to starting inventory
- ⏳ Clicking entity sends HitEntity packet
- ⏳ Server applies damage based on weapon
- ⏳ Health bar updates on damage
- ⏳ Attack cooldown prevents rapid spam
- ⏳ Combat message shows in chat

## Configuration

### Adjusting Combat Balance

Edit `ManicDiggerLib/Server/Mods/Fortress/CombatSystem.cs`:

```csharp
// Attack speed
private const double ATTACK_COOLDOWN_SECONDS = 0.5;  // Increase for slower attacks

// Attack range
private const float MELEE_RANGE = 3.0f;  // Increase for longer reach

// Base damage
private const int BASE_PUNCH_DAMAGE = 1;  // Fist damage
```

Edit `ManicDiggerLib/Server/Mods/Fortress/Weapons.cs`:

```csharp
// Sword damage
Strength = 4,  // Increase for more damage

// Shield blocking
float shieldReduction = 0.5f;  // 0.5 = 50% damage reduction
```

## Dependencies

### Required Mods
- `Core` - Core game functionality
- `CoreBlocks` - Block definitions
- `Tools` - Tool system (for ToolType enum)

### Required APIs
- `ModManager.GetPlayerHealth()`
- `ModManager.SetPlayerHealth()`
- `ModManager.GetPlayerPosition*()`
- `ModManager.GetPlayerName()`
- `ModManager.SendMessage()`
- `ModManager.RegisterOnPlayerJoin()`
- `ModManager.RegisterOnPlayerLeave()`
- `ModManager.RegisterOnEntityHit()` (to be added)

## Future Enhancements

### Phase 2: Advanced Combat

1. **Critical Hits**
   - Random chance for 2x damage
   - Based on weapon type or player stats

2. **Weapon Durability**
   - Tools.cs already has `Strength` field
   - Decrement on use
   - Break when durability reaches 0

3. **Combat Combos**
   - Chain attacks for bonus damage
   - Reset on missed attack or cooldown expiry

4. **Backstab Mechanic**
   - Bonus damage when attacking from behind
   - Check player orientation relative to attacker

### Phase 3: Advanced Defense

1. **Shield Bash**
   - Active ability when blocking
   - Knockback effect on attacker
   - Stun for 1 second

2. **Parrying**
   - Timing-based block for 100% mitigation
   - Counter-attack opportunity

3. **Dodge Roll**
   - Movement ability to avoid damage
   - Short cooldown

### Phase 4: Ranged Combat

1. **Bow Mechanics**
   - Draw and release system
   - Arrow trajectory physics
   - Damage based on draw time

2. **Arrow Types**
   - Normal arrows
   - Fire arrows (burn effect)
   - Poison arrows (damage over time)

3. **Crossbow**
   - Higher damage, slower reload
   - No charging required

### Phase 5: Magic Combat

1. **Spell System**
   - Mana resource
   - Cast time and cooldowns
   - Area of effect spells

2. **Elemental Damage**
   - Fire, ice, lightning
   - Status effects (burn, freeze, stun)

## Performance Considerations

### Server Load

- Combat state stored per-player (lightweight)
- Damage calculations are simple math
- No continuous loops or heavy processing
- Event-driven architecture (only processes on hits)

### Network Traffic

- Minimal additional packets
- Leverages existing EntityInteraction packet
- Health updates already implemented

### Scalability

- System scales linearly with player count
- Dictionary lookups are O(1)
- No global state that grows unbounded

## Security

### Anti-Cheat Measures

1. **Server-Side Validation**
   - All damage calculations on server
   - Client cannot fake damage values

2. **Range Checking**
   - Server validates distance before applying damage
   - Prevents teleport hacks from working

3. **Cooldown Enforcement**
   - Server tracks last attack time
   - Client cannot bypass attack speed

4. **Health Integrity**
   - Health changes only via server API
   - No direct client control

## Known Issues

None at this time - system not yet integrated for testing.

## Changelog

### Version 1.0 - December 14, 2025
- Initial implementation
- Added Weapons.cs with 6 weapon types
- Added CombatSystem.cs with core combat logic
- Integrated with Fortress game mode
- Ready for entity hit event integration

---

## Contributing

When extending the combat system:

1. Follow existing naming conventions
2. Add server-side validation for new mechanics
3. Document new features in this file
4. Test thoroughly in multiplayer
5. Balance new weapons against existing ones
6. Consider performance impact

## Support

For questions or issues with the combat system:
- Check this documentation first
- Review code comments in CombatSystem.cs
- Test with debug logging enabled
- Report bugs with reproduction steps

---

**Implementation Status**: Phase 1 Complete (Framework Ready)  
**Next Milestone**: Entity hit integration and testing  
**Last Updated**: December 14, 2025
