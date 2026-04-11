namespace EditorCoop.Functionality.Network;

public enum PacketType : byte
{
    CheckVersion,
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

    
    

    Test,


    Ping,

    __Template,
}