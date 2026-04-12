namespace EditorCoop.Functionality.Network;

public enum PacketType : byte
{
    CheckVersion,
    
    RequestLevel,
    SyncLevel,

    CreateEvents,
    EditEvents,
    MoveEvents,
    DeleteEvents,

    CreateRows,
    EditRows,
    MoveRows,
    DeleteRows,

    CreateSprites,
    EditSprites,
    MoveSprites,
    DeleteSprites,

    RequestAssetHashes,
    RequestAssets,
    SyncAssets,

    __Template,
}