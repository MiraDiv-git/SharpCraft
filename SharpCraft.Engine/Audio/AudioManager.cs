using NVorbis;
using Silk.NET.OpenAL;

namespace SharpCraft.Engine.Audio;

public static class AudioManager
{
    private static AL _al;
    private static ALContext _alc;
    private static unsafe Device* _device;
    private static unsafe Context* _context;
    
    public static unsafe void Initialize()
    {
        _alc = ALContext.GetApi();
        _al = AL.GetApi();
        
        _device = _alc.OpenDevice(string.Empty);
        _context = _alc.CreateContext(_device, null);
        _alc.MakeContextCurrent(_context);
        
        Console.WriteLine("[OK] Audio Manager initialized.");
    }
    
    public static Sound LoadAudio(string path)
    {
        using var vorbis = new VorbisReader(path);
        var channels = vorbis.Channels;
        var sampleRate = vorbis.SampleRate;
    
        var buffer = new float[vorbis.TotalSamples * channels];
        vorbis.ReadSamples(buffer, 0, buffer.Length);
    
        var shorts = new short[buffer.Length];
        for (int i = 0; i < buffer.Length; i++)
            shorts[i] = (short)(buffer[i] * short.MaxValue);
    
        var alBuffer = _al.GenBuffer();
        var format = channels == 1 ? BufferFormat.Mono16 : BufferFormat.Stereo16;
    
        unsafe
        {
            fixed (short* ptr = shorts)
                _al.BufferData(alBuffer, format, ptr, shorts.Length * sizeof(short), sampleRate);
        }
    
        return new Sound(alBuffer);
    }

    public static void Play(Sound sound, byte volume = 255)
    {
        var source = _al.GenSource();
        _al.SetSourceProperty(source, SourceFloat.Gain, volume / 255f);
        _al.SetSourceProperty(source, SourceInteger.Buffer, sound.Buffer);
        _al.SourcePlay(source);
    }
}