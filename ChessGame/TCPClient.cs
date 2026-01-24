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
        private string response;
        private Socket? client;
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

        public async Task<String> RecieveMessage()
        {
            var recievedMessage = "";
            while (true)
            {
                await Task.Delay(100);
                if (response != null)
                {
                    recievedMessage = response;
                    response = null;
                    return recievedMessage;
                }
            }
        }

        public async Task SendMessage(string input)
        {
            var eom = "<EOM>";
            var message = input+eom;
                var messageBytes = Encoding.UTF8.GetBytes(message); 
                _ = await client.SendAsync(messageBytes, SocketFlags.None);
                message = message.Replace(eom, "");
            //System.Diagnostics.Debug.WriteLine("Sent:" + message);
        }

        public async Task InitializeClient()
        {
            IPAddress serverIP = GetServerIP();
            IPEndPoint ipEndPoint = new(serverIP, 11_000);

            client = new Socket(
                ipEndPoint.AddressFamily,
                SocketType.Stream,
                ProtocolType.Tcp);
            await client.ConnectAsync(ipEndPoint);
            StartListening();
        }

        private async Task StartListening() 
        { 
            while (true)
            {
                var eom = "<EOM>";
                var buffer = new byte[1024];
                var recieved = await client.ReceiveAsync(buffer, SocketFlags.None);
                response = Encoding.UTF8.GetString(buffer, 0, recieved);
                if (response.IndexOf(eom) > -1)
                {
                    response = response.Replace(eom, "");
                    System.Diagnostics.Debug.WriteLine("SERVER -> CLIENT:" + response);
                }
            }
        }

    }
}
