using System;
using EditorCoop.Functionality.Packets;

namespace EditorCoop.Functionality.Handlers;

public class CustomHandler : Handler
{
    public static event Action<CustomPacket> OnCustomPacketRead;

    public static void Run(CustomPacket packet)
    {
        OnCustomPacketRead.Invoke(packet);
    }
}