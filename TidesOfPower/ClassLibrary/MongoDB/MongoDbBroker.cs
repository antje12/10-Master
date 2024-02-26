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

    public void CreateEntity(Entity entity)
    {
        _mongoDbContext.Entities.InsertOneAsync(entity).GetAwaiter().GetResult();
    }

    public List<Entity> ReadEntities()
    {
        var entities = _mongoDbContext.Entities.AsQueryable().ToListAsync().GetAwaiter().GetResult();
        return entities;
    }

    public Avatar? ReadAvatar(Guid avatarId)
    {
        var filterBuilder = Builders<Avatar>.Filter;
        var filter = filterBuilder.Eq(x => x.Id, avatarId);
        var avatars = _mongoDbContext.Entities.OfType<Avatar>().Find(filter).ToListAsync().GetAwaiter().GetResult();
        var avatar = avatars.FirstOrDefault();
        return avatar;
    }

    public Entity? ReadLocation(Coordinates location)
    {
        var filterBuilder = Builders<Entity>.Filter;
        var filterX = filterBuilder.Eq(x => x.Location.X, location.X);
        var filterY = filterBuilder.Eq(x => x.Location.Y, location.Y);
        var entities = _mongoDbContext.Entities.Find(filterX & filterY).ToListAsync().GetAwaiter().GetResult();
        var entity = entities.FirstOrDefault();
        return entity;
    }

    public List<Entity> ReadCloseScreen(Coordinates location)
    {
        var xFrom = location.X - 50;
        var xTo = location.X + 50;
        var yFrom = location.Y - 50;
        var yTo = location.Y + 50;
        return ReadState(xFrom, xTo, yFrom, yTo);
    }

    public List<Entity> ReadScreen(Coordinates location)
    {
        var xFrom = location.X - 400;
        var xTo = location.X + 400;
        var yFrom = location.Y - 240;
        var yTo = location.Y + 240;
        return ReadState(xFrom, xTo, yFrom, yTo);
    }

    private List<Entity> ReadState(float xFrom, float xTo, float yFrom, float yTo)
    {
        var filterBuilder = Builders<Entity>.Filter;
        var filterX =
            filterBuilder.Gte(x => x.Location.X, xFrom) &
            filterBuilder.Lte(x => x.Location.X, xTo);
        var filterY =
            filterBuilder.Gte(x => x.Location.Y, yFrom) &
            filterBuilder.Lte(x => x.Location.Y, yTo);
        var entities = _mongoDbContext.Entities.Find(filterX & filterY).ToListAsync().GetAwaiter().GetResult();
        return entities;
    }

    public void UpdateAvatarLocation(Avatar avatar)
    {
        var filter = Builders<Entity>.Filter.Eq(x => x.Id, avatar.Id);
        var update = Builders<Entity>.Update
            .Set(x => x.Location.X, avatar.Location.X)
            .Set(x => x.Location.Y, avatar.Location.Y);
        var result = _mongoDbContext.Entities.UpdateOneAsync(filter, update).GetAwaiter().GetResult();
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
        var filter = Builders<Entity>.Filter.Eq(x => x.Id, avatar.Id);
        var update = Builders<Entity>.Update
            .Set(x => x.Id, avatar.Id)
            .Set(x => x.Location, avatar.Location)
            .SetOnInsert("_t", new[] { "Entity", "Avatar" }); // Specify both base and derived types
        var options = new UpdateOptions {IsUpsert = true};
        var result = _mongoDbContext.Entities.UpdateOneAsync(filter, update, options).GetAwaiter().GetResult();

        if (result.IsAcknowledged)
        {
            Console.WriteLine(result.UpsertedId != null
                ? $"Avatar upserted with ID: {result.UpsertedId}"
                : "Avatar updated successfully!");
        }
        else
        {
            Console.WriteLine("Avatar upsert failed!");
        }
    }
}