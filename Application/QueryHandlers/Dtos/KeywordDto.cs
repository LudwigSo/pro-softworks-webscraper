using Domain;

namespace Application.QueryHandlers.Dtos;

public record KeywordDto(int Id, string Value)
{
    public static KeywordDto From(Keyword keyword)
    {
        return new KeywordDto(keyword.Id, keyword.Value);
    }
}
