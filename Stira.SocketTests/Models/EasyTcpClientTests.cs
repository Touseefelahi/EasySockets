using Microsoft.VisualStudio.TestTools.UnitTesting;
using Stira.Socket.Interfaces;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Stira.Socket.Models.Tests
{
    [TestClass()]
    public class EasyTcpClientTests
    {
        [TestMethod()]
        public async Task ConnectAsyncTest()
        {
            var threadID = Thread.CurrentThread.ManagedThreadId;
            EasyTcpClient easySocket = new EasyTcpClient() { Ip = "192.168.10.227", Port = 4545 };
            await Task.Run(async () => await easySocket.ConnectAsync(dataReady).ConfigureAwait(false)).ConfigureAwait(false);
        }

        private void dataReady(IReplyPacket obj)
        {
            List<byte> datareadyByte = obj.Reply;
            var threadID = Thread.CurrentThread.ManagedThreadId;
        }
    }
}