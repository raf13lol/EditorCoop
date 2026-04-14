

namespace EditorCoop.Functionality;

public enum PacketType : byte
{
    CheckVersion,
    
    RequestLevel,
    SyncLevel,

    CreateEvents,
    EditEvents,
    EditBaseEvents,
    MoveEvents,
    DeleteEvents,

    CreateRows,
    MoveRows,
    DeleteRows,

    CreateSprites,
    MoveSprites,
    DeleteSprites,

    CreateWindows,
    DeleteWindows,

    CreateConditionals,
    EditConditionals,
    DeleteConditionals,

    RequestLevelSettings,
    SyncLevelSettings,

    RequestAssetHashes,
    SyncAssetHashes,
    RequestAssets,
    SyncAssets,

    Custom,

    __Template,
}