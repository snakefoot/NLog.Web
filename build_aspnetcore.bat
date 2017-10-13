echo off

rem update NLog.Web.AspNetCore.csproj for version number

dotnet restore "examples/ASP.NET Core 2/Visual Studio 2017/ASP.NET Core 2 - VS2017/"

cd NLog.Web.AspNetCore
msbuild    /t:restore /t:build /p:configuration=release /verbosity:minimal
cd ..
