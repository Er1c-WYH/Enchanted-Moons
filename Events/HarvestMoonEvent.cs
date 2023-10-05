using BlueMoon.Buffs;
using BlueMoon.Items.Accessories;
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
    public class HarvestMoonEvent : ModSystem
    {
        public static bool harvestMoon;
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

                    if (config.EnableHarvestMoonSpawn && Main.rand.NextBool(9) && !harvestMoon && !BlueMoonEvent.blueMoon && !CherryMoonEvent.cherryMoon && !MintMoonEvent.mintMoon && !Main.bloodMoon)
                    {
                        StartHarvestMoon();
                    }
                }
            }

            if ((Main.dayTime || Main.bloodMoon) && harvestMoon)
            {
                EndHarvestMoon();
            }
        }
        public class HarvestMoonGlobalNPC : GlobalNPC
        {
            public override void EditSpawnRate(Player player, ref int spawnRate, ref int maxSpawns)
            {
                BlueMoonConfig config = ModContent.GetInstance<BlueMoonConfig>();
                if (HarvestMoonEvent.harvestMoon && config.DecreaseHarvestMoonSpawnRate)
                {
                    spawnRate = (int)(spawnRate * 1.5f);
                    maxSpawns = (int)(maxSpawns * 1.5f);

                }
            }
            public override void OnKill(NPC npc)
            {
                if (!npc.boss && !npc.friendly && !npc.townNPC)
                {
                    if (HarvestMoonEvent.harvestMoon)
                    {
                        int dropChance = 100; // Set the drop chance to 100 for the event
                        if (Main.rand.Next(dropChance) == 0)
                        {
                            Item.NewItem(npc.GetSource_Death(), npc.position, ModContent.ItemType<TopazRing>());
                        }
                    }
                }
            }
        }

            public static void StartHarvestMoon()
        {
            harvestMoon = true;
            Main.moonPhase = 0;
            Main.moonType = 7;
            Main.waterStyle = 12;
            Filters.Scene.Activate("HarvestMoonShader");

            if (Main.LocalPlayer.whoAmI == Main.myPlayer)
            {
                Main.LocalPlayer.AddBuff(ModContent.BuffType<BountifulHarvestBuff>(), 60 * 60 * 9);
            }

            if (Main.netMode == NetmodeID.SinglePlayer)
            {
                Main.NewText("The Harvest Moon is rising...", 255, 165, 0);
            }
            else if (Main.netMode == NetmodeID.Server)
            {
                ChatHelper.BroadcastChatMessage(NetworkText.FromLiteral("The Harvest Moon is rising..."), new Microsoft.Xna.Framework.Color(255, 165, 0));
            }
        }

        public static void EndHarvestMoon()
        {
            harvestMoon = false;
            Main.moonType = 0;
            Main.waterStyle = 0;
            Filters.Scene.Deactivate("HarvestMoonShader");

           
            if (Main.LocalPlayer.HasBuff(ModContent.BuffType<BountifulHarvestBuff>()))
            {
                int buffIndex = Main.LocalPlayer.FindBuffIndex(ModContent.BuffType<BountifulHarvestBuff>());
                if (buffIndex != -1)
                {
                    Main.LocalPlayer.DelBuff(buffIndex);
                }
            }

            if (Main.netMode == NetmodeID.SinglePlayer)
            {
                Main.NewText("The Harvest Moon has set...", 255, 165, 0);
            }
            else if (Main.netMode == NetmodeID.Server)
            {
                ChatHelper.BroadcastChatMessage(NetworkText.FromLiteral("The Harvest Moon has set..."), new Microsoft.Xna.Framework.Color(255, 165, 0));
            }
        }

        public override void SaveWorldData(TagCompound tag)
        {
            tag.Set("harvestMoon", harvestMoon);
        }

        public override void LoadWorldData(TagCompound tag)
        {
            harvestMoon = tag.GetBool("harvestMoon");
            if (harvestMoon)
            {
                Filters.Scene.Activate("HarvestMoonShader");
            }
        }
    }

    public class HarvestMoonPlayer : ModPlayer
    {
        public bool bountifulHarvest; 

        public override void ResetEffects()
        {
            
            bountifulHarvest = false;
        }

        public class HarvestMoonMod : ModSceneEffect
        {
            public override int Music => MusicLoader.GetMusicSlot(Mod, "Music/HarvestMoon");
            public override SceneEffectPriority Priority => SceneEffectPriority.Event;
            public override bool IsSceneEffectActive(Player player) => HarvestMoonEvent.harvestMoon && player.position.Y / 16f < Main.worldSurface;
        }
    }
}