<?xml version="1.0" encoding="utf-8"?>
<Project xmlns="http://schemas.microsoft.com/developer/msbuild/2003">
    <PropertyGroup>
        <WorkingFolder>$(MSBuildProjectDirectory)</WorkingFolder>
        <SolutionFile>SharpRaven.sln</SolutionFile>
    </PropertyGroup>

    <Target Name="Build" DependsOnTargets="CompileNET45;CompileNET40;CompileNET35" />

    <Target Name="CompileNET45">
        <Message Text="=== COMPILING Release 4.5 configuration ===" />
        <MSBuild Projects="$(SolutionFile)"
                 Properties="Configuration=Release 4.5" />
    </Target>

    <Target Name="CompileNET40">
        <Message Text="=== COMPILING Release 4.0 configuration ===" />
        <MSBuild Projects="$(SolutionFile)"
                 Properties="Configuration=Release 4.0" />
    </Target>

    <Target Name="CompileNET35">
        <Message Text="=== COMPILING Release 3.5 configuration ===" />
        <MSBuild Projects="$(SolutionFile)"
                 Properties="Configuration=Release 3.5" />
    </Target>
</Project>