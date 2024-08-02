using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lidas.LikeApi.Database.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Likeitems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    MangaId = table.Column<Guid>(type: "uuid", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Likeitems", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Likelists",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Likelists", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LikeitemLikelist",
                columns: table => new
                {
                    LikeitemsId = table.Column<Guid>(type: "uuid", nullable: false),
                    LikelistsId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LikeitemLikelist", x => new { x.LikeitemsId, x.LikelistsId });
                    table.ForeignKey(
                        name: "FK_LikeitemLikelist_Likeitems_LikeitemsId",
                        column: x => x.LikeitemsId,
                        principalTable: "Likeitems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LikeitemLikelist_Likelists_LikelistsId",
                        column: x => x.LikelistsId,
                        principalTable: "Likelists",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LikeitemLikelist_LikelistsId",
                table: "LikeitemLikelist",
                column: "LikelistsId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LikeitemLikelist");

            migrationBuilder.DropTable(
                name: "Likeitems");

            migrationBuilder.DropTable(
                name: "Likelists");
        }
    }
}
