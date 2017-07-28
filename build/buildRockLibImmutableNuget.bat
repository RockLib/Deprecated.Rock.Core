nuget restore -SolutionDirectory ../  ../Rock.Core\Immutable/RockLib.Immutable.csproj

msbuild /p:Configuration=Release /t:Clean ..\Rock.Core\Immutable\RockLib.Immutable.csproj

msbuild /p:Configuration=Release /t:Rebuild ..\Rock.Core\Immutable\RockLib.Immutable.csproj

msbuild /t:pack /p:PackageOutputPath=..\..\builtPackages  /p:Configuration=Release ..\Rock.Core\Immutable\RockLib.Immutable.csproj
