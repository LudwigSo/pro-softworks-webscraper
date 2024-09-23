namespace Domain;

public class Keyword(int tagId, string value)
{
    public int Id { get; init; }
    public int TagId { get; } = tagId;
    public string Value { get; } = value ?? throw new ArgumentNullException(nameof(value));
}