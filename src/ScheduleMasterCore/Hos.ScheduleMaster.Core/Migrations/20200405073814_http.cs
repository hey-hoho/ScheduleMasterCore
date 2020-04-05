using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Hos.ScheduleMaster.Core.Migrations
{
    public partial class http : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "scheduleexecutors",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    scheduleid = table.Column<Guid>(nullable: false),
                    workername = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_scheduleexecutors", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "schedulehttpoptions",
                columns: table => new
                {
                    scheduleid = table.Column<Guid>(nullable: false),
                    requesturl = table.Column<string>(maxLength: 500, nullable: false),
                    method = table.Column<string>(maxLength: 10, nullable: false),
                    contenttype = table.Column<string>(maxLength: 50, nullable: false),
                    headers = table.Column<string>(nullable: true),
                    body = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_schedulehttpoptions", x => x.scheduleid);
                });

            migrationBuilder.CreateTable(
                name: "schedulekeepers",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    scheduleid = table.Column<Guid>(nullable: false),
                    userid = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_schedulekeepers", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "schedulelocks",
                columns: table => new
                {
                    scheduleid = table.Column<Guid>(nullable: false),
                    status = table.Column<int>(nullable: false),
                    lockedtime = table.Column<DateTime>(nullable: true),
                    lockednode = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_schedulelocks", x => x.scheduleid);
                });

            migrationBuilder.CreateTable(
                name: "schedulereferences",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    scheduleid = table.Column<Guid>(nullable: false),
                    childid = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_schedulereferences", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "schedules",
                columns: table => new
                {
                    id = table.Column<Guid>(nullable: false),
                    title = table.Column<string>(maxLength: 50, nullable: false),
                    metatype = table.Column<int>(nullable: false),
                    remark = table.Column<string>(maxLength: 500, nullable: true),
                    runloop = table.Column<bool>(nullable: false),
                    cronexpression = table.Column<string>(maxLength: 50, nullable: true),
                    assemblyname = table.Column<string>(maxLength: 200, nullable: true),
                    classname = table.Column<string>(maxLength: 200, nullable: true),
                    customparamsjson = table.Column<string>(nullable: true),
                    status = table.Column<int>(nullable: false),
                    startdate = table.Column<DateTime>(nullable: true),
                    enddate = table.Column<DateTime>(nullable: true),
                    createtime = table.Column<DateTime>(nullable: false),
                    createuserid = table.Column<int>(nullable: false),
                    createusername = table.Column<string>(maxLength: 50, nullable: true),
                    lastruntime = table.Column<DateTime>(nullable: true),
                    nextruntime = table.Column<DateTime>(nullable: true),
                    totalruncount = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_schedules", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "scheduletraces",
                columns: table => new
                {
                    traceid = table.Column<Guid>(nullable: false),
                    scheduleid = table.Column<Guid>(nullable: false),
                    node = table.Column<string>(nullable: true),
                    starttime = table.Column<DateTime>(nullable: false),
                    endtime = table.Column<DateTime>(nullable: false),
                    elapsedtime = table.Column<double>(nullable: false),
                    result = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_scheduletraces", x => x.traceid);
                });

            migrationBuilder.CreateTable(
                name: "servernodes",
                columns: table => new
                {
                    nodename = table.Column<string>(nullable: false),
                    nodetype = table.Column<string>(nullable: false),
                    machinename = table.Column<string>(nullable: true),
                    accessprotocol = table.Column<string>(nullable: false),
                    host = table.Column<string>(nullable: false),
                    accesssecret = table.Column<string>(nullable: true),
                    lastupdatetime = table.Column<DateTime>(nullable: true),
                    status = table.Column<int>(nullable: false),
                    priority = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_servernodes", x => x.nodename);
                });

            migrationBuilder.CreateTable(
                name: "systemconfigs",
                columns: table => new
                {
                    key = table.Column<string>(maxLength: 50, nullable: false),
                    group = table.Column<string>(maxLength: 50, nullable: false),
                    name = table.Column<string>(maxLength: 100, nullable: false),
                    value = table.Column<string>(maxLength: 1000, nullable: true),
                    sort = table.Column<int>(nullable: false),
                    isreuired = table.Column<bool>(nullable: false),
                    remark = table.Column<string>(maxLength: 500, nullable: true),
                    createtime = table.Column<DateTime>(nullable: false),
                    updatetime = table.Column<DateTime>(nullable: true),
                    updateusername = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_systemconfigs", x => x.key);
                });

            migrationBuilder.CreateTable(
                name: "systemlogs",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    category = table.Column<int>(nullable: false),
                    message = table.Column<string>(nullable: false),
                    stacktrace = table.Column<string>(nullable: true),
                    scheduleid = table.Column<Guid>(nullable: true),
                    node = table.Column<string>(nullable: true),
                    traceid = table.Column<Guid>(nullable: true),
                    createtime = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_systemlogs", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "systemusers",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    username = table.Column<string>(nullable: false),
                    password = table.Column<string>(nullable: false),
                    realname = table.Column<string>(nullable: false),
                    phone = table.Column<string>(maxLength: 15, nullable: true),
                    email = table.Column<string>(maxLength: 500, nullable: true),
                    status = table.Column<int>(nullable: false),
                    createtime = table.Column<DateTime>(nullable: false),
                    lastlogintime = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_systemusers", x => x.id);
                });

            migrationBuilder.InsertData(
                table: "systemconfigs",
                columns: new[] { "key", "createtime", "group", "isreuired", "name", "remark", "sort", "updatetime", "updateusername", "value" },
                values: new object[,]
                {
                    { "Email_SmtpServer", new DateTime(2020, 4, 5, 15, 38, 14, 582, DateTimeKind.Local).AddTicks(8634), "邮件配置", true, "邮件服务器", "seed by efcore auto migration", 1, null, null, "" },
                    { "Email_SmtpPort", new DateTime(2020, 4, 5, 15, 38, 14, 583, DateTimeKind.Local).AddTicks(535), "邮件配置", true, "邮件服务器端口", "seed by efcore auto migration", 2, null, null, "" },
                    { "Email_FromAccount", new DateTime(2020, 4, 5, 15, 38, 14, 583, DateTimeKind.Local).AddTicks(606), "邮件配置", true, "发件人账号", "seed by efcore auto migration", 3, null, null, "" },
                    { "Email_FromAccountPwd", new DateTime(2020, 4, 5, 15, 38, 14, 583, DateTimeKind.Local).AddTicks(608), "邮件配置", true, "发件人账号密码", "seed by efcore auto migration", 4, null, null, "" }
                });

            migrationBuilder.InsertData(
                table: "systemusers",
                columns: new[] { "id", "createtime", "email", "lastlogintime", "password", "phone", "realname", "status", "username" },
                values: new object[] { 1, new DateTime(2020, 4, 5, 15, 38, 14, 577, DateTimeKind.Local).AddTicks(8843), null, null, "96e79218965eb72c92a549dd5a330112", null, "admin", 1, "admin" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
        }
    }
}
