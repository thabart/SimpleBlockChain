using Newtonsoft.Json.Linq;
using Org.BouncyCastle.Math;
using SimpleBlockChain.Core.Crypto;
using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace SimpleBlockChain.Core.Repositories
{
    public class KeyRepository
    {
        private const string _fileName = "wallet.json";
        private byte[] _salt = new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 };

        public KeyRepository()
        {

        }

        public Key Key { get; set; }

        public void Load(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentNullException(nameof(password));
            }

            if (!File.Exists(GetPath()))
            {
                // TODO : EXCEPTION
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
                        cStream.Close();
                    }
                    var json = System.Text.Encoding.Unicode.GetString(mStream.ToArray());
                    var jToken = JToken.Parse(json);
                    var publicKey = jToken.Value<string>("publicKey");
                    var privateKey = jToken.Value<string>("privateKey");
                    Key = Key.Deserialize(new BigInteger(publicKey), new BigInteger(privateKey));
                }
            }
        }

        public void Save(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentNullException(nameof(password));
            }

            var publicKey = new BigInteger(Key.GetPublicKey().ToArray());
            var privateKey = Key.GetPrivateKey();
            if (File.Exists(GetPath()))
            {
                File.Delete(GetPath());
            }

            File.Create(GetPath()).Close();
            var json = new JObject();
            json.Add("publicKey", publicKey.ToString());
            json.Add("privateKey", privateKey.ToString());
            var bytesBuff = System.Text.Encoding.Unicode.GetBytes(json.ToString());
            using (var aes = Aes.Create())
            {
                Rfc2898DeriveBytes crypto = new Rfc2898DeriveBytes(password, _salt);
                aes.Key = crypto.GetBytes(32);
                aes.IV = crypto.GetBytes(16);
                using (MemoryStream mStream = new MemoryStream())
                {
                    using (CryptoStream cStream = new CryptoStream(mStream, aes.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cStream.Write(bytesBuff, 0, bytesBuff.Length);
                        cStream.Close();
                    }

                    File.WriteAllText(_fileName, System.Convert.ToBase64String(mStream.ToArray()));
                }
            }
        }

        private static string GetPath()
        {
            string path = AppDomain.CurrentDomain.BaseDirectory;
            return Path.Combine(path, _fileName);
        }
    }
}
