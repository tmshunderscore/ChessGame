using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;

namespace ChessGame
{
    internal class TCPClient
    {
        public TCPClient() {
            System.Diagnostics.Debug.WriteLine("TCPClient initialized");
            Task.Run(InitializeClient);
        }

        private async Task InitializeClient()
        {
            var eom = "<EOM>";
            IPHostEntry ipHostInfo = await Dns.GetHostEntryAsync(Dns.GetHostName());
            IPAddress localIpAddress = IPAddress.Loopback;
            IPEndPoint ipEndPoint = new(localIpAddress, 11_000);

            using Socket client = new(
                ipEndPoint.AddressFamily,
                SocketType.Stream,
                ProtocolType.Tcp);
            await client.ConnectAsync(ipEndPoint);
                var message = "d2d4"+eom;
                var messageBytes = Encoding.UTF8.GetBytes(message);
                _ = await client.SendAsync(messageBytes, SocketFlags.None);
                message = message.Replace(eom, "");
            System.Diagnostics.Debug.WriteLine("Sent: " + message);
        }
    }
}
