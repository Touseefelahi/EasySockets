namespace Stira.Socket.Interfaces
{
    /// <summary>
    /// Encapsulates the UDP/TCP sockets for transmission and reception
    /// </summary>
    public interface ITranceiver : ISendable
    {
        /// <summary>
        /// Receiver IP
        /// </summary>
        string IP { get; }

        /// <summary>
        /// Receiver Port
        /// </summary>
        int Port { get; }

        /// <summary>
        /// Connection timetout (for TCP only)
        /// </summary>
        int TimeoutConnection { get; set; }

        /// <summary>
        /// Receive timeout
        /// </summary>
        int TimeoutRx { get; set; }

        /// <summary>
        /// Transmission timeout
        /// </summary>
        int TimeoutTx { get; set; }
    }
}