using MongoDB.Bson.Serialization.Attributes;

namespace ClassLibrary.Classes.Domain;

public class Profile
{
    [BsonId] public Guid Id { get; set; }

    [BsonElement("email")] public string Email { get; set; }

    [BsonElement("password")] public string Password { get; set; }
}