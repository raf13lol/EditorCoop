using System;
using System.IO;
using Network.Packets;

namespace EditorCoop.Functionality.Network.Packets;

public class TestPacket() : Packet
{
    public string RandomString;

    public override void Decode(BinaryReader reader)
    {
        RandomString = reader.ReadString();
    }

    public override void Encode(BinaryWriter writer)
    {
        string lettersAvailable = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!\"£$%^&*()-=_+[]{};'#:@~,./<>?|\\`¬";
        Random rand = new();

        RandomString = "";
        for (int i = 0; i < 16; i++)
            RandomString += lettersAvailable[rand.Next(lettersAvailable.Length)];
    
        writer.Write(RandomString);
        
        Patch.Log.LogMessage($"TestPacket sent to others with Rand string {RandomString}");
    }
}