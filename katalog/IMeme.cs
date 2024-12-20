public interface IMeme
{
    string Name { get; set; }
    string ImagePath { get; set; }
    string Category { get; set; }

    public List<string> Tags { get; set; }
}