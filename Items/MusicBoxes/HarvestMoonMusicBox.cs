using Terraria.ID;
using Terraria.ModLoader;

namespace BlueMoon.Items.MusicBoxes
{
    public class HarvestMoonMusicBox : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.CanGetPrefixes[Type] = false;
            ItemID.Sets.ShimmerTransformToItem[Type] = ItemID.MusicBox;

            MusicLoader.AddMusicBox(Mod, MusicLoader.GetMusicSlot(Mod, "Music/HarvestMoon"), ModContent.ItemType<HarvestMoonMusicBox>(), ModContent.TileType<HarvestMoonMusicBoxTile>());
        }

        public override void SetDefaults()
        {
            Item.DefaultToMusicBox(ModContent.TileType<HarvestMoonMusicBoxTile>(), 0);
        }
    }
}