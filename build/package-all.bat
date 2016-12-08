@echo off

cls

rd /s /q ..\src\ByrneLabs.Commons.MetadataDom\bin 1>nul
rd /s /q ..\src\ByrneLabs.Commons.MetadataDom\obj 1>nul

rem call:core netcoreapp1.0
rem call:core netcoreapp1.1
rem call:core netstandard1.3
rem call:core netstandard1.4
rem call:core netstandard1.5
call:core netstandard1.6

rem call:classic net45
rem call:classic net451
rem call:classic net452
rem call:classic net46
rem call:classic net461
rem call:classic net462

rem call:classic monoandroid10
rem call:classic xamarinios10

nuget pack -BasePath ..\src\ByrneLabs.Commons.MetadataDom\bin\release -OutputDirectory ..\src\ByrneLabs.Commons.MetadataDom\bin\release -Symbols

goto:eof


:core
dotnet restore ../src/ByrneLabs.Commons.MetadataDom/ByrneLabs.Commons.MetadataDom.csproj /property:BuildFrameworkTarget=%~1 /verbosity:minimal /maxcpucount
msbuild ../src/ByrneLabs.Commons.MetadataDom/ByrneLabs.Commons.MetadataDom.csproj /property:BuildFrameworkTarget=%~1 /property:Configuration=Release /verbosity:minimal /maxcpucount
goto:eof


:classic
nuget restore packages.config -PackagesDirectory ..\packages
msbuild ../src/ByrneLabs.Commons.MetadataDom/ByrneLabs.Commons.MetadataDom.csproj /property:BuildFrameworkTarget=%~1 /property:Configuration=Release /verbosity:minimal /maxcpucount
goto:eof

