namespace Stira.Socket.Interfaces
{
    public interface IMcu
    {
        string IP { get; set; }
        int PortTcp { get; set; }
        int PortUdp { get; set; }
    }
}