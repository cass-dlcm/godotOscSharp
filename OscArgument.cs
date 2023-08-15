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

namespace godotOscSharp
{
    // A class that represents a DWord
    public class OscArgument
    {
        // The value of the DWord as an unsigned integer
        public char Type { get; }
        public object Value { get; }

        // The constructor that takes an unsigned integer as the value
        public OscArgument(object value, char type)
        {
            Value = value;
            Type = type;
        }

        // A method that parses a byte array to a DWord
        public static OscArgument Parse(byte[] data, ref int index, char type)
        {
            // Use BitConverter to get the unsigned integer from the bytes at the given index in little-endian order
            object value = null;
            var start = index;
            switch (type) {
                case 'i':
                    value = BitConverter.ToInt32(data, index);
                    index += 4;
                    break;
                case 'f':
                    value = BitConverter.ToSingle(data, index);
                    index += 4;
                    break;
                case 's':
                    while (data[index] != 0) // Find the null terminator
                    {
                        index++;
                    }
                    value = System.Text.Encoding.ASCII.GetString(data, start, index - start);
                    while (data[index] == 0 && index < data.Length)
                    {
                        index++;
                    }
                    break;
                case 'h':
                    value = BitConverter.ToInt64(data, index);
                    index += 8;
                    break;
                case 'd':
                    value = BitConverter.ToDouble(data, index);
                    index += 8;
                    break;
                case 'T':
                    value = true;
                    break;
                case 'F':
                    value = false;
                    break;
                case 'N':
                    value = null;
                    break;
            }

            // Increment the index by 4 bytes

            // Return a new DWord instance with the value
            return new OscArgument(value, type);
        }

        public byte[] ToBytes()
        {
            switch (Type) {
                case 'i':
                    return BitConverter.GetBytes((int)Value);
                case 'f':
                    return BitConverter.GetBytes((float)Value);
                case 's':
                    var result = new System.Collections.Generic.List<byte>();
                    result.AddRange(System.Text.Encoding.ASCII.GetBytes((string)Value));
                    result.Add(0);
                    var padding = 4 - (result.Count % 4);
                    for (int i = 0; i < padding; i++)
                    {
                        result.Add(0);
                    }
                    return result.ToArray();
                case 'h':
                    return BitConverter.GetBytes((long)Value);
                case 'd':
                    return BitConverter.GetBytes((double)Value);
            }
            return new byte[0];
        }

        public override string ToString()
        {
            return Value.ToString();
        }
    }
}
