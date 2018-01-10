using System;
using System.Linq;

namespace SimpleBlockChain.Core.Helpers
{
    public class ByteBuffer
    {
        private readonly byte[] _buffer;

        private ByteBuffer(int nb)
        {
            _buffer = new byte[nb];
        }

        public static ByteBuffer Allocate(int i)
        {
            var instance = new ByteBuffer(i);
            return instance;
        }

        public ByteBuffer PutInt(int i)
        {
            var data = BitConverter.GetBytes(i);
            Array.Copy(data, 0, _buffer, _buffer.Length - data.Length, data.Length);
            return this;
        }

        public byte[] GetArray()
        {
            return _buffer.Reverse().ToArray();
        }
    }
}
