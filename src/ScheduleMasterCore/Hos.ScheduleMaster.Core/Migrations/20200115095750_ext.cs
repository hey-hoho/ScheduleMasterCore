using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Hos.ScheduleMaster.Core.Migrations
{
    public partial class ext : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ScheduleExecutors",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ScheduleId = table.Column<Guid>(nullable: false),
                    WorkerName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScheduleExecutors", x => x.Id);
                });

            migrationBuilder.UpdateData(
                table: "SystemConfigs",
                keyColumn: "Key",
                keyValue: "Email_FromAccount",
                columns: new[] { "CreateTime", "Group" },
                values: new object[] { new DateTime(2020, 1, 15, 17, 57, 50, 572, DateTimeKind.Local).AddTicks(6059), "邮件配置" });

            migrationBuilder.UpdateData(
                table: "SystemConfigs",
                keyColumn: "Key",
                keyValue: "Email_FromAccountPwd",
                columns: new[] { "CreateTime", "Group" },
                values: new object[] { new DateTime(2020, 1, 15, 17, 57, 50, 572, DateTimeKind.Local).AddTicks(6061), "邮件配置" });

            migrationBuilder.UpdateData(
                table: "SystemConfigs",
                keyColumn: "Key",
                keyValue: "Email_SmtpPort",
                columns: new[] { "CreateTime", "Group" },
                values: new object[] { new DateTime(2020, 1, 15, 17, 57, 50, 572, DateTimeKind.Local).AddTicks(5986), "邮件配置" });

            migrationBuilder.UpdateData(
                table: "SystemConfigs",
                keyColumn: "Key",
                keyValue: "Email_SmtpServer",
                columns: new[] { "CreateTime", "Group" },
                values: new object[] { new DateTime(2020, 1, 15, 17, 57, 50, 572, DateTimeKind.Local).AddTicks(4179), "邮件配置" });

            migrationBuilder.UpdateData(
                table: "SystemUsers",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreateTime",
                value: new DateTime(2020, 1, 15, 17, 57, 50, 562, DateTimeKind.Local).AddTicks(5886));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ScheduleExecutors");

            migrationBuilder.UpdateData(
                table: "SystemConfigs",
                keyColumn: "Key",
                keyValue: "Email_FromAccount",
                columns: new[] { "CreateTime", "Group" },
                values: new object[] { new DateTime(2019, 12, 25, 16, 44, 13, 846, DateTimeKind.Local).AddTicks(3252), "Email" });

            migrationBuilder.UpdateData(
                table: "SystemConfigs",
                keyColumn: "Key",
                keyValue: "Email_FromAccountPwd",
                columns: new[] { "CreateTime", "Group" },
                values: new object[] { new DateTime(2019, 12, 25, 16, 44, 13, 846, DateTimeKind.Local).AddTicks(3254), "Email" });

            migrationBuilder.UpdateData(
                table: "SystemConfigs",
                keyColumn: "Key",
                keyValue: "Email_SmtpPort",
                columns: new[] { "CreateTime", "Group" },
                values: new object[] { new DateTime(2019, 12, 25, 16, 44, 13, 846, DateTimeKind.Local).AddTicks(3196), "Email" });

            migrationBuilder.UpdateData(
                table: "SystemConfigs",
                keyColumn: "Key",
                keyValue: "Email_SmtpServer",
                columns: new[] { "CreateTime", "Group" },
                values: new object[] { new DateTime(2019, 12, 25, 16, 44, 13, 846, DateTimeKind.Local).AddTicks(1427), "Email" });

            migrationBuilder.UpdateData(
                table: "SystemUsers",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreateTime",
                value: new DateTime(2019, 12, 25, 16, 44, 13, 841, DateTimeKind.Local).AddTicks(1275));
        }
    }
}
