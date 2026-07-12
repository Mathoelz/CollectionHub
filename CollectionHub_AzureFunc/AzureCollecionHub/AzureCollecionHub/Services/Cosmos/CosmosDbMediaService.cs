using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CollectionHub.Shared.Dtos.Anime;
using CollectionHub.Shared.Dtos.Game;
using Microsoft.Azure.Cosmos;

namespace CollectionHub.Functions.Services.Cosmos
{
    public class CosmosDbMediaService : IMediaService
    {
        private string _endpoint = Environment.GetEnvironmentVariable("CosmosEndpoint")!; 
        private string _key = Environment.GetEnvironmentVariable("CosmosKey")!;

        private List<GameDto> _games = new List<GameDto>();

        private CosmosClient _client { get; set; }
        private Container _container { get; set; }

        public CosmosDbMediaService()
        {
            _client = new CosmosClient(_endpoint, _key, new CosmosClientOptions
            {
                SerializerOptions = new CosmosSerializationOptions
                {
                    PropertyNamingPolicy = CosmosPropertyNamingPolicy.CamelCase
                }
            });

            _container = _client.GetContainer(Environment.GetEnvironmentVariable("CosmosDatabase"), Environment.GetEnvironmentVariable("CosmosContainer")); 
        }

        #region Games

        public async Task<List<GameDto>> GetAllGames()
        {
            QueryDefinition query = new QueryDefinition("SELECT * FROM c WHERE c.mediaType = 0");

            FeedIterator<GameDto> resultSet = _container.GetItemQueryIterator<GameDto>(query);

            List<GameDto> games = [];

            while(resultSet.HasMoreResults)
            {
                FeedResponse<GameDto> response = await resultSet.ReadNextAsync();
                games.AddRange(response);
            }

            return games;
        }

        public async Task<GameDto> GetGameById(Guid id)
        {
            GameDto game = await _container.ReadItemAsync<GameDto>(id.ToString(), new PartitionKey(id.ToString()));
            return game;
        }

        public async Task UpdateGame(GameDto game)
        {
            await _container.ReplaceItemAsync(game, game.Id.ToString(), new PartitionKey(game.Id.ToString()));
        }

        public async Task<GameDto> EditGame(GameDto game)
        {
            GameDto editGame = new()
            {
                Id = game.Id,
                Title = game.Title,
                Status = game.Status,
                Rating = game.Rating,
                Notes = game.Notes
            };

            return editGame;
        }       

        public async Task AddGame(GameDto game)
        {
            await _container.CreateItemAsync(game, new PartitionKey(game.Id.ToString()));
        }

        private async Task SeedAsync()
        {
            if(_games.Count < 0)
            {
                foreach (var game in _seedGames)
                {
                    await _container.CreateItemAsync(game, new PartitionKey(game.Id.ToString()));
                }
            }          
        }

        private List<GameDto> _seedGames =
        [
        new()
                {
                    Id = Guid.Parse("6cb73441-87a3-4da0-9ef0-ca2c76e5f1e9"),
                    Title = "Persona 5 Royal",
                    Status = CollectionStatus.Completed,
                    Rating = 10,
                    Notes = "One of my all-time favorites."
                },

                new()
                {
                    Id = Guid.Parse("e825fc93-bd89-4ff8-b63f-eeb4f4d3aa8e"),
                    Title = "The Legend of Zelda: Tears of the Kingdom",
                    Status = CollectionStatus.Completed,
                    Rating = 10,
                    Notes = "Fantastic exploration and gameplay."
                },

                new()
                {
                    Id = Guid.Parse("bf8d68d9-d6a5-429d-9c88-aae75a1de3ad"),
                    Title = "NieR: Automata",
                    Status = CollectionStatus.Completed,
                    Rating = 10,
                    Notes = "Amazing story and soundtrack."
                },

                new()
                {
                    Id = Guid.Parse("d12634cf-b0da-4fa8-93ad-f269e66eaa35"),
                    Title = "Clair Obscur: Expedition 33",
                    Status = CollectionStatus.Playing,
                    Rating = 10,
                    Notes = "Absolutely amazing so far."
                },

                new()
                {
                    Id = Guid.Parse("269c08b9-9cd1-420c-8b31-fd48844bd9d8"),
                    Title = "Hollow Knight",
                    Status = CollectionStatus.Backlog,
                    Rating = null,
                    Notes = "Need to finally start it."
                },

                new()
                {
                    Id = Guid.Parse("d0b99c78-661d-4a02-a090-c39cdbb54a32"),
                    Title = "Metaphor: ReFantazio",
                    Status = CollectionStatus.Backlog,
                    Rating = null,
                    Notes = "Looking forward to playing it."
                },

                new()
                {
                    Id = Guid.Parse("657ecbf1-4f46-4454-a5c3-2cb03375af29"),
                    Title = "Elden Ring",
                    Status = CollectionStatus.Backlog,
                    Rating = null,
                    Notes = "Still waiting for the right moment."
                }
    ];

        #endregion

        #region Anime

        public async Task<List<AnimeDto>> GetAllAnimes()
        {
            QueryDefinition query = new QueryDefinition("SELECT * FROM c WHERE c.mediaType = 1");

            FeedIterator<AnimeDto> resultSet = _container.GetItemQueryIterator<AnimeDto>(query);

            List<AnimeDto> animes = [];

            while (resultSet.HasMoreResults)
            {
                FeedResponse<AnimeDto> response = await resultSet.ReadNextAsync();
                animes.AddRange(response);
            }

            return animes;
        }

        public async Task<AnimeDto> GetAnimeById(Guid id)
        {
            AnimeDto anime = await _container.ReadItemAsync<AnimeDto>(id.ToString(), new PartitionKey(id.ToString()));
            return anime;
        }

        public async Task AddAnime(AnimeDto anime)
        {
            await _container.CreateItemAsync(anime, new PartitionKey(anime.Id.ToString()));
        }

        public async Task UpdateAnime(AnimeDto anime)
        {
            await _container.ReplaceItemAsync(anime, anime.Id.ToString(), new PartitionKey(anime.Id.ToString()));
        }

        public async Task<AnimeDto> EditAnime(AnimeDto anime)
        {
            AnimeDto editAnime = new()
            {
                Id = anime.Id,
                Title = anime.Title,
                Status = anime.Status,
                Rating = anime.Rating,
                Notes = anime.Notes
            };
            return editAnime;
        }

        #endregion

        // Delete function for both games and anime, since they share the same container and partition key
        public async Task DeleteItem(Guid id)
        {
            await _container.DeleteItemAsync<GameDto>(id.ToString(), new PartitionKey(id.ToString()));
        }
    }
}
