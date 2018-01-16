using SimpleBlockChain.Core.Extensions;
using SimpleBlockChain.Core.Stores;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleBlockChain.Core.Compiler
{
    public interface ISolidityExecutor
    {
        SolidityExecutor Execute(IEnumerable<byte> scAddrPayload, IEnumerable<byte> addrPayload, IEnumerable<byte> data, bool addInTransaction = false);
    }

    public class SolidityExecutor : ISolidityExecutor
    {
        private readonly ISmartContractStore _smartContractStore;
        private SmartContracts _smartContracts;
        private SolidityProgram _solidityProgram;
    
        public SolidityExecutor(ISmartContractStore smartContractStore)
        {
            _smartContractStore = smartContractStore;
        }

        public SolidityExecutor Execute(IEnumerable<byte> scAddrPayload, IEnumerable<byte> addrPayload, IEnumerable<byte> data, bool addInTransaction = false)
        {
            if (scAddrPayload == null)
            {
                throw new ArgumentNullException(nameof(scAddrPayload));
            }

            if (addrPayload == null)
            {
                addrPayload = new byte[0];
            }

            var smartContract = _smartContractStore.GetSmartContracts().GetSmartContract(scAddrPayload);
            var defaultCallValue = new DataWord(new byte[] { 0x00 });
            _smartContracts = _smartContractStore.GetSmartContracts();
            var scode = smartContract.Code.ToHexString();
            _solidityProgram = new SolidityProgram(smartContract.Code.ToList(), new SolidityProgramInvoke(data, smartContract.Address, new DataWord(addrPayload.ToArray()), defaultCallValue, _smartContracts, addInTransaction));
            var vm = new SolidityVm();
            while (!_solidityProgram.IsStopped())
            {
                vm.Step(_solidityProgram);
            }

            return this;
        }

        public SolidityProgram GetProgram()
        {
            return _solidityProgram;
        }

        public SolidityExecutor Rollback()
        {
            _smartContracts.Rollback();
            return this;
        }

        public SolidityExecutor Commit()
        {
            _smartContracts.Commit();
            return this;
        }
    }
}
