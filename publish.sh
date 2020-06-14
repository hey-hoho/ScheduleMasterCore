#!/bin/bash

MASTERDIR="src/ScheduleMasterCore/Hos.ScheduleMaster.Web//Hos.ScheduleMaster.Web.csproj"

dotnet restore "${MASTERDIR}"
dotnet build "${MASTERDIR}"
dotnet publish "${MASTERDIR}" -c Release -o /home/sm-publish/master

WORKERDIR="src/ScheduleMasterCore/Hos.ScheduleMaster.QuartzHost//Hos.ScheduleMaster.QuartzHost.csproj"

dotnet restore "${WORKERDIR}"
dotnet build "${WORKERDIR}"
dotnet publish "${WORKERDIR}" -c Release -o /home/sm-publish/worker1


echo "published to [/home/sm-publish]."
