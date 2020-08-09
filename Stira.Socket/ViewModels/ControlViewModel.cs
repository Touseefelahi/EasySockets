using Stira.Socket.Interfaces;
using Stira.Socket.Models;
using Stira.Socket.Services;
using Stira.WpfCore;
using System;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Stira.Socket.ViewModels
{
    /// <summary>
    /// View model for the socket. Use Mcu class to send TCP/UDP commands from the viewmodels.
    /// </summary>
    public class ControlViewModel : BaseNotifyPropertyChanged, IControlViewModel
    {
        /// <summary>
        /// Initialize the control view model
        /// </summary>
        public ControlViewModel(IListenerTcp listenerTcp, ITcpClient tcpClient, IMcu mcu)
        {
            SendDeveloperCommand = new DelegateCommand(DeveloperCommandToMcuAsync);
            StartTcpListenerCommand = new DelegateCommand(ToggleTcpListener);
            ConnectToTcpServerCommand = new DelegateCommand(ConnectToTcp);
            Mcu = mcu;
            ListenerTcp = listenerTcp;
            TcpClient = tcpClient;
        }

        /// <summary>
        /// Initialize the control view model
        /// </summary>
        public ControlViewModel()
        {
            SendDeveloperCommand = new DelegateCommand(DeveloperCommandToMcuAsync);
            StartTcpListenerCommand = new DelegateCommand(ToggleTcpListener);
            ConnectToTcpServerCommand = new DelegateCommand(ConnectToTcp);

            Mcu = new Mcu()
            {
                IP = "192.168.10.252",
                PortTcp = 3030,
                PortUdp = 2020,
            };
            ListenerTcp = new ListenerTcp();
            TcpClient = new EasyTcpClient();
        }

        /// <summary>
        /// Developer Command from the UI
        /// </summary>
        public string DeveloperCommand { get; set; }

        /// <summary>
        /// This event will be fired when any Developer socket sends a command and receives
        /// something from the server
        /// </summary>
        public EventHandler<ReplyPacket> DevReplyIncoming { get; set; }

        /// <summary>
        /// This event will be fired when developer command is sent
        /// </summary>
        public EventHandler<ReplyPacket> DevSentCommandInfo { get; set; }

        /// <summary>
        /// Hex check for developer command
        /// </summary>
        public bool IsHex { get; set; }

        /// <summary>
        /// Main controller that has the sockets
        /// </summary>
        public IMcu Mcu { get; set; }

        /// <summary>
        /// Command to send the developer command from UI/Viewmodel
        /// </summary>
        public ICommand SendDeveloperCommand { get; }

        /// <summary>
        /// Command to enable TCP Listener
        /// </summary>
        public ICommand StartTcpListenerCommand { get; }

        /// <summary>
        /// Connects to the server
        /// </summary>
        public ICommand ConnectToTcpServerCommand { get; }

        /// <summary>
        /// TCP Listener
        /// </summary>
        public IListenerTcp ListenerTcp { get; }

        /// <summary>
        /// TCP Client
        /// </summary>
        public ITcpClient TcpClient { get; }

        private void ConnectToTcp()
        {
            if (TcpClient.IsConnected)
            {
                TcpClient.Disconnect();
            }
            else
            {
                TcpClient.Connect();
            }
            OnPropertyChanged(nameof(TcpClient));
        }

        private void ToggleTcpListener()
        {
            if (ListenerTcp.IsListening)
            {
                ListenerTcp.StopListener();
            }
            else
            {
                ListenerTcp.StartListener();
            }

            OnPropertyChanged(nameof(ListenerTcp));
        }

        private async void DeveloperCommandToMcuAsync()
        {
            byte[] byteCommand;
            try
            {
                if (IsHex)
                {
                    byteCommand = Conversion.StringToByteArray(DeveloperCommand);
                }
                else
                {
                    byteCommand = Encoding.ASCII.GetBytes(DeveloperCommand + "\n");
                }

                ReplyPacket reply = await Mcu.SendTcpAsync(byteCommand, true, fireOnSent: (isSent) =>
                {
                    ReplyPacket replyPacket = new ReplyPacket() { IsSent = isSent };
                    replyPacket.SetReply(byteCommand);
                    DevSentCommandInfo?.Invoke(this, replyPacket);
                }).ConfigureAwait(false);
                if (reply.IsSentAndReplyReceived)
                {
                    DevReplyIncoming?.Invoke(this, reply);
                }
            }
            catch (Exception ex)
            {
                ListenerTcp.ExceptionHandler?.Invoke(this, ex);
            }
        }
    }
}