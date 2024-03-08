using System.Globalization;
using ClassLibrary.Classes.Data;
using ClassLibrary.Classes.Domain;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
    private readonly ConnectionMultiplexer _redis;
    private readonly IDatabase _database;
    private readonly SearchCommands _ft;
    private readonly JsonCommands _json;

    public RedisBroker(bool isClient = false)
    {
        if (isClient)
        {
            nodes = "localhost";
        }

        _redis = ConnectionMultiplexer.Connect(nodes);
        _database = _redis.GetDatabase();
        _ft = _database.FT();
        _json = _database.JSON();
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
        var profile = _json.Get($"profile:{profileId}");
        return JsonConvert.DeserializeObject<Profile>(profile.ToString());
    }
    
    public Profile? GetProfiles(Guid profileId)
    {
        var src = _ft.Search("idx:profiles", new Query("*").Limit(0, 10000)); // 10000 max, may be a problem
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
    
    public void Insert(Entity entity)
    {
        _json.Set($@"entity:{entity.Id}", "$", entity);
    }
    
    public List<Entity> GetEntities()
    {
        var src = _ft.Search("idx:entities", new Query("*").Limit(0, 10000)); // 10000 max, may be a problem
        var json = src.ToJson();
        var res = json.Select(x => JsonConvert.DeserializeObject<Entity>(x));
        return res.ToList();
    }
    
    public List<Entity> GetEntities(Coordinates location)
    {
        var xFrom = (location.X - 50).ToString(CultureInfo.InvariantCulture);
        var xTo = (location.X + 50).ToString(CultureInfo.InvariantCulture);
        var yFrom = (location.Y - 50).ToString(CultureInfo.InvariantCulture);
        var yTo = (location.Y + 50).ToString(CultureInfo.InvariantCulture);
        var query = $"@Location\\.X:[{xFrom} {xTo}] @Location\\.Y:[{yFrom} {yTo}]";
        
        var src = _ft.Search("idx:entities", new Query(query)
            .Limit(0, 10000)); // 10000 max, may be a problem
        var json = src.ToJson();

        var j = json[0];
        var jsonObject = JObject.Parse(j);
        var typeValue = jsonObject["Type"].ToString();
        var typeEnum = (TheEntityType)Enum.Parse(typeof(TheEntityType), typeValue);
        
        var res = json.Select(x => JsonConvert.DeserializeObject<Avatar>(x));
        return new List<Entity>();
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