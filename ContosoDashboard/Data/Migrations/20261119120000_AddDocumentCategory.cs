using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ContosoDashboard.Data.Migrations;

[DbContext(typeof(ApplicationDbContext))]
[Migration("20261119120000_AddDocumentCategory")]
public sealed class AddDocumentCategory : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            name: "Category", table: "Documents", type: "nvarchar(50)", maxLength: 50, nullable: true);
        migrationBuilder.Sql("UPDATE [Documents] SET [Category] = N'Other' WHERE [Category] IS NULL;");
        migrationBuilder.AlterColumn<string>(
            name: "Category", table: "Documents", type: "nvarchar(50)", maxLength: 50, nullable: false,
            oldClrType: typeof(string), oldType: "nvarchar(50)", oldMaxLength: 50, oldNullable: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(name: "Category", table: "Documents");
    }
}
