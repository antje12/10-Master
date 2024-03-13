// <auto-generated>
//     Generated by the protocol buffer compiler.  DO NOT EDIT!
//     source: LocalState.proto
// </auto-generated>
#pragma warning disable 1591, 0612, 3021, 8981
#region Designer generated code

using pb = global::Google.Protobuf;
using pbc = global::Google.Protobuf.Collections;
using pbr = global::Google.Protobuf.Reflection;
using scg = global::System.Collections.Generic;
namespace ClassLibrary.Messages.Protobuf {

  /// <summary>Holder for reflection information generated from LocalState.proto</summary>
  public static partial class LocalStateReflection {

    #region Descriptor
    /// <summary>File descriptor for LocalState.proto</summary>
    public static pbr::FileDescriptor Descriptor {
      get { return descriptor; }
    }
    private static pbr::FileDescriptor descriptor;

    static LocalStateReflection() {
      byte[] descriptorData = global::System.Convert.FromBase64String(
          string.Concat(
            "ChBMb2NhbFN0YXRlLnByb3RvEh5DbGFzc0xpYnJhcnkuTWVzc2FnZXMuUHJv",
            "dG9idWYaDENvbW1vbi5wcm90byLhAQoKTG9jYWxTdGF0ZRIQCghQbGF5ZXJJ",
            "ZBgBIAEoCRI2CgRTeW5jGAIgASgOMiguQ2xhc3NMaWJyYXJ5Lk1lc3NhZ2Vz",
            "LlByb3RvYnVmLlN5bmNUeXBlEjcKB0F2YXRhcnMYAyADKAsyJi5DbGFzc0xp",
            "YnJhcnkuTWVzc2FnZXMuUHJvdG9idWYuQXZhdGFyEj8KC1Byb2plY3RpbGVz",
            "GAQgAygLMiouQ2xhc3NMaWJyYXJ5Lk1lc3NhZ2VzLlByb3RvYnVmLlByb2pl",
            "Y3RpbGUSDwoHRXZlbnRJZBgFIAEoCSKcAQoGQXZhdGFyEgoKAklkGAEgASgJ",
            "Ej0KCExvY2F0aW9uGAIgASgLMisuQ2xhc3NMaWJyYXJ5Lk1lc3NhZ2VzLlBy",
            "b3RvYnVmLkNvb3JkaW5hdGVzEgwKBE5hbWUYAyABKAkSFAoMV2Fsa2luZ1Nw",
            "ZWVkGAQgASgFEhAKCExpZmVQb29sGAUgASgFEhEKCUludmVudG9yeRgGIAEo",
            "BSKmAQoKUHJvamVjdGlsZRIKCgJJZBgBIAEoCRI9CghMb2NhdGlvbhgCIAEo",
            "CzIrLkNsYXNzTGlicmFyeS5NZXNzYWdlcy5Qcm90b2J1Zi5Db29yZGluYXRl",
            "cxI+CglEaXJlY3Rpb24YAyABKAsyKy5DbGFzc0xpYnJhcnkuTWVzc2FnZXMu",
            "UHJvdG9idWYuQ29vcmRpbmF0ZXMSDQoFVGltZXIYBCABKAEqKwoIU3luY1R5",
            "cGUSCAoERnVsbBAAEgkKBURlbHRhEAESCgoGRGVsZXRlEAJCIaoCHkNsYXNz",
            "TGlicmFyeS5NZXNzYWdlcy5Qcm90b2J1ZmIGcHJvdG8z"));
      descriptor = pbr::FileDescriptor.FromGeneratedCode(descriptorData,
          new pbr::FileDescriptor[] { global::ClassLibrary.Messages.Protobuf.CommonReflection.Descriptor, },
          new pbr::GeneratedClrTypeInfo(new[] {typeof(global::ClassLibrary.Messages.Protobuf.SyncType), }, null, new pbr::GeneratedClrTypeInfo[] {
            new pbr::GeneratedClrTypeInfo(typeof(global::ClassLibrary.Messages.Protobuf.LocalState), global::ClassLibrary.Messages.Protobuf.LocalState.Parser, new[]{ "PlayerId", "Sync", "Avatars", "Projectiles", "EventId" }, null, null, null, null),
            new pbr::GeneratedClrTypeInfo(typeof(global::ClassLibrary.Messages.Protobuf.Avatar), global::ClassLibrary.Messages.Protobuf.Avatar.Parser, new[]{ "Id", "Location", "Name", "WalkingSpeed", "LifePool", "Inventory" }, null, null, null, null),
            new pbr::GeneratedClrTypeInfo(typeof(global::ClassLibrary.Messages.Protobuf.Projectile), global::ClassLibrary.Messages.Protobuf.Projectile.Parser, new[]{ "Id", "Location", "Direction", "Timer" }, null, null, null, null)
          }));
    }
    #endregion

  }
  #region Enums
  public enum SyncType {
    [pbr::OriginalName("Full")] Full = 0,
    [pbr::OriginalName("Delta")] Delta = 1,
    [pbr::OriginalName("Delete")] Delete = 2,
  }

  #endregion

  #region Messages
  [global::System.Diagnostics.DebuggerDisplayAttribute("{ToString(),nq}")]
  public sealed partial class LocalState : pb::IMessage<LocalState>
  #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
      , pb::IBufferMessage
  #endif
  {
    private static readonly pb::MessageParser<LocalState> _parser = new pb::MessageParser<LocalState>(() => new LocalState());
    private pb::UnknownFieldSet _unknownFields;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public static pb::MessageParser<LocalState> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::ClassLibrary.Messages.Protobuf.LocalStateReflection.Descriptor.MessageTypes[0]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public LocalState() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public LocalState(LocalState other) : this() {
      playerId_ = other.playerId_;
      sync_ = other.sync_;
      avatars_ = other.avatars_.Clone();
      projectiles_ = other.projectiles_.Clone();
      eventId_ = other.eventId_;
      _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public LocalState Clone() {
      return new LocalState(this);
    }

    /// <summary>Field number for the "PlayerId" field.</summary>
    public const int PlayerIdFieldNumber = 1;
    private string playerId_ = "";
    /// <summary>
    /// UUIDs are represented as strings in proto
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public string PlayerId {
      get { return playerId_; }
      set {
        playerId_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }

    /// <summary>Field number for the "Sync" field.</summary>
    public const int SyncFieldNumber = 2;
    private global::ClassLibrary.Messages.Protobuf.SyncType sync_ = global::ClassLibrary.Messages.Protobuf.SyncType.Full;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public global::ClassLibrary.Messages.Protobuf.SyncType Sync {
      get { return sync_; }
      set {
        sync_ = value;
      }
    }

    /// <summary>Field number for the "Avatars" field.</summary>
    public const int AvatarsFieldNumber = 3;
    private static readonly pb::FieldCodec<global::ClassLibrary.Messages.Protobuf.Avatar> _repeated_avatars_codec
        = pb::FieldCodec.ForMessage(26, global::ClassLibrary.Messages.Protobuf.Avatar.Parser);
    private readonly pbc::RepeatedField<global::ClassLibrary.Messages.Protobuf.Avatar> avatars_ = new pbc::RepeatedField<global::ClassLibrary.Messages.Protobuf.Avatar>();
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public pbc::RepeatedField<global::ClassLibrary.Messages.Protobuf.Avatar> Avatars {
      get { return avatars_; }
    }

    /// <summary>Field number for the "Projectiles" field.</summary>
    public const int ProjectilesFieldNumber = 4;
    private static readonly pb::FieldCodec<global::ClassLibrary.Messages.Protobuf.Projectile> _repeated_projectiles_codec
        = pb::FieldCodec.ForMessage(34, global::ClassLibrary.Messages.Protobuf.Projectile.Parser);
    private readonly pbc::RepeatedField<global::ClassLibrary.Messages.Protobuf.Projectile> projectiles_ = new pbc::RepeatedField<global::ClassLibrary.Messages.Protobuf.Projectile>();
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public pbc::RepeatedField<global::ClassLibrary.Messages.Protobuf.Projectile> Projectiles {
      get { return projectiles_; }
    }

    /// <summary>Field number for the "EventId" field.</summary>
    public const int EventIdFieldNumber = 5;
    private string eventId_ = "";
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public string EventId {
      get { return eventId_; }
      set {
        eventId_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public override bool Equals(object other) {
      return Equals(other as LocalState);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool Equals(LocalState other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (PlayerId != other.PlayerId) return false;
      if (Sync != other.Sync) return false;
      if(!avatars_.Equals(other.avatars_)) return false;
      if(!projectiles_.Equals(other.projectiles_)) return false;
      if (EventId != other.EventId) return false;
      return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public override int GetHashCode() {
      int hash = 1;
      if (PlayerId.Length != 0) hash ^= PlayerId.GetHashCode();
      if (Sync != global::ClassLibrary.Messages.Protobuf.SyncType.Full) hash ^= Sync.GetHashCode();
      hash ^= avatars_.GetHashCode();
      hash ^= projectiles_.GetHashCode();
      if (EventId.Length != 0) hash ^= EventId.GetHashCode();
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
      if (PlayerId.Length != 0) {
        output.WriteRawTag(10);
        output.WriteString(PlayerId);
      }
      if (Sync != global::ClassLibrary.Messages.Protobuf.SyncType.Full) {
        output.WriteRawTag(16);
        output.WriteEnum((int) Sync);
      }
      avatars_.WriteTo(output, _repeated_avatars_codec);
      projectiles_.WriteTo(output, _repeated_projectiles_codec);
      if (EventId.Length != 0) {
        output.WriteRawTag(42);
        output.WriteString(EventId);
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
      if (PlayerId.Length != 0) {
        output.WriteRawTag(10);
        output.WriteString(PlayerId);
      }
      if (Sync != global::ClassLibrary.Messages.Protobuf.SyncType.Full) {
        output.WriteRawTag(16);
        output.WriteEnum((int) Sync);
      }
      avatars_.WriteTo(ref output, _repeated_avatars_codec);
      projectiles_.WriteTo(ref output, _repeated_projectiles_codec);
      if (EventId.Length != 0) {
        output.WriteRawTag(42);
        output.WriteString(EventId);
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
      if (PlayerId.Length != 0) {
        size += 1 + pb::CodedOutputStream.ComputeStringSize(PlayerId);
      }
      if (Sync != global::ClassLibrary.Messages.Protobuf.SyncType.Full) {
        size += 1 + pb::CodedOutputStream.ComputeEnumSize((int) Sync);
      }
      size += avatars_.CalculateSize(_repeated_avatars_codec);
      size += projectiles_.CalculateSize(_repeated_projectiles_codec);
      if (EventId.Length != 0) {
        size += 1 + pb::CodedOutputStream.ComputeStringSize(EventId);
      }
      if (_unknownFields != null) {
        size += _unknownFields.CalculateSize();
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void MergeFrom(LocalState other) {
      if (other == null) {
        return;
      }
      if (other.PlayerId.Length != 0) {
        PlayerId = other.PlayerId;
      }
      if (other.Sync != global::ClassLibrary.Messages.Protobuf.SyncType.Full) {
        Sync = other.Sync;
      }
      avatars_.Add(other.avatars_);
      projectiles_.Add(other.projectiles_);
      if (other.EventId.Length != 0) {
        EventId = other.EventId;
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
            PlayerId = input.ReadString();
            break;
          }
          case 16: {
            Sync = (global::ClassLibrary.Messages.Protobuf.SyncType) input.ReadEnum();
            break;
          }
          case 26: {
            avatars_.AddEntriesFrom(input, _repeated_avatars_codec);
            break;
          }
          case 34: {
            projectiles_.AddEntriesFrom(input, _repeated_projectiles_codec);
            break;
          }
          case 42: {
            EventId = input.ReadString();
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
            PlayerId = input.ReadString();
            break;
          }
          case 16: {
            Sync = (global::ClassLibrary.Messages.Protobuf.SyncType) input.ReadEnum();
            break;
          }
          case 26: {
            avatars_.AddEntriesFrom(ref input, _repeated_avatars_codec);
            break;
          }
          case 34: {
            projectiles_.AddEntriesFrom(ref input, _repeated_projectiles_codec);
            break;
          }
          case 42: {
            EventId = input.ReadString();
            break;
          }
        }
      }
    }
    #endif

  }

  [global::System.Diagnostics.DebuggerDisplayAttribute("{ToString(),nq}")]
  public sealed partial class Avatar : pb::IMessage<Avatar>
  #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
      , pb::IBufferMessage
  #endif
  {
    private static readonly pb::MessageParser<Avatar> _parser = new pb::MessageParser<Avatar>(() => new Avatar());
    private pb::UnknownFieldSet _unknownFields;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public static pb::MessageParser<Avatar> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::ClassLibrary.Messages.Protobuf.LocalStateReflection.Descriptor.MessageTypes[1]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public Avatar() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public Avatar(Avatar other) : this() {
      id_ = other.id_;
      location_ = other.location_ != null ? other.location_.Clone() : null;
      name_ = other.name_;
      walkingSpeed_ = other.walkingSpeed_;
      lifePool_ = other.lifePool_;
      inventory_ = other.inventory_;
      _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public Avatar Clone() {
      return new Avatar(this);
    }

    /// <summary>Field number for the "Id" field.</summary>
    public const int IdFieldNumber = 1;
    private string id_ = "";
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public string Id {
      get { return id_; }
      set {
        id_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }

    /// <summary>Field number for the "Location" field.</summary>
    public const int LocationFieldNumber = 2;
    private global::ClassLibrary.Messages.Protobuf.Coordinates location_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public global::ClassLibrary.Messages.Protobuf.Coordinates Location {
      get { return location_; }
      set {
        location_ = value;
      }
    }

    /// <summary>Field number for the "Name" field.</summary>
    public const int NameFieldNumber = 3;
    private string name_ = "";
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public string Name {
      get { return name_; }
      set {
        name_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }

    /// <summary>Field number for the "WalkingSpeed" field.</summary>
    public const int WalkingSpeedFieldNumber = 4;
    private int walkingSpeed_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public int WalkingSpeed {
      get { return walkingSpeed_; }
      set {
        walkingSpeed_ = value;
      }
    }

    /// <summary>Field number for the "LifePool" field.</summary>
    public const int LifePoolFieldNumber = 5;
    private int lifePool_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public int LifePool {
      get { return lifePool_; }
      set {
        lifePool_ = value;
      }
    }

    /// <summary>Field number for the "Inventory" field.</summary>
    public const int InventoryFieldNumber = 6;
    private int inventory_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public int Inventory {
      get { return inventory_; }
      set {
        inventory_ = value;
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public override bool Equals(object other) {
      return Equals(other as Avatar);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool Equals(Avatar other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (Id != other.Id) return false;
      if (!object.Equals(Location, other.Location)) return false;
      if (Name != other.Name) return false;
      if (WalkingSpeed != other.WalkingSpeed) return false;
      if (LifePool != other.LifePool) return false;
      if (Inventory != other.Inventory) return false;
      return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public override int GetHashCode() {
      int hash = 1;
      if (Id.Length != 0) hash ^= Id.GetHashCode();
      if (location_ != null) hash ^= Location.GetHashCode();
      if (Name.Length != 0) hash ^= Name.GetHashCode();
      if (WalkingSpeed != 0) hash ^= WalkingSpeed.GetHashCode();
      if (LifePool != 0) hash ^= LifePool.GetHashCode();
      if (Inventory != 0) hash ^= Inventory.GetHashCode();
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
      if (Id.Length != 0) {
        output.WriteRawTag(10);
        output.WriteString(Id);
      }
      if (location_ != null) {
        output.WriteRawTag(18);
        output.WriteMessage(Location);
      }
      if (Name.Length != 0) {
        output.WriteRawTag(26);
        output.WriteString(Name);
      }
      if (WalkingSpeed != 0) {
        output.WriteRawTag(32);
        output.WriteInt32(WalkingSpeed);
      }
      if (LifePool != 0) {
        output.WriteRawTag(40);
        output.WriteInt32(LifePool);
      }
      if (Inventory != 0) {
        output.WriteRawTag(48);
        output.WriteInt32(Inventory);
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
      if (Id.Length != 0) {
        output.WriteRawTag(10);
        output.WriteString(Id);
      }
      if (location_ != null) {
        output.WriteRawTag(18);
        output.WriteMessage(Location);
      }
      if (Name.Length != 0) {
        output.WriteRawTag(26);
        output.WriteString(Name);
      }
      if (WalkingSpeed != 0) {
        output.WriteRawTag(32);
        output.WriteInt32(WalkingSpeed);
      }
      if (LifePool != 0) {
        output.WriteRawTag(40);
        output.WriteInt32(LifePool);
      }
      if (Inventory != 0) {
        output.WriteRawTag(48);
        output.WriteInt32(Inventory);
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
      if (Id.Length != 0) {
        size += 1 + pb::CodedOutputStream.ComputeStringSize(Id);
      }
      if (location_ != null) {
        size += 1 + pb::CodedOutputStream.ComputeMessageSize(Location);
      }
      if (Name.Length != 0) {
        size += 1 + pb::CodedOutputStream.ComputeStringSize(Name);
      }
      if (WalkingSpeed != 0) {
        size += 1 + pb::CodedOutputStream.ComputeInt32Size(WalkingSpeed);
      }
      if (LifePool != 0) {
        size += 1 + pb::CodedOutputStream.ComputeInt32Size(LifePool);
      }
      if (Inventory != 0) {
        size += 1 + pb::CodedOutputStream.ComputeInt32Size(Inventory);
      }
      if (_unknownFields != null) {
        size += _unknownFields.CalculateSize();
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void MergeFrom(Avatar other) {
      if (other == null) {
        return;
      }
      if (other.Id.Length != 0) {
        Id = other.Id;
      }
      if (other.location_ != null) {
        if (location_ == null) {
          Location = new global::ClassLibrary.Messages.Protobuf.Coordinates();
        }
        Location.MergeFrom(other.Location);
      }
      if (other.Name.Length != 0) {
        Name = other.Name;
      }
      if (other.WalkingSpeed != 0) {
        WalkingSpeed = other.WalkingSpeed;
      }
      if (other.LifePool != 0) {
        LifePool = other.LifePool;
      }
      if (other.Inventory != 0) {
        Inventory = other.Inventory;
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
            Id = input.ReadString();
            break;
          }
          case 18: {
            if (location_ == null) {
              Location = new global::ClassLibrary.Messages.Protobuf.Coordinates();
            }
            input.ReadMessage(Location);
            break;
          }
          case 26: {
            Name = input.ReadString();
            break;
          }
          case 32: {
            WalkingSpeed = input.ReadInt32();
            break;
          }
          case 40: {
            LifePool = input.ReadInt32();
            break;
          }
          case 48: {
            Inventory = input.ReadInt32();
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
            Id = input.ReadString();
            break;
          }
          case 18: {
            if (location_ == null) {
              Location = new global::ClassLibrary.Messages.Protobuf.Coordinates();
            }
            input.ReadMessage(Location);
            break;
          }
          case 26: {
            Name = input.ReadString();
            break;
          }
          case 32: {
            WalkingSpeed = input.ReadInt32();
            break;
          }
          case 40: {
            LifePool = input.ReadInt32();
            break;
          }
          case 48: {
            Inventory = input.ReadInt32();
            break;
          }
        }
      }
    }
    #endif

  }

  [global::System.Diagnostics.DebuggerDisplayAttribute("{ToString(),nq}")]
  public sealed partial class Projectile : pb::IMessage<Projectile>
  #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
      , pb::IBufferMessage
  #endif
  {
    private static readonly pb::MessageParser<Projectile> _parser = new pb::MessageParser<Projectile>(() => new Projectile());
    private pb::UnknownFieldSet _unknownFields;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public static pb::MessageParser<Projectile> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::ClassLibrary.Messages.Protobuf.LocalStateReflection.Descriptor.MessageTypes[2]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public Projectile() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public Projectile(Projectile other) : this() {
      id_ = other.id_;
      location_ = other.location_ != null ? other.location_.Clone() : null;
      direction_ = other.direction_ != null ? other.direction_.Clone() : null;
      timer_ = other.timer_;
      _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public Projectile Clone() {
      return new Projectile(this);
    }

    /// <summary>Field number for the "Id" field.</summary>
    public const int IdFieldNumber = 1;
    private string id_ = "";
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public string Id {
      get { return id_; }
      set {
        id_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }

    /// <summary>Field number for the "Location" field.</summary>
    public const int LocationFieldNumber = 2;
    private global::ClassLibrary.Messages.Protobuf.Coordinates location_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public global::ClassLibrary.Messages.Protobuf.Coordinates Location {
      get { return location_; }
      set {
        location_ = value;
      }
    }

    /// <summary>Field number for the "Direction" field.</summary>
    public const int DirectionFieldNumber = 3;
    private global::ClassLibrary.Messages.Protobuf.Coordinates direction_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public global::ClassLibrary.Messages.Protobuf.Coordinates Direction {
      get { return direction_; }
      set {
        direction_ = value;
      }
    }

    /// <summary>Field number for the "Timer" field.</summary>
    public const int TimerFieldNumber = 4;
    private double timer_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public double Timer {
      get { return timer_; }
      set {
        timer_ = value;
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public override bool Equals(object other) {
      return Equals(other as Projectile);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool Equals(Projectile other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (Id != other.Id) return false;
      if (!object.Equals(Location, other.Location)) return false;
      if (!object.Equals(Direction, other.Direction)) return false;
      if (!pbc::ProtobufEqualityComparers.BitwiseDoubleEqualityComparer.Equals(Timer, other.Timer)) return false;
      return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public override int GetHashCode() {
      int hash = 1;
      if (Id.Length != 0) hash ^= Id.GetHashCode();
      if (location_ != null) hash ^= Location.GetHashCode();
      if (direction_ != null) hash ^= Direction.GetHashCode();
      if (Timer != 0D) hash ^= pbc::ProtobufEqualityComparers.BitwiseDoubleEqualityComparer.GetHashCode(Timer);
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
      if (Id.Length != 0) {
        output.WriteRawTag(10);
        output.WriteString(Id);
      }
      if (location_ != null) {
        output.WriteRawTag(18);
        output.WriteMessage(Location);
      }
      if (direction_ != null) {
        output.WriteRawTag(26);
        output.WriteMessage(Direction);
      }
      if (Timer != 0D) {
        output.WriteRawTag(33);
        output.WriteDouble(Timer);
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
      if (Id.Length != 0) {
        output.WriteRawTag(10);
        output.WriteString(Id);
      }
      if (location_ != null) {
        output.WriteRawTag(18);
        output.WriteMessage(Location);
      }
      if (direction_ != null) {
        output.WriteRawTag(26);
        output.WriteMessage(Direction);
      }
      if (Timer != 0D) {
        output.WriteRawTag(33);
        output.WriteDouble(Timer);
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
      if (Id.Length != 0) {
        size += 1 + pb::CodedOutputStream.ComputeStringSize(Id);
      }
      if (location_ != null) {
        size += 1 + pb::CodedOutputStream.ComputeMessageSize(Location);
      }
      if (direction_ != null) {
        size += 1 + pb::CodedOutputStream.ComputeMessageSize(Direction);
      }
      if (Timer != 0D) {
        size += 1 + 8;
      }
      if (_unknownFields != null) {
        size += _unknownFields.CalculateSize();
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void MergeFrom(Projectile other) {
      if (other == null) {
        return;
      }
      if (other.Id.Length != 0) {
        Id = other.Id;
      }
      if (other.location_ != null) {
        if (location_ == null) {
          Location = new global::ClassLibrary.Messages.Protobuf.Coordinates();
        }
        Location.MergeFrom(other.Location);
      }
      if (other.direction_ != null) {
        if (direction_ == null) {
          Direction = new global::ClassLibrary.Messages.Protobuf.Coordinates();
        }
        Direction.MergeFrom(other.Direction);
      }
      if (other.Timer != 0D) {
        Timer = other.Timer;
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
            Id = input.ReadString();
            break;
          }
          case 18: {
            if (location_ == null) {
              Location = new global::ClassLibrary.Messages.Protobuf.Coordinates();
            }
            input.ReadMessage(Location);
            break;
          }
          case 26: {
            if (direction_ == null) {
              Direction = new global::ClassLibrary.Messages.Protobuf.Coordinates();
            }
            input.ReadMessage(Direction);
            break;
          }
          case 33: {
            Timer = input.ReadDouble();
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
            Id = input.ReadString();
            break;
          }
          case 18: {
            if (location_ == null) {
              Location = new global::ClassLibrary.Messages.Protobuf.Coordinates();
            }
            input.ReadMessage(Location);
            break;
          }
          case 26: {
            if (direction_ == null) {
              Direction = new global::ClassLibrary.Messages.Protobuf.Coordinates();
            }
            input.ReadMessage(Direction);
            break;
          }
          case 33: {
            Timer = input.ReadDouble();
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
