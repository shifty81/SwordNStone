using System;
using System.Collections.Generic;
using ManicDigger;

namespace SwordAndStone.Mods
{
	/// <summary>
	/// Basic melee combat system with sword attacks and shield blocking
	/// </summary>
	public class CombatSystem : IMod
	{
		ModManager m;
		
		// Track last attack time per player for cooldown
		private Dictionary<int, DateTime> lastAttackTime = new Dictionary<int, DateTime>();
		
		// Track if player is blocking with shield
		private Dictionary<int, bool> playerBlocking = new Dictionary<int, bool>();
		
		// Combat configuration
		private const double ATTACK_COOLDOWN_SECONDS = 0.5;  // Half second between attacks
		private const float MELEE_RANGE = 3.0f;  // Maximum melee attack range in blocks
		private const int BASE_PUNCH_DAMAGE = 1;  // Damage when punching (no weapon)
		
		public void PreStart(ModManager m)
		{
			m.RequireMod("Core");
			m.RequireMod("Weapons");
		}

		public void Start(ModManager manager)
		{
			m = manager;
			
			// Register combat event handlers
			m.RegisterOnPlayerJoin(OnPlayerJoin);
			m.RegisterOnPlayerLeave(OnPlayerLeave);
			
			// Note: We would use RegisterOnWeaponHit if available,
			// but that's specific to the War mod. Instead, we'll need
			// client-side hit detection via entity interaction.
			
			Console.WriteLine("[CombatSystem] Melee combat system initialized");
		}
		
		void OnPlayerJoin(int playerId)
		{
			// Initialize player combat state
			lastAttackTime[playerId] = DateTime.UtcNow.AddSeconds(-10);  // Allow immediate first attack
			playerBlocking[playerId] = false;
			
			Console.WriteLine("[CombatSystem] Player {0} joined - combat state initialized", playerId);
		}
		
		void OnPlayerLeave(int playerId)
		{
			// Clean up player combat state
			if (lastAttackTime.ContainsKey(playerId))
			{
				lastAttackTime.Remove(playerId);
			}
			if (playerBlocking.ContainsKey(playerId))
			{
				playerBlocking.Remove(playerId);
			}
			
			Console.WriteLine("[CombatSystem] Player {0} left - combat state cleaned up", playerId);
		}
		
		/// <summary>
		/// Calculate damage based on weapon strength
		/// Returns weapon's Strength value from BlockType, or BASE_PUNCH_DAMAGE if not a weapon
		/// </summary>
		int CalculateWeaponDamage(int blockId)
		{
			if (blockId <= 0 || blockId >= m.GetMaxBlockTypes())
			{
				return BASE_PUNCH_DAMAGE;  // Punching damage
			}
			
			// Get BlockType to access properties
			BlockType blockType = m.GetBlockType(blockId);
			if (blockType == null)
			{
				return BASE_PUNCH_DAMAGE;
			}
			
			// Check if it's a weapon (sword or axe)
			if (blockType.ToolType == ToolType.Sword)
			{
				// Return weapon's damage (Strength property)
				return blockType.Strength > 0 ? blockType.Strength : BASE_PUNCH_DAMAGE;
			}
			// Axes can also be used as weapons (use tool strength)
			else if (blockType.ToolType == ToolType.Axe)
			{
				// Axes do less damage than swords (use 75% of tool strength)
				int axeDamage = (blockType.Strength * 3) / 4;
				return axeDamage > 0 ? axeDamage : BASE_PUNCH_DAMAGE;
			}
			
			// Not a weapon, use punch damage
			return BASE_PUNCH_DAMAGE;
		}
		
		/// <summary>
		/// Check if player is within melee range of target
		/// </summary>
		bool IsInMeleeRange(int attackerId, int targetId)
		{
			float x1 = m.GetPlayerPositionX(attackerId);
			float y1 = m.GetPlayerPositionY(attackerId);
			float z1 = m.GetPlayerPositionZ(attackerId);
			
			float x2 = m.GetPlayerPositionX(targetId);
			float y2 = m.GetPlayerPositionY(targetId);
			float z2 = m.GetPlayerPositionZ(targetId);
			
			float dx = x1 - x2;
			float dy = y1 - y2;
			float dz = z1 - z2;
			
			float distanceSquared = dx * dx + dy * dy + dz * dz;
			float maxRangeSquared = MELEE_RANGE * MELEE_RANGE;
			
			return distanceSquared <= maxRangeSquared;
		}
		
		/// <summary>
		/// Check if enough time has passed since last attack (cooldown)
		/// </summary>
		bool CanAttack(int playerId)
		{
			if (!lastAttackTime.ContainsKey(playerId))
			{
				return true;  // First attack
			}
			
			DateTime now = DateTime.UtcNow;
			TimeSpan timeSinceLastAttack = now - lastAttackTime[playerId];
			
			return timeSinceLastAttack.TotalSeconds >= ATTACK_COOLDOWN_SECONDS;
		}
		
		/// <summary>
		/// Get damage reduction from shield based on shield strength
		/// Shield strength * 10% = damage reduction (e.g., strength 5 = 50% reduction)
		/// </summary>
		float GetShieldDamageReduction(int playerId, int shieldBlockId)
		{
			if (!playerBlocking.ContainsKey(playerId) || !playerBlocking[playerId])
			{
				return 0f;  // Not blocking
			}
			
			if (shieldBlockId <= 0 || shieldBlockId >= m.GetMaxBlockTypes())
			{
				return 0f;  // No shield
			}
			
			// Get BlockType to check if it's a shield
			BlockType blockType = m.GetBlockType(shieldBlockId);
			if (blockType == null || blockType.Name == null)
			{
				return 0f;
			}
			
			// Check if it's a shield by name (no ToolType.Shield enum value)
			if (!blockType.Name.Contains("Shield"))
			{
				return 0f;  // Not a shield
			}
			
			// Get shield strength from BlockType
			int shieldStrength = blockType.Strength;
			
			// Convert strength to reduction percentage (strength * 10%)
			// Cap at 90% to prevent invulnerability
			float reduction = (shieldStrength * 0.1f);
			if (reduction > 0.9f)
			{
				reduction = 0.9f;
			}
			
			return reduction;
		}
		
		/// <summary>
		/// Apply damage from attacker to target
		/// </summary>
		public void ApplyMeleeDamage(int attackerId, int targetId, int weaponBlockId, int shieldBlockId)
		{
			// Validate players exist
			if (attackerId < 0 || targetId < 0)
			{
				return;
			}
			
			// Check cooldown
			if (!CanAttack(attackerId))
			{
				Console.WriteLine("[CombatSystem] Player {0} attack on cooldown", attackerId);
				return;
			}
			
			// Check range
			if (!IsInMeleeRange(attackerId, targetId))
			{
				Console.WriteLine("[CombatSystem] Player {0} out of range to hit {1}", attackerId, targetId);
				return;
			}
			
			// Calculate weapon damage
			int weaponDamage = CalculateWeaponDamage(weaponBlockId);
			
			// Calculate shield damage reduction
			float shieldReduction = GetShieldDamageReduction(targetId, shieldBlockId);
			
			// Get weapon name for logging (optional, for better messages)
			string weaponName = null;
			if (weaponBlockId > 0 && weaponBlockId < m.GetMaxBlockTypes())
			{
				BlockType weaponType = m.GetBlockType(weaponBlockId);
				if (weaponType != null)
				{
					weaponName = weaponType.Name;
				}
			}
			
			// Calculate final damage
			int finalDamage = (int)(weaponDamage * (1.0f - shieldReduction));
			if (finalDamage < 1)
			{
				finalDamage = 1;  // Minimum 1 damage
			}
			
			// Apply damage to target
			int currentHealth = m.GetPlayerHealth(targetId);
			int maxHealth = m.GetPlayerMaxHealth(targetId);
			int newHealth = currentHealth - finalDamage;
			
			if (newHealth < 0)
			{
				newHealth = 0;
			}
			
			m.SetPlayerHealth(targetId, newHealth, maxHealth);
			
			// Update attacker's last attack time
			lastAttackTime[attackerId] = DateTime.UtcNow;
			
			// Log combat event
			string attackerName = m.GetPlayerName(attackerId);
			string targetName = m.GetPlayerName(targetId);
			Console.WriteLine("[CombatSystem] {0} hit {1} for {2} damage (weapon: {3}, health: {4}/{5})",
				attackerName, targetName, finalDamage, weaponName ?? "fist", newHealth, maxHealth);
			
			// Send combat feedback message to attacker
			if (shieldReduction > 0)
			{
				int blockedAmount = (int)(weaponDamage * shieldReduction);
				m.SendMessage(attackerId, string.Format("Hit {0} for {1} damage ({2} blocked by shield)", 
					targetName, finalDamage, blockedAmount));
			}
			else
			{
				m.SendMessage(attackerId, string.Format("Hit {0} for {1} damage", targetName, finalDamage));
			}
			
			// Check if target died
			if (newHealth <= 0)
			{
				OnPlayerKilled(attackerId, targetId);
			}
		}
		
		/// <summary>
		/// Handle player kill event
		/// </summary>
		void OnPlayerKilled(int killerId, int victimId)
		{
			string killerName = m.GetPlayerName(killerId);
			string victimName = m.GetPlayerName(victimId);
			
			// Broadcast kill message
			m.SendMessageToAll(string.Format("{0} was slain by {1}", victimName, killerName));
			
			Console.WriteLine("[CombatSystem] {0} killed {1}", killerName, victimName);
			
			// Note: Respawn is handled by server core, not here
		}
		
		/// <summary>
		/// Toggle shield blocking for a player
		/// </summary>
		public void SetPlayerBlocking(int playerId, bool isBlocking)
		{
			playerBlocking[playerId] = isBlocking;
			
			if (isBlocking)
			{
				Console.WriteLine("[CombatSystem] Player {0} is now blocking", playerId);
			}
		}
		
		/// <summary>
		/// Check if player is currently blocking
		/// </summary>
		public bool IsPlayerBlocking(int playerId)
		{
			if (!playerBlocking.ContainsKey(playerId))
			{
				return false;
			}
			return playerBlocking[playerId];
		}
	}
}
