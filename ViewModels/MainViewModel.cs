using System.Runtime.CompilerServices;
using CommunityToolkit.Mvvm.ComponentModel;
using Noxius.Models;
using System.Collections.ObjectModel;
using System.IO;
using System;

namespace Noxius.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    private readonly AudioService _audioService = new();

    public  ObservableCollection<Track> Tracks { get; set; } = new();
    [ObservableProperty] private string _tracksCountText = "0 треков";
    [ObservableProperty] private bool _isPlaying;
    [ObservableProperty] private string _playButtonText = "▶";
    [ObservableProperty] private double _currentSeconds;
    [ObservableProperty] private double _totalSeconds;
    public string CurrentTimeText => TimeSpan.FromSeconds(CurrentSeconds).ToString(@"mm\:ss");
    public string TotalTimeText => TimeSpan.FromSeconds(TotalSeconds).ToString(@"mm\:ss");
    private bool _isEngineInitialized = false;
    private readonly Avalonia.Threading.DispatcherTimer _timer;

    [ObservableProperty] private Track? _currentTrack;
    partial void OnCurrentTrackChanged(Track? value)
    {
        if (value != null)
        {
            _audioService.Play(value.Path);
            _isEngineInitialized = true;
            IsPlaying = true;
            TotalSeconds = _audioService.TotalSeconds;
            CurrentSeconds = 0;
        }
        PlayButtonText = "⏸";

    }

    [ObservableProperty] private float _volume = 0f;
    partial void OnVolumeChanged(float value)
    {
        _audioService.SetVolume(value);
    }


    public MainViewModel()
    {
        LoadLocalTracks();
        _timer = new();
        _timer.Interval = TimeSpan.FromMilliseconds(250);
        _timer.Tick += TimerTick;
        _timer.Start();
        _audioService.TrackFinished += () =>
        {
            Avalonia.Threading.Dispatcher.UIThread.Post(() =>
            {
                IsPlaying = false;
                NextTrack();   
            });
        };


    }

    public void PlayTrack()
    {
        if (CurrentTrack == null) return;

        if (!_isEngineInitialized)
        {
            _audioService.Play(CurrentTrack.Path);
            IsPlaying = true;
            _isEngineInitialized = true;
            PlayButtonText = "⏸";
            return;
        }

        if (IsPlaying)
        {
            _audioService.Pause();
            IsPlaying = false;
            PlayButtonText = "▶";
        }
        else
        {
            _audioService.Resume();
            IsPlaying = true;
            PlayButtonText = "⏸";
        }
    }


    public void LoadLocalTracks()
    {
        var filePaths = Directory.GetFiles(@"C:\Users\artem\Downloads", "*.mp3");

        foreach (var path in filePaths)
        {
            var tfile = TagLib.File.Create(path);

            string title = tfile.Tag.Title;
            string artist = tfile.Tag.FirstPerformer;

            if (string.IsNullOrEmpty(title))
            {
                title = Path.GetFileNameWithoutExtension(path);
            }
            if (string.IsNullOrEmpty(artist))
            {
                artist = "Неизвестный исполнитель";
            }

            TimeSpan duration = tfile.Properties.Duration;

            var track = new Track(path, title, artist, duration);

            Tracks.Add(track);
            

        }
        TracksCountText = $"{Tracks.Count} треков";

    }

    public void NextTrack()
    {
        if (Tracks != null && CurrentTrack != null)
        {
        int currentIndex = Tracks.IndexOf(CurrentTrack);
        int nextIndex = currentIndex + 1;

        if (nextIndex == Tracks.Count)
            {
                nextIndex = 0;
            }
        CurrentTrack = Tracks[nextIndex];
        }
    }

    public void PreviousTrack()
    {
        if (Tracks != null && CurrentTrack != null)
        {
            int currentIndex = Tracks.IndexOf(CurrentTrack);
            int prevIndex = currentIndex - 1;

            if (prevIndex < 0)
            {
                prevIndex = Tracks.Count - 1;
            }
            CurrentTrack = Tracks[prevIndex];
        }
    }

    private void TimerTick(object? sender, EventArgs e)
    {
        if (IsPlaying)
        {
            TotalSeconds = _audioService.TotalSeconds;
            CurrentSeconds = _audioService.CurrentSeconds;
        }
        OnPropertyChanged(nameof(CurrentTimeText));
        OnPropertyChanged(nameof(TotalTimeText));

    }

    partial void OnCurrentSecondsChanged(double value)
    {
        if (Math.Abs(_audioService.CurrentSeconds - value) > 1.5)
        {
            _audioService.Seek(value);
        }
    }

}
