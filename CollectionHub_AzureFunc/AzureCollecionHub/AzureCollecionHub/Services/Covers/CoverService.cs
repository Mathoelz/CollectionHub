using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using CollectionHub.Functions.Services.Igdb;
using CollectionHub.Shared.Dtos.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CollectionHub.Functions.Services.Covers
{
    public class CoverService : ICoverService
    {
        private readonly BlobServiceClient _blobServiceClient;
        private readonly BlobContainerClient _blobContainerClient;
        private readonly HttpClient _httpClient;
        private readonly IGdbService _igdbService;

        public CoverService(BlobServiceClient blobServiceClient, HttpClient httpClient, IGdbService igdbService) 
        {
            _blobServiceClient = blobServiceClient;
            _httpClient = httpClient;
            _igdbService = igdbService;
            _blobContainerClient = _blobServiceClient.GetBlobContainerClient(Environment.GetEnvironmentVariable("CollectionHubCoversBlob"));
        }

        public async Task<string> GetCover(int coverId)
        {
            var blobClient = _blobContainerClient.GetBlobClient($"{coverId}.jpg");

            if (!await blobClient.ExistsAsync())
            {
                var cover = await _igdbService.GetCoverAsync(coverId);
                var stream = await _httpClient.GetStreamAsync("https:" + cover.Url);
                await blobClient.UploadAsync(stream, new BlobUploadOptions { 
                    HttpHeaders = new BlobHttpHeaders
                    {
                        ContentType = "image/jpeg"
                    }
                });
            }

            return blobClient.Uri.ToString();
        }
    }
}
