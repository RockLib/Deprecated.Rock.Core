nuget restore -SolutionDirectory ../  ../Rock.Core\Compression/RockLib.Compression.csproj

msbuild /p:Configuration=Release /t:Clean ..\Rock.Core\Compression\RockLib.Compression.csproj

msbuild /p:Configuration=Release /t:Rebuild ..\Rock.Core\Compression\RockLib.Compression.csproj

msbuild /t:pack /p:PackageOutputPath=..\..\builtPackages  /p:Configuration=Release ..\Rock.Core\Compression\RockLib.Compression.csproj
