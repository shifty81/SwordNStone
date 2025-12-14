/// <summary>
/// Emote System - Allows players to express themselves through character animations
/// Supports both chat commands (/wave, /cheer, etc.) and will eventually support emote wheel
/// </summary>
public class ModEmoteSystem : ClientMod
{
    public ModEmoteSystem()
    {
        currentEmote = null;
        emoteEndTime = 0;
    }

    internal string currentEmote;
    internal float emoteEndTime;
    internal const float EMOTE_COOLDOWN = 0.5f; // Half second between emotes

    // Available emotes - these match the animations in playerenhanced.txt
    internal static string[] AVAILABLE_EMOTES = new string[]
    {
        "wave",
        "point",
        "cheer",
        "talk"
    };
    
    // Override to identify this mod type (CiTo doesn't support 'is' operator)
    public override bool IsEmoteSystem() { return true; }

    public override void OnNewFrameReadOnlyMainThread(Game game, float deltaTime)
    {
        // Check if emote has finished
        if (currentEmote != null)
        {
            float currentTime = game.platform.TimeMillisecondsFromStart() / 1000.0f;
            if (currentTime >= emoteEndTime)
            {
                // Emote finished, return to normal state
                currentEmote = null;
            }
        }
    }

    // Check if player is currently playing an emote
    public bool IsPlayingEmote()
    {
        return currentEmote != null;
    }

    // Get current emote being played
    public string GetCurrentEmote()
    {
        return currentEmote;
    }

    // Play an emote animation
    public void PlayEmote(Game game, string emoteName)
    {
        // Validate emote name
        if (!IsValidEmote(emoteName))
        {
            return;
        }

        float currentTime = game.platform.TimeMillisecondsFromStart() / 1000.0f;

        // Check cooldown
        if (currentEmote != null && currentTime < emoteEndTime)
        {
            return; // Still playing previous emote
        }

        // Set current emote
        currentEmote = emoteName;

        // Get animation length from player model
        Entity player = game.player;
        if (player != null && player.drawModel != null && player.drawModel.renderer != null)
        {
            // Set the animation first so we can get its length
            player.drawModel.renderer.SetAnimation(emoteName);
            
            int animLength = player.drawModel.renderer.GetAnimationLength();
            float duration = animLength / 60.0f; // Assuming 60 FPS animation

            // Add minimum emote duration
            if (duration < 1.0f)
            {
                duration = 1.0f; // At least 1 second
            }

            emoteEndTime = currentTime + duration + EMOTE_COOLDOWN;

            // TODO: Send emote packet to server for synchronization
            // SendEmotePacket(emoteName);
        }
    }

    // Check if emote name is valid
    bool IsValidEmote(string emoteName)
    {
        if (emoteName == null)
        {
            return false;
        }

        for (int i = 0; i < AVAILABLE_EMOTES.Length; i++)
        {
            if (AVAILABLE_EMOTES[i] == emoteName)
            {
                return true;
            }
        }

        return false;
    }

    // Process chat command for emotes
    public void ProcessChatCommand(Game game, string message)
    {
        if (message == null || message.Length < 2)
        {
            return;
        }

        // Check if it's a command (starts with /)
        if (message[0] != '/')
        {
            return;
        }

        // Extract command (remove /)
        string command = "";
        for (int i = 1; i < message.Length; i++)
        {
            char c = message[i];
            if (c == ' ')
            {
                break;
            }
            command = game.platform.StringFormat("{0}{1}", command, game.platform.CharToString(c));
        }

        // Convert to lowercase for comparison
        command = game.platform.StringToLower(command);

        // Check if it's an emote command
        if (IsValidEmote(command))
        {
            PlayEmote(game, command);
        }
    }
}

/// <summary>
/// Integration with DrawPlayers to apply emote animations
/// This extends the animation selection system to include emotes
/// </summary>
public class EmoteAnimationIntegration
{
    // Get the appropriate animation considering emotes
    public static string GetAnimationWithEmotes(Game game, Entity player, int playerId, string defaultAnimation)
    {
        // Check if player has an active emote
        ModEmoteSystem emoteSystem = game.GetMod_EmoteSystem();
        if (emoteSystem != null && playerId == game.LocalPlayerId)
        {
            if (emoteSystem.IsPlayingEmote())
            {
                string emote = emoteSystem.GetCurrentEmote();
                if (emote != null)
                {
                    return emote;
                }
            }
        }

        // No active emote, use default animation
        return defaultAnimation;
    }
}
