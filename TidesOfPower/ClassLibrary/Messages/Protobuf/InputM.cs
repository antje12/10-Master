// <auto-generated>
//     Generated by the protocol buffer compiler.  DO NOT EDIT!
//     source: Input_M.proto
// </auto-generated>
#pragma warning disable 1591, 0612, 3021, 8981
#region Designer generated code

using pb = global::Google.Protobuf;
using pbc = global::Google.Protobuf.Collections;
using pbr = global::Google.Protobuf.Reflection;
using scg = global::System.Collections.Generic;
namespace ClassLibrary.Messages.Protobuf {

  /// <summary>Holder for reflection information generated from Input_M.proto</summary>
  public static partial class InputMReflection {

    #region Descriptor
    /// <summary>File descriptor for Input_M.proto</summary>
    public static pbr::FileDescriptor Descriptor {
      get { return descriptor; }
    }
    private static pbr::FileDescriptor descriptor;

    static InputMReflection() {
      byte[] descriptorData = global::System.Convert.FromBase64String(
          string.Concat(
            "Cg1JbnB1dF9NLnByb3RvEh5DbGFzc0xpYnJhcnkuTWVzc2FnZXMuUHJvdG9i",
            "dWYaDUNsYXNzZXMucHJvdG8i0AIKB0lucHV0X00SDwoHQWdlbnRJZBgBIAEo",
            "CRJECg1BZ2VudExvY2F0aW9uGAIgASgLMi0uQ2xhc3NMaWJyYXJ5Lk1lc3Nh",
            "Z2VzLlByb3RvYnVmLkNvb3JkaW5hdGVzX00SRAoNTW91c2VMb2NhdGlvbhgD",
            "IAEoCzItLkNsYXNzTGlicmFyeS5NZXNzYWdlcy5Qcm90b2J1Zi5Db29yZGlu",
            "YXRlc19NEjkKCEtleUlucHV0GAQgAygOMicuQ2xhc3NMaWJyYXJ5Lk1lc3Nh",
            "Z2VzLlByb3RvYnVmLkdhbWVLZXkSEAoIR2FtZVRpbWUYBSABKAESEgoKTGFz",
            "dFVwZGF0ZRgGIAEoARIPCgdFdmVudElkGAcgASgJEjYKBlNvdXJjZRgIIAEo",
            "DjImLkNsYXNzTGlicmFyeS5NZXNzYWdlcy5Qcm90b2J1Zi5Tb3VyY2UqSgoH",
            "R2FtZUtleRIGCgJVcBAAEggKBERvd24QARIICgRMZWZ0EAISCQoFUmlnaHQQ",
            "AxIKCgZBdHRhY2sQBBIMCghJbnRlcmFjdBAFKhwKBlNvdXJjZRIKCgZQbGF5",
            "ZXIQABIGCgJBaRABQiGqAh5DbGFzc0xpYnJhcnkuTWVzc2FnZXMuUHJvdG9i",
            "dWZiBnByb3RvMw=="));
      descriptor = pbr::FileDescriptor.FromGeneratedCode(descriptorData,
          new pbr::FileDescriptor[] { global::ClassLibrary.Messages.Protobuf.ClassesReflection.Descriptor, },
          new pbr::GeneratedClrTypeInfo(new[] {typeof(global::ClassLibrary.Messages.Protobuf.GameKey), typeof(global::ClassLibrary.Messages.Protobuf.Source), }, null, new pbr::GeneratedClrTypeInfo[] {
            new pbr::GeneratedClrTypeInfo(typeof(global::ClassLibrary.Messages.Protobuf.Input_M), global::ClassLibrary.Messages.Protobuf.Input_M.Parser, new[]{ "AgentId", "AgentLocation", "MouseLocation", "KeyInput", "GameTime", "LastUpdate", "EventId", "Source" }, null, null, null, null)
          }));
    }
    #endregion

  }
  #region Enums
  public enum GameKey {
    [pbr::OriginalName("Up")] Up = 0,
    [pbr::OriginalName("Down")] Down = 1,
    [pbr::OriginalName("Left")] Left = 2,
    [pbr::OriginalName("Right")] Right = 3,
    [pbr::OriginalName("Attack")] Attack = 4,
    [pbr::OriginalName("Interact")] Interact = 5,
  }

  public enum Source {
    [pbr::OriginalName("Player")] Player = 0,
    [pbr::OriginalName("Ai")] Ai = 1,
  }

  #endregion

  #region Messages
  [global::System.Diagnostics.DebuggerDisplayAttribute("{ToString(),nq}")]
  public sealed partial class Input_M : pb::IMessage<Input_M>
  #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
      , pb::IBufferMessage
  #endif
  {
    private static readonly pb::MessageParser<Input_M> _parser = new pb::MessageParser<Input_M>(() => new Input_M());
    private pb::UnknownFieldSet _unknownFields;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public static pb::MessageParser<Input_M> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::ClassLibrary.Messages.Protobuf.InputMReflection.Descriptor.MessageTypes[0]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public Input_M() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public Input_M(Input_M other) : this() {
      agentId_ = other.agentId_;
      agentLocation_ = other.agentLocation_ != null ? other.agentLocation_.Clone() : null;
      mouseLocation_ = other.mouseLocation_ != null ? other.mouseLocation_.Clone() : null;
      keyInput_ = other.keyInput_.Clone();
      gameTime_ = other.gameTime_;
      lastUpdate_ = other.lastUpdate_;
      eventId_ = other.eventId_;
      source_ = other.source_;
      _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public Input_M Clone() {
      return new Input_M(this);
    }

    /// <summary>Field number for the "AgentId" field.</summary>
    public const int AgentIdFieldNumber = 1;
    private string agentId_ = "";
    /// <summary>
    /// UUIDs are represented as strings in proto
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public string AgentId {
      get { return agentId_; }
      set {
        agentId_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }

    /// <summary>Field number for the "AgentLocation" field.</summary>
    public const int AgentLocationFieldNumber = 2;
    private global::ClassLibrary.Messages.Protobuf.Coordinates_M agentLocation_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public global::ClassLibrary.Messages.Protobuf.Coordinates_M AgentLocation {
      get { return agentLocation_; }
      set {
        agentLocation_ = value;
      }
    }

    /// <summary>Field number for the "MouseLocation" field.</summary>
    public const int MouseLocationFieldNumber = 3;
    private global::ClassLibrary.Messages.Protobuf.Coordinates_M mouseLocation_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public global::ClassLibrary.Messages.Protobuf.Coordinates_M MouseLocation {
      get { return mouseLocation_; }
      set {
        mouseLocation_ = value;
      }
    }

    /// <summary>Field number for the "KeyInput" field.</summary>
    public const int KeyInputFieldNumber = 4;
    private static readonly pb::FieldCodec<global::ClassLibrary.Messages.Protobuf.GameKey> _repeated_keyInput_codec
        = pb::FieldCodec.ForEnum(34, x => (int) x, x => (global::ClassLibrary.Messages.Protobuf.GameKey) x);
    private readonly pbc::RepeatedField<global::ClassLibrary.Messages.Protobuf.GameKey> keyInput_ = new pbc::RepeatedField<global::ClassLibrary.Messages.Protobuf.GameKey>();
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public pbc::RepeatedField<global::ClassLibrary.Messages.Protobuf.GameKey> KeyInput {
      get { return keyInput_; }
    }

    /// <summary>Field number for the "GameTime" field.</summary>
    public const int GameTimeFieldNumber = 5;
    private double gameTime_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public double GameTime {
      get { return gameTime_; }
      set {
        gameTime_ = value;
      }
    }

    /// <summary>Field number for the "LastUpdate" field.</summary>
    public const int LastUpdateFieldNumber = 6;
    private double lastUpdate_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public double LastUpdate {
      get { return lastUpdate_; }
      set {
        lastUpdate_ = value;
      }
    }

    /// <summary>Field number for the "EventId" field.</summary>
    public const int EventIdFieldNumber = 7;
    private string eventId_ = "";
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public string EventId {
      get { return eventId_; }
      set {
        eventId_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }

    /// <summary>Field number for the "Source" field.</summary>
    public const int SourceFieldNumber = 8;
    private global::ClassLibrary.Messages.Protobuf.Source source_ = global::ClassLibrary.Messages.Protobuf.Source.Player;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public global::ClassLibrary.Messages.Protobuf.Source Source {
      get { return source_; }
      set {
        source_ = value;
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public override bool Equals(object other) {
      return Equals(other as Input_M);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool Equals(Input_M other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (AgentId != other.AgentId) return false;
      if (!object.Equals(AgentLocation, other.AgentLocation)) return false;
      if (!object.Equals(MouseLocation, other.MouseLocation)) return false;
      if(!keyInput_.Equals(other.keyInput_)) return false;
      if (!pbc::ProtobufEqualityComparers.BitwiseDoubleEqualityComparer.Equals(GameTime, other.GameTime)) return false;
      if (!pbc::ProtobufEqualityComparers.BitwiseDoubleEqualityComparer.Equals(LastUpdate, other.LastUpdate)) return false;
      if (EventId != other.EventId) return false;
      if (Source != other.Source) return false;
      return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public override int GetHashCode() {
      int hash = 1;
      if (AgentId.Length != 0) hash ^= AgentId.GetHashCode();
      if (agentLocation_ != null) hash ^= AgentLocation.GetHashCode();
      if (mouseLocation_ != null) hash ^= MouseLocation.GetHashCode();
      hash ^= keyInput_.GetHashCode();
      if (GameTime != 0D) hash ^= pbc::ProtobufEqualityComparers.BitwiseDoubleEqualityComparer.GetHashCode(GameTime);
      if (LastUpdate != 0D) hash ^= pbc::ProtobufEqualityComparers.BitwiseDoubleEqualityComparer.GetHashCode(LastUpdate);
      if (EventId.Length != 0) hash ^= EventId.GetHashCode();
      if (Source != global::ClassLibrary.Messages.Protobuf.Source.Player) hash ^= Source.GetHashCode();
      if (_unknownFields != null) {
        hash ^= _unknownFields.GetHashCode();
      }
      return hash;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public override string ToString() {
      return pb::JsonFormatter.ToDiagnosticString(this);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void WriteTo(pb::CodedOutputStream output) {
    #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
      output.WriteRawMessage(this);
    #else
      if (AgentId.Length != 0) {
        output.WriteRawTag(10);
        output.WriteString(AgentId);
      }
      if (agentLocation_ != null) {
        output.WriteRawTag(18);
        output.WriteMessage(AgentLocation);
      }
      if (mouseLocation_ != null) {
        output.WriteRawTag(26);
        output.WriteMessage(MouseLocation);
      }
      keyInput_.WriteTo(output, _repeated_keyInput_codec);
      if (GameTime != 0D) {
        output.WriteRawTag(41);
        output.WriteDouble(GameTime);
      }
      if (LastUpdate != 0D) {
        output.WriteRawTag(49);
        output.WriteDouble(LastUpdate);
      }
      if (EventId.Length != 0) {
        output.WriteRawTag(58);
        output.WriteString(EventId);
      }
      if (Source != global::ClassLibrary.Messages.Protobuf.Source.Player) {
        output.WriteRawTag(64);
        output.WriteEnum((int) Source);
      }
      if (_unknownFields != null) {
        _unknownFields.WriteTo(output);
      }
    #endif
    }

    #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    void pb::IBufferMessage.InternalWriteTo(ref pb::WriteContext output) {
      if (AgentId.Length != 0) {
        output.WriteRawTag(10);
        output.WriteString(AgentId);
      }
      if (agentLocation_ != null) {
        output.WriteRawTag(18);
        output.WriteMessage(AgentLocation);
      }
      if (mouseLocation_ != null) {
        output.WriteRawTag(26);
        output.WriteMessage(MouseLocation);
      }
      keyInput_.WriteTo(ref output, _repeated_keyInput_codec);
      if (GameTime != 0D) {
        output.WriteRawTag(41);
        output.WriteDouble(GameTime);
      }
      if (LastUpdate != 0D) {
        output.WriteRawTag(49);
        output.WriteDouble(LastUpdate);
      }
      if (EventId.Length != 0) {
        output.WriteRawTag(58);
        output.WriteString(EventId);
      }
      if (Source != global::ClassLibrary.Messages.Protobuf.Source.Player) {
        output.WriteRawTag(64);
        output.WriteEnum((int) Source);
      }
      if (_unknownFields != null) {
        _unknownFields.WriteTo(ref output);
      }
    }
    #endif

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public int CalculateSize() {
      int size = 0;
      if (AgentId.Length != 0) {
        size += 1 + pb::CodedOutputStream.ComputeStringSize(AgentId);
      }
      if (agentLocation_ != null) {
        size += 1 + pb::CodedOutputStream.ComputeMessageSize(AgentLocation);
      }
      if (mouseLocation_ != null) {
        size += 1 + pb::CodedOutputStream.ComputeMessageSize(MouseLocation);
      }
      size += keyInput_.CalculateSize(_repeated_keyInput_codec);
      if (GameTime != 0D) {
        size += 1 + 8;
      }
      if (LastUpdate != 0D) {
        size += 1 + 8;
      }
      if (EventId.Length != 0) {
        size += 1 + pb::CodedOutputStream.ComputeStringSize(EventId);
      }
      if (Source != global::ClassLibrary.Messages.Protobuf.Source.Player) {
        size += 1 + pb::CodedOutputStream.ComputeEnumSize((int) Source);
      }
      if (_unknownFields != null) {
        size += _unknownFields.CalculateSize();
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void MergeFrom(Input_M other) {
      if (other == null) {
        return;
      }
      if (other.AgentId.Length != 0) {
        AgentId = other.AgentId;
      }
      if (other.agentLocation_ != null) {
        if (agentLocation_ == null) {
          AgentLocation = new global::ClassLibrary.Messages.Protobuf.Coordinates_M();
        }
        AgentLocation.MergeFrom(other.AgentLocation);
      }
      if (other.mouseLocation_ != null) {
        if (mouseLocation_ == null) {
          MouseLocation = new global::ClassLibrary.Messages.Protobuf.Coordinates_M();
        }
        MouseLocation.MergeFrom(other.MouseLocation);
      }
      keyInput_.Add(other.keyInput_);
      if (other.GameTime != 0D) {
        GameTime = other.GameTime;
      }
      if (other.LastUpdate != 0D) {
        LastUpdate = other.LastUpdate;
      }
      if (other.EventId.Length != 0) {
        EventId = other.EventId;
      }
      if (other.Source != global::ClassLibrary.Messages.Protobuf.Source.Player) {
        Source = other.Source;
      }
      _unknownFields = pb::UnknownFieldSet.MergeFrom(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void MergeFrom(pb::CodedInputStream input) {
    #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
      input.ReadRawMessage(this);
    #else
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, input);
            break;
          case 10: {
            AgentId = input.ReadString();
            break;
          }
          case 18: {
            if (agentLocation_ == null) {
              AgentLocation = new global::ClassLibrary.Messages.Protobuf.Coordinates_M();
            }
            input.ReadMessage(AgentLocation);
            break;
          }
          case 26: {
            if (mouseLocation_ == null) {
              MouseLocation = new global::ClassLibrary.Messages.Protobuf.Coordinates_M();
            }
            input.ReadMessage(MouseLocation);
            break;
          }
          case 34:
          case 32: {
            keyInput_.AddEntriesFrom(input, _repeated_keyInput_codec);
            break;
          }
          case 41: {
            GameTime = input.ReadDouble();
            break;
          }
          case 49: {
            LastUpdate = input.ReadDouble();
            break;
          }
          case 58: {
            EventId = input.ReadString();
            break;
          }
          case 64: {
            Source = (global::ClassLibrary.Messages.Protobuf.Source) input.ReadEnum();
            break;
          }
        }
      }
    #endif
    }

    #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    void pb::IBufferMessage.InternalMergeFrom(ref pb::ParseContext input) {
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, ref input);
            break;
          case 10: {
            AgentId = input.ReadString();
            break;
          }
          case 18: {
            if (agentLocation_ == null) {
              AgentLocation = new global::ClassLibrary.Messages.Protobuf.Coordinates_M();
            }
            input.ReadMessage(AgentLocation);
            break;
          }
          case 26: {
            if (mouseLocation_ == null) {
              MouseLocation = new global::ClassLibrary.Messages.Protobuf.Coordinates_M();
            }
            input.ReadMessage(MouseLocation);
            break;
          }
          case 34:
          case 32: {
            keyInput_.AddEntriesFrom(ref input, _repeated_keyInput_codec);
            break;
          }
          case 41: {
            GameTime = input.ReadDouble();
            break;
          }
          case 49: {
            LastUpdate = input.ReadDouble();
            break;
          }
          case 58: {
            EventId = input.ReadString();
            break;
          }
          case 64: {
            Source = (global::ClassLibrary.Messages.Protobuf.Source) input.ReadEnum();
            break;
          }
        }
      }
    }
    #endif

  }

  #endregion

}

#endregion Designer generated code
