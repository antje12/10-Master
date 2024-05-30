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

    public void Insert(Player player)
    {
        _mongoDbContext.Players.InsertOneAsync(player).GetAwaiter().GetResult();
    }

    public void Delete(Player player)
    {
        var filterBuilder = Builders<Player>.Filter;
        var filter = filterBuilder.Eq(x => x.Id, player.Id);
        var result = _mongoDbContext.Players.DeleteOneAsync(filter).GetAwaiter().GetResult();
        if (!result.IsAcknowledged || result.DeletedCount == 0)
        {
            Console.WriteLine("Entity delete failed!");
        }
    }

    public Player? GetPlayer(Guid playerId)
    {
        var filterBuilder = Builders<Player>.Filter;
        var filter = filterBuilder.Eq(x => x.Id, playerId);
        var players = _mongoDbContext.Players.OfType<Player>().Find(filter).ToListAsync().GetAwaiter().GetResult();
        var player = players.FirstOrDefault();
        return player;
    }

    public void UpdatePlayer(Player player)
    {
        var filter = Builders<Player>.Filter.Eq(x => x.Id, player.Id);
        var update = Builders<Player>.Update
            .Set(x => x.Location.X, player.Location.X)
            .Set(x => x.Location.Y, player.Location.Y)
            .Set(x => x.Score, player.Score);
        var result = _mongoDbContext.Players.UpdateOneAsync(filter, update).GetAwaiter().GetResult();
        if (!result.IsAcknowledged || result.ModifiedCount == 0)
        {
            Console.WriteLine("Player update failed!");
        }
    }

    public void CleanDB()
    {
        var filter = Builders<Player>.Filter.Empty;
        var result = _mongoDbContext.Players.DeleteManyAsync(filter).GetAwaiter().GetResult();
        if (!result.IsAcknowledged || result.DeletedCount == 0)
        {
            Console.WriteLine("Entity delete failed!");
        }
    }
}