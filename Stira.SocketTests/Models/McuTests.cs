using Microsoft.VisualStudio.TestTools.UnitTesting;
using Stira.Socket.Services;
using System.Threading.Tasks;

namespace Stira.Socket.Models.Tests
{
    /// <summary>
    /// Server must be listening on the MCU ip with specific port, and must be replying some data to
    /// test these methods
    /// </summary>
    [TestClass()]
    public class McuTests
    {
        private readonly Mcu mcu;

        public McuTests()
        {
            //Take current system IP as MCU IP
            mcu = new Mcu
            {
                IP = CommonUtility.GetIPAddress()
            };
        }

        public ListenerTcp ListenerTcp { get; }

        [TestMethod()]
        public async Task SendTcpAsyncTest()
        {
            var packet = new byte[] { 0x01, 0x02, 0x03, 0x04 };
            var reply = await mcu.SendTcpAsync(packet).ConfigureAwait(false);
            if (reply.IsSent)
            {
            }
            if (reply.IsSentAndReplyReceived)
            {
            }
            if (reply.Error != null)
            {
            }
        }

        [TestMethod()]
        public async Task SendUdpAsyncTest()
        {
            var packet = new byte[] { 0x01, 0x02, 0x03, 0x04 };
            await mcu.SendUdpAsync(packet, replyRequired: true).ConfigureAwait(false);
        }
    }
}