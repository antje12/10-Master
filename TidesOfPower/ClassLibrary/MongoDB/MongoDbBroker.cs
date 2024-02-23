using ClassLibrary.Classes.Data;
using ClassLibrary.Classes.Domain;
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

    public List<Avatar> ReadAvatars()
    {
        var avatars = _mongoDbContext.Avatars.AsQueryable().ToListAsync().GetAwaiter().GetResult();
        return avatars;
    }

    public Avatar? ReadAvatar(Guid avatarId)
    {
        var filterBuilder = Builders<Avatar>.Filter;
        var filter = filterBuilder.Eq(x => x.Id, avatarId);
        var avatars = _mongoDbContext.Avatars.Find(filter).ToListAsync().GetAwaiter().GetResult();
        var avatar = avatars.FirstOrDefault();
        return avatar;
    }

    public Avatar? ReadLocation(Coordinates location)
    {
        var filterBuilder = Builders<Avatar>.Filter;
        var filterX = filterBuilder.Eq(x => x.Location.X, location.X);
        var filterY = filterBuilder.Eq(x => x.Location.Y, location.Y);
        var avatars = _mongoDbContext.Avatars.Find(filterX & filterY).ToListAsync().GetAwaiter().GetResult();
        var avatar = avatars.FirstOrDefault();
        return avatar;
    }

    public List<Avatar> ReadCloseScreen(Coordinates location)
    {
        var xFrom = location.X - 50;
        var xTo = location.X + 50;
        var yFrom = location.Y - 50;
        var yTo = location.Y + 50;
        return ReadState(xFrom, xTo, yFrom, yTo);
    }

    public List<Avatar> ReadScreen(Coordinates location)
    {
        var xFrom = location.X - 400;
        var xTo = location.X + 400;
        var yFrom = location.Y - 240;
        var yTo = location.Y + 240;
        return ReadState(xFrom, xTo, yFrom, yTo);
    }

    private List<Avatar> ReadState(float xFrom, float xTo, float yFrom, float yTo)
    {
        var filterBuilder = Builders<Avatar>.Filter;
        var filterX =
            filterBuilder.Gte(x => x.Location.X, xFrom) &
            filterBuilder.Lte(x => x.Location.X, xTo);
        var filterY =
            filterBuilder.Gte(x => x.Location.Y, yFrom) &
            filterBuilder.Lte(x => x.Location.Y, yTo);
        var avatars = _mongoDbContext.Avatars.Find(filterX & filterY).ToListAsync().GetAwaiter().GetResult();
        return avatars;
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
        var options = new UpdateOptions {IsUpsert = true};
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
            Console.WriteLine("Avatar upsert failed!");
        }
    }
}