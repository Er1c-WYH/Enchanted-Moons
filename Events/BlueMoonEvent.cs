using BlueMoon.Buffs;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Chat;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace BlueMoon.Events
{
    public class BlueMoonEvent : ModSystem
    {
        public static bool blueMoon;
        public static int prevNightCount;

        public override void PostUpdateWorld()
        {
            BlueMoonConfig config = ModContent.GetInstance<BlueMoonConfig>();

            if (!Main.dayTime)
            {
                int currNightCount = (int)(Main.dayRate / 2 + Main.moonPhase);

                if (currNightCount != prevNightCount)
                {
                    prevNightCount = currNightCount;

                    if (config.EnableBlueMoonSpawn && Main.rand.NextBool(9) && !blueMoon && !CherryMoonEvent.cherryMoon && !HarvestMoonEvent.harvestMoon && Main.bloodMoon)
                    {
                        StartBlueMoon();
                    }
                }
            }

            if ((Main.dayTime || Main.bloodMoon || CherryMoonEvent.cherryMoon || HarvestMoonEvent.harvestMoon) && blueMoon)
            {
                EndBlueMoon();
            }
        }


        public class BlueMoonGlobalNPC : GlobalNPC
        {
            public override void EditSpawnRate(Player player, ref int spawnRate, ref int maxSpawns)
            {
                BlueMoonConfig config = ModContent.GetInstance<BlueMoonConfig>();
                if (BlueMoonEvent.blueMoon && config.DecreaseBlueMoonSpawnRate)
                {
                    spawnRate = (int)(spawnRate * 1.5f);
                    maxSpawns = (int)(maxSpawns * 1.5f);

                }
            }
        }

        public static void StartBlueMoon()
        {
            blueMoon = true;
            Filters.Scene.Activate("BlueMoonShader");
            Main.moonPhase = 0;
            Main.moonType = 4;
            Main.waterStyle = 12;

            if (Main.LocalPlayer.whoAmI == Main.myPlayer)
            {
                Main.LocalPlayer.AddBuff(ModContent.BuffType<LunarEmpowermentBuff>(), 60 * 60 * 9);
            }

            if (Main.netMode == NetmodeID.SinglePlayer)
            {
                Main.NewText("The Blue Moon is rising...", 50, 125, 255);
            }
            else if (Main.netMode == NetmodeID.Server)
            {
                ChatHelper.BroadcastChatMessage(NetworkText.FromLiteral("The Blue Moon is rising..."), new Microsoft.Xna.Framework.Color(50, 125, 255));
            }
        }

        public static void EndBlueMoon()
        {
            blueMoon = false;
            Filters.Scene.Deactivate("BlueMoonShader");
            Main.moonType = 0;
            Main.waterStyle = 0;

            
            if (Main.LocalPlayer.HasBuff(ModContent.BuffType<LunarEmpowermentBuff>()))
            {
                int buffIndex = Main.LocalPlayer.FindBuffIndex(ModContent.BuffType<LunarEmpowermentBuff>());
                if (buffIndex != -1)
                {
                    Main.LocalPlayer.DelBuff(buffIndex);
                }
            }

            if (Main.netMode == NetmodeID.SinglePlayer)
            {
                Main.NewText("The Blue Moon has set...", 50, 125, 255);
            }
            else if (Main.netMode == NetmodeID.Server)
            {
                ChatHelper.BroadcastChatMessage(NetworkText.FromLiteral("The Blue Moon has set..."), new Microsoft.Xna.Framework.Color(50, 125, 255));
            }
        }

        public override void SaveWorldData(TagCompound tag)
        {
            tag.Set("blueMoon", blueMoon);
        }

        public override void LoadWorldData(TagCompound tag)
        {
            blueMoon = tag.GetBool("blueMoon");
            if (blueMoon)
            {
                Filters.Scene.Activate("BlueMoonShader");
            }
        }
    }

    public class BlueMoonPlayer : ModPlayer
    {
        public bool lunarEmpowerment;

        public override void ResetEffects()
        {
            lunarEmpowerment = false;
        }

        public override void PostUpdate()
        {
            if (BlueMoonEvent.blueMoon)
            {
                if (Main.hardMode)
                {
                    Player.luck += 0.50f;
                }
                else
                {
                    Player.luck += 0.25f;
                }
            }
        }

        public class BlueMoonMod : ModSceneEffect
        {
            public override int Music => MusicLoader.GetMusicSlot(Mod, "Music/BlueMoon");
            public override SceneEffectPriority Priority => SceneEffectPriority.Event;
            public override bool IsSceneEffectActive(Player player) => BlueMoonEvent.blueMoon && player.position.Y / 16f < Main.worldSurface;
        }
    }
}
