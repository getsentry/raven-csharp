test:
	mono --runtime=v4.0.30319 ".nuget/NuGet.exe" Restore "src"
	xbuild "./src/SharpRaven.build"
	mono --runtime=v4.0.30319 ./src/packages/NUnit.Runners.2.6.3/tools/nunit-console.exe ./src/tests/SharpRaven.UnitTests/bin/Release/net45/SharpRaven.UnitTests.dll -exclude=NuGet,NoMono -nodots