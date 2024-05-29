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

    public void Insert(Agent agent)
    {
        _mongoDbContext.Agents.InsertOneAsync(agent).GetAwaiter().GetResult();
    }

    public void Delete(Agent agent)
    {
        var filterBuilder = Builders<Agent>.Filter;
        var filter = filterBuilder.Eq(x => x.Id, agent.Id);
        var result = _mongoDbContext.Agents.DeleteOneAsync(filter).GetAwaiter().GetResult();
        if (!result.IsAcknowledged || result.DeletedCount == 0)
        {
            Console.WriteLine("Entity delete failed!");
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
    }

    public void CleanDB()
    {
        var filter = Builders<Agent>.Filter.Empty;
        var result = _mongoDbContext.Agents.DeleteManyAsync(filter).GetAwaiter().GetResult();
        if (!result.IsAcknowledged || result.DeletedCount == 0)
        {
            Console.WriteLine("Entity delete failed!");
        }
    }
}