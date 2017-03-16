using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace WeWorkDotnet.Web.Migrations
{
    public partial class InsertContractTypes : Migration
    {
        private string GetNewGuid() => Guid.NewGuid().ToString();

        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql($"INSERT INTO dbo.ContractTypes (Id, Name) VALUES ('{GetNewGuid()}', 'Full Time')");
            migrationBuilder.Sql($"INSERT INTO dbo.ContractTypes (Id, Name) VALUES ('{GetNewGuid()}', 'Part Time')");
            migrationBuilder.Sql($"INSERT INTO dbo.ContractTypes (Id, Name) VALUES ('{GetNewGuid()}', 'Fixed Term')");
            migrationBuilder.Sql($"INSERT INTO dbo.ContractTypes (Id, Name) VALUES ('{GetNewGuid()}', 'Freelancer')");
            migrationBuilder.Sql($"INSERT INTO dbo.ContractTypes (Id, Name) VALUES ('{GetNewGuid()}', 'Zero Hour')");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM dbo.ContractTypes");
        }
    }
}
