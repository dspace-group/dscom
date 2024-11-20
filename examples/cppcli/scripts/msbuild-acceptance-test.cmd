@ECHO off

REM Run this script from VsDevCmd

SET root=%~dp0\..\..\..\

PUSHD %root%
dotnet build-server shutdown

dotnet pack src\dscom.build\dscom.build.csproj -p:Configuration=Release

IF NOT EXIST %root%\_packages MKDIR _packages

XCOPY /Y /I /C /F .\src\dscom\bin\x64\Release\*.nupkg _packages\
XCOPY /Y /I /C /F .\src\dscom.build\bin\x64\Release\*.nupkg _packages\
XCOPY /Y /I /C /F .\src\dscom\bin\Release\*.nupkg _packages\
XCOPY /Y /I /C /F .\src\dscom.build\bin\Release\*.nupkg _packages\

dotnet build-server shutdown

dotnet nuget locals global-packages --clear

POPD

PUSHD %~dp0\..\CsLibrary

msbuild -nodeReuse:False -t:Clean -p:Configuration=Release -p:PerformAcceptanceTest=Runtime -p:SkipResolvePackageAssets=true

dotnet build-server shutdown

dotnet add CsLibrary.csproj package --no-restore --prerelease -s %root%\_packages dSPACE.Runtime.InteropServices.BuildTasks

dotnet build-server shutdown

msbuild -nodeReuse:False -t:Restore -p:Configuration=Release -p:Platform=x86 -p:TargetPlatform=net8.0-windows -p:PerformAcceptanceTest=Runtime

dotnet build-server shutdown

msbuild -nodeReuse:False -t:Build -p:Configuration=Release -p:Platform=x86 -p:TargetPlatform=net8.0-windows -p:PerformAcceptanceTest=Runtime -bl:%~dp0\net80x86.binlog
SET ERRUNTIMEX86_NET80=%ERRORLEVEL%

msbuild -nodeReuse:False -t:Clean -p:Configuration=Release -p:Platform=x86 -p:TargetPlatform=net8.0-windows -p:PerformAcceptanceTest=Runtime ..\CppLibrary\CppLibrary.vcxproj

dotnet build-server shutdown

dotnet remove CsLibrary.csproj package dSPACE.Runtime.InteropServices.BuildTasks

POPD

SetLocal EnableDelayedExpansion

SET EXITCODE=0

IF NOT "%ERRUNTIMEX86_NET80%" == "0" (
  SET EXITCODE=1
  ECHO "::warning::Runtime specific acceptance test for platform x86 using .NET 8.0 failed."
)

IF NOT EXIST %~dp0\..\CsLibrary\bin\x86\Release\net8.0-windows\CsLibrary.tlb (
  ::SET EXITCODE=1
  ECHO "::warning::Could not find exported TLB file for .NET 8 (x86)"
)

IF "%EXITCODE%" == "0" (
  ECHO "Acceptance test completed successfully."
)

EXIT /B %EXITCODE%
