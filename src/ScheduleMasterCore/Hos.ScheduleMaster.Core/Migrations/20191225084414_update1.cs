using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Hos.ScheduleMaster.Core.Migrations
{
    public partial class update1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RunMoreTimes",
                table: "Schedules");

            migrationBuilder.AlterColumn<string>(
                name: "AccessProtocol",
                table: "ServerNodes",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext CHARACTER SET utf8mb4",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MachineName",
                table: "ServerNodes",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NodeType",
                table: "ServerNodes",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "RunLoop",
                table: "Schedules",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "SystemConfigs",
                keyColumn: "Key",
                keyValue: "Email_FromAccount",
                column: "CreateTime",
                value: new DateTime(2019, 12, 25, 16, 44, 13, 846, DateTimeKind.Local).AddTicks(3252));

            migrationBuilder.UpdateData(
                table: "SystemConfigs",
                keyColumn: "Key",
                keyValue: "Email_FromAccountPwd",
                column: "CreateTime",
                value: new DateTime(2019, 12, 25, 16, 44, 13, 846, DateTimeKind.Local).AddTicks(3254));

            migrationBuilder.UpdateData(
                table: "SystemConfigs",
                keyColumn: "Key",
                keyValue: "Email_SmtpPort",
                column: "CreateTime",
                value: new DateTime(2019, 12, 25, 16, 44, 13, 846, DateTimeKind.Local).AddTicks(3196));

            migrationBuilder.UpdateData(
                table: "SystemConfigs",
                keyColumn: "Key",
                keyValue: "Email_SmtpServer",
                column: "CreateTime",
                value: new DateTime(2019, 12, 25, 16, 44, 13, 846, DateTimeKind.Local).AddTicks(1427));

            migrationBuilder.UpdateData(
                table: "SystemUsers",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreateTime",
                value: new DateTime(2019, 12, 25, 16, 44, 13, 841, DateTimeKind.Local).AddTicks(1275));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MachineName",
                table: "ServerNodes");

            migrationBuilder.DropColumn(
                name: "NodeType",
                table: "ServerNodes");

            migrationBuilder.DropColumn(
                name: "RunLoop",
                table: "Schedules");

            migrationBuilder.AlterColumn<string>(
                name: "AccessProtocol",
                table: "ServerNodes",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AddColumn<bool>(
                name: "RunMoreTimes",
                table: "Schedules",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

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
    }
}
