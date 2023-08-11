using ServerYourWorldMMORPG.Models.Enums;

namespace ServerYourWorldMMORPG.Models.Packets
{
    public abstract class PacketBase
    {
        public abstract PacketType PacketType { get; }
    }
}
