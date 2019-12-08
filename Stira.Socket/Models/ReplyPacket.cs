using System.Collections.Generic;

namespace Stira.Socket.Models
{
    public class ReplyPacket
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

        public void SetReply(byte[] reply)
        {
            Reply = new List<byte>(reply);
        }

        public void SetReply(List<byte> reply)
        {
            Reply = reply;
        }
    }
}