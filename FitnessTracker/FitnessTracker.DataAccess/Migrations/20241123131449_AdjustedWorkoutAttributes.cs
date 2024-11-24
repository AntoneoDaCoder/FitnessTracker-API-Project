using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FitnessTracker.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AdjustedWorkoutAttributes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateOnly>(
                name: "Date",
                table: "Workouts",
                type: "date",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.CreateIndex(
                name: "IX_Workouts_Sport_Date",
                table: "Workouts",
                columns: new[] { "Sport", "Date" });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_Weight_Age_Gender",
                table: "AspNetUsers",
                columns: new[] { "Weight", "Age", "Gender" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Workouts_Sport_Date",
                table: "Workouts");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_Weight_Age_Gender",
                table: "AspNetUsers");

            migrationBuilder.AlterColumn<DateTime>(
                name: "Date",
                table: "Workouts",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateOnly),
                oldType: "date");
        }
    }
}
