using OTPManager.Shared.Components;
using OTPManager.Shared.Models;
using Plugin.FileSystem.Abstractions;
using Plugin.SecureStorage.Abstractions;
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
        private const string AppKeychainId = "Token";
        private const string DbFileName = "Data_v2.db3";
        private const string PasswordSalt = "bz77KNXdP,Bc4Acg";

        private ISecureStorage SecureStorage { get; }
        private IFileSystem FileSystem { get; }

        private FileInfo DbFile { get; }
        private SemaphoreSlim ConnectionMutex { get; } = new SemaphoreSlim(1, 1);
        private SQLiteAsyncConnection Connection { get; set; }

        public event ErrorEventHandler ErrorOccurred;

        public StorageService(ISecureStorage secureStorage, IFileSystem fileSystem)
        {
            SecureStorage = secureStorage ?? throw new ArgumentException(nameof(SecureStorage));
            FileSystem = fileSystem ?? throw new ArgumentException(nameof(fileSystem));

            DbFile = new FileInfo(Path.Combine(fileSystem.LocalStorage.FullName, DbFileName));
        }

        public async Task<List<OTPGenerator>> GetAllAsync()
        {
            await ConnectionMutex.WaitAsync();
            var connection = await GetConnectionAsync();
            var output = connection != null ? await connection.Table<OTPGenerator>().ToListAsync() : new List<OTPGenerator>();
            output = output.OrderBy(d => d.Label).ThenBy(d => d.Issuer).ToList();
            ConnectionMutex.Release();
            return output;
        }

        public async Task<int> InsertOrReplaceAsync(OTPGenerator input)
        {
            await ConnectionMutex.WaitAsync();
            var connection = await GetConnectionAsync();
            var result = connection != null ? await connection.InsertOrReplaceAsync(input) : -1;
            ConnectionMutex.Release();
            return result;
        }

        public async Task<int> DeleteAsync(OTPGenerator input)
        {
            await ConnectionMutex.WaitAsync();
            var connection = await GetConnectionAsync();
            var result = connection != null ? await connection.DeleteAsync(input) : -1;
            ConnectionMutex.Release();
            return result;
        }

        public async Task ClearAsync()
        {
            await ConnectionMutex.WaitAsync();
            var connection = await GetConnectionAsync();
            if (connection != null)
            {
                await connection.DropTableAsync<OTPGenerator>();
                await connection.CreateTableAsync<OTPGenerator>();
            }

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
                var dbPassword = GetDBPassword();
                var connString = new SQLiteConnectionString(DbFile.FullName, true, key: GetDBPassword());
                Connection = new SQLiteAsyncConnection(connString);
                try
                {
                    await Connection.CreateTableAsync<OTPGenerator>();
                }
                catch (SQLiteException e) when (e.Result == SQLite3.Result.NonDBFile)
                {
                    //Encryption password does not match anymore. Most likely due to roaming.
                    //Notify user and delete database file. Hope they had backup.
                    await Connection.CloseAsync();
                    Connection = null;
                    DbFile.Delete();
                    ErrorOccurred?.Invoke(this, new ErrorEventArgs(e));
                    return null;
                }

                await MigrateOldDb(Connection);
            }

            return Connection;
        }

        private async Task CloseConnectionAsync()
        {
            await Connection.CloseAsync();
            Connection = null;
        }

        private byte[] GetDBPassword()
        {
            var base64 = SecureStorage.GetValue(AppKeychainId);
            byte[] output;
            if (base64 != null)
            {
                output = Convert.FromBase64String(base64);
            }
            else
            {
                var rand = new Random();
                output = new byte[32];
                rand.NextBytes(output);
                base64 = Convert.ToBase64String(output);
                SecureStorage.SetValue(AppKeychainId, base64);
            }

            return output;
        }

        private async Task MigrateOldDb(SQLiteAsyncConnection connection)
        {
            var oldStore = new LegacyStorageService(SecureStorage, FileSystem);
            if (oldStore.DbFile.Exists)
            {
                var items = await oldStore.GetAllAsync();
                foreach (var i in items)
                {
                    await connection.InsertOrReplaceAsync(i, typeof(OTPGenerator));
                }

                await oldStore.CloseConnectionAsync();
                oldStore.DbFile.Delete();
            }
        }
    }
}
