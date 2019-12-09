using OTPManager.Shared.Components;
using OTPManager.Shared.Models;
using Plugin.FileSystem.Abstractions;
using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OTPManager.Shared.Services
{
    public class StorageService : IStorageService
    {
        private const string DbFileName = "Data_v2.db3";
        private const string PasswordSalt = "bz77KNXdP,Bc4Acg";

        private FileInfo DbFile { get; }
        private SemaphoreSlim ConnectionMutex { get; } = new SemaphoreSlim(1, 1);
        private SQLiteAsyncConnection Connection { get; set; }

        public StorageService(IFileSystem fileSystem)
        {
            DbFile = new FileInfo(Path.Combine(fileSystem.LocalStorage.FullName, DbFileName));
        }

        public async Task<List<OTPGenerator>> GetAllAsync()
        {
            await ConnectionMutex.WaitAsync();
            var connection = await GetConnectionAsync();
            var output = await connection.Table<OTPGenerator>().ToListAsync();
            output = output.OrderBy(d => d.Label).ThenBy(d => d.Issuer).ToList();
            ConnectionMutex.Release();
            return output;
        }

        public async Task<int> InsertOrReplaceAsync(OTPGenerator input)
        {
            await ConnectionMutex.WaitAsync();
            var connection = await GetConnectionAsync();
            var result = await connection.InsertOrReplaceAsync(input);
            ConnectionMutex.Release();
            return result;
        }

        public async Task<int> DeleteAsync(OTPGenerator input)
        {
            await ConnectionMutex.WaitAsync();
            var connection = await GetConnectionAsync();
            var result = await connection.DeleteAsync(input);
            ConnectionMutex.Release();
            return result;
        }

        public async Task ClearAsync()
        {
            await ConnectionMutex.WaitAsync();
            var connection = await GetConnectionAsync();
            await connection.DropTableAsync<OTPGenerator>();
            await connection.CreateTableAsync<OTPGenerator>();
            ConnectionMutex.Release();
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

        private async Task<SQLiteAsyncConnection> GetConnectionAsync()
        {
            if (Connection == null)
            {
                var connString = new SQLiteConnectionString(DbFile.FullName, true, key: "password");
                Connection = new SQLiteAsyncConnection(connString);
                await Connection.CreateTableAsync<OTPGenerator>();
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
