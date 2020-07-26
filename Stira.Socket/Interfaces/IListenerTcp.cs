using Stira.Socket.Models;
using System;
using System.Collections.Generic;

namespace Stira.Socket.Interfaces
{
    /// <summary>
    /// TCP server
    /// </summary>
    public interface IListenerTcp
    {
        /// <summary>
        /// Auto reply packet
        /// </summary>
        ICollection<byte> AutoReply { get; }

        /// <summary>
        /// Whenever it receives packet this event fires up
        /// </summary>
        EventHandler<ReplyPacket> DataEvent { get; set; }

        /// <summary>
        /// This event fires up whenever there's an exception
        /// </summary>
        EventHandler<Exception> ExceptionHandler { get; set; }

        /// <summary>
        /// Enables auto reply for the client
        /// </summary>
        bool IsAutoReplying { get; set; }

        /// <summary>
        /// Is server is enabled/listening
        /// </summary>
        bool IsListening { get; set; }

        /// <summary>
        /// Listening port
        /// </summary>
        int Port { get; set; }

        /// <summary>
        /// If this list is not null then server will entertain only the IP's that are listed in
        /// this list
        /// </summary>
        ICollection<string> ReceiveFromThisIPAddressList { get; set; }

        /// <summary>
        /// Sets the auto reply packet
        /// </summary>
        /// <param name="autoReply"></param>
        void SetAutoReply(byte[] autoReply);

        /// <summary>
        /// Start the server
        /// </summary>
        /// <returns></returns>
        bool StartListener();

        /// <summary>
        /// Starts the server with the given port
        /// </summary>
        /// <param name="port"></param>
        /// <returns></returns>
        bool StartListener(int port);

        /// <summary>
        /// Stops the server/listener
        /// </summary>
        /// <returns></returns>
        bool StopListener();
    }
}