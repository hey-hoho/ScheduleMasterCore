
![schedulemaster ](https://raw.githubusercontent.com/hey-hoho/ScheduleMasterCore/master/docs/images/logo.png)

ScheduleMaster是一个开源的分布式任务调度系统，它基于.Net Core 3.0平台构建，支持跨平台多节点部署运行。
[![Build Status](https://dev.azure.com/591310381/ScheduleMasterCore/_apis/build/status/hey-hoho.ScheduleMasterCore?branchName=master)](https://dev.azure.com/591310381/ScheduleMasterCore/_build/latest?definitionId=4&branchName=master)


## 主要特性
- [x] 简易的Web UI操作；
- [x] 任务动态管理:创建、启动、停止、暂停、恢复、删除等；
- [x] 高可用支持，跨平台多节点部署。
- [x] 支持自定义参数设置；
- [x] 支持设置监护人，运行异常时邮件告警；
- [x] 支持设置任务依赖，共享任务结果；
- [x] 插件式开发，任务运行环境隔离；
- [x] 全链路日志系统，运行轨迹轻松掌控；
- [x] 用户访问控制；
- [x] 提供开放REST API，业务系统可以无缝集成；
- [x] 调度报表统计；
- [ ] 任务分组管理；
- [ ] 指定节点运行；
- [ ] 支持http任务配置；
- [ ] 支持延时任务；
- [ ] 支持异常策略配置（失败重试、超时控制等）；
- [ ] 接入redis缓存；
- [ ] 用户权限更加精细化；
- [ ] 报表统计完善；

## 技术栈
Asp.Net Core3.0、EntityFramework Core3.0、Mysql5.7、Quartz.Net、BeyondAdmin、Jquery...

## 系统架构图
![](https://raw.githubusercontent.com/hey-hoho/ScheduleMasterCore/master/docs/images/architecture.png "Architecture")

## 如何使用

> 使用前请准备好所需环境：`Visual Studio 2019`、`.Net Core3.0 SDK`、`Mysql 5.7`、`Centos(可选)`、`Docker(可选)`。

下载源码到本地，然后用VS2019打开解决方案并编译通过。

打开项目Hos.ScheduleMaster.Web根目录下的`appsettings.json`文件，先修改Mysql数据库连接字符串以保证数据库正常访问，再找到`NodeSetting`节点，修改`IP`字段为master将要部署的ip地址（master端口为30000不用修改），在项目上右击选择发布...，发布到本地文件夹。

打开项目Hos.ScheduleMaster.QuartzHost根目录下的`appsettings.json`文件，同样先修改Mysql连接字符串，再找到`NodeSetting`节点，设置worker的名称`IdentityName`，修改`IP`字段为将要部署的ip地址，`Port`字段为要监听的地址（推荐为30001），在项目上右击选择发布...，发布到本地文件夹。如果要新增worker，按同样方式配置`IdentityName、IP、Port`即可，worker在启动后会把自己的信息注入到数据库中，在master中可以看到。

其他发布方式亦可。下面以运行2个worker节点为例：

#### 在Windows中运行
* 找到master的发布目录，执行命令`dotnet Hos.ScheduleMaster.Web.dll`启动程序，首次启动会自动迁移生成数据库结构并初始化种子数据，打开浏览器输入ip和端口访问即可。
* 找到worker的发布目录，执行命令`dotnet Hos.ScheduleMaster.QuartzHost.dll --urls http://*:30001`启动程序，打开浏览器输入ip和端口会看到一个欢迎页面，表示worker已启动成功。
* 修改worker下的`appsettings.json`文件为worker2的配置（如果发布前已经修改可跳过），执行命令`dotnet Hos.ScheduleMaster.QuartzHost.dll --urls http://*:30002`启动程序.
* 登录到master中，可以看到**节点管理**菜单下各节点的运行状态。

#### 在Linux(Centos)中运行
> 运行前请确保机器已经安装好`.Net Core3.0`运行时环境。

把发布文件复制到Centos中，操作步骤同Windows。

#### 在Docker中运行
* 在master的发布目录中执行`docker build -t ms_master .`命令生成master镜像，再执行`docker run -d -p 30000:30000 --name="mymaster" ms_master`运行容器。
* 在worker的发布目录中执行`docker build -t ms_worker .`命令生成worker镜像，再执行`docker run -d -p 30001:30001 --name="myworker1" ms_worker`运行容器启动worker1，在执行`docker run -d -p 30002:30001 --name="myworker2" ms_worker`运行容器启动worker2。
* 执行`docker ps`查看各容器运行状态。


