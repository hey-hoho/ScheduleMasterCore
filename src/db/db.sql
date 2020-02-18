create table `__efmigrationshistory`
(
  MigrationId    varchar(95) not null
    primary key,
  ProductVersion varchar(32) not null
);

create table schedulekeepers
(
  id         int auto_increment
    primary key,
  scheduleid char(36) not null,
  userId     int      not null
);

create table scheduleexecutors
(
  id         int auto_increment
    primary key,
  scheduleid char(36)                 not null,
  workername longtext charset utf8mb4 null
);

create table schedulelocks
(
  scheduleid char(36) not null
    primary key,
  status     int      not null
);

create table schedulereferences
(
  id         int auto_increment
    primary key,
  scheduleid char(36) not null,
  childId    char(36) not null
);

create table schedules
(
  id               char(36)                      not null
    primary key,
  title            varchar(50) charset utf8mb4   not null,
  remark           varchar(500) charset utf8mb4  null,
  runloop          tinyint(1)                    not null,
  cronexpression   varchar(50) charset utf8mb4   null,
  assemblyname     varchar(200) charset utf8mb4  not null,
  classname        varchar(200) charset utf8mb4  not null,
  customparamsjson varchar(2000) charset utf8mb4 null,
  status           int                           not null,
  startdate        datetime(6)                   null,
  enddate          datetime(6)                   null,
  createtime       datetime(6)                   not null,
  createuserid     int                           not null,
  createusername   longtext charset utf8mb4      null,
  lastruntime      datetime(6)                   null,
  nextruntime      datetime(6)                   null,
  totalruncount    int                           not null
);


create table scheduletraces
(
  traceid     char(36)                 not null
    primary key,
  scheduleid  char(36)                 not null,
  node        longtext charset utf8mb4 null,
  starttime   datetime(6)              not null,
  endtime     datetime(6)              not null,
  elapsedtime double                   not null,
  result      int                      not null
);

create table servernodes
(
  nodename       varchar(255) charset utf8mb4 not null
    primary key,
  nodetype       longtext charset utf8mb4     not null,
  machinename    longtext charset utf8mb4     null,
  accessprotocol longtext charset utf8mb4     not null,
  host           longtext charset utf8mb4     not null,
  accesssecret   longtext charset utf8mb4     null,
  lastupdatetime datetime(6)                  null,
  status         int                          not null,
  priority       int                          not null
);


create table systemconfigs
(
  `key`          varchar(50) charset utf8mb4   not null
    primary key,
  `group`        varchar(50) charset utf8mb4   not null,
  name           varchar(100) charset utf8mb4  not null,
  value          varchar(1000) charset utf8mb4 null,
  sort           int                           not null,
  isreuired      tinyint(1)                    not null,
  remark         varchar(500) charset utf8mb4  null,
  createtime     datetime(6)                   not null,
  updatetime     datetime(6)                   null,
  updateusername longtext charset utf8mb4      null
);

create table systemlogs
(
  id         int auto_increment
    primary key,
  category   int                      not null,
  message    longtext charset utf8mb4 not null,
  stacktrace longtext charset utf8mb4 null,
  scheduleid char(36)                 null,
  node       longtext charset utf8mb4 null,
  traceid    char(36)                 null,
  createtime datetime(6)              not null
);

create table systemusers
(
  id            int auto_increment
    primary key,
  username      longtext charset utf8mb4     not null,
  password      longtext charset utf8mb4     not null,
  realname      longtext charset utf8mb4     not null,
  phone         varchar(15) charset utf8mb4  null,
  email         varchar(500) charset utf8mb4 null,
  status        int                          not null,
  createtime    datetime(6)                  not null,
  lastlogintime datetime(6)                  null
);

insert into schedule_master.systemusers (id, username, password, realname, phone, email, status, createtime, lastlogintime) values (1, 'admin', '96e79218965eb72c92a549dd5a330112', 'admin', null, null, 1, '2019-12-25 16:45:24.538263', '2020-01-06 09:47:26.077974');
insert into schedule_master.systemconfigs (`key`, `group`, name, value, sort, isreuired, remark, createtime, updatetime, updateusername) values ('Email_FromAccount', '邮件配置', '发件人账号', '', 3, 1, 'seed by efcore auto migration', '2019-12-25 16:45:24.543845', '2019-12-27 16:18:30.085219', null);
insert into schedule_master.systemconfigs (`key`, `group`, name, value, sort, isreuired, remark, createtime, updatetime, updateusername) values ('Email_FromAccountPwd', '邮件配置', '发件人账号密码', 'h', 4, 1, 'seed by efcore auto migration', '2019-12-25 16:45:24.543845', '2019-12-27 16:18:30.085779', null);
insert into schedule_master.systemconfigs (`key`, `group`, name, value, sort, isreuired, remark, createtime, updatetime, updateusername) values ('Email_SmtpPort', '邮件配置', '邮件服务器端口', '', 2, 1, 'seed by efcore auto migration', '2019-12-25 16:45:24.543839', '2019-12-27 16:18:30.084619', null);
insert into schedule_master.systemconfigs (`key`, `group`, name, value, sort, isreuired, remark, createtime, updatetime, updateusername) values ('Email_SmtpServer', '邮件配置', '邮件服务器', '', 1, 1, 'seed by efcore auto migration', '2019-12-25 16:45:24.543653', '2019-12-27 16:18:30.080192', null);

