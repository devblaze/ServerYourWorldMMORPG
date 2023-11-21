public interface IDummyGameClient
{
    void ExecuteCommand(string[] arguments);
    Task Connect();
    void ReceivePackets();
    void SendPacket(string data);
    void Disconnect();
}