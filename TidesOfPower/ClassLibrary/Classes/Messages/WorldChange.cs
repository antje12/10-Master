﻿using Avro;
using Avro.Specific;
using ClassLibrary.Classes.Data;

namespace ClassLibrary.Classes.Messages;

public class WorldChange : ISpecificRecord
{
    public Guid PlayerId { get; set; }
    public Coordinates NewLocation { get; set; }

    public WorldChange()
    {
        NewLocation = new Coordinates();
    }

    public Schema Schema => StatSchema;
    public static Schema StatSchema => Schema.Parse($@"
    {{
        ""namespace"": ""ClassLibrary.Classes.Messages"",
        ""type"": ""record"",
        ""name"": ""WorldChange"",
        ""fields"": [
            {{ ""name"": ""PlayerId"", ""type"": ""string"" }},
            {{ ""name"": ""NewLocation"", ""type"": {Coordinates.StatSchema} }}
        ]
    }}");

    public object Get(int fieldPos)
    {
        switch (fieldPos)
        {
            case 0: return PlayerId.ToString();
            case 1: return NewLocation;
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
                NewLocation = (Coordinates) fieldValue;
                break;
            default: throw new AvroRuntimeException("Bad index " + fieldPos + " in Put()");
        }
    }
}