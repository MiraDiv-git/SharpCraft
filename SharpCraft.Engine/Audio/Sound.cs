namespace SharpCraft.Engine.Audio;

public class Sound
{
    internal uint Buffer { get; }
    internal uint ActiveSource { get; set; }

    internal Sound(uint buffer) => Buffer = buffer;
}