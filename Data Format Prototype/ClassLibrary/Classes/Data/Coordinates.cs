﻿using Avro;
using Avro.Specific;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace ClassLibrary.Classes.Data;

public class Coordinates : ISpecificRecord
{
    [JsonProperty("x")]
    [BsonElement("x")] 
    public float X { get; set; }
    [JsonProperty("y")]
    [BsonElement("y")] 
    public float Y { get; set; }
    
    public Schema Schema => StatSchema();

    public static Schema StatSchema(string nameSpace = "ClassLibrary.Classes.Data")
    {
        return Schema.Parse($@"
        {{
            ""namespace"": ""{nameSpace}"",
            ""type"": ""record"",
            ""name"": ""Coordinates"",
            ""fields"": [
                {{ ""name"": ""X"", ""type"": ""float"" }},
                {{ ""name"": ""Y"", ""type"": ""float"" }}
            ]
        }}");
    }

    public object Get(int fieldPos)
    {
        switch (fieldPos)
        {
            case 0: return X;
            case 1: return Y;
            default: throw new AvroRuntimeException("Bad index " + fieldPos + " in Get()");
        }
    }

    public void Put(int fieldPos, object fieldValue)
    {
        switch (fieldPos)
        {
            case 0:
                X = (float) fieldValue;
                break;
            case 1:
                Y = (float) fieldValue;
                break;
            default: throw new AvroRuntimeException("Bad index " + fieldPos + " in Put()");
        }
    }
}