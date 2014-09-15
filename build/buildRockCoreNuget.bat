msbuild /p:Configuration=Release ..\Rock.Core\Rock.Core.csproj
nuget pack ..\Rock.Core\Rock.Core.csproj -Properties Configuration=Release