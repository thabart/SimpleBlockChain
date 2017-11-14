using Org.BouncyCastle.Asn1.Sec;
using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Math.EC;
using Org.BouncyCastle.Security;
using SimpleBlockChain.Core.Encoding;
using System.Security.Cryptography;

namespace SimpleBlockChain.Core.Crypto
{
    public class Key
    {
        private static SecureRandom _secureRandom;
        private static X9ECParameters _curve;
        private static ECDomainParameters _domain;
        

        public Key(bool isCompressed = false)
        {
            _secureRandom = new SecureRandom();
            _curve = SecNamedCurves.GetByName("secp256k1");
            _domain = new ECDomainParameters(_curve.Curve, _curve.G, _curve.N, _curve.H);
            Generate(isCompressed);
        }

        public BigInteger PrivateKey { get; private set; }
        public byte[] PublicKey { get; private set; }
        public bool IsCompressed { get; private set; }

        public byte[] GetPublicKeyHashed()
        {
            var mySHA256 = SHA256Managed.Create(); // Explanations : https://bitcoin.org/en/developer-reference#address-conversion
            var myRIPEMD160 = RIPEMD160Managed.Create();
            var hashed = mySHA256.ComputeHash(PublicKey);
            hashed = myRIPEMD160.ComputeHash(hashed);
            return hashed;
        }

        private void Generate(bool isCompressed)
        {
            var gen = new ECKeyPairGenerator();
            var keygenParams = new ECKeyGenerationParameters(_domain, _secureRandom);
            gen.Init(keygenParams);
            var keyPair = gen.GenerateKeyPair();
            var privParams = (ECPrivateKeyParameters)keyPair.Private;
            var publicParams = (ECPublicKeyParameters)keyPair.Public;
            PrivateKey = privParams.D;
            IsCompressed = isCompressed;
            if (isCompressed)
            {
                var q = publicParams.Q;
                PublicKey = (new FpPoint(_domain.Curve, q.X, q.Y, true)).GetEncoded();
            }
            else
            {
                PublicKey = publicParams.Q.GetEncoded();
            }
        }
    }
}
