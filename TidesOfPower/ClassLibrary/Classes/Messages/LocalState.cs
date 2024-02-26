using Avro;
using Avro.Specific;
using ClassLibrary.Classes.Data;
using ClassLibrary.Classes.Domain;

namespace ClassLibrary.Classes.Messages;

public class LocalState : ISpecificRecord
{
    public Guid PlayerId { get; set; }
    public SyncType Sync { get; set; }
    public List<Avatar> Avatars { get; set; }
    public List<Projectile> Projectiles { get; set; }

    public LocalState()
    {
        Avatars = new List<Avatar>();
        Projectiles = new List<Projectile>();
    }

    public Schema Schema => StatSchema;
    public static Schema StatSchema => Schema.Parse($@"
    {{
        ""namespace"": ""ClassLibrary.Classes.Messages"",
        ""type"": ""record"",
        ""name"": ""LocalState"",
        ""fields"": [
            {{ ""name"": ""PlayerId"", ""type"": ""string"" }},
            {{
                ""name"": ""Sync"", 
                ""type"": {{
                    ""type"": ""enum"",
                    ""name"": ""SyncType"",
                    ""symbols"": [""Full"", ""Delta""]
                }}
            }},
            {{ 
                ""name"": ""Avatars"", 
                ""type"": {{ 
                    ""type"": ""array"", 
                    ""items"": {Avatar.StatSchema} 
                }} 
            }},
            {{ 
                ""name"": ""Projectiles"", 
                ""type"": {{ 
                    ""type"": ""array"", 
                    ""items"": {Projectile.StatSchema} 
                }} 
            }}
        ]
    }}");

    public object Get(int fieldPos)
    {
        switch (fieldPos)
        {
            case 0: return PlayerId.ToString();
            case 1: return Sync;
            case 2: return Avatars;
            case 3: return Projectiles;
            default: throw new AvroRuntimeException("Bad index " + fieldPos + " in Get()");
        }
    }

    public void Put(int fieldPos, object fieldValue)
    {
        switch (fieldPos)
        {
            case 0:
                PlayerId = Guid.Parse((string) fieldValue);
                break;
            case 1:
                Sync = (SyncType) fieldValue;
                break;
            case 2:
                Avatars = (List<Avatar>) fieldValue;
                break;
            case 3:
                Projectiles = (List<Projectile>) fieldValue;
                break;
            default: throw new AvroRuntimeException("Bad index " + fieldPos + " in Put()");
        }
    }
}