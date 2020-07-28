using System;
using System.Threading.Tasks;

namespace Stira.Socket.Interfaces
{
    /// <summary>
    /// TCP Client
    /// </summary>
    public interface ITcpClient : IDisposable
    {
        /// <summary>
        /// Whenever it receives packet this event fires up
        /// </summary>
        EventHandler<IReplyPacket> DataEvent { get; set; }

        /// <summary>
        /// This event fires up whenever there's an exception
        /// </summary>
        EventHandler<Exception> ExceptionHandler { get; set; }

        /// <summary>
        /// Server IP
        /// </summary>
        string Ip { get; set; }

        /// <summary>
        /// Is server is enabled/listening
        /// </summary>
        bool IsConnected { get; set; }

        /// <summary>
        /// Server port
        /// </summary>
        int Port { get; set; }

        /// <summary>
        /// It will connect to the server
        /// </summary>
        /// <param name="endBytesIdentifier">
        /// This identifies the end packet once receive these bytes it will push the packet. If null
        /// then it pushes as it receives
        /// </param>
        /// <param name="connectionTimeoutMs"></param>
        /// <param name="receiveTimeOut"></param>
        /// <param name="transmissionTimeout"></param>
        /// <returns></returns>
        bool Connect(byte[] endBytesIdentifier = null, int connectionTimeoutMs = 2000, int receiveTimeOut = 1000, int transmissionTimeout = 1000);

        /// <summary>
        /// It will connect on the server with given IP and Port then listens on it
        /// </summary>
        /// <param name="ip">Server IP</param>
        /// <param name="port">Server Port</param>
        /// <param name="endBytesIdentifier">
        /// This identifies the end packet once receive these bytes it will push the packet. If null
        /// then it pushes as it receives
        /// </param>
        /// <param name="connectionTimeoutMs"></param>
        /// <param name="receiveTimeOut"></param>
        /// <param name="transmissionTimeout"></param>
        /// <returns></returns>
        bool Connect(string ip, int port, byte[] endBytesIdentifier = null, int connectionTimeoutMs = 2000, int receiveTimeOut = 1000, int transmissionTimeout = 1000);

        /// <summary>
        /// Returns the current status of Listener
        /// </summary>
        /// <returns></returns>
        bool Disconnect();

        /// <summary>
        /// This will send the bytes to the server if connected
        /// </summary>
        /// <param name="bytesToSend"></param>
        /// <returns>Sent Status</returns>
        Task<bool> SendAsync(byte[] bytesToSend);
    }
}