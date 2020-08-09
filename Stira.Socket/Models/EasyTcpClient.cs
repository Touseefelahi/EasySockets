using Stira.Socket.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Stira.Socket.Models
{
    /// <summary>
    /// TCP Client
    /// </summary>
    public class EasyTcpClient : ITcpClient
    {
        private TcpClient clientTcp;

        private NetworkStream stream;

        private bool disposed = false;

        /// <summary>
        /// Is server is enabled/listening
        /// </summary>
        public bool IsConnected { get; private set; }

        /// <summary>
        /// This event fires up whenever there's an exception
        /// </summary>
        public EventHandler<IReplyPacket> DataReady { get; set; }

        /// <summary>
        /// This event fires up whenever there's an exception
        /// </summary>
        public EventHandler<Exception> ExceptionHandler { get; set; }

        /// <summary>
        /// Server port
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// Server IP
        /// </summary>
        public string Ip { get; set; }

        /// <summary>
        /// It will connect to the server
        /// </summary>
        /// <param name="dataReady">Incoming Data will be pushed to this action</param>
        /// <param name="endBytesIdentifier">
        /// This identifies the end packet once receive these bytes it will push the packet. If null
        /// then it pushes as it receives
        /// </param>
        /// <param name="connectionTimeoutMs"></param>
        /// <param name="receiveTimeOut"></param>
        /// <param name="transmissionTimeout"></param>
        /// <returns></returns>
        public async Task<bool> ConnectAsync(Action<IReplyPacket> dataReady, byte[] endBytesIdentifier = null, int connectionTimeoutMs = 2000, int receiveTimeOut = 1000, int transmissionTimeout = 1000)
        {
            return await ConnectAsync(Ip, Port, dataReady, endBytesIdentifier, connectionTimeoutMs, receiveTimeOut, transmissionTimeout).ConfigureAwait(false);
        }

        /// <summary>
        /// This will send the bytes to the server if connected
        /// </summary>
        /// <param name="bytesToSend"></param>
        /// <returns>Sent Status</returns>
        public async Task<bool> SendAsync(byte[] bytesToSend)
        {
            if (bytesToSend != null && clientTcp?.Connected == true)
            {
                await stream.WriteAsync(bytesToSend, 0, bytesToSend.Length).ConfigureAwait(false);
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// It will connect on the server with given IP and Port then listens on it
        /// </summary>
        /// <param name="ip">Server IP</param>
        /// <param name="port">Server Port</param>
        /// <param name="dataReady">Incoming Data will be pushed to this action</param>
        /// <param name="endBytesIdentifier">
        /// This identifies the end packet once receive these bytes it will push the packet. If null
        /// then it pushes as it receives
        /// </param>
        /// <param name="connectionTimeoutMs"></param>
        /// <param name="receiveTimeOut"></param>
        /// <param name="transmissionTimeout"></param>
        /// <returns></returns>
        public async Task<bool> ConnectAsync(string ip, int port, Action<IReplyPacket> dataReady, byte[] endBytesIdentifier = null, int connectionTimeoutMs = 2000, int receiveTimeOut = 1000, int transmissionTimeout = 1000)
        {
            if (endBytesIdentifier != null)
            {
                endBytesIdentifier = endBytesIdentifier.Reverse().ToArray();
            }
            if (IsConnected)
            {
                return IsConnected;
            }
            clientTcp = new TcpClient() { ReceiveTimeout = receiveTimeOut, SendTimeout = transmissionTimeout };

            try
            {
                IAsyncResult result = clientTcp.BeginConnect(ip, port, null, null);
                bool success = result.AsyncWaitHandle.WaitOne(connectionTimeoutMs, true);
                IsConnected = clientTcp.Connected;
                if (!IsConnected)
                {
                    return IsConnected;
                }
                Port = port;
                Ip = ip;
            }
            catch (Exception ex)
            {
                ExceptionHandler?.Invoke(this, ex);
                return IsConnected;
            }

            try
            {
                stream = clientTcp.GetStream();
                ReplyPacket replyPacket = new ReplyPacket() { IsSentAndReplyReceived = true, IPSender = ip };
                IsConnected = true;

                List<byte> completePacket = new List<byte>();
                while (clientTcp.Connected)
                {
                    if (!IsConnected)
                    {
                        break;
                    }

                    if (!stream.DataAvailable)
                    {
                        await Task.Delay(1).ConfigureAwait(false);
                    }
                    else
                    {
                        try
                        {
                            byte[] packet = new byte[clientTcp.Available];
                            await stream.ReadAsync(packet, 0, packet.Length).ConfigureAwait(false);
                            completePacket.AddRange(packet);
                            int lenth = completePacket.Count;
                            if (endBytesIdentifier != null)
                            {
                                //this will flush any data that was there before connecting to the software
                                //if (lenth > 5000) { completePacket.Clear(); continue; }
                                if (completePacket.Count > endBytesIdentifier.Length)
                                {
                                    bool isPacketEndIdentified = false;
                                    for (int i = 0; i < endBytesIdentifier.Length; i++)
                                    {
                                        if (completePacket[lenth - (i + 1)] == endBytesIdentifier[i])
                                        {
                                            isPacketEndIdentified = true;
                                        }
                                        else
                                        {
                                            isPacketEndIdentified = false;
                                            break;
                                        }
                                    }
                                    if (isPacketEndIdentified)
                                    {
                                        replyPacket.SetReply(completePacket);
                                        dataReady?.Invoke(replyPacket);
                                        completePacket.Clear();
                                    }
                                }
                            }
                            else
                            {
                                replyPacket.SetReply(completePacket);
                                dataReady?.Invoke(replyPacket);
                                completePacket.Clear();
                            }
                        }
                        catch (Exception ex)
                        {
                            ExceptionHandler?.Invoke(this, ex);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler?.Invoke(this, ex);
            }

            return IsConnected;
        }

        /// <summary>
        /// Returns the current status of Listener
        /// </summary>
        /// <returns></returns>
        public bool Disconnect()
        {
            try
            {
                clientTcp.Client.Close();
                clientTcp.Close();
                IsConnected = false;
            }
            catch (Exception ex)
            {
                ExceptionHandler?.Invoke(this, ex);
            }
            return IsConnected;
        }

        /// <summary>
        /// Disposes the Tcp socket
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// It will connect and listen to the server. Use it in the thread/task
        /// </summary>
        /// <param name="endBytesIdentifier">
        /// This identifies the end packet once receive these bytes it will push the packet. If null
        /// then it pushes as it receives
        /// </param>
        /// <param name="connectionTimeoutMs"></param>
        /// <param name="receiveTimeOut"></param>
        /// <param name="transmissionTimeout"></param>
        /// <returns>Connection Status</returns>
        public bool Connect(byte[] endBytesIdentifier = null, int connectionTimeoutMs = 2000, int receiveTimeOut = 1000, int transmissionTimeout = 1000)
        {
            return Connect(Ip, Port, endBytesIdentifier, connectionTimeoutMs, receiveTimeOut, transmissionTimeout);
        }

        /// <summary>
        /// It will connect on the server with given IP and Port then listens on it. Use it in the thread/task
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
        /// <returns>Connection Status</returns>
        public bool Connect(string ip, int port, byte[] endBytesIdentifier = null, int connectionTimeoutMs = 2000, int receiveTimeOut = 1000, int transmissionTimeout = 1000)
        {
            if (endBytesIdentifier != null)
            {
                endBytesIdentifier = endBytesIdentifier.Reverse().ToArray();
            }
            if (IsConnected)
            {
                return IsConnected;
            }
            clientTcp = new TcpClient() { ReceiveTimeout = receiveTimeOut, SendTimeout = transmissionTimeout };

            try
            {
                IAsyncResult result = clientTcp.BeginConnect(ip, port, null, null);
                bool success = result.AsyncWaitHandle.WaitOne(connectionTimeoutMs, true);
                IsConnected = clientTcp.Connected;
                if (!IsConnected)
                {
                    return IsConnected;
                }
                Port = port;
                Ip = ip;
            }
            catch (Exception ex)
            {
                ExceptionHandler?.Invoke(this, ex);
                return IsConnected;
            }

            try
            {
                stream = clientTcp.GetStream();
                ReplyPacket replyPacket = new ReplyPacket() { IsSentAndReplyReceived = true, IPSender = ip };
                IsConnected = true;

                List<byte> completePacket = new List<byte>();
                Task.Run(async () =>
                {
                    while (clientTcp.Connected)
                    {
                        if (!IsConnected)
                        {
                            break;
                        }

                        if (!stream.DataAvailable)
                        {
                            await Task.Delay(1).ConfigureAwait(false);
                        }
                        else
                        {
                            try
                            {
                                byte[] packet = new byte[clientTcp.Available];
                                await stream.ReadAsync(packet, 0, packet.Length).ConfigureAwait(false);
                                completePacket.AddRange(packet);
                                int lenth = completePacket.Count;
                                if (endBytesIdentifier != null)
                                {
                                    //this will flush any data that was there before connecting to the software
                                    //if (lenth > 5000) { completePacket.Clear(); continue; }
                                    if (completePacket.Count > endBytesIdentifier.Length)
                                    {
                                        bool isPacketEndIdentified = false;
                                        for (int i = 0; i < endBytesIdentifier.Length; i++)
                                        {
                                            if (completePacket[lenth - (i + 1)] == endBytesIdentifier[i])
                                            {
                                                isPacketEndIdentified = true;
                                            }
                                            else
                                            {
                                                isPacketEndIdentified = false;
                                                break;
                                            }
                                        }
                                        if (isPacketEndIdentified)
                                        {
                                            replyPacket.SetReply(completePacket);
                                            DataReady?.Invoke(this, replyPacket);
                                            completePacket.Clear();
                                        }
                                    }
                                }
                                else
                                {
                                    replyPacket.SetReply(completePacket);
                                    DataReady?.Invoke(this, replyPacket);
                                    completePacket.Clear();
                                }
                            }
                            catch (Exception ex)
                            {
                                ExceptionHandler?.Invoke(this, ex);
                            }
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                ExceptionHandler?.Invoke(this, ex);
            }

            return IsConnected;
        }

        // to detect redundant calls
        /// <summary>
        /// Disposes the Tcp socket
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    // Dispose managed resources.
                    Disconnect();
                }

                // Dispose unmanaged managed resources.
                disposed = true;
            }
        }
    }
}