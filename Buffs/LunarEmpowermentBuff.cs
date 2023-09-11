using BlueMoon.Events;
using Terraria;
using Terraria.ModLoader;

namespace BlueMoon.Buffs
{
    public class LunarEmpowermentBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
              // DisplayName.SetDefault("Lunar Empowerment");
              // Description.SetDefault("The moon bestows some of its luck onto you.");
            Main.buffNoSave[Type] = false;
            Main.buffNoTimeDisplay[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<BlueMoonPlayer>().lunarEmpowerment = true;
        }
    }
}
