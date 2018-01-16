using System.Collections.Generic;

namespace SimpleBlockChain.Core.Compiler
{
    public class SolidityProgramResult
    {
        private byte[] _hReturn = ByteUtil.EMPTY_BYTE_ARRAY;
        private bool _revert;
        private List<SolidityLogInfo> _logInfos;

        public SolidityProgramResult()
        {
            _logInfos = new List<SolidityLogInfo>();
        }

        public void AddLogInfo(SolidityLogInfo logInfo)
        {
            _logInfos.Add(logInfo);
        }

        public void SetHReturn(byte[] hReturn)
        {
            _hReturn = hReturn;
            _revert = false;
        }

        public byte[] GetHReturn()
        {
            return _hReturn;
        }

        public List<SolidityLogInfo> GetLogs()
        {
            return _logInfos;
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
