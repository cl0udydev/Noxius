using Microsoft.Extensions.Logging.Abstractions;
using NAudio.Wave;
using TagLib.Mpeg;
using System;

namespace Noxius.Models;

public class AudioService
{
    private AudioFileReader? _audioFile;
    private WaveOutEvent? _outputDevice;
    public event Action? TrackFinished;
    public void Play(string filePath)
    {
        if (_outputDevice != null)
        {
            _outputDevice.Stop();

            _outputDevice.Dispose();
            _audioFile?.Dispose();

            _outputDevice = null;
            _audioFile = null;
        }

        _audioFile = new AudioFileReader(filePath);
        _outputDevice = new WaveOutEvent();

        _outputDevice.PlaybackStopped += (sender, args) =>
        {
            if (_audioFile != null && CurrentSeconds >= TotalSeconds - 2.0)
            {
                TrackFinished?.Invoke();
            }
        };


        
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
    public double CurrentSeconds
    {
        get
        {
            if (_audioFile != null)
            {
                return _audioFile.CurrentTime.TotalSeconds;
            }
            return 0;
        }
    }

    public double TotalSeconds
    {
        get
        {
            if (_audioFile != null)
            {
                return _audioFile.TotalTime.TotalSeconds;
            }
            return 0;
        }
    }

    public void Seek(double seconds)
    {
        if (_audioFile != null)
        {
            _audioFile.CurrentTime = TimeSpan.FromSeconds(seconds);
        }
    }

}