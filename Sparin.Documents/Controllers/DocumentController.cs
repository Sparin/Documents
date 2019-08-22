using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sparin.Documents.Model.DTO;
using Sparin.Documents.Services.Interfaces;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Sparin.Documents.Controllers
{
    [Route("api/[controller]")]
    public class DocumentController : Controller
    {
        private readonly ICollectionService _collection;
        private readonly IDocumentService _document;
        private readonly ILogger<DocumentController> _logger;

        public DocumentController(
            ICollectionService collection,
            IDocumentService document,
            ILogger<DocumentController> logger)
        {
            _collection = collection;
            _document = document;
            _logger = logger;
        }

        /// <summary>
        /// Search documents from collection
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /api/document/orders/search
        ///     {
        ///         "name": {
        ///             "$regex": "pen"
        ///         }
        ///     }
        ///     
        /// </remarks>
        /// <param name="collectionName">Name of collection</param>
        /// <param name="filter">Filter definition</param>
        /// <param name="page">Offset. Depends on limit</param>
        /// <param name="limit">Count of documents per request (max 50)</param>
        /// <response code="200">Successful operation</response>
        /// <response code="400">Invalid format of filter</response>
        /// <response code="404">Collection is not found</response>
        [ProducesResponseType(200, Type = typeof(IEnumerable<object>))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [HttpPost("{collectionName}/search")]
        public async Task<IActionResult> GetDocumentsAsync(string collectionName, [FromBody]JObject filter, int page = Helpers.DEFAULT_PAGE, int limit = Helpers.MAX_LIMIT_ON_PAGE)
        {
            Helpers.CorrectPageLimitValues(ref page, ref limit);
            _logger.LogInformation($"User requesting documnet from collection names");
            var collection = await _collection.FindCollectionAsync(collectionName);

            if (collection == null)
            {
                _logger.LogWarning($"User requested not existing collection");
                return NotFound("Not existing collection");
            }

            if (!BsonDocument.TryParse(filter.ToString(), out BsonDocument bsonFilter))
            {
                _logger.LogWarning($"User send wrong format of filter");
                return BadRequest("Invalid format of filter");
            }

            var entities = await _document.SearchDocumentsAsync(collectionName, bsonFilter, page, limit);
            var count = await _document.CountDocumentsAsync(collectionName, bsonFilter);
            var response = new SearchResponse<object>(count, page, limit, entities.Select(x => BsonTypeMapper.MapToDotNetValue(x)));

            _logger.LogInformation($"User received {entities.Count()} documents from collection {collectionName}");
            return Ok(response);
        }

        /// <summary>
        /// Get document by id
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /api/document/orders/5d5d36003c9164f8adb6f62e
        ///     
        /// </remarks>
        /// <param name="collectionName">Name of collection</param>
        /// <param name="id">Identificator of document</param>
        /// <response code="200">Successful operation</response>
        /// <response code="400">Invalid format of identificator</response>
        /// <response code="404">Document or collection is not found</response>
        [ProducesResponseType(200, Type = typeof(object))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [HttpGet("{collectionName}/{id}")]
        public async Task<IActionResult> GetDocumentAsync(string collectionName, string id)
        {
            _logger.LogInformation($"User requesting documnet from collection names");
            var collection = await _collection.FindCollectionAsync(collectionName);

            if (collection == null)
            {
                _logger.LogWarning($"User requested not existing collection");
                return NotFound("Not existing collection");
            }

            if (!ObjectId.TryParse(id, out ObjectId objectId))
            {
                _logger.LogWarning($"User send wrong format of identificator - {id}");
                return BadRequest("Wrong format of identificator");
            }

            var entity = await _document.FindDocumentAsync(collectionName, objectId);
            if (entity == default)
            {
                _logger.LogWarning($"User requested not existing document");
                return NotFound();
            }

            _logger.LogInformation($"User received document with identificator {id} from collection {collectionName}");
            return Ok(BsonTypeMapper.MapToDotNetValue(entity));
        }

        /// <summary>
        /// Create new document 
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /api/document/orders
        ///     {
        ///         "Name": "Michael"
        ///         "ETA": 12312542344543
        ///     }
        ///     
        /// </remarks>
        /// <param name="collectionName">Name of collection</param>
        /// <param name="document">Descpition of future document</param>
        /// <response code="200">Successful operation</response>
        /// <response code="400">Invalid format of document or validation checks has failed</response>
        /// <response code="404">Collection is not found</response>
        [HttpPost("{collectionName}")]
        public async Task<IActionResult> CreateDocumentAsync(string collectionName, [Required][FromBody]JObject document)
        {
            _logger.LogInformation($"User requesting documnet from collection names");
            var collection = await _collection.FindCollectionAsync(collectionName);

            if (collection == null)
            {
                _logger.LogWarning($"User requested not existing collection");
                return NotFound("Not existing collection");
            }
            var json = JsonConvert.SerializeObject(document);
            if (!BsonDocument.TryParse(json, out BsonDocument bsonDocument))
            {
                _logger.LogWarning($"User send invalid document");
                return BadRequest("Invalid format of bson document");
            }

            BsonDocument entity;
            try
            {
                entity = await _document.CreateDocumentAsync(collectionName, bsonDocument);
            }
            catch (MongoDB.Driver.MongoWriteException e)
            {
                if (e.Message == "A write operation resulted in an error.\r\n  Document failed validation")
                {
                    _logger.LogWarning($"User's document failed validation");
                    return BadRequest("Document failed validation");
                }
                throw;
            }

            _logger.LogInformation($"User received new document with identificator {entity["_id"]} from collection {collectionName}");
            return Ok(BsonTypeMapper.MapToDotNetValue(entity));
        }

        /// <summary>
        /// Update existing document by id
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     PUT /api/document/orders/5d5d34e73c9164f8adb6f62b
        ///     {
        ///         "Name": "Michael"
        ///         "ETA": 12312542344543
        ///     }
        ///     
        /// </remarks>
        /// <param name="collectionName">Name of collection</param>
        /// <param name="id">Identificator of document</param>
        /// <param name="document">Descpition of future document</param>
        /// <response code="200">Successful operation</response>
        /// <response code="400">Invalid format of document/identificator or validation checks has failed</response>
        /// <response code="404">Collection or document is not found</response>
        [HttpPut("{collectionName}/{id}")]
        public async Task<IActionResult> UpdateDocumentAsync(string collectionName, string id, [Required][FromBody]JObject document)
        {
            _logger.LogInformation($"User trying to update document with identificator {id} from collection {collectionName}");
            var collection = await _collection.FindCollectionAsync(collectionName);

            if (collection == null)
            {
                _logger.LogWarning($"User requested not existing collection");
                return NotFound("Not existing collection");
            }        

            if (!ObjectId.TryParse(id, out ObjectId objectId))
            {
                _logger.LogWarning($"User send wrong format of identificator - {id}");
                return BadRequest("Wrong format of identificator");
            }

            if (!BsonDocument.TryParse(document.ToString(), out BsonDocument bsonDocument))
            {
                _logger.LogWarning($"User send wrong format of document");
                return BadRequest("Invalid format of document");
            }

            var entity = await _document.FindDocumentAsync(collectionName, objectId);
            if(entity == null)
            {
                _logger.LogWarning($"User requested not existing document");
                return NotFound();
            }

            try
            {
                bsonDocument = await _document.UpdateDocumentAsync(collectionName, objectId, bsonDocument);
            }
            catch (MongoDB.Driver.MongoWriteException e)
            {
                if (e.Message == "A write operation resulted in an error.\r\n  Document failed validation")
                {
                    _logger.LogWarning($"User's document failed validation");
                    return BadRequest("Document failed validation");
                }
                throw;
            }

            _logger.LogInformation($"User received updated document with identificator {id} from collection {collectionName}");
            return Ok(BsonTypeMapper.MapToDotNetValue(bsonDocument));
        }

        /// <summary>
        /// Delete document by id
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     DELETE /api/document/orders/5d5d34e73c9164f8adb6f62b
        ///     
        /// </remarks>
        /// <param name="collectionName">Name of collection</param>
        /// <param name="id">Identificator of document</param>
        /// <response code="204">Successful operation</response>
        /// <response code="400">Invalid format of identificator</response>
        /// <response code="404">Collection is not found</response>
        [HttpDelete("{collectionName}/{id}")]
        public async Task<IActionResult> DeleteDocumentAsync(string collectionName, string id)
        {
            _logger.LogInformation($"User trying to delete document with identificator {id} from collection {collectionName}");
            var collection = await _collection.FindCollectionAsync(collectionName);

            if (collection == null)
            {
                _logger.LogWarning($"User requested not existing collection");
                return NotFound("Not existing collection");
            }

            if (!ObjectId.TryParse(id, out ObjectId objectId))
            {
                _logger.LogWarning($"User send wrong format of identificator - {id}");
                return BadRequest("Wrong format of identificator");
            }

            var entity = await _document.FindDocumentAsync(collectionName, objectId);
            if (entity == null)
            {
                _logger.LogWarning($"User requested not existing document");
                return NotFound();
            }

            await _document.DeleteDocumentAsync(collectionName, objectId);

            _logger.LogInformation($"Document with identificator {id} from collection {collectionName} was deleted");
            return NoContent();
        }
    }
}
