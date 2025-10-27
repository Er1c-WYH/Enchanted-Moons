using BlueMoon.Buffs;
using BlueMoon.Items.Accessories;
using Terraria;
using Terraria.Chat;
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Microsoft.Xna.Framework;
using System;
using BlueMoon;

namespace BlueMoon.Events
{
    public class BlueMoonEvent : ModSystem
    {
        public static bool blueMoon;
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

                    if (config.EnableBlueMoonSpawn && 
                        Main.rand.NextBool(3) && 
                        Main.moonPhase == 0 && 
                        !blueMoon && 
                        !CherryMoonEvent.cherryMoon && 
                        !HarvestMoonEvent.harvestMoon && 
                        !MintMoonEvent.mintMoon && 
                        !Main.bloodMoon)
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
            public override void OnKill(NPC npc)
            {
                if (!npc.boss && !npc.friendly && !npc.townNPC)
                {
                    if (BlueMoonEvent.blueMoon)
                    {
                        int dropChance = 100;
                        if (Main.rand.Next(dropChance) == 0)
                        {
                            Item.NewItem(npc.GetSource_Death(), npc.position, ModContent.ItemType<SapphireRing>());
                        }
                    }
                }
            }
        }

        public static void StartBlueMoon()
        {
            if (Main.netMode == NetmodeID.MultiplayerClient) return;

            blueMoon = true;
            Main.moonType = 4;
            Main.moonPhase = 0;
            Main.waterStyle = 2;

            if (Main.netMode == NetmodeID.Server)
            {
                NetMessage.SendData(MessageID.WorldData);
            }

            MoonNetworking.SendMoonStatus(MoonID.Blue, true);

            // StartBlueMoon()
            if (Main.netMode == NetmodeID.SinglePlayer)
            {
                var msg = Language.GetTextValue("Mods.BlueMoon.Events.BlueMoonEvent.Rising");
                Main.NewText(msg, 50, 125, 255);
                if (Main.LocalPlayer.whoAmI == Main.myPlayer)
                    Main.LocalPlayer.AddBuff(ModContent.BuffType<LunarEmpowermentBuff>(), 60 * 60 * 9);
            }
            else if (Main.netMode == NetmodeID.Server)
            {
                // 用 FromKey 让每位玩家按各自语言显示
                ChatHelper.BroadcastChatMessage(
                    NetworkText.FromKey("Mods.BlueMoon.Events.BlueMoonEvent.Rising"),
                    new Color(50, 125, 255)
                );
            }
        }

        public static void EndBlueMoon()
        {
            if (Main.netMode == NetmodeID.MultiplayerClient) return;

            blueMoon = false;
            Main.moonType = 0;

            if (Main.netMode == NetmodeID.Server)
            {
                NetMessage.SendData(MessageID.WorldData);
            }

            MoonNetworking.SendMoonStatus(MoonID.Blue, false);

            // EndBlueMoon()
            if (Main.netMode == NetmodeID.SinglePlayer)
            {
                var msg = Language.GetTextValue("Mods.BlueMoon.Events.BlueMoonEvent.Set");
                Main.NewText(msg, 50, 125, 255);
                if (Main.LocalPlayer.HasBuff(ModContent.BuffType<LunarEmpowermentBuff>()))
                {
                    int buffIndex = Main.LocalPlayer.FindBuffIndex(ModContent.BuffType<LunarEmpowermentBuff>());
                    if (buffIndex != -1) Main.LocalPlayer.DelBuff(buffIndex);
                }
            }
            else if (Main.netMode == NetmodeID.Server)
            {
                ChatHelper.BroadcastChatMessage(
                    NetworkText.FromKey("Mods.BlueMoon.Events.BlueMoonEvent.Set"),
                    new Color(50, 125, 255)
                );
            }
        }

        public static void ApplyClientEffects(bool start)
        {
            if (Main.netMode == NetmodeID.Server) return;

            blueMoon = start;

            try
            {
                if (start)
                {
                    if (Main.LocalPlayer.whoAmI == Main.myPlayer)
                    {
                        Main.LocalPlayer.AddBuff(ModContent.BuffType<LunarEmpowermentBuff>(), 60 * 60 * 9);
                    }
                }
                else
                {
                    if (Main.LocalPlayer.HasBuff(ModContent.BuffType<LunarEmpowermentBuff>()))
                    {
                        int buffIndex = Main.LocalPlayer.FindBuffIndex(ModContent.BuffType<LunarEmpowermentBuff>());
                        if (buffIndex != -1) Main.LocalPlayer.DelBuff(buffIndex);
                    }
                }
            }
            catch (Exception ex)
            {
                ModContent.GetInstance<BlueMoon.BlueMoonMod>().Logger.Error("Error applying Blue Moon client effects state: " + ex);
                Main.NewText("Error applying Blue Moon effects state: " + ex.Message, 255, 50, 50);
            }
        }

        public override void PostUpdateEverything()
        {
            if (Main.netMode == NetmodeID.Server) return;

            bool playerIsSurface = Main.LocalPlayer.position.Y / 16f < Main.worldSurface;

            bool shouldBeActive = blueMoon && playerIsSurface;
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
                        if (!Filters.Scene["BlueMoonShader"].IsActive())
                            Filters.Scene.Activate("BlueMoonShader");
                    }
                    else
                    {
                        if (Filters.Scene["BlueMoonShader"].IsActive())
                            Filters.Scene.Deactivate("BlueMoonShader");
                    }
                }
                catch (Exception ex)
                {
                    ModContent.GetInstance<BlueMoon.BlueMoonMod>().Logger.Error("Error toggling Blue Moon shader: " + ex);
                }
                wasActiveLastFrame = isActiveNow;
            }

            if (isActiveNow || shaderOpacity > 0)
            {
                try
                {
                    Filters.Scene["BlueMoonShader"].GetShader().UseOpacity(shaderOpacity);
                }
                catch (Exception ex)
                {
                    if(isActiveNow)
                        ModContent.GetInstance<BlueMoon.BlueMoonMod>().Logger.Warn("Could not apply opacity to Blue Moon shader: " + ex);
                }
            }
        }

        public override void SaveWorldData(TagCompound tag)
        {
            tag.Set("blueMoon", blueMoon);
        }

        public override void LoadWorldData(TagCompound tag)
        {
            blueMoon = tag.GetBool("blueMoon");
            if (Main.netMode != NetmodeID.Server)
            {
                if (blueMoon) {
                    shaderOpacity = 1f;
                    wasActiveLastFrame = true;
                    try {
                        Filters.Scene.Activate("BlueMoonShader");
                        Filters.Scene["BlueMoonShader"].GetShader().UseOpacity(shaderOpacity);
                        if (Main.LocalPlayer.whoAmI == Main.myPlayer)
                        {
                            Main.LocalPlayer.AddBuff(ModContent.BuffType<LunarEmpowermentBuff>(), 60 * 60 * 9);
                        }
                    } catch (Exception ex) {
                        ModContent.GetInstance<BlueMoon.BlueMoonMod>().Logger.Error("Error applying Blue Moon effects on load: " + ex);
                    }
                } else {
                    shaderOpacity = 0f;
                    wasActiveLastFrame = false;
                }
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

        public class BlueMoonMod : ModSceneEffect
        {
            public override int Music => MusicLoader.GetMusicSlot(Mod, "Music/BlueMoon");
            public override SceneEffectPriority Priority => SceneEffectPriority.Event;
            public override bool IsSceneEffectActive(Player player) => BlueMoonEvent.blueMoon && player.position.Y / 16f < Main.worldSurface;
        }
    }
}
