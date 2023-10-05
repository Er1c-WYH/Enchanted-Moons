using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace BlueMoon.Items.Accessories
{
    public class TopazRing : ModItem
    {

        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 28;
            Item.accessory = true;
            Item.rare = ItemRarityID.Yellow;
            Item.value = Item.sellPrice(gold: 1);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.AddBuff(ModContent.BuffType<Buffs.BountifulHarvestBuff>(), 2);
        }
    }
}
