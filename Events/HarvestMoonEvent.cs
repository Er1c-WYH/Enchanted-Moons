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
    public class HarvestMoonEvent : ModSystem
    {
        public static bool harvestMoon;
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

                    if (config.EnableHarvestMoonSpawn && 
                        Main.rand.NextBool(3) && 
                        Main.moonPhase == 3 && 
                        !harvestMoon && 
                        !BlueMoonEvent.blueMoon && 
                        !CherryMoonEvent.cherryMoon && 
                        !MintMoonEvent.mintMoon && 
                        !Main.bloodMoon)
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
                        int dropChance = 100;
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
            if (Main.netMode == NetmodeID.MultiplayerClient) return;

            harvestMoon = true;
            Main.moonType = 7;
            Main.moonPhase = 3;
            Main.waterStyle = 3;

            if (Main.netMode == NetmodeID.Server)
            {
                NetMessage.SendData(MessageID.WorldData);
            }

            MoonNetworking.SendMoonStatus(MoonID.Harvest, true);

            if (Main.netMode == NetmodeID.SinglePlayer)
            {
                Main.NewText("The Harvest Moon is rising...", 255, 165, 0);
                if (Main.LocalPlayer.whoAmI == Main.myPlayer)
                {
                    Main.LocalPlayer.AddBuff(ModContent.BuffType<BountifulHarvestBuff>(), 60 * 60 * 9);
                }
            }
            else if (Main.netMode == NetmodeID.Server)
            {
                ChatHelper.BroadcastChatMessage(NetworkText.FromLiteral("The Harvest Moon is rising..."), new Color(255, 165, 0));
            }
        }

        public static void EndHarvestMoon()
        {
            if (Main.netMode == NetmodeID.MultiplayerClient) return;

            harvestMoon = false;
            Main.moonType = 0;

            if (Main.netMode == NetmodeID.Server)
            {
                NetMessage.SendData(MessageID.WorldData);
            }

            MoonNetworking.SendMoonStatus(MoonID.Harvest, false);

            if (Main.netMode == NetmodeID.SinglePlayer)
            {
                Main.NewText("The Harvest Moon has set...", 255, 165, 0);
                if (Main.LocalPlayer.HasBuff(ModContent.BuffType<BountifulHarvestBuff>()))
                {
                    int buffIndex = Main.LocalPlayer.FindBuffIndex(ModContent.BuffType<BountifulHarvestBuff>());
                    if (buffIndex != -1) Main.LocalPlayer.DelBuff(buffIndex);
                }
            }
            else if (Main.netMode == NetmodeID.Server)
            {
                ChatHelper.BroadcastChatMessage(NetworkText.FromLiteral("The Harvest Moon has set..."), new Color(255, 165, 0));
            }
        }

        public static void ApplyClientEffects(bool start)
        {
            if (Main.netMode == NetmodeID.Server) return;

            harvestMoon = start;

            try
            {
                if (start)
                {
                    if (Main.LocalPlayer.whoAmI == Main.myPlayer)
                    {
                        Main.LocalPlayer.AddBuff(ModContent.BuffType<BountifulHarvestBuff>(), 60 * 60 * 9);
                    }
                }
                else
                {
                    if (Main.LocalPlayer.HasBuff(ModContent.BuffType<BountifulHarvestBuff>()))
                    {
                        int buffIndex = Main.LocalPlayer.FindBuffIndex(ModContent.BuffType<BountifulHarvestBuff>());
                        if (buffIndex != -1) Main.LocalPlayer.DelBuff(buffIndex);
                    }
                }
            }
            catch (Exception ex)
            {
                ModContent.GetInstance<BlueMoon.BlueMoonMod>().Logger.Error("Error applying Harvest Moon client effects state: " + ex);
                Main.NewText("Error applying Harvest Moon effects state: " + ex.Message, 255, 50, 50);
            }
        }

        public override void PostUpdateEverything()
        {
            if (Main.netMode == NetmodeID.Server) return;

            bool playerIsSurface = Main.LocalPlayer.position.Y / 16f < Main.worldSurface;

            bool shouldBeActive = harvestMoon && playerIsSurface;
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
                        if (!Filters.Scene["HarvestMoonShader"].IsActive())
                            Filters.Scene.Activate("HarvestMoonShader");
                    }
                    else
                    {
                        if (Filters.Scene["HarvestMoonShader"].IsActive())
                            Filters.Scene.Deactivate("HarvestMoonShader");
                    }
                }
                catch (Exception ex)
                {
                    ModContent.GetInstance<BlueMoon.BlueMoonMod>().Logger.Error("Error toggling Harvest Moon shader: " + ex);
                }
                wasActiveLastFrame = isActiveNow;
            }

            if (isActiveNow || shaderOpacity > 0)
            {
                try
                {
                    Filters.Scene["HarvestMoonShader"].GetShader().UseOpacity(shaderOpacity);
                }
                catch (Exception ex)
                {
                    if(isActiveNow)
                        ModContent.GetInstance<BlueMoon.BlueMoonMod>().Logger.Warn("Could not apply opacity to Harvest Moon shader: " + ex);
                }
            }
        }

        public override void SaveWorldData(TagCompound tag)
        {
            tag.Set("harvestMoon", harvestMoon);
        }

        public override void LoadWorldData(TagCompound tag)
        {
            harvestMoon = tag.GetBool("harvestMoon");
            if (Main.netMode != NetmodeID.Server)
            {
                if (harvestMoon) {
                    shaderOpacity = 1f;
                    wasActiveLastFrame = true;
                    try {
                        Filters.Scene.Activate("HarvestMoonShader");
                        Filters.Scene["HarvestMoonShader"].GetShader().UseOpacity(shaderOpacity);
                        if (Main.LocalPlayer.whoAmI == Main.myPlayer)
                        {
                            Main.LocalPlayer.AddBuff(ModContent.BuffType<BountifulHarvestBuff>(), 60 * 60 * 9);
                        }
                    } catch (Exception ex) {
                        ModContent.GetInstance<BlueMoon.BlueMoonMod>().Logger.Error("Error applying Harvest Moon effects on load: " + ex);
                    }
                } else {
                    shaderOpacity = 0f;
                    wasActiveLastFrame = false;
                }
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