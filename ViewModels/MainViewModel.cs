using System.Runtime.CompilerServices;
using CommunityToolkit.Mvvm.ComponentModel;
using Noxius.Models;
using System.Collections.ObjectModel;
using System.IO;
using System;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;

namespace Noxius.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    private readonly AudioService _audioService = new();
    public ObservableCollection<Track> Tracks { get; set; } = new();
    public string TracksCountText => $"({Tracks.Count} треков)";
    [ObservableProperty] private bool _isPlaying;
    [ObservableProperty] private double _currentSeconds;
    [ObservableProperty] private double _totalSeconds;
    public string CurrentTimeText => TimeSpan.FromSeconds(CurrentSeconds).ToString(@"mm\:ss");
    public string TotalTimeText => TimeSpan.FromSeconds(TotalSeconds).ToString(@"mm\:ss");
    private bool _isEngineInitialized = false;
    private readonly Avalonia.Threading.DispatcherTimer _timer;
    [ObservableProperty] private Track? _currentTrack;
    private readonly Random _random = new Random();
    private bool _isLooping;
    private bool _isShuffle;

    public bool IsLooping
    {
        get => _isLooping;
        set
        {
            _isLooping = value;
            OnPropertyChanged(nameof(IsLooping));
        }
    }

    public bool IsShuffle
    {
        get => _isShuffle;
        set
        {
            _isShuffle = value;
            OnPropertyChanged(nameof(IsShuffle));
        }
    }

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

    }

    [ObservableProperty] private float _volume = 0f;
    partial void OnVolumeChanged(float value)
    {
        _audioService.SetVolume(value);
    }


    public MainViewModel()
    {
        _timer = new();
        _timer.Interval = TimeSpan.FromMilliseconds(250);
        _timer.Tick += TimerTick;
        _timer.Start();
        _audioService.SetVolume(_volume);

        _audioService.TrackFinished += () =>
        {
            Avalonia.Threading.Dispatcher.UIThread.Post(() => 
            {
                IsPlaying = false;
                
                Track? nextTrack = GetNextTrack(); 
                
                if (nextTrack != null)
                {
                    CurrentTrack = nextTrack;
                    _audioService.Play(nextTrack.Path); 
                    IsPlaying = true;
                }
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
            return;
        }

        if (IsPlaying)
        {
            _audioService.Pause();
            IsPlaying = false;
        }
        else
        {
            _audioService.Resume();
            IsPlaying = true;
        }
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

    public async Task ChangeFolder()
    {
        if (Application.Current?.ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime desktop || 
            desktop.MainWindow == null) return;

        var topLevel = TopLevel.GetTopLevel(desktop.MainWindow);
        if (topLevel == null) return;

        var folders = await topLevel.StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
        {
            Title = "Выберите папку с музыкой",
            AllowMultiple = false
        });

        var folder = folders.FirstOrDefault();
        if (folder == null) return;
        
        string folderPath = folder.Path.LocalPath;

        Tracks.Clear();

        var extensions = new[] { ".mp3", ".wav", ".flac" };
        var audioFiles = Directory.EnumerateFiles(folderPath, "*.*")
            .Where(file => extensions.Contains(Path.GetExtension(file).ToLower()));

        foreach (var file in audioFiles)
        {
            Tracks.Add(new Track(file, Path.GetFileNameWithoutExtension(file), "Неизвестен", TimeSpan.Zero));
        }

        OnPropertyChanged(nameof(TracksCountText));
    }

    private Track? GetNextTrack()
    {
        if (Tracks.Count == 0) return null;

        if (IsLooping && CurrentTrack != null)
        {
            return CurrentTrack;
        }

        if (IsShuffle)
        {
            int randomIndex = _random.Next(0, Tracks.Count);
            return Tracks[randomIndex];
        }

        if (CurrentTrack == null)
        {
            return Tracks[0]; 
        }

        int currentIndex = Tracks.IndexOf(CurrentTrack);
        int nextIndex = currentIndex + 1;

        if (nextIndex >= Tracks.Count)
        {
            return Tracks[0];
        }

        return Tracks[nextIndex];
    }

}
