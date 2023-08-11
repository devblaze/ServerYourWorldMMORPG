using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServerYourWorldMMORPG.Models.Enums;

namespace ServerYourWorldMMORPG.Models.Packets
{
    public class MovementPacket : PacketBase
    {
        public override PacketType PacketType => PacketType.PlayerMovement;
    }
}
