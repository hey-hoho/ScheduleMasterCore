$masterdir="src/ScheduleMasterCore/Hos.ScheduleMaster.Web/Hos.ScheduleMaster.Web.csproj"
$workerdir="src/ScheduleMasterCore/Hos.ScheduleMaster.QuartzHost/Hos.ScheduleMaster.QuartzHost.csproj"

dotnet restore "$masterdir"
dotnet build "$masterdir"
dotnet publish "$masterdir" -c Release -o d://sm-publish/master

dotnet restore "$workerdir"
dotnet build "$workerdir"
dotnet publish "$workerdir" -c Release -o d://sm-publish/worker1

echo "published to [d://sm-publish]."

Exit