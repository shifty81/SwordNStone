public class ModApplyCharacterCustomization : ClientMod
{
	public override void OnNewFrame(Game game, NewFrameEventArgs args)
	{
		// Apply character customization to local player
		if (game.LocalPlayerId >= 0 && game.LocalPlayerId < game.entitiesCount)
		{
			Entity localPlayer = game.entities[game.LocalPlayerId];
			if (localPlayer != null && localPlayer.drawModel != null)
			{
				if (!customizationApplied)
				{
					ApplyCustomization(game, localPlayer);
					customizationApplied = true;
				}
			}
		}
	}
	
	bool customizationApplied;
	
	void ApplyCustomization(Game game, Entity player)
	{
		// Load character customization from preferences
		string data = game.platform.PreferencesGet("CharacterCustomization");
		CharacterCustomization customization = CharacterCustomization.Deserialize(game.platform, data);
		
		// Set the model - use enhanced model for better animations
		player.drawModel.Model_ = customization.GetModelName();
		
		// Set the texture based on customization
		string textureName = customization.GetTextureName();
		
		// Try to load custom texture, fallback to default if not found
		byte[] textureFile = game.GetFile(textureName);
		if (textureFile != null)
		{
			player.drawModel.Texture_ = textureName;
		}
		else
		{
			// Fallback to default texture
			player.drawModel.Texture_ = "mineplayer.png";
		}
		
		// Reset current texture to force reload
		player.drawModel.CurrentTexture = -1;
		
		// If we have a renderer already, reset it to use new model
		if (player.drawModel.renderer != null)
		{
			player.drawModel.renderer = null;
		}
	}
}
