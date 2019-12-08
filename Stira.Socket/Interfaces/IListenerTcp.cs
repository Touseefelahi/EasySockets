using Stira.Socket.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Stira.Socket.Interfaces
{
    /// <summary>
    /// TCP server
    /// </summary>
    public interface IListenerTcp
    {
        ICollection<byte> AutoReply { get; }
        EventHandler<ReplyPacket> DataEvent { get; set; }
        EventHandler<Exception> ExceptionHandler { get; set; }
        bool IsAutoReplying { get; set; }
        bool IsListening { get; set; }
        int Port { get; set; }
        List<string> ReceiveFromThisIPAddressList { get; set; }

        void SetAutoReply(byte[] autoReply);

        bool StartListener();

        bool StartListener(int port);

        bool StopListener();
    }
}