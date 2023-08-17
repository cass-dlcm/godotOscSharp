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
using System.Text.RegularExpressions;

namespace godotOscSharp
{
    public class OscAddress
    {
        public string Pattern { get; }

        public OscAddress(string pattern)
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

        public static OscAddress Parse(byte[] data, ref int index)
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
            return new OscAddress(pattern);
        }

        public override string ToString()
        {
            return Pattern;
        }
    }
}
