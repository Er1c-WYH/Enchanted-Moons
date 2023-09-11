using Terraria.ID;
using Terraria.ModLoader;

namespace BlueMoon.Items.MusicBoxes
{
    public class CherryMoonMusicBox : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.CanGetPrefixes[Type] = false;
            ItemID.Sets.ShimmerTransformToItem[Type] = ItemID.MusicBox;

            MusicLoader.AddMusicBox(Mod, MusicLoader.GetMusicSlot(Mod, "Music/CherryMoon"), ModContent.ItemType<CherryMoonMusicBox>(), ModContent.TileType<CherryMoonMusicBoxTile>());
        }

        public override void SetDefaults()
        {
            Item.DefaultToMusicBox(ModContent.TileType<CherryMoonMusicBoxTile>(), 0);
        }
    }
}