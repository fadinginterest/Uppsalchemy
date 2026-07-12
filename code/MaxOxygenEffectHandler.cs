using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Datastructures;
using Vintagestory.GameContent;

namespace Uppsalchemy
{
    public class MaxOxygenEffectHandler : IAlchemyEffectHandler
    {
        public void Apply(JsonObject effect, EntityAgent byEntity)
        {
            EntityBehaviorBreathe breathe = byEntity.GetBehavior<EntityBehaviorBreathe>();
            if (breathe == null) return;

            breathe.Oxygen = breathe.MaxOxygen;
        }

        public void Tick(EntityAgent byEntity)
        {

            EntityBehaviorBreathe breathe = byEntity.GetBehavior<EntityBehaviorBreathe>();
            if (breathe == null) return;

            breathe.Oxygen = breathe.MaxOxygen;
        }

        public void Clear(EntityAgent byEntity)
        {
        }
    }
}