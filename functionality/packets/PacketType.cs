namespace EditorCoop.Functionality.Packets;

public enum PacketType : ushort
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

    CreateConditionals,
    EditConditionals,
    DeleteConditionals,

    RequestLevelSettings,
    SyncLevelSettings,

    RequestAssetHashes,
    SyncAssetHashes,
    RequestAssets,
    SyncAssets,

    // Hm ? there's no 'u' in 'colour' ? I don't know what you're on about.
    SyncSelectedColour,
    SyncSelectedEvents,

    __Template,
}