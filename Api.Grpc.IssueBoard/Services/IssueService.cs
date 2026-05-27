using Api.Grpc.IssueBoard.Data;
using Api.Grpc.IssueBoard.Mapping;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;

namespace Api.Grpc.IssueBoard.Services;

public class IssueService(IssuesDbContext db, ILogger<IssueService> logger) : global::Api.Grpc.IssueBoard.IssueService.IssueServiceBase
{
    public override async Task<GetIssuesReply> GetIssues(GetIssuesRequest request, ServerCallContext context)
    {
        var issues = await db.Issues
            .OrderByDescending(i => i.CreatedAt)
            .ToListAsync(context.CancellationToken);

        var reply = new GetIssuesReply();
        reply.Issues.AddRange(issues.Select(i => i.ToDto()));
        return reply;
    }

    public override async Task<IssueDto> GetIssue(GetIssueRequest request, ServerCallContext context)
    {
        var issue = await db.Issues.FindAsync([request.Id], context.CancellationToken);
        if (issue is null)
        {
            throw new RpcException(new Status(StatusCode.NotFound, $"Issue {request.Id} not found."));
        }
        return issue.ToDto();
    }

    public override async Task<IssueDto> CreateIssue(CreateIssueRequest request, ServerCallContext context)
    {
        var issue = new Models.Issue
        {
            AuthorName = request.AuthorName,
            Title = request.Title,
            Description = request.Description,
            Category = string.IsNullOrEmpty(request.Category) ? null : request.Category,
            CreatedAt = DateTime.UtcNow,
            Status = 0,
        };

        db.Issues.Add(issue);
        await db.SaveChangesAsync(context.CancellationToken);
        logger.LogInformation("Created issue {Id}", issue.Id);
        return issue.ToDto();
    }

    public override async Task<IssueDto> UpdateIssue(UpdateIssueRequest request, ServerCallContext context)
    {
        var issue = await db.Issues.FindAsync([request.Id], context.CancellationToken);
        if (issue is null)
        {
            throw new RpcException(new Status(StatusCode.NotFound, $"Issue {request.Id} not found."));
        }

        issue.Category = string.IsNullOrEmpty(request.Category) ? null : request.Category;
        issue.Title = request.Title;
        issue.Description = request.Description;
        issue.Status = request.Status;
        issue.Resolution = string.IsNullOrEmpty(request.Resolution) ? null : request.Resolution;
        issue.ResolverName = string.IsNullOrEmpty(request.ResolverName) ? null : request.ResolverName;
        issue.ResolvedAt = DateTime.UtcNow;

        await db.SaveChangesAsync(context.CancellationToken);
        logger.LogInformation("Updated issue {Id}", issue.Id);
        return issue.ToDto();
    }

    public override async Task<DeleteIssueReply> DeleteIssue(DeleteIssueRequest request, ServerCallContext context)
    {
        var issue = await db.Issues.FindAsync([request.Id], context.CancellationToken);
        if (issue is null)
        {
            throw new RpcException(new Status(StatusCode.NotFound, $"Issue {request.Id} not found."));
        }

        db.Issues.Remove(issue);
        await db.SaveChangesAsync(context.CancellationToken);
        logger.LogInformation("Deleted issue {Id}", request.Id);
        return new DeleteIssueReply { Success = true };
    }
}
