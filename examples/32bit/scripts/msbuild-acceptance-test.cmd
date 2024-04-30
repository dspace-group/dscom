@ECHO off

SET root=%~dp0\..\..\..\

PUSHD %root%

dotnet build-server shutdown

dotnet pack src\dscom.build\dscom.build.csproj -p:Configuration=Release

IF NOT EXIST %root%\_packages MKDIR _packages

XCOPY /Y /I /C /F .\src\dscom\bin\Release\*.nupkg _packages\
XCOPY /Y /I /C /F .\src\dscom.build\bin\Release\*.nupkg _packages\

dotnet build-server shutdown

dotnet nuget locals global-packages --clear

POPD

PUSHD %~dp0\..\comtestdotnet

dotnet msbuild -nodeReuse:False -t:Clean -p:Configuration=Release -p:PerformAcceptanceTest=Runtime -p:SkipResolvePackageAssets=true

dotnet build-server shutdown

dotnet msbuild -nodeReuse:False -t:Clean -p:Configuration=Release -p:PerformAcceptanceTest=Runtime -p:SkipResolvePackageAssets=true

dotnet build-server shutdown

dotnet add comtestdotnet.csproj package --prerelease -s %root%\_packages dSPACE.Runtime.InteropServices.BuildTasks

dotnet msbuild -nodeReuse:False -t:Restore -p:Configuration=Release -p:Platform=x64 -p:TargetPlatform=net48 -p:PerformAcceptanceTest=Runtime

dotnet build-server shutdown

dotnet msbuild -nodeReuse:False -t:Build -p:Configuration=Release -p:Platform=x64 -p:TargetPlatform=net48 -p:PerformAcceptanceTest=Runtime -bl:%~dp0\net48x64.binlog

SET ERRUNTIMEX64_NET48=%ERRORLEVEL%

dotnet build-server shutdown

dotnet msbuild -nodeReuse:False -t:Restore -p:Configuration=Release -p:Platform=x64 -p:TargetPlatform=net6.0-windows -p:PerformAcceptanceTest=Runtime

dotnet build-server shutdown

dotnet msbuild -nodeReuse:False -t:Build -p:Configuration=Release -p:Platform=x64 -p:TargetPlatform=net6.0-windows -p:PerformAcceptanceTest=Runtime -bl:%~dp0\net60x64.binlog

SET ERRUNTIMEX64_NET60=%ERRORLEVEL%

dotnet build-server shutdown

dotnet msbuild -nodeReuse:False -t:Restore -p:Configuration=Release -p:Platform=x64 -p:TargetPlatform=net8.0-windows -p:PerformAcceptanceTest=Runtime

dotnet build-server shutdown

dotnet msbuild -nodeReuse:False -t:Build -p:Configuration=Release -p:Platform=x64 -p:TargetPlatform=net8.0-windows -p:PerformAcceptanceTest=Runtime -bl:%~dp0\net80x64.binlog

SET ERRUNTIMEX64_NET80=%ERRORLEVEL%

dotnet build-server shutdown

dotnet msbuild -nodeReuse:False -t:Restore -p:Configuration=Release -p:Platform=x86 -p:TargetPlatform=net48 -p:PerformAcceptanceTest=Runtime

dotnet build-server shutdown

dotnet msbuild -nodeReuse:False -t:Build -p:Configuration=Release -p:Platform=x86 -p:TargetPlatform=net48 -p:PerformAcceptanceTest=Runtime -bl:%~dp0\net48x86.binlog
SET ERRUNTIMEX86_NET48=%ERRORLEVEL%

dotnet build-server shutdown

dotnet msbuild -nodeReuse:False -t:Restore -p:Configuration=Release -p:Platform=x86 -p:TargetPlatform=net6.0-windows -p:PerformAcceptanceTest=Runtime

dotnet build-server shutdown

dotnet msbuild -nodeReuse:False -t:Build -p:Configuration=Release -p:Platform=x86 -p:TargetPlatform=net6.0-windows -p:PerformAcceptanceTest=Runtime -bl:%~dp0\net60x86.binlog
SET ERRUNTIMEX86_NET60=%ERRORLEVEL%

dotnet build-server shutdown

dotnet msbuild -nodeReuse:False -t:Restore -p:Configuration=Release -p:Platform=x86 -p:TargetPlatform=net8.0-windows -p:PerformAcceptanceTest=Runtime

dotnet build-server shutdown

dotnet msbuild -nodeReuse:False -t:Build -p:Configuration=Release -p:Platform=x86 -p:TargetPlatform=net8.0-windows -p:PerformAcceptanceTest=Runtime -bl:%~dp0\net80x86.binlog
SET ERRUNTIMEX86_NET80=%ERRORLEVEL%

dotnet build-server shutdown

dotnet remove comtestdotnet.csproj package dSPACE.Runtime.InteropServices.BuildTasks

POPD

SetLocal EnableDelayedExpansion

SET EXITCODE=0

IF NOT "%ERRUNTIMEX64_NET48%" == "0" (
  SET EXITCODE=1
  ECHO "::warning file=%~dp0\..\comtestdotnet\comtestdotnet.csproj::Runtime specific acceptance test for platform x64 using .NET FullFramework 4.8 failed."
)

IF NOT "%ERRUNTIMEX64_NET60%" == "0" (
  SET EXITCODE=1
  ECHO "::warning file=%~dp0\..\comtestdotnet\comtestdotnet.csproj::Runtime specific acceptance test for platform x64 using .NET 6.0 failed."
)

IF NOT "%ERRUNTIMEX64_NET80%" == "0" (
  SET EXITCODE=1
  ECHO "::warning file=%~dp0\..\comtestdotnet\comtestdotnet.csproj::Runtime specific acceptance test for platform x64 using .NET 8.0 failed."
)

IF NOT "%ERRUNTIMEX86_NET48%" == "0" (
  ::SET EXITCODE=1
  ECHO "::warning file=%~dp0\..\comtestdotnet\comtestdotnet.csproj::Runtime specific acceptance test for platform x86 using .NET FullFramework 4.8 failed."
)

IF NOT "%ERRUNTIMEX86_NET80%" == "0" (
  ::SET EXITCODE=1
  ECHO "::warning file=%~dp0\..\comtestdotnet\comtestdotnet.csproj::Runtime specific acceptance test for platform x86 using .NET 8.0 failed."
)

IF NOT "%ERRUNTIMEX86_NET60%" == "0" (
  ::SET EXITCODE=1
  ECHO "::warning file=%~dp0\..\comtestdotnet\bin\x64\Release\net6.0\comtestdotnet.tlb::Runtime specific acceptance test for platform x64 using .NET 6.0 failed."
)

IF NOT EXIST %~dp0\..\comtestdotnet\bin\x64\Release\net6.0\comtestdotnet.tlb (
  SET EXITCODE=1
  ECHO "::warning file=%~dp0\..\comtestdotnet\bin\x64\Release\net6.0\comtestdotnet.tlb::Could not find exported TLB file for .NET 6 (x64)"
)

IF NOT EXIST %~dp0\..\comtestdotnet\bin\x86\Release\net6.0\comtestdotnet.tlb (
  ::SET EXITCODE=1
  ECHO "::warning file=%~dp0\..\comtestdotnet\bin\x86\Release\net6.0\comtestdotnet.tlb::Could not find exported TLB file for .NET 6 (x86)"
)

IF NOT EXIST %~dp0\..\comtestdotnet\bin\x64\Release\net48\comtestdotnet.tlb (
  SET EXITCODE=1
  ECHO "::warning file=%~dp0\..\comtestdotnet\bin\x64\Release\net48\comtestdotnet.tlb::Could not find exported TLB file for .NET 4.8 (x64)"
)

IF NOT EXIST %~dp0\..\comtestdotnet\bin\x86\Release\net48\comtestdotnet.tlb (
  ::SET EXITCODE=1
  ECHO "::warning file=%~dp0\..\comtestdotnet\bin\x86\Release\net48\comtestdotnet.tlb::Could not find exported TLB file for .NET 4.8 (x86)"
)

IF NOT EXIST %~dp0\..\comtestdotnet\bin\x64\Release\net8.0\comtestdotnet.tlb (
  SET EXITCODE=1
  ECHO "::warning file=%~dp0\..\comtestdotnet\bin\x64\Release\net8.0\comtestdotnet.tlb::Could not find exported TLB file for .NET 8 (x64)"
)

IF NOT EXIST %~dp0\..\comtestdotnet\bin\x86\Release\net8.0\comtestdotnet.tlb (
  ::SET EXITCODE=1
  ECHO "::warning file=%~dp0\..\comtestdotnet\bin\x86\Release\net8.0\comtestdotnet.tlb::Could not find exported TLB file for .NET 8 (x86)"
)

IF "%EXITCODE%" == "0" (
  ECHO "Acceptance test completed successfully."
)

EXIT /B %EXITCODE%
