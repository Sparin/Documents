using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using Sparin.Documents.Services.Interfaces;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Sparin.Documents.Controllers
{
    [Route("api/[controller]")]
    public class CollectionController : Controller
    {
        private readonly ICollectionService _collection;
        private readonly ILogger<CollectionController> _logger;

        public CollectionController(
            ICollectionService collection,
            ILogger<CollectionController> logger)
        {
            _collection = collection;
            _logger = logger;
        }

        /// <summary>
        /// Retreive collection's names
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /api/collection
        ///     {
        ///         "name": {
        ///             "$regex": "pen"
        ///         }
        ///     }
        ///
        ///     Response Body
        ///     [
        ///         "pens",
        ///         "pencils"
        ///     ]
        /// </remarks>
        /// <param name="filter">Filter definition</param>
        /// <response code="200">Successful operation</response>
        [ProducesResponseType(200, Type = typeof(IEnumerable<string>))]
        [HttpPost]
        public async Task<IActionResult> GetCollectionNamesAsync([FromBody]JObject filter)
        {
            _logger.LogInformation($"User listing collection names");

            if (!BsonDocument.TryParse(filter.ToString(), out BsonDocument bsonFilter))
                return BadRequest("Invalid format of filter");
            var names = await _collection.GetCollectionNamesAsync(bsonFilter);

            //_logger.LogInformation($"User received {names.Count()} orders");
            return Ok(names);
        }

        /// <summary>
        /// Create new collection
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /api/collection/orders?validationLevel=strict
        ///     {
        ///         $jsonSchema: {
        ///             required: [ "name" ],
        ///             properties: {
        ///                 name: {
        ///                     bsonType: "string",
        ///                     description: "must be a string and is required"
        ///                 }
        ///             }
        ///         }
        ///     }
        ///
        /// </remarks>
        /// <param name="collectionName">Collection name</param>
        /// <param name="validator">Validator of collection</param>
        /// <param name="validationLevel">Validation level</param>
        /// <response code="200">Successful operation</response>
        /// <response code="400">Validation checks is not passed</response>
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [HttpPost("{collectionName}")]
        public async Task<IActionResult> CreateCollectionAsync(string collectionName, [FromBody]JObject validator, DocumentValidationLevel validationLevel = DocumentValidationLevel.Strict)
        {
            _logger.LogInformation($"User trying to create new collection");

            var collection = await _collection.FindCollectionAsync(collectionName);
            if (collection != null)
            {
                _logger.LogWarning($"User tried to create a existing collection");
                return BadRequest("Collection already exists");
            }

            var json = JsonConvert.SerializeObject(validator);
            if (!BsonDocument.TryParse(json, out BsonDocument bsonValidator))
                return BadRequest("Invalid format of validator");

            await _collection.CreateCollectionAsync(collectionName, bsonValidator, validationLevel);
            _logger.LogInformation($"User created new collection with name {collectionName}");

            return NoContent();
        }

        /// <summary>
        /// Update existing collection by name
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     PUT /api/collection/orders?validationLevel=strict
        ///     {
        ///         $jsonSchema: {
        ///             required: [ "name" ],
        ///             properties: {
        ///                 name: {
        ///                     bsonType: "string",
        ///                     description: "must be a string and is required"
        ///                 }
        ///             }
        ///         }
        ///     }
        ///
        /// </remarks>
        /// <param name="collectionName">Collection name</param>
        /// <param name="validator">Validator of collection</param>
        /// <param name="validationLevel">Validation level</param>
        /// <response code="200">Successful operation</response>
        /// <response code="400">Validation checks is not passed</response>
        /// <response code="404">Collection is not found</response>
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(204)]
        [HttpPut("{collectionName}")]
        public async Task<IActionResult> UpdateCollectionAsync(string collectionName, [FromBody]JObject validator, DocumentValidationLevel validationLevel = DocumentValidationLevel.Strict)
        {
            _logger.LogInformation($"User trying to update existing collection with name {collectionName}");
            var collection = await _collection.FindCollectionAsync(collectionName);

            if (collection == null)
            {
                _logger.LogWarning($"User requested not existing collection");
                return NotFound();
            }

            var json = JsonConvert.SerializeObject(validator);
            if (!BsonDocument.TryParse(json, out BsonDocument bsonValidator))
                return BadRequest("Invalid format of validator");

            var result = await _collection.UpdateCollectionAsync(collectionName, bsonValidator, validationLevel);
            _logger.LogInformation($"Collection with name {collectionName} updated");

            return Ok(result);
        }

        /// <summary>
        /// Delete collection by name
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     DELETE /api/collection/orders
        ///
        /// </remarks>
        /// <param name="collectionName">Name of collection</param>
        /// <response code="204">Successful operation</response>
        /// <response code="404">Collection is not found</response>        
        [ProducesResponseType(404)]
        [ProducesResponseType(204)]
        [HttpDelete("{collectionName}")]
        public async Task<IActionResult> DeteleCollectionAsync(string collectionName)
        {
            _logger.LogInformation($"User trying to delete database collection with name {collectionName}");
            var collection = await _collection.FindCollectionAsync(collectionName);

            if (collection == null)
            {
                _logger.LogWarning($"User requested not existing collection");
                return NotFound();
            }

            await _collection.DeleteCollectionAsync(collectionName);
            _logger.LogInformation($"Collection with name {collectionName} was deleted");

            return NoContent();
        }
    }
}
