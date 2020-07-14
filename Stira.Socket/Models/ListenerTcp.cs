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

        public ListenerTcp()
        {
            ReceiveFromThisIPAddressList = new List<string>();
        }

        public ICollection<byte> AutoReply { get; private set; }
        public EventHandler<ReplyPacket> DataEvent { get; set; }
        public EventHandler<Exception> ExceptionHandler { get; set; }
        public bool IsAutoReplying { get; set; }
        public bool IsListening { get; set; }
        public int Port { get; set; }
        public List<string> ReceiveFromThisIPAddressList { get; set; }

        public void SetAutoReply(byte[] autoReply)
        {
            AutoReply = autoReply;
        }

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