using System;
using System.Threading.Tasks;

namespace OTPManager.Shared.Services
{
    public interface IUriService
    {
        Task CreateGeneratorFromUri(string uri);
        Task CreateGeneratorFromUri(Uri uri);
    }
}
