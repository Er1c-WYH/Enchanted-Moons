using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.ComponentModel;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;

namespace BlueMoon
{
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
        public bool DecreaseMintMoonSpawnRate { get; set; }
    }

    internal class BlueMoon
    {
        public class Main : Mod
        {
            public override void Load()
            {
                ModContent.GetInstance<BlueMoonConfig>();

                Filters.Scene["BlueMoonShader"] = new Filter(new ScreenShaderData(new Ref<Effect>(ModContent.Request<Effect>("BlueMoon/Effects/BlueMoonShader", AssetRequestMode.ImmediateLoad).Value), "BlueMoonShader"), EffectPriority.VeryHigh);
                Filters.Scene["CherryMoonShader"] = new Filter(new ScreenShaderData(new Ref<Effect>(ModContent.Request<Effect>("BlueMoon/Effects/CherryMoonShader", AssetRequestMode.ImmediateLoad).Value), "CherryMoonShader"), EffectPriority.VeryHigh);
                Filters.Scene["HarvestMoonShader"] = new Filter(new ScreenShaderData(new Ref<Effect>(ModContent.Request<Effect>("BlueMoon/Effects/HarvestMoonShader", AssetRequestMode.ImmediateLoad).Value), "HarvestMoonShader"), EffectPriority.VeryHigh);
                Filters.Scene["MintMoonShader"] = new Filter(new ScreenShaderData(new Ref<Effect>(ModContent.Request<Effect>("BlueMoon/Effects/MintMoonShader", AssetRequestMode.ImmediateLoad).Value), "MintMoonShader"), EffectPriority.VeryHigh);

                Filters.Scene["BlueMoonShader"].Load();
                Filters.Scene["CherryMoonShader"].Load();
                Filters.Scene["HarvestMoonShader"].Load();
                Filters.Scene["MintMoonShader"].Load();
            }
        }
    }
}

