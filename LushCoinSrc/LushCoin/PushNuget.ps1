del *.nupkg
C:\Windows\Microsoft.NET\Framework\v4.0.30319\msbuild.exe "..\LushCoin\LushCoin.csproj" -p:Configuration=Release
cd ..\LushCoin.NETCore
dotnet restore
dotnet build -c Release
cd ..\LushCoin
C:\Windows\Microsoft.NET\Framework\v4.0.30319\msbuild.exe "..\Build\Deploy.csproj"

.\GitLink.exe ".." -ignore "LushCoin.portable.tests,common,LushCoin.tests,build"

nuGet pack LushCoin.nuspec

forfiles /m *.nupkg /c "cmd /c NuGet.exe push @FILE -source https://api.nuget.org/v3/index.json"
(((dir *.nupkg).Name) -match "[0-9]+?\.[0-9]+?\.[0-9]+?\.[0-9]+")
$ver = $Matches.Item(0)
git tag -a "v$ver" -m "$ver"
git push --tags