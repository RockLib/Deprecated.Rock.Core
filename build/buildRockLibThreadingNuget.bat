nuget restore -SolutionDirectory ../  ../Rock.Core\Threading/RockLib.Threading.csproj

msbuild /p:Configuration=Release /t:Clean ..\Rock.Core\Threading\RockLib.Threading.csproj

msbuild /p:Configuration=Release /t:Rebuild ..\Rock.Core\Threading\RockLib.Threading.csproj

msbuild /t:pack /p:PackageOutputPath=..\..\builtPackages  /p:Configuration=Release ..\Rock.Core\Threading\RockLib.Threading.csproj
