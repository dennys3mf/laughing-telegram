using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;
namespace BookStoreApi.Models
{
    public class Genero
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string GeneroNombre { get; set; }
        public decimal Peso { get; set; }

    }
}
