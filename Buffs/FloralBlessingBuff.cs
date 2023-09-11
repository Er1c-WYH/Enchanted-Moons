using Terraria;
using Terraria.ModLoader;

namespace BlueMoon.Buffs
{
    public class FloralBlessingBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
               // DisplayName.SetDefault("Floral Blessing");
               // Description.SetDefault("Embrace the Cherry Moon's healing aura. Your wounds mend swiftly.");
            Main.buffNoSave[Type] = false;
            Main.buffNoTimeDisplay[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            int lifeRegenValue = Main.hardMode ? 3 : 2;

            player.lifeRegen += lifeRegenValue;
        }
    }
}
