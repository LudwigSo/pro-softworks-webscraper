namespace Domain;

public class Tag(string name)
{
    public int Id { get; init; }
    public string Name { get; } = name ?? throw new ArgumentNullException(nameof(name));
    public List<Keyword> Keywords{ get; } = new();

    public bool IsApplicable(string? text)
    {
        if (string.IsNullOrEmpty(text)) return false;
        foreach (var keyword in Keywords)
        {
            if (text.Contains(keyword.Value, StringComparison.OrdinalIgnoreCase)) return true;
        }
        return false;
    }

}