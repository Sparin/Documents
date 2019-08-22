using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sparin.Documents.Services.Interfaces
{
    public interface IDocumentService
    {
        Task<BsonDocument> CreateDocumentAsync(string collectionName, BsonDocument document);
        Task<IEnumerable<BsonDocument>> SearchDocumentsAsync(string collectionName, BsonDocument filter, int page, int limit);
        Task<BsonDocument> FindDocumentAsync(string collectionName, ObjectId id);
        Task<long> CountDocumentsAsync(string collectionName, BsonDocument filter);
        Task<BsonDocument> UpdateDocumentAsync(string collectionName, ObjectId id, BsonDocument document);
        Task DeleteDocumentAsync(string collectionName, ObjectId id);
    }
}
