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
using Microsoft.Xna.Framework;
using System;
using BlueMoon;

namespace BlueMoon.Events
{
    public class MintMoonEvent : ModSystem
    {
        public static bool mintMoon;
        public static int prevNightCount;
        private static float shaderOpacity = 0f;
        private const int FadeDurationTicks = 120;
        private static bool wasActiveLastFrame = false;

        public override void PostUpdateWorld()
        {
            BlueMoonConfig config = ModContent.GetInstance<BlueMoonConfig>();
            if (!Main.dayTime)
            {
                int currNightCount = (int)(Main.dayRate / 2 + Main.moonPhase);

                if (currNightCount != prevNightCount)
                {
                    prevNightCount = currNightCount;

                    if (config.EnableMintMoonSpawn && 
                        Main.rand.NextBool(3) &&
                        Main.moonPhase == 2 &&
                        !mintMoon && 
                        !BlueMoonEvent.blueMoon && 
                        !CherryMoonEvent.cherryMoon && 
                        !HarvestMoonEvent.harvestMoon && 
                        !Main.bloodMoon)
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
            if (Main.netMode == NetmodeID.MultiplayerClient) return;

            mintMoon = true;
            Main.moonType = 5;
            Main.moonPhase = 2;
            Main.waterStyle = 4;

            if (Main.netMode == NetmodeID.Server)
            {
                NetMessage.SendData(MessageID.WorldData);
            }

            MoonNetworking.SendMoonStatus(MoonID.Mint, true);

            if (Main.netMode == NetmodeID.SinglePlayer)
            {
                Main.NewText("The Mint Moon is rising...", 152, 251, 152);
                if (Main.LocalPlayer.whoAmI == Main.myPlayer)
                {
                    Main.LocalPlayer.AddBuff(ModContent.BuffType<MintyFreshnessBuff>(), 60 * 60 * 9);
                }
            }
            else if (Main.netMode == NetmodeID.Server)
            {
                ChatHelper.BroadcastChatMessage(NetworkText.FromLiteral("The Mint Moon is rising..."), new Color(152, 251, 152));
            }
        }

        public static void EndMintMoon()
        {
            if (Main.netMode == NetmodeID.MultiplayerClient) return;

            mintMoon = false;
            Main.moonType = 0;

            if (Main.netMode == NetmodeID.Server)
            {
                NetMessage.SendData(MessageID.WorldData);
            }

            MoonNetworking.SendMoonStatus(MoonID.Mint, false);

            if (Main.netMode == NetmodeID.SinglePlayer)
            {
                Main.NewText("The Mint Moon has set...", 152, 251, 152);
                if (Main.LocalPlayer.HasBuff(ModContent.BuffType<MintyFreshnessBuff>()))
                {
                    int buffIndex = Main.LocalPlayer.FindBuffIndex(ModContent.BuffType<MintyFreshnessBuff>());
                    if (buffIndex != -1) Main.LocalPlayer.DelBuff(buffIndex);
                }
            }
            else if (Main.netMode == NetmodeID.Server)
            {
                ChatHelper.BroadcastChatMessage(NetworkText.FromLiteral("The Mint Moon has set..."), new Color(152, 251, 152));
            }
        }

        public static void ApplyClientEffects(bool start)
        {
            if (Main.netMode == NetmodeID.Server) return;

            mintMoon = start;

            try
            {
                if (start)
                {
                    if (Main.LocalPlayer.whoAmI == Main.myPlayer)
                    {
                        Main.LocalPlayer.AddBuff(ModContent.BuffType<MintyFreshnessBuff>(), 60 * 60 * 9);
                    }
                }
                else
                {
                    if (Main.LocalPlayer.HasBuff(ModContent.BuffType<MintyFreshnessBuff>()))
                    {
                        int buffIndex = Main.LocalPlayer.FindBuffIndex(ModContent.BuffType<MintyFreshnessBuff>());
                        if (buffIndex != -1) Main.LocalPlayer.DelBuff(buffIndex);
                    }
                }
            }
            catch (Exception ex)
            {
                ModContent.GetInstance<BlueMoon.BlueMoonMod>().Logger.Error("Error applying Mint Moon client effects state: " + ex);
                Main.NewText("Error applying Mint Moon effects state: " + ex.Message, 255, 50, 50);
            }
        }

        public override void PostUpdateEverything()
        {
            if (Main.netMode == NetmodeID.Server) return;

            bool playerIsSurface = Main.LocalPlayer.position.Y / 16f < Main.worldSurface;

            bool shouldBeActive = mintMoon && playerIsSurface;
            float targetOpacity = shouldBeActive ? 1f : 0f;
            float fadeStep = 1f / FadeDurationTicks;

            if (shaderOpacity < targetOpacity)
            {
                shaderOpacity = Math.Min(targetOpacity, shaderOpacity + fadeStep);
            }
            else if (shaderOpacity > targetOpacity)
            {
                shaderOpacity = Math.Max(targetOpacity, shaderOpacity - fadeStep);
            }

            bool isActiveNow = shaderOpacity > 0f;
            if (isActiveNow != wasActiveLastFrame)
            {
                try {
                    if (isActiveNow)
                    {
                        if (!Filters.Scene["MintMoonShader"].IsActive())
                            Filters.Scene.Activate("MintMoonShader");
                    }
                    else
                    {
                        if (Filters.Scene["MintMoonShader"].IsActive())
                            Filters.Scene.Deactivate("MintMoonShader");
                    }
                }
                catch (Exception ex)
                {
                    ModContent.GetInstance<BlueMoon.BlueMoonMod>().Logger.Error("Error toggling Mint Moon shader: " + ex);
                }
                wasActiveLastFrame = isActiveNow;
            }

            if (isActiveNow || shaderOpacity > 0)
            {
                try
                {
                    Filters.Scene["MintMoonShader"].GetShader().UseOpacity(shaderOpacity);
                }
                catch (Exception ex)
                {
                    if(isActiveNow)
                        ModContent.GetInstance<BlueMoon.BlueMoonMod>().Logger.Warn("Could not apply opacity to Mint Moon shader: " + ex);
                }
            }
        }

        public override void SaveWorldData(TagCompound tag)
        {
            tag.Set("mintMoon", mintMoon);
        }

        public override void LoadWorldData(TagCompound tag)
        {
            mintMoon = tag.GetBool("mintMoon");
            if (Main.netMode != NetmodeID.Server)
            {
                if (mintMoon) {
                    shaderOpacity = 1f;
                    wasActiveLastFrame = true;
                    try {
                        Filters.Scene.Activate("MintMoonShader");
                        Filters.Scene["MintMoonShader"].GetShader().UseOpacity(shaderOpacity);
                        if (Main.LocalPlayer.whoAmI == Main.myPlayer)
                        {
                            Main.LocalPlayer.AddBuff(ModContent.BuffType<MintyFreshnessBuff>(), 60 * 60 * 9);
                        }
                    } catch (Exception ex) {
                        ModContent.GetInstance<BlueMoon.BlueMoonMod>().Logger.Error("Error applying Mint Moon effects on load: " + ex);
                    }
                } else {
                    shaderOpacity = 0f;
                    wasActiveLastFrame = false;
                }
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
