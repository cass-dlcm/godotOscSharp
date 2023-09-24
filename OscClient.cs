using System;
using System.Net;
using System.Net.Http;
using Godot;

namespace godotOscSharp
{
    public class OscClient
    {
        private readonly OscReceiver receiver;
        private readonly OscSender sender;

        public OscClient(IPAddress host, int port)
        {
            receiver = new OscReceiver(port);
            sender = new OscSender(host, port);
        }

        public void SenderConnect() {
            sender.Connect();
        }

        public void Send(OscMessage message) {
            sender.Send(message);
        }

        public void AddMessageReceived(EventHandler<OscMessageReceivedEventArgs> func) {
            receiver.MessageReceived += func;
        }

        public void AddErrorReceived(EventHandler<OscErrorReceivedEventArgs> func) {
            receiver.ErrorReceived += func;
        }

        public void Dispose()
        {
            receiver.Dispose();
            sender.Dispose();
        }
    }
}