using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atlantic.Data.MongoFs
{
    public class FilestoreDatabaseSettings : IFilestoreDatabaseSettings
    {
        public string PhotosCollectionName { get; set; }
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
    }

    public interface IFilestoreDatabaseSettings
    {
        string PhotosCollectionName { get; set; }
        string ConnectionString { get; set; }
        string DatabaseName { get; set; }
    }
    public class MongoFileObject
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string FileName { get; set; }

        public ObjectId PhotoObjectId { get; set; }

        public DateTime? DateCreated { get; set; }
    }
}
