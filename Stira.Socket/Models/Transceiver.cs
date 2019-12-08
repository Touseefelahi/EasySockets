using Stira.Socket.Interfaces;
using System;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Stira.Socket.Models
{
    public class Transceiver : ITranceiver
    {
        private const int defaultConnectionTimeout = 1000;
        private const int defaultTimeout = 500;

        public Transceiver()
        {
        }

        public Transceiver(string ip, int port, int timeOutTx = defaultTimeout,
            int timeOutRx = defaultTimeout, int timeoutConnection = defaultConnectionTimeout)
        {
            IP = ip;
            Port = port;
            TimeoutRx = timeOutRx;
            TimeoutTx = timeOutTx;
            TimeoutConnection = timeoutConnection;
        }

        public string IP { get; set; }
        public int Port { get; set; }
        public int TimeoutConnection { get; set; }
        public int TimeoutRx { get; set; }
        public int TimeoutTx { get; set; }

        /// <summary>
        /// This is generic function which will send TCP command to specific IP, port and return the
        /// /// reply if required [This method will use the default timeouts]
        /// </summary>
        /// <param name="inputCommand">Commnad to be sent</param>
        /// <param name="isReplyRequired">Wait for reply?</param>
        /// <param name="fireOnSent">Fires event on packet sent</param>
        /// <returns>Reply packet</returns>
        public async Task<ReplyPacket> SendTcpAsync(byte[] inputCommand, bool isReplyRequired = true, Action<bool> fireOnSent = null)
        {
            return await SendTcpAsync(inputCommand, isReplyRequired, TimeoutTx, TimeoutRx, TimeoutConnection, fireOnSent).ConfigureAwait(false);
        }

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
        public async Task<ReplyPacket> SendTcpAsync(byte[] inputCommand, bool isReplyRequired = true, int txTimeout = defaultTimeout,
        int rxTimeout = defaultTimeout, int connectionTimeout = defaultConnectionTimeout, Action<bool> fireOnSent = null)
        {
            ReplyPacket replyPacket = new ReplyPacket();
            if (inputCommand is null)
            {
                replyPacket.Error = "Bytes to send is null";
                return replyPacket;
            }

            using (TcpClient client = new TcpClient
            {
                ReceiveTimeout = rxTimeout,
                SendTimeout = txTimeout
            })
            {
                try
                {
                    IAsyncResult connectResult = client.BeginConnect(IP, Port, null, null);
                    replyPacket.IPSender = IP;
                    replyPacket.PortSender = Port;
                    Task<bool> connectionTask = new Task<bool>(() =>
                    connectResult.AsyncWaitHandle.WaitOne(TimeSpan.FromMilliseconds(connectionTimeout)));
                    connectionTask.Start();

                    if (await connectionTask.ConfigureAwait(false))
                    {
                        using (NetworkStream stream = client.GetStream())
                        {
                            await stream.WriteAsync(inputCommand, 0, inputCommand.Length).ConfigureAwait(false);
                            stream.Flush();
                            replyPacket.IsSent = true;
                            fireOnSent?.Invoke(replyPacket.IsSent);
                            if (isReplyRequired)
                            {
                                //Reading The first byte for the sake of timeout
                                byte[] reply = new byte[1];
                                await stream.ReadAsync(reply, 0, 1).ConfigureAwait(false);
                                byte[] replyComplete = null;

                                //Reading rest of the bytes
                                if (stream.DataAvailable)
                                {
                                    replyComplete = new byte[client.Available + 1];
                                    replyComplete[0] = reply[0];
                                    await stream.ReadAsync(replyComplete, 1, replyComplete.Length - 1).ConfigureAwait(false);
                                }

                                if (replyComplete != null)
                                {
                                    replyPacket.SetReply(replyComplete);
                                    replyPacket.IsSentAndReplyReceived = true;
                                }
                                else
                                {
                                    replyPacket.Error = "Unable to read from remote address";
                                }
                            }
                        }
                    }
                    else
                    {
                        fireOnSent?.Invoke(replyPacket.IsSent);
                        replyPacket.Error = "Connection TimeOut/Server Not Found";
                    }
                }
                catch (Exception ex)
                {
                    replyPacket.Error = ex.Message;
                }
            }

            return replyPacket;
        }

        /// <summary>
        /// This is generic function which will send UDP command to specific IP, port and return the
        /// reply if required
        /// </summary>
        /// <param name="bytes2Send"></param>
        /// <param name="replyRequired"></param>
        /// <param name="txTimeOut"></param>
        /// <param name="rxTimeOut"></param>
        /// <returns>Reply packet</returns>
        public async Task<ReplyPacket> SendUdpAsync(byte[] bytes2Send, bool replyRequired = true,
            int txTimeOut = defaultTimeout, int rxTimeOut = defaultTimeout)
        {
            try
            {
                using (UdpClient socket = new UdpClient())
                {
                    socket.Connect(IP, Port);
                    socket.Client.ReceiveTimeout = rxTimeOut;
                    socket.Client.SendTimeout = txTimeOut;
                    return await SendUdpAsync(socket, bytes2Send, replyRequired).ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                return new ReplyPacket { Error = ex.Message };
            }
        }

        /// <summary>
        /// This is generic function which will send UDP command to specific IP, port and return the
        /// reply if required
        /// </summary>
        /// <param name="socket">Socket - must be initialized</param>
        /// <param name="bytes2Send">Bytes to send to the given socket</param>
        /// <param name="replyRequired">Is reply required?</param>
        /// <returns>Reply packet</returns>
        public async Task<ReplyPacket> SendUdpAsync(UdpClient socket, byte[] bytes2Send, bool replyRequired = true)
        {
            ReplyPacket replyPacket = new ReplyPacket();
            if (socket is null)
            {
                replyPacket.Error = "Socket is null";
                return replyPacket;
            }

            if (bytes2Send is null)
            {
                replyPacket.Error = "Bytes to send is null";
                return replyPacket;
            }

            try
            {
                int bytesSent = await socket.SendAsync(bytes2Send, bytes2Send.Length).ConfigureAwait(false);
                replyPacket.IsSent = true;
                if (replyRequired)
                {
                    Task<UdpReceiveResult> taskRecievePacket = socket.ReceiveAsync();
                    taskRecievePacket.Wait(socket.Client.ReceiveTimeout);
                    if (taskRecievePacket.IsCompleted)
                    {
                        replyPacket.SetReply(taskRecievePacket.Result.Buffer);
                        replyPacket.IsSentAndReplyReceived = true;
                    }
                }
            }
            catch (Exception ex)
            {
                replyPacket.Error = ex.Message;
            }
            return replyPacket;
        }
    }
}