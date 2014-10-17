msbuild /p:Configuration=Release ..\Rock.Core.Netwonsoft\Rock.Core.Netwonsoft.csproj
nuget pack ..\Rock.Core.Netwonsoft\Rock.Core.Netwonsoft.csproj -Properties Configuration=Release