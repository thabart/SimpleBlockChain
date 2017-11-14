using SimpleBlockChain.Core.Crypto;
using SimpleBlockChain.Core.Encoding;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;

namespace SimpleBlockChain.Core
{
    public enum BlockChainAddressTypes
    {
        P2PKH,
        P2SH
    }

    public class BlockChainAddress
    {
        private BlockChainAddressTypes _type;
        private Networks _network;
        private Key _key;

        public BlockChainAddress(BlockChainAddressTypes type, Networks network)
        {
            _type = type;
            _network = network;
        }

        public void New()
        {
            _key = new Key();            
        }

        public string GetAddress()
        {
            byte version = 0x00; // Address conversion : https://bitcoin.org/en/developer-reference#address-conversion
            if (_network == Networks.MainNet && _type == BlockChainAddressTypes.P2PKH)
            {
                version = 0x00;
            }
            else if (_network == Networks.TestNet && _type == BlockChainAddressTypes.P2PKH)
            {
                version = 0x6f;
            }
            else if (_network == Networks.MainNet && _type == BlockChainAddressTypes.P2SH)
            {
                version = 0x05;
            }
            else if (_network == Networks.TestNet && _type == BlockChainAddressTypes.P2SH)
            {
                version = 0xc4;
            }
            var publicKeyHashed = _key.GetPublicKeyHashed();
            var content = new List<byte>();
            content.Add(version);
            content.AddRange(publicKeyHashed);
            var mySHA256 = SHA256Managed.Create();
            var hashedContent = mySHA256.ComputeHash(mySHA256.ComputeHash(content.ToArray()));
            var checkSum = hashedContent.Take(4);
            var result = new List<byte>();
            result.Add(version);
            result.AddRange(hashedContent);
            result.AddRange(checkSum);
            return Base58Encoding.Encode(result.ToArray());
        }
    }
}
