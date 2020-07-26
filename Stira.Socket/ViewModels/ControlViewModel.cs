using Stira.Socket.Interfaces;
using Stira.Socket.Models;
using Stira.WpfCore;
using System;
using System.Text;
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
        public ControlViewModel()
        {
            Mcu = new Mcu
            {
                IP = "192.168.10.252",
                PortTcp = 3030
            };
            SendDeveloperCommand = new DelegateCommand(DeveloperCommandToMcuAsync);
            StartTcpListenerCommand = new DelegateCommand(ToggleTcpListener);
            ListenerTcp = new ListenerTcp();
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
        /// TCP Listener
        /// </summary>
        public IListenerTcp ListenerTcp { get; }

        /// <summary>
        /// This method converts hex string to byte array e.g. "010203" or "0x01 0x02 0x03" =&gt;
        /// byte[3] with 01 02 03 values
        /// </summary>
        /// <param name="hex"></param>
        /// <returns>Byte Array</returns>
        private static byte[] StringToByteArray(string hex)
        {
            hex = hex.Replace(" ", ""); //Remove Spacing
            hex = hex.Replace("0x", ""); //Remove 0x for hex
            hex = hex.Replace("0X", ""); //Remove 0X for hex
            int NumberChars = hex.Length;
            byte[] bytes = new byte[NumberChars / 2];
            for (int i = 0; i < NumberChars; i += 2)
            {
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            }

            return bytes;
        }

        private void ToggleTcpListener()
        {
            if (ListenerTcp.IsListening)
                ListenerTcp.StopListener();
            else
                ListenerTcp.StartListener();
            OnPropertyChanged(nameof(ListenerTcp));
        }

        private async void DeveloperCommandToMcuAsync()
        {
            byte[] byteCommand;
            try
            {
                if (IsHex)
                {
                    byteCommand = StringToByteArray(DeveloperCommand);
                }
                else
                {
                    byteCommand = Encoding.ASCII.GetBytes(DeveloperCommand + "\n");
                }
                var reply = await Mcu.SendTcpAsync(byteCommand, true, fireOnSent: (isSent) =>
                {
                    var replyPacket = new ReplyPacket() { IsSent = isSent };
                    replyPacket.SetReply(byteCommand);
                    DevSentCommandInfo?.Invoke(this, replyPacket);
                }).ConfigureAwait(false);
                if (reply.IsSentAndReplyReceived)
                {
                    DevReplyIncoming?.Invoke(this, reply);
                }
            }
            catch (Exception)
            {
            }
        }
    }
}