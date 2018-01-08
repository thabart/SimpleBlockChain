namespace SimpleBlockChain.Core.Compiler
{
    public class SolidityProgramResult
    {
        private byte[] _hReturn = ByteUtil.EMPTY_BYTE_ARRAY;
        private bool _revert;

        public void SetHReturn(byte[] hReturn)
        {
            _hReturn = hReturn;
            _revert = false;
        }

        public byte[] GetHReturn()
        {
            return _hReturn;
        }

        public bool IsRevert()
        {
            return _revert;
        }

        public void SetRevert()
        {
            _revert = true;
        }
    }
}
