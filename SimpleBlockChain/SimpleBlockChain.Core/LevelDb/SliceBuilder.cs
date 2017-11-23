using System;
using System.Collections.Generic;

namespace SimpleBlockChain.Core.LevelDb
{
    public class SliceBuilder
    {
        private List<byte> data = new List<byte>();

        private SliceBuilder()
        {
        }

        public SliceBuilder Add(byte value)
        {
            data.Add(value);
            return this;
        }

        public SliceBuilder Add(ushort value)
        {
            data.AddRange(BitConverter.GetBytes(value));
            return this;
        }

        public SliceBuilder Add(uint value)
        {
            data.AddRange(BitConverter.GetBytes(value));
            return this;
        }

        public SliceBuilder Add(long value)
        {
            data.AddRange(BitConverter.GetBytes(value));
            return this;
        }

        public SliceBuilder Add(IEnumerable<byte> value)
        {
            data.AddRange(value);
            return this;
        }

        public SliceBuilder Add(string value)
        {
            data.AddRange(System.Text.Encoding.UTF8.GetBytes(value));
            return this;
        }

        public static SliceBuilder Begin()
        {
            return new SliceBuilder();
        }

        public static implicit operator Slice(SliceBuilder value)
        {
            return value.data.ToArray();
        }
    }
}
