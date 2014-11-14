test:
	mono --runtime=v4.0.30319 ".nuget/NuGet.exe" Restore "src"
	xbuild "./src/SharpRaven.build"