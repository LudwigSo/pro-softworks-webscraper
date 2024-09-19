namespace Domain;

public class Keyword(string value)
{
    public int Id { get; init; }
    public int TagId { get; init; }
    public string Value { get; } = value ?? throw new ArgumentNullException(nameof(value));
}