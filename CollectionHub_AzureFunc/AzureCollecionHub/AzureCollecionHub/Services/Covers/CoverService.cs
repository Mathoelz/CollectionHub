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
        private readonly IGameSearchService _gameSearchService;

        public CoverService(BlobServiceClient blobServiceClient, HttpClient httpClient, IGameSearchService gameSearchService, ILogger<CoverService> logger) 
        {
            _blobServiceClient = blobServiceClient;
            _httpClient = httpClient;
            _gameSearchService = gameSearchService;
            _logger = logger;

            _blobContainerClient = _blobServiceClient.GetBlobContainerClient(Environment.GetEnvironmentVariable("CollectionHubCoversBlob"));
        }

        public async Task<string> GetGameCover(int coverId)
        {
            var blobClient =
                _blobContainerClient.GetBlobClient(
                    $"{coverId}.jpg");

            var exists = await blobClient.ExistsAsync();

            if (exists.Value)
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
                await _gameSearchService.GetCoverAsync(coverId);

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

        public async Task<string?> GetAnimeCover(int animeId, string? sourceUrl)
        {
            if (string.IsNullOrWhiteSpace(sourceUrl))
            {
                _logger.LogWarning(
                    "Anime has no cover URL. AnimeId: {AnimeId}",
                    animeId);

                return null;
            }

            if (!Uri.TryCreate(sourceUrl, UriKind.Absolute, out Uri? coverUri) ||
                coverUri.Scheme != Uri.UriSchemeHttps)
            {
                _logger.LogWarning(
                    "Anime cover URL is invalid. AnimeId: {AnimeId}",
                    animeId);

                return null;
            }

            bool isAniListHost =
                coverUri.Host.Equals(
                    "anilist.co",
                    StringComparison.OrdinalIgnoreCase) ||
                coverUri.Host.EndsWith(
                    ".anilist.co",
                    StringComparison.OrdinalIgnoreCase);

            if (!isAniListHost)
            {
                _logger.LogWarning(
                    "Anime cover URL uses an unsupported host. AnimeId: {AnimeId}, Host: {Host}",
                    animeId,
                    coverUri.Host);

                return null;
            }

            var blobClient =
                _blobContainerClient.GetBlobClient(
                    $"anime-{animeId}.jpg");

            var exists = await blobClient.ExistsAsync();

            if (exists.Value)
            {
                _logger.LogInformation(
                    "Anime cover cache lookup completed. AnimeId: {AnimeId}, CacheResult: {CacheResult}",
                    animeId,
                    "Hit");

                return blobClient.Uri.ToString();
            }

            _logger.LogInformation(
                "Anime cover cache lookup completed. AnimeId: {AnimeId}, CacheResult: {CacheResult}",
                animeId,
                "Miss");

            await using var stream =
                await _httpClient.GetStreamAsync(coverUri);

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
                "Anime cover cached. AnimeId: {AnimeId}",
                animeId);

            return blobClient.Uri.ToString();
        }
    }
}
