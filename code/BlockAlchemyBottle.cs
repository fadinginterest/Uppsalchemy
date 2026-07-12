using System.Collections.Generic;
using ACulinaryArtillery;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Datastructures;
using Vintagestory.API.Server;

namespace Uppsalchemy
{
    public class BlockAlchemyBottle : BlockBottle
    {
        const string ActiveEffectKindAttribute = "uppsalchemyActiveEffectKind";
        const string ActiveItemCodeAttribute = "uppsalchemyActiveItemCode";
        const string EffectRemainingSecondsAttribute = "uppsalchemyEffectRemainingSeconds";
        const string EffectTotalSecondsAttribute = "uppsalchemyEffectTotalSeconds";
        public const int TickIntervalMs = 200;


        static readonly Dictionary<string, IAlchemyEffectHandler> Handlers = new Dictionary<string, IAlchemyEffectHandler>
        {
            { "stat", new StatEffectHandler() },
            { "maxoxygen", new MaxOxygenEffectHandler() },
            { "damage", new DamageEffectHandler() },
            { "light", new LightEffectHandler() },
            { "temperature", new TemperatureEffectHandler() },

            { "regeneration", new MultiStatEffectHandler(
                new (string, string)[0],
                healPerSecondField: "healPerSecond") },

            { "encumbrance", new MultiStatEffectHandler(
                new (string, string)[0],
                normalizeWalkspeed: true) },

            { "glowingbreath", new CompositeEffectHandler(
                new LightEffectHandler(),
                new MaxOxygenEffectHandler()) },

            { "shelteredexploration", new CompositeEffectHandler(
                new TemperatureEffectHandler(),
                new MaxOxygenEffectHandler()) },

            { "limitlessexploration", new CompositeEffectHandler(
                new TemperatureEffectHandler(),
                new LightEffectHandler(),
                new MaxOxygenEffectHandler()) },

            { "gracefulleaping", new MultiStatEffectHandler(new[]
            {
                ("fallDamageMagnitude", "fallDamageFactor"),
                ("jumpMagnitude", "jumpHeightMul")
            }) },

            { "greaterrestoration", new MultiStatEffectHandler(new[]
            {
                ("extraHpMagnitude", "maxhealthExtraPoints")
            }, healAmountField: "healAmount") },

            { "suddenrejuvenation", new MultiStatEffectHandler(
                new (string, string)[0],
                healAmountField: "healAmount",
                healPerSecondField: "healPerSecond") },

            { "cruelpatience", new MultiStatEffectHandler(new[]
            {
                ("accuracyMagnitude", "rangedWeaponsAcc"),
                ("drawingMagnitude", "bowDrawingStrength"),
                ("piercingMagnitude", "rangedWeaponsDamage"),
                ("speedMagnitude", "rangedWeaponsSpeed")
            }) },

            { "welcomepunishment", new MultiStatEffectHandler(new[]
            {
                ("durabilityMagnitude", "armorDurabilityLoss"),
                ("healingMagnitude", "healingeffectivness"),
                ("meleeMagnitude", "meleeWeaponsDamage")
            }, normalizeWalkspeed: true) }
        };

        protected override void tryEatStop(float secondsUsed, ItemSlot slot, EntityAgent byEntity)
        {
            if (secondsUsed >= 0.95f && byEntity.World is IServerWorldAccessor)
            {
                ItemStack contentStack = GetContent(slot.Itemstack);
                JsonObject effect = contentStack?.Collectible?.Attributes?["alchemyEffect"];

                if (effect != null && effect.Exists)
                {
                    ApplyAlchemyEffect(effect, byEntity, contentStack.Collectible.Code.ToString());
                }
            }

   
            base.tryEatStop(secondsUsed, slot, byEntity);
        }

        void ApplyAlchemyEffect(JsonObject effect, EntityAgent byEntity, string itemCode)
        {
            string configKey = effect["configKey"].AsString(null);
            if (!string.IsNullOrEmpty(configKey))
            {
                bool enabled = byEntity.World.Config.GetBool("Uppsalchemy.Enable" + configKey, true);
                if (!enabled) return;
            }

            string kind = effect["kind"].AsString("stat");

            if (!Handlers.TryGetValue(kind, out IAlchemyEffectHandler handler)) return;

            if (kind == "damage")
            {

                handler.Apply(effect, byEntity);
                return;
            }

            float durationSeconds = effect["durationSeconds"].AsFloat(0);
            if (durationSeconds <= 0) return;

 
            ClearActiveEffect(byEntity);

            handler.Apply(effect, byEntity);

            byEntity.WatchedAttributes.SetString(ActiveEffectKindAttribute, kind);
            byEntity.WatchedAttributes.SetString(ActiveItemCodeAttribute, itemCode);
            byEntity.WatchedAttributes.SetFloat(EffectTotalSecondsAttribute, durationSeconds);
            byEntity.WatchedAttributes.SetFloat(EffectRemainingSecondsAttribute, durationSeconds);

            TickEffect(byEntity, kind, durationSeconds);
        }

        void ClearActiveEffect(EntityAgent byEntity)
        {
            string activeKind = byEntity.WatchedAttributes.GetString(ActiveEffectKindAttribute, null);
            if (!string.IsNullOrEmpty(activeKind) && Handlers.TryGetValue(activeKind, out IAlchemyEffectHandler handler))
            {
                handler.Clear(byEntity);
            }

            byEntity.WatchedAttributes.RemoveAttribute(ActiveEffectKindAttribute);
            byEntity.WatchedAttributes.RemoveAttribute(ActiveItemCodeAttribute);
            byEntity.WatchedAttributes.RemoveAttribute(EffectRemainingSecondsAttribute);
            byEntity.WatchedAttributes.RemoveAttribute(EffectTotalSecondsAttribute);
        }


        void TickEffect(EntityAgent byEntity, string kind, float remainingSeconds)
        {
            byEntity.World.RegisterCallback((dt) =>
            {
                string stillActiveKind = byEntity.WatchedAttributes.GetString(ActiveEffectKindAttribute, null);
                if (stillActiveKind != kind)
                {

                    return;
                }

                float newRemaining = remainingSeconds - (TickIntervalMs / 1000f);

                if (newRemaining <= 0)
                {
                    ClearActiveEffect(byEntity);
                    return;
                }

                if (Handlers.TryGetValue(kind, out IAlchemyEffectHandler handler))
                {
                    handler.Tick(byEntity);
                }

                byEntity.WatchedAttributes.SetFloat(EffectRemainingSecondsAttribute, newRemaining);
                TickEffect(byEntity, kind, newRemaining);
            }, TickIntervalMs);
        }
    }
}