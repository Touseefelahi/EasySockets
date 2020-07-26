using Stira.Socket.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Stira.Socket.Models
{
    /// <summary>
    /// TCP Server
    /// </summary>
    public class ListenerTcp : IListenerTcp
    {
        private TcpListener listener;

        /// <summary>
        /// Initializer for TCP server
        /// </summary>
        public ListenerTcp()
        {
            ReceiveFromThisIPAddressList = new List<string>();
        }

        /// <summary>
        /// Auto reply packet
        /// </summary>
        public ICollection<byte> AutoReply { get; private set; }

        /// <summary>
        /// Whenever it receives packet this event fires up
        /// </summary>
        public EventHandler<ReplyPacket> DataEvent { get; set; }

        /// <summary>
        /// This event fires up whenever there's an exception
        /// </summary>
        public EventHandler<Exception> ExceptionHandler { get; set; }

        /// <summary>
        /// Enables auto reply for the client
        /// </summary>
        public bool IsAutoReplying { get; set; }

        /// <summary>
        /// Is server is enabled/listening
        /// </summary>
        public bool IsListening { get; set; }

        /// <summary>
        /// Listening port
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// If this list is not null then server will entertain only the IP's that are listed in
        /// this list
        /// </summary>
        public ICollection<string> ReceiveFromThisIPAddressList { get; set; }

        /// <summary>
        /// Sets the auto reply packet
        /// </summary>
        /// <param name="autoReply"></param>
        public void SetAutoReply(byte[] autoReply)
        {
            AutoReply = autoReply;
        }

        /// <summary>
        /// Starts the server with the given port
        /// </summary>
        /// <param name="port"></param>
        /// <returns></returns>
        public bool StartListener(int port)
        {
            Port = port; return StartListener();
        }

        /// <summary>
        /// Returns the current status of listener
        /// </summary>
        /// <returns></returns>
        public bool StartListener()
        {
            if (IsListening) return IsListening;
            IPAddress serverIP = IPAddress.Parse(Services.CommonUtility.GetIPAddress());
            listener = new TcpListener(serverIP, Port);

            try
            {
                listener.Start();
                IsListening = true;
            }
            catch (Exception ex)
            {
                IsListening = false;
                ExceptionHandler?.Invoke(null, ex);
                return IsListening;
            }

            Task.Run(async () =>
            {
                while (IsListening)
                {
                    try
                    {
                        ReplyPacket replyPacket = new ReplyPacket() { IsSentAndReplyReceived = true };
                        TcpClient clientTCP = await listener.AcceptTcpClientAsync().ConfigureAwait(false);
                        replyPacket.PortSender = ((IPEndPoint)clientTCP.Client.RemoteEndPoint).Port;
                        replyPacket.IPSender = ((IPEndPoint)clientTCP.Client.RemoteEndPoint).Address.ToString();
                        if (ReceiveFromThisIPAddressList.Count > 0)
                        {
                            bool isProcessingRequired = false;
                            foreach (var ipAcceptable in ReceiveFromThisIPAddressList)
                            {
                                if (ipAcceptable == replyPacket.IPSender)
                                {
                                    isProcessingRequired = true;
                                    break;
                                }
                            }
                            if (!isProcessingRequired) continue;
                        }

                        int length;
                        NetworkStream stream = clientTCP.GetStream();
                        List<byte> singlePacket = new List<byte>();
                        var byteArray = new byte[1024];
                        while ((length = stream.Read(byteArray, 0, byteArray.Length)) != 0)
                        {
                            var copy = new byte[length];
                            Array.Copy(byteArray, 0, copy, 0, length);
                            singlePacket.AddRange(copy);
                        }
                        replyPacket.SetReply(singlePacket);
                        DataEvent?.Invoke(this, replyPacket);
                        if (IsAutoReplying)
                        {
                            await stream.WriteAsync(AutoReply?.ToArray(), 0, AutoReply.Count).ConfigureAwait(false);
                            stream.Flush();
                        }
                        stream.Close();
                        clientTCP.Close();
                    }
                    catch (Exception ex)
                    {
                        ExceptionHandler?.Invoke(this, ex);
                    }
                }
                listener.Server.Close();
            });
            return IsListening;
        }

        /// <summary>
        /// Returns the current status of Listener
        /// </summary>
        /// <returns></returns>
        public bool StopListener()
        {
            try
            {
                listener.Server.Close();
                IsListening = false;
            }
            catch (Exception)
            {
            }
            return IsListening;
        }
    }
}