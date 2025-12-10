using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;

namespace ChessGame
{
    internal class TCPServer
    {
        public TCPServer()
        {
            System.Diagnostics.Debug.WriteLine("TCPServer initialized");
            Task.Run(InitializeServer);
        }


        private async Task InitializeServer()
        {
            var eom = "<EOM>";
            IPHostEntry ipHostInfo = await Dns.GetHostEntryAsync(Dns.GetHostName());
            IPAddress localIpAddress = IPAddress.Loopback;
            IPEndPoint ipEndPoint = new(localIpAddress, 11_000);

            using Socket listener = new(
                ipEndPoint.AddressFamily,
                SocketType.Stream,
                ProtocolType.Tcp);

            listener.Bind(ipEndPoint);
            listener.Listen(100);
            
            var handler = await listener.AcceptAsync();
            while (true)
            {
                var buffer = new byte[1024];
                var recieved = await handler.ReceiveAsync(buffer, SocketFlags.None);
                var response = Encoding.UTF8.GetString(buffer, 0, recieved);
                if(response.IndexOf(eom) > -1)
                {
                    response = response.Replace(eom, "");
                    System.Diagnostics.Debug.WriteLine("Received: " + response);
                    break;
                }
            }
        }

    }
}
