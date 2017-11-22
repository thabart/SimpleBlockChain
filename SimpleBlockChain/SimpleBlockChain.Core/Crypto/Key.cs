using Org.BouncyCastle.Asn1.Sec;
using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Math.EC;
using Org.BouncyCastle.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;

namespace SimpleBlockChain.Core.Crypto
{
    public class Key
    {
        private ECPublicKeyParameters _publicKey;
        private ECPrivateKeyParameters _privateKey;  

        private Key(ECPrivateKeyParameters privateKey, ECPublicKeyParameters publicKey)
        {
            _privateKey = privateKey;
            _publicKey = publicKey;
        }

        public static Key Genererate()
        {
            var secureRandom = new SecureRandom();
            var curve = SecNamedCurves.GetByName("secp256k1");
            var domain = new ECDomainParameters(curve.Curve, curve.G, curve.N, curve.H);
            var gen = new ECKeyPairGenerator();
            var keygenParams = new ECKeyGenerationParameters(domain, secureRandom);
            gen.Init(keygenParams);
            var keyPair = gen.GenerateKeyPair();
            return new Key((ECPrivateKeyParameters)keyPair.Private, (ECPublicKeyParameters)keyPair.Public);
        }

        public static Key Deserialize(IEnumerable<byte> payload)
        {
            if (payload == null)
            {
                throw new ArgumentNullException(nameof(payload));
            }

            var c = SecNamedCurves.GetByName("secp256k1");
            var domain = new ECDomainParameters(c.Curve, c.G, c.N, c.H);
            ECCurve curve = domain.Curve;
            ECPoint q = curve.DecodePoint(payload.ToArray());
            var publicKey = new ECPublicKeyParameters(q, domain);
            return new Key(null, publicKey);
        }

        public BigInteger PrivateKey { get; private set; }

        public IEnumerable<byte> GetPublicKey()
        {
            return _publicKey.Q.GetEncoded();
        }

        public byte[] GetPublicKeyHashed()
        {
            var mySHA256 = SHA256Managed.Create(); // Explanations : https://bitcoin.org/en/developer-reference#address-conversion
            var myRIPEMD160 = RIPEMD160Managed.Create();
            var hashed = mySHA256.ComputeHash(GetPublicKey().ToArray());
            hashed = myRIPEMD160.ComputeHash(hashed);
            return hashed;
        }
        
        public IEnumerable<byte> Sign(IEnumerable<byte> payload)
        {
            if (payload == null)
            {
                throw new ArgumentNullException(nameof(payload));
            }

            var signer = SignerUtilities.GetSigner("ECDSA");
            signer.Init(true, _privateKey);
            signer.BlockUpdate(payload.ToArray(), 0, payload.Count());
            return signer.GenerateSignature();
        }
        
        public bool CheckSignature(IEnumerable<byte> payload, IEnumerable<byte> signature)
        {
            if (payload == null)
            {
                throw new ArgumentNullException(nameof(payload));
            }

            if (signature == null)
            {
                throw new ArgumentNullException(nameof(signature));
            }

            var signer = SignerUtilities.GetSigner("ECDSA");
            signer.Init(false, _publicKey);
            signer.BlockUpdate(payload.ToArray(), 0, payload.Count());
            return signer.VerifySignature(signature.ToArray());
        }
    }
}
