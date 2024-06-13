using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

namespace BookStoreApi.Models
{
    public class MovieRating
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        public string UsuarioId { get; set; } = null!;
        public string PeliculaId { get; set; } = null!;
        public double ViewingTime { get; set; }
    }
}