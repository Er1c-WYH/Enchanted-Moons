using Terraria;
using Terraria.ModLoader;

namespace BlueMoon.Buffs
{
    public class MintyFreshnessBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Minty Freshness");
            // Description.SetDefault("The Mint Moon’s sparkling light enhances your magical abilities. You deal more damage and have lower cooldowns.");
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.moveSpeed += 0.2f;
            player.GetAttackSpeed(DamageClass.Melee) += 0.2f;
            player.GetAttackSpeed(DamageClass.Ranged) += 0.2f;
            player.GetAttackSpeed(DamageClass.Magic) += 0.2f;
            player.GetAttackSpeed(DamageClass.Summon) += 0.2f;
        }
    }
}
