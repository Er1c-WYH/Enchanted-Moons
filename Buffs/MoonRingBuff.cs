using Terraria;
using Terraria.ModLoader;

namespace BlueMoon.Buffs
{
    public class MoonsRingBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoTimeDisplay[Type] = true;
            Main.buffNoSave[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.AddBuff(ModContent.BuffType<Buffs.FloralBlessingBuff>(), 2);
            player.AddBuff(ModContent.BuffType<Buffs.MintyFreshnessBuff>(), 2);
            player.AddBuff(ModContent.BuffType<Buffs.LunarEmpowermentBuff>(), 2);
            player.AddBuff(ModContent.BuffType<Buffs.BountifulHarvestBuff>(), 2);
        }
    }
}