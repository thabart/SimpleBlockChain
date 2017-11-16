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
        private static SecureRandom _secureRandom;
        private static X9ECParameters _curve;
        private static ECDomainParameters _domain;        

        public Key(bool isCompressed = false)
        {
            _secureRandom = new SecureRandom();
            _curve = SecNamedCurves.GetByName("secp256k1");
            _domain = new ECDomainParameters(_curve.Curve, _curve.G, _curve.N, _curve.H);
            Generate();
        }

        public BigInteger PrivateKey { get; private set; }
        public byte[] PublicKey { get; private set; }

        /// <summary>
        /// Get public key.
        /// </summary>
        /// <param name="isCompressed"></param>
        /// <returns></returns>
        public IEnumerable<byte> GetPublicKey(bool isCompressed = false)
        {
            if (isCompressed)
            {
                var q = _publicKey.Q;
                return (new FpPoint(_domain.Curve, q.X, q.Y, true)).GetEncoded();
            }

            return _publicKey.Q.GetEncoded();
        }

        public byte[] GetPublicKeyHashed()
        {
            var mySHA256 = SHA256Managed.Create(); // Explanations : https://bitcoin.org/en/developer-reference#address-conversion
            var myRIPEMD160 = RIPEMD160Managed.Create();
            var hashed = mySHA256.ComputeHash(PublicKey);
            hashed = myRIPEMD160.ComputeHash(hashed);
            return hashed;
        }

        /// <summary>
        ///  Sign the payload.
        /// </summary>
        /// <param name="payload"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Check the signature.
        /// </summary>
        /// <param name="payload"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Generate a new key (Private & Public).
        /// </summary>
        private void Generate()
        {
            var gen = new ECKeyPairGenerator();
            var keygenParams = new ECKeyGenerationParameters(_domain, _secureRandom);
            gen.Init(keygenParams);
            var keyPair = gen.GenerateKeyPair();
            _privateKey = (ECPrivateKeyParameters)keyPair.Private;
            _publicKey = (ECPublicKeyParameters)keyPair.Public;
        }
    }
}
