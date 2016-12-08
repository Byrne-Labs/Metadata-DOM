@echo off

cls

rd /s /q ..\packages 1>nul
rd /s /q ..\src\ByrneLabs.Commons.MetadataDom\bin 1>nul
rd /s /q ..\src\ByrneLabs.Commons.MetadataDom\obj 1>nul

call:core netcoreapp1.0
call:core netcoreapp1.1
call:core netstandard1.3
call:core netstandard1.4
call:core netstandard1.5
call:core netstandard1.6

call:classic net45
call:classic net451
call:classic net452
call:classic net46
call:classic net461
call:classic net462

call:classic monoandroid10
call:classic xamarinios10

del /f /s /q ..\src\ByrneLabs.Commons.MetadataDom\bin\release\ByrneLabs.Commons.MetadataDom.xml

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

