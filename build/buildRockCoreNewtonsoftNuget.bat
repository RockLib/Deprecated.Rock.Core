msbuild /p:Configuration=Release ..\Rock.Core.Newtonsoft\Rock.Core.Newtonsoft.csproj
nuget pack ..\Rock.Core.Newtonsoft\Rock.Core.Newtonsoft.csproj -Properties Configuration=Release