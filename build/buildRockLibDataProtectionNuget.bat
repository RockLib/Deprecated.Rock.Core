nuget restore -SolutionDirectory ../  ../Rock.Core\DataProtection/RockLib.DataProtection.csproj

msbuild /p:Configuration=Release /t:Clean ..\Rock.Core\DataProtection\RockLib.DataProtection.csproj

msbuild /p:Configuration=Release /t:Rebuild ..\Rock.Core\DataProtection\RockLib.DataProtection.csproj

msbuild /t:pack /p:PackageOutputPath=..\..\builtPackages  /p:Configuration=Release ..\Rock.Core\DataProtection\RockLib.DataProtection.csproj
