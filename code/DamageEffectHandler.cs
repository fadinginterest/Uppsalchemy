using System;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Datastructures;

namespace Uppsalchemy
{
    public class DamageEffectHandler : IAlchemyEffectHandler
    {
        public void Apply(JsonObject effect, EntityAgent byEntity)
        {
            float magnitude = effect["magnitude"].AsFloat(0);
            float durationSeconds = effect["durationSeconds"].AsFloat(0);
            int ticks = effect["ticks"].AsInt(1);

            if (magnitude == 0) return;

            bool isHeal = magnitude > 0;

            byEntity.ReceiveDamage(new DamageSource
            {
                Source = EnumDamageSource.Internal,
                Type = isHeal ? EnumDamageType.Heal : EnumDamageType.Poison,
                Duration = TimeSpan.FromSeconds(durationSeconds),
                TicksPerDuration = ticks,
                DamageOverTimeTypeEnum = isHeal ? EnumDamageOverTimeEffectType.Unknown : EnumDamageOverTimeEffectType.Poison
            }, Math.Abs(magnitude));
        }

        public void Clear(EntityAgent byEntity)
        {
            // Nothing to clear - once triggered, the engine's own damage-over-time system
            // owns the rest of this, independent of our WatchedAttributes tracking.
        }

        public void Tick(EntityAgent byEntity)
        {
            // Damage effects bypass the tick system entirely in BlockAlchemyBottle - never called.
        }
    }
}