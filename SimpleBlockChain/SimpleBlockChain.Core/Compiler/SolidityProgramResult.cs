namespace SimpleBlockChain.Core.Compiler
{
    public class SolidityProgramResult
    {
        private byte[] _hReturn = ByteUtil.EMPTY_BYTE_ARRAY;

        public void SetHReturn(byte[] hReturn)
        {
            _hReturn = hReturn;
        }

        public byte[] GetHReturn()
        {
            return _hReturn;
        }
    }
}
