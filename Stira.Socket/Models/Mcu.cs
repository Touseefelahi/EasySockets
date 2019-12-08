using Stira.Socket.Interfaces;
using System;
using System.Threading.Tasks;

namespace Stira.Socket.Models
{
    public class Mcu : IMcu
    {
        private string ip = "192.168.10.252";
        private int portTcp = 3030;
        private int portUdp = 2020;
        private ITranceiver tcpTranceiver;
        private ITranceiver udpTranceiver;

        public Mcu()
        {
            SetTransceivers();
        }

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

        public async Task<ReplyPacket> SendTcpAsync(byte[] inputCommand, bool isReplyRequired = true, int txTimeout = 500, int rxTimeout = 500, int connectionTimeout = 1000, Action<bool> fireOnSent = null)
        {
            return await tcpTranceiver.SendTcpAsync(inputCommand, isReplyRequired, txTimeout, rxTimeout, connectionTimeout, fireOnSent).ConfigureAwait(false);
        }

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