using ClassLibrary.Domain;
using MongoDB.Driver;

namespace ClassLibrary.MongoDB;

public class MongoDbBroker
{
    private MongoDbContext _mongoDbContext;
    
    public virtual void Connect(bool isClient = false)
    {
        _mongoDbContext = new MongoDbContext(isClient);
    }

    public void Insert(Profile profile)
    {
        _mongoDbContext.Profiles.InsertOneAsync(profile).GetAwaiter().GetResult();
    }

    public Profile? GetProfile(Guid profileId)
    {
        var filterBuilder = Builders<Profile>.Filter;
        var filter = filterBuilder.Eq(x => x.Id, profileId);
        var profiles = _mongoDbContext.Profiles.Find(filter).ToListAsync().GetAwaiter().GetResult();
        var profile = profiles.FirstOrDefault();
        return profile;
    }

    public void Insert(Entity entity)
    {
        _mongoDbContext.Entities.InsertOneAsync(entity).GetAwaiter().GetResult();
    }

    public void Delete(Entity entity)
    {
        var filterBuilder = Builders<Entity>.Filter;
        var filter = filterBuilder.Eq(x => x.Id, entity.Id);
        var result = _mongoDbContext.Entities.DeleteOneAsync(filter).GetAwaiter().GetResult();
        if (!result.IsAcknowledged || result.DeletedCount == 0)
        {
            Console.WriteLine("Entity delete failed!");
        }
        else
        {
            //Console.WriteLine("Entity delete succeeded!");
        }
    }

    public List<Entity> GetEntities()
    {
        var entities = _mongoDbContext.Entities.AsQueryable().ToListAsync().GetAwaiter().GetResult();
        return entities;
    }

    public Entity? GetEntity(Coordinates location)
    {
        var filterBuilder = Builders<Entity>.Filter;
        var filterX = filterBuilder.Eq(x => x.Location.X, location.X);
        var filterY = filterBuilder.Eq(x => x.Location.Y, location.Y);
        var entities = _mongoDbContext.Entities.Find(filterX & filterY).ToListAsync().GetAwaiter().GetResult();
        var entity = entities.FirstOrDefault();
        return entity;
    }

    public List<Entity> GetCloseEntities(float x, float y)
    {
        var xFrom = x - 50;
        var xTo = x + 50;
        var yFrom = y - 50;
        var yTo = y + 50;
        return GetEntities(xFrom, xTo, yFrom, yTo);
    }

    public List<Entity> GetEntities(float x, float y)
    {
        var xFrom = x - 400;
        var xTo = x + 400;
        var yFrom = y - 240;
        var yTo = y + 240;
        return GetEntities(xFrom, xTo, yFrom, yTo);
    }

    private List<Entity> GetEntities(float xFrom, float xTo, float yFrom, float yTo)
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

    public void UpdateProjectile(Projectile entity)
    {
        var filter = Builders<Entity>.Filter.Eq(x => x.Id, entity.Id);
        var update = Builders<Entity>.Update
            .Set(x => x.Location.X, entity.Location.X)
            .Set(x => x.Location.Y, entity.Location.Y)
            .Set("timetolive", entity.TimeToLive);
        var result = _mongoDbContext.Entities.UpdateOneAsync(filter, update).GetAwaiter().GetResult();
        if (!result.IsAcknowledged || result.ModifiedCount == 0)
        {
            Console.WriteLine("Entity update failed!");
        }
        else
        {
            //Console.WriteLine("Entity update succeeded!");
        }
    }

    public Agent? GetAgent(Guid agentId)
    {
        var filterBuilder = Builders<Agent>.Filter;
        var filter = filterBuilder.Eq(x => x.Id, agentId);
        var agents = _mongoDbContext.Agents.OfType<Agent>().Find(filter).ToListAsync().GetAwaiter().GetResult();
        var agent = agents.FirstOrDefault();
        return agent;
    }

    public void UpdateAgentLocation(Agent agent)
    {
        var filter = Builders<Agent>.Filter.Eq(x => x.Id, agent.Id);
        var update = Builders<Agent>.Update
            .Set(x => x.Location.X, agent.Location.X)
            .Set(x => x.Location.Y, agent.Location.Y);
        var result = _mongoDbContext.Agents.UpdateOneAsync(filter, update).GetAwaiter().GetResult();
        if (!result.IsAcknowledged || result.ModifiedCount == 0)
        {
            Console.WriteLine("Agent update failed!");
        }
        else
        {
            //Console.WriteLine("Agent update succeeded!");
        }
    }

    public void UpsertAgentLocation(Agent agent)
    {
        var filter = Builders<Entity>.Filter.Eq(x => x.Id, agent.Id);
        var update = Builders<Entity>.Update
            .Set(x => x.Id, agent.Id)
            .Set(x => x.Location, agent.Location)
            .SetOnInsert("_t", new[] { "Entity", "Agent" }); // Specify both base and derived types
        var options = new UpdateOptions {IsUpsert = true};
        var result = _mongoDbContext.Entities.UpdateOneAsync(filter, update, options).GetAwaiter().GetResult();
        if (result.IsAcknowledged)
        {
            //Console.WriteLine(result.UpsertedId != null
            //    ? $"Agent upserted with ID: {result.UpsertedId}"
            //    : "Agent updated successfully!");
        }
        else
        {
            Console.WriteLine("Agent upsert failed!");
        }
    }

    public void CleanDB()
    {
        var filter = Builders<Entity>.Filter.Empty;
        var result = _mongoDbContext.Entities.DeleteManyAsync(filter).GetAwaiter().GetResult();
        if (!result.IsAcknowledged || result.DeletedCount == 0)
        {
            Console.WriteLine("Entity delete failed!");
        }
    }
}