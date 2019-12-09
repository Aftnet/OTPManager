using Newtonsoft.Json;
using OTPManager.Shared.Models;
using Plugin.FileSystem.Abstractions;
using Plugin.SecureStorage.Abstractions;
using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;

namespace OTPManager.Shared.Services
{
    public class LegacyStorageService : IStorageService
    {
        private class OTPGeneratorInner : OTPGenerator
        {
            public OTPGeneratorInner()
            {

            }

            public OTPGeneratorInner(OTPGenerator input)
            {
                Uid = input.Uid;
                Label = input.Label;
                Issuer = input.Issuer;
                AllowExporting = input.AllowExporting;
                AlgorithmName = input.AlgorithmName;
                Secret = input.Secret;
                NumDigits = input.NumDigits;
            }

            [Ignore]
            public override byte[] Secret { get; set; }

            [JsonIgnore]
            public string DbEncryptedSecret { get; set; }

            [JsonIgnore]
            public string DbEncryptedSecretIV { get; set; }
        }

        internal const string AppKeychainId = "Token";
        internal const string DbFileName = "Data.db3";
        internal const string PasswordSalt = "bz77KNXdP,Bc4Acg";

        private readonly ISecureStorage SecureStorage;
        private readonly IFileSystem FileSystem;

        private readonly Lazy<byte[]> EncryptionKey;

        private FileInfo DbFile { get; }
        private SemaphoreSlim ConnectionMutex { get; } = new SemaphoreSlim(1, 1);
        private SQLiteAsyncConnection Connection { get; set; }

        public LegacyStorageService(ISecureStorage secureStorage, IFileSystem fileSystem)
        {
            SecureStorage = secureStorage;

            DbFile = new FileInfo(Path.Combine(fileSystem.LocalStorage.FullName, DbFileName));
            EncryptionKey = new Lazy<byte[]>(GetEncryptionKey);
        }

        public async Task<List<OTPGenerator>> GetAllAsync()
        {
            await ConnectionMutex.WaitAsync();
            var connection = await GetConnectionAsync();
            var items = await connection.Table<OTPGeneratorInner>().ToListAsync();
            foreach (var i in items)
            {
                var secret = Convert.FromBase64String(i.DbEncryptedSecret);
                var iv = Convert.FromBase64String(i.DbEncryptedSecretIV);
                i.Secret = Decrypt(secret, iv);
            }

            var output = items.OrderBy(d => d.Label).ThenBy(d => d.Issuer).Cast<OTPGenerator>().ToList();
            ConnectionMutex.Release();
            return output;
        }

        public async Task<int> InsertOrReplaceAsync(OTPGenerator input)
        {
            var data = new OTPGeneratorInner(input);
            var secret = Encrypt(data.Secret, out var iv);
            data.DbEncryptedSecret = Convert.ToBase64String(secret);
            data.DbEncryptedSecretIV = Convert.ToBase64String(iv);

            await ConnectionMutex.WaitAsync();
            var connection = await GetConnectionAsync();
            var result = await connection.InsertOrReplaceAsync(data);
            ConnectionMutex.Release();
            return result;
        }

        public async Task<int> DeleteAsync(OTPGenerator input)
        {
            await ConnectionMutex.WaitAsync();
            var connection = await GetConnectionAsync();
            var result = await connection.DeleteAsync(new OTPGeneratorInner(input));
            ConnectionMutex.Release();
            return result;
        }

        public async Task ClearAsync()
        {
            await ConnectionMutex.WaitAsync();
            var connection = await GetConnectionAsync();
            await connection.DropTableAsync<OTPGeneratorInner>();
            await connection.CreateTableAsync<OTPGeneratorInner>();
            ConnectionMutex.Release();
        }

        private byte[] Encrypt(byte[] clearText, out byte[] IV)
        {
            using (var algorithm = Aes.Create())
            {
                algorithm.Padding = PaddingMode.Zeros;
                algorithm.Key = EncryptionKey.Value;
                algorithm.GenerateIV();

                using (var encryptor = algorithm.CreateEncryptor())
                using (var memStream = new MemoryStream())
                using (var cryptoStream = new CryptoStream(memStream, encryptor, CryptoStreamMode.Write))
                {
                    cryptoStream.Write(clearText, 0, clearText.Length);
                    cryptoStream.FlushFinalBlock();

                    var output = memStream.ToArray();
                    IV = algorithm.IV;
                    return output;
                }
            }
        }

        private byte[] Decrypt(byte[] cypherText, byte[] IV)
        {
            using (var algorithm = Aes.Create())
            {
                algorithm.Padding = PaddingMode.Zeros;
                algorithm.Key = EncryptionKey.Value;
                algorithm.IV = IV;

                using (var decryptor = algorithm.CreateDecryptor())
                using (var memStream = new MemoryStream(cypherText))
                using (var cryptoStream = new CryptoStream(memStream, decryptor, CryptoStreamMode.Read))
                using (var outStream = new MemoryStream())
                {
                    cryptoStream.CopyTo(outStream);
                    var output = outStream.ToArray();
                    return output;
                }
            }
        }

        private byte[] GetEncryptionKey()
        {
            var output = default(byte[]);
            var keyString = SecureStorage.GetValue(AppKeychainId);

            if (keyString == null)
            {
                using (var algorithm = Aes.Create())
                {
                    algorithm.GenerateKey();
                    output = algorithm.Key;
                }

                keyString = Convert.ToBase64String(output);
                SecureStorage.SetValue(AppKeychainId, keyString);
            }
            else
            {
                output = Convert.FromBase64String(keyString);
            }

            return output;
        }

        public Task<MemoryStream> DumpAsync(string password)
        {
            throw new NotImplementedException();
        }

        public Task<bool> RestoreAsync(Stream data, string password)
        {
            throw new NotImplementedException();
        }

        private async Task<SQLiteAsyncConnection> GetConnectionAsync()
        {
            if (Connection == null)
            {
                Connection = new SQLiteAsyncConnection(DbFile.FullName);
                await Connection.CreateTableAsync<OTPGeneratorInner>();
            }

            return Connection;
        }

        private async Task CloseConnectionAsync()
        {
            await Connection.CloseAsync();
            Connection = null;
        }
    }
}
