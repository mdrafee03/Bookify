using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bookify.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateUserTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(name: "fk_bookings_users_user_id", table: "bookings");

            migrationBuilder.DropForeignKey(name: "fk_reviews_users_user_id", table: "reviews");

            migrationBuilder.DropPrimaryKey(name: "pk_user", table: "user");

            migrationBuilder.RenameTable(name: "user", newName: "users");

            migrationBuilder.RenameIndex(
                name: "ix_user_email",
                table: "users",
                newName: "ix_users_email"
            );

            migrationBuilder.AddPrimaryKey(name: "pk_users", table: "users", column: "id");

            // Recreate foreign key constraints
            migrationBuilder.AddForeignKey(
                name: "fk_bookings_users_user_id",
                table: "bookings",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade
            );

            migrationBuilder.AddForeignKey(
                name: "fk_reviews_users_user_id",
                table: "reviews",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(name: "fk_bookings_users_user_id", table: "bookings");

            migrationBuilder.DropForeignKey(name: "fk_reviews_users_user_id", table: "reviews");

            migrationBuilder.DropPrimaryKey(name: "pk_users", table: "users");

            migrationBuilder.RenameTable(name: "users", newName: "user");

            migrationBuilder.RenameIndex(
                name: "ix_users_email",
                table: "user",
                newName: "ix_user_email"
            );

            migrationBuilder.AddPrimaryKey(name: "pk_user", table: "user", column: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_bookings_users_user_id",
                table: "bookings",
                column: "user_id",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade
            );

            migrationBuilder.AddForeignKey(
                name: "fk_reviews_users_user_id",
                table: "reviews",
                column: "user_id",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade
            );
        }
    }
}
