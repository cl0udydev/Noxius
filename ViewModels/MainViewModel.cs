using System.Runtime.CompilerServices;
using CommunityToolkit.Mvvm.ComponentModel;
using Noxius.Models;

namespace Noxius.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    private readonly AudioService _audioService = new();

    [ObservableProperty]
    public partial string Greeting { get; set; } = "Welcome to Avalonia!";

    [ObservableProperty]
    private float _volume = 0.3f;

    public void PlayTrack()
    {
        _audioService.Play(@"C:\Users\artem\Downloads\And_One_-_A_Kind_of_Deutsch_(SkySound.cc).mp3");
    }
    public void PauseTrack(){}
    public void ResumeTrack(){}

}
