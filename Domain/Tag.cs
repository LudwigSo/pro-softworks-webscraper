namespace Domain;

public class Tag(string name)
{
    public int Id { get; init; }
    public string Name { get; } = name ?? throw new ArgumentNullException(nameof(name));
}