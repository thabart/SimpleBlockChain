using System;
using System.Collections.Generic;

namespace SimpleBlockChain.Core.Compiler
{
    public class SolidityOpCode
    {
        private static SolidityOpCode _instance;
        private static Dictionary<byte, SolidityOpCodes> intToTypeMap = new Dictionary<byte, SolidityOpCodes>();
        private static Dictionary<SolidityOpCodes, Tier> _opCodeToTier = new Dictionary<SolidityOpCodes, Tier>
        {
            { SolidityOpCodes.STOP, Tier.ZeroTier },
            { SolidityOpCodes.ADD, Tier.VeryLowTier },
            { SolidityOpCodes.MUL, Tier.LowTier },
            { SolidityOpCodes.SUB, Tier.VeryLowTier },
            { SolidityOpCodes.DIV, Tier.LowTier },
            { SolidityOpCodes.SDIV, Tier.LowTier },
            { SolidityOpCodes.MOD, Tier.LowTier },
            { SolidityOpCodes.SMOD, Tier.LowTier },
            { SolidityOpCodes.ADDMOD, Tier.MidTier },
            { SolidityOpCodes.MULMOD, Tier.MidTier },
            { SolidityOpCodes.EXP, Tier.SpecialTier },
            { SolidityOpCodes.SIGNEXTEND, Tier.LowTier },
            { SolidityOpCodes.LT, Tier.VeryLowTier },
            { SolidityOpCodes.GT, Tier.VeryLowTier },
            { SolidityOpCodes.SLT, Tier.VeryLowTier },
            { SolidityOpCodes.SGT, Tier.VeryLowTier },
            { SolidityOpCodes.EQ, Tier.VeryLowTier },
            { SolidityOpCodes.ISZERO, Tier.VeryLowTier },
            { SolidityOpCodes.AND, Tier.VeryLowTier },
            { SolidityOpCodes.OR, Tier.VeryLowTier },
            { SolidityOpCodes.XOR, Tier.VeryLowTier },
            { SolidityOpCodes.NOT, Tier.VeryLowTier },
            { SolidityOpCodes.BYTE, Tier.VeryLowTier },
            { SolidityOpCodes.SHA3, Tier.SpecialTier },
            { SolidityOpCodes.ADDRESS, Tier.BaseTier },
            { SolidityOpCodes.BALANCE, Tier.ExtTier },
            { SolidityOpCodes.ORIGIN, Tier.BaseTier },
            { SolidityOpCodes.CALLER, Tier.BaseTier },
            { SolidityOpCodes.CALLVALUE, Tier.BaseTier },
            { SolidityOpCodes.CALLDATALOAD, Tier.VeryLowTier },
            { SolidityOpCodes.CALLDATASIZE, Tier.BaseTier },
            { SolidityOpCodes.CALLDATACOPY, Tier.VeryLowTier },
            { SolidityOpCodes.CODESIZE, Tier.BaseTier },
            { SolidityOpCodes.CODECOPY, Tier.VeryLowTier },
            { SolidityOpCodes.RETURNDATASIZE, Tier.BaseTier },
            { SolidityOpCodes.RETURNDATACOPY, Tier.VeryLowTier },
            { SolidityOpCodes.GASPRICE, Tier.BaseTier },
            { SolidityOpCodes.EXTCODESIZE, Tier.ExtTier },
            { SolidityOpCodes.EXTCODECOPY, Tier.ExtTier },
            { SolidityOpCodes.BLOCKHASH, Tier.ExtTier },
            { SolidityOpCodes.COINBASE, Tier.BaseTier },
            { SolidityOpCodes.TIMESTAMP, Tier.BaseTier },
            { SolidityOpCodes.NUMBER, Tier.BaseTier },
            { SolidityOpCodes.DIFFICULTY, Tier.BaseTier },
            { SolidityOpCodes.GASLIMIT, Tier.BaseTier },
            { SolidityOpCodes.POP, Tier.BaseTier },
            { SolidityOpCodes.MLOAD, Tier.VeryLowTier },
            { SolidityOpCodes.MSTORE, Tier.VeryLowTier },
            { SolidityOpCodes.MSTORE8, Tier.VeryLowTier },
            { SolidityOpCodes.SLOAD, Tier.SpecialTier },
            { SolidityOpCodes.SSTORE, Tier.SpecialTier },
            { SolidityOpCodes.JUMP, Tier.MidTier },
            { SolidityOpCodes.JUMPI, Tier.HighTier },
            { SolidityOpCodes.PC, Tier.BaseTier },
            { SolidityOpCodes.MSIZE, Tier.BaseTier },
            { SolidityOpCodes.GAS, Tier.BaseTier },
            { SolidityOpCodes.JUMPDEST, Tier.SpecialTier },
            { SolidityOpCodes.PUSH1, Tier.VeryLowTier },
            { SolidityOpCodes.PUSH2, Tier.VeryLowTier },
            { SolidityOpCodes.PUSH3, Tier.VeryLowTier },
            { SolidityOpCodes.PUSH4, Tier.VeryLowTier },
            { SolidityOpCodes.PUSH5, Tier.VeryLowTier },
            { SolidityOpCodes.PUSH6, Tier.VeryLowTier },
            { SolidityOpCodes.PUSH7, Tier.VeryLowTier },
            { SolidityOpCodes.PUSH8, Tier.VeryLowTier },
            { SolidityOpCodes.PUSH9, Tier.VeryLowTier },
            { SolidityOpCodes.PUSH10, Tier.VeryLowTier },
            { SolidityOpCodes.PUSH11, Tier.VeryLowTier },
            { SolidityOpCodes.PUSH12, Tier.VeryLowTier },
            { SolidityOpCodes.PUSH13, Tier.VeryLowTier },
            { SolidityOpCodes.PUSH14, Tier.VeryLowTier },
            { SolidityOpCodes.PUSH15, Tier.VeryLowTier },
            { SolidityOpCodes.PUSH16, Tier.VeryLowTier },
            { SolidityOpCodes.PUSH17, Tier.VeryLowTier },
            { SolidityOpCodes.PUSH18, Tier.VeryLowTier },
            { SolidityOpCodes.PUSH19, Tier.VeryLowTier },
            { SolidityOpCodes.PUSH20, Tier.VeryLowTier },
            { SolidityOpCodes.PUSH21, Tier.VeryLowTier },
            { SolidityOpCodes.PUSH22, Tier.VeryLowTier },
            { SolidityOpCodes.PUSH23, Tier.VeryLowTier },
            { SolidityOpCodes.PUSH24, Tier.VeryLowTier },
            { SolidityOpCodes.PUSH25, Tier.VeryLowTier },
            { SolidityOpCodes.PUSH26, Tier.VeryLowTier },
            { SolidityOpCodes.PUSH27, Tier.VeryLowTier },
            { SolidityOpCodes.PUSH28, Tier.VeryLowTier },
            { SolidityOpCodes.PUSH29, Tier.VeryLowTier },
            { SolidityOpCodes.PUSH30, Tier.VeryLowTier },
            { SolidityOpCodes.PUSH31, Tier.VeryLowTier },
            { SolidityOpCodes.PUSH32, Tier.VeryLowTier },
            { SolidityOpCodes.DUP1, Tier.VeryLowTier },
            { SolidityOpCodes.DUP2, Tier.VeryLowTier },
            { SolidityOpCodes.DUP3, Tier.VeryLowTier },
            { SolidityOpCodes.DUP4, Tier.VeryLowTier },
            { SolidityOpCodes.DUP5, Tier.VeryLowTier },
            { SolidityOpCodes.DUP6, Tier.VeryLowTier },
            { SolidityOpCodes.DUP7, Tier.VeryLowTier },
            { SolidityOpCodes.DUP8, Tier.VeryLowTier },
            { SolidityOpCodes.DUP9, Tier.VeryLowTier },
            { SolidityOpCodes.DUP10, Tier.VeryLowTier },
            { SolidityOpCodes.DUP11, Tier.VeryLowTier },
            { SolidityOpCodes.DUP12, Tier.VeryLowTier },
            { SolidityOpCodes.DUP13, Tier.VeryLowTier },
            { SolidityOpCodes.DUP14, Tier.VeryLowTier },
            { SolidityOpCodes.DUP15, Tier.VeryLowTier },
            { SolidityOpCodes.DUP16, Tier.VeryLowTier },
            { SolidityOpCodes.SWAP1, Tier.VeryLowTier },
            { SolidityOpCodes.SWAP2, Tier.VeryLowTier },
            { SolidityOpCodes.SWAP3, Tier.VeryLowTier },
            { SolidityOpCodes.SWAP4, Tier.VeryLowTier },
            { SolidityOpCodes.SWAP5, Tier.VeryLowTier },
            { SolidityOpCodes.SWAP6, Tier.VeryLowTier },
            { SolidityOpCodes.SWAP7, Tier.VeryLowTier },
            { SolidityOpCodes.SWAP8, Tier.VeryLowTier },
            { SolidityOpCodes.SWAP9, Tier.VeryLowTier },
            { SolidityOpCodes.SWAP10, Tier.VeryLowTier },
            { SolidityOpCodes.SWAP11, Tier.VeryLowTier },
            { SolidityOpCodes.SWAP12, Tier.VeryLowTier },
            { SolidityOpCodes.SWAP13, Tier.VeryLowTier },
            { SolidityOpCodes.SWAP14, Tier.VeryLowTier },
            { SolidityOpCodes.SWAP15, Tier.VeryLowTier },
            { SolidityOpCodes.SWAP16, Tier.VeryLowTier },
            { SolidityOpCodes.CALL, Tier.SpecialTier },
            { SolidityOpCodes.RETURN, Tier.ZeroTier },
            { SolidityOpCodes.DELEGATECALL, Tier.SpecialTier },
            { SolidityOpCodes.STATICCALL, Tier.SpecialTier },
            { SolidityOpCodes.REVERT, Tier.ZeroTier },
            { SolidityOpCodes.SUICIDE, Tier.ZeroTier }
        };

        private SolidityOpCode()
        {
            foreach(SolidityOpCodes opCode in Enum.GetValues(typeof(SolidityOpCodes)))
            {
                var key = (byte)((byte)opCode & 0xFF);
                intToTypeMap.Add(key, opCode);
            }
        }

        public static SolidityOpCode Instance()
        {
            if (_instance == null)
            {
                _instance = new SolidityOpCode();
            }

            return _instance;
        }

        public SolidityOpCodes? GetCode(byte code)
        {
            var key = (byte)((byte)code & 0xFF);
            if (!intToTypeMap.ContainsKey(key))
            {
                return null;
            }

            return intToTypeMap[key];
        }

        public Tier? GetTier(byte code)
        {
            var opCode = GetCode(code);
            if (opCode == null)
            {
                return null;
            }

            return _opCodeToTier[opCode.Value];
        }

        public Tier GetTier(SolidityOpCodes opCode)
        {
            return _opCodeToTier[opCode];
        }
    }

    public enum Tier
    {
        ZeroTier = 0,
        BaseTier = 2,
        VeryLowTier = 3,
        LowTier = 5,
        MidTier = 8,
        HighTier = 10,
        ExtTier = 20,
        SpecialTier = 1,
        InvalidTier = 0
    }

    public enum SolidityOpCodes : byte
    {
        STOP = 0x00,
        ADD = 0x01,
        MUL = 0x02,
        SUB = 0x03,
        DIV = 0x04,
        SDIV = 0x05,
        MOD = 0x06,
        SMOD = 0x07,
        ADDMOD = 0x08,
        MULMOD = 0x09,
        EXP = 0x0a,
        SIGNEXTEND = 0x0b,
        LT = 0x10,
        GT = 0x11,
        SLT  = 0x12,
        SGT = 0x13,
        EQ = 0x14,
        ISZERO = 0x15,
        AND = 0x16,
        OR = 0x17,
        XOR = 0x18,
        NOT = 0x19,
        BYTE = 0x1a,
        SHA3 = 0x20,
        ADDRESS = 0x30,
        BALANCE = 0x31,
        ORIGIN = 0x32,
        CALLER = 0x33,
        CALLVALUE = 0x34,
        CALLDATALOAD = 0x35,
        CALLDATASIZE = 0x36,
        CALLDATACOPY = 0x37,
        CODESIZE = 0x38,
        CODECOPY = 0x39,
        RETURNDATASIZE = 0x3d,
        RETURNDATACOPY = 0x3e,
        GASPRICE = 0x3a,
        EXTCODESIZE = 0x3b,
        EXTCODECOPY = 0x3c,
        BLOCKHASH = 0x40,
        COINBASE = 0x41,
        TIMESTAMP = 0x42,
        NUMBER = 0x43,
        DIFFICULTY = 0x44,
        GASLIMIT = 0x45,
        POP = 0x50,
        MLOAD = 0x51,
        MSTORE = 0x52,
        MSTORE8 = 0x53,
        SLOAD = 0x54,
        SSTORE = 0x55,
        JUMP = 0x56,
        JUMPI = 0x57,
        PC = 0x58,
        MSIZE = 0x59,
        GAS = 0x5a,
        JUMPDEST = 0x5b,
        PUSH1 = 0x60,
        PUSH2 = 0x61,
        PUSH3 = 0x62,
        PUSH4 = 0x63,
        PUSH5 = 0x64,
        PUSH6 = 0x65,
        PUSH7 = 0x66,
        PUSH8 = 0x67,
        PUSH9 = 0x68,
        PUSH10 = 0x69,
        PUSH11 = 0x6a,
        PUSH12 = 0x6b,
        PUSH13 = 0x6c,
        PUSH14 = 0x6d,
        PUSH15 = 0x6e,
        PUSH16 = 0x6f,
        PUSH17 = 0x70,
        PUSH18 = 0x71,
        PUSH19 = 0x72,
        PUSH20 = 0x73,
        PUSH21 = 0x74,
        PUSH22 = 0x75,
        PUSH23 = 0x76,
        PUSH24 = 0x77,
        PUSH25 = 0x78,
        PUSH26 = 0x79,
        PUSH27 = 0x7a,
        PUSH28 = 0x7b,
        PUSH29 = 0x7c,
        PUSH30 = 0x7d,
        PUSH31 = 0x7e,
        PUSH32 = 0x7f,
        DELEGATECALL = 0xf4,
        STATICCALL = 0xfa,
        REVERT = 0xfd,
        SUICIDE = 0xff,
        DUP1 = 0x80,
        DUP2 = 0x81,
        DUP3 = 0x82,
        DUP4 = 0x83,
        DUP5 = 0x84,
        DUP6 = 0x85,
        DUP7 = 0x86,
        DUP8 = 0x87,
        DUP9 = 0x88,
        DUP10 = 0x89,
        DUP11 = 0x8a,
        DUP12 = 0x8b,
        DUP13 = 0x8c,
        DUP14 = 0x8d,
        DUP15 = 0x8e,
        DUP16 = 0x8f,
        SWAP1 = 0x90,
        SWAP2 = 0x91,
        SWAP3 = 0x92,
        SWAP4 = 0x93,
        SWAP5 = 0x94,
        SWAP6 = 0x95,
        SWAP7 = 0x96,
        SWAP8 = 0x97,
        SWAP9 = 0x98,
        SWAP10 = 0x99,
        SWAP11 = 0x9A,
        SWAP12 = 0x9b,
        SWAP13 = 0x9c,
        SWAP14 = 0x9d,
        SWAP15 = 0x9e,
        SWAP16 = 0x9f,
        RETURN = 0xf3,
        CALL = 0xf1
    }
}
