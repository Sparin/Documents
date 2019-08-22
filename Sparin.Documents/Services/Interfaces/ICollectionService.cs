using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sparin.Documents.Services.Interfaces
{
    public interface ICollectionService
    {
        Task CreateCollectionAsync(string collectionName, BsonDocument validator, DocumentValidationLevel validationLevel);
        Task DeleteCollectionAsync(string collectionName);
        Task<IEnumerable<string>> GetCollectionNamesAsync(BsonDocument filter);
        Task<BsonDocument> FindCollectionAsync(string collectionName);
        Task<BsonDocument> UpdateCollectionAsync(string collectionName, BsonDocument validator, DocumentValidationLevel validationLevel);
    }
}
