using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Hos.ScheduleMaster.Core.Migrations
{
    public partial class update : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TimeSpan",
                table: "ScheduleTraces");

            migrationBuilder.AlterColumn<Guid>(
                name: "ScheduleId",
                table: "SystemLogs",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "char(36)");

            migrationBuilder.AddColumn<string>(
                name: "Node",
                table: "SystemLogs",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AccessProtocol",
                table: "ServerNodes",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AccessSecret",
                table: "ServerNodes",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "ElapsedTime",
                table: "ScheduleTraces",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "Node",
                table: "ScheduleTraces",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "SystemConfigs",
                keyColumn: "Key",
                keyValue: "Email_FromAccount",
                column: "CreateTime",
                value: new DateTime(2019, 11, 29, 16, 42, 7, 939, DateTimeKind.Local).AddTicks(3558));

            migrationBuilder.UpdateData(
                table: "SystemConfigs",
                keyColumn: "Key",
                keyValue: "Email_FromAccountPwd",
                column: "CreateTime",
                value: new DateTime(2019, 11, 29, 16, 42, 7, 939, DateTimeKind.Local).AddTicks(3560));

            migrationBuilder.UpdateData(
                table: "SystemConfigs",
                keyColumn: "Key",
                keyValue: "Email_SmtpPort",
                column: "CreateTime",
                value: new DateTime(2019, 11, 29, 16, 42, 7, 939, DateTimeKind.Local).AddTicks(3492));

            migrationBuilder.UpdateData(
                table: "SystemConfigs",
                keyColumn: "Key",
                keyValue: "Email_SmtpServer",
                column: "CreateTime",
                value: new DateTime(2019, 11, 29, 16, 42, 7, 939, DateTimeKind.Local).AddTicks(136));

            migrationBuilder.UpdateData(
                table: "SystemUsers",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreateTime",
                value: new DateTime(2019, 11, 29, 16, 42, 7, 933, DateTimeKind.Local).AddTicks(812));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Node",
                table: "SystemLogs");

            migrationBuilder.DropColumn(
                name: "AccessProtocol",
                table: "ServerNodes");

            migrationBuilder.DropColumn(
                name: "AccessSecret",
                table: "ServerNodes");

            migrationBuilder.DropColumn(
                name: "ElapsedTime",
                table: "ScheduleTraces");

            migrationBuilder.DropColumn(
                name: "Node",
                table: "ScheduleTraces");

            migrationBuilder.AlterColumn<Guid>(
                name: "ScheduleId",
                table: "SystemLogs",
                type: "char(36)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AddColumn<double>(
                name: "TimeSpan",
                table: "ScheduleTraces",
                type: "double",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.UpdateData(
                table: "SystemConfigs",
                keyColumn: "Key",
                keyValue: "Email_FromAccount",
                column: "CreateTime",
                value: new DateTime(2019, 11, 26, 16, 52, 4, 928, DateTimeKind.Local).AddTicks(8850));

            migrationBuilder.UpdateData(
                table: "SystemConfigs",
                keyColumn: "Key",
                keyValue: "Email_FromAccountPwd",
                column: "CreateTime",
                value: new DateTime(2019, 11, 26, 16, 52, 4, 928, DateTimeKind.Local).AddTicks(8852));

            migrationBuilder.UpdateData(
                table: "SystemConfigs",
                keyColumn: "Key",
                keyValue: "Email_SmtpPort",
                column: "CreateTime",
                value: new DateTime(2019, 11, 26, 16, 52, 4, 928, DateTimeKind.Local).AddTicks(8775));

            migrationBuilder.UpdateData(
                table: "SystemConfigs",
                keyColumn: "Key",
                keyValue: "Email_SmtpServer",
                column: "CreateTime",
                value: new DateTime(2019, 11, 26, 16, 52, 4, 928, DateTimeKind.Local).AddTicks(5233));

            migrationBuilder.UpdateData(
                table: "SystemUsers",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreateTime",
                value: new DateTime(2019, 11, 26, 16, 52, 4, 920, DateTimeKind.Local).AddTicks(4991));
        }
    }
}
