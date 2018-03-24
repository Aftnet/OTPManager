using System.Collections.Generic;
using System.Threading.Tasks;
using OTPManager.Shared.Models;

namespace OTPManager.Shared.Services
{
    public interface IStorageService
    {
        Task<int> DeleteAsync(OTPGenerator input);
        Task<List<OTPGenerator>> GetAllAsync();
        Task<int> InsertOrReplaceAsync(OTPGenerator input);
        Task ClearAsync();
    }
}