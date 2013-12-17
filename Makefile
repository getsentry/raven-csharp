test:
	mono ".nuget/NuGet.exe" Restore "src"
	xbuild "./src/SharpRaven.sln"