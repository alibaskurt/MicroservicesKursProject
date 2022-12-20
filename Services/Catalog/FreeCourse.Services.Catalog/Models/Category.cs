using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace FreeCourse.Services.Catalog.Models
{
    internal class Category
    {
        [BsonId] //Mongodb de bu alanın Id oldugunu belirtiriz.
        [BsonRepresentation(BsonType.ObjectId)] //Mongodb tarafında objectId tipinde gösterilir.
        public string Id { get; set; }
        public string Name { get; set; }
    }
}
