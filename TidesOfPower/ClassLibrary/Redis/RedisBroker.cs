using System.Globalization;
using ClassLibrary.Domain;
using Newtonsoft.Json;
using NRedisStack;
using NRedisStack.RedisStackCommands;
using NRedisStack.Search;
using NRedisStack.Search.Aggregation;
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
            nodes = "34.32.47.73:30006";
        }
        _redis = ConnectionMultiplexer.Connect(nodes);
        _database = _redis.GetDatabase();
        _ft = _database.FT();
        _json = _database.JSON();
        InitEntity();
    }

    public void InitProfile()
    {
        var indexes = _ft._List();
        if (indexes.All(x => x.ToString() != "idx:profiles"))
        {
            var schema = new Schema() // search-able fields
                .AddTextField(new FieldName("$.Id", "Id"))
                .AddTextField(new FieldName("$.Email", "Email"))
                .AddTextField(new FieldName("$.Password", "Password"));
            _ft.Create(
                "idx:profiles",
                new FTCreateParams().On(IndexDataType.JSON).Prefix("profile:"),
                schema);
        }
    }
    
    public void Insert(Profile profile)
    {
        _json.Set($@"profile:{profile.Id}", "$", profile);
    }

    public Profile? GetProfile(Guid profileId)
    {
        var result = _json.Get($"profile:{profileId}");
        return result.IsNull ? null : JsonConvert.DeserializeObject<Profile>(result.ToString());
    }
    
    public Profile? GetProfiles(Guid profileId)
    {
        var src = _ft.Search("idx:profiles", new Query("*").Limit(0, 10000)); // 10000 max
        var json = src.ToJson();
        var res = json.Select(x => JsonConvert.DeserializeObject<Profile>(x));
        return res.FirstOrDefault(x => x.Id == profileId);
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
    
    public List<Entity> GetEntities()
    {
        var src = _ft.Search("idx:entities", new Query("*").Limit(0, 10000)); // 10000 max
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

    public void UpdateProjectile(Projectile entity)
    {
        _json.Set($@"entity:{entity.Id}", ".TimeToLive", entity.TimeToLive);
        _json.Set($@"entity:{entity.Id}", ".Location.X", entity.Location.X);
        _json.Set($@"entity:{entity.Id}", ".Location.Y", entity.Location.Y);
    }

    public virtual void UpsertAgentLocation(Agent entity)
    {        
        _json.Set($@"entity:{entity.Id}", "$", entity);
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

    public void Test()
    {
        var user1 = new
        {
            name = "Paul John",
            email = "paul.john@example.com",
            age = 42,
            city = "London"
        };

        var user2 = new
        {
            name = "Eden Zamir",
            email = "eden.zamir@example.com",
            age = 29,
            city = "Tel Aviv"
        };

        var user3 = new
        {
            name = "Paul Zamir",
            email = "paul.zamir@example.com",
            age = 35,
            city = "Tel Aviv"
        };

        var schema = new Schema() // search-able fields
            .AddTextField(new FieldName("$.name", "name"))
            .AddTagField(new FieldName("$.city", "city"))
            .AddNumericField(new FieldName("$.age", "age"));

        _ft.Create(
            "idx:users",
            new FTCreateParams().On(IndexDataType.JSON).Prefix("user:"),
            schema);

        _json.Set("user:1", "$", user1);
        _json.Set("user:2", "$", user2);
        _json.Set("user:3", "$", user3);

        var src = _ft.Search("idx:users", new Query("Paul @age:[30 40]"));
        var res = src.Documents.Select(x => x["json"]);
        Console.WriteLine(string.Join("\n", res));

        var res_cities = _ft.Search("idx:users", new Query("Paul").ReturnFields(new FieldName("$.city", "city")))
            .Documents.Select(x => x["city"]);
        Console.WriteLine(string.Join(", ", res_cities));

        var request = new AggregationRequest("*").GroupBy("@city", Reducers.Count().As("count"));
        var result = _ft.Aggregate("idx:users", request);

        for (var i = 0; i < result.TotalResults; i++)
        {
            var row = result.GetRow(i);
            Console.WriteLine($"{row["city"]} - {row["count"]}");
        }
    }
}