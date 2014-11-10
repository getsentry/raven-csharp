@echo off
SETLOCAL
SET PATH=%PATH%;%WINDIR%\Microsoft.NET\Framework\v4.0.30319;%CD%\..\.nuget;
echo.
echo.
echo ==================
echo === GitVersion ===
echo ==================
echo.
packages\GitVersion.CommandLine.1.3.3\Tools\GitVersion.exe /output json /updateassemblyinfo true /l gitversion.log
echo.
echo.
echo.
echo.
echo ==================
echo ===  MSBuild   ===
echo ==================
echo.
MSBUILD SharpRaven.build
echo.
echo ==================
echo ===   NuGet    ===
echo ==================
echo.
NuGet pack app\SharpRaven\SharpRaven.nuspec -Properties ReleaseInfo='Test'
echo.
ENDLOCAL