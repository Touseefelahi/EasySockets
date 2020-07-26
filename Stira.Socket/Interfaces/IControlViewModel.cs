using Stira.Socket.Models;
using System;
using System.Windows.Input;

namespace Stira.Socket.Interfaces
{
    /// <summary>
    /// View model for the socket. Use Mcu class to send TCP/UDP commands from the viewmodels.
    /// </summary>
    public interface IControlViewModel
    {
        /// <summary>
        /// Developer Command from the UI
        /// </summary>
        string DeveloperCommand { get; set; }

        /// <summary>
        /// This event will be fired when any Developer socket sends a command and receives
        /// something from the server
        /// </summary>
        EventHandler<ReplyPacket> DevReplyIncoming { get; set; }

        /// <summary>
        /// This event will be fired when developer command is sent
        /// </summary>
        EventHandler<ReplyPacket> DevSentCommandInfo { get; set; }

        /// <summary>
        /// Hex check for developer command
        /// </summary>
        bool IsHex { get; set; }

        /// <summary>
        /// Main controller that has the sockets
        /// </summary>
        IMcu Mcu { get; set; }

        /// <summary>
        /// Command to send the developer command from UI/Viewmodel
        /// </summary>
        ICommand SendDeveloperCommand { get; }

        /// <summary>
        /// Command to enable TCP Listener
        /// </summary>
        ICommand StartTcpListenerCommand { get; }

        /// <summary>
        /// TCP Listener
        /// </summary>
        IListenerTcp ListenerTcp { get; }
    }
}