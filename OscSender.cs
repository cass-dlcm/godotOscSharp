using System;
using System.Net;
using System.Net.Sockets;

namespace godotOscSharp
{
    public class OscSender : IDisposable
    {
        private UdpClient udpClient;

        private IPAddress host;

        private int port;

        public OscSender(IPAddress host, int port)
        {
            udpClient = new UdpClient(0);
            this.host = host;
            this.port = port;
        }

        public void Connect()
        {
            udpClient.Connect(host, port);
        }

        public void Send(OscMessage message)
        {
            byte[] data = message.ToBytes();
            udpClient.Send(data, data.Length);
        }

        public void Dispose()
        {
            udpClient.Close();
        }
    }
}
