nuget restore -SolutionDirectory ../  ../Rock.Core.Newtonsoft/Rock.Core.Newtonsoft.csproj

msbuild /p:Configuration=Release /t:Clean;Rebuildmsbuild /p:Configuration=Release ..\Rock.Core.Newtonsoft\Rock.Core.Newtonsoft.csproj

nuget pack ..\Rock.Core.Newtonsoft\Rock.Core.Newtonsoft.csproj -Properties Configuration=Release