using Microsoft.Extensions.Logging.Abstractions;
using NAudio.Wave;
using TagLib.Mpeg;

namespace Noxius.Models;

public class AudioService
{
    private AudioFileReader? _audioFile;
    private WaveOutEvent? _outputDevice;

    public void Play(string filePath)
    {
        if (_outputDevice != null)
        {
            _outputDevice.Stop();

            _outputDevice.Dispose();
            _audioFile.Dispose();

            _outputDevice = null;
            _audioFile = null;
        }

        _audioFile = new AudioFileReader(filePath);
        _outputDevice = new WaveOutEvent();
        
        _outputDevice.Init(_audioFile);
        _outputDevice.Play();
    }

    public void Pause()
    {
        if (_outputDevice != null && _outputDevice.PlaybackState == PlaybackState.Playing)
        {
            _outputDevice.Pause();
        }
    }

    public void Resume()
    {
        if (_outputDevice != null && _outputDevice.PlaybackState == PlaybackState.Paused)
        {
            _outputDevice.Play();
        }
    }

    public void SetVolume(float volume)
    {
        if (_outputDevice != null)
        {
            _outputDevice.Volume = volume;
        }
    }
}