using Stira.Socket.Models;
using System;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Stira.Socket.Interfaces
{
    public interface ISendable
    {
        /// <summary>
        /// This is generic function which will send TCP command to specific IP, port and return the
        /// reply if required
        /// </summary>
        /// <param name="inputCommand">Commnad to be sent</param>
        /// <param name="isReplyRequired">Wait for reply?</param>
        /// <param name="txTimeout">Transmission timeout (ms)</param>
        /// <param name="rxTimeout">Receive timeout (ms)</param>
        /// <param name="connectionTimeout">Connection timeout (ms)</param>
        /// <param name="fireOnSent">Fires event on packet sent</param>
        /// <returns>Reply packet</returns>
        Task<ReplyPacket> SendTcpAsync(byte[] inputCommand, bool isReplyRequired = true, int txTimeout = 500, int rxTimeout = 500, int connectionTimeout = 1000, Action<bool> fireOnSent = null);

        /// <summary>
        /// This is generic function which will send UDP command to specific IP, port and return the
        /// reply if required
        /// </summary>
        /// <param name="bytes2Send"></param>
        /// <param name="replyRequired"></param>
        /// <param name="txTimeOut"></param>
        /// <param name="rxTimeOut"></param>
        /// <returns>Reply packet</returns>
        Task<ReplyPacket> SendUdpAsync(byte[] bytes2Send, bool replyRequired = false, int txTimeOut = 1000, int rxTimeOut = 1000);

        Task<ReplyPacket> SendUdpAsync(UdpClient socket, byte[] bytes2Send, bool replyRequired = true);
    }
}