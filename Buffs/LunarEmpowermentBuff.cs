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

        public class LunarEmpowerment : ModPlayer
        {
            public override void ModifyLuck(ref float luck)
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
