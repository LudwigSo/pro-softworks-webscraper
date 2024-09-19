using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.QueryHandlers.Dtos;

public record ProjectDto(
    int Id,
    string Title,
    string Url,
    TagWithoutKeywordsDto[] Tags,
    DateTime FirstSeenAt
)
{
    public static ProjectDto From(Project project)
    {
        return new ProjectDto(
            project.Id,
            project.Title,
            project.Url,
            project.Tags.Select(TagWithoutKeywordsDto.From).ToArray(),
            project.FirstSeenAt
        );
    }
}
