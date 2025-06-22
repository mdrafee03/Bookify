using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bookify.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ChangePermissionUsersRead : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_permission_user_permission_permissions_id",
                table: "permission_user"
            );

            migrationBuilder.UpdateData(
                table: "permissions",
                keyColumn: "id",
                keyValue: 1,
                column: "name",
                value: "Users:Read"
            );

            migrationBuilder.AddForeignKey(
                name: "fk_permission_user_permissions_permissions_id",
                table: "permission_user",
                column: "permissions_id",
                principalTable: "permissions",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_permission_user_permissions_permissions_id",
                table: "permission_user"
            );

            migrationBuilder.UpdateData(
                table: "permissions",
                keyColumn: "id",
                keyValue: 1,
                column: "name",
                value: "UsersRead"
            );

            migrationBuilder.AddForeignKey(
                name: "fk_permission_user_permission_permissions_id",
                table: "permission_user",
                column: "permissions_id",
                principalTable: "permissions",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade
            );
        }
    }
}
