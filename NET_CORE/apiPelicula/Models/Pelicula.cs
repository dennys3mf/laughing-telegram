using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace BookStoreApi.Models
{
    public class Pelicula
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        public string Titulo { get; set; } = null!;

        public string VideoURL { get; set; } = null!;

        public List<Genero> Generos { get; set; } = new List<Genero>();
    }
}
