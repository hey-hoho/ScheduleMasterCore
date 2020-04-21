
# 添加基础镜像
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1

#容器中系统的工作空间
WORKDIR /app

#拷贝当前文件夹下的文件到容器中系统的工作空间
COPY . /app
 
#设置Docker容器对外暴露的端口
EXPOSE 30000

#设置默认时区
ENV TZ="Asia/Shanghai"

#容器中使用 ["dotnet","系统启动的dll"] 来运行应用程序
ENTRYPOINT ["dotnet", "Hos.ScheduleMaster.Web.dll"]