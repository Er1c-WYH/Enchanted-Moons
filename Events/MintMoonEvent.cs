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

                    if (config.EnableMintMoonSpawn && Main.rand.NextBool(9) && !mintMoon && !BlueMoonEvent.blueMoon && !CherryMoonEvent.cherryMoon && !HarvestMoonEvent.harvestMoon && Main.bloodMoon)
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
                if (MintMoonEvent.mintMoon && config.DecreaseMintMoonSpawnRate)
                {
                    spawnRate = (int)(spawnRate * 1.5f);
                    maxSpawns = (int)(maxSpawns * 1.5f);
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

            // Apply any buffs to the player if needed
            if (Main.LocalPlayer.whoAmI == Main.myPlayer)
            {
                Main.LocalPlayer.AddBuff(ModContent.BuffType<MintyFreshnessBuff>(), 60 * 60 * 9);
            }

            // Display a message to players
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

            // Remove any buffs from the player if needed
            if (Main.LocalPlayer.HasBuff(ModContent.BuffType<MintyFreshnessBuff>()))
            {
                int buffIndex = Main.LocalPlayer.FindBuffIndex(ModContent.BuffType<MintyFreshnessBuff>());
                if (buffIndex != -1)
                {
                    Main.LocalPlayer.DelBuff(buffIndex);
                }
            }

            // Display a message to players
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
