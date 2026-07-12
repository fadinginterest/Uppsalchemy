using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Datastructures;
using Vintagestory.GameContent;

namespace Uppsalchemy
{
    public class TemperatureEffectHandler : IAlchemyEffectHandler
    {
        const string OriginalBodyTemperatureAttribute = "uppsalchemyOriginalBodyTemperature";
        const string TargetBodyTemperatureAttribute = "uppsalchemyTargetBodyTemperature";

        public void Apply(JsonObject effect, EntityAgent byEntity)
        {
            float magnitude = effect["magnitude"].AsFloat(0);

            EntityBehaviorBodyTemperature bodyTemp = byEntity.GetBehavior<EntityBehaviorBodyTemperature>();
            if (bodyTemp == null) return;

            float original = bodyTemp.CurBodyTemperature;
            float target = bodyTemp.NormalBodyTemperature + magnitude;

            byEntity.WatchedAttributes.SetFloat(OriginalBodyTemperatureAttribute, original);
            byEntity.WatchedAttributes.SetFloat(TargetBodyTemperatureAttribute, target);
            bodyTemp.CurBodyTemperature = target;
        }

        public void Tick(EntityAgent byEntity)
        {

            EntityBehaviorBodyTemperature bodyTemp = byEntity.GetBehavior<EntityBehaviorBodyTemperature>();
            if (bodyTemp == null) return;

            float target = byEntity.WatchedAttributes.GetFloat(TargetBodyTemperatureAttribute, bodyTemp.NormalBodyTemperature);
            bodyTemp.CurBodyTemperature = target;
        }

        public void Clear(EntityAgent byEntity)
        {
            EntityBehaviorBodyTemperature bodyTemp = byEntity.GetBehavior<EntityBehaviorBodyTemperature>();
            if (bodyTemp != null)
            {
                float original = byEntity.WatchedAttributes.GetFloat(OriginalBodyTemperatureAttribute, bodyTemp.CurBodyTemperature);
                bodyTemp.CurBodyTemperature = System.Math.Min(bodyTemp.CurBodyTemperature, original);
            }
            byEntity.WatchedAttributes.RemoveAttribute(OriginalBodyTemperatureAttribute);
            byEntity.WatchedAttributes.RemoveAttribute(TargetBodyTemperatureAttribute);
        }
    }
}