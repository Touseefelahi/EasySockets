using Stira.Socket.Models;
using System;
using System.Windows.Input;

namespace Stira.Socket.Interfaces
{
    public interface IControlViewModel
    {
        string DeveloperCommand { get; set; }
        EventHandler<ReplyPacket> DevReplyIncoming { get; set; }
        EventHandler<ReplyPacket> DevSentCommandInfo { get; set; }
        bool IsHex { get; set; }
        Mcu Mcu { get; set; }
        ICommand SendDeveloperCommand { get; }
        IListenerTcp ListenerTcp { get; }
    }
}