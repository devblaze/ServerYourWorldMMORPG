namespace ServerYourWorldMMORPG.Models.Network
{
    public class UserClient
    {
        public string Id { get; set; }
        public string IP { get; set; }
        public int Port { get; set; }
        public bool IsConnected { get; set; }
        public bool IsRealClient { get; set; }
    }
}
