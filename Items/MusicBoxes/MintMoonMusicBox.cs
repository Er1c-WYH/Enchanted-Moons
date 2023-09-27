/* broke for no reason 
using Terraria.ID;
using Terraria.ModLoader;

namespace BlueMoon.Items.MusicBoxes
{
    public class MintMoonMusicBox : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.CanGetPrefixes[Type] = false;
            ItemID.Sets.ShimmerTransformToItem[Type] = ItemID.MusicBox;

            MusicLoader.AddMusicBox(Mod, MusicLoader.GetMusicSlot(Mod, "Music/MintMoon"), ModContent.ItemType<MintMoonMusicBox>(), ModContent.TileType<MintMoonMusicBoxTile>());
        }

        public override void SetDefaults()
        {
            Item.DefaultToMusicBox(ModContent.TileType<MintMoonMusicBoxTile>(), 0);
        }
    }
} */