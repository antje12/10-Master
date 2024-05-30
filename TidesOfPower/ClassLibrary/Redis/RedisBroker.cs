using System.Globalization;
using ClassLibrary.Domain;
using Newtonsoft.Json;
using NRedisStack;
using NRedisStack.RedisStackCommands;
using NRedisStack.Search;
using NRedisStack.Search.Literals.Enums;
using StackExchange.Redis;

namespace ClassLibrary.Redis;

public class RedisBroker
{
    //https://redis.io/docs/connect/clients/dotnet/
    private string nodes = "redis-stack";
    private ConnectionMultiplexer _redis;
    private IDatabase _database;
    private SearchCommands _ft;
    private JsonCommands _json;

    public virtual void Connect(bool isClient = false)
    {
        if (isClient)
        {
            nodes = "34.32.33.189:30006";
        }
        _redis = ConnectionMultiplexer.Connect(nodes);
        _database = _redis.GetDatabase();
        _ft = _database.FT();
        _json = _database.JSON();
        InitEntity();
    }
    
    public void InitEntity()
    {
        var indexes = _ft._List();
        if (indexes.All(x => x.ToString() != "idx:entities"))
        {
            var schema = new Schema() // search-able fields
                .AddTextField(new FieldName("$.Id", "Id"))
                .AddNumericField(new FieldName("$.Location.X", "Location.X"))
                .AddNumericField(new FieldName("$.Location.Y", "Location.Y"));
            _ft.Create(
                "idx:entities",
                new FTCreateParams().On(IndexDataType.JSON).Prefix("entity:"),
                schema);
        }
    }
    
    public virtual void Insert(Entity entity)
    {
        _json.Set($@"entity:{entity.Id}", "$", entity);
        _database.KeyExpire($@"entity:{entity.Id}", new TimeSpan(0, 0, 1, 0));
    }

    public virtual void DeleteEntity(Guid id)
    {
        _json.Del($@"entity:{id}");
    }

    public virtual Entity? Get(Guid id)
    {
        var get = _json.Get($@"entity:{id}");
        var result = get.IsNull ? null : JsonConvert.DeserializeObject<Entity>(get.ToString());
        if (result != null)
        {
            switch (result.Type)
            {
                case EntityType.Player:
                    return JsonConvert.DeserializeObject<Player>(get.ToString());
                case EntityType.Enemy:
                    return JsonConvert.DeserializeObject<Enemy>(get.ToString());
                case EntityType.Projectile:
                    return JsonConvert.DeserializeObject<Projectile>(get.ToString());
                case EntityType.Ship:
                    return JsonConvert.DeserializeObject<Ship>(get.ToString());
                case EntityType.Treasure:
                    return JsonConvert.DeserializeObject<Treasure>(get.ToString());
            }
        }
        return result;
    }

    public virtual List<Entity> GetCloseEntities(float x, float y)
    {
        var xFrom = (x - 50).ToString(CultureInfo.InvariantCulture);
        var xTo = (x + 50).ToString(CultureInfo.InvariantCulture);
        var yFrom = (y - 50).ToString(CultureInfo.InvariantCulture);
        var yTo = (y + 50).ToString(CultureInfo.InvariantCulture);
        return GetEntities(xFrom, xTo, yFrom, yTo);
    }

    public virtual List<Entity> GetEntities(float x, float y)
    {
        var xFrom = (x - 400).ToString(CultureInfo.InvariantCulture);
        var xTo = (x + 400).ToString(CultureInfo.InvariantCulture);
        var yFrom = (y - 240).ToString(CultureInfo.InvariantCulture);
        var yTo = (y + 240).ToString(CultureInfo.InvariantCulture);
        return GetEntities(xFrom, xTo, yFrom, yTo);
    }
    
    private List<Entity> GetEntities(string xFrom, string xTo, string yFrom, string yTo)
    {
        var query = $"@Location\\.X:[{xFrom} {xTo}] @Location\\.Y:[{yFrom} {yTo}]";
        var src = _ft.Search("idx:entities", new Query(query)
            .Limit(0, 10000)); // 10000 max
        var json = src.ToJson();

        var results = new List<Entity>();
        foreach (var j in json)
        {
            var result = JsonConvert.DeserializeObject<Entity>(j);
            if (result == null) continue;
            switch (result.Type)
            {
                case EntityType.Player:
                    result = JsonConvert.DeserializeObject<Player>(j);
                    break;
                case EntityType.Enemy:
                    result = JsonConvert.DeserializeObject<Enemy>(j);
                    break;
                case EntityType.Projectile:
                    result = JsonConvert.DeserializeObject<Projectile>(j);
                    break;
                case EntityType.Ship:
                    result = JsonConvert.DeserializeObject<Ship>(j);
                    break;
                case EntityType.Treasure:
                    result = JsonConvert.DeserializeObject<Treasure>(j);
                    break;
            }
            if (result == null) continue;
            results.Add(result);
        }
        return results;
    }

    public virtual void UpsertAgentLocation(Agent entity)
    {        
        _json.Set($@"entity:{entity.Id}", "$", entity);
        _database.KeyExpire($@"entity:{entity.Id}", new TimeSpan(0, 0, 1, 0));
    }
    public void Clean()
    {
        var query = new Query("*").Limit(0, 10000); // 10000 max
        var results = _ft.Search("idx:entities", query);
        foreach (var doc in results.Documents)
        {
            var key = doc.Id;
            _json.Del(key);
        }
    }
}