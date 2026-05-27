using System.ComponentModel.DataAnnotations;

namespace Net10.Mvc.Grpc.IssueBoard.Models;

public enum IssueStatus
{
    未着手 = 0,
    着手中 = 1,
    解決失敗 = 2,
    課題確認不能 = 3,
    解決済み = 4,
}

public class IssueViewModel
{
    public int Id { get; set; }
    public string AuthorName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public string? Category { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public IssueStatus Status { get; set; }
    public string? Resolution { get; set; }
    public string? ResolverName { get; set; }
    public DateTime? ResolvedAt { get; set; }

    public string StatusDisplayName => Status switch
    {
        IssueStatus.未着手 => "未着手",
        IssueStatus.着手中 => "着手中",
        IssueStatus.解決失敗 => "解決失敗",
        IssueStatus.課題確認不能 => "課題確認不能",
        IssueStatus.解決済み => "解決済み",
        _ => Status.ToString(),
    };
}

public class IssueCreateViewModel
{
    [Required(ErrorMessage = "記入者氏名は必須です。")]
    [MaxLength(50, ErrorMessage = "記入者氏名は50文字以内で入力してください。")]
    [Display(Name = "記入者氏名")]
    public string AuthorName { get; set; } = string.Empty;

    [MaxLength(30, ErrorMessage = "カテゴリ名は30文字以内で入力してください。")]
    [Display(Name = "カテゴリ名")]
    public string? Category { get; set; }

    [Required(ErrorMessage = "課題タイトルは必須です。")]
    [MaxLength(100, ErrorMessage = "課題タイトルは100文字以内で入力してください。")]
    [Display(Name = "課題タイトル")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "課題の文面は必須です。")]
    [MaxLength(2000, ErrorMessage = "課題の文面は2000文字以内で入力してください。")]
    [Display(Name = "課題の文面")]
    public string Description { get; set; } = string.Empty;
}

public class IssueEditViewModel
{
    public int Id { get; set; }

    [Display(Name = "記入者氏名")]
    public string AuthorName { get; set; } = string.Empty;

    [Display(Name = "登録日時")]
    public DateTime CreatedAt { get; set; }

    [MaxLength(30, ErrorMessage = "カテゴリ名は30文字以内で入力してください。")]
    [Display(Name = "カテゴリ名")]
    public string? Category { get; set; }

    [Required(ErrorMessage = "課題タイトルは必須です。")]
    [MaxLength(100, ErrorMessage = "課題タイトルは100文字以内で入力してください。")]
    [Display(Name = "課題タイトル")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "課題の文面は必須です。")]
    [MaxLength(2000, ErrorMessage = "課題の文面は2000文字以内で入力してください。")]
    [Display(Name = "課題の文面")]
    public string Description { get; set; } = string.Empty;

    [Required(ErrorMessage = "現在の状況は必須です。")]
    [Display(Name = "現在の状況")]
    public IssueStatus Status { get; set; }

    [MaxLength(2000, ErrorMessage = "解決の文面は2000文字以内で入力してください。")]
    [Display(Name = "解決の文面")]
    public string? Resolution { get; set; }

    [MaxLength(50, ErrorMessage = "解決担当者名は50文字以内で入力してください。")]
    [Display(Name = "解決担当者名")]
    public string? ResolverName { get; set; }

    [Display(Name = "最終結果日時")]
    public DateTime? ResolvedAt { get; set; }
}
