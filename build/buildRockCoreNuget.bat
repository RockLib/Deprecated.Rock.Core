nuget restore -SolutionDirectory ../  ../Rock.Core/Rock.Core.csproj

msbuild /p:Configuration=Release /t:Clean;Rebuildmsbuild /p:Configuration=Release ..\Rock.Core\Rock.Core.csproj

nuget pack ..\Rock.Core\Rock.Core.csproj -Properties Configuration=Release