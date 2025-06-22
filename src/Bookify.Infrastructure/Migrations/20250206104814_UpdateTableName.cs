using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bookify.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTableName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_booking_apartment_apartment_id",
                table: "Booking"
            );

            migrationBuilder.DropForeignKey(name: "fk_booking_users_user_id", table: "Booking");

            migrationBuilder.DropForeignKey(
                name: "fk_review_apartment_apartment_id",
                table: "Review"
            );

            migrationBuilder.DropForeignKey(name: "fk_review_booking_booking_id", table: "Review");

            migrationBuilder.DropForeignKey(name: "fk_review_users_user_id", table: "Review");

            migrationBuilder.DropPrimaryKey(name: "pk_review", table: "Review");

            migrationBuilder.DropPrimaryKey(name: "pk_booking", table: "Booking");

            migrationBuilder.DropPrimaryKey(name: "pk_apartment", table: "Apartment");

            migrationBuilder.RenameTable(name: "User", newName: "user");

            migrationBuilder.RenameTable(name: "Review", newName: "reviews");

            migrationBuilder.RenameTable(name: "Booking", newName: "bookings");

            migrationBuilder.RenameTable(name: "Apartment", newName: "apartments");

            migrationBuilder.RenameIndex(
                name: "ix_review_user_id",
                table: "reviews",
                newName: "ix_reviews_user_id"
            );

            migrationBuilder.RenameIndex(
                name: "ix_review_booking_id",
                table: "reviews",
                newName: "ix_reviews_booking_id"
            );

            migrationBuilder.RenameIndex(
                name: "ix_review_apartment_id",
                table: "reviews",
                newName: "ix_reviews_apartment_id"
            );

            migrationBuilder.RenameIndex(
                name: "ix_booking_user_id",
                table: "bookings",
                newName: "ix_bookings_user_id"
            );

            migrationBuilder.RenameIndex(
                name: "ix_booking_apartment_id",
                table: "bookings",
                newName: "ix_bookings_apartment_id"
            );

            migrationBuilder.AddPrimaryKey(name: "pk_reviews", table: "reviews", column: "id");

            migrationBuilder.AddPrimaryKey(name: "pk_bookings", table: "bookings", column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_apartments",
                table: "apartments",
                column: "id"
            );

            migrationBuilder.AddForeignKey(
                name: "fk_bookings_apartments_apartment_id",
                table: "bookings",
                column: "apartment_id",
                principalTable: "apartments",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade
            );

            migrationBuilder.AddForeignKey(
                name: "fk_bookings_users_user_id",
                table: "bookings",
                column: "user_id",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade
            );

            migrationBuilder.AddForeignKey(
                name: "fk_reviews_apartments_apartment_id",
                table: "reviews",
                column: "apartment_id",
                principalTable: "apartments",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade
            );

            migrationBuilder.AddForeignKey(
                name: "fk_reviews_bookings_booking_id",
                table: "reviews",
                column: "booking_id",
                principalTable: "bookings",
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_bookings_apartments_apartment_id",
                table: "bookings"
            );

            migrationBuilder.DropForeignKey(name: "fk_bookings_users_user_id", table: "bookings");

            migrationBuilder.DropForeignKey(
                name: "fk_reviews_apartments_apartment_id",
                table: "reviews"
            );

            migrationBuilder.DropForeignKey(
                name: "fk_reviews_bookings_booking_id",
                table: "reviews"
            );

            migrationBuilder.DropForeignKey(name: "fk_reviews_users_user_id", table: "reviews");

            migrationBuilder.DropPrimaryKey(name: "pk_reviews", table: "reviews");

            migrationBuilder.DropPrimaryKey(name: "pk_bookings", table: "bookings");

            migrationBuilder.DropPrimaryKey(name: "pk_apartments", table: "apartments");

            migrationBuilder.RenameTable(name: "user", newName: "User");

            migrationBuilder.RenameTable(name: "reviews", newName: "Review");

            migrationBuilder.RenameTable(name: "bookings", newName: "Booking");

            migrationBuilder.RenameTable(name: "apartments", newName: "Apartment");

            migrationBuilder.RenameIndex(
                name: "ix_reviews_user_id",
                table: "Review",
                newName: "ix_review_user_id"
            );

            migrationBuilder.RenameIndex(
                name: "ix_reviews_booking_id",
                table: "Review",
                newName: "ix_review_booking_id"
            );

            migrationBuilder.RenameIndex(
                name: "ix_reviews_apartment_id",
                table: "Review",
                newName: "ix_review_apartment_id"
            );

            migrationBuilder.RenameIndex(
                name: "ix_bookings_user_id",
                table: "Booking",
                newName: "ix_booking_user_id"
            );

            migrationBuilder.RenameIndex(
                name: "ix_bookings_apartment_id",
                table: "Booking",
                newName: "ix_booking_apartment_id"
            );

            migrationBuilder.AddPrimaryKey(name: "pk_review", table: "Review", column: "id");

            migrationBuilder.AddPrimaryKey(name: "pk_booking", table: "Booking", column: "id");

            migrationBuilder.AddPrimaryKey(name: "pk_apartment", table: "Apartment", column: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_booking_apartment_apartment_id",
                table: "Booking",
                column: "apartment_id",
                principalTable: "Apartment",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade
            );

            migrationBuilder.AddForeignKey(
                name: "fk_booking_users_user_id",
                table: "Booking",
                column: "user_id",
                principalTable: "User",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade
            );

            migrationBuilder.AddForeignKey(
                name: "fk_review_apartment_apartment_id",
                table: "Review",
                column: "apartment_id",
                principalTable: "Apartment",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade
            );

            migrationBuilder.AddForeignKey(
                name: "fk_review_booking_booking_id",
                table: "Review",
                column: "booking_id",
                principalTable: "Booking",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade
            );

            migrationBuilder.AddForeignKey(
                name: "fk_review_users_user_id",
                table: "Review",
                column: "user_id",
                principalTable: "User",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade
            );
        }
    }
}
