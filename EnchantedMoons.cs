using BlueMoon.Events;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.ComponentModel;
using System.IO;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;

namespace BlueMoon
{
    public class Recipes : ModSystem
    {
        public override void AddRecipeGroups()
        {
            RecipeGroup group = new RecipeGroup(() => Language.GetTextValue("LegacyMisc.37") + "Any Platinum Bar", new int[]
            {
                ItemID.PlatinumBar,
                ItemID.GoldBar
            });
            RecipeGroup.RegisterGroup("PlatinumBar", group);
        }
    }

    public class MoonEventPlayer : ModPlayer
    {
        public override void OnEnterWorld()
        {
            if (Main.netMode != NetmodeID.MultiplayerClient) return;
            Events.MoonNetworking.RequestMoonStatus();
        }
    }

    public class BlueMoonConfig : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ServerSide;

        [Header("Blue_Moon")]
        [DefaultValue(true)]
        public bool EnableBlueMoonSpawn { get; set; }

        [DefaultValue(true)]
        public bool DecreaseBlueMoonSpawnRate { get; set; }

        [Header("Cherry_Moon")]
        [DefaultValue(true)]
        public bool EnableCherryMoonSpawn { get; set; }

        [DefaultValue(true)]
        public bool DecreaseCherryMoonSpawnRate { get; set; }

        [Header("Harvest_Moon")]
        [DefaultValue(true)]
        public bool EnableHarvestMoonSpawn { get; set; }

        [DefaultValue(false)]
        public bool DecreaseHarvestMoonSpawnRate { get; set; }

        [Header("Mint_Moon")]
        [DefaultValue(true)]
        public bool EnableMintMoonSpawn { get; set; }

        [DefaultValue(true)]
        public bool IncreaseMintMoonSpawnRate { get; set; }
    }

    namespace BlueMoon
    {
        public class BlueMoonMod : Mod
        {
            public static BlueMoonMod Instance { get; private set; }

            public BlueMoonMod()
            {
                Instance = this;
            }

            public override void HandlePacket(BinaryReader reader, int whoAmI)
            {
                MoonMessageType msgType = (MoonMessageType)reader.ReadByte();

                switch (msgType)
                {
                    case MoonMessageType.MoonStatus:
                        MoonID moonId = (MoonID)reader.ReadByte();
                        bool isStarting = reader.ReadBoolean();
                        HandleMoonStatus(moonId, isStarting);
                        break;
                    case MoonMessageType.RequestMoonStatus:
                        if (Main.netMode != NetmodeID.Server) return;
                        if (Events.BlueMoonEvent.blueMoon)
                            Events.MoonNetworking.SendMoonStatus(Events.MoonID.Blue, true, toClient: whoAmI);
                        if (Events.CherryMoonEvent.cherryMoon)
                            Events.MoonNetworking.SendMoonStatus(Events.MoonID.Cherry, true, toClient: whoAmI);
                        if (Events.HarvestMoonEvent.harvestMoon)
                            Events.MoonNetworking.SendMoonStatus(Events.MoonID.Harvest, true, toClient: whoAmI);
                        if (Events.MintMoonEvent.mintMoon)
                            Events.MoonNetworking.SendMoonStatus(Events.MoonID.Mint, true, toClient: whoAmI);
                        break;
                    default:
                        Logger.WarnFormat("BlueMoon: Unknown Message type: {0}", msgType);
                        break;
                }
            }

            public override void Load()
            {
                ModContent.GetInstance<BlueMoonConfig>();
                Ref<Effect> blueMoonRef = new(Assets.Request<Effect>("Effects/BlueMoonShader", AssetRequestMode.ImmediateLoad).Value);
                Ref<Effect> cherryMoonRef = new(Assets.Request<Effect>("Effects/CherryMoonShader", AssetRequestMode.ImmediateLoad).Value);
                Ref<Effect> harvestMoonRef = new(Assets.Request<Effect>("Effects/HarvestMoonShader", AssetRequestMode.ImmediateLoad).Value);
                Ref<Effect> mintMoonRef = new(Assets.Request<Effect>("Effects/MintMoonShader", AssetRequestMode.ImmediateLoad).Value);

                Filters.Scene["BlueMoonShader"] = new Filter(new ScreenShaderData(blueMoonRef, "BlueMoonPass"), EffectPriority.VeryHigh);
                Filters.Scene["CherryMoonShader"] = new Filter(new ScreenShaderData(cherryMoonRef, "CherryMoonPass"), EffectPriority.VeryHigh);
                Filters.Scene["HarvestMoonShader"] = new Filter(new ScreenShaderData(harvestMoonRef, "HarvestMoonPass"), EffectPriority.VeryHigh);
                Filters.Scene["MintMoonShader"] = new Filter(new ScreenShaderData(mintMoonRef, "MintMoonPass"), EffectPriority.VeryHigh);

                Filters.Scene["BlueMoonShader"].Load();
                Filters.Scene["CherryMoonShader"].Load();
                Filters.Scene["HarvestMoonShader"].Load();
                Filters.Scene["MintMoonShader"].Load();
            }

            private void HandleMoonStatus(MoonID moonId, bool isStarting)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient) return;

                switch (moonId)
                {
                    case MoonID.Blue:
                        BlueMoonEvent.blueMoon = isStarting;
                        BlueMoonEvent.ApplyClientEffects(isStarting);
                        break;
                    case MoonID.Cherry:
                        CherryMoonEvent.cherryMoon = isStarting;
                        CherryMoonEvent.ApplyClientEffects(isStarting);
                        break;
                    case MoonID.Harvest:
                        HarvestMoonEvent.harvestMoon = isStarting;
                        HarvestMoonEvent.ApplyClientEffects(isStarting);
                        break;
                    case MoonID.Mint:
                        MintMoonEvent.mintMoon = isStarting;
                        MintMoonEvent.ApplyClientEffects(isStarting);
                        break;
                }
            }
        }
    }
}