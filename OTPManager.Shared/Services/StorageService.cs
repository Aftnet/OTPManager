using OTPManager.Shared.Models;
using Plugin.FileSystem.Abstractions;
using Plugin.SecureStorage.Abstractions;
using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace OTPManager.Shared.Services
{
    public class StorageService : IStorageService
    {
        internal const string AppKeychainId = "Token";
        internal const string DbFileName = "Data.db3";

        private readonly ISecureStorage SecureStorage;
        private readonly IFileSystem FileSystem;

        private readonly Lazy<byte[]> EncryptionKey;
        private readonly Lazy<SQLiteAsyncConnection> DBConnection;

        public StorageService(ISecureStorage secureStorage, IFileSystem fileSystem)
        {
            SecureStorage = secureStorage;
            FileSystem = fileSystem;

            EncryptionKey = new Lazy<byte[]>(GetEncryptionKey);
            DBConnection = new Lazy<SQLiteAsyncConnection>(() =>
            {
                var dbPath = Path.Combine(FileSystem.LocalStorage.FullName, DbFileName);
                var connection = new SQLiteAsyncConnection(dbPath);
                connection.CreateTableAsync<OTPGenerator>().Wait();
                return connection;
            });
        }

        public async Task<List<OTPGenerator>> GetAllAsync()
        {
            var connection = DBConnection.Value;
            var output = await connection.Table<OTPGenerator>().ToListAsync();
            foreach (var i in output)
            {
                var secret = Convert.FromBase64String(i.DbEncryptedSecret);
                var iv = Convert.FromBase64String(i.DbEncryptedSecretIV);
                i.Secret = Decrypt(secret, iv);
            }

            return output.OrderBy(d => d.Label).ThenBy(d => d.Issuer).ToList();
        }

        public Task<int> InsertOrReplaceAsync(OTPGenerator input)
        {
            var secret = Encrypt(input.Secret, out var iv);
            input.DbEncryptedSecret = Convert.ToBase64String(secret);
            input.DbEncryptedSecretIV = Convert.ToBase64String(iv);

            var connection = DBConnection.Value;
            return connection.InsertOrReplaceAsync(input);
        }

        public Task<int> DeleteAsync(OTPGenerator input)
        {
            var connection = DBConnection.Value;
            return connection.DeleteAsync(input);
        }

        public async Task ClearAsync()
        {
            var connection = DBConnection.Value;
            await connection.DropTableAsync<OTPGenerator>();
            await connection.CreateTableAsync<OTPGenerator>();
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
    }
}
