using ClassLibrary.Classes.Domain;
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
    //docker compose:   "mongodb://mongodb-1:27017/"
    //kubernetes:       "mongodb://mongodb-service:27017/"
    //kubernetes(shard):"mongodb://{_username}:{_password}@mongodb-sharded:27017/"
    private const string _mongos = "mongodb://mongodb-service:27017/"; // Routers
    private readonly IMongoDatabase _database;

    public MongoDbContext()
    {
        MongoClient client = new(_mongos);
        _database = client.GetDatabase("TidesOfPower");
        BsonSerializer.RegisterSerializer(new GuidSerializer(BsonType.String));
    }

    public IMongoCollection<Profile> Profiles =>
        _database.GetCollection<Profile>("Profiles",
            new MongoCollectionSettings
            {
                ReadPreference = ReadPreference.SecondaryPreferred
            }); // Always read from a secondary, read from the primary if no secondary is available (https://severalnines.com/blog/become-mongodb-dba-how-scale-reads)

    public IMongoCollection<Avatar> Avatars =>
        _database.GetCollection<Avatar>("Avatars",
            new MongoCollectionSettings
            {
                ReadPreference = ReadPreference.SecondaryPreferred
            });

    public IMongoCollection<Entity> Entities =>
        _database.GetCollection<Entity>("Entities",
            new MongoCollectionSettings
            {
                ReadPreference = ReadPreference.SecondaryPreferred
            });
}