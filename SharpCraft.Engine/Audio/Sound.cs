using System.Runtime.InteropServices;
using Miniaudio;

namespace SharpCraft.Engine.Audio;

public sealed class Sound : IDisposable
{
    private unsafe ma_sound* SoundPtr => (ma_sound*)_soundHandle;
    private unsafe ma_audio_buffer* BufferPtr => (ma_audio_buffer*)_bufferHandle;

    private nint _soundHandle;
    private nint _bufferHandle;
    private nint _pcm;
    private bool _disposed;
    private bool _looping;

    public float Volume
    {
        get
        {
            ThrowIfDisposed();
            unsafe { return ma.sound_get_volume(SoundPtr); }
        }
        set
        {
            ThrowIfDisposed();
            unsafe { ma.sound_set_volume(SoundPtr, value); }
        }
    }

    public float Pitch
    {
        get
        {
            ThrowIfDisposed();
            unsafe { return ma.sound_get_pitch(SoundPtr); }
        }
        set
        {
            ThrowIfDisposed();
            unsafe { ma.sound_set_pitch(SoundPtr, value); }
        }
    }

    public float Pan
    {
        get
        {
            ThrowIfDisposed();
            unsafe { return ma.sound_get_pan(SoundPtr); }
        }
        set
        {
            ThrowIfDisposed();
            unsafe { ma.sound_set_pan(SoundPtr, value); }
        }
    }

    public bool Looping
    {
        get => _looping;
        set
        {
            ThrowIfDisposed();
            _looping = value;
            unsafe { ma.sound_set_looping(SoundPtr, value ? 1u : 0u); }
        }
    }

    public bool IsPlaying
    {
        get
        {
            if (_disposed) return false;
            unsafe { return ma.sound_is_playing(SoundPtr) != 0; }
        }
    }

    public bool IsAtEnd
    {
        get
        {
            if (_disposed) return false;
            unsafe { return ma.sound_at_end(SoundPtr) != 0; }
        }
    }

    internal unsafe Sound(ma_engine* engine, nint pcm, long frames, int channels, int sampleRate)
    {
        _pcm = pcm;

        _bufferHandle = Marshal.AllocHGlobal(sizeof(ma_audio_buffer));
        _soundHandle = Marshal.AllocHGlobal(sizeof(ma_sound));

        var bufferConfig = ma.audio_buffer_config_init(
            ma_format.ma_format_s16, (uint)channels, (ulong)sampleRate, (void*)_pcm, null);

        var bufferResult = ma.audio_buffer_init(&bufferConfig, BufferPtr);
        if (bufferResult != ma_result.MA_SUCCESS)
            throw new InvalidOperationException(
                $"AudioServer: failed to create audio buffer ({bufferResult}).");

        var soundResult = ma.sound_init_from_data_source(engine, BufferPtr, 0, null, SoundPtr);
        if (soundResult != ma_result.MA_SUCCESS)
        {
            ma.audio_buffer_uninit(BufferPtr);
            throw new InvalidOperationException(
                $"AudioServer: failed to create sound ({soundResult}).");
        }

        _looping = false;
        LoopLengthFrames = frames;
    }

    public long LoopLengthFrames { get; }

    public void Play()
    {
        ThrowIfDisposed();
        unsafe
        {
            if (ma.sound_at_end(SoundPtr) != 0)
                ma.sound_seek_to_pcm_frame(SoundPtr, 0);
            ma.sound_start(SoundPtr);
        }
    }

    public void Restart()
    {
        ThrowIfDisposed();
        unsafe
        {
            ma.sound_stop(SoundPtr);
            ma.sound_seek_to_pcm_frame(SoundPtr, 0);
            ma.sound_start(SoundPtr);
        }
    }

    public void Pause()
    {
        ThrowIfDisposed();
        unsafe { ma.sound_stop(SoundPtr); }
    }

    public void Stop()
    {
        ThrowIfDisposed();
        unsafe
        {
            ma.sound_stop(SoundPtr);
            ma.sound_seek_to_pcm_frame(SoundPtr, 0);
        }
    }

    public void SeekTo(long pcmFrame)
    {
        ThrowIfDisposed();
        unsafe { ma.sound_seek_to_pcm_frame(SoundPtr, (ulong)pcmFrame); }
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;

        unsafe
        {
            if (_soundHandle != 0)
            {
                ma.sound_uninit(SoundPtr);
                Marshal.FreeHGlobal(_soundHandle);
                _soundHandle = 0;
            }

            if (_bufferHandle != 0)
            {
                ma.audio_buffer_uninit(BufferPtr);
                Marshal.FreeHGlobal(_bufferHandle);
                _bufferHandle = 0;
            }
        }

        if (_pcm != 0)
        {
            Marshal.FreeHGlobal(_pcm);
            _pcm = 0;
        }
    }

    private void ThrowIfDisposed()
    {
        if (_disposed) throw new ObjectDisposedException(nameof(Sound));
    }
}