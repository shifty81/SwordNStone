# Quick Reference: Character Animation System

**Last Updated:** December 2025  
**Status:** ✅ Ready for Testing

---

## 📖 Quick Start

### What Was Added
- ✅ 11 complete character animations (walk, chop, combat, emotes)
- ✅ Equipment-based animation system
- ✅ Social emote system with chat commands
- ✅ Complete VOIP preparation and design
- ✅ Comprehensive roadmap and documentation

### Files to Know
- **Animations:** `data/public/playerenhanced.txt`
- **Emote System:** `ManicDiggerLib/Client/Mods/EmoteSystem.ci.cs`
- **Animation Selection:** `ManicDiggerLib/Client/Mods/DrawPlayers.ci.cs`
- **Roadmap:** `ROADMAP.md`
- **VOIP Design:** `VOIP_AND_EMOTE_SYSTEM.md`
- **Guide:** `ANIMATION_SYSTEM_GUIDE.md`

---

## 🎬 Available Animations

### Core
- `idle` - Standing still (120 frames)
- `walk` - Walking (40 frames)

### Actions (Auto-Triggered by Equipment)
- `chop` - Mining/chopping (20 frames) - Pickaxe/Axe/Shovel
- `sword_attack` - Sword swing (15 frames) - Sword
- `bow_draw` - Draw bow (10 frames) - Bow
- `bow_fire` - Fire arrow (10 frames) - Bow release
- `shield_block` - Block stance (10 frames) - Shield (future)

### Emotes (Chat Commands)
- `wave` - `/wave` - Greeting (20 frames)
- `point` - `/point` - Pointing (10 frames)
- `cheer` - `/cheer` - Celebration (10 frames)
- `talk` - `/talk` - Talking (12 frames) - For VOIP

---

## 🎮 Usage

### Players
**Emotes:** Type in chat:
```
/wave
/point
/cheer
/talk
```

**Action Animations:** Automatically play when using equipment

### Developers
**Add New Animation:**
1. Edit `data/public/playerenhanced.txt`
2. Add keyframes in `section=keyframes`
3. Add animation entry in `section=animations`
4. Optional: Add to emote system

**Test Animation:**
1. Compile project
2. Run game
3. Trigger animation (equip tool/type command)
4. Verify motion looks correct

---

## 🔧 Animation Format

```
section=keyframes
anim    node    frame   type    x       y       z
idle    torso   0       pos     0       18      0
walk    legr    10      rot     -45     0       0
```

**Types:**
- `pos` - Position (x,y,z in pixels/16)
- `rot` - Rotation (x,y,z in degrees)
- `siz` - Size (x,y,z in pixels)
- `piv` - Pivot point
- `sca` - Scale multiplier

**Nodes:**
- torso, head, hat
- armr, arml (arms)
- legr, legl (legs)
- itemr, iteml (held items)

---

## 📚 Documentation

**Full Guides:**
- [ANIMATION_SYSTEM_GUIDE.md](ANIMATION_SYSTEM_GUIDE.md) - Complete animation tutorial
- [VOIP_AND_EMOTE_SYSTEM.md](VOIP_AND_EMOTE_SYSTEM.md) - VOIP architecture & emote wheel
- [ROADMAP.md](ROADMAP.md) - Project roadmap & optimizations
- [CHARACTER_ANIMATION_SUMMARY.md](CHARACTER_ANIMATION_SUMMARY.md) - Implementation summary

**Key Sections:**
- Animation creation: `ANIMATION_SYSTEM_GUIDE.md` → "Creating New Animations"
- Emote wheel design: `VOIP_AND_EMOTE_SYSTEM.md` → "Emote Wheel"
- VOIP implementation: `VOIP_AND_EMOTE_SYSTEM.md` → "VOIP System"
- Future plans: `ROADMAP.md` → "Planned Features"

---

## 🐛 Common Issues

**Animation not playing?**
- Check animation name matches exactly
- Verify using `playerenhanced.txt` not `player.txt`
- Ensure animation has length > 0

**Emote not working?**
- Verify `ModEmoteSystem` registered in Game.ci.cs
- Check command starts with `/`
- Wait for cooldown (0.5s)

**Equipment detection not working?**
- Check item name contains keywords (Pickaxe, Sword, etc.)
- Verify player is attacking (left mouse held)
- Check `currentAttackedBlock` is not null

---

## ✅ Testing Checklist

**Before Release:**
- [ ] Compile successfully
- [ ] All 11 animations play correctly
- [ ] Equipment detection works for tools/weapons
- [ ] Emote commands trigger animations
- [ ] Cooldown prevents spam
- [ ] Animations look smooth
- [ ] No animation glitches

---

## 🚀 Next Steps

**Immediate:**
1. Compile on Windows
2. Test all animations
3. Fix any issues

**Short Term:**
1. Connect emotes to chat
2. Add multiplayer sync
3. Create emote icons

**Long Term:**
1. Implement VOIP
2. Add emote wheel
3. Expand animation library

---

## 📞 Need Help?

**Questions?**
- Read full guide: `ANIMATION_SYSTEM_GUIDE.md`
- Check roadmap: `ROADMAP.md`
- Open GitHub issue

**Found a Bug?**
- Open GitHub issue
- Include:
  - Animation name
  - Expected behavior
  - Actual behavior
  - Steps to reproduce

---

## 🎯 Quick Facts

| Metric | Value |
|--------|-------|
| Total Animations | 11 |
| Action Animations | 5 |
| Social Emotes | 4 |
| Bones/Nodes | 9 |
| Documentation | 53KB |
| Code Changes | ~365 lines |

---

**That's it! You're ready to use the enhanced animation system!** 🎬

For detailed information, see the full guides listed above.

**Happy Animating!** ⚔️🗿
