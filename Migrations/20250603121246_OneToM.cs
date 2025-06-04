using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ToDoGrpc.Migrations
{
    /// <inheritdoc />
    public partial class OneToM : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "ToDoItems");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "ToDoItems");

            migrationBuilder.RenameColumn(
                name: "ToDoStatus",
                table: "ToDoItems",
                newName: "ItemName");

            migrationBuilder.AddColumn<bool>(
                name: "IsDone",
                table: "ToDoItems",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ToDolistId",
                table: "ToDoItems",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "toDoLists",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ListName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_toDoLists", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ToDoItems_ToDolistId",
                table: "ToDoItems",
                column: "ToDolistId");

            migrationBuilder.AddForeignKey(
                name: "FK_ToDoItems_toDoLists_ToDolistId",
                table: "ToDoItems",
                column: "ToDolistId",
                principalTable: "toDoLists",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ToDoItems_toDoLists_ToDolistId",
                table: "ToDoItems");

            migrationBuilder.DropTable(
                name: "toDoLists");

            migrationBuilder.DropIndex(
                name: "IX_ToDoItems_ToDolistId",
                table: "ToDoItems");

            migrationBuilder.DropColumn(
                name: "IsDone",
                table: "ToDoItems");

            migrationBuilder.DropColumn(
                name: "ToDolistId",
                table: "ToDoItems");

            migrationBuilder.RenameColumn(
                name: "ItemName",
                table: "ToDoItems",
                newName: "ToDoStatus");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "ToDoItems",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "ToDoItems",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
