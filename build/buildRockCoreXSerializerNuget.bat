nuget restore -SolutionDirectory ../  ../Rock.Core.XSerializer/Rock.Core.XSerializer.csproj

msbuild /p:Configuration=Release /t:Clean;Rebuildmsbuild /p:Configuration=Release ..\Rock.Core.XSerializer\Rock.Core.XSerializer.csproj

nuget pack ..\Rock.Core.XSerializer\Rock.Core.XSerializer.csproj -Properties Configuration=Release