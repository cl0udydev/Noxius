

using System;
using System.Threading;

namespace Noxius.Models;

public class Track
{
    public string Path { get; set; }
    public string Name { get; set; }
    public string Author { get; set; }
    public TimeSpan Duration {get; set; }

    public Track(string path, string name, string author, TimeSpan duration)
    {
        this.Path = path;
        this.Name = name;
        this.Author = author;
        this.Duration = duration;
    }
}