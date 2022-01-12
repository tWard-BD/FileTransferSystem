using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using FileTransferSystem.Services.Services.Contracts;

namespace FileTransferSystem.Services.Services
{
    public class FileProcessingService : IFileProcessingService
    {
        public async Task<byte[]> ConvertFileToByteArray(IFormFile file)
        {
            byte[] fileAsBytes;

            using (var memoryStream = new MemoryStream())
            {
                memoryStream.Flush();
                memoryStream.Position = 0;
                await file.CopyToAsync(memoryStream);
                fileAsBytes = memoryStream.ToArray();
            }

            return fileAsBytes;
        }
    }
}
