using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CollectionHub.Shared.Dtos;
using Microsoft.Azure.Cosmos;

namespace CollectionHub.Functions.Services
{
    public class CosmosDbGameService : IGameService
    {
        private string _endpoint = Environment.GetEnvironmentVariable("CosmosEndpoint")!; 
        private string _key = Environment.GetEnvironmentVariable("CosmosKey")!;

        private List<GameDto> _games = new List<GameDto>();

        private CosmosClient _client { get; set; }
        private Container _container { get; set; }

        public CosmosDbGameService()
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

        public async Task<List<GameDto>> GetAll()
        {
            QueryDefinition query = new QueryDefinition("SELECT * FROM c");

            FeedIterator<GameDto> resultSet = _container.GetItemQueryIterator<GameDto>(query);

            List<GameDto> games = [];

            while(resultSet.HasMoreResults)
            {
                FeedResponse<GameDto> response = await resultSet.ReadNextAsync();
                games.AddRange(response);
            }

            return games;
        }

        public async Task<GameDto> GetById(Guid id)
        {
            GameDto game = await _container.ReadItemAsync<GameDto>(id.ToString(), new PartitionKey(id.ToString()));
            return game;
        }

        public async Task Update(GameDto game)
        {
            await _container.ReplaceItemAsync(game, game.Id.ToString(), new PartitionKey(game.Id.ToString()));
        }

        public async Task<GameDto> Edit(GameDto game)
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

        public async Task Delete(Guid id)
        {
            await _container.DeleteItemAsync<GameDto>(id.ToString(), new PartitionKey(id.ToString()));
        }

        public async Task Add(GameDto game)
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
    }
}
