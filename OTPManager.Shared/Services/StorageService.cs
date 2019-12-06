﻿using Newtonsoft.Json;
using OTPManager.Shared.Components;
using OTPManager.Shared.Models;
using Plugin.FileSystem.Abstractions;
using Plugin.SecureStorage.Abstractions;
using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace OTPManager.Shared.Services
{
    public class StorageService : IStorageService
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
                connection.CreateTableAsync<OTPGeneratorInner>().Wait();
                return connection;
            });
        }

        public async Task<List<OTPGenerator>> GetAllAsync()
        {
            var connection = DBConnection.Value;
            var output = await connection.Table<OTPGeneratorInner>().ToListAsync();
            foreach (var i in output)
            {
                var secret = Convert.FromBase64String(i.DbEncryptedSecret);
                var iv = Convert.FromBase64String(i.DbEncryptedSecretIV);
                i.Secret = Decrypt(secret, iv);
            }

            return output.OrderBy(d => d.Label).ThenBy(d => d.Issuer).Cast<OTPGenerator>().ToList();
        }

        public Task<int> InsertOrReplaceAsync(OTPGenerator input)
        {
            var data = new OTPGeneratorInner(input);
            var secret = Encrypt(data.Secret, out var iv);
            data.DbEncryptedSecret = Convert.ToBase64String(secret);
            data.DbEncryptedSecretIV = Convert.ToBase64String(iv);

            var connection = DBConnection.Value;
            return connection.InsertOrReplaceAsync(data);
        }

        public Task<int> DeleteAsync(OTPGenerator input)
        {
            var connection = DBConnection.Value;
            return connection.DeleteAsync(new OTPGeneratorInner(input));
        }

        public async Task ClearAsync()
        {
            var connection = DBConnection.Value;
            await connection.DropTableAsync<OTPGeneratorInner>();
            await connection.CreateTableAsync<OTPGeneratorInner>();
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

        public async Task<MemoryStream> DumpAsync(string password)
        {
            password = password ?? string.Empty;

            var data = await GetAllAsync();
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(data);
            var jsonBytes = Encoding.UTF8.GetBytes(json);
            var payload = Encryptor.SimpleEncryptWithPassword(jsonBytes, password + PasswordSalt);
            return new MemoryStream(payload);
        }

        public async Task<bool> RestoreAsync(Stream data, string password)
        {
            password = password ?? string.Empty;

            if (data.Position != 0)
            {
                if (data.CanSeek)
                {
                    data.Position = 0;
                }
                else
                {
                    return false;
                }
            }

            var payload = new byte[data.Length];
            data.Read(payload, 0, payload.Length);
            byte[] jsonBytes;
            try
            {
                jsonBytes = Encryptor.SimpleDecryptWithPassword(payload, password + PasswordSalt);
            }
            catch
            {
                return false;
            }

            if (jsonBytes == null)
            {
                return false;
            }

            OTPGenerator[] items;
            try
            {
                var json = Encoding.UTF8.GetString(jsonBytes);
                items = Newtonsoft.Json.JsonConvert.DeserializeObject<OTPGenerator[]>(json);
            }
            catch
            {
                return false;
            }

            if (!items.Any())
            {
                return false;
            }

            await ClearAsync();
            foreach (var i in items)
            {
                await InsertOrReplaceAsync(i);
            }

            return true;
        }
    }
}
