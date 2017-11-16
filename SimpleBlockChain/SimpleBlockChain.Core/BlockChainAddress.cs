using SimpleBlockChain.Core.Crypto;
using SimpleBlockChain.Core.Encoding;
using SimpleBlockChain.Core.Exceptions;
using SimpleBlockChain.Core.Transactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;

namespace SimpleBlockChain.Core
{
    public class BlockChainAddress
    {
        private Key _key;

        public BlockChainAddress(ScriptTypes type, Networks network)
        {
            Type = type;
            Network = network;
        }

        public BlockChainAddress(ScriptTypes type, Networks network, IEnumerable<byte> publicKeyHash)
        {
            Type = type;
            Network = network;
            PublicKeyHash = publicKeyHash;
        }

        public void New()
        {
            _key = new Key();
        }

        public IEnumerable<byte> PublicKeyHash { get; private set; }
        public ScriptTypes Type { get; private set; }
        public Networks Network { get; private set; }
        public IEnumerable<byte> GetPublicKey()
        {
            return _key.PublicKey;
        }

        public Key GetKey()
        {
            return _key;
        }

        public static BlockChainAddress Parse(string hash)
        {
            if (hash == null)
            {
                throw new ArgumentNullException(nameof(hash));
            }

            var type = ScriptTypes.P2PKH;
            var network = Networks.MainNet;
            var decoded = Base58Encoding.Decode(hash);
            var versionPayload = decoded.First();
            if (versionPayload == 0x00)
            {
                type = ScriptTypes.P2PKH;
                network = Networks.MainNet;
            }
            else if (versionPayload == 0x6f)
            {
                type = ScriptTypes.P2PKH;
                network = Networks.TestNet;
            }
            else if (versionPayload == 0x05)
            {
                type = ScriptTypes.P2SH;
                network = Networks.MainNet;
            }
            else if (versionPayload == 0xc4)
            {
                type = ScriptTypes.P2SH;
                network = Networks.TestNet;
            }

            var checksum = decoded.Skip(decoded.Length - 4).Take(4);
            var content = decoded.Take(decoded.Length - 4);
            var mySHA256 = SHA256Managed.Create();
            var calculatedHash = mySHA256.ComputeHash(mySHA256.ComputeHash(content.ToArray()));
            var calculatedChecksum = calculatedHash.Take(4);
            if (!checksum.SequenceEqual(calculatedChecksum))
            {
                throw new ParseMessageException(ErrorCodes.InvalidChecksum);
            }

            return new BlockChainAddress(type, network, content.Skip(1));
        }

        public string GetAddress()
        {
            byte version = 0x00; // Address conversion : https://bitcoin.org/en/developer-reference#address-conversion
            if (Network == Networks.MainNet && Type == ScriptTypes.P2PKH)
            {
                version = 0x00;
            }
            else if (Network == Networks.TestNet && Type == ScriptTypes.P2PKH)
            {
                version = 0x6f;
            }
            else if (Network == Networks.MainNet && Type == ScriptTypes.P2SH)
            {
                version = 0x05;
            }
            else if (Network == Networks.TestNet && Type == ScriptTypes.P2SH)
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
            result.AddRange(publicKeyHashed);
            result.AddRange(checkSum);
            return Base58Encoding.Encode(result.ToArray());
        }
    }
}
