using BlueMoon.Events;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace BlueMoon.Items
{
    public class MintRing : ModItem
    {
        public override void SetStaticDefaults()
        {
        //    DisplayName.SetDefault("Mint Ring");
        //    Tooltip.SetDefault("Summons a refreshing Mint Moon at night");
        }

        public override void SetDefaults()
        {
            Item.rare = ItemRarityID.Green;
            Item.value = Item.sellPrice(silver: 50);
            Item.maxStack = 20;

            Item.width = 20;
            Item.height = 20;
            Item.useTime = 45;
            Item.useAnimation = 45;
            Item.useStyle = ItemUseStyleID.HoldUp;

            Item.consumable = true;
            Item.noMelee = true;

            Item.UseSound = SoundID.Item44;
        }

        public override void AddRecipes()
        {
            CreateRecipe(1)
                .AddIngredient(ItemID.Emerald, 1)
                .AddRecipeGroup(RecipeGroupID.IronBar, 5)
                .AddTile(TileID.Anvils)
                .Register();
        }

        public override bool CanUseItem(Player player)
        {
            return !Main.dayTime && !MintMoonEvent.mintMoon && !BlueMoonEvent.blueMoon && !CherryMoonEvent.cherryMoon && !HarvestMoonEvent.harvestMoon && !Main.pumpkinMoon && !Main.snowMoon;
        }

        public override bool? UseItem(Player player)
        {
            MintMoonEvent.StartMintMoon();
            return true;
        }
    }
}
