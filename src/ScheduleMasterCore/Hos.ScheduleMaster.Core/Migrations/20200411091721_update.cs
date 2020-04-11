using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Hos.ScheduleMaster.Core.Migrations
{
    public partial class update : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "username",
                table: "systemusers",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext CHARACTER SET utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "realname",
                table: "systemusers",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext CHARACTER SET utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "password",
                table: "systemusers",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext CHARACTER SET utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "node",
                table: "systemlogs",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext CHARACTER SET utf8mb4",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "updateusername",
                table: "systemconfigs",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext CHARACTER SET utf8mb4",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "nodetype",
                table: "servernodes",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext CHARACTER SET utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "machinename",
                table: "servernodes",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext CHARACTER SET utf8mb4",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "host",
                table: "servernodes",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext CHARACTER SET utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "accesssecret",
                table: "servernodes",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext CHARACTER SET utf8mb4",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "accessprotocol",
                table: "servernodes",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext CHARACTER SET utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "node",
                table: "scheduletraces",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext CHARACTER SET utf8mb4",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "title",
                table: "schedules",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(50) CHARACTER SET utf8mb4",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "lockednode",
                table: "schedulelocks",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext CHARACTER SET utf8mb4",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "workername",
                table: "scheduleexecutors",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext CHARACTER SET utf8mb4",
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "systemconfigs",
                keyColumn: "key",
                keyValue: "Email_FromAccount",
                columns: new[] { "createtime", "remark" },
                values: new object[] { new DateTime(2020, 4, 11, 17, 17, 21, 363, DateTimeKind.Local).AddTicks(5896), "邮箱账号" });

            migrationBuilder.UpdateData(
                table: "systemconfigs",
                keyColumn: "key",
                keyValue: "Email_FromAccountPwd",
                columns: new[] { "createtime", "remark" },
                values: new object[] { new DateTime(2020, 4, 11, 17, 17, 21, 363, DateTimeKind.Local).AddTicks(5899), "登录密码或授权码等" });

            migrationBuilder.UpdateData(
                table: "systemconfigs",
                keyColumn: "key",
                keyValue: "Email_SmtpPort",
                columns: new[] { "createtime", "remark" },
                values: new object[] { new DateTime(2020, 4, 11, 17, 17, 21, 363, DateTimeKind.Local).AddTicks(5841), "smtp端口号" });

            migrationBuilder.UpdateData(
                table: "systemconfigs",
                keyColumn: "key",
                keyValue: "Email_SmtpServer",
                columns: new[] { "createtime", "remark" },
                values: new object[] { new DateTime(2020, 4, 11, 17, 17, 21, 363, DateTimeKind.Local).AddTicks(4035), "smtp服务器地址" });

            migrationBuilder.InsertData(
                table: "systemconfigs",
                columns: new[] { "key", "createtime", "group", "isreuired", "name", "remark", "sort", "updatetime", "updateusername", "value" },
                values: new object[,]
                {
                    { "Assembly_ImagePullPolicy", new DateTime(2020, 4, 11, 17, 17, 21, 363, DateTimeKind.Local).AddTicks(5900), "程序集配置", true, "文件包拉取策略", "Always-总是拉取，IfNotPresent-本地没有时拉取", 1, null, null, "Always" },
                    { "Http_RequestTimeout", new DateTime(2020, 4, 11, 17, 17, 21, 363, DateTimeKind.Local).AddTicks(5902), "HTTP配置", true, "请求超时时间", "单位是秒，默认值是10", 1, null, null, "10" }
                });

            migrationBuilder.UpdateData(
                table: "systemusers",
                keyColumn: "id",
                keyValue: 1,
                column: "createtime",
                value: new DateTime(2020, 4, 11, 17, 17, 21, 358, DateTimeKind.Local).AddTicks(3038));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "systemconfigs",
                keyColumn: "key",
                keyValue: "Assembly_ImagePullPolicy");

            migrationBuilder.DeleteData(
                table: "systemconfigs",
                keyColumn: "key",
                keyValue: "Http_RequestTimeout");

            migrationBuilder.AlterColumn<string>(
                name: "username",
                table: "systemusers",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "realname",
                table: "systemusers",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "password",
                table: "systemusers",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "node",
                table: "systemlogs",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "updateusername",
                table: "systemconfigs",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "nodetype",
                table: "servernodes",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<string>(
                name: "machinename",
                table: "servernodes",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "host",
                table: "servernodes",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "accesssecret",
                table: "servernodes",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "accessprotocol",
                table: "servernodes",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<string>(
                name: "node",
                table: "scheduletraces",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "title",
                table: "schedules",
                type: "varchar(50) CHARACTER SET utf8mb4",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "lockednode",
                table: "schedulelocks",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "workername",
                table: "scheduleexecutors",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "systemconfigs",
                keyColumn: "key",
                keyValue: "Email_FromAccount",
                columns: new[] { "createtime", "remark" },
                values: new object[] { new DateTime(2020, 4, 5, 15, 38, 14, 583, DateTimeKind.Local).AddTicks(606), "seed by efcore auto migration" });

            migrationBuilder.UpdateData(
                table: "systemconfigs",
                keyColumn: "key",
                keyValue: "Email_FromAccountPwd",
                columns: new[] { "createtime", "remark" },
                values: new object[] { new DateTime(2020, 4, 5, 15, 38, 14, 583, DateTimeKind.Local).AddTicks(608), "seed by efcore auto migration" });

            migrationBuilder.UpdateData(
                table: "systemconfigs",
                keyColumn: "key",
                keyValue: "Email_SmtpPort",
                columns: new[] { "createtime", "remark" },
                values: new object[] { new DateTime(2020, 4, 5, 15, 38, 14, 583, DateTimeKind.Local).AddTicks(535), "seed by efcore auto migration" });

            migrationBuilder.UpdateData(
                table: "systemconfigs",
                keyColumn: "key",
                keyValue: "Email_SmtpServer",
                columns: new[] { "createtime", "remark" },
                values: new object[] { new DateTime(2020, 4, 5, 15, 38, 14, 582, DateTimeKind.Local).AddTicks(8634), "seed by efcore auto migration" });

            migrationBuilder.UpdateData(
                table: "systemusers",
                keyColumn: "id",
                keyValue: 1,
                column: "createtime",
                value: new DateTime(2020, 4, 5, 15, 38, 14, 577, DateTimeKind.Local).AddTicks(8843));
        }
    }
}
