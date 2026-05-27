using Api.Grpc.IssueBoard;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Net10.Mvc.Grpc.IssueBoard.Models;
using System.Diagnostics;

namespace Net10.Mvc.Grpc.IssueBoard.Controllers;

public class IssuesController(IssueService.IssueServiceClient client) : Controller
{
    private static IssueViewModel ToViewModel(IssueDto dto) => new()
    {
        Id = dto.Id,
        AuthorName = dto.AuthorName,
        CreatedAt = dto.CreatedAt.ToDateTime().ToLocalTime(),
        Category = string.IsNullOrEmpty(dto.Category) ? null : dto.Category,
        Title = dto.Title,
        Description = dto.Description,
        Status = (IssueStatus)dto.Status,
        Resolution = string.IsNullOrEmpty(dto.Resolution) ? null : dto.Resolution,
        ResolverName = string.IsNullOrEmpty(dto.ResolverName) ? null : dto.ResolverName,
        ResolvedAt = dto.HasResolvedAt ? dto.ResolvedAt?.ToDateTime().ToLocalTime() : null,
    };

    public async Task<IActionResult> Index()
    {
        var reply = await client.GetIssuesAsync(new GetIssuesRequest());
        var viewModels = reply.Issues.Select(ToViewModel).ToList();
        return View(viewModels);
    }

    public async Task<IActionResult> Details(int id)
    {
        var dto = await client.GetIssueAsync(new GetIssueRequest { Id = id });
        return View(ToViewModel(dto));
    }

    public IActionResult Create()
    {
        return View(new IssueCreateViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(IssueCreateViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        await client.CreateIssueAsync(new CreateIssueRequest
        {
            AuthorName = model.AuthorName,
            Title = model.Title,
            Description = model.Description,
            Category = model.Category ?? string.Empty,
        });

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var dto = await client.GetIssueAsync(new GetIssueRequest { Id = id });
        var vm = ToViewModel(dto);

        var editModel = new IssueEditViewModel
        {
            Id = vm.Id,
            AuthorName = vm.AuthorName,
            CreatedAt = vm.CreatedAt,
            Category = vm.Category,
            Title = vm.Title,
            Description = vm.Description,
            Status = vm.Status,
            Resolution = vm.Resolution,
            ResolverName = vm.ResolverName,
            ResolvedAt = vm.ResolvedAt,
        };

        ViewBag.StatusList = GetStatusSelectList(editModel.Status);
        return View(editModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, IssueEditViewModel model)
    {
        if (id != model.Id)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            ViewBag.StatusList = GetStatusSelectList(model.Status);
            return View(model);
        }

        await client.UpdateIssueAsync(new UpdateIssueRequest
        {
            Id = model.Id,
            Category = model.Category ?? string.Empty,
            Title = model.Title,
            Description = model.Description,
            Status = (int)model.Status,
            Resolution = model.Resolution ?? string.Empty,
            ResolverName = model.ResolverName ?? string.Empty,
        });

        return RedirectToAction(nameof(Details), new { id = model.Id });
    }

    public async Task<IActionResult> Delete(int id)
    {
        var dto = await client.GetIssueAsync(new GetIssueRequest { Id = id });
        return View(ToViewModel(dto));
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        await client.DeleteIssueAsync(new DeleteIssueRequest { Id = id });
        return RedirectToAction(nameof(Index));
    }

    private static SelectList GetStatusSelectList(IssueStatus selected)
    {
        var items = Enum.GetValues<IssueStatus>()
            .Select(s => new SelectListItem(s.ToString(), ((int)s).ToString()))
            .ToList();
        return new SelectList(items, "Value", "Text", ((int)selected).ToString());
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        var requestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
        ViewData["RequestId"] = requestId;
        return View();
    }
}
