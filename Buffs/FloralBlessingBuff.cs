using Terraria;
using Terraria.ModLoader;

namespace BlueMoon.Buffs
{
    public class FloralBlessingBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoSave[Type] = false;
            Main.buffNoTimeDisplay[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<FloralBlessingPlayer>().hasFloralBlessing = true;
        }
    }

    public class FloralBlessingPlayer : ModPlayer
    {
        public bool hasFloralBlessing;

        public override void ResetEffects()
        {
            hasFloralBlessing = false;
        }

        public override void PostUpdate()
        {
            if (hasFloralBlessing)
            {
                int lifeRegenValue = Main.hardMode ? 3 : 2;
                Player.lifeRegen += lifeRegenValue;
            }
        }
    }
}