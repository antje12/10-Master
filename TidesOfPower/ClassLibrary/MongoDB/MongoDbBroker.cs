using ClassLibrary.Classes.Domain;
using MongoDB.Bson;
using MongoDB.Driver;

namespace ClassLibrary.MongoDB;

public class MongoDbBroker
{
    private readonly MongoDbContext _mongoDbContext;

    public MongoDbBroker()
    {
        _mongoDbContext = new MongoDbContext();
    }

    public void Create(Profile profile)
    {
        _mongoDbContext.Profiles.InsertOneAsync(profile).GetAwaiter().GetResult();
    }

    public Profile? Read(Guid profileId)
    {
        var filterBuilder = Builders<Profile>.Filter;
        var filter = filterBuilder.Eq(x => x.Id, profileId);
        var profiles = _mongoDbContext.Profiles.Find(filter).ToListAsync().GetAwaiter().GetResult();
        var profile = profiles.FirstOrDefault();
        return profile;
    }

    public void CreateAvatar(Avatar avatar)
    {
        _mongoDbContext.Avatars.InsertOneAsync(avatar).GetAwaiter().GetResult();
    }

    public Avatar? ReadAvatar(Guid avatarId)
    {
        var filterBuilder = Builders<Avatar>.Filter;
        var filter = filterBuilder.Eq(x => x.Id, avatarId);
        var profiles = _mongoDbContext.Avatars.Find(filter).ToListAsync().GetAwaiter().GetResult();
        var profile = profiles.FirstOrDefault();
        return profile;
    }

    public void UpdateAvatarLocation(Avatar avatar)
    {
        var filter = Builders<Avatar>.Filter.Eq(x => x.Id, avatar.Id);
        var update = Builders<Avatar>.Update
            .Set(x => x.Location.X, avatar.Location.X)
            .Set(x => x.Location.Y, avatar.Location.Y);
        var result = _mongoDbContext.Avatars.UpdateOneAsync(filter, update).GetAwaiter().GetResult();
        if (!result.IsAcknowledged || result.ModifiedCount == 0)
        {
            Console.WriteLine("Avatar update failed!");
        }
        else
        {
            Console.WriteLine("Avatar update succeeded!");
        }
    }
    
    public void UpsertAvatarLocation(Avatar avatar)
    {
        var filter = Builders<Avatar>.Filter.Eq(x => x.Id, avatar.Id);
        var update = Builders<Avatar>.Update
                .Set(x => x.Id, avatar.Id)
                .Set(x => x.Location, avatar.Location);
        var options = new UpdateOptions { IsUpsert = true };
        var result = _mongoDbContext.Avatars.UpdateOneAsync(filter, update, options).GetAwaiter().GetResult();

        if (result.IsAcknowledged && result.ModifiedCount != 0)
        {
            if (result.UpsertedId != null)
            {
                Console.WriteLine($"Avatar upserted with ID: {result.UpsertedId}");
            }
            else
            {
                Console.WriteLine("Avatar updated successfully!");
            }
        }
        else
        {
            Console.WriteLine("Avatar upsert operation failed!");
        }
    }

}