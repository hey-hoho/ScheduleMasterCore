using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Hos.ScheduleMaster.Core.Migrations
{
    public partial class tracessummary : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "tracestatistics",
                columns: table => new
                {
                    datenum = table.Column<int>(nullable: false),
                    datestamp = table.Column<long>(nullable: false),
                    success = table.Column<int>(nullable: false),
                    fail = table.Column<int>(nullable: false),
                    other = table.Column<int>(nullable: false),
                    lastupdatetime = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tracestatistics", x => x.datenum);
                });

            migrationBuilder.UpdateData(
                table: "systemconfigs",
                keyColumn: "key",
                keyValue: "Assembly_ImagePullPolicy",
                column: "createtime",
                value: new DateTime(2020, 6, 22, 11, 50, 44, 616, DateTimeKind.Local).AddTicks(7637));

            migrationBuilder.UpdateData(
                table: "systemconfigs",
                keyColumn: "key",
                keyValue: "DelayTask_DelayPattern",
                column: "createtime",
                value: new DateTime(2020, 6, 22, 11, 50, 44, 616, DateTimeKind.Local).AddTicks(7643));

            migrationBuilder.UpdateData(
                table: "systemconfigs",
                keyColumn: "key",
                keyValue: "DelayTask_RetrySpans",
                column: "createtime",
                value: new DateTime(2020, 6, 22, 11, 50, 44, 616, DateTimeKind.Local).AddTicks(7646));

            migrationBuilder.UpdateData(
                table: "systemconfigs",
                keyColumn: "key",
                keyValue: "DelayTask_RetryTimes",
                column: "createtime",
                value: new DateTime(2020, 6, 22, 11, 50, 44, 616, DateTimeKind.Local).AddTicks(7644));

            migrationBuilder.UpdateData(
                table: "systemconfigs",
                keyColumn: "key",
                keyValue: "Email_FromAccount",
                column: "createtime",
                value: new DateTime(2020, 6, 22, 11, 50, 44, 616, DateTimeKind.Local).AddTicks(7631));

            migrationBuilder.UpdateData(
                table: "systemconfigs",
                keyColumn: "key",
                keyValue: "Email_FromAccountPwd",
                column: "createtime",
                value: new DateTime(2020, 6, 22, 11, 50, 44, 616, DateTimeKind.Local).AddTicks(7634));

            migrationBuilder.UpdateData(
                table: "systemconfigs",
                keyColumn: "key",
                keyValue: "Email_SmtpPort",
                column: "createtime",
                value: new DateTime(2020, 6, 22, 11, 50, 44, 616, DateTimeKind.Local).AddTicks(7556));

            migrationBuilder.UpdateData(
                table: "systemconfigs",
                keyColumn: "key",
                keyValue: "Email_SmtpServer",
                column: "createtime",
                value: new DateTime(2020, 6, 22, 11, 50, 44, 616, DateTimeKind.Local).AddTicks(3140));

            migrationBuilder.UpdateData(
                table: "systemconfigs",
                keyColumn: "key",
                keyValue: "Http_RequestTimeout",
                column: "createtime",
                value: new DateTime(2020, 6, 22, 11, 50, 44, 616, DateTimeKind.Local).AddTicks(7638));

            migrationBuilder.UpdateData(
                table: "systemconfigs",
                keyColumn: "key",
                keyValue: "System_WorkerUnHealthTimes",
                column: "createtime",
                value: new DateTime(2020, 6, 22, 11, 50, 44, 616, DateTimeKind.Local).AddTicks(7641));

            migrationBuilder.UpdateData(
                table: "systemusers",
                keyColumn: "id",
                keyValue: 1,
                column: "createtime",
                value: new DateTime(2020, 6, 22, 11, 50, 44, 608, DateTimeKind.Local).AddTicks(3312));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tracestatistics");

            migrationBuilder.UpdateData(
                table: "systemconfigs",
                keyColumn: "key",
                keyValue: "Assembly_ImagePullPolicy",
                column: "createtime",
                value: new DateTime(2020, 5, 15, 10, 44, 1, 64, DateTimeKind.Local).AddTicks(6310));

            migrationBuilder.UpdateData(
                table: "systemconfigs",
                keyColumn: "key",
                keyValue: "DelayTask_DelayPattern",
                column: "createtime",
                value: new DateTime(2020, 5, 15, 10, 44, 1, 64, DateTimeKind.Local).AddTicks(6315));

            migrationBuilder.UpdateData(
                table: "systemconfigs",
                keyColumn: "key",
                keyValue: "DelayTask_RetrySpans",
                column: "createtime",
                value: new DateTime(2020, 5, 15, 10, 44, 1, 64, DateTimeKind.Local).AddTicks(6318));

            migrationBuilder.UpdateData(
                table: "systemconfigs",
                keyColumn: "key",
                keyValue: "DelayTask_RetryTimes",
                column: "createtime",
                value: new DateTime(2020, 5, 15, 10, 44, 1, 64, DateTimeKind.Local).AddTicks(6317));

            migrationBuilder.UpdateData(
                table: "systemconfigs",
                keyColumn: "key",
                keyValue: "Email_FromAccount",
                column: "createtime",
                value: new DateTime(2020, 5, 15, 10, 44, 1, 64, DateTimeKind.Local).AddTicks(6306));

            migrationBuilder.UpdateData(
                table: "systemconfigs",
                keyColumn: "key",
                keyValue: "Email_FromAccountPwd",
                column: "createtime",
                value: new DateTime(2020, 5, 15, 10, 44, 1, 64, DateTimeKind.Local).AddTicks(6309));

            migrationBuilder.UpdateData(
                table: "systemconfigs",
                keyColumn: "key",
                keyValue: "Email_SmtpPort",
                column: "createtime",
                value: new DateTime(2020, 5, 15, 10, 44, 1, 64, DateTimeKind.Local).AddTicks(6246));

            migrationBuilder.UpdateData(
                table: "systemconfigs",
                keyColumn: "key",
                keyValue: "Email_SmtpServer",
                column: "createtime",
                value: new DateTime(2020, 5, 15, 10, 44, 1, 64, DateTimeKind.Local).AddTicks(3346));

            migrationBuilder.UpdateData(
                table: "systemconfigs",
                keyColumn: "key",
                keyValue: "Http_RequestTimeout",
                column: "createtime",
                value: new DateTime(2020, 5, 15, 10, 44, 1, 64, DateTimeKind.Local).AddTicks(6312));

            migrationBuilder.UpdateData(
                table: "systemconfigs",
                keyColumn: "key",
                keyValue: "System_WorkerUnHealthTimes",
                column: "createtime",
                value: new DateTime(2020, 5, 15, 10, 44, 1, 64, DateTimeKind.Local).AddTicks(6314));

            migrationBuilder.UpdateData(
                table: "systemusers",
                keyColumn: "id",
                keyValue: 1,
                column: "createtime",
                value: new DateTime(2020, 5, 15, 10, 44, 1, 57, DateTimeKind.Local).AddTicks(9626));
        }
    }
}
