using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using BlueMoon;

namespace BlueMoon.Events
{
    public enum MoonID : byte
    {
        Blue = 1,
        Cherry = 2,
        Harvest = 3,
        Mint = 4
    }

    public enum MoonMessageType : byte
    {
        MoonStatus = 1,
        RequestMoonStatus = 2
    }

    public static class MoonNetworking
    {
        public static void SendMoonStatus(MoonID moonId, bool isStarting, int toClient = -1, int ignoreClient = -1)
        {
            if (Main.netMode == NetmodeID.SinglePlayer) return;

            ModPacket packet = ModContent.GetInstance<BlueMoon.BlueMoonMod>().GetPacket();
            packet.Write((byte)MoonMessageType.MoonStatus);
            packet.Write((byte)moonId);
            packet.Write(isStarting);
            packet.Send(toClient, ignoreClient);
        }

        public static void RequestMoonStatus()
        {
            if (Main.netMode != NetmodeID.MultiplayerClient) return;

            ModPacket packet = ModContent.GetInstance<BlueMoon.BlueMoonMod>().GetPacket();
            packet.Write((byte)MoonMessageType.RequestMoonStatus);
            packet.Send();
        }
    }
}