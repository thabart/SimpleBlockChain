namespace SimpleBlockChain.Core.Compiler
{
    public class SolidityMessageCall
    {
        public SolidityMessageCall(SolidityOpCodes opCode, DataWord gas, 
            DataWord codeAddress, DataWord endowment, DataWord inDataOffs, DataWord inDataSize)
        {
            OpCode = opCode;
            Gas = gas;
            CodeAddress = codeAddress;
            Endowment = endowment;
            InDataOffs = inDataOffs;
            InDataSize = inDataSize;
        }

        public SolidityMessageCall(SolidityOpCodes opCode, DataWord gas,
            DataWord codeAddress, DataWord endowment, DataWord inDataOffs, DataWord inDataSize, DataWord outDataOffs, DataWord outDataSize)
        {
            OpCode = opCode;
            Gas = gas;
            CodeAddress = codeAddress;
            Endowment = endowment;
            InDataOffs = inDataOffs;
            InDataSize = inDataSize;
            OutDataOffs = outDataOffs;
            OutDataSize = outDataSize;
        }

        public SolidityOpCodes OpCode { get; private set; }
        public DataWord Gas { get; private set; }
        public DataWord CodeAddress { get; private set; }
        public DataWord Endowment { get; private set; }
        public DataWord InDataOffs { get; private set; }
        public DataWord InDataSize { get; private set; }
        public DataWord OutDataOffs { get; private set; }
        public DataWord OutDataSize { get; private set; }
    }
}
