using Stira.Socket.Models;
using Stira.WpfCore;
using System;
using System.Text;
using System.Windows.Input;

namespace Stira.Socket.ViewModels
{
    public class ControlViewModel : BaseNotifyPropertyChanged
    {
        public ControlViewModel()
        {
            Mcu = new Mcu
            {
                IP = "192.168.10.252",
                PortTcp = 3030
            };
            SendDeveloperCommand = new DelegateCommand(DeveloperCommandToMcuAsync);
        }

        public string DeveloperCommand { get; set; }

        public EventHandler<ReplyPacket> DevReplyIncoming { get; set; }
        public EventHandler<ReplyPacket> DevSentCommandInfo { get; set; }

        public bool IsHex { get; set; }

        public Mcu Mcu { get; set; }

        public ICommand SendDeveloperCommand { get; }

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
            catch (Exception ex)
            {
            }
        }
    }
}