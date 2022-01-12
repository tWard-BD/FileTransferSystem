using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace FileTransferSystem.Services.Services.Contracts
{
    public interface IMoveItApiClient
    {
        Task UploadFileToFolder(string folderId, IFormFile file);

        Task<string> GetFolderId();
    }
}
