test:
	mono ".nuget/NuGet.exe" Install "src/tests/SharpRaven.UnitTests/packages.config" -o "src/packages"
	xbuild "src/SharpRaven.sln"