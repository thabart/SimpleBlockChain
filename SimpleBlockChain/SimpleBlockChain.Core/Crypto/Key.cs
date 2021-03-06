﻿using Newtonsoft.Json.Linq;
using Org.BouncyCastle.Asn1.Sec;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Security;
using SimpleBlockChain.Core.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;

namespace SimpleBlockChain.Core.Crypto
{
    public class Key
    {
        private const string _publicKeyName = "public_key";
        private const string _privateKeyName = "private_key";
        private ECPublicKeyParameters _publicKey;
        private ECPrivateKeyParameters _privateKey;  

        private Key(ECPrivateKeyParameters privateKey, ECPublicKeyParameters publicKey)
        {
            _privateKey = privateKey;
            _publicKey = publicKey;
        }

        public IEnumerable<byte> GetSignature()
        {
            var payload = System.Text.Encoding.UTF8.GetBytes(Constants.DEFAULT_SIGNATURE_CONTENT);
            return Sign(payload);
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

        public static Key Deserialize(BigInteger publicKey, BigInteger privateKey)
        {
            if (publicKey == null)
            {
                throw new ArgumentNullException(nameof(publicKey));
            }

            if (privateKey == null)
            {
                throw new ArgumentNullException(nameof(privateKey));
            }

            var c = SecNamedCurves.GetByName("secp256k1");
            var domain = new ECDomainParameters(c.Curve, c.G, c.N, c.H);
            Org.BouncyCastle.Math.EC.ECCurve curve = domain.Curve;
            Org.BouncyCastle.Math.EC.ECPoint q = curve.DecodePoint(publicKey.ToByteArray());
            var pubK = new ECPublicKeyParameters(q, domain);
            var prK = new ECPrivateKeyParameters(privateKey, domain);
            return new Key(prK, pubK);
        }

        public static Key Deserialize(IEnumerable<byte> payload)
        {
            if (payload == null)
            {
                throw new ArgumentNullException(nameof(payload));
            }

            var c = SecNamedCurves.GetByName("secp256k1");
            var domain = new ECDomainParameters(c.Curve, c.G, c.N, c.H);
            Org.BouncyCastle.Math.EC.ECCurve curve = domain.Curve;
            Org.BouncyCastle.Math.EC.ECPoint q = curve.DecodePoint(payload.ToArray());
            var publicKey = new ECPublicKeyParameters(q, domain);
            return new Key(null, publicKey);
        }

        public BigInteger GetPrivateKey()
        {
            return _privateKey.D;
        }

        public IEnumerable<byte> GetPublicKey()
        {
            return _publicKey.Q.GetEncoded();
        }

        public byte[] GetPublicKeyHashed()
        {
            var mySHA256 = SHA256.Create(); // Explanations : https://bitcoin.org/en/developer-reference#address-conversion
            var myRIPEMD160 = new RIPEMD160Managed();
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

        public JObject GetJson()
        {
            var result = new JObject();
            var publicKey = GetPublicKey();
            if (publicKey == null)
            {
                throw new SerializeException(string.Format(ErrorCodes.ParameterMissing, _publicKeyName));
            }

            var privateKey = GetPrivateKey();
            if (privateKey == null)
            {
                throw new SerializeException(string.Format(ErrorCodes.ParameterMissing, _privateKey));
            }

            result.Add(_publicKeyName, (new BigInteger(publicKey.ToArray())).ToString());
            result.Add(_privateKeyName, GetPrivateKey().ToString());
            return result;
        }

        public static Key FromJson(JObject jObj)
        {
            if (jObj == null)
            {
                throw new ArgumentNullException(nameof(jObj));
            }

            JToken publicKeyToken = null;
            if (!jObj.TryGetValue(_publicKeyName, out publicKeyToken))
            {
                throw new ParseException(string.Format(ErrorCodes.ParameterMissing, _publicKeyName));
            }
            
            JToken privateKeyToken = null;
            if (!jObj.TryGetValue(_privateKeyName, out privateKeyToken))
            {
                throw new ParseException(string.Format(ErrorCodes.ParameterMissing, _privateKeyName));
            }

            BigInteger publicKey,
                privateKey;
            publicKey = new BigInteger(publicKeyToken.ToString());
            privateKey = new BigInteger(privateKeyToken.ToString());
            return Deserialize(publicKey, privateKey);
        }
    }
}
