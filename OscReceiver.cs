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
            // Create a UDP client with the given port
            udpClient = new UdpClient(port);

            // Create a thread for listening to incoming messages
            listenThread = new Thread(new ThreadStart(Listen));

            // Set the running flag to true
            running = true;

            // Start the thread
            listenThread.Start();
        }

        // A method that listens to incoming messages
        private void Listen()
        {
            // While the receiver is running
            while (running)
            {
                try
                {
                    // Receive data from any source
                    IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);
                    byte[] data = udpClient.Receive(ref remoteEndPoint);
                    // Parse the data to an OSC message
                    if (data[0] == 0x2f)
                    {
                        OscMessage message = OscMessage.Parse(data);
                        MessageReceived?.Invoke(this, new OscMessageReceivedEventArgs(message, remoteEndPoint.Address.ToString(), remoteEndPoint.Port));
                    }
                    else
                    {
                        // GD.Print(string.Join(", ", data));
                        OscBundle bundle = OscBundle.Parse(data);
                        foreach (var message in bundle.Messages)
                        {
                            MessageReceived?.Invoke(this, new OscMessageReceivedEventArgs(message, remoteEndPoint.Address.ToString(), remoteEndPoint.Port));
                        }
                    }
                }
                catch (Exception e)
                {
                    // If an exception occurs, invoke the error received event with the exception message
                    ErrorReceived?.Invoke(this, new OscErrorReceivedEventArgs(e.Message));
                }
            }
        }

        // A method that disposes the receiver and releases resources
        public void Dispose()
        {
            // Set the running flag to false
            running = false;

            // Close the UDP client
            udpClient.Close();

            // Join the thread
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
