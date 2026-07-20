using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using CollectionHub.Functions.Services.Igdb;
using CollectionHub.Shared.Dtos.Game;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CollectionHub.Functions.Services.Covers
{
    public class CoverService : ICoverService
    {
        private readonly ILogger<CoverService> _logger;
        private readonly BlobServiceClient _blobServiceClient;
        private readonly BlobContainerClient _blobContainerClient;
        private readonly HttpClient _httpClient;
        private readonly IGdbService _igdbService;

        public CoverService(BlobServiceClient blobServiceClient, HttpClient httpClient, IGdbService igdbService, ILogger<CoverService> logger) 
        {
            _blobServiceClient = blobServiceClient;
            _httpClient = httpClient;
            _igdbService = igdbService;
            _logger = logger;

            _blobContainerClient = _blobServiceClient.GetBlobContainerClient(Environment.GetEnvironmentVariable("CollectionHubCoversBlob"));
        }

        public async Task<string> GetCover(int coverId)
        {
            var blobClient =
                _blobContainerClient.GetBlobClient(
                    $"{coverId}.jpg");

            var exists = await blobClient.ExistsAsync();

            if (exists)
            {
                _logger.LogInformation(
                    "Cover cache lookup completed. CoverId: {CoverId}, CacheResult: {CacheResult}",
                    coverId,
                    "Hit");

                return blobClient.Uri.ToString();
            }

            _logger.LogInformation(
                "Cover cache lookup completed. CoverId: {CoverId}, CacheResult: {CacheResult}",
                coverId,
                "Miss");

            var cover =
                await _igdbService.GetCoverAsync(coverId);

            await using var stream =
                await _httpClient.GetStreamAsync(
                    "https:" + cover.Url);

            await blobClient.UploadAsync(
                stream,
                new BlobUploadOptions
                {
                    HttpHeaders = new BlobHttpHeaders
                    {
                        ContentType = "image/jpeg"
                    }
                });

            _logger.LogInformation(
                "Cover cached. CoverId: {CoverId}",
                coverId);

            return blobClient.Uri.ToString();
        }
    }
}
