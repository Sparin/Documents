using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using Sparin.Documents.Configuration;
using Sparin.Documents.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sparin.Documents.Services
{
    public class CollectionService : ICollectionService
    {
        private readonly MongoClient _mongoClient;
        private readonly DocumentDbOptions _options;

        public CollectionService(MongoClient mongoClient, IOptionsMonitor<DocumentDbOptions> options)
        {
            _mongoClient = mongoClient;
            _options = options.CurrentValue;
        }

        public async Task CreateCollectionAsync(string collectionName, BsonDocument validator, DocumentValidationLevel validationLevel)
        {
            var database = _mongoClient.GetDatabase(_options.DatabaseName);
            var options = new CreateCollectionOptions<BsonDocument>()
            {
                Validator = validator,
                ValidationLevel = validationLevel
            };

            await database.CreateCollectionAsync(collectionName, options);
        }

        public async Task DeleteCollectionAsync(string collectionName)
        {
            var database = _mongoClient.GetDatabase(_options.DatabaseName);
            await database.DropCollectionAsync(collectionName);
        }

        public async Task<IEnumerable<string>> GetCollectionNamesAsync(BsonDocument filter)
        {
            var database = _mongoClient.GetDatabase(_options.DatabaseName);
            using (var cursor = await database.ListCollectionsAsync(new ListCollectionsOptions() { Filter = filter }))
            {
                var result = await cursor.ToListAsync();
                return result.Select(x => x["name"].AsString);
            }
        }

        public async Task<BsonDocument> FindCollectionAsync(string collectionName)
        {
            var database = _mongoClient.GetDatabase(_options.DatabaseName);
            var filter = Builders<BsonDocument>.Filter.Eq("name", collectionName);
            using (var cursor = await database.ListCollectionsAsync(new ListCollectionsOptions() { Filter = filter }))
            {
                var result = await cursor.FirstOrDefaultAsync();
                return result;
            }
            //return database.GetCollection<BsonDocument>(collectionName);
        }


        public async Task<BsonDocument> UpdateCollectionAsync(string collectionName, BsonDocument validator, DocumentValidationLevel validationLevel)
        {
            var database = _mongoClient.GetDatabase(_options.DatabaseName);

            var updateSchemaCommand = new BsonDocument();
            updateSchemaCommand.Add("collMod", collectionName);
            updateSchemaCommand.Add("validator", validator);
            updateSchemaCommand.Add("validationLevel", validationLevel.ToString().ToLower());

            var result = await database.RunCommandAsync<BsonDocument>(updateSchemaCommand);
            return result;
        }
    }
}
