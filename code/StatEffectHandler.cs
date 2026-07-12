using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Datastructures;
using Vintagestory.GameContent;

namespace Uppsalchemy
{
    public class StatEffectHandler : IAlchemyEffectHandler
    {
        const string ModifierCode = "uppsalchemyPotion";
        const string ActiveStatAttribute = "uppsalchemyActiveEffectStat";

        public void Apply(JsonObject effect, EntityAgent byEntity)
        {
            string statKey = effect["type"].AsString(null);
            float magnitude = effect["magnitude"].AsFloat(1f);

            if (string.IsNullOrEmpty(statKey)) return;

            float delta = magnitude - 1f;
            byEntity.Stats.Set(statKey, ModifierCode, delta, true);
            byEntity.WatchedAttributes.SetString(ActiveStatAttribute, statKey);

            byEntity.GetBehavior<EntityBehaviorHealth>()?.UpdateMaxHealth();
        }

        public void Clear(EntityAgent byEntity)
        {
            string statKey = byEntity.WatchedAttributes.GetString(ActiveStatAttribute, null);
            if (!string.IsNullOrEmpty(statKey))
            {
                byEntity.Stats.Remove(statKey, ModifierCode);
            }
            byEntity.WatchedAttributes.RemoveAttribute(ActiveStatAttribute);

            byEntity.GetBehavior<EntityBehaviorHealth>()?.UpdateMaxHealth();
        }

        public void Tick(EntityAgent byEntity)
        {
            // Stats.Set persists on its own - nothing to reassert each tick.
        }
    }
}