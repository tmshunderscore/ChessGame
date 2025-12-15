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
        
        private IPAddress GetServerIP()
        {
            using UdpClient udpClient = new UdpClient(11001);

            while (true)
            {
                var result = udpClient.ReceiveAsync();
                var message = Encoding.UTF8.GetString(result.Result.Buffer);

                if(message.StartsWith("CLIENTFOUND"))
                {
                    return result.Result.RemoteEndPoint.Address;
                }
            }
        }

        public async Task InitializeClient()
        {
            IPAddress serverIP = GetServerIP();
            var eom = "<EOM>";
            IPEndPoint ipEndPoint = new(serverIP, 11_000);

            using Socket client = new(
                ipEndPoint.AddressFamily,
                SocketType.Stream,
                ProtocolType.Tcp);
            await client.ConnectAsync(ipEndPoint);
                var message = "If you see this I'm the best programmer that has ever lived"+eom;
                var messageBytes = Encoding.UTF8.GetBytes(message);
                _ = await client.SendAsync(messageBytes, SocketFlags.None);
                message = message.Replace(eom, "");
            System.Diagnostics.Debug.WriteLine("Sent: " + message);
        }
    }
}
