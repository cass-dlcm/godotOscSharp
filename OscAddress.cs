using System;
using System.Text.RegularExpressions;

namespace godotOscSharp
{
    public class Address
    {
        public string Pattern { get; }

        public Address(string pattern)
        {
            if (string.IsNullOrEmpty(pattern))
            {
                throw new ArgumentException("Pattern cannot be null or empty");
            }
            if (!pattern.StartsWith("/"))
            {
                throw new ArgumentException("Pattern must start with a slash (/)");
            }
            if (pattern.Contains("//"))
            {
                throw new ArgumentException("Pattern cannot contain two consecutive slashes (//)");
            }
            if (pattern.Contains(" "))
            {
                throw new ArgumentException("Pattern cannot contain spaces");
            }
            if (!Regex.IsMatch(pattern, @"^/([\w\-\.\*]+/)*([\w\-\.\*]+|\*)$"))
            {
                throw new ArgumentException("Pattern contains invalid characters");
            }
            Pattern = pattern;
        }

        public byte[] ToBytes()
        {
            var result = new System.Collections.Generic.List<byte>();
            result.AddRange(System.Text.Encoding.ASCII.GetBytes(Pattern));
            result.Add(0);
            var padding = 4 - (result.Count % 4);
            for (int i = 0; i < padding; i++)
            {
                result.Add(0);
            }
            return result.ToArray();
        }

        public static Address Parse(byte[] data, ref int index)
        {
            var start = index;
            while (data[index] != 0)
            {
                index++;
            }
            var pattern = System.Text.Encoding.ASCII.GetString(data, start, index - start);
            index++;
            var padding = 4 - ((index - start) % 4);
            index += padding;
            return new Address(pattern);
        }

        public override string ToString()
        {
            return Pattern;
        }
    }
}
