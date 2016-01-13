build: setup-nuget restore

setup-nuget:
	mkdir -p .nuget
	wget -O .nuget/nuget.exe https://dist.nuget.org/win-x86-commandline/latest/nuget.exe

restore:
	mono --runtime=v4.0.30319 ".nuget/NuGet.exe" Restore "src"

test: restore
	xbuild "./src/SharpRaven.build"
	mono --runtime=v4.0.30319 ./src/packages/NUnit.Runners.2.6.4/tools/nunit-console.exe ./src/tests/SharpRaven.UnitTests/bin/Release/net45/SharpRaven.UnitTests.dll -exclude=NuGet,NoMono -nodots
