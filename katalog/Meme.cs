public class Meme : IMeme
{
    public string Name { get; set; }
    public string ImagePath { get; set; }
    public string Category { get; set; }
    public List<string> Tags { get; set; } = new List<string>();
}   // класс, который содержит свойства типа стринг, которые используются для получения и хранения имени, пути и категории объекта.