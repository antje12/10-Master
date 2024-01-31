using Avro;
using Avro.Specific;

namespace ClassLibrary;

public partial class PlayerPos : ISpecificRecord
{
    public static Schema _SCHEMA = Avro.Schema.Parse(
        @"{
	        ""namespace"": ""git.avro"",
	        ""type"": ""record"",
	        ""name"": ""Leak"",
	        ""fields"": [
		        {""name"": ""ID"",  ""type"": ""string""},
		        {""name"": ""X"", ""type"": ""int""},
		        {""name"": ""Y"", ""type"": ""int""}
	        ]
        }");

    private string _ID;
    private int _X;
    private int _Y;

    public virtual Schema Schema
    {
        get { return PlayerPos._SCHEMA; }
    }

    public string ID
    {
        get { return this._ID; }
        set { this._ID = value; }
    }

    public int X
    {
        get { return this._X; }
        set { this._X = value; }
    }

    public int Y
    {
        get { return this._Y; }
        set { this._Y = value; }
    }

    public virtual object Get(int fieldPos)
    {
        switch (fieldPos)
        {
            case 0: return this.ID;
            case 1: return this.X;
            case 2: return this.Y;
            default: throw new AvroRuntimeException("Bad index " + fieldPos + " in Get()");
        }
    }

    public virtual void Put(int fieldPos, object fieldValue)
    {
        switch (fieldPos)
        {
            case 0:
                this.ID = (System.String)fieldValue;
                break;
            case 1:
                this.X = (System.Int32)fieldValue;
                break;
            case 2:
                this.Y = (System.Int32)fieldValue;
                break;
            default: throw new AvroRuntimeException("Bad index " + fieldPos + " in Put()");
        }
    }
}