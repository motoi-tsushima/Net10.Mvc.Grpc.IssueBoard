using Api.Grpc.IssueBoard.Models;
using Google.Protobuf.WellKnownTypes;

namespace Api.Grpc.IssueBoard.Mapping;

public static class IssueMapping
{
    public static IssueDto ToDto(this Issue issue)
    {
        var dto = new IssueDto
        {
            Id = issue.Id,
            AuthorName = issue.AuthorName,
            CreatedAt = Timestamp.FromDateTime(DateTime.SpecifyKind(issue.CreatedAt, DateTimeKind.Utc)),
            Category = issue.Category ?? string.Empty,
            Title = issue.Title,
            Description = issue.Description,
            Status = issue.Status,
            Resolution = issue.Resolution ?? string.Empty,
            ResolverName = issue.ResolverName ?? string.Empty,
            HasResolvedAt = issue.ResolvedAt.HasValue,
        };

        if (issue.ResolvedAt.HasValue)
        {
            dto.ResolvedAt = Timestamp.FromDateTime(DateTime.SpecifyKind(issue.ResolvedAt.Value, DateTimeKind.Utc));
        }

        return dto;
    }

    public static Issue ToModel(this IssueDto dto)
    {
        var issue = new Issue
        {
            Id = dto.Id,
            AuthorName = dto.AuthorName,
            CreatedAt = dto.CreatedAt.ToDateTime(),
            Category = string.IsNullOrEmpty(dto.Category) ? null : dto.Category,
            Title = dto.Title,
            Description = dto.Description,
            Status = dto.Status,
            Resolution = string.IsNullOrEmpty(dto.Resolution) ? null : dto.Resolution,
            ResolverName = string.IsNullOrEmpty(dto.ResolverName) ? null : dto.ResolverName,
            ResolvedAt = dto.HasResolvedAt ? dto.ResolvedAt?.ToDateTime() : null,
        };

        return issue;
    }
}
