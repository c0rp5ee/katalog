public abstract class VideoMeme : IMeme
{
    public string Name { get; set; }
    public string ImagePath { get; set; }
    public string Category { get; set; }

    public List<string> Tags { get; set; } = new List<string>();

}