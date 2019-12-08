namespace Stira.Socket.Interfaces
{
    public interface ITranceiver : ISendable
    {
        string IP { get; }
        int Port { get; }
        int TimeoutConnection { get; set; }
        int TimeoutRx { get; set; }
        int TimeoutTx { get; set; }
    }
}