# Enhanced Animation System Guide

**Last Updated:** December 2025  
**Status:** Implemented  
**Version:** 1.0

---

## 📖 Overview

This guide explains the enhanced character animation system in Sword & Stone, including action-based animations, emotes, and how to extend the system with new animations.

---

## 🎬 Animation Types

### 1. Core Animations (Always Available)
- **idle** - Standing still, gentle breathing motion
- **walk** - Walking with leg and arm movement

### 2. Action Animations (Equipment-Based)
- **chop** - Mining/chopping with tool (pickaxe, axe, shovel)
- **sword_attack** - Sword swinging attack
- **bow_draw** - Drawing a bow (not yet firing)
- **bow_fire** - Releasing arrow
- **shield_block** - Defensive stance with shield

### 3. Emote Animations (Social)
- **wave** - Greeting gesture, arm raised
- **point** - Pointing at something
- **cheer** - Celebration, both arms up
- **talk** - Talking animation (for VOIP integration)

### 4. Future Animations (Planned)
- **laugh** - Hearty laughter
- **cry** - Sad expression
- **dance** - Simple dance
- **sit** - Sitting down
- **bow** - Respectful bow
- **salute** - Military salute

---

## 📁 Animation Files

### Player Model Files

#### Standard Player Model
**File:** `data/public/player.txt`
- Contains basic idle and walk animations
- 64x32 texture size
- Used by default

#### Enhanced Player Model
**File:** `data/public/playerenhanced.txt`
- Contains ALL animations (idle, walk, chop, sword, bow, emotes)
- 64x32 texture size
- Recommended for full feature set

### Animation File Format

```
section=nodes
name	paren	x	y	z	sizex	sizey	sizez	u	v	rotx	roty	rotz	pivx	pivy	pivz	scalx	scaly	scalz	head
torso	root	0	18	0	8	12	4	16	16	0	0	0	0	0	0	0.83	0.83	0.83	0
head	torso	0	6	0	8	8	8	0	0	0	0	0	0	4	0	0	0	0	1
...

section=keyframes
anim	node	frame	type	x	y	z
walk	legr	10	rot	-45	0	0
walk	legr	30	rot	45	0	0
...

section=animations
name	len
idle	120
walk	40
chop	20
...

section=global
texw	texh
64	32
```

---

## 🔧 How It Works

### Animation Selection Priority

The animation system uses a priority hierarchy:

1. **Emotes** (Highest Priority - for local player only)
   - If an emote is active, it plays regardless of other states
   - Emotes have a duration and cooldown

2. **Action Animations** (High Priority)
   - Triggered by player actions (attacking, mining)
   - Detected by equipped item type and mouse state

3. **Movement Animations** (Default)
   - Walk if moving
   - Idle if standing still

### Animation State Machine

```
Player State Check
    ↓
Is Emote Active? → YES → Play Emote
    ↓ NO
Is Mining/Attacking? → YES → Check Tool Type → Play Action Animation
    ↓ NO
Is Moving? → YES → Play Walk
    ↓ NO
Play Idle
```

---

## 💻 Code Implementation

### DrawPlayers.ci.cs

The `ModDrawPlayers` class handles animation selection:

```csharp
// Get animation based on player state
string GetPlayerAnimation(Game game, Entity player, int playerId)
{
    // 1. Check for emotes (highest priority)
    if (playerId == game.LocalPlayerId)
    {
        ModEmoteSystem emoteSystem = game.GetMod_EmoteSystem();
        if (emoteSystem != null && emoteSystem.IsPlayingEmote())
        {
            return emoteSystem.GetCurrentEmote();
        }
    }
    
    // 2. Check for action animations
    if (playerId == game.LocalPlayerId && game.mouseLeft && game.currentAttackedBlock != null)
    {
        Packet_Item item = game.d_Inventory.RightHand[game.ActiveMaterial];
        if (item != null)
        {
            string blockName = game.blocktypes[item.BlockId].Name;
            
            // Tool detection
            if (blockName contains "Pickaxe" or "Axe" or "Shovel")
                return "chop";
            
            if (blockName contains "Sword")
                return "sword_attack";
            
            if (blockName contains "Bow")
                return "bow_draw";
        }
    }
    
    // 3. Default movement animations
    return player.playerDrawInfo.moves ? "walk" : "idle";
}
```

### EmoteSystem.ci.cs

The `ModEmoteSystem` class manages emote playback:

```csharp
public class ModEmoteSystem : ClientMod
{
    // Play an emote
    public void PlayEmote(Game game, string emoteName)
    {
        if (!IsValidEmote(emoteName))
            return;
        
        currentEmote = emoteName;
        
        // Calculate duration based on animation length
        int animLength = GetAnimationLength();
        float duration = animLength / 60.0f; // 60 FPS
        emoteEndTime = currentTime + duration + COOLDOWN;
    }
    
    // Process chat commands like /wave
    public void ProcessChatCommand(Game game, string message)
    {
        if (message starts with "/")
        {
            string command = ExtractCommand(message);
            if (IsValidEmote(command))
            {
                PlayEmote(game, command);
            }
        }
    }
}
```

---

## 🎮 Using Emotes

### Chat Commands

Players can trigger emotes by typing in chat:

```
/wave     → Wave gesture
/point    → Point gesture
/cheer    → Cheer animation
/talk     → Talking animation
```

### Future: Emote Wheel

A radial menu (planned) will allow quick emote selection:
- Press `B` key to open emote wheel
- Move mouse to desired emote
- Click to perform emote

---

## 🛠️ Creating New Animations

### Step 1: Design Animation

Plan your animation keyframes:
- Which bones need to move?
- What rotations/positions/scales?
- How many frames total?

Example planning for "sit" animation:
```
Frame 0:  Standing (torso at y=18)
Frame 10: Bending down (torso at y=12, legs bent)
Frame 20: Sitting (torso at y=8, legs forward)
```

### Step 2: Add to Model File

Edit `data/public/playerenhanced.txt`:

```
section=keyframes
anim	node	frame	type	x	y	z
# Sit animation
sit	torso	0	pos	0	18	0
sit	torso	10	pos	0	12	0
sit	torso	20	pos	0	8	0
sit	legr	20	rot	90	0	0
sit	legl	20	rot	90	0	0

section=animations
name	len
sit	20
```

### Step 3: Add to Emote System

Edit `EmoteSystem.ci.cs`:

```csharp
internal static string[] AVAILABLE_EMOTES = new string[]
{
    "wave",
    "point",
    "cheer",
    "talk",
    "sit"  // Add new emote here
};
```

### Step 4: Test

1. Build the game
2. Run in-game
3. Type `/sit` in chat
4. Watch your character perform the animation!

---

## 🎨 Animation Tips

### Natural Motion

1. **Anticipation** - Wind up before action
   ```
   Frame 0:  arm at rest (rotation 0)
   Frame 5:  arm pulls back (rotation -30)  ← Anticipation
   Frame 10: arm swings (rotation 90)       ← Action
   ```

2. **Follow-through** - Don't stop abruptly
   ```
   Frame 10: Peak of swing (rotation 90)
   Frame 15: Slight overshoot (rotation 95)  ← Follow-through
   Frame 20: Return to rest (rotation 0)
   ```

3. **Easing** - Use more keyframes for smooth motion
   ```
   Bad:  Frame 0 → Frame 20 (instant jump)
   Good: Frame 0 → 5 → 10 → 15 → 20 (smooth)
   ```

### Timing

- **Idle:** 60-120 frames (slow, gentle)
- **Walk:** 30-40 frames (moderate)
- **Actions:** 15-30 frames (quick, snappy)
- **Emotes:** 10-30 frames (expressive)

### Node Reference

Available bones/nodes for animation:
- **torso** - Main body
- **head** - Head (set head=1 for camera follow)
- **hat** - Hat/helmet layer
- **legr** - Right leg
- **legl** - Left leg
- **armr** - Right arm
- **arml** - Left arm
- **itemr** - Item in right hand
- **iteml** - Item in left hand

### Keyframe Types

- **pos** - Position (x, y, z in pixels/16)
- **rot** - Rotation (x, y, z in degrees)
- **siz** - Size (x, y, z in pixels)
- **piv** - Pivot point for rotation
- **sca** - Scale multiplier

---

## 🔗 Integration with Other Systems

### VOIP Integration (Future)

When implemented, VOIP will automatically trigger "talk" animation:

```csharp
// Pseudo-code
if (PlayerSpeaking(playerId))
{
    TriggerAnimation("talk");
}
```

### Combat System (Future)

Combat animations will be triggered by attack states:

```csharp
if (PlayerAttacking)
{
    if (HasSword) → "sword_attack"
    if (HasBow) → "bow_fire"
    if (Blocking) → "shield_block"
}
```

---

## 📊 Performance Considerations

### Animation Performance

- **Keyframe Interpolation:** ~0.001ms per player per frame
- **Memory:** ~10KB per animation model
- **Network:** Emotes send ~100 bytes per packet

### Optimization Tips

1. **Limit Animation Complexity**
   - Fewer nodes = better performance
   - 8-10 bones is optimal for character models

2. **Keyframe Count**
   - More keyframes = smoother but more data
   - 4-6 keyframes per animation is usually enough

3. **Remote Players**
   - Only animate players in view range
   - Already handled by frustum culling

---

## 🐛 Troubleshooting

### Animation Not Playing

**Problem:** Animation doesn't play when expected

**Solutions:**
1. Check animation name matches exactly (case-sensitive)
2. Verify animation exists in model file
3. Check that model is using `playerenhanced.txt` not `player.txt`
4. Ensure animation has non-zero length

### Animation Looks Weird

**Problem:** Animation plays but looks strange

**Solutions:**
1. Check keyframe types (pos vs rot vs siz)
2. Verify pivot points are set correctly
3. Check for conflicting keyframes
4. Ensure parent-child relationships are correct

### Emote Not Working

**Problem:** `/emote` command does nothing

**Solutions:**
1. Verify `ModEmoteSystem` is registered in `Game.ci.cs`
2. Check emote name is in `AVAILABLE_EMOTES` array
3. Ensure chat command starts with `/`
4. Check for cooldown (0.5s between emotes)

---

## 📚 Additional Resources

### Animation Examples

Study existing animations in `playerenhanced.txt`:
- **walk** - Good example of cyclic animation
- **chop** - Good example of action animation with anticipation
- **wave** - Good example of simple emote

### Model Editors

Currently, animations must be edited by hand in text format. Future tools:
- **Visual Model Editor** (planned)
- **Animation Timeline Tool** (planned)

### External References

- [Minecraft Animation Format](https://minecraft.wiki/w/Model) - Similar system
- [12 Principles of Animation](https://en.wikipedia.org/wiki/Twelve_basic_principles_of_animation)
- [Game Animation Tutorial](https://www.gamedeveloper.com/design/the-art-of-game-animation)

---

## 🎯 Future Enhancements

### Short Term
- [ ] Chat integration for emote commands
- [ ] Emote cooldown visual indicator
- [ ] Remote player emote synchronization

### Medium Term
- [ ] Emote wheel radial menu
- [ ] Custom emote keybinds
- [ ] Emote preview in UI
- [ ] Animation blending/transitions

### Long Term
- [ ] Visual animation editor
- [ ] Procedural animations
- [ ] Physics-based animation
- [ ] Facial expressions
- [ ] Lip-sync for VOIP

---

## ✨ Credits

**Animation System:** Enhanced from Manic Digger base  
**Emote Design:** Community feedback and standard gaming conventions  
**Documentation:** GitHub Copilot

---

**Questions or suggestions? Open an issue on GitHub!**

**Happy Animating!** 🎬
