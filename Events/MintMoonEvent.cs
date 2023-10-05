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
    public class MintMoonEvent : ModSystem
    {
        public static bool mintMoon;
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

                    if (config.EnableMintMoonSpawn && Main.rand.NextBool(9) && !mintMoon && !BlueMoonEvent.blueMoon && !CherryMoonEvent.cherryMoon && !HarvestMoonEvent.harvestMoon && !Main.bloodMoon)
                    {
                        StartMintMoon();
                    }
                }
            }

            if ((Main.dayTime || Main.bloodMoon) && mintMoon)
            {
                EndMintMoon();
            }
        }

        public class MintMoonGlobalNPC : GlobalNPC
        {
            public override void EditSpawnRate(Player player, ref int spawnRate, ref int maxSpawns)
            {
                BlueMoonConfig config = ModContent.GetInstance<BlueMoonConfig>();
                if (MintMoonEvent.mintMoon && config.IncreaseMintMoonSpawnRate)
                {
                    spawnRate = (int)(spawnRate * 0.5f);
                    maxSpawns = (int)(maxSpawns * 0.5f);
                }
            }
            public override void OnKill(NPC npc)
            {
                if (!npc.boss && !npc.friendly && !npc.townNPC)
                {
                    if (MintMoonEvent.mintMoon)
                    {
                        int dropChance = 100;
                        if (Main.rand.Next(dropChance) == 0)
                        {
                            Item.NewItem(npc.GetSource_Death(), npc.position, ModContent.ItemType<EmeraldRing>());
                        }
                    }
                }
            }
        }

        public static void StartMintMoon()
        {
            mintMoon = true;
            Main.moonPhase = 0;
            Main.moonType = 5;
            Main.waterStyle = 14;
            Filters.Scene.Activate("MintMoonShader");

            if (Main.LocalPlayer.whoAmI == Main.myPlayer)
            {
                Main.LocalPlayer.AddBuff(ModContent.BuffType<MintyFreshnessBuff>(), 60 * 60 * 9);
            }

            if (Main.netMode == NetmodeID.SinglePlayer)
            {
                Main.NewText("The Mint Moon is rising...", 0, 255, 0);
            }
            else if (Main.netMode == NetmodeID.Server)
            {
                ChatHelper.BroadcastChatMessage(NetworkText.FromLiteral("The Mint Moon is rising..."), new Microsoft.Xna.Framework.Color(0, 255, 0));
            }
        }

        public static void EndMintMoon()
        {
            mintMoon = false;
            Main.moonType = 0;
            Main.waterStyle = 0;
            Filters.Scene.Deactivate("MintMoonShader");

            if (Main.LocalPlayer.HasBuff(ModContent.BuffType<MintyFreshnessBuff>()))
            {
                int buffIndex = Main.LocalPlayer.FindBuffIndex(ModContent.BuffType<MintyFreshnessBuff>());
                if (buffIndex != -1)
                {
                    Main.LocalPlayer.DelBuff(buffIndex);
                }
            }

            if (Main.netMode == NetmodeID.SinglePlayer)
            {
                Main.NewText("The Mint Moon has set...", 0, 255, 0);
            }
            else if (Main.netMode == NetmodeID.Server)
            {
                ChatHelper.BroadcastChatMessage(NetworkText.FromLiteral("The Mint Moon has set..."), new Microsoft.Xna.Framework.Color(0, 255, 0));
            }
        }

        public override void SaveWorldData(TagCompound tag)
        {
            tag.Set("mintMoon", mintMoon);
        }

        public override void LoadWorldData(TagCompound tag)
        {
            mintMoon = tag.GetBool("mintMoon");
            if (mintMoon)
            {
                Filters.Scene.Activate("MintMoonShader");
            }
        }

        public class MintMoonMod : ModSceneEffect
        {
            public override int Music => MusicLoader.GetMusicSlot(Mod, "Music/MintMoon");
            public override SceneEffectPriority Priority => SceneEffectPriority.Event;

            public override bool IsSceneEffectActive(Player player) => MintMoonEvent.mintMoon && player.position.Y / 16f < Main.worldSurface;
        }
    }
}
