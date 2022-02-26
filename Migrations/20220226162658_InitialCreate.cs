using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TodoApi.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "mhs",
                columns: table => new
                {
                    nrp = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    nama = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    alamat = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    telepon = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mhs", x => x.nrp);
                });

            migrationBuilder.CreateTable(
                name: "nilai",
                columns: table => new
                {
                    nrp = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    matkul = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    nilai = table.Column<byte>(type: "tinyint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_nilai", x => new { x.nrp, x.matkul });
                    table.ForeignKey(
                        name: "FK_nilai_mhs_nrp",
                        column: x => x.nrp,
                        principalTable: "mhs",
                        principalColumn: "nrp",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "nilai");

            migrationBuilder.DropTable(
                name: "mhs");
        }
    }
}
