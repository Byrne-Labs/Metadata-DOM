@echo off

cls

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


nuget pack -BasePath ..\src\ByrneLabs.Commons.MetadataDom\bin\release -OutputDirectory ..\src\ByrneLabs.Commons.MetadataDom\bin\release -Symbols

goto:eof


:core
dotnet restore ../ByrneLabs.Commons.MetadataDom.sln /property:BuildFrameworkTarget=%~1
msbuild ../ByrneLabs.Commons.MetadataDom.sln /property:BuildFrameworkTarget=%~1 /property:Configuration=Release
goto:eof


:classic
nuget restore packages.config -PackagesDirectory ..\packages
msbuild ../ByrneLabs.Commons.MetadataDom.sln /property:BuildFrameworkTarget=%~1 /property:Configuration=Release
goto:eof

