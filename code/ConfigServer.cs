using Vintagestory.API.Common;

namespace Uppsalchemy
{
	public class ConfigServer : IModConfig
	{
		public const string ConfigServerName = "Uppsalchemy.json";

		public static ConfigServer Instance { get; set; } = null;

		// Utility
		public bool EnableSwiftness { get; set; } = true;
		public bool EnableSustenance { get; set; } = true;
		public bool EnableRegeneration { get; set; } = true;
		public bool EnableAqualung { get; set; } = true;
		public bool EnableWarmth { get; set; } = true;
		public bool EnableHealing { get; set; } = true;
		public bool EnableRadiance { get; set; } = true;
		public bool EnableFeatherfall { get; set; } = true;
			
		public bool EnableVitality { get; set; } = true;
		public bool EnableDurability { get; set; } = true;
		public bool EnableEncumbrance { get; set; } = true;
		public bool EnableJumping { get; set; } = true;

		// Combat 
		public bool EnableStrength { get; set; } = true;
		public bool EnablePiercing { get; set; } = true;
		public bool EnableAccuracy { get; set; } = true;
		public bool EnableFlow { get; set; } = true;
		public bool EnableDrawing { get; set; } = true;

		// Luck 
		public bool EnableOreFortune { get; set; } = true;
		public bool EnableHuntersFortune { get; set; } = true;
		public bool EnableForagersFortune { get; set; } = true;
		public bool EnableHarvestFortune { get; set; } = true;
		public bool EnableAncientFortune { get; set; } = true;

		// Advanced
		public bool EnableGlowingBreath { get; set; } = true;
		public bool EnableGracefulLeaping { get; set; } = true;
		public bool EnableGreaterRestoration { get; set; } = true;
		public bool EnableShelteredExploration { get; set; } = true;
		public bool EnableLimitlessExploration { get; set; } = true;
		public bool EnableSuddenRejuvenation { get; set; } = true;
		public bool EnableCruelPatience { get; set; } = true;
		public bool EnableWelcomePunishment { get; set; } = true;

		public ConfigServer(ICoreAPI api, ConfigServer previousConfig = null)
		{
			if (previousConfig == null) return;

			EnableSwiftness = previousConfig.EnableSwiftness;
			EnableSustenance = previousConfig.EnableSustenance;
			EnableRegeneration = previousConfig.EnableRegeneration;
			EnableAqualung = previousConfig.EnableAqualung;
			EnableWarmth = previousConfig.EnableWarmth;
			EnableHealing = previousConfig.EnableHealing;
			EnableRadiance = previousConfig.EnableRadiance;
			EnableFeatherfall = previousConfig.EnableFeatherfall;

			EnableVitality = previousConfig.EnableVitality;
			EnableDurability = previousConfig.EnableDurability;
			EnableEncumbrance = previousConfig.EnableEncumbrance;
			EnableJumping = previousConfig.EnableJumping;

			EnableStrength = previousConfig.EnableStrength;
			EnablePiercing = previousConfig.EnablePiercing;
			EnableAccuracy = previousConfig.EnableAccuracy;
			EnableFlow = previousConfig.EnableFlow;
			EnableDrawing = previousConfig.EnableDrawing;

			EnableOreFortune = previousConfig.EnableOreFortune;
			EnableHuntersFortune = previousConfig.EnableHuntersFortune;
			EnableForagersFortune = previousConfig.EnableForagersFortune;
			EnableHarvestFortune = previousConfig.EnableHarvestFortune;
			EnableAncientFortune = previousConfig.EnableAncientFortune;

			EnableGlowingBreath = previousConfig.EnableGlowingBreath;
			EnableGracefulLeaping = previousConfig.EnableGracefulLeaping;
			EnableGreaterRestoration = previousConfig.EnableGreaterRestoration;
			EnableShelteredExploration = previousConfig.EnableShelteredExploration;
			EnableLimitlessExploration = previousConfig.EnableLimitlessExploration;
			EnableSuddenRejuvenation = previousConfig.EnableSuddenRejuvenation;
			EnableCruelPatience = previousConfig.EnableCruelPatience;
			EnableWelcomePunishment = previousConfig.EnableWelcomePunishment;
		}

		public static void Initialize(ICoreAPI api)
		{
			if (api.Side != EnumAppSide.Server) return;

			Instance = ModConfig.ReadConfig<ConfigServer>(api, ConfigServerName);

			api.World.Config.SetBool("Uppsalchemy.EnableSwiftness", Instance.EnableSwiftness);
			api.World.Config.SetBool("Uppsalchemy.EnableSustenance", Instance.EnableSustenance);
			api.World.Config.SetBool("Uppsalchemy.EnableRegeneration", Instance.EnableRegeneration);
			api.World.Config.SetBool("Uppsalchemy.EnableAqualung", Instance.EnableAqualung);
			api.World.Config.SetBool("Uppsalchemy.EnableWarmth", Instance.EnableWarmth);
			api.World.Config.SetBool("Uppsalchemy.EnableHealing", Instance.EnableHealing);
			api.World.Config.SetBool("Uppsalchemy.EnableRadiance", Instance.EnableRadiance);
			api.World.Config.SetBool("Uppsalchemy.EnableFeatherfall", Instance.EnableFeatherfall);

			api.World.Config.SetBool("Uppsalchemy.EnableVitality", Instance.EnableVitality);
			api.World.Config.SetBool("Uppsalchemy.EnableDurability", Instance.EnableDurability);
			api.World.Config.SetBool("Uppsalchemy.EnableEncumbrance", Instance.EnableEncumbrance);
			api.World.Config.SetBool("Uppsalchemy.EnableJumping", Instance.EnableJumping);

			api.World.Config.SetBool("Uppsalchemy.EnableStrength", Instance.EnableStrength);
			api.World.Config.SetBool("Uppsalchemy.EnablePiercing", Instance.EnablePiercing);
			api.World.Config.SetBool("Uppsalchemy.EnableAccuracy", Instance.EnableAccuracy);
			api.World.Config.SetBool("Uppsalchemy.EnableFlow", Instance.EnableFlow);
			api.World.Config.SetBool("Uppsalchemy.EnableDrawing", Instance.EnableDrawing);

			api.World.Config.SetBool("Uppsalchemy.EnableOreFortune", Instance.EnableOreFortune);
			api.World.Config.SetBool("Uppsalchemy.EnableHuntersFortune", Instance.EnableHuntersFortune);
			api.World.Config.SetBool("Uppsalchemy.EnableForagersFortune", Instance.EnableForagersFortune);
			api.World.Config.SetBool("Uppsalchemy.EnableHarvestFortune", Instance.EnableHarvestFortune);
			api.World.Config.SetBool("Uppsalchemy.EnableAncientFortune", Instance.EnableAncientFortune);

			api.World.Config.SetBool("Uppsalchemy.EnableGlowingBreath", Instance.EnableGlowingBreath);
			api.World.Config.SetBool("Uppsalchemy.EnableGracefulLeaping", Instance.EnableGracefulLeaping);
			api.World.Config.SetBool("Uppsalchemy.EnableGreaterRestoration", Instance.EnableGreaterRestoration);
			api.World.Config.SetBool("Uppsalchemy.EnableShelteredExploration", Instance.EnableShelteredExploration);
			api.World.Config.SetBool("Uppsalchemy.EnableLimitlessExploration", Instance.EnableLimitlessExploration);
			api.World.Config.SetBool("Uppsalchemy.EnableSuddenRejuvenation", Instance.EnableSuddenRejuvenation);
			api.World.Config.SetBool("Uppsalchemy.EnableCruelPatience", Instance.EnableCruelPatience);
			api.World.Config.SetBool("Uppsalchemy.EnableWelcomePunishment", Instance.EnableWelcomePunishment);
		}
	}
}
