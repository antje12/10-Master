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
            "dG9idWYaDUNsYXNzZXMucHJvdG8i4QEKCkxvY2FsU3RhdGUSEAoIUGxheWVy",
            "SWQYASABKAkSNgoEU3luYxgCIAEoDjIoLkNsYXNzTGlicmFyeS5NZXNzYWdl",
            "cy5Qcm90b2J1Zi5TeW5jVHlwZRI3CgdBdmF0YXJzGAMgAygLMiYuQ2xhc3NM",
            "aWJyYXJ5Lk1lc3NhZ2VzLlByb3RvYnVmLkF2YXRhchI/CgtQcm9qZWN0aWxl",
            "cxgEIAMoCzIqLkNsYXNzTGlicmFyeS5NZXNzYWdlcy5Qcm90b2J1Zi5Qcm9q",
            "ZWN0aWxlEg8KB0V2ZW50SWQYBSABKAkqKwoIU3luY1R5cGUSCAoERnVsbBAA",
            "EgkKBURlbHRhEAESCgoGRGVsZXRlEAJCIaoCHkNsYXNzTGlicmFyeS5NZXNz",
            "YWdlcy5Qcm90b2J1ZmIGcHJvdG8z"));
      descriptor = pbr::FileDescriptor.FromGeneratedCode(descriptorData,
          new pbr::FileDescriptor[] { global::ClassLibrary.Messages.Protobuf.ClassesReflection.Descriptor, },
          new pbr::GeneratedClrTypeInfo(new[] {typeof(global::ClassLibrary.Messages.Protobuf.SyncType), }, null, new pbr::GeneratedClrTypeInfo[] {
            new pbr::GeneratedClrTypeInfo(typeof(global::ClassLibrary.Messages.Protobuf.LocalState), global::ClassLibrary.Messages.Protobuf.LocalState.Parser, new[]{ "PlayerId", "Sync", "Avatars", "Projectiles", "EventId" }, null, null, null, null)
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

  #endregion

}

#endregion Designer generated code
