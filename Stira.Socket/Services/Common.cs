using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace Stira.Socket.Services
{
    public static class CommonUtility
    {
        public static string GetIPAddress()
        {
            string localIP;
            using (var socket = new System.Net.Sockets.Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0))
            {
                socket.Connect("8.8.8.8", 65530);
                IPEndPoint endPoint = socket.LocalEndPoint as IPEndPoint;
                localIP = endPoint.Address.ToString();
            }
            return localIP;
        }

        public static bool ValidateIPv4(string ipString)
        {
            if (ipString.Count(c => c == '.') != 3) return false;
            return IPAddress.TryParse(ipString, out IPAddress address);
        }
    }
}