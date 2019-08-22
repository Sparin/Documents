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
    public class DocumentService : IDocumentService
    {
        private readonly MongoClient _mongoClient;
        private readonly DocumentDbOptions _options;

        public DocumentService(MongoClient mongoClient, IOptionsMonitor<DocumentDbOptions> options)
        {
            _mongoClient = mongoClient;
            _options = options.CurrentValue;
        }

        public async Task<long> CountDocumentsAsync(string collectionName, BsonDocument filter)
        {
            if (collectionName == null)
                throw new ArgumentNullException(nameof(collectionName));
            if (filter == null)
                throw new ArgumentNullException(nameof(filter));

            var database = _mongoClient.GetDatabase(_options.DatabaseName);
            var collection = database.GetCollection<BsonDocument>(collectionName);

            var result = await collection.CountDocumentsAsync(filter);

            return result;
        }

        public async Task<BsonDocument> CreateDocumentAsync(string collectionName, BsonDocument document)
        {
            if (collectionName == null)
                throw new ArgumentNullException(nameof(collectionName));
            if (document == null)
                throw new ArgumentNullException(nameof(document));

            var database = _mongoClient.GetDatabase(_options.DatabaseName);
            var collection = database.GetCollection<BsonDocument>(collectionName);

            await collection.InsertOneAsync(document, options: null);

            return document;
        }

        public async Task DeleteDocumentAsync(string collectionName, ObjectId id)
        {
            if (collectionName == null)
                throw new ArgumentNullException(nameof(collectionName));

            var database = _mongoClient.GetDatabase(_options.DatabaseName);
            var collection = database.GetCollection<BsonDocument>(collectionName);

            await collection.DeleteOneAsync(Builders<BsonDocument>.Filter.Eq("_id", id));
        }

        public async Task<BsonDocument> FindDocumentAsync(string collectionName, BsonDocument filter)
        {
            if (collectionName == null)
                throw new ArgumentNullException(nameof(collectionName));
            if (filter == null)
                throw new ArgumentNullException(nameof(filter));

            var database = _mongoClient.GetDatabase(_options.DatabaseName);
            var collection = database.GetCollection<BsonDocument>(collectionName);

            using (var results = await collection.FindAsync(filter, options: new FindOptions<BsonDocument>() { Limit = 1 }))
                return results.Current.FirstOrDefault();
        }

        public async Task<BsonDocument> FindDocumentAsync(string collectionName, ObjectId id)
        {
            if (collectionName == null)
                throw new ArgumentNullException(nameof(collectionName));

            var database = _mongoClient.GetDatabase(_options.DatabaseName);
            var collection = database.GetCollection<BsonDocument>(collectionName);

            var result = await collection.FindAsync(Builders<BsonDocument>.Filter.Eq("_id", id));

            return await result.FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<BsonDocument>> SearchDocumentsAsync(string collectionName, BsonDocument filter, int page, int limit)
        {
            if (collectionName == null)
                throw new ArgumentNullException(nameof(collectionName));
            if (filter == null)
                throw new ArgumentNullException(nameof(filter));
            if (page <= 0)
                throw new ArgumentOutOfRangeException(nameof(page), page, "Page must be positive number and more than 0");
            if (limit <= 0)
                throw new ArgumentOutOfRangeException(nameof(limit), limit, "Limit must be positive number and more than 0");

            page--;

            var database = _mongoClient.GetDatabase(_options.DatabaseName);
            var collection = database.GetCollection<BsonDocument>(collectionName);

            var results = await collection.FindAsync(filter, options: new FindOptions<BsonDocument>() { Limit = limit, Skip = limit * page, BatchSize = limit });

            return await results.ToListAsync();
        }

        public async Task<BsonDocument> UpdateDocumentAsync(string collectionName, ObjectId id, BsonDocument document)
        {
            if (collectionName == null)
                throw new ArgumentNullException(nameof(collectionName));
            if (document == null)
                throw new ArgumentNullException(nameof(document));

            var database = _mongoClient.GetDatabase(_options.DatabaseName);
            var collection = database.GetCollection<BsonDocument>(collectionName);
            var options = new FindOneAndUpdateOptions<BsonDocument>() { ReturnDocument = ReturnDocument.After };

            var result = await collection.FindOneAndUpdateAsync(Builders<BsonDocument>.Filter.Eq("_id", id), document, options);

            return result;
        }
    }
}
