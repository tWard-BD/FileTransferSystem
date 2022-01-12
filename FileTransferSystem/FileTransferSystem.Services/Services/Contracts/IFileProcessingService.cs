using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace FileTransferSystem.Services.Services.Contracts
{
    public interface IFileProcessingService
    {
        Task<byte[]> ConvertFileToByteArray(IFormFile file);
    }
}
