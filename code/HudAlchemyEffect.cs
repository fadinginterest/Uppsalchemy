using System.Collections.Generic;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.MathTools;
using Vintagestory.GameContent;

namespace Uppsalchemy
{
    public class HudElementAlchemyEffect : HudElement
    {
        const string ActiveEffectKindAttribute = "uppsalchemyActiveEffectKind";
        const string ActiveItemCodeAttribute = "uppsalchemyActiveItemCode";
        const string EffectRemainingSecondsAttribute = "uppsalchemyEffectRemainingSeconds";
        const string EffectTotalSecondsAttribute = "uppsalchemyEffectTotalSeconds";

        const float BlinkThresholdSeconds = 10f;
        const float BlinkIntervalSeconds = 0.25f;

        static readonly double[] TurquoiseColor = { 0.1, 0.9, 0.85 };
        static readonly double[] GoldColor = { 1.0, 0.98, 0.85 };

        GuiElementStatbar bar;
        double[] barColor;
        bool currentlyVisible;

        float blinkAccum;
        bool blinkOn = true;


        readonly HashSet<long> glowingEntityIds = new HashSet<long>();

        public HudElementAlchemyEffect(ICoreClientAPI capi) : base(capi)
        {
            ElementBounds barBounds = ElementBounds.Fixed(0, 0, 300, 17)
                .WithAlignment(EnumDialogArea.CenterBottom)
                .WithFixedAlignmentOffset(0, -60);

            ElementBounds dialogBounds = ElementBounds.Fixed(0, 0, 300, 17)
                .WithAlignment(EnumDialogArea.CenterBottom)
                .WithFixedAlignmentOffset(0, -60);


            barColor = new double[] { TurquoiseColor[0], TurquoiseColor[1], TurquoiseColor[2] };

            bar = new GuiElementStatbar(capi, barBounds, barColor, false, false);
            bar.ShowValueOnHover = false;

            SingleComposer = capi.Gui
                .CreateCompo("uppsalchemyeffectbar", dialogBounds)
                .AddInteractiveElement(bar, "effectbar")
                .Compose();
        }

        public override void OnRenderGUI(float deltaTime)
        {
            UpdateGlowingEntities();
            UpdateOwnEffectBar(deltaTime);
        }

        void UpdateGlowingEntities()
        {
            foreach (Entity entity in capi.World.LoadedEntities.Values)
            {
                bool hasLightData = entity.WatchedAttributes.HasAttribute(LightEffectHandler.LightHueAttribute);

                if (hasLightData)
                {
                    int hue = entity.WatchedAttributes.GetInt(LightEffectHandler.LightHueAttribute, 33);
                    int saturation = entity.WatchedAttributes.GetInt(LightEffectHandler.LightSaturationAttribute, 7);
                    int brightness = entity.WatchedAttributes.GetInt(LightEffectHandler.LightBrightnessAttribute, 10);

                    entity.LightHsv = new byte[] { (byte)hue, (byte)saturation, (byte)brightness };
                    glowingEntityIds.Add(entity.EntityId);
                }
                else if (glowingEntityIds.Contains(entity.EntityId))
                {
                    entity.LightHsv = null;
                    glowingEntityIds.Remove(entity.EntityId);
                }
            }
        }

        void UpdateOwnEffectBar(float deltaTime)
        {
            IClientPlayer player = capi.World.Player;
            Entity entity = player?.Entity;

            bool hasActiveEffect = entity != null && !string.IsNullOrEmpty(entity.WatchedAttributes.GetString(ActiveEffectKindAttribute, null));

            if (hasActiveEffect)
            {
                float remainingSeconds = entity.WatchedAttributes.GetFloat(EffectRemainingSecondsAttribute, 0);
                float totalSeconds = entity.WatchedAttributes.GetFloat(EffectTotalSecondsAttribute, 1);
                float remainingFraction = remainingSeconds / totalSeconds;

                bool inBlinkWindow = remainingSeconds <= BlinkThresholdSeconds;

                if (inBlinkWindow)
                {
                    blinkAccum += deltaTime;
                    if (blinkAccum >= BlinkIntervalSeconds)
                    {
                        blinkAccum -= BlinkIntervalSeconds;
                        blinkOn = !blinkOn;
                    }
                }
                else
                {
                    blinkAccum = 0;
                    blinkOn = true;
                }

                double[] targetColor = (inBlinkWindow && !blinkOn) ? GoldColor : TurquoiseColor;
                barColor[0] = targetColor[0];
                barColor[1] = targetColor[1];
                barColor[2] = targetColor[2];

                bar.SetValues(remainingFraction, 0, 1);

                currentlyVisible = true;

                // Icon blinks in sync with the bar - both driven by the same blinkOn flag,
                // so they can never drift out of phase with each other.
                bool showIcon = !inBlinkWindow || blinkOn;
                if (showIcon)
                {
                    string itemCode = entity.WatchedAttributes.GetString(ActiveItemCodeAttribute, null);
                    if (!string.IsNullOrEmpty(itemCode))
                    {
                        RenderPotionIcon(itemCode);
                    }
                }
            }
            else
            {
                currentlyVisible = false;
                blinkAccum = 0;
                blinkOn = true;
            }

            if (currentlyVisible)
            {
                base.OnRenderGUI(deltaTime);
            }
        }

        void RenderPotionIcon(string liquidItemCode)
        {

            Item liquidItem = capi.World.GetItem(new AssetLocation(liquidItemCode));
            Block bottleBlock = capi.World.GetBlock(new AssetLocation("aculinaryartillery:bottle-glass-quartz-fired"));
            if (liquidItem == null || bottleBlock == null) return;

            ItemStack bottleStack = new ItemStack(bottleBlock);
            ItemStack liquidStack = new ItemStack(liquidItem, 100);

            if (bottleBlock is BlockLiquidContainerBase liquidContainer)
            {
                liquidContainer.SetContent(bottleStack, liquidStack);
            }

            DummySlot iconSlot = new DummySlot(bottleStack);
            double iconX = bar.Bounds.renderX + bar.Bounds.OuterWidth + 20;
            double iconY = bar.Bounds.renderY + (bar.Bounds.OuterHeight / 2) - 8;
            capi.Render.RenderItemstackToGui(iconSlot, iconX, iconY, 100, 32, -1, true, false, true);
        }

        public override EnumDialogType DialogType => EnumDialogType.HUD;

        public override bool ShouldReceiveRenderEvents()
        {
            return true;
        }
    }
}