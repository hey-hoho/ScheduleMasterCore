create table `__efmigrationshistory`
(
  MigrationId    varchar(95) not null
    primary key,
  ProductVersion varchar(32) not null
);

create table schedulekeepers
(
  Id         int auto_increment
    primary key,
  ScheduleId char(36) not null,
  UserId     int      not null
);

create table schedulelocks
(
  ScheduleId char(36) not null
    primary key,
  Status     int      not null
);

create table schedulereferences
(
  Id         int auto_increment
    primary key,
  ScheduleId char(36) not null,
  ChildId    char(36) not null
);

create table schedules
(
  Id               char(36)                      not null
    primary key,
  Title            varchar(50) charset utf8mb4   not null,
  Remark           varchar(500) charset utf8mb4  null,
  RunMoreTimes     tinyint(1)                    not null,
  CronExpression   varchar(50) charset utf8mb4   null,
  AssemblyName     varchar(200) charset utf8mb4  not null,
  ClassName        varchar(200) charset utf8mb4  not null,
  CustomParamsJson varchar(2000) charset utf8mb4 null,
  Status           int                           not null,
  StartDate        datetime(6)                   null,
  EndDate          datetime(6)                   null,
  CreateTime       datetime(6)                   not null,
  CreateUserId     int                           not null,
  CreateUserName   longtext charset utf8mb4      null,
  LastRunTime      datetime(6)                   null,
  NextRunTime      datetime(6)                   null,
  TotalRunCount    int                           not null
);

create table scheduletraces
(
  TraceId     char(36)                 not null
    primary key,
  ScheduleId  char(36)                 not null,
  StartTime   datetime(6)              not null,
  EndTime     datetime(6)              not null,
  Result      int                      not null,
  ElapsedTime double default '0'       not null,
  Node        longtext charset utf8mb4 null
);

create table servernodes
(
  NodeName       varchar(255) charset utf8mb4 not null
    primary key,
  Host           longtext charset utf8mb4     not null,
  LastUpdateTime datetime(6)                  null,
  Status         int                          not null,
  Priority       int                          not null,
  AccessProtocol longtext charset utf8mb4     null,
  AccessSecret   longtext charset utf8mb4     null
);

create table systemconfigs
(
  `Key`          varchar(50) charset utf8mb4   not null
    primary key,
  `Group`        varchar(50) charset utf8mb4   not null,
  Name           varchar(100) charset utf8mb4  not null,
  Value          varchar(1000) charset utf8mb4 null,
  Sort           int                           not null,
  IsReuired      tinyint(1)                    not null,
  Remark         varchar(500) charset utf8mb4  null,
  CreateTime     datetime(6)                   not null,
  UpdateTime     datetime(6)                   null,
  UpdateUserName longtext charset utf8mb4      null
);

create table systemlogs
(
  Id         int auto_increment
    primary key,
  Category   int                      not null,
  Message    longtext charset utf8mb4 not null,
  StackTrace longtext charset utf8mb4 null,
  ScheduleId char(36)                 null,
  TraceId    char(36)                 null,
  CreateTime datetime(6)              not null,
  Node       longtext charset utf8mb4 null
);

create table systemusers
(
  Id            int auto_increment
    primary key,
  UserName      longtext charset utf8mb4     not null,
  Password      longtext charset utf8mb4     not null,
  RealName      longtext charset utf8mb4     not null,
  Phone         varchar(15) charset utf8mb4  null,
  Email         varchar(500) charset utf8mb4 null,
  Status        int                          not null,
  CreateTime    datetime(6)                  not null,
  LastLoginTime datetime(6)                  null
);

INSERT INTO schedule_master.systemusers (Id, UserName, Password, RealName, Phone, Email, Status, CreateTime, LastLoginTime) VALUES (1, 'admin', '96e79218965eb72c92a549dd5a330112', 'admin', null, null, 1, '2019-12-25 16:45:24.538263', '2020-01-06 09:47:26.077974');
INSERT INTO schedule_master.systemconfigs (`Key`, `Group`, Name, Value, Sort, IsReuired, Remark, CreateTime, UpdateTime, UpdateUserName) VALUES ('Email_FromAccount', '邮件配置', '发件人账号', '', 3, 1, 'seed by efcore auto migration', '2019-12-25 16:45:24.543845', '2019-12-27 16:18:30.085219', null);
INSERT INTO schedule_master.systemconfigs (`Key`, `Group`, Name, Value, Sort, IsReuired, Remark, CreateTime, UpdateTime, UpdateUserName) VALUES ('Email_FromAccountPwd', '邮件配置', '发件人账号密码', 'h', 4, 1, 'seed by efcore auto migration', '2019-12-25 16:45:24.543845', '2019-12-27 16:18:30.085779', null);
INSERT INTO schedule_master.systemconfigs (`Key`, `Group`, Name, Value, Sort, IsReuired, Remark, CreateTime, UpdateTime, UpdateUserName) VALUES ('Email_SmtpPort', '邮件配置', '邮件服务器端口', '', 2, 1, 'seed by efcore auto migration', '2019-12-25 16:45:24.543839', '2019-12-27 16:18:30.084619', null);
INSERT INTO schedule_master.systemconfigs (`Key`, `Group`, Name, Value, Sort, IsReuired, Remark, CreateTime, UpdateTime, UpdateUserName) VALUES ('Email_SmtpServer', '邮件配置', '邮件服务器', '', 1, 1, 'seed by efcore auto migration', '2019-12-25 16:45:24.543653', '2019-12-27 16:18:30.080192', null);

