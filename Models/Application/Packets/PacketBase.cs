using ServerYourWorldMMORPG.Models.Enums;

namespace ServerYourWorldMMORPG.Models.Application.Packets
{
    public abstract class PacketBase
    {
        public abstract PacketType PacketType { get; }
    }
}
