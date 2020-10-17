
![schedulemaster ](https://imgkr.cn-bj.ufileos.com/3e2e493c-8813-4f4a-8b42-0a4882929ccd.png)

ScheduleMaster是一个开源的分布式任务调度系统，它基于.NET Core 3.1平台构建，支持跨平台多节点部署运行。


[![Build Status](https://dev.azure.com/591310381/ScheduleMasterCore/_apis/build/status/ScheduleMasterCore?branchName=master)](https://dev.azure.com/591310381/ScheduleMasterCore/_build/latest?definitionId=4&branchName=master)
![Nuget](https://img.shields.io/nuget/dt/ScheduleMaster)
![PowerShell Gallery](https://img.shields.io/powershellgallery/p/DNS.1.1.1.1)
![GitHub last commit](https://img.shields.io/github/last-commit/hey-hoho/ScheduleMasterCore)
![.NET Core](https://github.com/hey-hoho/ScheduleMasterCore/workflows/.NET%20Core/badge.svg)


## 主要特性
- [x] 简易的Web UI操作；
- [x] 任务动态管理:创建、启动、停止、暂停、恢复、删除等；
- [x] 高可用支持，跨平台多节点部署。
- [x] 数据安全性，不会出现多实例并发调度。
- [x] 支持自定义参数设置；
- [x] 支持.NET Core和.NET Framework（4.6.1+）；
- [x] 支持自定义配置文件和热更新；
- [x] 支持设置监护人，运行异常时邮件告警；
- [x] 支持设置任务依赖，自动触发，共享任务结果；
- [x] 插件式开发，任务运行环境隔离；
- [x] 全链路日志系统，运行轨迹轻松掌控；
- [x] 用户访问控制；
- [x] 提供开放REST API，业务系统可以无缝集成；
- [x] 调度报表统计；
- [ ] 任务分组管理；
- [ ] 计划表拆分实现复用；
- [x] 指定节点运行；
- [x] 支持http任务配置；
- [x] 支持延时任务；
- [ ] 任务监控；
- [ ] 资源监控；
- [x] 支持异常策略配置（失败重试、超时控制等）；
- [ ] 接入redis缓存；
- [x] 多数据库类型支持；
- [ ] 用户权限更加精细化；
- [ ] 报表统计完善；

<br />

## 技术栈
ASP.NET Core3.1、EntityFramework Core3.0、Mysql5.7、Quartz.Net、BeyondAdmin、Jquery...

<br />

## 系统架构图
![Architecture ](https://imgkr.cn-bj.ufileos.com/9b61a8f3-fabf-4a87-ad60-1d25bf92fc12.png)


## 如何使用

> 使用前请准备好所需环境：`Visual Studio 2019`、`.NET Core3.1 SDK`、`Mysql 5.7(可选)`、`SQLServer(可选)`、`PostgreSQL(可选)`、`Centos(可选)`、`Docker(可选)`。

下面以Mysql作为数据库，用**配置文件方式**启动为例做介绍，其他方式参考详细文档。

下载源码到本地，然后用VS2019打开解决方案并编译通过。

打开项目Hos.ScheduleMaster.Web根目录下的`appsettings.json`文件，先修改Mysql数据库连接字符串以保证数据库正常访问，再找到`NodeSetting`节点，修改`IP`字段为master将要部署的ip地址（master端口为30000不用修改），在项目上右击选择发布...，发布到本地文件夹。

打开项目Hos.ScheduleMaster.QuartzHost根目录下的`appsettings.json`文件，同样先修改Mysql连接字符串，再找到`NodeSetting`节点，设置worker的名称`IdentityName`，修改`IP`字段为将要部署的ip地址，`Port`字段为要监听的地址（推荐为30001），在项目上右击选择发布...，发布到本地文件夹。如果要新增worker，按同样方式配置`IdentityName、IP、Port`即可，worker在启动后会把自己的信息注入到数据库中，在master中可以看到。

> 快速发布小贴士：windows平台下用powershell执行脚本`publish.ps1`快速发布到`d:/sm-publish`目录，linux平台下执行脚本`sh publish.sh`快速发布到`/home/sm-publish`目录。

其他发布方式亦可。下面以运行2个worker节点为例：

### 在Windows中运行
* 找到master的发布目录，执行命令`dotnet Hos.ScheduleMaster.Web.dll`启动程序，**首次启动会自动迁移生成数据库结构并初始化种子数据**，打开浏览器输入ip和端口访问即可（初始用户名`admin`，密码`111111`）。
* 找到worker的发布目录，执行命令`dotnet Hos.ScheduleMaster.QuartzHost.dll --urls http://*:30001`启动程序，打开浏览器输入ip和端口会看到一个欢迎页面，表示worker已启动成功。
* 修改worker下的`appsettings.json`文件为worker2的配置（如果发布前已经修改可跳过），执行命令`dotnet Hos.ScheduleMaster.QuartzHost.dll --urls http://*:30002`启动程序.
* 登录到master中，可以看到**节点管理**菜单下各节点的运行状态。

### 在Linux(Centos)中运行
> 运行前请确保机器已经安装好`.NET Core3.1`运行时环境。

把发布文件复制到Centos中，操作步骤同Windows。

### 在Docker中运行
* 在master的发布目录中执行`docker build -t ms_master .`命令生成master镜像，再执行`docker run -d -p 30000:30000 --name="mymaster" ms_master`运行容器。
* 在worker的发布目录中执行`docker build -t ms_worker .`命令生成worker镜像，再执行`docker run -d -p 30001:80 --name="myworker1" ms_worker bash --identity=docker-worker1 --ip=你的宿主机IP --port=30001`运行容器启动worker1。
* 继续执行`docker run -d -p 30002:80 --name="myworker2" ms_worker bash --identity=docker-worker2 --ip=你的宿主机IP --port=30002`运行容器启动worker2。
* 执行`docker ps`查看各容器运行状态，如果运行不起来请检查容器log。

<br />

## 效果图
![ ](https://imgkr.cn-bj.ufileos.com/11abe3ce-5ffa-4275-9b34-582a0f202934.png)

![ ](https://imgkr.cn-bj.ufileos.com/c5331959-ca55-4377-9c27-9b3639a3d223.png)

![ ](https://imgkr.cn-bj.ufileos.com/b81930d6-e067-4086-ad1d-df69d9ff1623.png)

![ ](https://imgkr.cn-bj.ufileos.com/7acd35ed-b634-4ab8-a919-3a43a0f43f87.png)

![ ](https://imgkr.cn-bj.ufileos.com/d0f48272-ab36-45d9-a093-f14a5bd2d7d1.png)

<br />

## 文档

- https://github.com/hey-hoho/ScheduleMasterCore/wiki

<br />

## 交流学习

QQ群：824535095

<br />

## 使用情况

- https://github.com/hey-hoho/ScheduleMasterCore/issues/24

<br />

## 赞赏

金额请随意，赞赏请备注~

![ ](https://imgkr.cn-bj.ufileos.com/a755bba0-0601-4a2b-9078-2466016faaa3.png)
![ ](https://imgkr.cn-bj.ufileos.com/cdb08098-5fd5-4518-aacb-bf5c310a07cc.png)
