using Terraria.ID;
using Terraria.ModLoader;

namespace BlueMoon.Items.MusicBoxes
{
    public class BlueMoonMusicBox : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.CanGetPrefixes[Type] = false;
            ItemID.Sets.ShimmerTransformToItem[Type] = ItemID.MusicBox;

            MusicLoader.AddMusicBox(Mod, MusicLoader.GetMusicSlot(Mod, "Music/BlueMoon"), ModContent.ItemType<BlueMoonMusicBox>(), ModContent.TileType<BlueMoonMusicBoxTile>());
        }

        public override void SetDefaults()
        {
            Item.DefaultToMusicBox(ModContent.TileType<BlueMoonMusicBoxTile>(), 0);
        }
    }
}