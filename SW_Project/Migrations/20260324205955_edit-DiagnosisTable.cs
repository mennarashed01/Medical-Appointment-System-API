using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SW_Project.Migrations
{
    /// <inheritdoc />
    public partial class editDiagnosisTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Notes",
                table: "Diagnoses",
                newName: "DoctorNotes");

            migrationBuilder.AlterColumn<string>(
                name: "Prescription",
                table: "Diagnoses",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BloodTestResults",
                table: "Diagnoses",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Diagnoses",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Version",
                table: "Diagnoses",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BloodTestResults",
                table: "Diagnoses");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Diagnoses");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "Diagnoses");

            migrationBuilder.RenameColumn(
                name: "DoctorNotes",
                table: "Diagnoses",
                newName: "Notes");

            migrationBuilder.AlterColumn<string>(
                name: "Prescription",
                table: "Diagnoses",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }
    }
}
