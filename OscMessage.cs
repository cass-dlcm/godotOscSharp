using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Godot;

namespace godotOscSharp
{
    public class OscMessage
    {
        public Address Address { get; }

        public List<OscArgument> Data { get; }

        public OscMessage(Address address, List<OscArgument> data)
        {
            Address = address;
            Data = data;
        }

        public static OscMessage Parse(byte[] data)
        {
            var index = 0;
            var address = Address.Parse(data, ref index);
            var start = index;
            while (data[index] != 0)
            {
                index++;
            }
            var pattern = System.Text.Encoding.ASCII.GetString(data, start, index - start);
            while (data[index] == 0)
            {
                index++;
            }
            var dataList = new List<OscArgument>();
            for (var items = 1; items < pattern.Length; items++)
            {
                dataList.Add(OscArgument.Parse(data, ref index, pattern[items]));
            }
            return new OscMessage(address, dataList);
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append(Address.ToString());
            foreach (var d in Data)
            {
                sb.Append(" ");
                sb.Append(d.ToString());
            }
            return sb.ToString();
        }

        public byte[] ToBytes()
        {
            var result = new System.Collections.Generic.List<byte>();
            result.AddRange(Address.ToBytes().ToList<byte>());
            result.Add(0x2c);
            for (var i = 0; i < Data.Count(); i++) {
                result.Add(BitConverter.GetBytes(Data[i].Type).ToList<byte>()[0]);
            }
            var padding = 4 - (result.Count % 4);
            for (int i = 0; i < padding; i++)
            {
                result.Add(0);
            }
            for (var i = 0; i < Data.Count(); i++) {
                result.AddRange(Data[i].ToBytes().ToList<byte>());
            }
            return result.ToArray();
        }
    }
}