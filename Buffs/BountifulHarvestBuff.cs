using BlueMoon.Buffs;
using BlueMoon.Events;
using Terraria;
using Terraria.ModLoader;

namespace BlueMoon.Buffs
{
    public class BountifulHarvestBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoSave[Type] = false;
            Main.buffNoTimeDisplay[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<HarvestMoonPlayer>().bountifulHarvest = true;
        }
    }
}

public class HarvestMoonGlobalNPCBuff : GlobalNPC
{
    public override void OnKill(NPC npc)
    {
        Player player = Main.player[npc.lastInteraction];
        if (player.HasBuff(ModContent.BuffType<BountifulHarvestBuff>()))
        {
            // increase the coin value of the NPC
            if (Main.hardMode)
            {
                npc.value *= 2f;
            }
            else
            {
                npc.value *= 1.5f;
            }
        }
    }
}