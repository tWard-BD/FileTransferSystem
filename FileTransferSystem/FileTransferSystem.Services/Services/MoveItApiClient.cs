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
        private readonly IFileProcessingService fileProcessingService;
        private readonly IOptions<UserConfiguration> userOptions;
        private readonly HttpClient httpClient;

        public MoveItApiClient(HttpClient httpClient, IOptions<UserConfiguration> userOptions, IFileProcessingService fileProcessingService)
        {
            this.httpClient = httpClient;
            this.userOptions = userOptions;
            this.fileProcessingService = fileProcessingService;
        }

        public async Task<string> GetFolderId()
        {
            var queryString = new Dictionary<string, string>()
            {
                { "name", "interview.bojidar.dimitrov" }
            };

            var token = await this.GetAuthToken(); 

            var httpClient = this.httpClient;
            
            var requestUri = QueryHelpers.AddQueryString("folders", queryString);
            var req = new HttpRequestMessage(HttpMethod.Get, requestUri);
            req.Headers.Add("Authorization", token);

            var response = await httpClient.SendAsync(req);
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<Root>(content);
            return result.Items[0].Id;
        }

        public async Task UploadFileToFolder(string folderId, IFormFile file)
        {
            var token = await this.GetAuthToken();
            var fileAsBinary = await this.fileProcessingService.ConvertFileToByteArray(file);
            var requestUri = $"/api/v1/folders/{folderId}/files";

            var httpClient = this.httpClient;
            httpClient.DefaultRequestHeaders.Add("Authorization", token);
            var content1 = new MultipartFormDataContent();
            var byteArrayContent = new ByteArrayContent(fileAsBinary);
            content1.Add(byteArrayContent, "file");
            var response = await httpClient.PostAsync(requestUri, content1);
            var contentResponse = await response.Content.ReadAsStringAsync();
        }

        private async Task<string> GetAuthToken()
        {
            var httpClient = this.httpClient;
            var requestUri = "token";
            var req = new HttpRequestMessage(HttpMethod.Post, requestUri);
            req.Content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                { "grant_type", this.userOptions.Value.GrantType },
                { "username", this.userOptions.Value.Username },
                { "password", this.userOptions.Value.Password }
            });

            
            var response = await httpClient.SendAsync(req);
            var content = await response.Content.ReadAsStringAsync();
            // maybe move converting the body into another method
            var result = JsonConvert.DeserializeObject<AuthenticationToken>(content);
            return $"Bearer {result.Access_Token}";
        }
    }
}