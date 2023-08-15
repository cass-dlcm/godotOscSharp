using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Godot;

namespace godotOscSharp
{
    public class OscBundle
    {
        public long TimeTag { get; }
        public List<OscMessage> Messages { get; }

        public OscBundle(long timeTag, List<OscMessage> messages)
        {
            TimeTag = timeTag;
            Messages = messages;
        }

        public byte[] ToBytes()
        {
            var result = new List<byte>();
            result.AddRange(Encoding.ASCII.GetBytes("#bundle"));
            result.Add(0);
            result.AddRange(BitConverter.GetBytes(TimeTag));
            foreach (var message in Messages)
            {
                var messageBytes = message.ToBytes();
                result.AddRange(BitConverter.GetBytes(messageBytes.Length));
                result.AddRange(messageBytes);
            }
            return result.ToArray();
        }

        public static OscBundle Parse(byte[] data)
        {
            var index = 0;
            var identifier = Encoding.ASCII.GetString(data, index, 7);
            if (identifier != "#bundle")
            {
                throw new ArgumentException("Invalid bundle identifier");
            }
            index += 8;
            var timeTag = BitConverter.ToInt64(data, index);
            index += 8;
            var messages = new List<OscMessage>();
            while (index < data.Length)
            {
                var size = BitConverter.ToInt32(data, index);
                index += 4;
                var messageData = data.Skip(index).Take(size).ToArray();
                var message = OscMessage.Parse(messageData);
                messages.Add(message);
                index += size;
            }
            return new OscBundle(timeTag, messages);
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("OscBundle: ");
            sb.Append("TimeTag: ");
            sb.Append(TimeTag);
            sb.Append(", ");
            sb.Append("Messages: ");
            sb.Append("[");
            foreach (var message in Messages)
            {
                sb.Append(message.ToString());
                sb.Append(", ");
            }
            sb.Remove(sb.Length - 2, 2);
            sb.Append("]");
            return sb.ToString();
        }
    }
}
