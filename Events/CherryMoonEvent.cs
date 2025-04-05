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

namespace BlueMoon.Events
{
    public class CherryMoonEvent : ModSystem
    {
        public static bool cherryMoon;
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

                    if (config.EnableCherryMoonSpawn && 
                        Main.rand.NextBool(3) && 
                        Main.moonPhase == 7 && 
                        !cherryMoon && 
                        !BlueMoonEvent.blueMoon && 
                        !HarvestMoonEvent.harvestMoon && 
                        !MintMoonEvent.mintMoon && 
                        !Main.bloodMoon)
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
            if (Main.netMode == NetmodeID.MultiplayerClient) return;

            cherryMoon = true;
            Main.moonType = 6;
            Main.moonPhase = 7;
            Main.waterStyle = 13;

            if (Main.netMode == NetmodeID.Server)
            {
                NetMessage.SendData(MessageID.WorldData);
            }

            MoonNetworking.SendMoonStatus(MoonID.Cherry, true);

            if (Main.netMode == NetmodeID.SinglePlayer)
            {
                Main.NewText("The Cherry Moon is rising...", 255, 20, 147);
                if (Main.LocalPlayer.whoAmI == Main.myPlayer)
                {
                    Main.LocalPlayer.AddBuff(ModContent.BuffType<FloralBlessingBuff>(), 60 * 60 * 9);
                }
            }
            else if (Main.netMode == NetmodeID.Server)
            {
                ChatHelper.BroadcastChatMessage(NetworkText.FromLiteral("The Cherry Moon is rising..."), new Color(255, 20, 147));
            }
        }

        public static void EndCherryMoon()
        {
            if (Main.netMode == NetmodeID.MultiplayerClient) return;

            cherryMoon = false;
            Main.moonType = 0;

            if (Main.netMode == NetmodeID.Server)
            {
                NetMessage.SendData(MessageID.WorldData);
            }

            MoonNetworking.SendMoonStatus(MoonID.Cherry, false);

            if (Main.netMode == NetmodeID.SinglePlayer)
            {
                Main.NewText("The Cherry Moon has set...", 255, 20, 147);
                if (Main.LocalPlayer.HasBuff(ModContent.BuffType<FloralBlessingBuff>()))
                {
                    int buffIndex = Main.LocalPlayer.FindBuffIndex(ModContent.BuffType<FloralBlessingBuff>());
                    if (buffIndex != -1) Main.LocalPlayer.DelBuff(buffIndex);
                }
            }
            else if (Main.netMode == NetmodeID.Server)
            {
                ChatHelper.BroadcastChatMessage(NetworkText.FromLiteral("The Cherry Moon has set..."), new Color(255, 20, 147));
            }
        }

        public static void ApplyClientEffects(bool start)
        {
            if (Main.netMode == NetmodeID.Server) return;

            cherryMoon = start;

            try
            {
                if (start)
                {
                    if (Main.LocalPlayer.whoAmI == Main.myPlayer)
                    {
                        Main.LocalPlayer.AddBuff(ModContent.BuffType<FloralBlessingBuff>(), 60 * 60 * 9);
                    }
                }
                else
                {
                    if (Main.LocalPlayer.HasBuff(ModContent.BuffType<FloralBlessingBuff>()))
                    {
                        int buffIndex = Main.LocalPlayer.FindBuffIndex(ModContent.BuffType<FloralBlessingBuff>());
                        if (buffIndex != -1) Main.LocalPlayer.DelBuff(buffIndex);
                    }
                }
            }
            catch (Exception ex)
            {
                ModContent.GetInstance<BlueMoon.BlueMoonMod>().Logger.Error("Error applying Cherry Moon client effects state: " + ex);
                Main.NewText("Error applying Cherry Moon effects state: " + ex.Message, 255, 50, 50);
            }
        }

        public override void PostUpdateEverything()
        {
            if (Main.netMode == NetmodeID.Server) return;

            bool playerIsSurface = Main.LocalPlayer.position.Y / 16f < Main.worldSurface;

            bool shouldBeActive = cherryMoon && playerIsSurface;
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
                        if (!Filters.Scene["CherryMoonShader"].IsActive())
                            Filters.Scene.Activate("CherryMoonShader");
                    }
                    else
                    {
                        if (Filters.Scene["CherryMoonShader"].IsActive())
                            Filters.Scene.Deactivate("CherryMoonShader");
                    }
                }
                 catch (Exception ex)
                {
                    ModContent.GetInstance<BlueMoon.BlueMoonMod>().Logger.Error("Error toggling Cherry Moon shader: " + ex);
                }
                wasActiveLastFrame = isActiveNow;
            }

            if (isActiveNow || shaderOpacity > 0)
            {
                 try
                {
                    Filters.Scene["CherryMoonShader"].GetShader().UseOpacity(shaderOpacity);
                }
                 catch (Exception ex)
                {
                    if(isActiveNow)
                        ModContent.GetInstance<BlueMoon.BlueMoonMod>().Logger.Warn("Could not apply opacity to Cherry Moon shader: " + ex);
                }
            }
        }

        public override void SaveWorldData(TagCompound tag)
        {
            tag.Set("cherryMoon", cherryMoon);
        }

        public override void LoadWorldData(TagCompound tag)
        {
            cherryMoon = tag.GetBool("cherryMoon");
            if (Main.netMode != NetmodeID.Server)
            {
                 if (cherryMoon) {
                     shaderOpacity = 1f;
                     wasActiveLastFrame = true;
                     try {
                        Filters.Scene.Activate("CherryMoonShader");
                        Filters.Scene["CherryMoonShader"].GetShader().UseOpacity(shaderOpacity);
                        if (Main.LocalPlayer.whoAmI == Main.myPlayer)
                        {
                            Main.LocalPlayer.AddBuff(ModContent.BuffType<FloralBlessingBuff>(), 60 * 60 * 9);
                        }
                     } catch (Exception ex) {
                         ModContent.GetInstance<BlueMoon.BlueMoonMod>().Logger.Error("Error applying Cherry Moon effects on load: " + ex);
                     }
                 } else {
                     shaderOpacity = 0f;
                     wasActiveLastFrame = false;
                 }
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