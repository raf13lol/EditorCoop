using System;

namespace Network.Steam;

[Flags]
public enum MessageFlags
{
    Unreliable = 0,
    NoNagle = 1 << 0,
    NoDelay = 1 << 3,
    Reliable = 1 << 4,
}