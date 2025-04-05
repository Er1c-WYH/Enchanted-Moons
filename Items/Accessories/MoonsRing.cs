using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace BlueMoon.Items.Accessories
{
    public class MoonsRing : ModItem
    {

        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 28;
            Item.accessory = true;
            Item.rare = ItemRarityID.Pink;
            Item.value = Item.sellPrice(gold: 5);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.AddBuff(ModContent.BuffType<Buffs.MoonsRingBuff>(), 2);
        }
        public override void AddRecipes()
        {
            CreateRecipe(1)
                .AddIngredient(ModContent.ItemType<AmethystRing>())
                .AddIngredient(ModContent.ItemType<EmeraldRing>())
                .AddIngredient(ModContent.ItemType<SapphireRing>())
                .AddIngredient(ModContent.ItemType<TopazRing>())
                .AddIngredient(ItemID.Obsidian, 10)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}