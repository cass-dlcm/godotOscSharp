/*
    godotOscSharp
    Copyright (C) 2023  Cassandra de la Cruz-Munoz

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU Affero General Public License as published
    by the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU Affero General Public License for more details.

    You should have received a copy of the GNU Affero General Public License
    along with this program.  If not, see <https://www.gnu.org/licenses/>.
    */

using System;
using System.Net;
using System.Net.Sockets;

namespace godotOscSharp
{
    public class OscSender : IDisposable
    {
        private readonly UdpClient udpClient;

        private readonly IPAddress host;

        private readonly int port;

        public OscSender(IPAddress host, int port)
        {
            udpClient = new UdpClient(port);
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
