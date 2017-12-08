using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SimpleBlockChain.Core.Repositories;
using SimpleBlockChain.Core.Aggregates;
using System.Security;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using SimpleBlockChain.Data.Sqlite.Models;
using System.Text;
using System.Security.Cryptography;
using System.IO;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using SimpleBlockChain.Core.Extensions;
using SimpleBlockChain.Core.Exceptions;
using SimpleBlockChain.Core;

namespace SimpleBlockChain.Data.Sqlite.Repositories
{
    public class WalletRepository : IWalletRepository
    {
        private readonly CurrentDbContext _currentDbContext;
        private static byte[] _salt = new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 };

        public WalletRepository(CurrentDbContext currentDbContext)
        {
            _currentDbContext = currentDbContext;
        }

        public async Task<bool> Add(WalletAggregate wallet, SecureString password)
        {
            if (wallet == null)
            {
                throw new ArgumentNullException(nameof(wallet));
            }

            if (password == null)
            {
                throw new ArgumentNullException(nameof(password));
            }

            var exists = await _currentDbContext.Wallets.AnyAsync(w => w.Name == wallet.Name).ConfigureAwait(false);
            if (exists)
            {
                throw new DataWalletException(ErrorCodes.AlreadyExists);
            }

            var walletJson = ToJson(wallet);
            var record = new Wallet(wallet.Name, Protect(walletJson, password));
            _currentDbContext.Wallets.Add(record);
            await _currentDbContext.SaveChangesAsync().ConfigureAwait(false);
            return true;
        }

        public async Task<WalletAggregate> Get(string name, SecureString password)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            var record = await _currentDbContext.Wallets.FirstOrDefaultAsync(w => w.Name == name);
            if (record == null)
            {
                return null;
            }

            var unprotectedJson = Unprotect(record.SerializedContent, password);
            return GetJson(unprotectedJson);
        }

        public async Task<IEnumerable<string>> GetAll()
        {
            return await _currentDbContext.Wallets.Select(w => w.Name).ToListAsync().ConfigureAwait(false);
        }

        private static WalletAggregate GetJson(JObject jObj)
        {
            if (jObj == null)
            {
                throw new ArgumentNullException(nameof(jObj));
            }

            var name = jObj.Value<string>("name");
            var network = jObj.Value<int>("network");
            JToken arrToken = null;
            var addresses = new List<WalletAggregateAddress>();
            if (jObj.TryGetValue("addrs", out arrToken))
            {
                var arr = arrToken as JArray;
                if (arr != null)
                {
                    foreach(var adr in arr)
                    {
                        addresses.Add(WalletAggregateAddress.FromJson(JObject.Parse(adr.ToString())));
                    }
                }
            }

            return new WalletAggregate
            {
                Name = name,
                Network = (Networks)network,
                Addresses = addresses
            };
        }

        private static JObject ToJson(WalletAggregate wallet)
        {
            var result = new JObject();
            result.Add("name", wallet.Name);
            result.Add("network", (int)wallet.Network);
            if (wallet.Addresses != null)
            {
                var arr = new JArray();
                foreach(var addr in wallet.Addresses)
                {
                    arr.Add(addr.GetJson());
                }

                result.Add("addrs", arr);
            }

            return result;
        }

        private static string Protect(JObject json, SecureString password)
        {
            var bytesBuff = Encoding.Unicode.GetBytes(json.ToString());
            using (var rijndael = Rijndael.Create())
            {
                var p = password.SecureStringToString();
                Rfc2898DeriveBytes crypto = new Rfc2898DeriveBytes(p, _salt);
                rijndael.IV = crypto.GetBytes(rijndael.BlockSize / 8);
                rijndael.Key = crypto.GetBytes(rijndael.KeySize / 8);
                using (MemoryStream mStream = new MemoryStream())
                {
                    using (CryptoStream cStream = new CryptoStream(mStream, rijndael.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cStream.Write(bytesBuff, 0, bytesBuff.Length);
                        cStream.Dispose();
                    }

                    return Convert.ToBase64String(mStream.ToArray());
                }
            }
        }

        private static JObject Unprotect(string protectedStr, SecureString password)
        {
            byte[] bytesBuff = Convert.FromBase64String(protectedStr);
            Rijndael rijndael = Rijndael.Create();
            MemoryStream mStream = new MemoryStream();
            CryptoStream cStream = null;
            try
            {
                var p = password.SecureStringToString();
                Rfc2898DeriveBytes crypto = new Rfc2898DeriveBytes(p, _salt);
                rijndael.IV = crypto.GetBytes(rijndael.BlockSize / 8);
                rijndael.Key = crypto.GetBytes(rijndael.KeySize / 8);
                cStream = new CryptoStream(mStream, rijndael.CreateDecryptor(), CryptoStreamMode.Write);
                cStream.Write(bytesBuff, 0, bytesBuff.Length);
                cStream.Dispose();
                var json = System.Text.Encoding.Unicode.GetString(mStream.ToArray());
                return JObject.Parse(json);
            }
            catch (CryptographicException)
            {
                throw new NotAuthorizedWalletException(ErrorCodes.BadPassword);
            }
            catch (JsonReaderException)
            {
                throw new NotAuthorizedWalletException(ErrorCodes.BadPassword);
            }
            finally
            {
                rijndael.Dispose();
                mStream.Dispose();
            }
        }
    }
}
