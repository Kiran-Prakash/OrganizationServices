using Microsoft.EntityFrameworkCore.Migrations;

namespace OrgDAL.Migrations
{
    public partial class initialCreate5 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "id",
                table: "Employee",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "Department",
                newName: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Employee",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Department",
                newName: "id");
        }
    }
}
