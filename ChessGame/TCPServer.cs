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
        private Socket? listener;
        private Socket? clientSocket;
        public TCPServer()
        {
            System.Diagnostics.Debug.WriteLine("TCPServer initialized");

            // TODO: Consider using CancellationToken to manage task lifetimes
            // TODO: Get rid of Task.Run if possible for better control over async flow
            Task.Run(StartUDPBroadcast);
            Task.Run(InitializeServer);
        }

        private async Task StartUDPBroadcast()
        {
            using var udp = new UdpClient();
            udp.EnableBroadcast = true;
            var data = Encoding.UTF8.GetBytes("CLIENTFOUND");
            var endpoint = new IPEndPoint(IPAddress.Broadcast, 11001);

            while (true)
            {
                await udp.SendAsync(data, data.Length, endpoint);
                await Task.Delay(1000);
            }
        }

        private async Task StartListening()
        {
            while (true)
            {
                var eom = "<EOM>";
                var buffer = new byte[1024];
                var recieved = await clientSocket.ReceiveAsync(buffer, SocketFlags.None);
                var response = Encoding.UTF8.GetString(buffer, 0, recieved);
                if (response.IndexOf(eom) > -1)
                {
                    response = response.Replace(eom, "");
                    System.Diagnostics.Debug.WriteLine("CLIENT -> SERVER:" + response);
                    response = null;
                }
            }
        }

        public async Task SendMessage(string input)
        {
            var eom = "<EOM>";
            var message = input + eom;
            var messageBytes = Encoding.UTF8.GetBytes(message);
            _ = await clientSocket.SendAsync(messageBytes, SocketFlags.None);
            message = message.Replace(eom, "");
            //System.Diagnostics.Debug.WriteLine("Sent:" + message);
        }



        private async Task InitializeServer()
        {
            IPHostEntry ipHostInfo = await Dns.GetHostEntryAsync(Dns.GetHostName());
            IPAddress localIpAddress = IPAddress.Any;
            IPEndPoint ipEndPoint = new(localIpAddress, 11_000);

            listener = new Socket(
                ipEndPoint.AddressFamily,
                SocketType.Stream,
                ProtocolType.Tcp);

            listener.Bind(ipEndPoint);
            listener.Listen(100);
            clientSocket = await listener.AcceptAsync();
            StartListening();

        }

    }
}
