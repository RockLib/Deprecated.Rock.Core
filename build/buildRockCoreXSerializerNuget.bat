msbuild /p:Configuration=Release ..\Rock.Core.XSerializer\Rock.Core.XSerializer.csproj
nuget pack ..\Rock.Core.XSerializer\Rock.Core.XSerializer.csproj -Properties Configuration=Release