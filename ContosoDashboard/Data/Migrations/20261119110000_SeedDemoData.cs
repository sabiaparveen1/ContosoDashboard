using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ContosoDashboard.Data.Migrations;

[DbContext(typeof(ApplicationDbContext))]
[Migration("20261119110000_SeedDemoData")]
public sealed class SeedDemoData : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        var seedDate = new DateTime(2026, 11, 19, 10, 24, 31, 386, DateTimeKind.Utc);

        migrationBuilder.InsertData(
            table: "Users",
            columns: ["UserId", "Email", "DisplayName", "Department", "JobTitle", "Role",
                "AvailabilityStatus", "CreatedDate", "EmailNotificationsEnabled", "InAppNotificationsEnabled"],
            columnTypes: ["int", "nvarchar(450)", "nvarchar(max)", "nvarchar(max)", "nvarchar(max)",
                "int", "int", "datetime2", "bit", "bit"],
            values: new object[,]
            {
                { 1, "admin@contoso.com", "System Administrator", "IT", "Administrator", 3, 0, seedDate, true, true },
                { 2, "camille.nicole@contoso.com", "Camille Nicole", "Engineering", "Project Manager", 2, 0, seedDate, true, true },
                { 3, "floris.kregel@contoso.com", "Floris Kregel", "Engineering", "Team Lead", 1, 0, seedDate, true, true },
                { 4, "ni.kang@contoso.com", "Ni Kang", "Engineering", "Software Engineer", 0, 0, seedDate, true, true }
            });

        migrationBuilder.InsertData(
            table: "Projects",
            columns: ["ProjectId", "Name", "Description", "ProjectManagerId", "StartDate",
                "TargetCompletionDate", "Status", "CreatedDate", "UpdatedDate"],
            columnTypes: ["int", "nvarchar(max)", "nvarchar(max)", "int", "datetime2",
                "datetime2", "int", "datetime2", "datetime2"],
            values: new object[]
            {
                1, "ContosoDashboard Development", "Internal employee productivity dashboard", 2,
                seedDate.AddDays(-30), seedDate.AddDays(60), 0, seedDate.AddDays(-30), seedDate
            });

        migrationBuilder.InsertData(
            table: "Tasks",
            columns: ["TaskId", "Title", "Description", "Priority", "Status", "DueDate",
                "AssignedUserId", "CreatedByUserId", "ProjectId", "CreatedDate", "UpdatedDate"],
            columnTypes: ["int", "nvarchar(max)", "nvarchar(max)", "int", "int", "datetime2",
                "int", "int", "int", "datetime2", "datetime2"],
            values: new object[,]
            {
                { 1, "Design database schema", "Create entity relationship diagram and database design",
                    2, 2, seedDate.AddDays(-20), 4, 2, 1, seedDate.AddDays(-30), seedDate.AddDays(-20) },
                { 2, "Implement authentication", "Set up Microsoft Entra ID authentication",
                    3, 1, seedDate.AddDays(5), 4, 2, 1, seedDate.AddDays(-25), seedDate },
                { 3, "Create UI mockups", "Design user interface mockups for all main pages",
                    1, 0, seedDate.AddDays(10), 4, 2, 1, seedDate.AddDays(-20), seedDate.AddDays(-20) }
            });

        migrationBuilder.InsertData(
            table: "ProjectMembers",
            columns: ["ProjectMemberId", "ProjectId", "UserId", "Role", "AssignedDate"],
            columnTypes: ["int", "int", "int", "nvarchar(max)", "datetime2"],
            values: new object[,]
            {
                { 1, 1, 3, "TeamLead", seedDate.AddDays(-30) },
                { 2, 1, 4, "Developer", seedDate.AddDays(-30) }
            });

        migrationBuilder.InsertData(
            table: "Announcements",
            columns: ["AnnouncementId", "Title", "Content", "CreatedByUserId", "PublishDate", "ExpiryDate", "IsActive"],
            columnTypes: ["int", "nvarchar(max)", "nvarchar(max)", "int", "datetime2", "datetime2", "bit"],
            values: new object[]
            {
                1, "Welcome to ContosoDashboard",
                "Welcome to the new ContosoDashboard application. This platform will help you manage your tasks and projects more efficiently.",
                1, seedDate, seedDate.AddDays(30), true
            });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DeleteData("Announcements", "AnnouncementId", "int", 1);
        for (var id = 1; id <= 2; id++)
            migrationBuilder.DeleteData("ProjectMembers", "ProjectMemberId", "int", id);
        for (var id = 1; id <= 3; id++)
            migrationBuilder.DeleteData("Tasks", "TaskId", "int", id);
        migrationBuilder.DeleteData("Projects", "ProjectId", "int", 1);
        for (var id = 1; id <= 4; id++)
            migrationBuilder.DeleteData("Users", "UserId", "int", id);
    }
}
