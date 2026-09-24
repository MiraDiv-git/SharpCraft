using System.Runtime.InteropServices;
using Miniaudio;
using NVorbis;
using SharpCraft.Engine.Assets;

namespace SharpCraft.Engine.Audio;

public sealed class AudioServer : IDisposable
{
    private unsafe ma_engine* EnginePtr => (ma_engine*)_engineHandle;

    private nint _engineHandle;
    private readonly List<Sound> _sounds = new();
    private bool _disposed;

    public float MasterVolume
    {
        get
        {
            ThrowIfDisposed();
            unsafe { return ma.engine_get_volume(EnginePtr); }
        }
        set
        {
            ThrowIfDisposed();
            unsafe { ma.engine_set_volume(EnginePtr, value); }
        }
    }

    public AudioServer()
    {
        var config = ma.engine_config_init();
        unsafe { _engineHandle = Marshal.AllocHGlobal(sizeof(ma_engine)); }

        unsafe
        {
            if (ma.engine_init(&config, EnginePtr) != ma_result.MA_SUCCESS)
            {
                Marshal.FreeHGlobal(_engineHandle);
                _engineHandle = 0;
                throw new InvalidOperationException("AudioServer: failed to initialize audio engine.");
            }
        }

        Console.WriteLine("[LOAD] Audio Server initialized.");
    }

    public Sound CreateSound(float[] samples, int channels, int sampleRate)
    {
        ThrowIfDisposed();

        var shorts = new short[samples.Length];
        for (int i = 0; i < samples.Length; i++)
            shorts[i] = (short)(Math.Clamp(samples[i], -1f, 1f) * short.MaxValue);

        var pcm = Marshal.AllocHGlobal(shorts.Length * sizeof(short));
        Marshal.Copy(shorts, 0, pcm, shorts.Length);

        Sound sound;
        try
        {
            unsafe { sound = new Sound(EnginePtr, pcm, samples.Length / channels, channels, sampleRate); }
        }
        catch
        {
            Marshal.FreeHGlobal(pcm);
            throw;
        }

        _sounds.Add(sound);
        return sound;
    }

    public Sound LoadAudio(string path)
    {
        ThrowIfDisposed();

        using var vorbis = new VorbisReader(AssetManager.OpenResource(path));
        var channels = vorbis.Channels;
        var sampleRate = vorbis.SampleRate;

        var samples = new float[vorbis.TotalSamples * channels];
        vorbis.ReadSamples(samples, 0, samples.Length);

        return CreateSound(samples, channels, sampleRate);
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;

        foreach (var sound in _sounds)
            sound.Dispose();
        _sounds.Clear();

        if (_engineHandle != 0)
        {
            unsafe { ma.engine_uninit(EnginePtr); }
            Marshal.FreeHGlobal(_engineHandle);
            _engineHandle = 0;
        }
    }

    private void ThrowIfDisposed()
    {
        if (_disposed) throw new ObjectDisposedException(nameof(AudioServer));
    }
}