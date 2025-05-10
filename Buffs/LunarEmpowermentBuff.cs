using Terraria;
using Terraria.ModLoader;

namespace BlueMoon.Buffs
{
    public class LunarEmpowermentBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoSave[Type] = false;
            Main.buffNoTimeDisplay[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<LunarEmpowermentPlayer>().lunarEmpowerment = true;
        }
    }

    public class LunarEmpowermentPlayer : ModPlayer
    {
        public bool lunarEmpowerment;

        public override void ResetEffects()
        {
            lunarEmpowerment = false;
        }

        public override void ModifyLuck(ref float luck)
        {
            if (lunarEmpowerment)
            {
                if (Main.hardMode)
                {
                    luck += 0.50f;
                }
                else
                {
                    luck += 0.25f;
                }
            }
        }
    }
}
