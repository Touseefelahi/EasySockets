using Stira.Socket.Interfaces;
using System.Collections.Generic;

namespace Stira.Socket.Models
{
    /// <summary>
    /// General reply: can be used for UDP/TCP
    /// </summary>
    public class ReplyPacket : IReplyPacket
    {
        /// <summary>
        /// Error to display if any
        /// </summary>
        public string Error { get; set; }

        /// <summary>
        /// IP address of the sender
        /// </summary>
        public string IPSender { get; set; }

        /// <summary>
        /// Command sent
        /// </summary>
        public bool IsSent { get; set; }

        /// <summary>
        /// Command Sent and camera replied
        /// </summary>
        public bool IsSentAndReplyReceived { get; set; }

        /// <summary>
        /// Sending Port
        /// </summary>
        public int PortSender { get; set; }

        /// <summary>
        /// Raw reply packet
        /// </summary>
        public List<byte> Reply { get; private set; }

        /// <summary>
        /// It sets the list of byte
        /// </summary>
        /// <param name="reply"></param>
        public void SetReply(byte[] reply)
        {
            Reply = new List<byte>(reply);
        }

        /// <summary>
        /// It sets the list of byte
        /// </summary>
        /// <param name="reply"></param>
        public void SetReply(List<byte> reply)
        {
            Reply = reply;
        }
    }
}