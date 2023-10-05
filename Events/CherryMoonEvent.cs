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
    public class CherryMoonEvent : ModSystem
    {
        public static bool cherryMoon;
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

                    if (config.EnableCherryMoonSpawn && Main.rand.NextBool(9) && !cherryMoon && !BlueMoonEvent.blueMoon && !HarvestMoonEvent.harvestMoon && !MintMoonEvent.mintMoon && !Main.bloodMoon)
                    {
                        StartCherryMoon();
                    }
                }
            }

            if ((Main.dayTime || Main.bloodMoon || BlueMoonEvent.blueMoon || HarvestMoonEvent.harvestMoon) && cherryMoon)
            {
                EndCherryMoon();
            }
        }

        public class CherryMoonGlobalNPC : GlobalNPC
        {
            public override void EditSpawnRate(Player player, ref int spawnRate, ref int maxSpawns)
            {
                BlueMoonConfig config = ModContent.GetInstance<BlueMoonConfig>();
                if (CherryMoonEvent.cherryMoon && config.DecreaseCherryMoonSpawnRate)
                {
                    spawnRate = (int)(spawnRate * 1.5f);
                    maxSpawns = (int)(maxSpawns * 1.5f);

                }
            }
            public override void OnKill(NPC npc)
            {
                if (!npc.boss && !npc.friendly && !npc.townNPC)
                {
                    if (CherryMoonEvent.cherryMoon)
                    {
                        int dropChance = 100;
                        if (Main.rand.Next(dropChance) == 0)
                        {
                            Item.NewItem(npc.GetSource_Death(), npc.position, ModContent.ItemType<AmethystRing>());
                        }
                    }
                }
            }
        }


        public static void StartCherryMoon()
        {
            cherryMoon = true;
            Main.moonPhase = 0;
            Main.moonType = 6; 
            Main.waterStyle = 13;
            Filters.Scene.Activate("CherryMoonShader");

            if (Main.LocalPlayer.whoAmI == Main.myPlayer)
            {
                Main.LocalPlayer.AddBuff(ModContent.BuffType<FloralBlessingBuff>(), 60 * 60 * 9);
            }

            if (Main.netMode == NetmodeID.SinglePlayer)
            {
                Main.NewText("The Cherry Moon is rising...", 255, 20, 147); 
            }
            else if (Main.netMode == NetmodeID.Server)
            {
                ChatHelper.BroadcastChatMessage(NetworkText.FromLiteral("The Cherry Moon is rising..."), new Microsoft.Xna.Framework.Color(255, 20, 147));
            }
        }

        public static void EndCherryMoon()
        {
            cherryMoon = false;
            Main.moonType = 0;
            Main.waterStyle = 0;
            Filters.Scene.Deactivate("CherryMoonShader");

            // Remove the Floral Blessing buff from the player
            if (Main.LocalPlayer.HasBuff(ModContent.BuffType<FloralBlessingBuff>()))
            {
                int buffIndex = Main.LocalPlayer.FindBuffIndex(ModContent.BuffType<FloralBlessingBuff>());
                if (buffIndex != -1)
                {
                    Main.LocalPlayer.DelBuff(buffIndex);
                }
            }

            if (Main.netMode == NetmodeID.SinglePlayer)
            {
                Main.NewText("The Cherry Moon has set...", 255, 20, 147); 
            }
            else if (Main.netMode == NetmodeID.Server)
            {
                ChatHelper.BroadcastChatMessage(NetworkText.FromLiteral("The Cherry Moon has set..."), new Microsoft.Xna.Framework.Color(255, 20, 147)); 
            }
        }

        public override void SaveWorldData(TagCompound tag)
        {
            tag.Set("cherryMoon", cherryMoon);
        }

        public override void LoadWorldData(TagCompound tag)
        {
            cherryMoon = tag.GetBool("cherryMoon");
            if (cherryMoon)
            {
                Filters.Scene.Activate("CherryMoonShader");
            }
        }

        public class CherryMoonMod : ModSceneEffect
        {
            public override int Music => MusicLoader.GetMusicSlot(Mod, "Music/CherryMoon");
            public override SceneEffectPriority Priority => SceneEffectPriority.Event;
            public override bool IsSceneEffectActive(Player player) => CherryMoonEvent.cherryMoon && player.position.Y / 16f < Main.worldSurface;
        }
    }
}