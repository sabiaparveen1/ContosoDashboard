namespace ContosoDashboard.Models;

public static class DocumentCategories
{
    public const string Other = "Other";

    public static IReadOnlyList<string> All { get; } = Array.AsReadOnly(new[]
    {
        "Project Documents", "Team Resources", "Personal Files", "Reports", "Presentations", Other
    });
}
