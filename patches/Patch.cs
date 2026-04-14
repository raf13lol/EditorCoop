using BepInEx.Logging;
using Network.Packets;

namespace EditorCoop.Patches;

public class Patch
{
    public static ManualLogSource Log;

    public static bool CalledFromHandler => Encoding.CallingHandler;
}