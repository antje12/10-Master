using Avro;
using Avro.Specific;

namespace ProducerApp.DTOs;

public partial class Message : ISpecificRecord
{
    public static Schema _SCHEMA = Avro.Schema.Parse(
        @"{
	        ""namespace"": ""git.avro"",
	        ""type"": ""record"",
	        ""name"": ""Message"",
	        ""fields"": [
		        {""name"": ""ID"",  ""type"": ""string""},
		        {""name"": ""Num1"",  ""type"": ""string""},
		        {""name"": ""Num2"", ""type"": ""string""}
	        ]
        }");

    private string _ID;
    private string _Num1;
    private string _Num2;

    public virtual Schema Schema
    {
        get { return Message._SCHEMA; }
    }

    public string ID
    {
        get { return this._ID; }
        set { this._ID = value; }
    }

    public string Num1
    {
        get { return this._Num1; }
        set { this._Num1 = value; }
    }

    public string Num2
    {
        get { return this._Num2; }
        set { this._Num2 = value; }
    }

    public virtual object Get(int fieldPos)
    {
        switch (fieldPos)
        {
            case 0: return this.ID;
            case 1: return this.Num1;
            case 2: return this.Num2;
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
                this.Num1 = (System.String)fieldValue;
                break;
            case 2:
                this.Num2 = (System.String)fieldValue;
                break;
            default: throw new AvroRuntimeException("Bad index " + fieldPos + " in Put()");
        }
    }
}