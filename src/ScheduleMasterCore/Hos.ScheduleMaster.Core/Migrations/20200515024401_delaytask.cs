using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Hos.ScheduleMaster.Core.Migrations
{
    public partial class delaytask : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "nodename",
                table: "servernodes",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(255) CHARACTER SET utf8mb4");

            migrationBuilder.CreateTable(
                name: "scheduledelayeds",
                columns: table => new
                {
                    id = table.Column<Guid>(nullable: false),
                    sourceapp = table.Column<string>(maxLength: 50, nullable: false),
                    topic = table.Column<string>(maxLength: 100, nullable: false),
                    contentkey = table.Column<string>(maxLength: 100, nullable: false),
                    delaytimespan = table.Column<int>(nullable: false),
                    delayabsolutetime = table.Column<DateTime>(nullable: false),
                    createtime = table.Column<DateTime>(nullable: false),
                    createusername = table.Column<string>(maxLength: 50, nullable: true),
                    executetime = table.Column<DateTime>(nullable: true),
                    finishtime = table.Column<DateTime>(nullable: true),
                    status = table.Column<int>(nullable: false),
                    failedretrys = table.Column<int>(nullable: false),
                    remark = table.Column<string>(maxLength: 255, nullable: true),
                    notifyurl = table.Column<string>(maxLength: 255, nullable: false),
                    notifydatatype = table.Column<string>(maxLength: 50, nullable: false),
                    notifybody = table.Column<string>(maxLength: 1000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_scheduledelayeds", x => x.id);
                });

            migrationBuilder.UpdateData(
                table: "systemconfigs",
                keyColumn: "key",
                keyValue: "Assembly_ImagePullPolicy",
                columns: new[] { "createtime", "remark" },
                values: new object[] { new DateTime(2020, 5, 15, 10, 44, 1, 64, DateTimeKind.Local).AddTicks(6310), "Always-总是拉取，IfNotPresent-本地没有时拉取，默认是Always" });

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

            migrationBuilder.InsertData(
                table: "systemconfigs",
                columns: new[] { "key", "createtime", "group", "isreuired", "name", "remark", "sort", "updatetime", "updateusername", "value" },
                values: new object[,]
                {
                    { "System_WorkerUnHealthTimes", new DateTime(2020, 5, 15, 10, 44, 1, 64, DateTimeKind.Local).AddTicks(6314), "系统配置", true, "Worker允许无响应次数", "健康检查失败达到最大次数会被下线剔除，默认值是3", 1, null, null, "3" },
                    { "DelayTask_DelayPattern", new DateTime(2020, 5, 15, 10, 44, 1, 64, DateTimeKind.Local).AddTicks(6315), "延时任务配置", true, "延迟模式", "Relative-相对时间，Absolute-绝对时间，默认值是Relative", 1, null, null, "Relative" },
                    { "DelayTask_RetryTimes", new DateTime(2020, 5, 15, 10, 44, 1, 64, DateTimeKind.Local).AddTicks(6317), "延时任务配置", true, "回调失败重试次数", "回调失败重试次数，默认值是3", 2, null, null, "3" },
                    { "DelayTask_RetrySpans", new DateTime(2020, 5, 15, 10, 44, 1, 64, DateTimeKind.Local).AddTicks(6318), "延时任务配置", true, "回调失败重试间隔", "回调失败重试间隔时间(s)，会随着重试次数递增，默认值是10秒", 3, null, null, "10" }
                });

            migrationBuilder.UpdateData(
                table: "systemusers",
                keyColumn: "id",
                keyValue: 1,
                column: "createtime",
                value: new DateTime(2020, 5, 15, 10, 44, 1, 57, DateTimeKind.Local).AddTicks(9626));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "scheduledelayeds");

            migrationBuilder.DeleteData(
                table: "systemconfigs",
                keyColumn: "key",
                keyValue: "DelayTask_DelayPattern");

            migrationBuilder.DeleteData(
                table: "systemconfigs",
                keyColumn: "key",
                keyValue: "DelayTask_RetrySpans");

            migrationBuilder.DeleteData(
                table: "systemconfigs",
                keyColumn: "key",
                keyValue: "DelayTask_RetryTimes");

            migrationBuilder.DeleteData(
                table: "systemconfigs",
                keyColumn: "key",
                keyValue: "System_WorkerUnHealthTimes");

            migrationBuilder.AlterColumn<string>(
                name: "nodename",
                table: "servernodes",
                type: "varchar(255) CHARACTER SET utf8mb4",
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 100);

            migrationBuilder.UpdateData(
                table: "systemconfigs",
                keyColumn: "key",
                keyValue: "Assembly_ImagePullPolicy",
                columns: new[] { "createtime", "remark" },
                values: new object[] { new DateTime(2020, 4, 11, 17, 17, 21, 363, DateTimeKind.Local).AddTicks(5900), "Always-总是拉取，IfNotPresent-本地没有时拉取" });

            migrationBuilder.UpdateData(
                table: "systemconfigs",
                keyColumn: "key",
                keyValue: "Email_FromAccount",
                column: "createtime",
                value: new DateTime(2020, 4, 11, 17, 17, 21, 363, DateTimeKind.Local).AddTicks(5896));

            migrationBuilder.UpdateData(
                table: "systemconfigs",
                keyColumn: "key",
                keyValue: "Email_FromAccountPwd",
                column: "createtime",
                value: new DateTime(2020, 4, 11, 17, 17, 21, 363, DateTimeKind.Local).AddTicks(5899));

            migrationBuilder.UpdateData(
                table: "systemconfigs",
                keyColumn: "key",
                keyValue: "Email_SmtpPort",
                column: "createtime",
                value: new DateTime(2020, 4, 11, 17, 17, 21, 363, DateTimeKind.Local).AddTicks(5841));

            migrationBuilder.UpdateData(
                table: "systemconfigs",
                keyColumn: "key",
                keyValue: "Email_SmtpServer",
                column: "createtime",
                value: new DateTime(2020, 4, 11, 17, 17, 21, 363, DateTimeKind.Local).AddTicks(4035));

            migrationBuilder.UpdateData(
                table: "systemconfigs",
                keyColumn: "key",
                keyValue: "Http_RequestTimeout",
                column: "createtime",
                value: new DateTime(2020, 4, 11, 17, 17, 21, 363, DateTimeKind.Local).AddTicks(5902));

            migrationBuilder.UpdateData(
                table: "systemusers",
                keyColumn: "id",
                keyValue: 1,
                column: "createtime",
                value: new DateTime(2020, 4, 11, 17, 17, 21, 358, DateTimeKind.Local).AddTicks(3038));
        }
    }
}
