using System.Collections.Generic;

namespace Stira.Socket.Interfaces
{
    public interface IReplyPacket
    {
        /// <summary>
        /// Error to display if any
        /// </summary>
        string Error { get; set; }

        /// <summary>
        /// IP address of the sender
        /// </summary>
        string IPSender { get; set; }

        /// <summary>
        /// Command sent
        /// </summary>
        bool IsSent { get; set; }

        /// <summary>
        /// Command Sent and camera replied
        /// </summary>
        bool IsSentAndReplyReceived { get; set; }

        /// <summary>
        /// Sending Port
        /// </summary>
        int PortSender { get; set; }

        /// <summary>
        /// Raw reply packet
        /// </summary>
        List<byte> Reply { get; }

        void SetReply(byte[] reply);

        void SetReply(List<byte> reply);
    }
}