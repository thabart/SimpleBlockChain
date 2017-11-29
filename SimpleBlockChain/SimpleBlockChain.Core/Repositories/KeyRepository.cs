using Newtonsoft.Json.Linq;
using Org.BouncyCastle.Math;
using SimpleBlockChain.Core.Crypto;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;

namespace SimpleBlockChain.Core.Repositories
{
    public class KeyRepository
    {
        private object obj = new object();
        private const string _fileName = "wallet.json";
        private static byte[] _salt = new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 };

        public KeyRepository()
        {
            Keys = new List<Key>();
        }

        public IList<Key> Keys { get; set; }

        public bool Exists()
        {
            return File.Exists(GetPath());
        }

        public void Load(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentNullException(nameof(password));
            }

            if (!File.Exists(GetPath()))
            {
                return;
            }

            var encrypted = File.ReadAllText(GetPath());
            byte[] bytesBuff = Convert.FromBase64String(encrypted);
            using (var aes = Aes.Create())
            {
                Rfc2898DeriveBytes crypto = new Rfc2898DeriveBytes(password, _salt);
                aes.Key = crypto.GetBytes(32);
                aes.IV = crypto.GetBytes(16);
                using (MemoryStream mStream = new MemoryStream())
                {
                    using (CryptoStream cStream = new CryptoStream(mStream, aes.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cStream.Write(bytesBuff, 0, bytesBuff.Length);
                        cStream.Dispose();
                    }
                    var json = System.Text.Encoding.Unicode.GetString(mStream.ToArray());
                    var jArr = JToken.Parse(json) as JArray;
                    if (jArr != null)
                    {
                        var keys = new List<Key>();
                        foreach(var jToken in jArr)
                        {
                            var publicKey = jToken.Value<string>("publicKey");
                            var privateKey = jToken.Value<string>("privateKey");
                            keys.Add(Key.Deserialize(new BigInteger(publicKey), new BigInteger(privateKey)));
                        }

                        Keys = keys;
                    }
                }
            }
        }

        public void Save(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentNullException(nameof(password));
            }

            lock(obj)
            {
                if (File.Exists(GetPath()))
                {
                    File.Delete(GetPath());
                }

                File.Create(GetPath()).Dispose();
            }

            var jArr = new JArray();
            foreach(var key in Keys)
            {
                jArr.Add(GetJson(key));
            }
            
            var bytesBuff = System.Text.Encoding.Unicode.GetBytes(jArr.ToString());
            using (var aes = Aes.Create())
            {
                Rfc2898DeriveBytes crypto = new Rfc2898DeriveBytes(password, _salt);
                aes.Key = crypto.GetBytes(32);
                aes.IV = crypto.GetBytes(16);
                lock (obj)
                {
                    using (MemoryStream mStream = new MemoryStream())
                    {
                        using (CryptoStream cStream = new CryptoStream(mStream, aes.CreateEncryptor(), CryptoStreamMode.Write))
                        {
                            cStream.Write(bytesBuff, 0, bytesBuff.Length);
                            cStream.Dispose();
                        }

                        File.WriteAllText(GetPath(), System.Convert.ToBase64String(mStream.ToArray()));
                    }
                }
            }
        }

        private static JObject GetJson(Key key)
        {
            var publicKey = new BigInteger(key.GetPublicKey().ToArray());
            var privateKey = key.GetPrivateKey();
            var json = new JObject();
            json.Add("publicKey", publicKey.ToString());
            json.Add("privateKey", privateKey.ToString());
            return json;
        } 

        private static string GetPath()
        {
            string path = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            return Path.Combine(path, _fileName);
        }
    }
}
