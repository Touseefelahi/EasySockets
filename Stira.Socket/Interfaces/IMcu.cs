namespace Stira.Socket.Interfaces
{
    /// <summary>
    /// Main controller unit
    /// </summary>
    public interface IMcu
    {
        /// <summary>
        /// Controller IP
        /// </summary>
        string IP { get; set; }

        /// <summary>
        /// Controller TCP port
        /// </summary>
        int PortTcp { get; set; }

        /// <summary>
        /// Controller UDP port
        /// </summary>
        int PortUdp { get; set; }
    }
}