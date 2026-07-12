using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Datastructures;

namespace Uppsalchemy
{
    public class LightEffectHandler : IAlchemyEffectHandler
    {
        public const string LightHueAttribute = "uppsalchemyLightHue";
        public const string LightSaturationAttribute = "uppsalchemyLightSaturation";
        public const string LightBrightnessAttribute = "uppsalchemyLightBrightness";

        public void Apply(JsonObject effect, EntityAgent byEntity)
        {
            int hue = effect["hue"].AsInt(33);
            int saturation = effect["saturation"].AsInt(7);
            int brightness = effect["brightness"].AsInt(10);

            byEntity.WatchedAttributes.SetInt(LightHueAttribute, hue);
            byEntity.WatchedAttributes.SetInt(LightSaturationAttribute, saturation);
            byEntity.WatchedAttributes.SetInt(LightBrightnessAttribute, brightness);
        }

        public void Clear(EntityAgent byEntity)
        {
            byEntity.WatchedAttributes.RemoveAttribute(LightHueAttribute);
            byEntity.WatchedAttributes.RemoveAttribute(LightSaturationAttribute);
            byEntity.WatchedAttributes.RemoveAttribute(LightBrightnessAttribute);
        }

        public void Tick(EntityAgent byEntity)
        {

        }
    }
}