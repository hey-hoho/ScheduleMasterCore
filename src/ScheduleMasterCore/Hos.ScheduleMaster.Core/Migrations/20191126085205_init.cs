using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Hos.ScheduleMaster.Core.Migrations
{
    public partial class init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ScheduleKeepers",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ScheduleId = table.Column<Guid>(nullable: false),
                    UserId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScheduleKeepers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ScheduleLocks",
                columns: table => new
                {
                    ScheduleId = table.Column<Guid>(nullable: false),
                    Status = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScheduleLocks", x => x.ScheduleId);
                });

            migrationBuilder.CreateTable(
                name: "ScheduleReferences",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ScheduleId = table.Column<Guid>(nullable: false),
                    ChildId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScheduleReferences", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Schedules",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Title = table.Column<string>(maxLength: 50, nullable: false),
                    Remark = table.Column<string>(maxLength: 500, nullable: true),
                    RunMoreTimes = table.Column<bool>(nullable: false),
                    CronExpression = table.Column<string>(maxLength: 50, nullable: true),
                    AssemblyName = table.Column<string>(maxLength: 200, nullable: false),
                    ClassName = table.Column<string>(maxLength: 200, nullable: false),
                    CustomParamsJson = table.Column<string>(maxLength: 2000, nullable: true),
                    Status = table.Column<int>(nullable: false),
                    StartDate = table.Column<DateTime>(nullable: true),
                    EndDate = table.Column<DateTime>(nullable: true),
                    CreateTime = table.Column<DateTime>(nullable: false),
                    CreateUserId = table.Column<int>(nullable: false),
                    CreateUserName = table.Column<string>(nullable: true),
                    LastRunTime = table.Column<DateTime>(nullable: true),
                    NextRunTime = table.Column<DateTime>(nullable: true),
                    TotalRunCount = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Schedules", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ScheduleTraces",
                columns: table => new
                {
                    TraceId = table.Column<Guid>(nullable: false),
                    ScheduleId = table.Column<Guid>(nullable: false),
                    StartTime = table.Column<DateTime>(nullable: false),
                    EndTime = table.Column<DateTime>(nullable: false),
                    TimeSpan = table.Column<double>(nullable: false),
                    Result = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScheduleTraces", x => x.TraceId);
                });

            migrationBuilder.CreateTable(
                name: "ServerNodes",
                columns: table => new
                {
                    NodeName = table.Column<string>(nullable: false),
                    Host = table.Column<string>(nullable: false),
                    LastUpdateTime = table.Column<DateTime>(nullable: true),
                    Status = table.Column<int>(nullable: false),
                    Priority = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServerNodes", x => x.NodeName);
                });

            migrationBuilder.CreateTable(
                name: "SystemConfigs",
                columns: table => new
                {
                    Key = table.Column<string>(maxLength: 50, nullable: false),
                    Group = table.Column<string>(maxLength: 50, nullable: false),
                    Name = table.Column<string>(maxLength: 100, nullable: false),
                    Value = table.Column<string>(maxLength: 1000, nullable: true),
                    Sort = table.Column<int>(nullable: false),
                    IsReuired = table.Column<bool>(nullable: false),
                    Remark = table.Column<string>(maxLength: 500, nullable: true),
                    CreateTime = table.Column<DateTime>(nullable: false),
                    UpdateTime = table.Column<DateTime>(nullable: true),
                    UpdateUserName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemConfigs", x => x.Key);
                });

            migrationBuilder.CreateTable(
                name: "SystemLogs",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Category = table.Column<int>(nullable: false),
                    Message = table.Column<string>(nullable: false),
                    StackTrace = table.Column<string>(nullable: true),
                    ScheduleId = table.Column<Guid>(nullable: false),
                    TraceId = table.Column<Guid>(nullable: true),
                    CreateTime = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SystemUsers",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    UserName = table.Column<string>(nullable: false),
                    Password = table.Column<string>(nullable: false),
                    RealName = table.Column<string>(nullable: false),
                    Phone = table.Column<string>(maxLength: 15, nullable: true),
                    Email = table.Column<string>(maxLength: 500, nullable: true),
                    Status = table.Column<int>(nullable: false),
                    CreateTime = table.Column<DateTime>(nullable: false),
                    LastLoginTime = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemUsers", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "SystemConfigs",
                columns: new[] { "Key", "CreateTime", "Group", "IsReuired", "Name", "Remark", "Sort", "UpdateTime", "UpdateUserName", "Value" },
                values: new object[,]
                {
                    { "Email_SmtpServer", new DateTime(2019, 11, 26, 16, 52, 4, 928, DateTimeKind.Local).AddTicks(5233), "Email", true, "邮件服务器", "seed by efcore auto migration", 1, null, null, "" },
                    { "Email_SmtpPort", new DateTime(2019, 11, 26, 16, 52, 4, 928, DateTimeKind.Local).AddTicks(8775), "Email", true, "邮件服务器端口", "seed by efcore auto migration", 2, null, null, "" },
                    { "Email_FromAccount", new DateTime(2019, 11, 26, 16, 52, 4, 928, DateTimeKind.Local).AddTicks(8850), "Email", true, "发件人账号", "seed by efcore auto migration", 3, null, null, "" },
                    { "Email_FromAccountPwd", new DateTime(2019, 11, 26, 16, 52, 4, 928, DateTimeKind.Local).AddTicks(8852), "Email", true, "发件人账号密码", "seed by efcore auto migration", 4, null, null, "" }
                });

            migrationBuilder.InsertData(
                table: "SystemUsers",
                columns: new[] { "Id", "CreateTime", "Email", "LastLoginTime", "Password", "Phone", "RealName", "Status", "UserName" },
                values: new object[] { 1, new DateTime(2019, 11, 26, 16, 52, 4, 920, DateTimeKind.Local).AddTicks(4991), null, null, "96e79218965eb72c92a549dd5a330112", null, "admin", 1, "admin" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ScheduleKeepers");

            migrationBuilder.DropTable(
                name: "ScheduleLocks");

            migrationBuilder.DropTable(
                name: "ScheduleReferences");

            migrationBuilder.DropTable(
                name: "Schedules");

            migrationBuilder.DropTable(
                name: "ScheduleTraces");

            migrationBuilder.DropTable(
                name: "ServerNodes");

            migrationBuilder.DropTable(
                name: "SystemConfigs");

            migrationBuilder.DropTable(
                name: "SystemLogs");

            migrationBuilder.DropTable(
                name: "SystemUsers");
        }
    }
}
