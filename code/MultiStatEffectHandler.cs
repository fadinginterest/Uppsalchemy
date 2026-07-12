using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Datastructures;

namespace Uppsalchemy
{
	public class MultiStatEffectHandler : IAlchemyEffectHandler
	{
		const string ModifierCode = "uppsalchemyPotion";
		const string WalkspeedDeltaAttribute = "uppsalchemyMultiStatWalkspeedDelta";
		const string HealPerTickAttribute = "uppsalchemyMultiStatHealPerTick";

		readonly (string jsonField, string statKey)[] statMappings;
		readonly string healAmountField;
		readonly bool normalizeWalkspeed;
		readonly string healPerSecondField;

		public MultiStatEffectHandler(
			(string jsonField, string statKey)[] statMappings,
			string healAmountField = null,
			bool normalizeWalkspeed = false,
			string healPerSecondField = null)
		{
			this.statMappings = statMappings;
			this.healAmountField = healAmountField;
			this.normalizeWalkspeed = normalizeWalkspeed;
			this.healPerSecondField = healPerSecondField;
		}

		public void Apply(JsonObject effect, EntityAgent byEntity)
		{
			foreach (var (jsonField, statKey) in statMappings)
			{
				float magnitude = effect[jsonField].AsFloat(1f);
				byEntity.Stats.Set(statKey, ModifierCode, magnitude - 1f, true);
			}

			if (healAmountField != null)
			{
				float healAmount = effect[healAmountField].AsFloat(0);
				if (healAmount > 0)
				{
					byEntity.ReceiveDamage(new DamageSource
					{
						Source = EnumDamageSource.Internal,
						Type = EnumDamageType.Heal,
						Duration = System.TimeSpan.Zero,
						TicksPerDuration = 1,
						DamageOverTimeTypeEnum = EnumDamageOverTimeEffectType.Unknown
					}, healAmount);
				}
			}

			if (normalizeWalkspeed)
			{
				RetargetWalkspeedToNormal(byEntity);
			}

			if (healPerSecondField != null)
			{
				float healPerSecond = effect[healPerSecondField].AsFloat(0);
				float healPerTick = healPerSecond * (BlockAlchemyBottle.TickIntervalMs / 1000f);
				byEntity.WatchedAttributes.SetFloat(HealPerTickAttribute, healPerTick);
			}
		}

		public void Tick(EntityAgent byEntity)
		{
			if (normalizeWalkspeed)
			{
				RetargetWalkspeedToNormal(byEntity);
			}

			if (healPerSecondField != null)
			{
				float healPerTick = byEntity.WatchedAttributes.GetFloat(HealPerTickAttribute, 0f);
				if (healPerTick > 0)
				{
					byEntity.ReceiveDamage(new DamageSource
					{
						Source = EnumDamageSource.Internal,
						Type = EnumDamageType.Heal,
						Duration = System.TimeSpan.Zero,
						TicksPerDuration = 1,
						DamageOverTimeTypeEnum = EnumDamageOverTimeEffectType.Unknown
					}, healPerTick);
				}
			}
		}

		void RetargetWalkspeedToNormal(EntityAgent byEntity)
		{
			float currentBlended = byEntity.Stats.GetBlended("walkspeed");
			float ourPreviousDelta = byEntity.WatchedAttributes.GetFloat(WalkspeedDeltaAttribute, 0f);

			float everyoneElseContribution = currentBlended - ourPreviousDelta;
			float ourNewDelta = 1f - everyoneElseContribution;

			byEntity.Stats.Set("walkspeed", ModifierCode, ourNewDelta, true);
			byEntity.WatchedAttributes.SetFloat(WalkspeedDeltaAttribute, ourNewDelta);
		}

		public void Clear(EntityAgent byEntity)
		{
			foreach (var (jsonField, statKey) in statMappings)
			{
				byEntity.Stats.Remove(statKey, ModifierCode);
			}

			if (normalizeWalkspeed)
			{
				byEntity.Stats.Remove("walkspeed", ModifierCode);
				byEntity.WatchedAttributes.RemoveAttribute(WalkspeedDeltaAttribute);
			}

			if (healPerSecondField != null)
			{
				byEntity.WatchedAttributes.RemoveAttribute(HealPerTickAttribute);
			}
		}
	}
}
