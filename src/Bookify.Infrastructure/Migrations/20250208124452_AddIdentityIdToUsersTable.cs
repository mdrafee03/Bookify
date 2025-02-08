using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bookify.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddIdentityIdToUsersTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1️⃣ Add IdentityId column as NULLABLE first
            migrationBuilder.AddColumn<string>(
                name: "identity_id",
                table: "users",
                type: "text",
                nullable: true
            );

            // 2️⃣ Backfill existing users with unique values
            migrationBuilder.Sql(
                @"
            UPDATE users
SET identity_id = md5(random()::text || clock_timestamp()::text)
WHERE identity_id IS NULL; 
            "
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "identity_id", table: "users");
        }
    }
}
