@echo off
SETLOCAL
SET PATH=%PATH%;%WINDIR%\Microsoft.NET\Framework\v4.0.30319;%CD%\..\.nuget;
echo.
echo.
echo =====================
echo === NuGet Restore ===
echo =====================
echo.
NuGet restore SharpRaven.sln
echo.
echo.
echo =====================
echo ===   GitVersion  ===
echo =====================
echo.
packages\GitVersion.CommandLine.1.3.3\Tools\GitVersion.exe /output json /updateassemblyinfo false /l gitversion.log
echo.
echo.
echo.
echo.
echo =====================
echo ===    MSBuild    ===
echo =====================
echo.
MSBUILD SharpRaven.build
echo.
echo =====================
echo ===  NuGet Pack   ===
echo =====================
echo.
NuGet pack app\SharpRaven\SharpRaven.csproj -Properties ReleaseNotes='Test'
NuGet pack app\SharpRaven.Nancy\SharpRaven.Nancy.csproj -Properties ReleaseNotes='Test'
echo.
ENDLOCAL