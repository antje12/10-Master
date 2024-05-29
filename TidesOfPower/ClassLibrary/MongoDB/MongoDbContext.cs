using ClassLibrary.Domain;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;

namespace ClassLibrary.MongoDB;

public class MongoDbContext
{
    private const string _username = "root";
    private const string _password = "password";
    //client:           "mongodb://localhost:27017/"
    //kubernetes:       "mongodb://mongodb-service:27017/"
    //kubernetes(shard):"mongodb://{_username}:{_password}@mongodb-sharded:27017/"
    private string _mongos = "mongodb://mongodb-service:27017/"; // Routers
    private readonly IMongoDatabase _database;

    public MongoDbContext(bool isClient)
    {
        if (isClient)
        {
            _mongos = "mongodb://34.32.33.189:30007/";
        }
        MongoClient client = new(_mongos);
        _database = client.GetDatabase("TidesOfPower");
        BsonSerializer.RegisterSerializer(new GuidSerializer(BsonType.String));
    }
    
    public IMongoCollection<Agent> Agents =>
        _database.GetCollection<Agent>("Agents",
            new MongoCollectionSettings
            {
                ReadPreference = ReadPreference.SecondaryPreferred
            });
}