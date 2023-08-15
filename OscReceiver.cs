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

using Godot;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace godotOscSharp
{
    // A class that represents an OSC receiver
    public class OscReceiver : IDisposable
    {
        // The UDP client for receiving data
        private UdpClient udpClient;

        // The thread for listening to incoming messages
        private Thread listenThread;

        // The flag to indicate if the receiver is running
        private bool running;

        // The constructor that takes a port number
        public OscReceiver(int port)
        {
            udpClient = new UdpClient(port);
            listenThread = new Thread(new ThreadStart(Listen));
            running = true;
            listenThread.Start();
        }

        // A method that listens to incoming messages
        private void Listen()
        {
            while (running)
            {
                try
                {
                    IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);
                    byte[] data = udpClient.Receive(ref remoteEndPoint);
                    if (data[0] == 0x2f)
                    {
                        OscMessage message = OscMessage.Parse(data);
                        MessageReceived?.Invoke(this, new OscMessageReceivedEventArgs(message, remoteEndPoint.Address.ToString(), remoteEndPoint.Port));
                    }
                    else
                    {
                        OscBundle bundle = OscBundle.Parse(data);
                        foreach (var message in bundle.Messages)
                        {
                            MessageReceived?.Invoke(this, new OscMessageReceivedEventArgs(message, remoteEndPoint.Address.ToString(), remoteEndPoint.Port));
                        }
                    }
                }
                catch (Exception e)
                {
                    ErrorReceived?.Invoke(this, new OscErrorReceivedEventArgs(e.Message));
                }
            }
        }

        // A method that disposes the receiver and releases resources
        public void Dispose()
        {
            running = false;
            udpClient.Close();
            listenThread.Join();
        }

        // An event that occurs when a message is received
        public event EventHandler<OscMessageReceivedEventArgs> MessageReceived;

        // An event that occurs when an error is received
        public event EventHandler<OscErrorReceivedEventArgs> ErrorReceived;
    }

    // A class that contains the data for the message received event
    public class OscMessageReceivedEventArgs : EventArgs
    {
        // The OSC message that was received
        public OscMessage Message { get; }

        // The sender's IP address
        public string IPAddress { get; }

        // The sender's port number
        public int Port { get; }

        // The constructor that takes a message, an IP address and a port number
        public OscMessageReceivedEventArgs(OscMessage message, string ipAddress, int port)
        {
            Message = message;
            IPAddress = ipAddress;
            Port = port;
        }
    }

    // A class that contains the data for the error received event
    public class OscErrorReceivedEventArgs : EventArgs
    {
        // The error message that was received
        public string ErrorMessage { get; }

        // The constructor that takes an error message
        public OscErrorReceivedEventArgs(string errorMessage)
        {
            ErrorMessage = errorMessage;
        }
    }
}
