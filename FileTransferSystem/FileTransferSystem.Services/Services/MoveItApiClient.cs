using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;
using FileTransferSystem.Common.Configuration;
using FileTransferSystem.Services.Models.MoveItApi;
using FileTransferSystem.Services.Services.Contracts;

namespace FileTransferSystem.Services.Services
{
    public class MoveItApiClient : IMoveItApiClient
    {
        private readonly IFileProcessingService _fileProcessingService;
        private readonly IOptions<UserConfiguration> _userOptions;
        private readonly HttpClient _httpClient;

        public MoveItApiClient(HttpClient httpClient, IOptions<UserConfiguration> userOptions, IFileProcessingService fileProcessingService)
        {
            _httpClient = httpClient;
            _userOptions = userOptions;
            _fileProcessingService = fileProcessingService;
        }

        public async Task<string> GetFolderId()
        {
            var token = await GetAuthToken(); 
            var queryString = new Dictionary<string, string>()
            {
                { "name", _userOptions.Value.UploadToFolder }
            };

            var httpClient = _httpClient;
            var requestUri = QueryHelpers.AddQueryString("folders", queryString);
            var req = new HttpRequestMessage(HttpMethod.Get, requestUri);
            req.Headers.Add("Authorization", token);

            var response = await httpClient.SendAsync(req);
            var responseContent = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<FolderList>(responseContent);

            return result.Items[0].Id;
        }

        public async Task UploadFileToFolder(string folderId, IFormFile file)
        {
            var token = await GetAuthToken();
            var fileAsBinary = await _fileProcessingService.ConvertFileToByteArray(file);
            var requestUri = $"/api/v1/folders/{folderId}/files";

            var httpClient = _httpClient;
            httpClient.DefaultRequestHeaders.Add("Authorization", token);

            var requestContent = new MultipartFormDataContent();
            var fileContent = new ByteArrayContent(fileAsBinary);
            requestContent.Add(fileContent, "file", file.FileName);

            var response = await httpClient.PostAsync(requestUri, requestContent);
        }

        private async Task<string> GetAuthToken()
        {
            var httpClient = _httpClient;
            var requestUri = "token";
            var req = new HttpRequestMessage(HttpMethod.Post, requestUri);
            req.Content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                { "grant_type", _userOptions.Value.GrantType },
                { "username", _userOptions.Value.Username },
                { "password", _userOptions.Value.Password }
            });

            var response = await httpClient.SendAsync(req);
            var content = await response.Content.ReadAsStringAsync();

            var result = JsonConvert.DeserializeObject<AuthenticationToken>(content);
            return $"Bearer {result.Access_Token}";
        }
    }
}