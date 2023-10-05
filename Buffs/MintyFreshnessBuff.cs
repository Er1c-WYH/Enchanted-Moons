using Terraria;
using Terraria.ModLoader;

namespace BlueMoon.Buffs
{
    public class MintyFreshnessBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<MintyFreshnessPlayer>().hasMintyFreshness = true;
        }
    }

    public class MintyFreshnessPlayer : ModPlayer
    {
        public bool hasMintyFreshness;

        public override void ResetEffects()
        {
            hasMintyFreshness = false;
        }

        public override void PostUpdateBuffs()
        {
            if (hasMintyFreshness)
            {
                Player.moveSpeed += 0.2f;
                Player.GetDamage(DamageClass.Melee) += 0.2f;
                Player.GetDamage(DamageClass.Ranged) += 0.2f;
                Player.GetDamage(DamageClass.Magic) += 0.2f;
                Player.GetDamage(DamageClass.Summon) += 0.2f;
            }
        }
    }
}