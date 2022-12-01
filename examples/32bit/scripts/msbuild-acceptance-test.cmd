@ECHO off

SET root=%~dp0\..\..\..\

PUSHD %root%

dotnet clean
dotnet restore
dotnet msbuild -nodeReuse:False -t:Build -p:Configuration=Release
dotnet msbuild -nodeReuse:False -t:Pack -p:Configuration=Release

IF NOT EXIST %root%\_packages MKDIR _packages

XCOPY /Y /I /C /F .\src\dscom\bin\Release\*.nupkg _packages\
XCOPY /Y /I /C /F .\src\dscom.build\bin\Release\*.nupkg _packages\

POPD

PUSHD %~dp0\..\comtestdotnet

dotnet msbuild -nodeReuse:False -t:Clean -p:Configuration=Release -p:PerformAcceptanceTest=Runtime

dotnet add comtestdotnet.csproj package --prerelease -s %root%\_packages dSPACE.Runtime.InteropServices.BuildTasks

dotnet msbuild -nodeReuse:False -t:Restore -p:Configuration=Release -p:Platform=x64 -p:TargetPlatform=net48 -p:PerformAcceptanceTest=Runtime
dotnet msbuild -nodeReuse:False -t:Build -p:Configuration=Release -p:Platform=x64 -p:TargetPlatform=net48 -p:PerformAcceptanceTest=Runtime -bl
SET ERRUNTIMEX64_NET48=%ERRORLEVEL%

dotnet msbuild -nodeReuse:False -t:Restore -p:Configuration=Release -p:Platform=x64 -p:TargetPlatform=net6.0-windows -p:PerformAcceptanceTest=Runtime
dotnet msbuild -nodeReuse:False -t:Build -p:Configuration=Release -p:Platform=x64 -p:TargetPlatform=net6.0-windows -p:PerformAcceptanceTest=Runtime -bl
SET ERRUNTIMEX64_NET60=%ERRORLEVEL%

dotnet msbuild -nodeReuse:False -t:Restore -p:Configuration=Release -p:Platform=x86 -p:TargetPlatform=net48 -p:PerformAcceptanceTest=Runtime
dotnet msbuild -nodeReuse:False -t:Build -p:Configuration=Release -p:Platform=x86 -p:TargetPlatform=net48 -p:PerformAcceptanceTest=Runtime -bl
SET ERRUNTIMEX86_NET48=%ERRORLEVEL%

dotnet msbuild -nodeReuse:False -t:Restore -p:Configuration=Release -p:Platform=x86 -p:TargetPlatform=net6.0-windows -p:PerformAcceptanceTest=Runtime
dotnet msbuild -nodeReuse:False -t:Build -p:Configuration=Release -p:Platform=x86 -p:TargetPlatform=net6.0-windows -p:PerformAcceptanceTest=Runtime -bl
SET ERRUNTIMEX86_NET60=%ERRORLEVEL%

dotnet remove comtestdotnet.csproj package dSPACE.Runtime.InteropServices.BuildTasks

POPD

SetLocal EnableDelayedExpansion

SET EXITCODE=0

IF NOT "%ERRUNTIMEX64_NET48%" == "0" (
  SET EXITCODE=1
  ECHO "Runtime specific acceptance test for platform x64 using .NET FullFramework 4.8 failed."
)

IF NOT "%ERRUNTIMEX64_NET60%" == "0" (
  SET EXITCODE=1
  ECHO "Runtime specific acceptance test for platform x64 using .NET 6.0 failed."
)

IF NOT "%ERRUNTIMEX86_NET48%" == "0" (
  SET EXITCODE=1
  ECHO "Runtime specific acceptance test for platform x86 using .NET FullFramework 4.8 failed."
)

IF NOT "%ERRUNTIMEX86_NET60%" == "0" (
  SET EXITCODE=1
  ECHO "Runtime specific acceptance test for platform x64 using .NET 6.0 failed."
)

IF "%EXITCODE%" == "0" (
  ECHO "Acceptance test completed successfully."
)

EXIT /B %EXITCODE%
