namespace SimpleBlockChain.Core.Converters
{
    internal sealed class BigEndianBitConverter
    {
        public byte[] GetBytes(decimal value)
        {
            byte[] buffer = new byte[16];
            int[] parts = decimal.GetBits(value);
            for (int i = 0; i < 4; i++)
            {
                var v = parts[i];
                int endOffset = (i * 4) + 4 - 1;
                buffer[endOffset - i] = unchecked((byte)(v & 0xff));
                v = v >> 8;
            }

            return buffer;
        }
    }
}
