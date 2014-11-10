@echo off
SETLOCAL
SET PATH=%PATH%;%WINDIR%\Microsoft.NET\Framework\v4.0.30319;%CD%\..\.nuget;
packages\GitVersion.CommandLine.1.3.3\Tools\GitVersion.exe /output json /updateassemblyinfo true /l gitversion.log
MSBUILD SharpRaven.build
NuGet pack app\SharpRaven\SharpRaven.nuspec -Properties ReleaseInfo='Test'
ENDLOCAL