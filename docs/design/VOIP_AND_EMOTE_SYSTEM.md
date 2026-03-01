# VOIP and Emote System Design

**Status:** Design Document / Foundation  
**Implementation Status:** Planned  
**Priority:** High  
**Last Updated:** December 2025

---

## 🎯 Overview

This document outlines the design and implementation plan for the Voice Over IP (VOIP) and Emote system in Sword & Stone. The goal is to create an immersive social experience where players can communicate naturally through voice and express themselves through character animations and gestures.

---

## 🎤 VOIP System

### Core Requirements

1. **Low Latency:** Voice communication must feel instantaneous (<150ms)
2. **Spatial Audio:** Voice volume based on player distance and direction
3. **Quality:** Clear audio even at lower bitrates to save bandwidth
4. **Privacy:** Push-to-talk by default, optional open mic
5. **Visual Feedback:** Clear indication of who is speaking

### Architecture

#### Audio Codec Selection

**Recommended: Opus Codec**
- Excellent quality at low bitrates (16-32 kbps for voice)
- Low latency (frame sizes as low as 2.5ms)
- Open source and royalty-free
- Wide platform support

**Alternative: Speex** (fallback for older systems)
- Lower quality but very lightweight
- Good for low-bandwidth scenarios

#### Voice Capture and Transmission Flow

```
Player A:
[Microphone] → [Audio Capture] → [Opus Encoder] → [Network Layer] → Server

Server:
[Receive Audio] → [Spatial Processing] → [Distribute to Nearby Players]

Player B (Listener):
Server → [Receive Audio] → [Opus Decoder] → [3D Audio Positioning] → [Audio Output]
```

### Technical Implementation

#### 1. Audio Capture (Client-Side)

```csharp
public class VoiceCapture
{
    // Configuration
    private const int SampleRate = 24000; // 24kHz for voice
    private const int FrameSize = 960;    // 40ms at 24kHz
    private const int Channels = 1;        // Mono for voice
    private const int Bitrate = 24000;     // 24 kbps
    
    // Audio input device
    private IAudioInput audioInput;
    
    // Opus encoder
    private OpusEncoder encoder;
    
    // Push-to-talk state
    private bool isTransmitting = false;
    private KeyCode pttKey = KeyCode.V; // Default: V key
    
    public void Initialize()
    {
        // Initialize audio input device
        audioInput = AudioInputDevice.Create(SampleRate, Channels);
        
        // Initialize Opus encoder
        encoder = new OpusEncoder(SampleRate, Channels, Application.VoIP);
        encoder.Bitrate = Bitrate;
        
        // Start audio capture
        audioInput.StartCapture(OnAudioFrameCaptured);
    }
    
    private void OnAudioFrameCaptured(short[] audioData)
    {
        if (!isTransmitting)
            return;
        
        // Encode audio with Opus
        byte[] encodedData = encoder.Encode(audioData, FrameSize);
        
        // Send to server
        SendVoicePacket(encodedData);
    }
    
    public void Update()
    {
        // Check push-to-talk key
        if (Input.GetKeyDown(pttKey))
        {
            isTransmitting = true;
            OnTransmitStart();
        }
        
        if (Input.GetKeyUp(pttKey))
        {
            isTransmitting = false;
            OnTransmitStop();
        }
    }
}
```

#### 2. Spatial Audio Processing (Server-Side)

```csharp
public class VoiceServer
{
    // Voice range in blocks
    private const float MaxVoiceRange = 50.0f;
    private const float MinVoiceRange = 5.0f;
    
    public void OnVoicePacketReceived(int senderId, byte[] audioData)
    {
        // Get sender position
        Vector3 senderPos = GetPlayerPosition(senderId);
        
        // Find players in voice range
        List<int> listeners = GetPlayersInRange(senderPos, MaxVoiceRange);
        
        // Send audio to each listener with distance info
        foreach (int listenerId in listeners)
        {
            if (listenerId == senderId)
                continue; // Don't echo back to sender
            
            Vector3 listenerPos = GetPlayerPosition(listenerId);
            float distance = Vector3.Distance(senderPos, listenerPos);
            
            // Calculate volume based on distance
            float volume = CalculateVoiceVolume(distance);
            
            // Send audio packet to listener
            SendVoiceToClient(listenerId, senderId, audioData, volume, senderPos);
        }
        
        // Trigger talking animation on sender
        TriggerTalkingAnimation(senderId);
    }
    
    private float CalculateVoiceVolume(float distance)
    {
        // Linear falloff with min/max range
        if (distance <= MinVoiceRange)
            return 1.0f;
        
        if (distance >= MaxVoiceRange)
            return 0.0f;
        
        // Linear interpolation
        float t = (distance - MinVoiceRange) / (MaxVoiceRange - MinVoiceRange);
        return 1.0f - t;
    }
}
```

#### 3. Audio Playback (Client-Side)

```csharp
public class VoicePlayback
{
    // Opus decoder
    private OpusDecoder decoder;
    
    // 3D audio sources (one per speaking player)
    private Dictionary<int, AudioSource3D> playerAudioSources;
    
    public void Initialize()
    {
        decoder = new OpusDecoder(SampleRate, Channels);
        playerAudioSources = new Dictionary<int, AudioSource3D>();
    }
    
    public void OnVoicePacketReceived(int speakerId, byte[] encodedAudio, float volume, Vector3 position)
    {
        // Decode audio
        short[] audioData = decoder.Decode(encodedAudio);
        
        // Get or create audio source for this player
        AudioSource3D audioSource = GetOrCreateAudioSource(speakerId);
        
        // Update 3D position
        audioSource.Position = position;
        audioSource.Volume = volume;
        
        // Play audio
        audioSource.PlayBuffer(audioData);
        
        // Update visual feedback
        ShowSpeakerIndicator(speakerId);
    }
    
    private AudioSource3D GetOrCreateAudioSource(int playerId)
    {
        if (!playerAudioSources.ContainsKey(playerId))
        {
            AudioSource3D source = new AudioSource3D();
            source.Spatial = true;
            source.RolloffMode = AudioRolloffMode.Linear;
            source.MinDistance = MinVoiceRange;
            source.MaxDistance = MaxVoiceRange;
            playerAudioSources[playerId] = source;
        }
        
        return playerAudioSources[playerId];
    }
}
```

### Visual Feedback

#### Speaking Indicator UI

```csharp
public class VoiceChatUI
{
    public void ShowSpeakerIndicator(int playerId)
    {
        // Show microphone icon above player's head
        Entity player = game.entities[playerId];
        if (player != null)
        {
            // Draw voice icon above player
            DrawVoiceIcon(player.position.x, player.position.y + 2.5f, player.position.z);
            
            // Trigger talking animation
            if (player.drawModel != null && player.drawModel.renderer != null)
            {
                player.drawModel.renderer.SetAnimation("talk");
            }
        }
        
        // Add to active speakers list in UI
        AddToSpeakersList(playerId, player.name);
    }
    
    private void DrawVoiceIcon(float x, float y, float z)
    {
        // Convert 3D position to 2D screen position
        Vector2 screenPos = WorldToScreen(x, y, z);
        
        // Draw microphone icon
        string iconPath = "local/gui/icons/voice_active.png";
        game.Draw2dBitmapFile(iconPath, 
            game.platform.FloatToInt(screenPos.x) - 16, 
            game.platform.FloatToInt(screenPos.y) - 16, 
            32, 32);
    }
    
    private void DrawSpeakersList(Game game)
    {
        // Draw list of currently speaking players in top-left
        int y = 100;
        foreach (var speaker in activeSpeakers)
        {
            // Draw microphone icon
            game.Draw2dBitmapFile("local/gui/icons/voice_active.png", 10, y, 24, 24);
            
            // Draw player name
            FontCi font = new FontCi();
            font.size = 10;
            game.Draw2dText(speaker.name, font, 40, y + 7, null, false);
            
            y += 30;
        }
    }
}
```

### Configuration Options

```csharp
public class VoiceSettings
{
    // User configurable
    public bool VoiceEnabled = true;
    public KeyCode PushToTalkKey = KeyCode.V;
    public bool OpenMic = false;
    public float InputVolume = 1.0f;
    public float OutputVolume = 1.0f;
    public bool ShowSpeakerNames = true;
    public bool SpatialAudio = true;
    
    // Advanced
    public int Bitrate = 24000;
    public bool EchoCancellation = true;
    public bool NoiseReduction = true;
    public int AudioDevice = 0; // Device index
}
```

---

## 🎭 Emote System

### Overview

The emote system allows players to express emotions and communicate non-verbally through character animations and gestures.

### Emote Categories

#### 1. Communication Emotes
- **Wave** - Greeting gesture
- **Point** - Point at something or someone
- **Beckon** - "Come here" gesture
- **Nod** - Yes/agreement
- **Shake** - No/disagreement

#### 2. Emotional Emotes
- **Cheer** - Celebrate, jump with arms up
- **Laugh** - Hearty laughter animation
- **Cry** - Sad animation
- **Angry** - Frustrated gesture
- **Surprised** - Jump back in surprise

#### 3. Social Emotes
- **Dance** - Simple dance animation
- **Sit** - Sit down
- **Sleep** - Lie down and sleep
- **Bow** - Respectful bow
- **Salute** - Military salute

#### 4. Action Emotes (Already Implemented)
- **Talk** - Speaking animation (auto-triggered by VOIP)
- **Chop** - Mining/chopping animation
- **Bow Draw** - Drawing bow
- **Sword Attack** - Swinging sword
- **Shield Block** - Defensive stance

### Implementation

#### Emote Command System

```csharp
public class EmoteSystem : ClientMod
{
    private Dictionary<string, int> emoteCommands;
    private float emoteTimeout = 0;
    private const float EMOTE_COOLDOWN = 0.5f; // Half second between emotes
    
    public EmoteSystem()
    {
        // Map emote commands to animation IDs
        emoteCommands = new Dictionary<string, int>();
        emoteCommands["wave"] = GetAnimationId("wave");
        emoteCommands["point"] = GetAnimationId("point");
        emoteCommands["cheer"] = GetAnimationId("cheer");
        emoteCommands["dance"] = GetAnimationId("dance");
        emoteCommands["bow"] = GetAnimationId("bow");
        emoteCommands["sit"] = GetAnimationId("sit");
        emoteCommands["laugh"] = GetAnimationId("laugh");
        emoteCommands["cry"] = GetAnimationId("cry");
    }
    
    public override void OnKeyPress(Game game, KeyPressEventArgs args)
    {
        // Check for emote commands in chat
        if (args.GetKeyChar() == '/')
        {
            // Handle chat command
            string command = GetChatInput();
            ProcessEmoteCommand(game, command);
        }
    }
    
    private void ProcessEmoteCommand(Game game, string command)
    {
        // Parse command (e.g., "/wave", "/cheer")
        command = command.Trim().ToLower().TrimStart('/');
        
        if (emoteCommands.ContainsKey(command))
        {
            PlayEmote(game, command);
        }
    }
    
    public void PlayEmote(Game game, string emoteName)
    {
        // Check cooldown
        if (game.platform.TimeMillisecondsFromStart() < emoteTimeout)
            return;
        
        // Get local player
        Entity player = game.player;
        if (player == null || player.drawModel == null)
            return;
        
        // Play animation
        if (emoteCommands.ContainsKey(emoteName))
        {
            player.drawModel.renderer.SetAnimation(emoteName);
            
            // Send to server for synchronization
            SendEmotePacket(emoteName);
            
            // Set cooldown
            emoteTimeout = game.platform.TimeMillisecondsFromStart() + EMOTE_COOLDOWN * 1000;
        }
    }
    
    private void SendEmotePacket(string emoteName)
    {
        // Create packet to sync emote with other players
        // Packet_Emote p = new Packet_Emote();
        // p.EmoteName = emoteName;
        // game.SendPacket(p);
    }
}
```

#### Emote Radial Menu

```csharp
public class EmoteWheel : ClientMod
{
    private bool isOpen = false;
    private KeyCode emoteWheelKey = KeyCode.B; // B for emote wheel
    
    private string[] wheelEmotes = new string[]
    {
        "wave", "point", "cheer", "laugh", 
        "bow", "dance", "sit", "cry"
    };
    
    public override void OnNewFrameDraw2d(Game game, float deltaTime)
    {
        if (isOpen)
        {
            DrawEmoteWheel(game);
        }
    }
    
    public override void OnKeyPress(Game game, KeyPressEventArgs args)
    {
        if (args.GetKeyChar() == emoteWheelKey)
        {
            isOpen = !isOpen;
            
            if (isOpen)
            {
                // Pause player movement when wheel is open
                game.guistate = GuiState.Menu;
            }
            else
            {
                game.guistate = GuiState.Normal;
            }
        }
    }
    
    private void DrawEmoteWheel(Game game)
    {
        int centerX = game.Width() / 2;
        int centerY = game.Height() / 2;
        int radius = 150;
        
        // Draw circular background
        game.Draw2dTexture(game.WhiteTexture(), 
            centerX - radius - 20, centerY - radius - 20,
            (radius + 20) * 2, (radius + 20) * 2,
            null, 0, Game.ColorFromArgb(200, 20, 20, 30), false);
        
        // Draw emote options in circle
        float angleStep = 360.0f / wheelEmotes.Length;
        
        for (int i = 0; i < wheelEmotes.Length; i++)
        {
            float angle = angleStep * i - 90; // Start at top
            float rad = angle * (3.14159f / 180.0f);
            
            int x = centerX + game.platform.FloatToInt(radius * game.platform.MathCos(rad));
            int y = centerY + game.platform.FloatToInt(radius * game.platform.MathSin(rad));
            
            // Draw emote button
            bool isHovered = IsMouseOverEmote(game, x, y, 40);
            DrawEmoteButton(game, wheelEmotes[i], x, y, isHovered);
        }
        
        // Draw center "Cancel" button
        DrawEmoteButton(game, "Cancel", centerX, centerY, false);
    }
    
    private void DrawEmoteButton(Game game, string emoteName, int x, int y, bool hovered)
    {
        int size = 40;
        
        // Draw button background
        int color = hovered ? 
            Game.ColorFromArgb(255, 200, 150, 50) : 
            Game.ColorFromArgb(255, 100, 80, 30);
        
        game.Draw2dTexture(game.WhiteTexture(), 
            x - size/2, y - size/2, size, size,
            null, 0, color, false);
        
        // Draw emote icon (if available)
        string iconPath = game.platform.StringFormat("local/gui/icons/emote_{0}.png", emoteName);
        game.Draw2dBitmapFile(iconPath, x - size/2 + 4, y - size/2 + 4, size - 8, size - 8);
        
        // Draw emote name below button
        FontCi font = new FontCi();
        font.size = 8;
        game.Draw2dText(emoteName, font, x - 20, y + size/2 + 5, null, false);
    }
    
    public override void OnMouseDown(Game game, MouseEventArgs args)
    {
        if (!isOpen)
            return;
        
        int centerX = game.Width() / 2;
        int centerY = game.Height() / 2;
        int radius = 150;
        
        int mouseX = args.GetX();
        int mouseY = args.GetY();
        
        // Check which emote was clicked
        float angleStep = 360.0f / wheelEmotes.Length;
        
        for (int i = 0; i < wheelEmotes.Length; i++)
        {
            float angle = angleStep * i - 90;
            float rad = angle * (3.14159f / 180.0f);
            
            int x = centerX + game.platform.FloatToInt(radius * game.platform.MathCos(rad));
            int y = centerY + game.platform.FloatToInt(radius * game.platform.MathSin(rad));
            
            if (IsMouseOverEmote(game, x, y, 40))
            {
                // Play emote
                EmoteSystem emoteSystem = game.GetMod<EmoteSystem>();
                if (emoteSystem != null)
                {
                    emoteSystem.PlayEmote(game, wheelEmotes[i]);
                }
                
                // Close wheel
                isOpen = false;
                game.guistate = GuiState.Normal;
                break;
            }
        }
    }
}
```

### Emote Synchronization

```csharp
// Server-side emote handling
public class EmoteServerHandler
{
    public void OnEmoteReceived(int playerId, string emoteName)
    {
        // Validate emote name
        if (!IsValidEmote(emoteName))
            return;
        
        // Get player position
        Vector3 playerPos = GetPlayerPosition(playerId);
        
        // Find nearby players to sync animation
        List<int> nearbyPlayers = GetPlayersInRange(playerPos, 100.0f);
        
        // Broadcast emote to nearby players
        foreach (int nearbyId in nearbyPlayers)
        {
            if (nearbyId != playerId)
            {
                SendEmoteToClient(nearbyId, playerId, emoteName);
            }
        }
    }
}

// Client-side emote reception
public class EmoteClientHandler
{
    public void OnEmoteReceived(int playerId, string emoteName)
    {
        Entity player = game.entities[playerId];
        if (player != null && player.drawModel != null)
        {
            // Play animation on remote player
            player.drawModel.renderer.SetAnimation(emoteName);
            
            // Show emote bubble/text above player (optional)
            ShowEmoteBubble(player, emoteName);
        }
    }
    
    private void ShowEmoteBubble(Entity player, string emoteName)
    {
        // Display emote text or icon above player for 2 seconds
        // EmoteBubble bubble = new EmoteBubble();
        // bubble.Text = "*" + emoteName + "*";
        // bubble.Position = player.position;
        // bubble.Duration = 2.0f;
        // AddEmoteBubble(bubble);
    }
}
```

---

## 🔗 Integration: VOIP + Emotes

### Talking Animation with VOIP

When a player speaks through VOIP, automatically trigger the "talk" emote animation:

```csharp
public class VoiceAnimationIntegration
{
    private Dictionary<int, float> playerTalkingState = new Dictionary<int, float>();
    private const float TALK_ANIMATION_FADE = 0.5f; // Fade time in seconds
    
    public void Update(Game game, float deltaTime)
    {
        // Update all players with active voice
        foreach (var kvp in playerTalkingState)
        {
            int playerId = kvp.Key;
            float talkTime = kvp.Value;
            
            Entity player = game.entities[playerId];
            if (player == null || player.drawModel == null)
                continue;
            
            // If player is currently speaking, play talk animation
            if (talkTime > 0)
            {
                player.drawModel.renderer.SetAnimation("talk");
                playerTalkingState[playerId] -= deltaTime;
            }
            else
            {
                // Return to idle or walk animation
                if (player.playerDrawInfo.moves)
                {
                    player.drawModel.renderer.SetAnimation("walk");
                }
                else
                {
                    player.drawModel.renderer.SetAnimation("idle");
                }
            }
        }
    }
    
    public void OnVoiceReceived(int playerId, byte[] audioData)
    {
        // Analyze audio volume to determine if actually speaking
        float volume = AnalyzeAudioVolume(audioData);
        
        if (volume > 0.1f) // Threshold for speech detection
        {
            // Set talk animation state
            playerTalkingState[playerId] = TALK_ANIMATION_FADE;
        }
    }
    
    private float AnalyzeAudioVolume(byte[] audioData)
    {
        // Simple RMS (Root Mean Square) calculation
        if (audioData == null || audioData.Length == 0)
            return 0;
        
        float sum = 0;
        for (int i = 0; i < audioData.Length; i++)
        {
            float sample = audioData[i] / 255.0f;
            sum += sample * sample;
        }
        
        return game.platform.MathSqrt(sum / audioData.Length);
    }
}
```

### Gesture Recognition (Advanced Future Feature)

Using audio analysis to trigger specific emotes based on voice patterns:

```csharp
public class GestureRecognition
{
    // Map voice patterns to gestures
    // Example: Loud voice = excited gesture
    // Example: Questioning tone = confused gesture
    
    public string DetectGesture(byte[] audioData)
    {
        float volume = AnalyzeVolume(audioData);
        float pitch = AnalyzePitch(audioData);
        
        // Very loud = cheer
        if (volume > 0.8f)
            return "cheer";
        
        // Rising pitch = question (maybe point gesture)
        if (pitch > 300) // Hz
            return "point";
        
        // Default to talk
        return "talk";
    }
}
```

---

## 📦 Required Assets

### Icons
- `voice_active.png` - Microphone icon (active)
- `voice_muted.png` - Microphone icon (muted)
- `emote_wave.png` - Wave emote icon
- `emote_point.png` - Point emote icon
- `emote_cheer.png` - Cheer emote icon
- `emote_laugh.png` - Laugh emote icon
- `emote_cry.png` - Cry emote icon
- `emote_bow.png` - Bow emote icon
- `emote_dance.png` - Dance emote icon
- `emote_sit.png` - Sit emote icon

### Sounds (Optional)
- Voice transmission beep (when starting to talk)
- Emote sound effects (laugh sound, etc.)

---

## 🔧 Technical Requirements

### Libraries Needed

#### For C# (.NET/Mono)
- **NAudio** (Windows) - Audio capture and playback
- **OpenTK.Audio** (Cross-platform) - OpenAL wrapper
- **OpusSharp** or **Concentus** - Opus codec implementation

#### For Native/Java (Cito transpiled)
- **OpenAL** - 3D audio
- **Opus codec** - Native library
- Platform-specific audio capture APIs

### Network Protocol

#### Voice Packet Structure
```
[Packet Type: VOICE] [1 byte]
[Player ID] [4 bytes]
[Sequence Number] [4 bytes]
[Audio Data Length] [2 bytes]
[Opus Encoded Audio] [variable]
```

#### Emote Packet Structure
```
[Packet Type: EMOTE] [1 byte]
[Player ID] [4 bytes]
[Emote ID] [1 byte]
[Timestamp] [4 bytes]
```

---

## 🎯 Implementation Phases

### Phase 1: Foundation (Current)
- ✅ Enhanced player model with talk animation
- ✅ Emote animations created (wave, point, cheer, talk)
- ✅ Animation system supports multiple animations
- ✅ Design document complete

### Phase 2: Basic Emote System
- ⏳ Implement chat command emotes (/wave, /cheer, etc.)
- ⏳ Emote synchronization across multiplayer
- ⏳ Cooldown and animation management
- ⏳ Create emote icons

### Phase 3: VOIP Core
- ⏳ Integrate Opus codec
- ⏳ Implement audio capture
- ⏳ Implement push-to-talk
- ⏳ Network packet handling for voice
- ⏳ Basic audio playback

### Phase 4: Spatial Audio
- ⏳ 3D audio positioning
- ⏳ Distance-based volume falloff
- ⏳ Voice range configuration

### Phase 5: Visual Integration
- ⏳ Speaking indicator UI
- ⏳ Voice-triggered talk animation
- ⏳ Active speakers list
- ⏳ Voice settings menu

### Phase 6: Emote Wheel
- ⏳ Radial emote menu UI
- ⏳ Mouse-based emote selection
- ⏳ Quick emote keybinds
- ⏳ Emote bubble display

### Phase 7: Polish
- ⏳ Echo cancellation
- ⏳ Noise reduction
- ⏳ Volume normalization
- ⏳ Voice activity detection (VAD)
- ⏳ Advanced gesture recognition

---

## 🔐 Privacy & Moderation

### User Controls
- Toggle voice chat on/off globally
- Mute individual players
- Block player voice permanently
- Adjust individual player volumes
- Report inappropriate voice behavior

### Server Controls
- Voice chat enable/disable per server
- Voice range limits
- Mute/ban voice privileges
- Voice chat logs (timestamps, not recordings)

---

## 📊 Performance Considerations

### Bandwidth Usage
- **Voice:** ~24 kbps per speaking player
- **Emotes:** <100 bytes per emote (negligible)
- **Total:** With 10 players speaking: ~240 kbps (~30 KB/s)

### CPU Usage
- **Encoding:** Low (Opus is efficient)
- **Decoding:** Low (one decoder per speaking player)
- **3D Audio:** Moderate (spatial calculations)

### Optimization Tips
1. Only transmit when actually speaking (VAD)
2. Reduce bitrate for distant players
3. Limit max simultaneous voices (e.g., closest 5 players)
4. Use client-side VAD to reduce network traffic
5. Aggressive noise gate to reduce silence transmission

---

## 🧪 Testing Plan

### Unit Tests
- Audio encoding/decoding accuracy
- Packet serialization/deserialization
- Volume calculations
- Emote command parsing

### Integration Tests
- Voice transmission end-to-end
- Multi-player voice scenarios
- Animation synchronization
- Network packet handling

### Manual Testing
- Voice quality at various bitrates
- Spatial audio accuracy
- Latency measurements
- Emote responsiveness
- UI usability

---

## 📚 References

### Technical Documentation
- [Opus Codec](https://opus-codec.org/)
- [OpenAL Programmer's Guide](https://www.openal.org/documentation/)
- [Voice Chat Best Practices](https://developer.valvesoftware.com/wiki/Voice_Chat)

### Similar Implementations
- Mumble (open source voice chat)
- Discord (proprietary but well-documented API)
- TeamSpeak (client-server voice)
- Proximity Voice mods for Minecraft

---

## 🎉 Conclusion

The VOIP and Emote system will significantly enhance the social experience in Sword & Stone, allowing players to communicate naturally and express themselves beyond text chat. The modular design allows for incremental implementation, starting with basic emotes and building up to full spatial voice chat.

**Next Steps:**
1. Implement basic emote command system
2. Create emote icons and test animations
3. Research and select VOIP library
4. Prototype audio capture and playback
5. Implement push-to-talk and basic voice transmission

---

**Questions or suggestions? Open an issue on GitHub!**
