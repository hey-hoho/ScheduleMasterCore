using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Hos.ScheduleMaster.Core.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "scheduledelayeds",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    sourceapp = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    topic = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    contentkey = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    delaytimespan = table.Column<int>(type: "int", nullable: false),
                    delayabsolutetime = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    createtime = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    createusername = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    executetime = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    finishtime = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    status = table.Column<int>(type: "int", nullable: false),
                    failedretrys = table.Column<int>(type: "int", nullable: false),
                    remark = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    notifyurl = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    notifydatatype = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    notifybody = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_scheduledelayeds", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "scheduleexecutors",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    scheduleid = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    workername = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_scheduleexecutors", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "schedulehttpoptions",
                columns: table => new
                {
                    scheduleid = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    requesturl = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    method = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    contenttype = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    headers = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    body = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_schedulehttpoptions", x => x.scheduleid);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "schedulekeepers",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    scheduleid = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    userid = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_schedulekeepers", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "schedulelocks",
                columns: table => new
                {
                    scheduleid = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    status = table.Column<int>(type: "int", nullable: false),
                    lockedtime = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    lockednode = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_schedulelocks", x => x.scheduleid);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "schedulereferences",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    scheduleid = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    childid = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_schedulereferences", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "schedules",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    title = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    metatype = table.Column<int>(type: "int", nullable: false),
                    remark = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    runloop = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    cronexpression = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    assemblyname = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    classname = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    customparamsjson = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    status = table.Column<int>(type: "int", nullable: false),
                    startdate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    enddate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    createtime = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    createuserid = table.Column<int>(type: "int", nullable: false),
                    createusername = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    lastruntime = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    nextruntime = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    totalruncount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_schedules", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "scheduletraces",
                columns: table => new
                {
                    traceid = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    scheduleid = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    node = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    starttime = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    endtime = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    elapsedtime = table.Column<double>(type: "double", nullable: false),
                    result = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_scheduletraces", x => x.traceid);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "servernodes",
                columns: table => new
                {
                    nodename = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    nodetype = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    machinename = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    accessprotocol = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    host = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    accesssecret = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    lastupdatetime = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    status = table.Column<int>(type: "int", nullable: false),
                    priority = table.Column<int>(type: "int", nullable: false),
                    maxconcurrency = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_servernodes", x => x.nodename);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "systemconfigs",
                columns: table => new
                {
                    key = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    group = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    value = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    sort = table.Column<int>(type: "int", nullable: false),
                    isreuired = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    remark = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    createtime = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    updatetime = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    updateusername = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_systemconfigs", x => x.key);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "systemlogs",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    category = table.Column<int>(type: "int", nullable: false),
                    message = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    stacktrace = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    scheduleid = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    node = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    traceid = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    createtime = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_systemlogs", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "systemusers",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    username = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    password = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    realname = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    phone = table.Column<string>(type: "varchar(15)", maxLength: 15, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    email = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    status = table.Column<int>(type: "int", nullable: false),
                    createtime = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    lastlogintime = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_systemusers", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "tracestatistics",
                columns: table => new
                {
                    datenum = table.Column<int>(type: "int", nullable: false),
                    datestamp = table.Column<long>(type: "bigint", nullable: false),
                    success = table.Column<int>(type: "int", nullable: false),
                    fail = table.Column<int>(type: "int", nullable: false),
                    other = table.Column<int>(type: "int", nullable: false),
                    lastupdatetime = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tracestatistics", x => x.datenum);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.InsertData(
                table: "systemconfigs",
                columns: new[] { "key", "createtime", "group", "isreuired", "name", "remark", "sort", "updatetime", "updateusername", "value" },
                values: new object[,]
                {
                    { "Assembly_ImagePullPolicy", new DateTime(2024, 1, 8, 9, 11, 44, 452, DateTimeKind.Local).AddTicks(6414), "程序集配置", true, "文件包拉取策略", "Always-总是拉取，IfNotPresent-本地没有时拉取，默认是Always", 1, null, null, "Always" },
                    { "DelayTask_DelayPattern", new DateTime(2024, 1, 8, 9, 11, 44, 452, DateTimeKind.Local).AddTicks(6418), "延时任务配置", true, "延迟模式", "Relative-相对时间，Absolute-绝对时间，默认值是Relative", 1, null, null, "Relative" },
                    { "DelayTask_RetrySpans", new DateTime(2024, 1, 8, 9, 11, 44, 452, DateTimeKind.Local).AddTicks(6420), "延时任务配置", true, "回调失败重试间隔", "回调失败重试间隔时间(s)，会随着重试次数递增，默认值是10秒", 3, null, null, "10" },
                    { "DelayTask_RetryTimes", new DateTime(2024, 1, 8, 9, 11, 44, 452, DateTimeKind.Local).AddTicks(6419), "延时任务配置", true, "回调失败重试次数", "回调失败重试次数，默认值是3", 2, null, null, "3" },
                    { "Email_FromAccount", new DateTime(2024, 1, 8, 9, 11, 44, 452, DateTimeKind.Local).AddTicks(6412), "邮件配置", true, "发件人账号", "邮箱账号", 3, null, null, "" },
                    { "Email_FromAccountPwd", new DateTime(2024, 1, 8, 9, 11, 44, 452, DateTimeKind.Local).AddTicks(6413), "邮件配置", true, "发件人账号密码", "登录密码或授权码等", 4, null, null, "" },
                    { "Email_SmtpPort", new DateTime(2024, 1, 8, 9, 11, 44, 452, DateTimeKind.Local).AddTicks(6411), "邮件配置", true, "邮件服务器端口", "smtp端口号", 2, null, null, "" },
                    { "Email_SmtpServer", new DateTime(2024, 1, 8, 9, 11, 44, 452, DateTimeKind.Local).AddTicks(6406), "邮件配置", true, "邮件服务器", "smtp服务器地址", 1, null, null, "" },
                    { "Http_RequestTimeout", new DateTime(2024, 1, 8, 9, 11, 44, 452, DateTimeKind.Local).AddTicks(6415), "HTTP配置", true, "请求超时时间", "单位是秒，默认值是10", 1, null, null, "10" },
                    { "System_WorkerUnHealthTimes", new DateTime(2024, 1, 8, 9, 11, 44, 452, DateTimeKind.Local).AddTicks(6416), "系统配置", true, "Worker允许无响应次数", "健康检查失败达到最大次数会被下线剔除，默认值是3", 1, null, null, "3" }
                });

            migrationBuilder.InsertData(
                table: "systemusers",
                columns: new[] { "id", "createtime", "email", "lastlogintime", "password", "phone", "realname", "status", "username" },
                values: new object[] { 1, new DateTime(2024, 1, 8, 9, 11, 44, 452, DateTimeKind.Local).AddTicks(5847), null, null, "96e79218965eb72c92a549dd5a330112", null, "admin", 1, "admin" });

            migrationBuilder.CreateIndex(
                name: "scheduledelayeds_contentkey_index",
                table: "scheduledelayeds",
                column: "contentkey");

            migrationBuilder.CreateIndex(
                name: "scheduledelayeds_createtime_index",
                table: "scheduledelayeds",
                column: "createtime");

            migrationBuilder.CreateIndex(
                name: "scheduletraces_result_index",
                table: "scheduletraces",
                column: "result");

            migrationBuilder.CreateIndex(
                name: "scheduletraces_scheduleid_index",
                table: "scheduletraces",
                column: "scheduleid");

            migrationBuilder.CreateIndex(
                name: "scheduletraces_starttime_index",
                table: "scheduletraces",
                column: "starttime");

            migrationBuilder.CreateIndex(
                name: "systemlogs_createtime_index",
                table: "systemlogs",
                column: "createtime");

            migrationBuilder.CreateIndex(
                name: "systemlogs_traceid_index",
                table: "systemlogs",
                column: "traceid");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "scheduledelayeds");

            migrationBuilder.DropTable(
                name: "scheduleexecutors");

            migrationBuilder.DropTable(
                name: "schedulehttpoptions");

            migrationBuilder.DropTable(
                name: "schedulekeepers");

            migrationBuilder.DropTable(
                name: "schedulelocks");

            migrationBuilder.DropTable(
                name: "schedulereferences");

            migrationBuilder.DropTable(
                name: "schedules");

            migrationBuilder.DropTable(
                name: "scheduletraces");

            migrationBuilder.DropTable(
                name: "servernodes");

            migrationBuilder.DropTable(
                name: "systemconfigs");

            migrationBuilder.DropTable(
                name: "systemlogs");

            migrationBuilder.DropTable(
                name: "systemusers");

            migrationBuilder.DropTable(
                name: "tracestatistics");
        }
    }
}
