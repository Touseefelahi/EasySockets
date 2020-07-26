using Stira.Socket.Interfaces;
using System;
using System.Threading.Tasks;

namespace Stira.Socket.Models
{
    /// <summary>
    /// Main controller unit
    /// </summary>
    public class Mcu : IMcu
    {
        private string ip = "192.168.10.252";
        private int portTcp = 3030;
        private int portUdp = 2020;
        private ITranceiver tcpTranceiver;
        private ITranceiver udpTranceiver;

        /// <summary>
        /// Main controller unit
        /// </summary>
        public Mcu()
        {
            SetTransceivers();
        }

        /// <summary>
        /// Controller IP
        /// </summary>
        public string IP
        {
            get => ip;
            set
            {
                if (value != ip)
                {
                    if (Services.CommonUtility.ValidateIPv4(value))
                    {
                        ip = value;
                        SetTransceivers();
                    }
                }
            }
        }

        /// <summary>
        /// Controller TCP port
        /// </summary>
        public int PortTcp
        {
            get => portTcp;
            set
            {
                if (value != portTcp)
                {
                    portTcp = value;
                    SetTcpTransceiver();
                }
            }
        }

        /// <summary>
        /// Controller UDP port
        /// </summary>
        public int PortUdp
        {
            get => portUdp;
            set
            {
                if (value != portUdp)
                {
                    portUdp = value;
                    SetUdpTransceiver();
                }
            }
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
        public async Task<ReplyPacket> SendTcpAsync(byte[] inputCommand, bool isReplyRequired = true, int txTimeout = 500, int rxTimeout = 500, int connectionTimeout = 1000, Action<bool> fireOnSent = null)
        {
            return await tcpTranceiver.SendTcpAsync(inputCommand, isReplyRequired, txTimeout, rxTimeout, connectionTimeout, fireOnSent).ConfigureAwait(false);
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
        public async Task<ReplyPacket> SendUdpAsync(byte[] bytes2Send, bool replyRequired = false, int txTimeOut = 1000, int rxTimeOut = 1000)
        {
            return await udpTranceiver.SendUdpAsync(bytes2Send, replyRequired, txTimeOut, rxTimeOut).ConfigureAwait(false);
        }

        private void SetTcpTransceiver()
        {
            tcpTranceiver = new Transceiver(IP, PortTcp);
        }

        private void SetTransceivers()
        {
            SetUdpTransceiver();
            SetTcpTransceiver();
        }

        private void SetUdpTransceiver()
        {
            udpTranceiver = new Transceiver(IP, PortUdp);
        }
    }
}