using Domain;

namespace Application.QueryHandlers.Dtos;

public record TagDto(int Id, string Name, KeywordDto[] Keywords)
{
    public static TagDto From(Tag tag)
    {
        return new TagDto(tag.Id, tag.Name, tag.Keywords.Select(KeywordDto.From).ToArray());
    }
}
