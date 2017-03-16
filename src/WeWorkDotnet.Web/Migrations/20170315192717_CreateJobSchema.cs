using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WeWorkDotnet.Web.Migrations
{
    public partial class CreateJobSchema : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Jobs",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Company = table.Column<string>(nullable: true),
                    Contact = table.Column<string>(nullable: true),
                    ContractTypeId = table.Column<Guid>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    ExternalUrl = table.Column<string>(nullable: true),
                    IsRemote = table.Column<bool>(nullable: false),
                    IsVisaSponsor = table.Column<bool>(nullable: false),
                    Location = table.Column<string>(nullable: true),
                    PostedAt = table.Column<DateTime>(nullable: false),
                    PostedByUserId = table.Column<string>(nullable: true),
                    Title = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Jobs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Jobs_ContractTypes_ContractTypeId",
                        column: x => x.ContractTypeId,
                        principalTable: "ContractTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Jobs_AspNetUsers_PostedByUserId",
                        column: x => x.PostedByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Jobs_ContractTypeId",
                table: "Jobs",
                column: "ContractTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Jobs_PostedByUserId",
                table: "Jobs",
                column: "PostedByUserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Jobs");
        }
    }
}
