namespace EditorCoop.Functionality.Network.Packets;

public enum PacketType : byte
{
    CheckVersion,
    SyncLevel,

    CreateEvents,
    EditEvents,
    MoveEvents,
    

    Test,


    Ping,

    __Template,
}