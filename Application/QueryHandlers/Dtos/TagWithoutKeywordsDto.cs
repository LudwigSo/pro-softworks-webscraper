using Domain;

namespace Application.QueryHandlers.Dtos;

public record TagWithoutKeywordsDto(int Id, string Name)
{
    public static TagWithoutKeywordsDto From(Tag tag)
    {
        return new TagWithoutKeywordsDto(tag.Id, tag.Name);
    }
}
