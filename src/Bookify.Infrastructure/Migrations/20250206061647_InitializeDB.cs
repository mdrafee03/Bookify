using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bookify.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitializeDB : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Apartment",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(
                        type: "character varying(200)",
                        maxLength: 200,
                        nullable: false
                    ),
                    description = table.Column<string>(
                        type: "character varying(2000)",
                        maxLength: 2000,
                        nullable: false
                    ),
                    address_country = table.Column<string>(type: "text", nullable: false),
                    address_state = table.Column<string>(type: "text", nullable: false),
                    address_city = table.Column<string>(type: "text", nullable: false),
                    address_zip_code = table.Column<string>(type: "text", nullable: false),
                    address_street = table.Column<string>(type: "text", nullable: false),
                    price_amount = table.Column<decimal>(type: "numeric", nullable: false),
                    price_currency = table.Column<string>(type: "text", nullable: false),
                    cleaning_fee_amount = table.Column<decimal>(type: "numeric", nullable: false),
                    cleaning_fee_currency = table.Column<string>(type: "text", nullable: false),
                    last_booked_on_utc = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                    amenities = table.Column<int[]>(type: "integer[]", nullable: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_apartment", x => x.id);
                }
            );

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    first_name = table.Column<string>(
                        type: "character varying(200)",
                        maxLength: 200,
                        nullable: false
                    ),
                    last_name = table.Column<string>(
                        type: "character varying(200)",
                        maxLength: 200,
                        nullable: false
                    ),
                    email = table.Column<string>(
                        type: "character varying(400)",
                        maxLength: 400,
                        nullable: false
                    ),
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user", x => x.id);
                }
            );

            migrationBuilder.CreateTable(
                name: "Booking",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    apartment_id = table.Column<Guid>(type: "uuid", nullable: false),
                    duration_start = table.Column<DateOnly>(type: "date", nullable: false),
                    duration_end = table.Column<DateOnly>(type: "date", nullable: false),
                    price_for_period_amount = table.Column<decimal>(
                        type: "numeric",
                        nullable: false
                    ),
                    price_for_period_currency = table.Column<string>(type: "text", nullable: false),
                    cleaning_fee_amount = table.Column<decimal>(type: "numeric", nullable: false),
                    cleaning_fee_currency = table.Column<string>(type: "text", nullable: false),
                    amenities_up_charge_amount = table.Column<decimal>(
                        type: "numeric",
                        nullable: false
                    ),
                    amenities_up_charge_currency = table.Column<string>(
                        type: "text",
                        nullable: false
                    ),
                    total_price_amount = table.Column<decimal>(type: "numeric", nullable: false),
                    total_price_currency = table.Column<string>(type: "text", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    created_on_utc = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: false
                    ),
                    confirmed_on_utc = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                    rejected_on_utc = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                    completed_on_utc = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                    canceled_on_utc = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_booking", x => x.id);
                    table.ForeignKey(
                        name: "fk_booking_apartment_apartment_id",
                        column: x => x.apartment_id,
                        principalTable: "Apartment",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade
                    );
                    table.ForeignKey(
                        name: "fk_booking_users_user_id",
                        column: x => x.user_id,
                        principalTable: "User",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "Review",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    apartment_id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    booking_id = table.Column<Guid>(type: "uuid", nullable: false),
                    rating = table.Column<int>(type: "integer", nullable: false),
                    comment = table.Column<string>(
                        type: "character varying(200)",
                        maxLength: 200,
                        nullable: false
                    ),
                    created_on_utc = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: false
                    ),
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_review", x => x.id);
                    table.ForeignKey(
                        name: "fk_review_apartment_apartment_id",
                        column: x => x.apartment_id,
                        principalTable: "Apartment",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade
                    );
                    table.ForeignKey(
                        name: "fk_review_booking_booking_id",
                        column: x => x.booking_id,
                        principalTable: "Booking",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade
                    );
                    table.ForeignKey(
                        name: "fk_review_users_user_id",
                        column: x => x.user_id,
                        principalTable: "User",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateIndex(
                name: "ix_booking_apartment_id",
                table: "Booking",
                column: "apartment_id"
            );

            migrationBuilder.CreateIndex(
                name: "ix_booking_user_id",
                table: "Booking",
                column: "user_id"
            );

            migrationBuilder.CreateIndex(
                name: "ix_review_apartment_id",
                table: "Review",
                column: "apartment_id"
            );

            migrationBuilder.CreateIndex(
                name: "ix_review_booking_id",
                table: "Review",
                column: "booking_id"
            );

            migrationBuilder.CreateIndex(
                name: "ix_review_user_id",
                table: "Review",
                column: "user_id"
            );

            migrationBuilder.CreateIndex(
                name: "ix_user_email",
                table: "User",
                column: "email",
                unique: true
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "Review");

            migrationBuilder.DropTable(name: "Booking");

            migrationBuilder.DropTable(name: "Apartment");

            migrationBuilder.DropTable(name: "User");
        }
    }
}
